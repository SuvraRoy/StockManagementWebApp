using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_StockTransferHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadTransfers();

            string saved = Request.QueryString["saved"];

            if (!string.IsNullOrEmpty(saved))
            {
                ShowMessage(
                    "Stock transfer saved successfully. Transfer: "
                    + Server.HtmlEncode(saved),
                    "success");
            }
        }
    }


    // =========================================================
    // LOAD TRANSFERS
    // =========================================================

    private void LoadTransfers()
    {
        string query = @"
            SELECT
                st.TransferID,
                st.TransferNumber,
                st.TransferDate,
                fw.WarehouseName AS FromWarehouse,
                tw.WarehouseName AS ToWarehouse,
                COUNT(std.TransferDetailID) AS ItemCount,
                st.DocumentFilePath
            FROM tbl_StockTransfers st
            INNER JOIN tbl_Warehouses fw
                ON st.FromWarehouseID = fw.WarehouseID
            INNER JOIN tbl_Warehouses tw
                ON st.ToWarehouseID = tw.WarehouseID
            LEFT JOIN tbl_StockTransferDetails std
                ON st.TransferID = std.TransferID
            WHERE
                (
                    @Search = ''
                    OR st.TransferNumber LIKE '%' + @Search + '%'
                    OR fw.WarehouseName LIKE '%' + @Search + '%'
                    OR tw.WarehouseName LIKE '%' + @Search + '%'
                )
                AND
                (
                    @FromDate = ''
                    OR st.TransferDate >= @FromDate
                )
                AND
                (
                    @ToDate = ''
                    OR st.TransferDate < DATEADD(DAY, 1, @ToDate)
                )
            GROUP BY
                st.TransferID,
                st.TransferNumber,
                st.TransferDate,
                fw.WarehouseName,
                tw.WarehouseName,
                st.DocumentFilePath
            ORDER BY
                st.TransferID DESC";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Search", txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@FromDate", txtFromDate.Text.Trim());
                cmd.Parameters.AddWithValue("@ToDate", txtToDate.Text.Trim());

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvTransfers.DataSource = dt;
                    gvTransfers.DataBind();
                }
            }
        }
    }


    // =========================================================
    // SEARCH
    // =========================================================

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        pnlDetails.Visible = false;

        // Reset to first page whenever a new search/filter is run
        gvTransfers.PageIndex = 0;

        LoadTransfers();
    }


    // =========================================================
    // GRID PAGING
    // =========================================================

    protected void gvTransfers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Hide details panel when moving across pages
        pnlDetails.Visible = false;

        gvTransfers.PageIndex = e.NewPageIndex;
        LoadTransfers();
    }


    // =========================================================
    // GRID ACTIONS
    // =========================================================

    protected void gvTransfers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewTransfer")
        {
            int transferID;

            if (int.TryParse(e.CommandArgument.ToString(), out transferID))
            {
                LoadTransferDetails(transferID);
                pnlDetails.Visible = true;
            }
        }
    }


    // =========================================================
    // LOAD DETAILS
    // =========================================================

    private void LoadTransferDetails(int transferID)
    {
        string headerQuery = @"
            SELECT
                st.TransferNumber,
                st.TransferDate,
                fw.WarehouseName AS FromWarehouse,
                tw.WarehouseName AS ToWarehouse,
                st.Remarks
            FROM tbl_StockTransfers st
            INNER JOIN tbl_Warehouses fw
                ON st.FromWarehouseID = fw.WarehouseID
            INNER JOIN tbl_Warehouses tw
                ON st.ToWarehouseID = tw.WarehouseID
            WHERE
                st.TransferID = @TransferID";

        string detailQuery = @"
            SELECT
                p.ProductName,
                p.SKU,
                std.Quantity
            FROM tbl_StockTransferDetails std
            INNER JOIN tbl_Products p
                ON std.ProductID = p.ProductID
            WHERE
                std.TransferID = @TransferID
            ORDER BY
                std.TransferDetailID";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            con.Open();

            // -------------------------------------------------
            // HEADER
            // -------------------------------------------------
            using (SqlCommand cmd = new SqlCommand(headerQuery, con))
            {
                cmd.Parameters.AddWithValue("@TransferID", transferID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblDetailTransferNumber.Text = dr["TransferNumber"].ToString();
                        lblDetailDate.Text = Convert.ToDateTime(dr["TransferDate"]).ToString("dd-MM-yyyy");
                        lblDetailFrom.Text = dr["FromWarehouse"].ToString();
                        lblDetailTo.Text = dr["ToWarehouse"].ToString();
                        lblDetailRemarks.Text = dr["Remarks"] == DBNull.Value ? "-" : dr["Remarks"].ToString();
                    }
                }
            }

            // -------------------------------------------------
            // DETAILS
            // -------------------------------------------------
            using (SqlCommand cmd = new SqlCommand(detailQuery, con))
            {
                cmd.Parameters.AddWithValue("@TransferID", transferID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvTransferDetails.DataSource = dt;
                    gvTransferDetails.DataBind();
                }
            }
        }
    }


    // =========================================================
    // CLOSE
    // =========================================================

    protected void btnClose_Click(object sender, EventArgs e)
    {
        pnlDetails.Visible = false;
    }


    // =========================================================
    // MESSAGE
    // =========================================================

    private void ShowMessage(string message, string type)
    {
        divMessage.Attributes["class"] = "alert alert-" + type;
        lblMessage.Text = message;
    }
}