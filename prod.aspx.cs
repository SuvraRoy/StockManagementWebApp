//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//public partial class Pages_Products : System.Web.UI.Page
//{
//    protected void Page_Load(object sender, EventArgs e)
//    {

//    }
//}

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Products : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Initial data load for your table/repeater goes here
            LoadProductData();
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
        string name = txtProdName.Text.Trim();
        string sku = txtSKU.Text.Trim();
        string cost = txtCost.Text.Trim();
        string reorder = txtReorder.Text.Trim();

        // Validate and insert into your database here
        // if (IsValid) { ... }

        // Optionally clear fields and rebind grid
        ClearModalFields();
        LoadProductData();
    }

    // Triggered when clicking Edit in the Details section
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Redirect to edit page or open a different modal
    }

    // Triggered when clicking Delete in the Details section
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Perform deletion logic based on the selected Product ID
    }

    private void LoadProductData()
    {
        // Database call logic to bind data to your table/grid
    }

    private void ClearModalFields()
    {
        txtProdName.Text = string.Empty;
        txtSKU.Text = string.Empty;
        txtCost.Text = string.Empty;
        txtReorder.Text = string.Empty;
    }
}