using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class Pages_SupplierDetails : System.Web.UI.Page
{
    protected void Page_Load(
    object sender,
    EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
            return;
        }


    if (!IsPostBack)
        {
            LoadSupplierDetails();
        }
    }


    // =========================================================
    // LOAD SUPPLIER DETAILS
    // =========================================================

    private void LoadSupplierDetails()
    {
        int supplierId;

        if (!int.TryParse(
            Request.QueryString["SupplierID"],
            out supplierId) ||
            supplierId <= 0)
        {
            ShowError(
                "Invalid supplier ID."
            );

            return;
        }


        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
            SELECT
                SupplierID,
                SupplierCode,
                CompanyName,
                Phone,
                CreatedDate,
                IsActive,
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


            using (SqlCommand cmd =
                new SqlCommand(
                    query,
                    conn))
            {
                cmd.Parameters.Add(
                    "@SupplierID",
                    SqlDbType.Int
                ).Value =
                    supplierId;


                try
                {
                    conn.Open();


                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateSupplierDetails(
                                reader
                            );

                            pnlSupplier.Visible =
                                true;

                            pnlError.Visible =
                                false;
                        }
                        else
                        {
                            ShowError(
                                "Supplier record was not found."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to load supplier details: "
                        + ex.Message
                    );
                }
            }
        }
    }


    // =========================================================
    // POPULATE DETAILS
    // =========================================================

    private void PopulateSupplierDetails(
        SqlDataReader reader)
    {
        lblSupplierID.Text =
            GetValue(
                reader,
                "SupplierID"
            );

        lblSupplierCode.Text =
            GetValue(
                reader,
                "SupplierCode"
            );

        lblCompanyName.Text =
            GetValue(
                reader,
                "CompanyName"
            );

        lblContactPerson.Text =
            GetValue(
                reader,
                "ContactPerson"
            );

        lblContactPersonNo.Text =
            GetValue(
                reader,
                "ContactPersonNo"
            );

        lblPhone.Text =
            GetValue(
                reader,
                "Phone"
            );

        lblEmail.Text =
            GetValue(
                reader,
                "Email"
            );

        lblCountry.Text =
            GetValue(
                reader,
                "Country"
            );

        lblState.Text =
            GetValue(
                reader,
                "State"
            );

        lblCity.Text =
            GetValue(
                reader,
                "City"
            );

        lblPinCode.Text =
            GetValue(
                reader,
                "PinCode"
            );

        lblAddress.Text =
            GetValue(
                reader,
                "Address"
            );

        lblGSTIN.Text =
            GetValue(
                reader,
                "GSTIN"
            );

        lblPAN.Text =
            GetValue(
                reader,
                "PAN"
            );

        lblBankName.Text =
            GetValue(
                reader,
                "BankName"
            );

        lblAccountHolderName.Text =
            GetValue(
                reader,
                "AccountHolderName"
            );

        lblAccountType.Text =
            GetValue(
                reader,
                "AccountType"
            );

        lblAccountNumber.Text =
            GetValue(
                reader,
                "AccountNumber"
            );

        lblIFSCCode.Text =
            GetValue(
                reader,
                "IFSCCode"
            );


        // CREATED DATE
        if (reader["CreatedDate"] != DBNull.Value)
        {
            DateTime createdDate =
                Convert.ToDateTime(
                    reader["CreatedDate"]
                );

            lblCreatedDate.Text =
                createdDate.ToString(
                    "dd MMM yyyy, hh:mm tt"
                );
        }
        else
        {
            lblCreatedDate.Text =
                "-";
        }


        // STATUS
        bool isActive =
            reader["IsActive"] != DBNull.Value &&
            Convert.ToBoolean(
                reader["IsActive"]
            );


        if (isActive)
        {
            lblStatus.Text =
                "Active";

            lblStatus.CssClass =
                "badge bg-label-success supplier-status";
        }
        else
        {
            lblStatus.Text =
                "Inactive";

            lblStatus.CssClass =
                "badge bg-label-secondary supplier-status";
        }
    }


    // =========================================================
    // SAFE DATABASE VALUE
    // =========================================================

    private string GetValue(
        SqlDataReader reader,
        string columnName)
    {
        if (reader[columnName] == DBNull.Value)
        {
            return "-";
        }

        string value =
            reader[columnName]
                .ToString()
                .Trim();

        return string.IsNullOrWhiteSpace(value)
            ? "-"
            : value;
    }


    // =========================================================
    // EDIT SUPPLIER
    // =========================================================

    protected void btnEdit_Click(
        object sender,
        EventArgs e)
    {
        int supplierId;

        if (!int.TryParse(
            Request.QueryString["SupplierID"],
            out supplierId) ||
            supplierId <= 0)
        {
            ShowError(
                "Invalid supplier ID."
            );

            return;
        }


        Response.Redirect(
            "SupplierForm.aspx?SupplierID="
            + supplierId
        );
    }


    // =========================================================
    // BACK TO SUPPLIER LIST
    // =========================================================

    protected void btnBack_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "Suppliers.aspx"
        );
    }


    // =========================================================
    // ERROR MESSAGE
    // =========================================================

    private void ShowError(
        string message)
    {
        pnlSupplier.Visible =
            false;

        pnlError.Visible =
            true;

        lblError.Text =
            message;
    }


}
