using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Suppliers : System.Web.UI.Page
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
            BindGrid();

            string message =
                Request.QueryString["msg"];

            switch (message)
            {
                case "saved":

                    ShowToast(
                        "Supplier added successfully.",
                        "success"
                    );

                    break;

                case "updated":

                    ShowToast(
                        "Supplier updated successfully.",
                        "success"
                    );

                    break;
            }
        }

        if (Session["ToastMessage"] != null)
        {
            string toastMessage =
                Session["ToastMessage"].ToString();

            string toastType =
                Session["ToastType"] != null
                    ? Session["ToastType"].ToString()
                    : "info";

            ShowToast(
                toastMessage,
                toastType
            );

            Session.Remove("ToastMessage");
            Session.Remove("ToastType");
        }
    }


    // =========================================================
    // LOAD SUPPLIER GRID
    // =========================================================

    private void BindGrid(
        string searchText = "")
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string loadQry = @"
            SELECT
                SupplierID,
                SupplierCode,
                CompanyName,
                ContactPerson,
                Phone,
                City,
                State,
                IsActive
            FROM tbl_Suppliers
            WHERE
                CompanyName LIKE @Search
                OR SupplierCode LIKE @Search
                OR Phone LIKE @Search
                OR ISNULL(ContactPerson, '') LIKE @Search
                OR ISNULL(Email, '') LIKE @Search
                OR ISNULL(City, '') LIKE @Search
                OR ISNULL(State, '') LIKE @Search
            ORDER BY SupplierID DESC";


            using (SqlCommand cmd =
                new SqlCommand(
                    loadQry,
                    conn))
            {
                cmd.Parameters.Add(
                    "@Search",
                    SqlDbType.VarChar,
                    200
                ).Value =
                    "%" + searchText.Trim() + "%";


                using (SqlDataAdapter sda =
                    new SqlDataAdapter(cmd))
                {
                    DataTable dt =
                        new DataTable();

                    sda.Fill(dt);

                    gvSuppliers.DataSource =
                        dt;

                    gvSuppliers.DataBind();
                }
            }
        }
    }


    // =========================================================
    // LOCATION
    // =========================================================

    public string GetLocation(
        object city,
        object state)
    {
        string cityText =
            city == DBNull.Value
                ? ""
                : city.ToString().Trim();

        string stateText =
            state == DBNull.Value
                ? ""
                : state.ToString().Trim();

        if (cityText == "" &&
            stateText == "")
        {
            return "-";
        }

        if (cityText == "")
        {
            return stateText;
        }

        if (stateText == "")
        {
            return cityText;
        }

        return cityText +
            ", " +
            stateText;
    }


    // =========================================================
    // SEARCH
    // =========================================================

    protected void txtSearch_TextChanged(
        object sender,
        EventArgs e)
    {
        gvSuppliers.PageIndex = 0;

        BindGrid(
            txtSearch.Text.Trim()
        );
    }


    // =========================================================
    // ADD SUPPLIER
    // =========================================================

    protected void btnAddSupplier_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "SupplierForm.aspx"
        );
    }


    // =========================================================
    // GRID COMMANDS
    // =========================================================

    protected void gvSuppliers_RowCommand1(
        object sender,
        GridViewCommandEventArgs e)
    {
        int supplierId;

        if (!int.TryParse(
            e.CommandArgument.ToString(),
            out supplierId))
        {
            ShowToast(
                "Invalid Supplier ID.",
                "danger"
            );

            return;
        }


        // VIEW
        if (e.CommandName == "ViewSupplier")
        {
            Response.Redirect(
                "SupplierDetails.aspx?SupplierID="
                + supplierId
            );

            return;
        }


        // EDIT
        if (e.CommandName == "EditSupplier")
        {
            Response.Redirect(
                "SupplierForm.aspx?SupplierID="
                + supplierId
            );

            return;
        }


        // TOGGLE STATUS
        if (e.CommandName == "ToggleStatus")
        {
            ToggleSupplierStatus(
                supplierId
            );

            BindGrid(
                txtSearch.Text.Trim()
            );

            return;
        }
    }


    // =========================================================
    // TOGGLE SUPPLIER STATUS
    // =========================================================

    private void ToggleSupplierStatus(
        int supplierId)
    {
        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
            UPDATE tbl_Suppliers
            SET IsActive =
                CASE
                    WHEN ISNULL(IsActive, 0) = 1
                    THEN 0
                    ELSE 1
                END
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

                    int rowsAffected =
                        cmd.ExecuteNonQuery();


                    if (rowsAffected > 0)
                    {
                        ShowToast(
                            "Supplier status updated successfully.",
                            "success"
                        );
                    }
                    else
                    {
                        ShowToast(
                            "Supplier could not be found.",
                            "danger"
                        );
                    }
                }
                catch (Exception ex)
                {
                    ShowToast(
                        "Unable to update supplier status: "
                        + ex.Message,
                        "danger"
                    );
                }
            }
        }
    }


    // =========================================================
    // PAGINATION
    // =========================================================

    protected void gvSuppliers_PageIndexChanging(
        object sender,
        GridViewPageEventArgs e)
    {
        gvSuppliers.PageIndex =
            e.NewPageIndex;

        BindGrid(
            txtSearch.Text.Trim()
        );
    }


    // =========================================================
    // TOAST
    // =========================================================

    private void ShowToast(
        string message,
        string type)
    {
        lblToast.Text =
            message;


        liveToast.Attributes["class"] =
            "toast shadow-lg border-0";


        switch (
            type.ToLower())
        {
            case "success":

                liveToast.Attributes["class"] +=
                    " bg-success text-white";

                break;


            case "danger":
            case "error":

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


        string safeScript = @"
        setTimeout(function () {

            var toastEl =
                document.getElementById('" +
                    liveToast.ClientID +
                    @"');

            if (toastEl) {

                var bsToast =
                    new bootstrap.Toast(
                        toastEl,
                        {
                            autohide: true,
                            delay: 4000
                        }
                    );

                bsToast.show();
            }

        }, 50);";


        ScriptManager.RegisterStartupScript(
            this.Page,
            this.Page.GetType(),
            "showToast_" + Guid.NewGuid(),
            safeScript,
            true
        );
    }


}
