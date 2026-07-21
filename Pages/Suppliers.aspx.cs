using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class Pages_Suppliers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!this.IsPostBack)
        {
            this.BindGrid();
            //divForm.Visible = false;
            //divGrid.Attributes["class"] = "col-lg-12";
            string message = Request.QueryString["msg"];

            switch (message)
            {
                case "saved":
                    ShowToast("Supplier added successfully.", "success");
                    break;

                case "updated":
                    ShowToast("Supplier updated successfully.", "success");
                    break;

                case "deleted":
                    ShowToast("Supplier deleted successfully.", "success");
                    break;
            }
        }

        if (Session["ToastMessage"] != null)
        {
            ShowToast(
                Session["ToastMessage"].ToString(),
                Session["ToastType"].ToString());

            Session.Remove("ToastMessage");
            Session.Remove("ToastType");
        }
    }

    private void BindGrid(string searchText = "")
    {
        string connStr = Connection.getConnectionString();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            //conn.Open();
            string loadQry = "SELECT * FROM tbl_Suppliers  WHERE CompanyName LIKE @Search OR Phone LIKE @Search ORDER BY SupplierID DESC ";

            using (SqlCommand cmd = new SqlCommand(loadQry, conn))
            {
                cmd.Parameters.AddWithValue("@Search",
                "%" + searchText.Trim() + "%");
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    sda.Fill(dt);

                    gvSuppliers.DataSource = dt;
                    gvSuppliers.DataBind();
                }
            }
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        BindGrid(txtSearch.Text.Trim());
    }

    // Opens the side form
    protected void btnAddSupplier_Click(object sender, EventArgs e)
    {
        ClearForm();
        showForm();
    }

    // function to close/hide the form
    private void hideForm()
    {
        divForm.Visible = false;
        divGrid.Attributes["class"] = "col-lg-12";
    }

    private void showForm()
    {
        divForm.Visible = true;
        divGrid.Attributes["class"] = "col-lg-8";
    }

    protected void btnSaveSupplier_Click(object sender, EventArgs e)
    {
        // 1. Core Field Validations
        if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
        {
            ShowToast("Company Name is required.", "danger");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPhone.Text))
        {
            ShowToast("Phone number is required.", "danger");
            return;
        }

        if (!txtPhone.Text.Trim().All(char.IsDigit))
        {
            ShowToast("Phone number must contain digits only.", "danger"); // Changed from "error" to "danger"
            return;
        }

        if (txtPhone.Text.Trim().Length != 10)
        {
            ShowToast("Phone number must be exactly 10 digits.", "danger");
            return;
        }

        int currentSupplierId = 0;
        if (!string.IsNullOrEmpty(hfSupplierID.Value))
        {
            currentSupplierId = Convert.ToInt32(hfSupplierID.Value);
        }

        // 2. Database Uniqueness Check
        string validationMessage = ValidateSupplier(txtCompanyName.Text, txtPhone.Text, currentSupplierId);

        if (!string.IsNullOrEmpty(validationMessage))
        {
            ShowToast(validationMessage, "danger"); // Changed from "error" to "danger"
            return;
        }

        // 3. Execution routing
        if (string.IsNullOrEmpty(hfSupplierID.Value))
        {
            InsertSupplier();
        }
        else
        {
            UpdateSupplier();
        }

        // 4. View UI Resets
        BindGrid();
        ClearForm();
        hideForm();
    }

    private void UpdateSupplier()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
                UPDATE tbl_Suppliers
                SET CompanyName = @CompanyName,
                    Phone = @Phone
                WHERE SupplierID = @SupplierID";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@SupplierID", Convert.ToInt32(hfSupplierID.Value));

                conn.Open();
                cmd.ExecuteNonQuery();
                ShowToast("Supplier updated successfully.", "success");
            }
        }
    }

    private void InsertSupplier()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"INSERT INTO tbl_Suppliers(
                            CompanyName,
                            Phone,            
                            CreatedDate,
                            IsActive
                        )
                        VALUES
                        (
                            @CompanyName,
                            @Phone,
                            @CreatedDate,
                            1
                        )";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
                ShowToast("Supplier added successfully.", "success");
            }
        }
    }

    protected void btnCancelSupplier_Click(object sender, EventArgs e)
    {
        ClearForm();
        hideForm();
    }

    protected void btnResetData_Click(object sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        txtCompanyName.Text = "";
        txtPhone.Text = "";
        hfSupplierID.Value = "";
        btnSaveSupplier.Text = "Save Supplier";
    }

    protected void gvSuppliers_RowCommand1(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ToggleStatus")
        {
            int supplierId = Convert.ToInt32(e.CommandArgument);

            ToggleSupplierStatus(supplierId);

            //ShowToast(
            //    "Supplier status updated successfully.",
            //    "success");

            //BindGrid();
            Session["ToastMessage"] = "Supplier status updated successfully.";
            Session["ToastType"] = "success";
            Response.Redirect("~/Pages/Suppliers.aspx");
        }

        if (e.CommandName == "EditSupplier")
        {
            int supplierId = Convert.ToInt32(e.CommandArgument);

            Response.Redirect("SupplierForm.aspx?SupplierID=" + supplierId);
        }
    }

    private void LoadSupplierForEdit(int supplierId)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"SELECT SupplierID, CompanyName, Phone, Address
                           FROM tbl_Suppliers
                           WHERE SupplierID = @SupplierID";

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfSupplierID.Value = dr["SupplierID"].ToString();
                    txtCompanyName.Text = dr["CompanyName"].ToString();
                    txtPhone.Text = dr["Phone"].ToString();

                    btnSaveSupplier.Text = "Update Supplier";
                    showForm();
                }
            }
        }
    }

    private void ToggleSupplierStatus(int supplierId)
    {
        string connStr = Connection.getConnectionString();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string updateQuery = "UPDATE tbl_Suppliers SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE SupplierID = @SupplierID;";
            using (SqlCommand insertCmd = new SqlCommand(updateQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@SupplierID", supplierId);
                try
                {
                    conn.Open();
                    insertCmd.ExecuteNonQuery();
                    ShowToast("Status updated successfully.", "success");
                }
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
                //}

                catch (Exception ex)
                {
                    ShowToast(ex.Message, "danger");
                }
            }
        }
    }

    private string ValidateSupplier(string companyName, string phone, int supplierId = 0)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string qry = @"
                SELECT CompanyName, Phone
                FROM tbl_Suppliers
                WHERE (UPPER(CompanyName) = UPPER(@CompanyName)
                       OR Phone = @Phone)";

            if (supplierId > 0)
            {
                qry += " AND SupplierID <> @SupplierID";
            }

            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@CompanyName", companyName.Trim());
                cmd.Parameters.AddWithValue("@Phone", phone.Trim());

                if (supplierId > 0)
                {
                    cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                }

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (dr["CompanyName"].ToString().Equals(companyName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return "Company Name already exists.";
                    }

                    if (dr["Phone"].ToString() == phone.Trim())
                    {
                        return "Phone number already exists.";
                    }
                }
            }
        }
        return "";
    }

    private void ShowToast(string message, string type)
    {
        lblToast.Text = message;
        liveToast.Attributes["class"] = "toast shadow-lg border-0";

        // Safely assign Bootstrap 5 background color classes
        switch (type.ToLower())
        {
            case "success":
                liveToast.Attributes["class"] += " bg-success text-white";
                break;
            case "danger":
            case "error": // Capture "error" safely just in case!
                liveToast.Attributes["class"] += " bg-danger text-white";
                break;
            case "warning":
                liveToast.Attributes["class"] += " bg-warning text-dark";
                break;
            case "info":
                liveToast.Attributes["class"] += " bg-info text-white";
                break;
        }

        // DOM-Safe Script: Waits 50ms for ASP.NET HTML rendering to settle before showing
        string safeScript = @"
        setTimeout(function() {
            var toastEl = document.getElementById('" + liveToast.ClientID + @"');
            if (toastEl) {
                var bsToast = new bootstrap.Toast(toastEl, { autohide: true, delay: 4000 });
                bsToast.show();
            } else {
                console.error('Toast HTML element was not found in the DOM.');
            }
        }, 50);";

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "showToast_" + Guid.NewGuid(), safeScript, true);
    }

    protected void gvSuppliers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {      
        gvSuppliers.PageIndex = e.NewPageIndex;
        BindGrid(txtSearch.Text.Trim());
    }
}