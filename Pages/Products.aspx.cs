using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class Pages_Products : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
            return;
        }


    if (!IsPostBack)
        {
            LoadProductData();
            ClearModalFields();
        }
    }


    // =========================================================
    // SEARCH
    // =========================================================

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        LoadProductData();

        pnlProductDetails.Visible = false;

        ViewState["SelectedProductID"] = null;
    }


    // =========================================================
    // ADD PRODUCT BUTTON
    // =========================================================

    protected void btnAddProduct_Click(object sender, EventArgs e)
    {
        ClearModalFields();

        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "OpenModal",
            @"
        var myModal = new bootstrap.Modal(
            document.getElementById('backDropModal')
        );
        myModal.show();
        ",
            true
        );
    }


    // =========================================================
    // SAVE PRODUCT
    // =========================================================

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string productName = txtProdName.Text.Trim();
        string sku = txtSKU.Text.Trim();

        decimal costPrice;
        int reorderLevel;


        // -----------------------------------------------------
        // VALIDATION
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(productName))
        {
            ShowToast(
                "Product Name is required",
                "danger"
            );

            return;
        }


        if (string.IsNullOrWhiteSpace(sku))
        {
            ShowToast(
                "SKU is required",
                "danger"
            );

            return;
        }


        if (!decimal.TryParse(
            txtCostPrice.Text.Trim(),
            out costPrice))
        {
            ShowToast(
                "Please enter a valid Cost Price",
                "danger"
            );

            return;
        }


        if (!int.TryParse(
            txtReorderLevel.Text.Trim(),
            out reorderLevel))
        {
            ShowToast(
                "Please enter a valid Reorder Level",
                "danger"
            );

            return;
        }


        if (costPrice <= 0)
        {
            ShowToast(
                "Cost Price must be greater than 0",
                "danger"
            );

            return;
        }


        if (reorderLevel < 0)
        {
            ShowToast(
                "Reorder Level cannot be negative",
                "danger"
            );

            return;
        }


        // -----------------------------------------------------
        // DATABASE CONNECTION
        // -----------------------------------------------------

        string connStr = Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();


            // -------------------------------------------------
            // CHECK DUPLICATE SKU
            // -------------------------------------------------

            string checkSkuQuery = @"
            SELECT COUNT(*)
            FROM tbl_Products
            WHERE SKU = @SKU";


            using (SqlCommand cmd =
                new SqlCommand(
                    checkSkuQuery,
                    conn))
            {
                cmd.Parameters.Add(
                    "@SKU",
                    SqlDbType.NVarChar,
                    100
                ).Value = sku;


                int skuCount =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );


                if (skuCount > 0)
                {
                    ShowToast(
                        "SKU already exists",
                        "danger"
                    );

                    return;
                }
            }


            // -------------------------------------------------
            // INSERT PRODUCT
            // -------------------------------------------------

            string insertQuery = @"
            INSERT INTO tbl_Products
            (
                ProductName,
                SKU,
                CostPrice,
                CurrentStock,
                ReorderLevel,
                CreatedDate,
                IsActive
            )
            VALUES
            (
                @ProductName,
                @SKU,
                @CostPrice,
                @CurrentStock,
                @ReorderLevel,
                @CreatedDate,
                @IsActive
            )";


            using (SqlCommand insertCmd =
                new SqlCommand(
                    insertQuery,
                    conn))
            {
                insertCmd.Parameters.Add(
                    "@ProductName",
                    SqlDbType.NVarChar,
                    200
                ).Value = productName;


                insertCmd.Parameters.Add(
                    "@SKU",
                    SqlDbType.NVarChar,
                    100
                ).Value = sku;


                SqlParameter costParameter =
                    insertCmd.Parameters.Add(
                        "@CostPrice",
                        SqlDbType.Decimal
                    );


                costParameter.Precision = 18;
                costParameter.Scale = 2;
                costParameter.Value = costPrice;


                insertCmd.Parameters.Add(
                    "@CurrentStock",
                    SqlDbType.Int
                ).Value = 0;


                insertCmd.Parameters.Add(
                    "@ReorderLevel",
                    SqlDbType.Int
                ).Value = reorderLevel;


                insertCmd.Parameters.Add(
                    "@CreatedDate",
                    SqlDbType.DateTime
                ).Value = DateTime.Now;


                insertCmd.Parameters.Add(
                    "@IsActive",
                    SqlDbType.Bit
                ).Value = true;


                int rowsAffected =
                    insertCmd.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    ClearModalFields();

                    LoadProductData();

                    ShowToast(
                        "Product inserted successfully",
                        "success"
                    );
                }
                else
                {
                    ShowToast(
                        "Unable to insert product",
                        "danger"
                    );
                }
            }
        }
    }


    // =========================================================
    // LOAD PRODUCT LIST
    // =========================================================

    private void LoadProductData()
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string searchTerm =
                txtSearch.Text.Trim();


            string query = @"
            SELECT
                ProductID,
                ProductName,
                SKU,
                CostPrice,
                CurrentStock,
                ReorderLevel,
                IsActive
            FROM tbl_Products
            WHERE
                @SearchTerm = ''
                OR ProductName LIKE '%' + @SearchTerm + '%'
                OR SKU LIKE '%' + @SearchTerm + '%'
            ORDER BY ProductID DESC";


            using (SqlCommand cmd =
                new SqlCommand(
                    query,
                    conn))
            {
                cmd.Parameters.Add(
                    "@SearchTerm",
                    SqlDbType.NVarChar,
                    100
                ).Value = searchTerm;


                using (SqlDataAdapter da =
                    new SqlDataAdapter(
                        cmd))
                {
                    DataTable dt =
                        new DataTable();


                    da.Fill(dt);


                    gvProducts.DataSource =
                        dt;


                    gvProducts.DataBind();
                }
            }
        }
    }


    // =========================================================
    // GRID VIEW - VIEW PRODUCT
    // =========================================================

    protected void gvProducts_RowCommand(
        object sender,
        GridViewCommandEventArgs e)
    {
        if (e.CommandName != "ViewProduct")
        {
            return;
        }


        int productID;


        if (!int.TryParse(
            e.CommandArgument.ToString(),
            out productID))
        {
            ShowToast(
                "Invalid product selected",
                "danger"
            );

            return;
        }


        ViewState["SelectedProductID"] =
            productID;


        LoadProductDetails(
            productID
        );


        pnlProductDetails.Visible =
            true;
    }


    // =========================================================
    // LOAD PRODUCT DETAILS
    // =========================================================

    private void LoadProductDetails(
        int productID)
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();


            string loadQry = @"
            SELECT
                ProductID,
                ProductName,
                SKU,
                CostPrice,
                CurrentStock,
                ReorderLevel,
                IsActive
            FROM tbl_Products
            WHERE ProductID = @ProductID";


            using (SqlCommand cmd =
                new SqlCommand(
                    loadQry,
                    conn))
            {
                cmd.Parameters.Add(
                    "@ProductID",
                    SqlDbType.Int
                ).Value = productID;


                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        pnlProductDetails.Visible =
                            false;


                        ShowToast(
                            "Product not found",
                            "danger"
                        );


                        return;
                    }


                    lblDetailName.Text =
                        reader["ProductName"]
                        .ToString();


                    lblDetailSKU.Text =
                        reader["SKU"]
                        .ToString();


                    lblDetailCost.Text =
                        Convert.ToDecimal(
                            reader["CostPrice"]
                        ).ToString("0.00");


                    lblDetailStock.Text =
                        reader["CurrentStock"]
                        .ToString();


                    lblDetailReorder.Text =
                        reader["ReorderLevel"]
                        .ToString();


                    bool isActive =
                        Convert.ToBoolean(
                            reader["IsActive"]
                        );


                    // STATUS LABEL
                    lblDetailStatus.Text =
                        isActive
                            ? "Active"
                            : "Inactive";


                    lblDetailStatus.CssClass =
                        isActive
                            ? "badge bg-label-success"
                            : "badge bg-label-secondary";


                    // ACTIVATE / DEACTIVATE BUTTON
                    btnToggleStatus.Text =
                        isActive
                            ? "Deactivate"
                            : "Activate";


                    btnToggleStatus.CssClass =
                        isActive
                            ? "btn btn-warning"
                            : "btn btn-success";
                }
            }
        }
    }


    // =========================================================
    // EDIT PRODUCT
    // =========================================================

    protected void btnEdit_Click(
        object sender,
        EventArgs e)
    {
        if (ViewState["SelectedProductID"] == null)
        {
            ShowToast(
                "Please select a product first",
                "warning"
            );

            return;
        }


        int productID =
            Convert.ToInt32(
                ViewState["SelectedProductID"]
            );


        Response.Redirect(
            "EditProduct.aspx?ProductID="
            + productID
        );
    }


    // =========================================================
    // ACTIVATE / DEACTIVATE PRODUCT
    // =========================================================

    protected void btnToggleStatus_Click(
        object sender,
        EventArgs e)
    {
        if (ViewState["SelectedProductID"] == null)
        {
            ShowToast(
                "Please select a product first",
                "warning"
            );

            return;
        }


        int productID =
            Convert.ToInt32(
                ViewState["SelectedProductID"]
            );


        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();


            string updateQuery = @"
            UPDATE tbl_Products
            SET IsActive =
                CASE
                    WHEN IsActive = 1 THEN 0
                    ELSE 1
                END
            WHERE ProductID = @ProductID";


            using (SqlCommand cmd =
                new SqlCommand(
                    updateQuery,
                    conn))
            {
                cmd.Parameters.Add(
                    "@ProductID",
                    SqlDbType.Int
                ).Value = productID;


                int rowsAffected =
                    cmd.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    LoadProductData();

                    LoadProductDetails(
                        productID
                    );


                    pnlProductDetails.Visible =
                        true;


                    ShowToast(
                        "Product status updated successfully",
                        "success"
                    );
                }
                else
                {
                    ShowToast(
                        "Unable to update product status",
                        "danger"
                    );
                }
            }
        }
    }


    // =========================================================
    // CLEAR ADD PRODUCT FORM
    // =========================================================

    private void ClearModalFields()
    {
        txtProdName.Text =
            string.Empty;


        txtSKU.Text =
            string.Empty;


        txtCostPrice.Text =
            string.Empty;


        txtReorderLevel.Text =
            string.Empty;
    }


    // =========================================================
    // SHOW TOAST
    // =========================================================

    private void ShowToast(
        string message,
        string type)
    {
        lblToast.Text =
            message;


        liveToast.Attributes["class"] =
            "toast shadow-lg";


        switch (type)
        {
            case "success":

                liveToast.Attributes["class"] +=
                    " text-bg-success";

                break;


            case "danger":

                liveToast.Attributes["class"] +=
                    " text-bg-danger";

                break;


            case "warning":

                liveToast.Attributes["class"] +=
                    " text-bg-warning";

                break;


            case "info":

                liveToast.Attributes["class"] +=
                    " text-bg-info";

                break;
        }


        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "showToast",
            @"
        var toastElement =
            document.getElementById('liveToast');

        var toast =
            new bootstrap.Toast(toastElement);

        toast.show();
        ",
            true
        );
    }


}
