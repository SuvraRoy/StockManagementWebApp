using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class Pages_EditProduct : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
// -----------------------------------------------------
// AUTHENTICATION CHECK
// -----------------------------------------------------


    if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
            return;
        }


        // -----------------------------------------------------
        // LOAD PRODUCT ONLY ON FIRST PAGE LOAD
        // -----------------------------------------------------

        if (!IsPostBack)
        {
            int productID;

            if (!int.TryParse(
                Request.QueryString["ProductID"],
                out productID))
            {
                ShowToast(
                    "Invalid Product ID.",
                    "danger"
                );

                btnUpdate.Enabled = false;

                return;
            }


            hfProductID.Value =
                productID.ToString();


            LoadProduct(productID);
        }
    }


    // =========================================================
    // LOAD PRODUCT
    // =========================================================

    private void LoadProduct(int productID)
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();


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
            WHERE ProductID = @ProductID";


            using (SqlCommand cmd =
                new SqlCommand(
                    query,
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
                        ShowToast(
                            "Product not found.",
                            "danger"
                        );

                        btnUpdate.Enabled =
                            false;

                        return;
                    }


                    // -------------------------------------------------
                    // LOAD FORM VALUES
                    // -------------------------------------------------

                    txtProdName.Text =
                        reader["ProductName"]
                        .ToString();


                    txtSKU.Text =
                        reader["SKU"]
                        .ToString();


                    txtCostPrice.Text =
                        Convert.ToDecimal(
                            reader["CostPrice"]
                        ).ToString("0.00");


                    txtCurrentStock.Text =
                        reader["CurrentStock"]
                        .ToString();


                    txtReorderLevel.Text =
                        reader["ReorderLevel"]
                        .ToString();


                    // -------------------------------------------------
                    // PRODUCT STATUS
                    // -------------------------------------------------

                    bool isActive =
                        Convert.ToBoolean(
                            reader["IsActive"]
                        );


                    lblProductStatus.Text =
                        isActive
                            ? "Active"
                            : "Inactive";


                    lblProductStatus.CssClass =
                        isActive
                            ? "badge bg-label-success"
                            : "badge bg-label-secondary";
                }
            }
        }
    }


    // =========================================================
    // UPDATE PRODUCT
    // =========================================================

    protected void btnUpdate_Click(
        object sender,
        EventArgs e)
    {
        int productID;


        // -----------------------------------------------------
        // VALIDATE PRODUCT ID
        // -----------------------------------------------------

        if (!int.TryParse(
            hfProductID.Value,
            out productID))
        {
            ShowToast(
                "Invalid Product ID.",
                "danger"
            );

            return;
        }


        // -----------------------------------------------------
        // READ FORM VALUES
        // -----------------------------------------------------

        string productName =
            txtProdName.Text.Trim();


        string sku =
            txtSKU.Text.Trim();


        decimal costPrice;


        int reorderLevel;


        // -----------------------------------------------------
        // VALIDATION
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(
            productName))
        {
            ShowToast(
                "Product Name is required.",
                "danger"
            );

            return;
        }


        if (string.IsNullOrWhiteSpace(
            sku))
        {
            ShowToast(
                "SKU is required.",
                "danger"
            );

            return;
        }


        if (!decimal.TryParse(
            txtCostPrice.Text.Trim(),
            out costPrice))
        {
            ShowToast(
                "Please enter a valid Cost Price.",
                "danger"
            );

            return;
        }


        if (costPrice <= 0)
        {
            ShowToast(
                "Cost Price must be greater than 0.",
                "danger"
            );

            return;
        }


        if (!int.TryParse(
            txtReorderLevel.Text.Trim(),
            out reorderLevel))
        {
            ShowToast(
                "Please enter a valid Reorder Level.",
                "danger"
            );

            return;
        }


        if (reorderLevel < 0)
        {
            ShowToast(
                "Reorder Level cannot be negative.",
                "danger"
            );

            return;
        }


        // -----------------------------------------------------
        // DATABASE
        // -----------------------------------------------------

        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();


            // -------------------------------------------------
            // CHECK DUPLICATE SKU
            // EXCLUDE CURRENT PRODUCT
            // -------------------------------------------------

            string checkSkuQuery = @"
            SELECT COUNT(*)
            FROM tbl_Products
            WHERE SKU = @SKU
            AND ProductID <> @ProductID";


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


                cmd.Parameters.Add(
                    "@ProductID",
                    SqlDbType.Int
                ).Value = productID;


                int skuCount =
                    Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );


                if (skuCount > 0)
                {
                    ShowToast(
                        "SKU already exists for another product.",
                        "danger"
                    );

                    return;
                }
            }


            // -------------------------------------------------
            // UPDATE PRODUCT
            // -------------------------------------------------

            string updateQuery = @"
            UPDATE tbl_Products
            SET
                ProductName = @ProductName,
                SKU = @SKU,
                CostPrice = @CostPrice,
                ReorderLevel = @ReorderLevel
            WHERE ProductID = @ProductID";


            using (SqlCommand updateCmd =
                new SqlCommand(
                    updateQuery,
                    conn))
            {
                updateCmd.Parameters.Add(
                    "@ProductName",
                    SqlDbType.NVarChar,
                    200
                ).Value = productName;


                updateCmd.Parameters.Add(
                    "@SKU",
                    SqlDbType.NVarChar,
                    100
                ).Value = sku;


                SqlParameter costParameter =
                    updateCmd.Parameters.Add(
                        "@CostPrice",
                        SqlDbType.Decimal
                    );


                costParameter.Precision = 18;
                costParameter.Scale = 2;
                costParameter.Value = costPrice;


                updateCmd.Parameters.Add(
                    "@ReorderLevel",
                    SqlDbType.Int
                ).Value = reorderLevel;


                updateCmd.Parameters.Add(
                    "@ProductID",
                    SqlDbType.Int
                ).Value = productID;


                int rowsAffected =
                    updateCmd.ExecuteNonQuery();


                if (rowsAffected > 0)
                {
                    // -------------------------------------------------
                    // REDIRECT BACK TO PRODUCT LIST
                    // -------------------------------------------------

                    Response.Redirect(
                        "Products.aspx?updated=1"
                    );
                }
                else
                {
                    ShowToast(
                        "Unable to update product.",
                        "danger"
                    );
                }
            }
        }
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
