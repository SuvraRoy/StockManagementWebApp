using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient; // Must include this to use SqlConnection


public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Intercept unauthorized requests lacking a verified authentication token
        if (Session["Username"] == null)
        {
            // Lock out visitor and force route them back to the sign-in prompt
            Response.Redirect("~/login.aspx");
        }

        string connStr = Connection.getConnectionString();
        SqlConnection conn = new SqlConnection(connStr);
        conn.Open();

        string prodQuery = "SELECT COUNT(*) FROM tbl_Products";
        using (SqlCommand cmd = new SqlCommand(prodQuery, conn))
        {
            int productCount = (int)cmd.ExecuteScalar();
            lblProducts.Text = productCount.ToString();
        }

        string supQuery = "SELECT COUNT(*) FROM tbl_Suppliers";
        using(SqlCommand cmd =  new SqlCommand(supQuery, conn))
        {
            int supCount = (int)cmd.ExecuteScalar();
            lblSuppliers.Text = supCount.ToString();
        }

        string purQuery = "SELECT COUNT(*) FROM tbl_PurchaseLedger";
        using (SqlCommand cmd = new SqlCommand(purQuery, conn))
        {
            int supCount = (int)cmd.ExecuteScalar();
            lblPurchases.Text = supCount.ToString();
        }

        string lowQuery = "SELECT COUNT(*) FROM tbl_Products WHERE CurrentStock <= ReorderLevel";
        using (SqlCommand cmd = new SqlCommand(lowQuery, conn))
        {
            int lowCount = (int)cmd.ExecuteScalar();
            lblLowStock.Text = lowCount.ToString();
        }




        conn.Close();
    }
}

