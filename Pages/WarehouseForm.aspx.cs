using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class Pages_WarehouseForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!IsPostBack)
        {
            string warehouseID =
                Request.QueryString["WarehouseID"];

            if (!string.IsNullOrEmpty(warehouseID))
            {
                LoadWarehouse(Convert.ToInt32(warehouseID));
            }
            else
            {
                txtWarehouseCode.Text =
                    GenerateWarehouseCode();

                lblPageTitle.InnerText =
                    "Add Warehouse";

                btnSave.Text =
                    "Save Warehouse";
            }
        }
    }

    private string GenerateWarehouseCode()
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT ISNULL(MAX(WarehouseID), 0) + 1
                FROM tbl_Warehouses";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                conn.Open();

                int nextID =
                    Convert.ToInt32(
                        cmd.ExecuteScalar());

                return "WH-" +
                    nextID.ToString("0000");
            }
        }
    }

    private void LoadWarehouse(int warehouseID)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    WarehouseID,
                    WarehouseCode,
                    WarehouseName,
                    WarehouseType,
                    Phone,
                    Email,
                    Country,
                    State,
                    City,
                    PinCode,
                    Address,
                    IsMainWarehouse,
                    IsActive
                FROM tbl_Warehouses
                WHERE WarehouseID = @WarehouseID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Response.Redirect(
                            "Warehouses.aspx");

                        return;
                    }

                    hfWarehouseID.Value =
                        reader["WarehouseID"].ToString();

                    txtWarehouseCode.Text =
                        reader["WarehouseCode"].ToString();

                    txtWarehouseName.Text =
                        reader["WarehouseName"].ToString();

                    string warehouseType =
                        reader["WarehouseType"].ToString();

                    if (ddlWarehouseType.Items.FindByValue(
                        warehouseType) != null)
                    {
                        ddlWarehouseType.SelectedValue =
                            warehouseType;
                    }

                    txtPhone.Text =
                        reader["Phone"].ToString();

                    txtEmail.Text =
                        reader["Email"].ToString();

                    if (ddlCountry.Items.FindByValue(
                        reader["Country"].ToString()) != null)
                    {
                        ddlCountry.SelectedValue =
                            reader["Country"].ToString();
                    }

                    txtState.Text =
                        reader["State"].ToString();

                    txtCity.Text =
                        reader["City"].ToString();

                    txtPinCode.Text =
                        reader["PinCode"].ToString();

                    txtAddress.Text =
                        reader["Address"].ToString();

                    lblPageTitle.InnerText =
                        "Edit Warehouse";

                    btnSave.Text =
                        "Update Warehouse";
                }
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string warehouseName =
            txtWarehouseName.Text.Trim();

        string phone =
            txtPhone.Text.Trim();

        string email =
            txtEmail.Text.Trim();

        string state =
            txtState.Text.Trim();

        string city =
            txtCity.Text.Trim();

        string pinCode =
            txtPinCode.Text.Trim();

        string address =
            txtAddress.Text.Trim();

        string warehouseType =
            ddlWarehouseType.SelectedValue;

        int warehouseID = 0;

        if (!string.IsNullOrEmpty(hfWarehouseID.Value))
        {
            warehouseID =
                Convert.ToInt32(
                    hfWarehouseID.Value);
        }

        // ============================================
        // BASIC VALIDATION
        // ============================================

        if (string.IsNullOrWhiteSpace(warehouseName))
        {
            ShowToast(
                "Warehouse name is required.",
                "danger");

            return;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            ShowToast(
                "Phone number is required.",
                "danger");

            return;
        }

        if (!IsValidPhone(phone))
        {
            ShowToast(
                "Enter a valid 10-digit Indian mobile number.",
                "danger");

            return;
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            !IsValidEmail(email))
        {
            ShowToast(
                "Enter a valid email address.",
                "danger");

            return;
        }

        if (!string.IsNullOrWhiteSpace(pinCode) &&
            !IsValidPinCode(pinCode))
        {
            ShowToast(
                "Enter a valid pin code.",
                "danger");

            return;
        }

        // ============================================
        // DUPLICATE CHECKS
        // ============================================

        string duplicateMessage =
            CheckDuplicates(
                warehouseName,
                phone,
                email,
                warehouseID);

        if (!string.IsNullOrEmpty(duplicateMessage))
        {
            ShowToast(
                duplicateMessage,
                "danger");

            return;
        }

        // ============================================
        // MAIN WAREHOUSE RULE
        // ============================================

        if (warehouseType == "Main")
        {
            if (!IsActiveWarehouse(warehouseID))
            {
                ShowToast(
                    "An inactive warehouse cannot be made Main.",
                    "danger");

                return;
            }

            if (MainWarehouseExists(warehouseID))
            {
                ShowToast(
                    "A Main Warehouse already exists. Change the existing Main Warehouse to Branch first.",
                    "danger");

                return;
            }
        }

        // ============================================
        // INSERT
        // ============================================

        if (warehouseID == 0)
        {
            InsertWarehouse(
                warehouseName,
                warehouseType,
                phone,
                email,
                state,
                city,
                pinCode,
                address);

            return;
        }

        // ============================================
        // UPDATE
        // ============================================

        UpdateWarehouse(
            warehouseID,
            warehouseName,
            warehouseType,
            phone,
            email,
            state,
            city,
            pinCode,
            address);
    }

    private void InsertWarehouse(
        string warehouseName,
        string warehouseType,
        string phone,
        string email,
        string state,
        string city,
        string pinCode,
        string address)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                INSERT INTO tbl_Warehouses
                (
                    WarehouseCode,
                    WarehouseName,
                    WarehouseType,
                    Phone,
                    Email,
                    Country,
                    State,
                    City,
                    PinCode,
                    Address,
                    IsMainWarehouse,
                    IsActive
                )
                VALUES
                (
                    @WarehouseCode,
                    @WarehouseName,
                    @WarehouseType,
                    @Phone,
                    @Email,
                    @Country,
                    @State,
                    @City,
                    @PinCode,
                    @Address,
                    @IsMainWarehouse,
                    1
                )";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseCode",
                    txtWarehouseCode.Text.Trim());

                cmd.Parameters.AddWithValue(
                    "@WarehouseName",
                    warehouseName);

                cmd.Parameters.AddWithValue(
                    "@WarehouseType",
                    warehouseType);

                cmd.Parameters.AddWithValue(
                    "@Phone",
                    phone);

                cmd.Parameters.AddWithValue(
                    "@Email",
                    email);

                cmd.Parameters.AddWithValue(
                    "@Country",
                    ddlCountry.SelectedValue);

                cmd.Parameters.AddWithValue(
                    "@State",
                    state);

                cmd.Parameters.AddWithValue(
                    "@City",
                    city);

                cmd.Parameters.AddWithValue(
                    "@PinCode",
                    pinCode);

                cmd.Parameters.AddWithValue(
                    "@Address",
                    address);

                int isMain =
                    warehouseType == "Main" ? 1 : 0;

                cmd.Parameters.AddWithValue(
                    "@IsMainWarehouse",
                    isMain);

                conn.Open();

                int result =
                    cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    Response.Redirect(
                        "Warehouses.aspx?msg=saved");
                }
            }
        }
    }

    private void UpdateWarehouse(
        int warehouseID,
        string warehouseName,
        string warehouseType,
        string phone,
        string email,
        string state,
        string city,
        string pinCode,
        string address)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                UPDATE tbl_Warehouses
                SET
                    WarehouseName = @WarehouseName,
                    WarehouseType = @WarehouseType,
                    Phone = @Phone,
                    Email = @Email,
                    Country = @Country,
                    State = @State,
                    City = @City,
                    PinCode = @PinCode,
                    Address = @Address,
                    IsMainWarehouse = @IsMainWarehouse
                WHERE WarehouseID = @WarehouseID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseName",
                    warehouseName);

                cmd.Parameters.AddWithValue(
                    "@WarehouseType",
                    warehouseType);

                cmd.Parameters.AddWithValue(
                    "@Phone",
                    phone);

                cmd.Parameters.AddWithValue(
                    "@Email",
                    email);

                cmd.Parameters.AddWithValue(
                    "@Country",
                    ddlCountry.SelectedValue);

                cmd.Parameters.AddWithValue(
                    "@State",
                    state);

                cmd.Parameters.AddWithValue(
                    "@City",
                    city);

                cmd.Parameters.AddWithValue(
                    "@PinCode",
                    pinCode);

                cmd.Parameters.AddWithValue(
                    "@Address",
                    address);

                int isMain =
                    warehouseType == "Main" ? 1 : 0;

                cmd.Parameters.AddWithValue(
                    "@IsMainWarehouse",
                    isMain);

                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                conn.Open();

                int result =
                    cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    Response.Redirect(
                        "Warehouses.aspx?msg=updated");
                }
                else
                {
                    ShowToast(
                        "Warehouse could not be updated.",
                        "danger");
                }
            }
        }
    }

    private string CheckDuplicates(
        string warehouseName,
        string phone,
        string email,
        int warehouseID)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT TOP 1
                    WarehouseName,
                    Phone,
                    Email
                FROM tbl_Warehouses
                WHERE WarehouseID <> @WarehouseID
                AND
                (
                    UPPER(WarehouseName) =
                        UPPER(@WarehouseName)
                    OR Phone = @Phone
                    OR
                    (
                        @Email <> ''
                        AND Email = @Email
                    )
                )";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                cmd.Parameters.AddWithValue(
                    "@WarehouseName",
                    warehouseName);

                cmd.Parameters.AddWithValue(
                    "@Phone",
                    phone);

                cmd.Parameters.AddWithValue(
                    "@Email",
                    email);

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (string.Equals(
                            reader["WarehouseName"].ToString(),
                            warehouseName,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return "Warehouse name already exists.";
                        }

                        if (reader["Phone"].ToString() == phone)
                        {
                            return "Phone number already exists.";
                        }

                        if (!string.IsNullOrWhiteSpace(email) &&
                            string.Equals(
                                reader["Email"].ToString(),
                                email,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "Email address already exists.";
                        }
                    }
                }
            }
        }

        return "";
    }

    private bool MainWarehouseExists(int warehouseID)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT COUNT(*)
                FROM tbl_Warehouses
                WHERE IsMainWarehouse = 1
                AND IsActive = 1
                AND WarehouseID <> @WarehouseID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0;
            }
        }
    }

    private bool IsActiveWarehouse(int warehouseID)
    {
        // New warehouse is active by default.
        if (warehouseID == 0)
            return true;

        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT IsActive
                FROM tbl_Warehouses
                WHERE WarehouseID = @WarehouseID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                conn.Open();

                object result =
                    cmd.ExecuteScalar();

                if (result == null)
                    return false;

                return Convert.ToBoolean(result);
            }
        }
    }

    private bool IsValidPhone(string phone)
    {
        return Regex.IsMatch(
            phone,
            @"^[6-9][0-9]{9}$");
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(
            email,
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
    }

    private bool IsValidPinCode(string pinCode)
    {
        return Regex.IsMatch(
            pinCode,
            @"^[0-9]{6}$");
    }

    protected void btnCancel_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "Warehouses.aspx");
    }

    private void ShowToast(
        string message,
        string type)
    {
        lblToast.Text = message;

        liveToast.Attributes["class"] =
            "toast shadow-lg border-0";

        switch (type.ToLower())
        {
            case "success":
                liveToast.Attributes["class"] +=
                    " bg-success text-white";
                break;

            case "danger":
                liveToast.Attributes["class"] +=
                    " bg-danger text-white";
                break;

            case "warning":
                liveToast.Attributes["class"] +=
                    " bg-warning text-dark";
                break;

            case "info":
                liveToast.Attributes["class"] +=
                    " bg-info text-white";
                break;
        }

        string script = @"
            setTimeout(function () {

                var toastEl =
                    document.getElementById('" +
                    liveToast.ClientID + @"');

                if (toastEl) {

                    var toast =
                        new bootstrap.Toast(
                            toastEl,
                            {
                                autohide: true,
                                delay: 4000
                            });

                    toast.show();
                }

            }, 50);";

        ScriptManager.RegisterStartupScript(
            this.Page,
            this.Page.GetType(),
            "showToast_" + Guid.NewGuid(),
            script,
            true);
    }
}