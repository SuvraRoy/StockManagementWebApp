using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Warehouses : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!IsPostBack)
        {
            BindGrid();

            string message = Request.QueryString["msg"];

            switch (message)
            {
                case "saved":
                    ShowToast("Warehouse added successfully.", "success");
                    break;

                case "updated":
                    ShowToast("Warehouse updated successfully.", "success");
                    break;
            }
        }
    }

    private void BindGrid(string searchText = "")
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    WarehouseID,
                    WarehouseCode,
                    WarehouseName,
                    WarehouseType,
                    Phone,
                    Email,
                    City,
                    State,
                    IsActive,
                    IsMainWarehouse
                FROM tbl_Warehouses
                WHERE
                    WarehouseName LIKE @Search
                    OR WarehouseCode LIKE @Search
                    OR Phone LIKE @Search
                    OR Email LIKE @Search
                ORDER BY WarehouseID DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Search",
                    "%" + searchText.Trim() + "%");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    gvWarehouses.DataSource = dt;
                    gvWarehouses.DataBind();
                }
            }
        }
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        gvWarehouses.PageIndex = 0;
        BindGrid(txtSearch.Text.Trim());
    }

    protected void btnAddWarehouse_Click(object sender, EventArgs e)
    {
        Response.Redirect("WarehouseForm.aspx");
    }

    protected void gvWarehouses_RowCommand(
        object sender,
        GridViewCommandEventArgs e)
    {
        int warehouseID;

        if (!int.TryParse(
            Convert.ToString(e.CommandArgument),
            out warehouseID))
        {
            return;
        }

        if (e.CommandName == "ViewWarehouse")
        {
            Response.Redirect(
                "WarehouseDetails.aspx?WarehouseID=" + warehouseID);
        }

        else if (e.CommandName == "EditWarehouse")
        {
            // Every warehouse uses the same edit page.
            Response.Redirect(
                "WarehouseForm.aspx?WarehouseID=" + warehouseID);
        }

        else if (e.CommandName == "ToggleStatus")
        {
            ToggleWarehouseStatus(warehouseID);
            BindGrid(txtSearch.Text.Trim());
        }
    }

    private void ToggleWarehouseStatus(int warehouseID)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = @"
                UPDATE tbl_Warehouses
                SET IsActive =
                    CASE
                        WHEN IsActive = 1 THEN 0
                        ELSE 1
                    END
                WHERE WarehouseID = @WarehouseID
                AND IsMainWarehouse = 0";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID);

                try
                {
                    conn.Open();

                    int rowsAffected =
                        cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowToast(
                            "Warehouse status updated successfully.",
                            "success");
                    }
                    else
                    {
                        ShowToast(
                            "Main Warehouse cannot be deactivated.",
                            "warning");
                    }
                }
                catch (Exception ex)
                {
                    ShowToast(ex.Message, "danger");
                }
            }
        }
    }

    protected string GetLocation(object city, object state)
    {
        string cityName =
            city == DBNull.Value
                ? ""
                : city.ToString().Trim();

        string stateName =
            state == DBNull.Value
                ? ""
                : state.ToString().Trim();

        if (cityName != "" && stateName != "")
            return cityName + ", " + stateName;

        if (cityName != "")
            return cityName;

        if (stateName != "")
            return stateName;

        return "-";
    }

    protected void gvWarehouses_PageIndexChanging(
        object sender,
        GridViewPageEventArgs e)
    {
        gvWarehouses.PageIndex = e.NewPageIndex;

        BindGrid(txtSearch.Text.Trim());
    }

    private void ShowToast(string message, string type)
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

                    var bsToast =
                        new bootstrap.Toast(
                            toastEl,
                            {
                                autohide: true,
                                delay: 4000
                            });

                    bsToast.show();
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