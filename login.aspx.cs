using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Clear any lingering session tokens if they reload the login page
            Session.Clear();
            Session.Abandon();
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // 1. Sanity Check inputs
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Please enter both username and password.";
            pnlAlert.Visible = true;
            return;
        }

        // 2. Database validation using the connection string from Web.config
        // (Assuming you have a connection string named "DBConnectionString")
        string connString = ConfigurationManager.ConnectionStrings["StockDB"].ConnectionString;

        string query = "SELECT Username, Role FROM tbl_Users WHERE Username = @Username AND Password = @Password";

        try
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Using parameterization to eliminate SQL injection vulnerabilities
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Note: For production systems, passwords should be hashed!

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 3. User authenticated successfully! Save session values
                            Session["Username"] = reader["Username"].ToString();
                            Session["UserRole"] = reader["Role"].ToString();

                            // 4. Redirect into protected system territory
                            Response.Redirect("Default.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            // Authentication Failed: Expose Sneat's built-in alert banner components
                            lblError.Text = "Invalid username or password.";
                            pnlAlert.Visible = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Fail-safe handler for unexpected pipeline database disconnections
            lblError.Text = "An error occurred while connecting to the server. Please try again later.";
            pnlAlert.Visible = true;

            // Optional: Log ex.Message safely to debug server logs
        }
    }
}