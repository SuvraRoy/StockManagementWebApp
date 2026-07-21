using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;


public partial class Pages_EditProduct : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!IsPostBack)
        {
            if (Request.QueryString["ProductID"] == null)
            {
                Response.Redirect("Products.aspx");
                return;
            }

            int productID =
                Convert.ToInt32(Request.QueryString["ProductID"]);

            LoadProduct(productID);
        }
    }

    private void LoadProduct(int productID)
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();

            string query =
                "SELECT * FROM tbl_Products WHERE ProductID=@ProductID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    productID);

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtProdName.Text =
                            reader["ProductName"].ToString();

                        txtSKU.Text =
                            reader["SKU"].ToString();

                        txtCostPrice.Text =
                            reader["CostPrice"].ToString();

                        txtReorderLevel.Text =
                            reader["ReorderLevel"].ToString();

                        hfProductID.Value =
                            reader["ProductID"].ToString();
                    }
                }
            }
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string productName = txtProdName.Text.Trim();
        string sku = txtSKU.Text.Trim();

        decimal costPrice;
        int reorderLevel;

        // Validate required fields
        if (string.IsNullOrWhiteSpace(productName))
        {
            ShowToast("Product Name is required", "danger");
            return;
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            ShowToast("SKU is required", "danger");
            return;
        }

        if (!decimal.TryParse(txtCostPrice.Text, out costPrice))
        {
            ShowToast("Please enter a valid Cost Price", "danger");
            return;
        }

        if (!int.TryParse(txtReorderLevel.Text, out reorderLevel))
        {
            ShowToast("Please enter a valid Reorder Level", "danger");
            return;
        }

        if (costPrice <= 0)
        {
            ShowToast("Cost Price must be greater than 0", "danger");
            return;
        }

        if (reorderLevel < 0)
        {
            ShowToast("Reorder Level cannot be negative", "danger");
            return;
        }

        int productID = Convert.ToInt32(hfProductID.Value);

        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();

            // Check SKU uniqueness (excluding current product)
            string checkSkuQuery =
                @"SELECT COUNT(*)
              FROM tbl_Products
              WHERE SKU = @SKU
              AND ProductID <> @ProductID";

            using (SqlCommand cmd = new SqlCommand(checkSkuQuery, conn))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@ProductID", productID);

                int skuCount = (int)cmd.ExecuteScalar();

                if (skuCount > 0)
                {
                    ShowToast("SKU already exists", "danger");
                    return;
                }
            }

            // Update Product
            string updateQuery =
                @"UPDATE tbl_Products
              SET ProductName = @ProductName,
                  SKU = @SKU,
                  CostPrice = @CostPrice,
                  ReorderLevel = @ReorderLevel
              WHERE ProductID = @ProductID";

            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
            {
                updateCmd.Parameters.AddWithValue("@ProductName", productName);
                updateCmd.Parameters.AddWithValue("@SKU", sku);
                updateCmd.Parameters.AddWithValue("@CostPrice", costPrice);
                updateCmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                updateCmd.Parameters.AddWithValue("@ProductID", productID);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Response.Redirect("Products.aspx");
                }
                else
                {
                    ShowToast("Failed to update product", "danger");
                }
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

}