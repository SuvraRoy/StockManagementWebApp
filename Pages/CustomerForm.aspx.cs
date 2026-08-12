using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

public partial class Pages_CustomerForm : System.Web.UI.Page
{
    private readonly string connStr = Connection.getConnectionString();

    private int CustomerID
    {
        get
        {
            int id;

            if (int.TryParse(Request.QueryString["id"], out id))
            {
                return id;
            }

            return 0;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CustomerID > 0)
            {
                // Edit Mode
                litPageTitle.Text = "Edit Customer";
                btnSave.Text = "Update Customer";

                LoadCustomer(CustomerID);
            }
            else
            {
                // Add Mode
                litPageTitle.Text = "Add Customer";
                btnSave.Text = "Save Customer";

                GenerateCustomerCode();
            }
        }
    }


    // =========================================================
    // GENERATE CUSTOMER CODE
    // =========================================================

    private void GenerateCustomerCode()
    {
        string query = @"
            SELECT ISNULL(MAX(CustomerID), 0) + 1
            FROM tbl_Customers";


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();

                int nextId = Convert.ToInt32(cmd.ExecuteScalar());

                txtCustomerCode.Text =
                    "CUS-" + nextId.ToString("D4");
            }
        }
    }


    // =========================================================
    // LOAD CUSTOMER FOR EDIT
    // =========================================================

    private void LoadCustomer(int customerId)
    {
        string query = @"
            SELECT
                CustomerCode,
                CustomerName,
                Phone,
                Email,
                Address,
                City,
                State,
                PinCode,
                GSTIN
            FROM tbl_Customers
            WHERE CustomerID = @CustomerID";


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    customerId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtCustomerCode.Text =
                            reader["CustomerCode"].ToString();

                        txtCustomerName.Text =
                            reader["CustomerName"].ToString();

                        txtPhone.Text =
                            reader["Phone"].ToString();

                        txtEmail.Text =
                            reader["Email"] == DBNull.Value
                                ? ""
                                : reader["Email"].ToString();

                        txtAddress.Text =
                            reader["Address"] == DBNull.Value
                                ? ""
                                : reader["Address"].ToString();

                        txtCity.Text =
                            reader["City"] == DBNull.Value
                                ? ""
                                : reader["City"].ToString();

                        txtState.Text =
                            reader["State"] == DBNull.Value
                                ? ""
                                : reader["State"].ToString();

                        txtPinCode.Text =
                            reader["PinCode"] == DBNull.Value
                                ? ""
                                : reader["PinCode"].ToString();

                        txtGSTIN.Text =
                            reader["GSTIN"] == DBNull.Value
                                ? ""
                                : reader["GSTIN"].ToString();
                    }
                    else
                    {
                        ShowError(
                            "Customer not found.");

                        btnSave.Enabled = false;
                    }
                }
            }
        }
    }


    // =========================================================
    // SAVE / UPDATE CUSTOMER
    // =========================================================

    protected void btnSave_Click(
        object sender,
        EventArgs e)
    {
        // Run ASP.NET validators
        Page.Validate();

        if (!Page.IsValid)
        {
            return;
        }


        // Clean input
        string customerName =
            txtCustomerName.Text.Trim();

        string phone =
            txtPhone.Text.Trim();

        string email =
            txtEmail.Text.Trim();

        string address =
            txtAddress.Text.Trim();

        string city =
            txtCity.Text.Trim();

        string state =
            txtState.Text.Trim();

        string pinCode =
            txtPinCode.Text.Trim();

        string gstin =
            txtGSTIN.Text.Trim().ToUpper();


        // Additional server-side validation
        if (!ValidateCustomerInput(
            phone,
            email,
            pinCode,
            gstin))
        {
            return;
        }


        // Check duplicate phone
        if (IsDuplicatePhone(phone, CustomerID))
        {
            ShowError(
                "This phone number is already registered with another customer.");

            return;
        }


        // Check duplicate email
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (IsDuplicateEmail(email, CustomerID))
            {
                ShowError(
                    "This email address is already registered with another customer.");

                return;
            }
        }

        // Check duplicate GSTIN
        if (!string.IsNullOrWhiteSpace(gstin))
        {
            if (IsDuplicateGSTIN(gstin, CustomerID))
            {
                ShowError(
                    "This GSTIN is already registered with another customer.");

                return;
            }
        }


        try
        {
            if (CustomerID > 0)
            {
                // UPDATE
                UpdateCustomer(
                    customerName,
                    phone,
                    email,
                    address,
                    city,
                    state,
                    pinCode,
                    gstin);
            }
            else
            {
                // INSERT
                InsertCustomer(
                    customerName,
                    phone,
                    email,
                    address,
                    city,
                    state,
                    pinCode,
                    gstin);
            }
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to save customer. " +
                ex.Message);
        }
    }

    // =========================================================
    // DUPLICATE EMAIL CHECK
    // =========================================================

    private bool IsDuplicateEmail(
        string email,
        int customerId)
    {
        string query = @"
        SELECT COUNT(*)
        FROM tbl_Customers
        WHERE LOWER(Email) = LOWER(@Email)
        AND CustomerID <> @CustomerID";


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Email",
                    email.Trim());

                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    customerId);

                conn.Open();

                int count =
                    Convert.ToInt32(
                        cmd.ExecuteScalar());

                return count > 0;
            }
        }
    }


    // =========================================================
    // INSERT CUSTOMER
    // =========================================================

    private void InsertCustomer(
        string customerName,
        string phone,
        string email,
        string address,
        string city,
        string state,
        string pinCode,
        string gstin)
    {
        string customerCode =
            GenerateNextCustomerCode();


        string query = @"
            INSERT INTO tbl_Customers
            (
                CustomerCode,
                CustomerName,
                Phone,
                Email,
                Address,
                City,
                State,
                PinCode,
                GSTIN
            )
            VALUES
            (
                @CustomerCode,
                @CustomerName,
                @Phone,
                @Email,
                @Address,
                @City,
                @State,
                @PinCode,
                @GSTIN
            )";


        using (SqlConnection conn = new SqlConnection(connStr))

        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                AddCustomerParameters(
                    cmd,
                    customerCode,
                    customerName,
                    phone,
                    email,
                    address,
                    city,
                    state,
                    pinCode,
                    gstin);


                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }


        Response.Redirect(
            "Customers.aspx?message=added");
    }


    // =========================================================
    // UPDATE CUSTOMER
    // =========================================================

    private void UpdateCustomer(
        string customerName,
        string phone,
        string email,
        string address,
        string city,
        string state,
        string pinCode,
        string gstin)
    {
        string query = @"
            UPDATE tbl_Customers
            SET
                CustomerName = @CustomerName,
                Phone = @Phone,
                Email = @Email,
                Address = @Address,
                City = @City,
                State = @State,
                PinCode = @PinCode,
                GSTIN = @GSTIN
            WHERE CustomerID = @CustomerID";


        using (SqlConnection conn = new SqlConnection(connStr))

        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    CustomerID);

                cmd.Parameters.AddWithValue(
                    "@CustomerName",
                    customerName);

                cmd.Parameters.AddWithValue(
                    "@Phone",
                    phone);

                cmd.Parameters.AddWithValue(
                    "@Email",
                    string.IsNullOrWhiteSpace(email)
                        ? (object)DBNull.Value
                        : email);

                cmd.Parameters.AddWithValue(
                    "@Address",
                    string.IsNullOrWhiteSpace(address)
                        ? (object)DBNull.Value
                        : address);

                cmd.Parameters.AddWithValue(
                    "@City",
                    string.IsNullOrWhiteSpace(city)
                        ? (object)DBNull.Value
                        : city);

                cmd.Parameters.AddWithValue(
                    "@State",
                    string.IsNullOrWhiteSpace(state)
                        ? (object)DBNull.Value
                        : state);

                cmd.Parameters.AddWithValue(
                    "@PinCode",
                    string.IsNullOrWhiteSpace(pinCode)
                        ? (object)DBNull.Value
                        : pinCode);

                cmd.Parameters.AddWithValue(
                    "@GSTIN",
                    string.IsNullOrWhiteSpace(gstin)
                        ? (object)DBNull.Value
                        : gstin);


                conn.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    Response.Redirect(
                        "Customers.aspx?message=updated");
                }
                else
                {
                    ShowError(
                        "Customer could not be updated.");
                }
            }
        }
    }


    // =========================================================
    // GENERATE NEXT CUSTOMER CODE
    // =========================================================

    private string GenerateNextCustomerCode()
    {
        string query = @"
            SELECT ISNULL(MAX(CustomerID), 0) + 1
            FROM tbl_Customers";


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                conn.Open();

                int nextId =
                    Convert.ToInt32(
                        cmd.ExecuteScalar());

                return "CUS-" +
                       nextId.ToString("D4");
            }
        }
    }


    // =========================================================
    // DUPLICATE PHONE CHECK
    // =========================================================

    private bool IsDuplicatePhone(
        string phone,
        int customerId)
    {
        string query = @"
            SELECT COUNT(*)
            FROM tbl_Customers
            WHERE Phone = @Phone
            AND CustomerID <> @CustomerID";


        using (SqlConnection conn = new SqlConnection(connStr))

        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Phone",
                    phone);

                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    customerId);

                conn.Open();

                int count =
                    Convert.ToInt32(
                        cmd.ExecuteScalar());

                return count > 0;
            }
        }
    }


    // =========================================================
    // DUPLICATE GSTIN CHECK
    // =========================================================

    private bool IsDuplicateGSTIN(
        string gstin,
        int customerId)
    {
        string query = @"
            SELECT COUNT(*)
            FROM tbl_Customers
            WHERE GSTIN = @GSTIN
            AND CustomerID <> @CustomerID";


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@GSTIN",
                    gstin);

                cmd.Parameters.AddWithValue(
                    "@CustomerID",
                    customerId);

                conn.Open();

                int count =
                    Convert.ToInt32(
                        cmd.ExecuteScalar());

                return count > 0;
            }
        }
    }


    // =========================================================
    // ADD PARAMETERS
    // =========================================================

    private void AddCustomerParameters(
        SqlCommand cmd,
        string customerCode,
        string customerName,
        string phone,
        string email,
        string address,
        string city,
        string state,
        string pinCode,
        string gstin)
    {
        cmd.Parameters.AddWithValue(
            "@CustomerCode",
            customerCode);

        cmd.Parameters.AddWithValue(
            "@CustomerName",
            customerName);

        cmd.Parameters.AddWithValue(
            "@Phone",
            phone);

        cmd.Parameters.AddWithValue(
            "@Email",
            string.IsNullOrWhiteSpace(email)
                ? (object)DBNull.Value
                : email);

        cmd.Parameters.AddWithValue(
            "@Address",
            string.IsNullOrWhiteSpace(address)
                ? (object)DBNull.Value
                : address);

        cmd.Parameters.AddWithValue(
            "@City",
            string.IsNullOrWhiteSpace(city)
                ? (object)DBNull.Value
                : city);

        cmd.Parameters.AddWithValue(
            "@State",
            string.IsNullOrWhiteSpace(state)
                ? (object)DBNull.Value
                : state);

        cmd.Parameters.AddWithValue(
            "@PinCode",
            string.IsNullOrWhiteSpace(pinCode)
                ? (object)DBNull.Value
                : pinCode);

        cmd.Parameters.AddWithValue(
            "@GSTIN",
            string.IsNullOrWhiteSpace(gstin)
                ? (object)DBNull.Value
                : gstin);
    }


    // =========================================================
    // VALIDATE CUSTOMER INPUT
    // =========================================================

    private bool ValidateCustomerInput(
        string phone,
        string email,
        string pinCode,
        string gstin)
    {
        // Phone
        if (!Regex.IsMatch(
    phone,
    @"^[6-9][0-9]{9}$"))
        {
            ShowError(
                "Please enter a valid 10-digit Indian mobile number starting with 6, 7, 8, or 9.");

            return false;
        }


        // Email
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowError(
                    "Please enter a valid email address.");

                return false;
            }
        }


        // PIN Code
        if (!string.IsNullOrWhiteSpace(pinCode))
        {
            if (!Regex.IsMatch(
                pinCode,
                @"^[0-9]{6}$"))
            {
                ShowError(
                    "PIN code must contain exactly 6 digits.");

                return false;
            }
        }


        // GSTIN
        if (!string.IsNullOrWhiteSpace(gstin))
        {
            if (!Regex.IsMatch(
                gstin,
                @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$"))
            {
                ShowError(
                    "Please enter a valid GSTIN.");

                return false;
            }
        }


        return true;
    }


    // =========================================================
    // ERROR MESSAGE
    // =========================================================

    private void ShowError(
        string message)
    {
        lblMessage.Text =
            message;

        lblMessage.CssClass =
            "alert alert-danger error-message d-block";

        lblMessage.Visible = true;
    }
}