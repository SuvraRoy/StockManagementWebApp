
using System;
using System.Data;
using System.Data.SqlClient;

public partial class Pages_PurchaseDetails : System.Web.UI.Page
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
            int purchaseId;

            if (!int.TryParse(
                Request.QueryString["PurchaseID"],
                out purchaseId))
            {
                Response.Redirect("PurchaseHistory.aspx");
                return;
            }

            LoadPurchase(purchaseId);
            LoadPurchaseItems(purchaseId);
        }
    }


    private void LoadPurchase(int purchaseId)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
               new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    p.PurchaseID,
                    p.PurchaseDate,
                    p.InvoiceNumber,
                    p.TotalAmount,
                    s.CompanyName
                FROM tbl_Purchases p

                INNER JOIN tbl_Suppliers s
                    ON p.SupplierID = s.SupplierID

                WHERE p.PurchaseID = @PurchaseID";


            using (SqlCommand cmd =
                   new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@PurchaseID",
                    purchaseId
                );

                conn.Open();

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    //lblPurchaseNumber.Text =
                    //    reader["PurchaseNumber"].ToString();

                    lblSupplier.Text =
                        reader["CompanyName"].ToString();

                    lblPurchaseDate.Text =
                        Convert.ToDateTime(
                            reader["PurchaseDate"]
                        ).ToString("dd-MMM-yyyy");

                    lblInvoiceNumber.Text =
                        string.IsNullOrWhiteSpace(
                            reader["InvoiceNumber"].ToString())
                            ? "-"
                            : reader["InvoiceNumber"].ToString();

                    lblTotalAmount.Text =
                        "₹ " +
                        Convert.ToDecimal(
                            reader["TotalAmount"]
                        ).ToString("N2");
                }
                else
                {
                    Response.Redirect(
                        "PurchaseHistory.aspx"
                    );
                }
            }
        }
    }


    private void LoadPurchaseItems(int purchaseId)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
               new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                     pr.ProductName,
 pr.SKU,
 d.Quantity,
 d.PurchasePrice

                FROM tbl_PurchaseDetails d

                INNER JOIN tbl_Products pr
                    ON d.ProductID = pr.ProductID

                WHERE d.PurchaseID = @PurchaseID

                ORDER BY d.PurchaseDetailID";


            using (SqlCommand cmd =
                   new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@PurchaseID",
                    purchaseId
                );

                using (SqlDataAdapter da =
                       new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    gvPurchaseDetails.DataSource = dt;
                    gvPurchaseDetails.DataBind();
                }
            }
        }
    }


    protected void btnBack_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect("PurchaseHistory.aspx");
    }
}
