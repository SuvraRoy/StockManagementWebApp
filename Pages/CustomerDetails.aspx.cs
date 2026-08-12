using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

public partial class Pages_CustomerDetails : System.Web.UI.Page
{
    private readonly string connStr = Connection.getConnectionString();


    private int CustomerID
    {
        get
        {
            int id;

            if (int.TryParse(
                Request.QueryString["id"],
                out id))
            {
                return id;
            }

            return 0;
        }
    }


    protected void Page_Load(
        object sender,
        EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CustomerID <= 0)
            {
                ShowError(
                    "Invalid customer ID.");

                return;
            }

            LoadCustomerDetails(CustomerID);
        }
    }


    // =========================================================
    // LOAD CUSTOMER DETAILS
    // =========================================================

    private void LoadCustomerDetails(
        int customerId)
    {
        string query = @"
            SELECT
                CustomerID,
                CustomerCode,
                CustomerName,
                Phone,
                Email,
                Address,
                City,
                State,
                PinCode,
                GSTIN,
                CreatedDate,
                IsActive
            FROM tbl_Customers
            WHERE CustomerID = @CustomerID";


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    customerId);

                try
                {
                    conn.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DisplayCustomerDetails(
                                reader);
                        }
                        else
                        {
                            ShowError(
                                "Customer not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to load customer details. " +
                        ex.Message);
                }
            }
        }
    }


    // =========================================================
    // DISPLAY CUSTOMER DETAILS
    // =========================================================

    private void DisplayCustomerDetails(
      SqlDataReader reader)
    {
        string customerName =
            reader["CustomerName"].ToString();


        // Customer Name
        lblCustomerName.Text =
            HttpUtility.HtmlEncode(
                customerName);


        // Customer Code
        lblCustomerCode.Text =
            "Customer Code: " +
            HttpUtility.HtmlEncode(
                reader["CustomerCode"].ToString());


        // Avatar
        


        // Status
        bool isActive =
            Convert.ToBoolean(
                reader["IsActive"]);


        if (isActive)
        {
            lblStatus.Text = "Active";
            lblStatus.CssClass =
                "status-badge status-active";
        }
        else
        {
            lblStatus.Text = "Inactive";
            lblStatus.CssClass =
                "status-badge status-inactive";
        }


        // Edit Link
        lnkEdit.NavigateUrl =
            "~/Pages/CustomerForm.aspx?id=" +
            reader["CustomerID"].ToString();


        // Customer Details
        lblPhone.Text =
            FormatValue(reader["Phone"]);

        lblEmail.Text =
            FormatValue(reader["Email"]);

        lblAddress.Text =
            FormatValue(reader["Address"]);

        lblCity.Text =
            FormatValue(reader["City"]);

        lblState.Text =
            FormatValue(reader["State"]);

        lblPinCode.Text =
            FormatValue(reader["PinCode"]);

        lblGSTIN.Text =
            FormatValue(reader["GSTIN"]);


        // Created Date
        if (reader["CreatedDate"] != DBNull.Value)
        {
            DateTime createdDate =
                Convert.ToDateTime(
                    reader["CreatedDate"]);

            lblCreatedDate.Text =
                createdDate.ToString(
                    "dd MMM yyyy");
        }
        else
        {
            lblCreatedDate.Text =
                "Not available";

            lblCreatedDate.CssClass =
                "detail-value empty-value";
        }
    }
    // =========================================================
    // Format Optional Value
    // =========================================================


    private string FormatValue(
        object value)
    {
        if (value == null ||
            value == DBNull.Value ||
            string.IsNullOrWhiteSpace(
                value.ToString()))
        {
            return "Not provided";
        }


        return HttpUtility.HtmlEncode(
            value.ToString());
    }

    // =========================================================
    // SHOW ERROR
    // =========================================================

    private void ShowError(
        string message)
    {
        pnlError.Visible = true;

        lblError.Text =
            HttpUtility.HtmlEncode(
                message);

        lnkEdit.Visible = false;
    }
}