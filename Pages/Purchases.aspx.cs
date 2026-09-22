using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Purchases : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }

        if (!IsPostBack)
        {
            LoadSuppliers();
            LoadWarehouses();

            txtPurchaseDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            CreateInitialRows();
        }
    }


    // ============================================
    // LOAD SUPPLIERS
    // ============================================

    private void LoadSuppliers()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = @"SELECT SupplierID, CompanyName FROM tbl_Suppliers WHERE IsActive = 1 ORDER BY CompanyName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                ddlSupplier.DataSource = reader;
                ddlSupplier.DataTextField = "CompanyName";
                ddlSupplier.DataValueField = "SupplierID";
                ddlSupplier.DataBind();
            }
        }

        ddlSupplier.Items.Insert(0,
            new ListItem("-- Select Supplier --", ""));
    }


    // ============================================
    // LOAD WAREHOUSES
    // ============================================

    private void LoadWarehouses()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = @"
                SELECT WarehouseID, WarehouseName
                FROM tbl_Warehouses
                WHERE IsActive = 1
                ORDER BY WarehouseName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                ddlWarehouse.DataSource = reader;
                ddlWarehouse.DataTextField = "WarehouseName";
                ddlWarehouse.DataValueField = "WarehouseID";
                ddlWarehouse.DataBind();
            }
        }

        ddlWarehouse.Items.Insert(0,
            new ListItem("-- Select Warehouse --", ""));
    }


    // ============================================
    // PRODUCT LIST
    // ============================================

    private DataTable GetProducts()
    {
        DataTable table = new DataTable();

        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = @"
                SELECT ProductID, ProductName, SKU, CostPrice
                FROM tbl_Products
                ORDER BY ProductName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);
            }
        }

        return table;
    }


    // ============================================
    // CREATE INITIAL ROW
    // ============================================

    private void CreateInitialRows()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ProductID");
        table.Columns.Add("Quantity");
        table.Columns.Add("Rate");

        DataRow row = table.NewRow();

        row["ProductID"] = "";
        row["Quantity"] = "1";
        row["Rate"] = "0";

        table.Rows.Add(row);

        ViewState["PurchaseItems"] = table;

        BindItems();
    }


    // ============================================
    // BIND ITEMS
    // ============================================

    private void BindItems()
    {
        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        rptItems.DataSource = table;
        rptItems.DataBind();

        DataTable products = GetProducts();

        for (int i = 0; i < rptItems.Items.Count; i++)
        {
            RepeaterItem item = rptItems.Items[i];

            DropDownList ddlProduct =
                item.FindControl("ddlProduct") as DropDownList;

            Label lblSKU =
                item.FindControl("lblSKU") as Label;

            TextBox txtQuantity =
                item.FindControl("txtQuantity") as TextBox;

            TextBox txtRate =
                item.FindControl("txtRate") as TextBox;

            DataRow row = table.Rows[i];

            ddlProduct.DataSource = products;
            ddlProduct.DataTextField = "ProductName";
            ddlProduct.DataValueField = "ProductID";
            ddlProduct.DataBind();

            ddlProduct.Items.Insert(0,
                new ListItem("-- Select Product --", ""));

            string productId = row["ProductID"].ToString();

            if (productId != "")
            {
                ListItem selectedItem =
                    ddlProduct.Items.FindByValue(productId);

                if (selectedItem != null)
                {
                    ddlProduct.SelectedValue = productId;

                    DataRow[] productRows =
                        products.Select(
                            "ProductID = " + productId);

                    if (productRows.Length > 0)
                    {
                        lblSKU.Text =
                            productRows[0]["SKU"].ToString();
                    }
                }
            }

            txtQuantity.Text =
                row["Quantity"].ToString();

            txtRate.Text =
                row["Rate"].ToString();
        }

        CalculateGrandTotal();
    }


    // ============================================
    // SAVE CURRENT ROW VALUES
    // ============================================

    private void SaveCurrentRows()
    {
        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        for (int i = 0; i < rptItems.Items.Count; i++)
        {
            RepeaterItem item = rptItems.Items[i];

            DropDownList ddlProduct =
                item.FindControl("ddlProduct") as DropDownList;

            TextBox txtQuantity =
                item.FindControl("txtQuantity") as TextBox;

            TextBox txtRate =
                item.FindControl("txtRate") as TextBox;

            table.Rows[i]["ProductID"] =
                ddlProduct.SelectedValue;

            table.Rows[i]["Quantity"] =
                txtQuantity.Text.Trim();

            table.Rows[i]["Rate"] =
                txtRate.Text.Trim();
        }

        ViewState["PurchaseItems"] = table;
    }


    // ============================================
    // ADD ITEM
    // ============================================

    protected void btnAddRow_Click(object sender, EventArgs e)
    {
        SaveCurrentRows();

        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        DataRow row = table.NewRow();

        row["ProductID"] = "";
        row["Quantity"] = "1";
        row["Rate"] = "0";

        table.Rows.Add(row);

        ViewState["PurchaseItems"] = table;

        BindItems();
    }


    // ============================================
    // DELETE ITEM
    // ============================================

    protected void rptItems_ItemCommand(
        object source,
        RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "DeleteRow")
        {
            SaveCurrentRows();

            DataTable table =
                ViewState["PurchaseItems"] as DataTable;

            int index =
                Convert.ToInt32(e.CommandArgument);

            if (table.Rows.Count > 1)
            {
                table.Rows.RemoveAt(index);
            }

            ViewState["PurchaseItems"] = table;

            BindItems();
        }
    }


    // ============================================
    // PRODUCT SELECTED
    // ============================================

    protected void ddlProduct_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        DropDownList ddlProduct =
            sender as DropDownList;

        RepeaterItem item =
            ddlProduct.NamingContainer as RepeaterItem;

        Label lblSKU =
            item.FindControl("lblSKU") as Label;

        TextBox txtRate =
            item.FindControl("txtRate") as TextBox;

        if (ddlProduct.SelectedValue == "")
        {
            lblSKU.Text = "";
            txtRate.Text = "0";
            return;
        }

        int productId =
            Convert.ToInt32(ddlProduct.SelectedValue);

        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            string query = @"
                SELECT SKU, CostPrice
                FROM tbl_Products
                WHERE ProductID = @ProductID";

            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    productId);

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
                                reader["CostPrice"])
                            .ToString("0.00");
                    }
                }
            }
        }

        SaveCurrentRows();
    }


    // ============================================
    // VALIDATION
    // ============================================

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

        if (string.IsNullOrWhiteSpace(
            txtInvoiceNumber.Text))
        {
            return "Supplier invoice number is required.";
        }

        DateTime purchaseDate;

        if (!DateTime.TryParse(
            txtPurchaseDate.Text,
            out purchaseDate))
        {
            return "Please select a valid purchase date.";
        }

        SaveCurrentRows();

        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        if (table == null || table.Rows.Count == 0)
        {
            return "Please add at least one product.";
        }

        foreach (DataRow row in table.Rows)
        {
            if (row["ProductID"].ToString() == "")
            {
                return "Please select a product for every row.";
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
                rate < 0)
            {
                return "Please enter a valid purchase rate.";
            }
        }

        return "";
    }


    // ============================================
    // SAVE PURCHASE
    // ============================================

    protected void btnSave_Click(
        object sender,
        EventArgs e)
    {
        divMessage.Visible = false;

        string validationMessage =
            ValidatePurchase();

        if (validationMessage != "")
        {
            ShowMessage(validationMessage);
            return;
        }

        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        decimal grandTotal = 0;

        foreach (DataRow row in table.Rows)
        {
            int quantity =
                Convert.ToInt32(row["Quantity"]);

            decimal rate =
                Convert.ToDecimal(row["Rate"]);

            grandTotal += quantity * rate;
        }


        string connStr =
            Connection.getConnectionString();

        using (SqlConnection conn =
            new SqlConnection(connStr))
        {
            conn.Open();

            SqlTransaction transaction =
                conn.BeginTransaction();

            try
            {
                // ==================================
                // 1. INSERT PURCHASE HEADER
                // ==================================

                string purchaseQuery = @"
                    INSERT INTO tbl_Purchases
                    (
                        SupplierID,
                        InvoiceNumber,
                        PurchaseDate,
                        TotalAmount,
                        Remarks,
                        CreatedDate
                    )
                    VALUES
                    (
                        @SupplierID,
                        @InvoiceNumber,
                        @PurchaseDate,
                        @TotalAmount,
                        @Remarks,
                        @CreatedDate
                    );

                    SELECT SCOPE_IDENTITY();";


                int purchaseId;

                using (SqlCommand cmd =
                    new SqlCommand(
                        purchaseQuery,
                        conn,
                        transaction))
                {
                    cmd.Parameters.AddWithValue(
                        "@SupplierID",
                        Convert.ToInt32(
                            ddlSupplier.SelectedValue));

                    cmd.Parameters.AddWithValue(
                        "@InvoiceNumber",
                        txtInvoiceNumber.Text.Trim());

                    cmd.Parameters.AddWithValue(
                        "@PurchaseDate",
                        Convert.ToDateTime(
                            txtPurchaseDate.Text));

                    cmd.Parameters.AddWithValue(
                        "@TotalAmount",
                        grandTotal);

                    cmd.Parameters.AddWithValue(
                        "@Remarks",
                        txtRemarks.Text.Trim());

                    cmd.Parameters.AddWithValue(
                        "@CreatedDate",
                        DateTime.Now);

                    purchaseId =
                        Convert.ToInt32(
                            cmd.ExecuteScalar());
                }


                // ==================================
                // 2. SAVE EACH PURCHASE ITEM
                // ==================================

                foreach (DataRow row in table.Rows)
                {
                    int productId =
                        Convert.ToInt32(
                            row["ProductID"]);

                    int quantity =
                        Convert.ToInt32(
                            row["Quantity"]);

                    decimal rate =
                        Convert.ToDecimal(
                            row["Rate"]);

                    decimal amount =
                        quantity * rate;


                    // ------------------------------
                    // Purchase Detail
                    // ------------------------------

                    string detailQuery = @"
                        INSERT INTO tbl_PurchaseDetails
                        (
                            PurchaseID,
                            ProductID,
                            WarehouseID,
                            Quantity,
                            PurchasePrice,
                            TotalAmount
                        )
                        VALUES
                        (
                            @PurchaseID,
                            @ProductID,
                            @WarehouseID,
                            @Quantity,
                            @PurchasePrice,
                            @TotalAmount
                        )";


                    using (SqlCommand cmd =
                        new SqlCommand(
                            detailQuery,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@PurchaseID",
                            purchaseId);

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productId);

                        cmd.Parameters.AddWithValue(
                            "@WarehouseID",
                            Convert.ToInt32(
                                ddlWarehouse.SelectedValue));

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity);

                        cmd.Parameters.AddWithValue(
                            "@PurchasePrice",
                            rate);

                        cmd.Parameters.AddWithValue(
                            "@TotalAmount",
                            amount);

                        cmd.ExecuteNonQuery();
                    }


                    // ------------------------------
                    // Warehouse Stock
                    // ------------------------------

                    string stockQuery = @"
                        IF EXISTS
                        (
                            SELECT 1
                            FROM tbl_WarehouseStock
                            WHERE WarehouseID = @WarehouseID
                            AND ProductID = @ProductID
                        )
                        BEGIN

                            UPDATE tbl_WarehouseStock
                            SET Quantity = Quantity + @Quantity
                            WHERE WarehouseID = @WarehouseID
                            AND ProductID = @ProductID;

                        END
                        ELSE
                        BEGIN

                            INSERT INTO tbl_WarehouseStock
                            (
                                WarehouseID,
                                ProductID,
                                Quantity,
                                CreatedAt
                            )
                            VALUES
                            (
                                @WarehouseID,
                                @ProductID,
                                @Quantity,
                                @CreatedAt
                            );

                        END";


                    using (SqlCommand cmd =
                        new SqlCommand(
                            stockQuery,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@WarehouseID",
                            Convert.ToInt32(
                                ddlWarehouse.SelectedValue));

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productId);

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity);

                        cmd.Parameters.AddWithValue(
                            "@CreatedAt",
                            DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }


                    // ------------------------------
                    // Overall Product Stock
                    // ------------------------------

                    string productStockQuery = @"
                        UPDATE tbl_Products
                        SET CurrentStock =
                            CurrentStock + @Quantity
                        WHERE ProductID = @ProductID";


                    using (SqlCommand cmd =
                        new SqlCommand(
                            productStockQuery,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity);

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productId);

                        cmd.ExecuteNonQuery();
                    }
                }


                // ==================================
                // COMMIT
                // ==================================

                transaction.Commit();

                Response.Redirect(
                    "Purchases.aspx?msg=saved");
            }
            catch (Exception ex)
            {
                // Only roll back if the connection is open and the transaction object isn't dead
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Rollback();
                }

                ShowMessage("Purchase could not be saved: " + ex.Message);
            }

        }
    }


    // ============================================
    // GRAND TOTAL
    // ============================================

    private void CalculateGrandTotal()
    {
        decimal total = 0;

        DataTable table =
            ViewState["PurchaseItems"] as DataTable;

        if (table == null)
        {
            lblGrandTotal.Text = "0.00";
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            decimal quantity;
            decimal rate;

            if (decimal.TryParse(
                row["Quantity"].ToString(),
                out quantity) &&
                decimal.TryParse(
                    row["Rate"].ToString(),
                    out rate))
            {
                total += quantity * rate;
            }
        }

        lblGrandTotal.Text =
            total.ToString("0.00");
    }


    // ============================================
    // CANCEL
    // ============================================

    protected void btnCancel_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect("PurchaseHistory.aspx");
    }


    // ============================================
    // MESSAGE
    // ============================================

    private void ShowMessage(string message)
    {
        divMessage.Visible = true;

        lblMessage.Text = message;
    }
}