using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class Pages_SalesList : System.Web.UI.Page
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
            LoadSales();

            string savedInvoice = Request.QueryString["saved"];

            if (!string.IsNullOrEmpty(savedInvoice))
            {
                ShowMessage(
                    "Sale saved successfully. Invoice: " + Server.HtmlEncode(savedInvoice),
                    "success"
                );
            }

            //  Check if redirected from PaymentHistory.aspx
            if (Request.QueryString["SalesID"] != null)
            {
                int salesID;
                if (int.TryParse(Request.QueryString["SalesID"], out salesID))
                {
                    // Replicates the exact behavior of clicking btnView on gvSales
                    LoadSaleDetails(salesID);

                    pnlDetails.Visible = true;
                    pnlPrintInvoice.Visible = false;

                    // Optional: Smoothly scrolls the page down to the details panel
                    ClientScript.RegisterStartupScript(this.GetType(), "ScrollToDetails",
                        "window.onload = function() { var el = document.getElementById('" + pnlDetails.ClientID + "'); if(el) el.scrollIntoView({ behavior: 'smooth' }); };", true);
                }
            }
        }
    }

    // =========================================================
    // LOAD SALES
    // =========================================================

    private void LoadSales()
    {
        string query = @"
            SELECT
                s.SalesID,
                s.InvoiceNumber,
                s.SalesDate,
                c.CustomerName,
                w.WarehouseName,
                COUNT(sd.SalesDetailID) AS ItemCount,
                s.TotalAmount,

                ISNULL(
                    (
                        SELECT SUM(sp.AmountPaid)
                        FROM tbl_SalesPayments sp
                        WHERE sp.SalesID = s.SalesID
                    ), 0
                ) AS AmountPaid,

                s.TotalAmount -
                ISNULL(
                    (
                        SELECT SUM(sp.AmountPaid)
                        FROM tbl_SalesPayments sp
                        WHERE sp.SalesID = s.SalesID
                    ), 0
                ) AS AmountDue

            FROM tbl_Sales s

            INNER JOIN tbl_Customers c
                ON s.CustomerID = c.CustomerID

            INNER JOIN tbl_Warehouses w
                ON s.WarehouseID = w.WarehouseID

            LEFT JOIN tbl_SalesDetails sd
                ON s.SalesID = sd.SalesID

            WHERE
                (
                    @Search = ''
                    OR s.InvoiceNumber LIKE '%' + @Search + '%'
                    OR c.CustomerName LIKE '%' + @Search + '%'
                )

                AND
                (
                    @FromDate = ''
                    OR s.SalesDate >= @FromDate
                )

                AND
                (
                    @ToDate = ''
                    OR s.SalesDate < DATEADD(DAY, 1, @ToDate)
                )

            GROUP BY
                s.SalesID,
                s.InvoiceNumber,
                s.SalesDate,
                c.CustomerName,
                w.WarehouseName,
                s.TotalAmount

            ORDER BY
                s.SalesID DESC";

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

                    // Calculate Payment Status
                    dt.Columns.Add("PaymentStatus", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);
                        decimal amountPaid = Convert.ToDecimal(row["AmountPaid"]);

                        row["PaymentStatus"] = GetPaymentStatus(totalAmount, amountPaid);
                    }

                    gvSales.DataSource = dt;
                    gvSales.DataBind();

                }
            }
        }
    }

    // =========================================================
    // PAYMENT STATUS
    // =========================================================

    private string GetPaymentStatus(decimal totalAmount, decimal amountPaid)
    {
        if (amountPaid <= 0)
        {
            return "UNPAID";
        }

        if (amountPaid >= totalAmount)
        {
            return "PAID";
        }

        return "PARTIALLY PAID";
    }

    // =========================================================
    // PAYMENT STATUS BADGE
    // =========================================================

    protected string GetPaymentStatusClass(string status)
    {
        if (status == "PAID")
        {
            return "badge bg-label-success";
        }

        if (status == "PARTIALLY PAID")
        {
            return "badge bg-label-warning";
        }

        return "badge bg-label-danger";
    }

    // =========================================================
    // SEARCH
    // =========================================================

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        pnlDetails.Visible = false;
        pnlPrintInvoice.Visible = false;

        LoadSales();
    }

    // =========================================================
    // GRID ACTIONS
    // =========================================================

    protected void gvSales_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewSale")
        {
            int salesID;

            if (int.TryParse(e.CommandArgument.ToString(), out salesID))
            {
                LoadSaleDetails(salesID);

                pnlDetails.Visible = true;
                pnlPrintInvoice.Visible = false;
            }
        }

        if (e.CommandName == "PrintSale")
        {
            int salesID;

            if (int.TryParse(e.CommandArgument.ToString(), out salesID))
            {
                LoadPrintInvoice(salesID);

                pnlPrintInvoice.Visible = true;
                pnlDetails.Visible = false;

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "PrintInvoice",
                    "setTimeout(function(){ window.print(); }, 300);",
                    true
                );
            }
        }
    }

    // =========================================================
    // LOAD SALE DETAILS
    // =========================================================

    private void LoadSaleDetails(int salesID)
    {
        string headerQuery = @"
            SELECT
                s.InvoiceNumber,
                s.SalesDate,
                c.CustomerName,
                w.WarehouseName,
                s.TotalAmount
            FROM tbl_Sales s

            INNER JOIN tbl_Customers c
                ON s.CustomerID = c.CustomerID

            INNER JOIN tbl_Warehouses w
                ON s.WarehouseID = w.WarehouseID

            WHERE s.SalesID = @SalesID";

        string detailQuery = @"
            SELECT
                p.ProductName,
                p.SKU,
                sd.Quantity,
                sd.SalePrice,
                sd.TotalAmount
            FROM tbl_SalesDetails sd

            INNER JOIN tbl_Products p
                ON sd.ProductID = p.ProductID

            WHERE sd.SalesID = @SalesID

            ORDER BY sd.SalesDetailID";

        decimal totalAmount = 0;

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            con.Open();

            // HEADER
            using (SqlCommand cmd = new SqlCommand(headerQuery, con))
            {
                cmd.Parameters.AddWithValue("@SalesID", salesID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblDetailInvoice.Text = dr["InvoiceNumber"].ToString();
                        lblDetailDate.Text = Convert.ToDateTime(dr["SalesDate"]).ToString("dd-MM-yyyy");
                        lblDetailCustomer.Text = dr["CustomerName"].ToString();
                        lblDetailWarehouse.Text = dr["WarehouseName"].ToString();
                        totalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                        lblDetailTotal.Text = totalAmount.ToString("N2");
                    }
                }
            }

            // DETAILS
            using (SqlCommand cmd = new SqlCommand(detailQuery, con))
            {
                cmd.Parameters.AddWithValue("@SalesID", salesID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSaleDetails.DataSource = dt;
                    gvSaleDetails.DataBind();
                }
            }

            // PAYMENT SUMMARY - Fixed parameter order
            decimal amountPaid = GetAmountPaid(con, salesID);
            decimal amountDue = totalAmount - amountPaid;

            if (amountDue < 0)
            {
                amountDue = 0;
            }

            lblDetailPaid.Text = amountPaid.ToString("N2");
            lblDetailDue.Text = amountDue.ToString("N2");

            string paymentStatus = GetPaymentStatus(totalAmount, amountPaid);

            lblDetailPaymentStatus.Text = paymentStatus;
            lblDetailPaymentStatus.CssClass = GetPaymentStatusClass(paymentStatus);
        }
    }

    // =========================================================
    // GET TOTAL AMOUNT PAID
    // =========================================================

    private decimal GetAmountPaid(SqlConnection con, int salesID)
    {
        string query = @"
        SELECT ISNULL(SUM(AmountPaid), 0)
        FROM tbl_SalesPayments
        WHERE SalesID = @SalesID";

        using (SqlCommand cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@SalesID", salesID);

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToDecimal(result);
        }
    }

    // =========================================================
    // PRINT INVOICE
    // =========================================================

    private void LoadPrintInvoice(int salesID)
    {
        string headerQuery = @"
        SELECT
            s.InvoiceNumber,
            s.SalesDate,
            c.CustomerName,
            w.WarehouseName,
            s.TotalAmount
        FROM tbl_Sales s
        INNER JOIN tbl_Customers c ON s.CustomerID = c.CustomerID
        INNER JOIN tbl_Warehouses w ON s.WarehouseID = w.WarehouseID
        WHERE s.SalesID = @SalesID";

        string detailQuery = @"
        SELECT
            p.ProductName,
            sd.Quantity,
            sd.SalePrice,
            sd.TotalAmount
        FROM tbl_SalesDetails sd
        INNER JOIN tbl_Products p ON sd.ProductID = p.ProductID
        WHERE sd.SalesID = @SalesID
        ORDER BY sd.SalesDetailID";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            con.Open();

            string invoice = "";
            string customer = "";
            string warehouse = "";
            decimal totalAmount = 0;

            // 1. LOAD INVOICE HEADER
            using (SqlCommand cmd = new SqlCommand(headerQuery, con))
            {
                cmd.Parameters.AddWithValue("@SalesID", salesID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        invoice = dr["InvoiceNumber"].ToString();
                        customer = dr["CustomerName"].ToString();
                        warehouse = dr["WarehouseName"].ToString();
                        totalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    }
                }
            }

            // 2. GET PAYMENT INFORMATION
            decimal amountPaid = GetAmountPaid(con, salesID);
            decimal amountDue = totalAmount - amountPaid;

            if (amountDue < 0)
            {
                amountDue = 0;
            }

            string paymentStatus = GetPaymentStatus(totalAmount, amountPaid);

            // 3. SET SELLER COPY
            lblPrintInvoiceSeller.Text = invoice;
            lblPrintCustomerSeller.Text = customer;
            lblPrintWarehouseSeller.Text = warehouse;
            lblPrintTotalSeller.Text = totalAmount.ToString("N2");
            lblPrintPaidSeller.Text = amountPaid.ToString("N2");
            lblPrintDueSeller.Text = amountDue.ToString("N2");
            lblPrintPaymentStatusSeller.Text = paymentStatus;
            lblPrintPaymentStatusSeller.Attributes["class"] = "badge " + GetPaymentStatusClass(paymentStatus);

            // 4. SET CUSTOMER COPY
            lblPrintInvoiceCustomer.Text = invoice;
            lblPrintCustomerCustomer.Text = customer;
            lblPrintWarehouseCustomer.Text = warehouse;
            lblPrintTotalCustomer.Text = totalAmount.ToString("N2");
            lblPrintPaidCustomer.Text = amountPaid.ToString("N2");
            lblPrintDueCustomer.Text = amountDue.ToString("N2");
            lblPrintPaymentStatusCustomer.Text = paymentStatus;
            lblPrintPaymentStatusCustomer.Attributes["class"] = "badge " + GetPaymentStatusClass(paymentStatus);

            // 5. LOAD PRODUCT DETAILS
            using (SqlCommand cmd = new SqlCommand(detailQuery, con))
            {
                cmd.Parameters.AddWithValue("@SalesID", salesID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvPrintDetailsSeller.DataSource = dt;
                    gvPrintDetailsSeller.DataBind();

                    gvPrintDetailsCustomer.DataSource = dt;
                    gvPrintDetailsCustomer.DataBind();
                }
            }
        }
    }

    // =========================================================
    // CLOSE DETAILS
    // =========================================================

    protected void btnCloseDetails_Click(object sender, EventArgs e)
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

    protected void gvSales_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSales.PageIndex = e.NewPageIndex;

        LoadSales();
    }
}