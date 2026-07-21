
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;  //for gridview

public partial class Pages_Products : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/login.aspx");
        }

        if (!IsPostBack)
        {
            // Initial data load for your table/repeater goes here
            LoadProductData();
            ClearModalFields();
        }
    }

    // Triggered when typing in the search box
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        string searchTerm = txtSearch.Text.Trim();
        // Logic to filter your data source based on searchTerm
        // RefreshGrid(searchTerm);
    }

    // Triggered when clicking the Save button in the Modal
    protected void btnSave_Click(object sender, EventArgs e)
    {

        // Read textbox values
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
            ShowToast("Please enter a valid reorder level", "danger");
            return;
        }

        // Validate numeric fields
        if (costPrice <= 0)
        {
            ShowToast("Cost Price must be greater than 0", "danger");
            return;
        }

        if (reorderLevel < 0)
        {
            ShowToast("Reorder Level can not be negative", "danger");
            return;
        }

        // Check SKU uniqueness
        string connStr = Connection.getConnectionString();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string checkSkuQuery = "SELECT COUNT(*) FROM tbl_Products WHERE SKU = @SKU";
            using (SqlCommand cmd = new SqlCommand(checkSkuQuery, conn))
            {

                cmd.Parameters.AddWithValue("@SKU", sku); //telling SQL what @SKU means
                int skuCount = (int)cmd.ExecuteScalar();
                if (skuCount > 0)
                {
                    ShowToast("SKU already exists", "danger");
                    return;
                }

            }

            // Insert product
            string insertQuery = "INSERT INTO tbl_Products (ProductName, SKU, CostPrice, CurrentStock, ReorderLevel, CreatedDate) VALUES (@ProductName, @SKU, @CostPrice,  @CurrentStock, @ReorderLevel, @CreatedDate)";
            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@ProductName", productName);
                insertCmd.Parameters.AddWithValue("@SKU", sku);
                insertCmd.Parameters.AddWithValue("@CostPrice", costPrice);
                insertCmd.Parameters.AddWithValue("@CurrentStock", 0);
                insertCmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                insertCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                // Show success message
                int rowsAffected = insertCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ShowToast("Product inserted successfully", "success");
                    ClearModalFields();
                    LoadProductData();


                }
            }

        }
    } // end of btnSave_Click

    private void ClearModalFields()
    {
        txtProdName.Text = string.Empty;
        txtSKU.Text = string.Empty;
        txtCostPrice.Text = string.Empty;
        txtReorderLevel.Text = string.Empty;
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


    private void LoadProductData()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = "SELECT * FROM tbl_Products ORDER BY ProductID ASC";

            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }
    }


    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (ViewState["SelectedProductID"] == null)
        {
            ShowToast("Please select a product first", "warning");
            return;
        }

        int productID =
            Convert.ToInt32(ViewState["SelectedProductID"]);

        Response.Redirect(
            "EditProduct.aspx?ProductID=" + productID
        );
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Perform deletion logic based on the selected Product ID
    }


    protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewProduct")
        {
            int productID = Convert.ToInt32(e.CommandArgument);

            ViewState["SelectedProductID"] = productID;
            //ShowToast("Selected Product ID: " + productID, "info");
            LoadProductDetails(productID);
        }
    }

    private void LoadProductDetails(int productID)
    {
        string connStr = Connection.getConnectionString();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string loadQry = "SELECT * FROM tbl_Products WHERE ProductID = @ProductID";

            using (SqlCommand cmd = new SqlCommand(loadQry, conn))
            {
                cmd.Parameters.AddWithValue("@ProductID", productID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblDetailName.Text = reader["ProductName"].ToString();
                        lblDetailSKU.Text = reader["SKU"].ToString();
                        lblDetailCost.Text = Convert.ToDecimal(reader["CostPrice"]).ToString("0.00");
                        lblDetailStock.Text = reader["CurrentStock"].ToString();
                        lblDetailReorder.Text = reader["ReorderLevel"].ToString();
                    }

                }
            }
        }
    }

 
    protected void btnAddProduct_Click(object sender, EventArgs e)
    {
        ClearModalFields();

        

        ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "OpenModal",
            @"
        var myModal = new bootstrap.Modal(document.getElementById('backDropModal'));
        myModal.show();
        ",
            true
        );
    }

    protected void btnAddProduct_Click1(object sender, EventArgs e)
    {

    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        pnlProductDetails.Visible = true;
    }
}