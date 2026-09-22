using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class Pages_Payments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check login
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadInvoices();

            txtPaymentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            ClearInvoiceInformation();
        }
    }


    // =========================================================
    // LOAD SALES INVOICES
    // =========================================================

    private void LoadInvoices()
    {
        string query = @"SELECT SalesID,InvoiceNumber FROM tbl_Sales ORDER BY SalesID DESC";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    ddlInvoice.DataSource = dt;
                    ddlInvoice.DataTextField = "InvoiceNumber";
                    ddlInvoice.DataValueField = "SalesID";
                    ddlInvoice.DataBind();
                }
            }
        }

        ddlInvoice.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Invoice", "" )
        );
    }


    // =========================================================
    // WHEN INVOICE IS SELECTED
    // =========================================================

    protected void ddlInvoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlInvoice.SelectedValue))
        {
            ClearInvoiceInformation();
            return;
        }

        int salesID;

        if (int.TryParse(ddlInvoice.SelectedValue, out salesID))
        {
            LoadInvoiceInformation(salesID);
        }
    }


    // =========================================================
    // LOAD SELECTED INVOICE INFORMATION
    // =========================================================

    private void LoadInvoiceInformation(int salesID)
    {
        string query = @"SELECT s.TotalAmount, c.CustomerName, ISNULL(
                    (
                        SELECT SUM(sp.AmountPaid)
                        FROM tbl_SalesPayments sp
                        WHERE sp.SalesID = s.SalesID),
                    0
                ) AS AmountPaid

            FROM tbl_Sales s

            INNER JOIN tbl_Customers c
                ON s.CustomerID = c.CustomerID

            WHERE s.SalesID = @SalesID";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@SalesID", salesID);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        decimal totalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                        decimal amountPaid = Convert.ToDecimal(dr["AmountPaid"]);
                        decimal outstanding = totalAmount - amountPaid;

                        if (outstanding < 0)
                        {
                            outstanding = 0;
                        }

                        txtCustomer.Text = dr["CustomerName"].ToString();
                        txtInvoiceTotal.Text = totalAmount.ToString("N2");
                        txtAlreadyPaid.Text = amountPaid.ToString("N2");
                        txtOutstanding.Text = outstanding.ToString("N2");
                    }
                }
            }
        }
    }


    // =========================================================
    // SAVE PAYMENT
    // =========================================================

    protected void btnSavePayment_Click(object sender, EventArgs e)
    {
        // ---------------------------------------------
        // 1. Check invoice
        // ---------------------------------------------

        if (string.IsNullOrEmpty(ddlInvoice.SelectedValue))
        {
            ShowMessage("Please select a sales invoice.", "danger" );

            return;
        }


        // ---------------------------------------------
        // 2. Check payment date
        // ---------------------------------------------

        DateTime paymentDate;

        if (!DateTime.TryParse(txtPaymentDate.Text.Trim(), out paymentDate))
        {
            ShowMessage("Please enter a valid payment date.","danger" );

            return;
        }


        // ---------------------------------------------
        // 3. Check payment amount
        // ---------------------------------------------

        decimal amountPaid;

        if (!decimal.TryParse(txtAmountPaid.Text.Trim(), out amountPaid))
        {
            ShowMessage( "Please enter a valid payment amount.", "danger");

            return;
        }

        if (amountPaid <= 0)
        {
            ShowMessage("Payment amount must be greater than zero.", "danger" );

            return;
        }


        // ---------------------------------------------
        // 4. Get SalesID
        // ---------------------------------------------

        int salesID;

        if (!int.TryParse(ddlInvoice.SelectedValue, out salesID))
        {
            ShowMessage("Invalid invoice selected.", "danger");

            return;
        }


        // ---------------------------------------------
        // 5. Check outstanding amount again
        // ---------------------------------------------

        decimal outstanding = GetOutstandingAmount(salesID);


        if (outstanding <= 0)
        {
            ShowMessage("This invoice is already fully paid.", "warning" );

            LoadInvoiceInformation(salesID);

            return;
        }


        if (amountPaid > outstanding)
        {
            ShowMessage("Payment cannot be greater than the outstanding amount of ₹" + outstanding.ToString("N2") + ".", "danger");

            return;
        }


        // ---------------------------------------------
        // 6. Save payment
        // ---------------------------------------------

        string query = @"
            INSERT INTO tbl_SalesPayments
            (
                SalesID,
                PaymentDate,
                AmountPaid,
                PaymentMode,
                ReferenceNumber,
                Remarks,
                CreatedDate
            )
            VALUES
            (
                @SalesID,
                @PaymentDate,
                @AmountPaid,
                @PaymentMode,
                @ReferenceNumber,
                @Remarks,
                GETDATE()
            )";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@SalesID",
                    salesID
                );

                cmd.Parameters.AddWithValue(
                    "@PaymentDate",
                    paymentDate
                );

                cmd.Parameters.AddWithValue(
                    "@AmountPaid",
                    amountPaid
                );

                cmd.Parameters.AddWithValue(
                    "@PaymentMode",
                    ddlPaymentMode.SelectedValue
                );

                cmd.Parameters.AddWithValue(
                    "@ReferenceNumber",
                    txtReferenceNumber.Text.Trim()
                );

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    txtRemarks.Text.Trim()
                );

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }


        // ---------------------------------------------
        // 7. Success
        // ---------------------------------------------

        ShowMessage(
            "Payment saved successfully.",
            "success"
        );

        LoadInvoiceInformation(salesID);

        txtAmountPaid.Text = "";
        txtReferenceNumber.Text = "";
        txtRemarks.Text = "";

        ddlPaymentMode.SelectedIndex = 0;
    }


    // =========================================================
    // GET OUTSTANDING AMOUNT
    // =========================================================

    private decimal GetOutstandingAmount(int salesID)
    {
        string query = @"
            SELECT
                s.TotalAmount
                -
                ISNULL(
                    (
                        SELECT SUM(sp.AmountPaid)
                        FROM tbl_SalesPayments sp
                        WHERE sp.SalesID = s.SalesID
                    ),
                    0
                ) AS Outstanding

            FROM tbl_Sales s

            WHERE s.SalesID = @SalesID";

        using (SqlConnection con =
            new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@SalesID",
                    salesID
                );

                con.Open();

                object result =
                    cmd.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    return 0;
                }

                decimal outstanding =
                    Convert.ToDecimal(result);

                if (outstanding < 0)
                {
                    outstanding = 0;
                }

                return outstanding;
            }
        }
    }


    // =========================================================
    // CLEAR BUTTON
    // =========================================================

    protected void btnClear_Click(
        object sender,
        EventArgs e)
    {
        ddlInvoice.SelectedIndex = 0;

        ClearInvoiceInformation();

        txtAmountPaid.Text = "";
        txtReferenceNumber.Text = "";
        txtRemarks.Text = "";

        ddlPaymentMode.SelectedIndex = 0;

        txtPaymentDate.Text =
            DateTime.Now.ToString("yyyy-MM-dd");

        divMessage.Attributes["class"] =
            "alert d-none";
    }


    // =========================================================
    // CLEAR INVOICE INFORMATION
    // =========================================================

    private void ClearInvoiceInformation()
    {
        txtCustomer.Text = "";
        txtInvoiceTotal.Text = "";
        txtAlreadyPaid.Text = "";
        txtOutstanding.Text = "";
    }


    // =========================================================
    // SHOW MESSAGE
    // =========================================================

    private void ShowMessage(
        string message,
        string type)
    {
        divMessage.Attributes["class"] =
            "alert alert-" + type;

        lblMessage.Text = message;
    }
}