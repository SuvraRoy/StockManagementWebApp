using System;
using System.Data.SqlClient; // Must include this to use SqlConnection
using System.Web.UI;

public partial class TestConn : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Clear status text on initial load
        if (!IsPostBack)
        {
            lblStatus.Text = "";
        }
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        // 1. Grab your connection string through your Connection helper class
        string connStr = Connection.getConnectionString();

        // 2. Open a scoped SQL Connection pipeline
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            try
            {
                // Attempt an active handshake with your SQL Server
                conn.Open();

                // Success response mapping
                lblStatus.Text = "✓ Connection Successful! Your app is talking to SQL Server.";
                lblStatus.CssClass = "alert alert-success text-success fw-bold mt-2";
            }
            catch (Exception ex)
            {
                // Failure parsing to catch bad server names, spelling errors or authentication drops
                lblStatus.Text = "✕ Connection Failed!<br/><small class='fw-normal text-start d-block mt-2'>" + ex.Message + "</small>";
                lblStatus.CssClass = "alert alert-danger text-danger fw-bold mt-2 text-start";
            }
        }
    }
}