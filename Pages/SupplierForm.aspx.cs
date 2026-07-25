using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class Pages_SupplierForm : System.Web.UI.Page
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
            int supplierId;

            if (int.TryParse(Request.QueryString["SupplierID"], out supplierId) && supplierId > 0)
            {
                // EDIT MODE
                hfSupplierID.Value = supplierId.ToString();

                lblPageTitle.InnerText = "Edit Supplier";
                btnSave.Text = "Update Supplier";

                if (!LoadSupplier(supplierId))
                {
                    ShowValidationMessage("Supplier record was not found.");
                    btnSave.Enabled = false;
                }
            }
            else
            {
                // ADD MODE
                hfSupplierID.Value = "";

                lblPageTitle.InnerText = "Add Supplier";
                btnSave.Text = "Save Supplier";

                GenerateSupplierCode();
            }
        }
    }


    // =========================================================
    // GENERATE SUPPLIER CODE
    // =========================================================

    private void GenerateSupplierCode()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
            SELECT ISNULL(MAX(SupplierID), 0) + 1
            FROM tbl_Suppliers";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                try
                {
                    conn.Open();

                    int nextId = Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );

                    txtSupplierCode.Text =
                        "SUP" + nextId.ToString("D6");
                }
                catch (Exception ex)
                {
                    ShowValidationMessage(
                        "Unable to generate Supplier Code: "
                        + ex.Message
                    );
                }
            }
        }
    }


    // =========================================================
    // LOAD SUPPLIER FOR EDIT
    // =========================================================

    private bool LoadSupplier(int supplierId)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
            SELECT
                SupplierID,
                SupplierCode,
                CompanyName,
                Phone,
                ContactPerson,
                ContactPersonNo,
                Email,
                Country,
                State,
                City,
                PinCode,
                Address,
                GSTIN,
                PAN,
                BankName,
                AccountHolderName,
                AccountType,
                AccountNumber,
                IFSCCode
            FROM tbl_Suppliers
            WHERE SupplierID = @SupplierID";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.Add(
                    "@SupplierID",
                    SqlDbType.Int
                ).Value = supplierId;

                try
                {
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            return false;
                        }

                        txtSupplierCode.Text =
                            GetDbValue(dr, "SupplierCode");

                        txtCompanyName.Text =
                            GetDbValue(dr, "CompanyName");

                        txtWorkPhone.Text =
                            GetDbValue(dr, "Phone");

                        txtContactName.Text =
                            GetDbValue(dr, "ContactPerson");

                        txtContMobile.Text =
                            GetDbValue(dr, "ContactPersonNo");

                        txtEmail.Text =
                            GetDbValue(dr, "Email");

                        SetCountryValue(
                            GetDbValue(dr, "Country")
                        );

                        txtState.Text =
                            GetDbValue(dr, "State");

                        txtCity.Text =
                            GetDbValue(dr, "City");

                        txtPincode.Text =
                            GetDbValue(dr, "PinCode");

                        txtAddress.Text =
                            GetDbValue(dr, "Address");

                        txtGstIn.Text =
                            GetDbValue(dr, "GSTIN");

                        txtPan.Text =
                            GetDbValue(dr, "PAN");

                        txtBankName.Text =
                            GetDbValue(dr, "BankName");

                        txtAccountHolder.Text =
                            GetDbValue(
                                dr,
                                "AccountHolderName"
                            );

                        SetAccountTypeValue(
                            GetDbValue(
                                dr,
                                "AccountType"
                            )
                        );

                        txtAccountNumber.Text =
                            GetDbValue(
                                dr,
                                "AccountNumber"
                            );

                        txtIfscCode.Text =
                            GetDbValue(
                                dr,
                                "IFSCCode"
                            );

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ShowValidationMessage(
                        "Unable to load supplier: "
                        + ex.Message
                    );

                    return false;
                }
            }
        }
    }


    // =========================================================
    // SAVE / UPDATE
    // =========================================================

    protected void btnSave_Click(
        object sender,
        EventArgs e)
    {
        divMessage.Visible = false;

        string validationMessage =
            ValidateSupplier();

        if (!string.IsNullOrEmpty(
            validationMessage))
        {
            ShowValidationMessage(
                validationMessage
            );

            return;
        }


        string duplicateError =
            CheckDuplicateSupplier();

        if (!string.IsNullOrEmpty(
            duplicateError))
        {
            ShowValidationMessage(
                duplicateError
            );

            return;
        }


        try
        {
            if (string.IsNullOrEmpty(
                hfSupplierID.Value))
            {
                InsertSupplier();

                Response.Redirect(
                    "Suppliers.aspx?msg=saved"
                );
            }
            else
            {
                int supplierId;

                if (!int.TryParse(
                    hfSupplierID.Value,
                    out supplierId) ||
                    supplierId <= 0)
                {
                    ShowValidationMessage(
                        "Invalid Supplier ID."
                    );

                    return;
                }

                if (UpdateSupplier(supplierId))
                {
                    Response.Redirect(
                        "Suppliers.aspx?msg=updated"
                    );
                }
                else
                {
                    ShowValidationMessage(
                        "Supplier could not be updated."
                    );
                }
            }
        }
        catch (Exception ex)
        {
            ShowValidationMessage(
                "Unable to save supplier: "
                + ex.Message
            );
        }
    }


    // =========================================================
    // INSERT SUPPLIER
    // =========================================================

    private void InsertSupplier()
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string qry = @"
            INSERT INTO tbl_Suppliers
            (
                SupplierCode,
                CompanyName,
                Phone,
                ContactPerson,
                ContactPersonNo,
                Email,
                Country,
                State,
                City,
                PinCode,
                Address,
                GSTIN,
                PAN,
                BankName,
                AccountHolderName,
                AccountType,
                AccountNumber,
                IFSCCode,
                CreatedDate,
                IsActive
            )
            VALUES
            (
                @SupplierCode,
                @CompanyName,
                @Phone,
                @ContactPerson,
                @ContactPersonNo,
                @Email,
                @Country,
                @State,
                @City,
                @PinCode,
                @Address,
                @GSTIN,
                @PAN,
                @BankName,
                @AccountHolderName,
                @AccountType,
                @AccountNumber,
                @IFSCCode,
                GETDATE(),
                1
            )";

            using (SqlCommand cmd =
                new SqlCommand(qry, conn))
            {
                AddSupplierParameters(cmd);

                conn.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                if (rowsAffected <= 0)
                {
                    throw new Exception(
                        "Supplier was not inserted."
                    );
                }
            }
        }
    }


    // =========================================================
    // UPDATE SUPPLIER
    // =========================================================

    private bool UpdateSupplier(
        int supplierId)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string qry = @"
            UPDATE tbl_Suppliers
            SET
                CompanyName = @CompanyName,
                Phone = @Phone,
                ContactPerson = @ContactPerson,
                ContactPersonNo = @ContactPersonNo,
                Email = @Email,
                Country = @Country,
                State = @State,
                City = @City,
                PinCode = @PinCode,
                Address = @Address,
                GSTIN = @GSTIN,
                PAN = @PAN,
                BankName = @BankName,
                AccountHolderName = @AccountHolderName,
                AccountType = @AccountType,
                AccountNumber = @AccountNumber,
                IFSCCode = @IFSCCode
            WHERE SupplierID = @SupplierID";

            using (SqlCommand cmd =
                new SqlCommand(qry, conn))
            {
                AddSupplierParameters(cmd);

                cmd.Parameters.Add(
                    "@SupplierID",
                    SqlDbType.Int
                ).Value = supplierId;

                conn.Open();

                int rowsAffected =
                    cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }


    // =========================================================
    // COMMON SUPPLIER PARAMETERS
    // =========================================================

    private void AddSupplierParameters(
        SqlCommand cmd)
    {
        cmd.Parameters.Add(
            "@SupplierCode",
            SqlDbType.VarChar,
            20
        ).Value =
            txtSupplierCode.Text.Trim();

        cmd.Parameters.Add(
            "@CompanyName",
            SqlDbType.VarChar,
            150
        ).Value =
            txtCompanyName.Text.Trim();

        cmd.Parameters.Add(
            "@Phone",
            SqlDbType.VarChar,
            20
        ).Value =
            txtWorkPhone.Text.Trim();

        cmd.Parameters.Add(
            "@ContactPerson",
            SqlDbType.VarChar,
            100
        ).Value =
            ToDbValue(
                txtContactName.Text
            );

        cmd.Parameters.Add(
            "@ContactPersonNo",
            SqlDbType.VarChar,
            20
        ).Value =
            ToDbValue(
                txtContMobile.Text
            );

        cmd.Parameters.Add(
            "@Email",
            SqlDbType.VarChar,
            150
        ).Value =
            ToDbValue(
                txtEmail.Text
            );

        cmd.Parameters.Add(
            "@Country",
            SqlDbType.VarChar,
            100
        ).Value =
            ToDbValue(
                ddlCountry.SelectedValue
            );

        cmd.Parameters.Add(
            "@State",
            SqlDbType.VarChar,
            100
        ).Value =
            ToDbValue(
                txtState.Text
            );

        cmd.Parameters.Add(
            "@City",
            SqlDbType.VarChar,
            100
        ).Value =
            ToDbValue(
                txtCity.Text
            );

        cmd.Parameters.Add(
            "@PinCode",
            SqlDbType.VarChar,
            10
        ).Value =
            ToDbValue(
                txtPincode.Text
            );

        cmd.Parameters.Add(
            "@Address",
            SqlDbType.VarChar,
            300
        ).Value =
            ToDbValue(
                txtAddress.Text
            );

        cmd.Parameters.Add(
            "@GSTIN",
            SqlDbType.VarChar,
            20
        ).Value =
            ToDbValue(
                txtGstIn.Text.ToUpper()
            );

        cmd.Parameters.Add(
            "@PAN",
            SqlDbType.VarChar,
            20
        ).Value =
            ToDbValue(
                txtPan.Text.ToUpper()
            );

        cmd.Parameters.Add(
            "@BankName",
            SqlDbType.VarChar,
            150
        ).Value =
            ToDbValue(
                txtBankName.Text
            );

        cmd.Parameters.Add(
            "@AccountHolderName",
            SqlDbType.VarChar,
            150
        ).Value =
            ToDbValue(
                txtAccountHolder.Text
            );

        cmd.Parameters.Add(
            "@AccountType",
            SqlDbType.VarChar,
            50
        ).Value =
            ToDbValue(
                ddlAccountType.SelectedValue
            );

        cmd.Parameters.Add(
            "@AccountNumber",
            SqlDbType.VarChar,
            50
        ).Value =
            ToDbValue(
                txtAccountNumber.Text
            );

        cmd.Parameters.Add(
            "@IFSCCode",
            SqlDbType.VarChar,
            20
        ).Value =
            ToDbValue(
                txtIfscCode.Text.ToUpper()
            );
    }


    // =========================================================
    // VALIDATION
    // =========================================================

    private string ValidateSupplier()
    {
        string companyName =
            txtCompanyName.Text.Trim();

        string email =
            txtEmail.Text.Trim();

        string phone =
            txtWorkPhone.Text.Trim();

        string contactMobile =
            txtContMobile.Text.Trim();

        string gstin =
            txtGstIn.Text.Trim().ToUpper();

        string pan =
            txtPan.Text.Trim().ToUpper();

        string pinCode =
            txtPincode.Text.Trim();

        string ifsc =
            txtIfscCode.Text.Trim().ToUpper();


        // COMPANY
        if (string.IsNullOrWhiteSpace(
            companyName))
        {
            return "Company Name is required.";
        }


        // COMPANY PHONE
        if (string.IsNullOrWhiteSpace(
            phone))
        {
            return "Company Phone Number is required.";
        }

        if (!Regex.IsMatch(
            phone,
            @"^[6-9][0-9]{9}$"))
        {
            return "Enter a valid Company Phone Number.";
        }


        // EMAIL
        if (!string.IsNullOrWhiteSpace(
            email))
        {
            if (!Regex.IsMatch(
                email,
                @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
            {
                return "Enter a valid Email Address.";
            }
        }


        // CONTACT MOBILE
        if (!string.IsNullOrWhiteSpace(
            contactMobile))
        {
            if (!Regex.IsMatch(
                contactMobile,
                @"^[6-9][0-9]{9}$"))
            {
                return "Enter a valid Contact Person Number.";
            }
        }


        // PIN CODE
        if (!string.IsNullOrWhiteSpace(
            pinCode))
        {
            if (!Regex.IsMatch(
                pinCode,
                @"^[0-9]{6}$"))
            {
                return "Enter a valid 6-digit PIN Code.";
            }
        }


        // GSTIN
        if (!string.IsNullOrWhiteSpace(
            gstin))
        {
            if (!Regex.IsMatch(
                gstin,
                @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$"))
            {
                return "Please enter a valid GSTIN.";
            }
        }


        // PAN
        if (!string.IsNullOrWhiteSpace(
            pan))
        {
            if (!Regex.IsMatch(
                pan,
                @"^[A-Z]{5}[0-9]{4}[A-Z]$"))
            {
                return "Please enter a valid PAN.";
            }
        }


        // IFSC
        if (!string.IsNullOrWhiteSpace(
            ifsc))
        {
            if (!Regex.IsMatch(
                ifsc,
                @"^[A-Z]{4}0[A-Z0-9]{6}$"))
            {
                return "Please enter a valid IFSC Code.";
            }
        }


        return "";
    }


    // =========================================================
    // DUPLICATE CHECK
    // =========================================================

    private string CheckDuplicateSupplier()
    {
        string connStr =
            Connection.getConnectionString();

        int supplierId = 0;

        if (!string.IsNullOrEmpty(
            hfSupplierID.Value))
        {
            int.TryParse(
                hfSupplierID.Value,
                out supplierId
            );
        }


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string qry = @"
            SELECT TOP 1
                SupplierCode,
                CompanyName,
                Phone,
                ContactPersonNo,
                Email,
                GSTIN,
                PAN
            FROM tbl_Suppliers
            WHERE SupplierID <> @SupplierID
            AND
            (
                UPPER(SupplierCode) = UPPER(@SupplierCode)
                OR UPPER(CompanyName) = UPPER(@CompanyName)
                OR Phone = @Phone
                OR
                (
                    @ContactPersonNo <> ''
                    AND ContactPersonNo = @ContactPersonNo
                )
                OR
                (
                    @Email <> ''
                    AND UPPER(Email) = UPPER(@Email)
                )
                OR
                (
                    @GSTIN <> ''
                    AND UPPER(GSTIN) = UPPER(@GSTIN)
                )
                OR
                (
                    @PAN <> ''
                    AND UPPER(PAN) = UPPER(@PAN)
                )
            )";


            using (SqlCommand cmd =
                new SqlCommand(qry, conn))
            {
                cmd.Parameters.Add(
                    "@SupplierID",
                    SqlDbType.Int
                ).Value = supplierId;

                cmd.Parameters.Add(
                    "@SupplierCode",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    txtSupplierCode.Text.Trim();

                cmd.Parameters.Add(
                    "@CompanyName",
                    SqlDbType.VarChar,
                    150
                ).Value =
                    txtCompanyName.Text.Trim();

                cmd.Parameters.Add(
                    "@Phone",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    txtWorkPhone.Text.Trim();

                cmd.Parameters.Add(
                    "@ContactPersonNo",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    txtContMobile.Text.Trim();

                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    150
                ).Value =
                    txtEmail.Text.Trim();

                cmd.Parameters.Add(
                    "@GSTIN",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    txtGstIn.Text.Trim().ToUpper();

                cmd.Parameters.Add(
                    "@PAN",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    txtPan.Text.Trim().ToUpper();


                conn.Open();

                using (SqlDataReader dr =
                    cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (
                            string.Equals(
                                dr["SupplierCode"].ToString(),
                                txtSupplierCode.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "Supplier Code already exists.";
                        }


                        if (
                            string.Equals(
                                dr["CompanyName"].ToString(),
                                txtCompanyName.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "Company Name already exists.";
                        }


                        if (
                            dr["Phone"].ToString() ==
                            txtWorkPhone.Text.Trim())
                        {
                            return "Company Phone Number already exists.";
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                txtContMobile.Text) &&
                            dr["ContactPersonNo"].ToString() ==
                            txtContMobile.Text.Trim())
                        {
                            return "Contact Person Number already exists.";
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                txtEmail.Text) &&
                            string.Equals(
                                dr["Email"].ToString(),
                                txtEmail.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "Email already exists.";
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                txtGstIn.Text) &&
                            string.Equals(
                                dr["GSTIN"].ToString(),
                                txtGstIn.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "GSTIN already exists.";
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                txtPan.Text) &&
                            string.Equals(
                                dr["PAN"].ToString(),
                                txtPan.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return "PAN already exists.";
                        }
                    }
                }
            }
        }

        return "";
    }


    // =========================================================
    // CANCEL
    // =========================================================

    protected void btnCancel_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "Suppliers.aspx"
        );
    }


    // =========================================================
    // COUNTRY DROPDOWN SAFETY
    // =========================================================

    private void SetCountryValue(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (ddlCountry.Items.FindByValue(
            value) != null)
        {
            ddlCountry.SelectedValue =
                value;
        }
    }


    // =========================================================
    // ACCOUNT TYPE DROPDOWN SAFETY
    // =========================================================

    private void SetAccountTypeValue(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (ddlAccountType.Items.FindByValue(
            value) != null)
        {
            ddlAccountType.SelectedValue =
                value;
        }
    }


    // =========================================================
    // DATABASE VALUE HELPER
    // =========================================================

    private string GetDbValue(
        SqlDataReader reader,
        string columnName)
    {
        if (reader[columnName] == DBNull.Value)
        {
            return "";
        }

        return reader[columnName]
            .ToString()
            .Trim();
    }


    // =========================================================
    // NULL / EMPTY VALUE HELPER
    // =========================================================

    private object ToDbValue(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DBNull.Value;
        }

        return value.Trim();
    }


    // =========================================================
    // VALIDATION MESSAGE
    // =========================================================

    private void ShowValidationMessage(
        string message)
    {
        divMessage.Visible = true;

        lblMessage.Text =
            message;
    }


}
