using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;


public partial class Pages_SupplierForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }

        if (!IsPostBack)
        {
            if (Request.QueryString["SupplierID"] != null)
            {
                // Edit Mode

                int supplierId = Convert.ToInt32(Request.QueryString["SupplierID"]);

                LoadSupplier(supplierId);

                lblPageTitle.InnerText = "Edit Supplier";
                btnSave.Text = "Update Supplier";
            }
            else
            {
                // Add Mode

                lblPageTitle.InnerText = "Add Supplier";

                btnSave.Text = "Save Supplier";

                GenerateSupplierCode();
            }
        }
    }

    private void GenerateSupplierCode()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
            SELECT ISNULL(MAX(SupplierID),0) + 1
            FROM tbl_Suppliers";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                conn.Open();

                int nextId = Convert.ToInt32(cmd.ExecuteScalar());

                txtSupplierCode.Text = "SUP" + nextId.ToString("D6");
            }
        }
    }

    private void LoadSupplier(int supplierId)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
        SELECT *
        FROM tbl_Suppliers
        WHERE SupplierID = @SupplierID";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@SupplierID", supplierId);

                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfSupplierID.Value = supplierId.ToString();

                    txtSupplierCode.Text = dr["SupplierCode"].ToString();

                    txtCompanyName.Text = dr["CompanyName"].ToString();

                    txtWorkPhone.Text = dr["Phone"].ToString();

                    txtContactName.Text = dr["ContactPerson"].ToString();

                    txtContMobile.Text = dr["ContactPersonNo"].ToString();

                    txtEmail.Text = dr["Email"].ToString();

                    ddlCountry.SelectedValue = dr["Country"].ToString();

                    txtState.Text = dr["State"].ToString();

                    txtCity.Text = dr["City"].ToString();

                    txtPincode.Text = dr["PinCode"].ToString();

                    txtAddress.Text = dr["Address"].ToString();

                    txtGstIn.Text = dr["GSTIN"].ToString();

                    txtPan.Text = dr["PAN"].ToString();

                    txtBankName.Text = dr["BankName"].ToString();

                    txtAccountHolder.Text = dr["AccountHolderName"].ToString();

                    ddlAccountType.SelectedValue = dr["AccountType"].ToString();

                    txtAccountNumber.Text = dr["AccountNumber"].ToString();

                    txtIfscCode.Text = dr["IFSCCode"].ToString();
                }
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Step 1: hide any previous message:
        divMessage.Visible = false;

        // Step 2: Validate Form
        string validationMessage = ValidateSupplier();

        if (!string.IsNullOrEmpty(validationMessage))
        {
            //ShowToast(validationMessage, "danger");
            ShowValidationMessage(validationMessage); 
            return;
        }
        string duplicateError = CheckDuplicateSupplier();

        if (duplicateError != "")
        {
            ShowValidationMessage(duplicateError);
            return;
        }

        // Step 3: Save or Update
        if (string.IsNullOrEmpty(hfSupplierID.Value))
        {
            InsertSupplier();
            Response.Redirect("Suppliers.aspx?msg=saved");
        }
        else
        {
            UpdateSupplier();
            Response.Redirect("Suppliers.aspx?msg=updated");
        }
    }


    private void AddSupplierParameters(SqlCommand cmd)
    {
        cmd.Parameters.AddWithValue("@SupplierCode", txtSupplierCode.Text.Trim());
        cmd.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text.Trim());
        cmd.Parameters.AddWithValue("@Phone", txtWorkPhone.Text.Trim());

        cmd.Parameters.AddWithValue("@ContactPerson", txtContactName.Text.Trim());
        cmd.Parameters.AddWithValue("@ContactPersonNo", txtContMobile.Text.Trim());

        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

        cmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue);
        cmd.Parameters.AddWithValue("@State", txtState.Text.Trim());
        cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim());
        cmd.Parameters.AddWithValue("@PinCode", txtPincode.Text.Trim());

        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

        cmd.Parameters.AddWithValue("@GSTIN", txtGstIn.Text.Trim());
        cmd.Parameters.AddWithValue("@PAN", txtPan.Text.Trim());

        cmd.Parameters.AddWithValue("@BankName", txtBankName.Text.Trim());
        cmd.Parameters.AddWithValue("@AccountHolderName", txtAccountHolder.Text.Trim());
        cmd.Parameters.AddWithValue("@AccountType", ddlAccountType.SelectedValue);
        cmd.Parameters.AddWithValue("@AccountNumber", txtAccountNumber.Text.Trim());
        cmd.Parameters.AddWithValue("@IFSCCode", txtIfscCode.Text.Trim());
    }

    private void InsertSupplier()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"INSERT INTO tbl_Suppliers( SupplierCode, CompanyName,Phone,ContactPerson,ContactPersonNo, Email, Country, State, City, PinCode, Address, GSTIN, PAN, BankName, AccountHolderName, AccountType, AccountNumber, IFSCCode, CreatedDate, IsActive
)
VALUES
( @SupplierCode,
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
    @CreatedDate,
    1
)";
            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                AddSupplierParameters(cmd);

                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }


    private void UpdateSupplier()
    {

        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
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

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                AddSupplierParameters(cmd);

                cmd.Parameters.AddWithValue(
                    "@SupplierID",
                    Convert.ToInt32(hfSupplierID.Value));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }


    private void ShowToast(string message, string type)
    {
        lblToast.Text = message;

        liveToast.Attributes["class"] = "toast shadow-lg";

        switch (type)
        {
            case "success":
                liveToast.Attributes["class"] += " text-bg-success";
                break;

            case "danger":
                liveToast.Attributes["class"] += " text-bg-danger";
                break;

            case "warning":
                liveToast.Attributes["class"] += " text-bg-warning";
                break;

            case "info":
                liveToast.Attributes["class"] += " text-bg-info";
                break;
        }

        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "showToast",
            @"
        var toastElement = document.getElementById('liveToast');
        var toast = new bootstrap.Toast(toastElement);
        toast.show();
        ",
            true);
    }



    //protected void btnReset_Click(object sender, EventArgs e)
    //{
    //    if (string.IsNullOrEmpty(hfSupplierID.Value))
    //    {
    //        ClearForm();

    //        //GenerateSupplierCode();
    //        Response.Write(txtCompanyName.Text == "" ? "Company Cleared" : "Company NOT Cleared");
    //    }
    //    else
    //    {
    //        LoadSupplier(Convert.ToInt32(hfSupplierID.Value));
    //    }
    //}

    //private void ClearForm()
    //{
    //    txtCompanyName.Text = "";
    //}
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Suppliers.aspx");
    }

    private string ValidateSupplier()
    {
        if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            return "Company Name is required.";

        //if (string.IsNullOrWhiteSpace(txtContactName.Text))
        //    return "Contact Person is required.";

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
            return "Email is required.";

        if (!Regex.IsMatch(txtEmail.Text, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
            return "Enter Proper Email";

        if (string.IsNullOrWhiteSpace(txtWorkPhone.Text))
            return "Company's Phone Number is required.";

        if (!Regex.IsMatch(txtWorkPhone.Text, @"^[6-9][0-9]{9}$"))
        {
            return "Enter a valid Work Phone number.";
        }

        // GSTIN Validation
        string gstin = txtGstIn.Text.Trim().ToUpper();

        if (!string.IsNullOrWhiteSpace(gstin))
        {
            if (!Regex.IsMatch(gstin, @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$"))
            {
                return "Please enter a valid GSTIN.";
            }
        }

        // PAN Validation
        string pan = txtPan.Text.Trim().ToUpper();

        if (!string.IsNullOrWhiteSpace(pan))
        {
            if (!Regex.IsMatch(pan, @"^[A-Z]{5}[0-9]{4}[A-Z]$"))
            {
                return "Please enter a valid PAN.";
            }
        }

        if (!string.IsNullOrWhiteSpace(txtContMobile.Text))
        {
            if (!Regex.IsMatch(txtContMobile.Text, @"^[6-9][0-9]{9}$"))
            {
                return "Enter a valid Contact Mobile number.";
            }
        }

        //if (ddlCountry.SelectedIndex == 0)
        //    return "Please select a Country.";

        return "";
    }


    private string CheckDuplicateSupplier()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
SELECT TOP 1
    Phone,
    ContactPersonNo,
    Email,
    GSTIN
FROM tbl_Suppliers
WHERE SupplierID <> @SupplierID
AND
(
    Phone = @Phone
    OR
    (@ContactPersonNo <> '' AND ContactPersonNo = @ContactPersonNo)
    OR
    (@Email <> '' AND Email = @Email)
    OR
    (@GSTIN <> '' AND GSTIN = @GSTIN)
)";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@Phone", txtWorkPhone.Text.Trim());

                cmd.Parameters.AddWithValue("@ContactPersonNo", txtContMobile.Text.Trim());

                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                cmd.Parameters.AddWithValue("@GSTIN", txtGstIn.Text.Trim().ToUpper());

                int supplierId = 0;

                if (!string.IsNullOrEmpty(hfSupplierID.Value))
                {
                    supplierId = Convert.ToInt32(hfSupplierID.Value);
                }

                cmd.Parameters.AddWithValue("@SupplierID", supplierId);

                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (dr["Phone"].ToString() == txtWorkPhone.Text.Trim())
                        return "Work Phone already exists.";

                    if (!string.IsNullOrWhiteSpace(txtContMobile.Text) &&
                        dr["ContactPersonNo"].ToString() == txtContMobile.Text.Trim())
                        return "Contact Mobile already exists.";

                    if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                        dr["Email"].ToString().Equals(txtEmail.Text.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                        return "Email already exists.";

                    if (!string.IsNullOrWhiteSpace(txtGstIn.Text) &&
                        dr["GSTIN"].ToString().Equals(txtGstIn.Text.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                        return "GSTIN already exists.";
                }
            
        }
        }

        return "";
    }

    private void ShowValidationMessage(string message)
    {
        divMessage.Visible = true;
        lblMessage.Text = message;
    }

}