using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class Pages_PaymentHistory : System.Web.UI.Page
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
            LoadPayments();


        }
    }


    // =========================================================
    // LOAD PAYMENTS
    // =========================================================

    private void LoadPayments()
    {
        string query = @"SELECT sp.PaymentID, sp.SalesID, sp.PaymentDate, s.InvoiceNumber, c.CustomerName, sp.AmountPaid, sp.PaymentMode, sp.ReferenceNumber,sp.Remarks FROM tbl_SalesPayments sp
                INNER JOIN tbl_Sales s
                ON sp.SalesID = s.SalesID
                
                INNER JOIN tbl_Customers c
                ON s.CustomerID = c.CustomerID

            WHERE
                (
                    @Search = ''
                    OR s.InvoiceNumber LIKE '%' + @Search + '%'
                    OR c.CustomerName LIKE '%' + @Search + '%'
                    OR ISNULL(sp.ReferenceNumber, '') LIKE '%' + @Search + '%'
                )

                AND
                (
                    @FromDate = ''
                    OR sp.PaymentDate >= @FromDate
                )

                AND
                (
                    @ToDate = ''
                    OR sp.PaymentDate < DATEADD(DAY, 1, @ToDate)
                )

            ORDER BY
                sp.PaymentID DESC";


        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Search",txtSearch.Text.Trim());
                cmd.Parameters.AddWithValue("@FromDate",txtFromDate.Text.Trim());
                cmd.Parameters.AddWithValue("@ToDate",txtToDate.Text.Trim());

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    // Bind payment table

                    gvPayments.DataSource = dt;
                    gvPayments.DataBind();


                    // Update summary

                    UpdateSummary(dt);
                }
            }
        }
    }


    // =========================================================
    // UPDATE SUMMARY
    // =========================================================

    private void UpdateSummary(DataTable dt)
    {
        // Number of payment records

        lblPaymentCount.Text = dt.Rows.Count.ToString();


        // Total amount received

        decimal totalReceived = 0;

        foreach (DataRow row in dt.Rows)
        {
            if (row["AmountPaid"] != DBNull.Value)
            {
                totalReceived += Convert.ToDecimal(row["AmountPaid"]);
            }
        }

        lblTotalReceived.Text = totalReceived.ToString("N2");


        // Number of different payment modes

        int paymentModes = 0;

        DataTable modeTable = dt.DefaultView.ToTable(true, "PaymentMode" );

        foreach (DataRow row in modeTable.Rows)
        {
            if (row["PaymentMode"] != DBNull.Value &&
                !string.IsNullOrEmpty(
                    row["PaymentMode"].ToString()))
            {
                paymentModes++;
            }
        }

        lblPaymentModes.Text = paymentModes.ToString();
    }


    // =========================================================
    // SEARCH BUTTON
    // =========================================================

    protected void btnSearch_Click(object sender,EventArgs e)
    {
        LoadPayments();
    }
}