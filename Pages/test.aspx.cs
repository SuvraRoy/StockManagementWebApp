using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


public partial class Pages_test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Login check
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }


        if (!IsPostBack)
        {
            txtPurchaseDate.Text =
                DateTime.Now.ToString("yyyy-MM-dd");

            LoadSuppliers();

            LoadWarehouses();

            GeneratePurchaseNumber();

            CreateItemTable();
        }
    }



    // =====================================================
    // LOAD SUPPLIERS
    // =====================================================

    private void LoadSuppliers()
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    SupplierID,
                    CompanyName
                FROM tbl_Suppliers
                WHERE IsActive = 1
                ORDER BY CompanyName";


            using (SqlDataAdapter da =
                new SqlDataAdapter(query, conn))
            {
                DataTable dt =
                    new DataTable();

                da.Fill(dt);


                ddlSupplier.DataSource = dt;

                ddlSupplier.DataTextField =
                    "CompanyName";

                ddlSupplier.DataValueField =
                    "SupplierID";

                ddlSupplier.DataBind();


                ddlSupplier.Items.Insert(
                    0,
                    new ListItem(
                        "Select Supplier",
                        ""
                    )
                );
            }
        }
    }



    // =====================================================
    // LOAD WAREHOUSES
    // =====================================================

    private void LoadWarehouses()
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    WarehouseID,
                    WarehouseName
                FROM tbl_Warehouses
                ORDER BY WarehouseName";


            using (SqlDataAdapter da =
                new SqlDataAdapter(query, conn))
            {
                DataTable dt =
                    new DataTable();

                da.Fill(dt);


                ddlWarehouse.DataSource = dt;

                ddlWarehouse.DataTextField =
                    "WarehouseName";

                ddlWarehouse.DataValueField =
                    "WarehouseID";

                ddlWarehouse.DataBind();


                ddlWarehouse.Items.Insert(
                    0,
                    new ListItem(
                        "Select Warehouse",
                        ""
                    )
                );
            }
        }
    }



    // =====================================================
    // PURCHASE NUMBER
    // =====================================================

    private void GeneratePurchaseNumber()
    {
        // Temporary purchase number.
        // We will connect this to tbl_Purchases
        // when creating the database transaction.

        txtPurchaseNumber.Text =
            "PUR-" +
            DateTime.Now.ToString("yyyyMMddHHmmss");
    }



    // =====================================================
    // CREATE FIRST ITEM ROW
    // =====================================================

    private void CreateItemTable()
    {
        DataTable dt =
            CreateTableStructure();


        DataRow row =
            dt.NewRow();


        row["RowIndex"] = 1;

        row["ProductID"] = "";

        row["SKU"] = "--";

        row["Quantity"] = "1";

        row["Rate"] = "0.00";

        row["Amount"] = "0.00";


        dt.Rows.Add(row);


        ViewState["PurchaseItems"] = dt;


        BindItems();
    }



    // =====================================================
    // CREATE DATATABLE STRUCTURE
    // =====================================================

    private DataTable CreateTableStructure()
    {
        DataTable dt =
            new DataTable();


        dt.Columns.Add(
            "RowIndex",
            typeof(int)
        );

        dt.Columns.Add(
            "ProductID",
            typeof(string)
        );

        dt.Columns.Add(
            "SKU",
            typeof(string)
        );

        dt.Columns.Add(
            "Quantity",
            typeof(string)
        );

        dt.Columns.Add(
            "Rate",
            typeof(string)
        );

        dt.Columns.Add(
            "Amount",
            typeof(string)
        );


        return dt;
    }



    // =====================================================
    // BIND ITEM REPEATER
    // =====================================================

    private void BindItems()
    {
        DataTable dt =
            ViewState["PurchaseItems"]
            as DataTable;


        rptItems.DataSource = dt;

        rptItems.DataBind();


        CalculateGrandTotal();
    }



    // =====================================================
    // LOAD PRODUCT DROPDOWN FOR EVERY ROW
    // =====================================================

    protected void rptItems_ItemDataBound(
        object sender,
        RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType !=
                ListItemType.Item &&
            e.Item.ItemType !=
                ListItemType.AlternatingItem)
        {
            return;
        }


        DropDownList ddlProduct =
            (DropDownList)e.Item.FindControl(
                "ddlProduct"
            );


        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    ProductID,
                    ProductName
                FROM tbl_Products
                ORDER BY ProductName";


            using (SqlDataAdapter da =
                new SqlDataAdapter(query, conn))
            {
                DataTable products =
                    new DataTable();

                da.Fill(products);


                ddlProduct.DataSource =
                    products;

                ddlProduct.DataTextField =
                    "ProductName";

                ddlProduct.DataValueField =
                    "ProductID";

                ddlProduct.DataBind();


                ddlProduct.Items.Insert(
                    0,
                    new ListItem(
                        "Select Product",
                        ""
                    )
                );
            }
        }


        DataRowView row =
            (DataRowView)e.Item.DataItem;


        string productID =
            row["ProductID"].ToString();


        if (productID != "" &&
            ddlProduct.Items.FindByValue(productID) != null)
        {
            ddlProduct.SelectedValue =
                productID;
        }
    }



    // =====================================================
    // ADD NEW ITEM
    // =====================================================

    protected void btnAddRow_Click(
        object sender,
        EventArgs e)
    {
        SaveCurrentRows();


        DataTable dt =
            ViewState["PurchaseItems"]
            as DataTable;


        DataRow row =
            dt.NewRow();


        row["RowIndex"] =
            dt.Rows.Count + 1;

        row["ProductID"] = "";

        row["SKU"] = "--";

        row["Quantity"] = "1";

        row["Rate"] = "0.00";

        row["Amount"] = "0.00";


        dt.Rows.Add(row);


        ViewState["PurchaseItems"] = dt;


        BindItems();
    }



    // =====================================================
    // DELETE ITEM
    // =====================================================

    protected void btnDelete_Click(
        object sender,
        EventArgs e)
    {
        SaveCurrentRows();


        DataTable dt =
            ViewState["PurchaseItems"]
            as DataTable;


        if (dt.Rows.Count <= 1)
        {
            ShowMessage(
                "At least one purchase item is required."
            );

            return;
        }


        LinkButton button =
            (LinkButton)sender;


        int rowIndex =
            Convert.ToInt32(
                button.CommandArgument
            );


        for (int i = 0;
             i < dt.Rows.Count;
             i++)
        {
            int currentRow =
                Convert.ToInt32(
                    dt.Rows[i]["RowIndex"]
                );


            if (currentRow == rowIndex)
            {
                dt.Rows.RemoveAt(i);

                break;
            }
        }



        // Re-number rows

        for (int i = 0;
             i < dt.Rows.Count;
             i++)
        {
            dt.Rows[i]["RowIndex"] =
                i + 1;
        }


        ViewState["PurchaseItems"] = dt;


        BindItems();
    }



    // =====================================================
    // PRODUCT SELECTED
    // =====================================================

    protected void ddlProduct_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        DropDownList ddlProduct =
            (DropDownList)sender;


        RepeaterItem item =
            (RepeaterItem)
            ddlProduct.NamingContainer;


        Label lblSKU =
            (Label)item.FindControl(
                "lblSKU"
            );


        TextBox txtRate =
            (TextBox)item.FindControl(
                "txtRate"
            );


        if (ddlProduct.SelectedValue == "")
        {
            lblSKU.Text = "--";

            txtRate.Text = "0.00";

            SaveCurrentRows();

            return;
        }


        int productID =
            Convert.ToInt32(
                ddlProduct.SelectedValue
            );


        LoadProductInformation(
            productID,
            lblSKU,
            txtRate
        );


        SaveCurrentRows();

        CalculateRow(item);

        CalculateGrandTotal();
    }



    // =====================================================
    // LOAD SKU + COST PRICE
    // =====================================================

    private void LoadProductInformation(
        int productID,
        Label lblSKU,
        TextBox txtRate)
    {
        string connStr =
            Connection.getConnectionString();


        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT
                    SKU,
                    CostPrice
                FROM tbl_Products
                WHERE ProductID = @ProductID";


            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    productID
                );


                conn.Open();


                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblSKU.Text =
                            reader["SKU"].ToString();


                        txtRate.Text =
                            Convert.ToDecimal(
                                reader["CostPrice"]
                            )
                            .ToString("0.00");
                    }
                }
            }
        }
    }



    // =====================================================
    // QUANTITY OR RATE CHANGED
    // =====================================================

    protected void ItemValueChanged(
        object sender,
        EventArgs e)
    {
        TextBox textbox =
            (TextBox)sender;


        RepeaterItem item =
            (RepeaterItem)
            textbox.NamingContainer;


        CalculateRow(item);


        SaveCurrentRows();


        CalculateGrandTotal();
    }



    // =====================================================
    // CALCULATE ONE ROW
    // =====================================================

    private void CalculateRow(
        RepeaterItem item)
    {
        TextBox txtQuantity =
            (TextBox)item.FindControl(
                "txtQuantity"
            );


        TextBox txtRate =
            (TextBox)item.FindControl(
                "txtRate"
            );


        Label lblAmount =
            (Label)item.FindControl(
                "lblAmount"
            );


        int quantity = 0;

        decimal rate = 0;


        int.TryParse(
            txtQuantity.Text,
            out quantity
        );


        decimal.TryParse(
            txtRate.Text,
            out rate
        );


        decimal amount =
            quantity * rate;


        lblAmount.Text =
            amount.ToString("0.00");
    }



    // =====================================================
    // SAVE CURRENT CONTROLS INTO VIEWSTATE
    // =====================================================

    private void SaveCurrentRows()
    {
        DataTable dt =
            ViewState["PurchaseItems"]
            as DataTable;


        if (dt == null)
        {
            return;
        }


        for (int i = 0;
             i < rptItems.Items.Count;
             i++)
        {
            RepeaterItem item =
                rptItems.Items[i];


            DropDownList ddlProduct =
                (DropDownList)item.FindControl(
                    "ddlProduct"
                );


            Label lblSKU =
                (Label)item.FindControl(
                    "lblSKU"
                );


            TextBox txtQuantity =
                (TextBox)item.FindControl(
                    "txtQuantity"
                );


            TextBox txtRate =
                (TextBox)item.FindControl(
                    "txtRate"
                );


            Label lblAmount =
                (Label)item.FindControl(
                    "lblAmount"
                );


            dt.Rows[i]["ProductID"] =
                ddlProduct.SelectedValue;


            dt.Rows[i]["SKU"] =
                lblSKU.Text;


            dt.Rows[i]["Quantity"] =
                txtQuantity.Text;


            dt.Rows[i]["Rate"] =
                txtRate.Text;


            dt.Rows[i]["Amount"] =
                lblAmount.Text;
        }


        ViewState["PurchaseItems"] = dt;
    }



    // =====================================================
    // GRAND TOTAL
    // =====================================================

    private void CalculateGrandTotal()
    {
        decimal grandTotal = 0;


        foreach (RepeaterItem item
            in rptItems.Items)
        {
            Label lblAmount =
                (Label)item.FindControl(
                    "lblAmount"
                );


            decimal amount = 0;


            decimal.TryParse(
                lblAmount.Text,
                out amount
            );


            grandTotal += amount;
        }


        lblGrandTotal.Text =
            grandTotal.ToString("0.00");
    }



    // =====================================================
    // SAVE PURCHASE
    // =====================================================

    protected void btnSave_Click(
        object sender,
        EventArgs e)
    {
        divMessage.Visible = false;


        SaveCurrentRows();


        string validation =
            ValidatePurchase();


        if (validation != "")
        {
            ShowMessage(validation);

            return;
        }


        /*
            IMPORTANT:

            We deliberately DO NOT insert into the
            database here yet.

            The next step will create:

            tbl_Purchases
            tbl_PurchaseItems

            and update:

            tbl_WarehouseStock

            inside ONE SqlTransaction.

            That is where the actual stock increase
            must happen.
        */


        ShowMessage(
            "Purchase form is valid. Database save will be added next."
        );
    }



    // =====================================================
    // VALIDATION
    // =====================================================

    private string ValidatePurchase()
    {
        if (ddlSupplier.SelectedValue == "")
        {
            return "Please select a supplier.";
        }


        if (ddlWarehouse.SelectedValue == "")
        {
            return "Please select a warehouse.";
        }


        DateTime purchaseDate;


        if (!DateTime.TryParse(
            txtPurchaseDate.Text,
            out purchaseDate))
        {
            return "Please enter a valid purchase date.";
        }


        DataTable dt =
            ViewState["PurchaseItems"]
            as DataTable;


        if (dt == null ||
            dt.Rows.Count == 0)
        {
            return "Please add at least one product.";
        }


        foreach (DataRow row
            in dt.Rows)
        {
            if (row["ProductID"].ToString() == "")
            {
                return "Please select a product in every row.";
            }


            int quantity;


            if (!int.TryParse(
                    row["Quantity"].ToString(),
                    out quantity) ||
                quantity <= 0)
            {
                return "Quantity must be greater than 0.";
            }


            decimal rate;


            if (!decimal.TryParse(
                    row["Rate"].ToString(),
                    out rate) ||
                rate <= 0)
            {
                return "Purchase rate must be greater than 0.";
            }
        }


        return "";
    }



    // =====================================================
    // MESSAGE
    // =====================================================

    private void ShowMessage(
        string message)
    {
        divMessage.Visible = true;

        lblMessage.Text = message;
    }



    // =====================================================
    // CANCEL
    // =====================================================

    protected void btnCancel_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "Purchases.aspx"
        );
    }
}