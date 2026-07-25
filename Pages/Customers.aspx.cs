using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class Pages_Customers : System.Web.UI.Page
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
            LoadCustomers();
        }
    }


    // =========================================================
    // LOAD CUSTOMERS
    // =========================================================

    private void LoadCustomers()
    {
        string searchText = txtSearch.Text.Trim();

        string query = @"
            SELECT
                CustomerID,
                CustomerCode,
                CustomerName,
                Phone,
                Email,
                City,
                IsActive
            FROM tbl_Customers
            WHERE
                (
                    @SearchText = ''
                    OR CustomerCode LIKE '%' + @SearchText + '%'
                    OR CustomerName LIKE '%' + @SearchText + '%'
                    OR Phone LIKE '%' + @SearchText + '%'
                )
            ORDER BY CustomerID DESC";

        string connStr = Connection.getConnectionString();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SearchText", searchText);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    gvCustomers.DataSource = dt;
                    gvCustomers.DataBind();
                }
            }
        }
    }


    // =========================================================
    // SEARCH BUTTON
    // =========================================================

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvCustomers.PageIndex = 0;

        LoadCustomers();
    }


    // =========================================================
    // SEARCH TEXT CHANGED
    // =========================================================

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        gvCustomers.PageIndex = 0;

        LoadCustomers();
    }


    // =========================================================
    // CLEAR SEARCH
    // =========================================================

    protected void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearch.Text = string.Empty;

        gvCustomers.PageIndex = 0;

        LoadCustomers();
    }


    // =========================================================
    // PAGING
    // =========================================================

    protected void gvCustomers_PageIndexChanging(
        object sender,
        System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        gvCustomers.PageIndex = e.NewPageIndex;

        LoadCustomers();
    }


    // =========================================================
    // ROW COMMAND
    // =========================================================

    protected void gvCustomers_RowCommand(
        object sender,
        System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
        {
            return;
        }


        int customerId;

        if (!int.TryParse(
            e.CommandArgument.ToString(),
            out customerId))
        {
            return;
        }


        // =============================================
        // VIEW CUSTOMER
        // =============================================

        if (e.CommandName == "ViewCustomer")
        {
            Response.Redirect(
                "CustomerDetails.aspx?id=" + customerId);
        }


        // =============================================
        // EDIT CUSTOMER
        // =============================================

        else if (e.CommandName == "EditCustomer")
        {
            Response.Redirect(
                "CustomerForm.aspx?id=" + customerId);
        }
    }
}