
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class Pages_PurchaseHistory : System.Web.UI.Page
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
            LoadPurchaseHistory("");
        }
    }


    private void LoadPurchaseHistory(string search)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn =
               new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    p.PurchaseID,
                    p.PurchaseDate,
                    s.CompanyName,
                    p.InvoiceNumber,
                    COUNT(d.PurchaseDetailID) AS ItemCount,
                    p.TotalAmount
                FROM tbl_Purchases p

                INNER JOIN tbl_Suppliers s
                    ON p.SupplierID = s.SupplierID

                LEFT JOIN tbl_PurchaseDetails d
                    ON p.PurchaseID = d.PurchaseID

                WHERE
                    @Search = ''
                    OR p.PurchaseID LIKE '%' + @Search + '%'
                    OR ISNULL(p.InvoiceNumber, '') LIKE '%' + @Search + '%'
                    OR s.CompanyName LIKE '%' + @Search + '%'

                GROUP BY
                    p.PurchaseID,
                    p.PurchaseDate,
                    s.CompanyName,
                    p.InvoiceNumber,
                    p.TotalAmount

                ORDER BY p.PurchaseID DESC";


            using (SqlCommand cmd =
                   new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Search",
                    search.Trim()
                );

                using (SqlDataAdapter da =
                       new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    gvPurchaseHistory.DataSource = dt;
                    gvPurchaseHistory.DataBind();
                }
            }
        }
    }


    protected void btnSearch_Click(
        object sender,
        EventArgs e)
    {
        LoadPurchaseHistory(
            txtSearch.Text.Trim()
        );
    }


    protected void btnClear_Click(
        object sender,
        EventArgs e)
    {
        txtSearch.Text = "";

        LoadPurchaseHistory("");
    }


    protected void btnNewPurchase_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect("Purchases.aspx");
    }


    protected void gvPurchaseHistory_RowCommand(
        object sender,
        GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewPurchase")
        {
            int purchaseId =
                Convert.ToInt32(e.CommandArgument);

            Response.Redirect(
                "PurchaseDetails.aspx?PurchaseID=" +
                purchaseId
            );
        }
    }
}
