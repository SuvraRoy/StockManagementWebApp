using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

public partial class Pages_WarehouseDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!IsPostBack)
        {
            string warehouseID = Request.QueryString["WarehouseID"];

            if (string.IsNullOrEmpty(warehouseID))
            {
                Response.Redirect("Warehouses.aspx");
                return;
            }

            LoadWarehouse(Convert.ToInt32(warehouseID));
        }
    }


    // ============================================
    // LOAD WAREHOUSE
    // ============================================

    private void LoadWarehouse(int warehouseID)
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
                    Country,
                    State,
                    City,
                    PinCode,
                    Address,
                    IsMainWarehouse,
                    IsActive
                FROM tbl_Warehouses
                WHERE WarehouseID = @WarehouseID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@WarehouseID", warehouseID);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string warehouseName =
                            reader["WarehouseName"].ToString();

                        string warehouseCode =
                            reader["WarehouseCode"].ToString();

                        string warehouseType =
                            reader["WarehouseType"].ToString();

                        bool isActive =
                            Convert.ToBoolean(reader["IsActive"]);

                        // --------------------------------
                        // HEADER
                        // --------------------------------

                        lblWarehouseName.Text =
                            warehouseName;

                        // Type badge
                        if (warehouseType == "Main")
                        {
                            lblWarehouseType.Text = "Main";
                            lblWarehouseType.CssClass =
                                "badge badge-main";
                        }
                        else
                        {
                            lblWarehouseType.Text = "Branch";
                            lblWarehouseType.CssClass =
                                "badge badge-branch";
                        }

                        // Status badge
                        if (isActive)
                        {
                            lblStatus.Text = "Active";
                            lblStatus.CssClass =
                                "badge badge-active";
                        }
                        else
                        {
                            lblStatus.Text = "Inactive";
                            lblStatus.CssClass =
                                "badge badge-inactive";
                        }


                        // --------------------------------
                        // BASIC INFORMATION
                        // --------------------------------

                        lblWarehouseCode.Text =
                            warehouseCode;

                        lblTypeDetail.Text =
                            warehouseType == "Main"
                                ? "Main Warehouse"
                                : "Branch Warehouse";

                        lblPhone.Text =
                            GetValue(reader["Phone"]);
                        
                        lblEmail.Text =
                            GetValue(reader["Email"]);



                        // --------------------------------
                        // ADDRESS
                        // --------------------------------

                        lblCountry.Text =
                            GetValue(reader["Country"]);

                        lblState.Text =
                            GetValue(reader["State"]);

                        lblCity.Text =
                            GetValue(reader["City"]);

                        lblPinCode.Text =
                            GetValue(reader["PinCode"]);

                        lblAddress.Text =
                            GetValue(reader["Address"]);


                        // --------------------------------
                        // STATUS
                        // --------------------------------

                        if (isActive)
                        {
                            lblStatusDetail.Text = "Active";
                            lblStatusDetail.CssClass =
                                "badge badge-active";
                        }
                        else
                        {
                            lblStatusDetail.Text = "Inactive";
                            lblStatusDetail.CssClass =
                                "badge badge-inactive";
                        }
                    }
                    else
                    {
                        Response.Redirect("Warehouses.aspx");
                    }
                }
            }
        }
    }


    // ============================================
    // HANDLE NULL / EMPTY VALUES
    // ============================================

    private string GetValue(object value)
    {
        if (value == null ||
            value == DBNull.Value ||
            string.IsNullOrWhiteSpace(value.ToString()))
        {
            return "-";
        }

        return value.ToString();
    }


    // ============================================
    // EDIT
    // ============================================

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        string warehouseID =
            Request.QueryString["WarehouseID"];

        Response.Redirect(
            "WarehouseForm.aspx?WarehouseID=" +
            warehouseID);
    }


    // ============================================
    // BACK
    // ============================================

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Warehouses.aspx");
    }
}