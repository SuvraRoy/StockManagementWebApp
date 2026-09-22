using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Sales : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadCustomers();
            LoadWarehouses();

            txtSalesDate.Text =  DateTime.Now.ToString("yyyy-MM-dd");

            // Generate visible invoice number immediately
            txtInvoiceNumber.Text = GenerateInvoiceNumber();

            CreateInitialRows();
        }
    }


    // =========================================================
    // GENERATE INVOICE NUMBER
    // =========================================================

    private string GenerateInvoiceNumber()
    {
        return "INV-" +  DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
    }


    // =========================================================
    // CUSTOMERS
    // =========================================================

    private void LoadCustomers()
    {
        ddlCustomer.Items.Clear(); // clearing any existing selection in the dropdown

        ddlCustomer.Items.Add( new ListItem("-- Select Customer --", "") );

        string query = @"SELECT CustomerID, CustomerName, CustomerCode FROM tbl_Customers WHERE IsActive = 1 ORDER BY CustomerName";

        using (SqlConnection con = new SqlConnection( Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read()) //As long as dr.Read() finds another customer row, it runs the code inside the loop.
                {
                    ddlCustomer.Items.Add( new ListItem( dr["CustomerName"].ToString() + " (" + dr["CustomerCode"].ToString() + ")", dr["CustomerID"].ToString()));
                }
            }
        }
    }


    // =========================================================
    // WAREHOUSES
    // =========================================================

    private void LoadWarehouses()
    {
        ddlWarehouse.Items.Clear();

        ddlWarehouse.Items.Add( new ListItem("-- Select Warehouse --", "")
        );

        string query = @" SELECT WarehouseID, WarehouseName, WarehouseCode FROM tbl_Warehouses WHERE IsActive = 1 ORDER BY WarehouseName";

        using (SqlConnection con = new SqlConnection( Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                SqlDataReader dr =  cmd.ExecuteReader();

                while (dr.Read())
                {
                    ddlWarehouse.Items.Add( new ListItem( dr["WarehouseName"].ToString()  + " (" + dr["WarehouseCode"].ToString()+ ")", dr["WarehouseID"].ToString()));
                }
            }
        }
    }


    // =========================================================
    // CREATE INITIAL ROW
    // =========================================================

    private void CreateInitialRows()
    {
        // Step 1: Create a fresh DataTable structure in memory
        DataTable dt = CreateSalesTable();

        // Step 2: Create a new blank data row
        DataRow row = dt.NewRow();

        // Step 3: Set default starting values
        row["ProductID"] = 0;   // No product selected yet
        row["Quantity"] = 1;   // Default quantity is 1
        row["SalePrice"] = 0;   // Default price is 0

        // Step 4: Add this blank row to the DataTable
        dt.Rows.Add(row);

        // Step 5: Save this row to ASP.NET's temporary memory (ViewState)
        ViewState["SalesItems"] = dt;

        // Step 6: Render this blank row onto the web browser
        BindRepeater();
    }


    private DataTable CreateSalesTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("ProductID", typeof(int));

        dt.Columns.Add("Quantity", typeof(int) );

        dt.Columns.Add("SalePrice", typeof(decimal));

        return dt;
    }


    // =========================================================
    // GET PRODUCTS FOR WAREHOUSE
    // =========================================================

    private DataTable GetWarehouseProducts(int warehouseID)
    {
        DataTable dt = new DataTable();

        string query = @" SELECT p.ProductID, p.ProductName, p.SKU, ws.Quantity FROM tbl_WarehouseStock ws INNER JOIN tbl_Products p ON ws.ProductID = p.ProductID 
                        WHERE ws.WarehouseID = @WarehouseID AND ws.Quantity > 0 AND p.IsActive = 1 ORDER BY p.ProductName";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue( "@WarehouseID", warehouseID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }

        return dt;
    }


    // =========================================================
    // BIND REPEATER
    // =========================================================

    private void BindRepeater()
    {
        DataTable dt = ViewState["SalesItems"] as DataTable;

        if (dt == null)
        {
            dt = CreateSalesTable();
        }

        rptItems.DataSource = dt;
        rptItems.DataBind();


        if (ddlWarehouse.SelectedValue != "")
        {
            BindProductDropdowns();
        }


        RestoreRepeaterValues();

    }


    // =========================================================
    // BIND PRODUCT DROPDOWNS
    // =========================================================

    private void BindProductDropdowns()
    {
        int warehouseID;

        if (!int.TryParse(ddlWarehouse.SelectedValue, out warehouseID))
        {
            return;
        }

        DataTable products = GetWarehouseProducts( warehouseID);


        foreach (RepeaterItem item
            in rptItems.Items)
        {
            DropDownList ddlProduct =  item.FindControl("ddlProduct") as DropDownList;

            if (ddlProduct == null)
            {
                continue;
            }

            ddlProduct.Items.Clear();

            ddlProduct.Items.Add( new ListItem( "-- Select Product --",  ""  )
            );


            foreach (DataRow row in products.Rows)
            {
                string text = row["ProductName"].ToString();

                if (row["SKU"] != DBNull.Value && row["SKU"].ToString() != "")
                {
                    text += " (" + row["SKU"].ToString() + ")";
                }

                ddlProduct.Items.Add( new ListItem( text, row["ProductID"].ToString())
                );
            }
        }
    }


    // =========================================================
    // RESTORE REPEATER VALUES
    // =========================================================

    private void RestoreRepeaterValues()
    {
        DataTable dt =
            ViewState["SalesItems"] as DataTable;

        if (dt == null)
        {
            return;
        }


        for (int i = 0;
            i < rptItems.Items.Count &&
            i < dt.Rows.Count;
            i++)
        {
            RepeaterItem item =
                rptItems.Items[i];


            DropDownList ddlProduct =
                item.FindControl("ddlProduct")
                as DropDownList;

            Label lblStock =
                item.FindControl("lblAvailableStock")
                as Label;

            TextBox txtQuantity =
                item.FindControl("txtQuantity")
                as TextBox;

            TextBox txtRate =
                item.FindControl("txtRate")
                as TextBox;


            int productID =
                Convert.ToInt32(
                    dt.Rows[i]["ProductID"]
                );


            int quantity =
                Convert.ToInt32(
                    dt.Rows[i]["Quantity"]
                );


            decimal rate =
                Convert.ToDecimal(
                    dt.Rows[i]["SalePrice"]
                );


            if (productID > 0 &&
                ddlProduct.Items.FindByValue(
                    productID.ToString()) != null)
            {
                ddlProduct.SelectedValue =
                    productID.ToString();

                int warehouseID;

                if (int.TryParse(
                    ddlWarehouse.SelectedValue,
                    out warehouseID))
                {
                    int stock =
                        GetWarehouseStock(
                            warehouseID,
                            productID
                        );

                    lblStock.Text =
                        stock.ToString();
                }
            }
            else
            {
                ddlProduct.SelectedIndex = 0;
                lblStock.Text = "-";
            }


            txtQuantity.Text =
                quantity.ToString();


            if (rate > 0)
            {
                txtRate.Text =
                    rate.ToString("0.00");
            }
            else
            {
                txtRate.Text = "";
            }
        }
    }


    // =========================================================
    // WAREHOUSE CHANGE
    // =========================================================

    protected void ddlWarehouse_SelectedIndexChanged( object sender, EventArgs e)
    {
        SaveRepeaterValues();

        DataTable dt = ViewState["SalesItems"] as DataTable;

        if (dt != null)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["ProductID"] = 0;
                row["SalePrice"] = 0;
            }

            ViewState["SalesItems"] = dt;
        }

        BindRepeater();

        ShowMessage( "Warehouse selected. Products are now loaded from warehouse stock.",  "success" );
    }


    // =========================================================
    // PRODUCT CHANGE
    // =========================================================

    protected void ddlProduct_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        SaveRepeaterValues();


        DropDownList ddlProduct =
            sender as DropDownList;

        RepeaterItem item =
            ddlProduct.NamingContainer
            as RepeaterItem;


        if (item == null)
        {
            return;
        }


        Label lblStock =
            item.FindControl("lblAvailableStock")
            as Label;


        TextBox txtRate =
            item.FindControl("txtRate")
            as TextBox;


        if (ddlProduct.SelectedValue == "")
        {
            lblStock.Text = "-";
            txtRate.Text = "";

            return;
        }


        int productID;

        if (!int.TryParse(
            ddlProduct.SelectedValue,
            out productID))
        {
            return;
        }


        int warehouseID;

        if (!int.TryParse(
            ddlWarehouse.SelectedValue,
            out warehouseID))
        {
            return;
        }


        int stock =
            GetWarehouseStock(
                warehouseID,
                productID
            );


        decimal costPrice =
            GetProductCostPrice(
                productID
            );


        lblStock.Text =
            stock.ToString();


        txtRate.Text =
            costPrice.ToString("0.00");


        SaveRepeaterValues();
    }


    // =========================================================
    // GET STOCK
    // =========================================================

    private int GetWarehouseStock(
        int warehouseID,
        int productID)
    {
        string query = @"
            SELECT Quantity
            FROM tbl_WarehouseStock
            WHERE WarehouseID = @WarehouseID
                AND ProductID = @ProductID";

        using (SqlConnection con =
            new SqlConnection(
                Connection.getConnectionString()))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@WarehouseID",
                    warehouseID
                );

                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    productID
                );

                con.Open();

                object result =
                    cmd.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
        }
    }


    // =========================================================
    // GET COST PRICE
    // =========================================================

    private decimal GetProductCostPrice(
        int productID)
    {
        string query = @"
            SELECT CostPrice
            FROM tbl_Products
            WHERE ProductID = @ProductID";

        using (SqlConnection con =
            new SqlConnection(
                Connection.getConnectionString()))
        {
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    productID
                );

                con.Open();

                object result =
                    cmd.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToDecimal(result);
            }
        }
    }


    // =========================================================
    // SAVE CURRENT REPEATER VALUES
    // =========================================================

    private void SaveRepeaterValues()
    {
        DataTable dt =  ViewState["SalesItems"] as DataTable;


        if (dt == null)
        {
            dt = CreateSalesTable();
        }


        while (dt.Rows.Count <
               rptItems.Items.Count)
        {
            DataRow newRow =
                dt.NewRow();

            newRow["ProductID"] = 0;
            newRow["Quantity"] = 1;
            newRow["SalePrice"] = 0;

            dt.Rows.Add(newRow);
        }


        for (int i = 0; i < rptItems.Items.Count; i++)
        {
            RepeaterItem item = rptItems.Items[i];

            DropDownList ddlProduct =  item.FindControl("ddlProduct") as DropDownList;

            TextBox txtQuantity = item.FindControl("txtQuantity") as TextBox;

            TextBox txtRate = item.FindControl("txtRate") as TextBox;


            int productID = 0;
            int quantity = 1;
            decimal rate = 0;


            if (ddlProduct != null)
            {
                int.TryParse(
                    ddlProduct.SelectedValue,
                    out productID
                );
            }


            if (txtQuantity != null)
            {
                int.TryParse(
                    txtQuantity.Text,
                    out quantity
                );
            }


            if (txtRate != null)
            {
                decimal.TryParse(
                    txtRate.Text,
                    out rate
                );
            }


            dt.Rows[i]["ProductID"] =
                productID;

            dt.Rows[i]["Quantity"] =
                quantity;

            dt.Rows[i]["SalePrice"] =
                rate;
        }


        ViewState["SalesItems"] = dt;
    }


    // =========================================================
    // ADD ROW
    // =========================================================

    protected void btnAddRow_Click(
        object sender,
        EventArgs e)
    {
        SaveRepeaterValues();


        DataTable dt =
            ViewState["SalesItems"] as DataTable;


        DataRow row =
            dt.NewRow();

        row["ProductID"] = 0;
        row["Quantity"] = 1;
        row["SalePrice"] = 0;

        dt.Rows.Add(row);


        ViewState["SalesItems"] = dt;


        BindRepeater();
    }


    // =========================================================
    // DELETE ROW
    // =========================================================

    protected void rptItems_ItemCommand(
        object source,
        RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "DeleteRow")
        {
            return;
        }


        SaveRepeaterValues();


        int index;

        if (!int.TryParse(
            e.CommandArgument.ToString(),
            out index))
        {
            return;
        }


        DataTable dt =
            ViewState["SalesItems"] as DataTable;


        if (dt == null)
        {
            return;
        }


        if (index >= 0 &&
            index < dt.Rows.Count)
        {
            dt.Rows.RemoveAt(index);
        }


        // Always keep one empty row
        if (dt.Rows.Count == 0)
        {
            DataRow row =
                dt.NewRow();

            row["ProductID"] = 0;
            row["Quantity"] = 1;
            row["SalePrice"] = 0;

            dt.Rows.Add(row);
        }


        ViewState["SalesItems"] = dt;


        BindRepeater();
    }


    // =========================================================
    // SAVE SALE
    // =========================================================

    protected void btnSave_Click(
        object sender,
        EventArgs e)
    {
        SaveRepeaterValues();


        string validationMessage =
            ValidateSale();


        if (validationMessage != "")
        {
            ShowMessage(
                validationMessage,
                "danger"
            );

            return;
        }


        DataTable dt =
            ViewState["SalesItems"] as DataTable;


        int customerID =
            Convert.ToInt32(
                ddlCustomer.SelectedValue
            );


        int warehouseID =
            Convert.ToInt32(
                ddlWarehouse.SelectedValue
            );


        DateTime salesDate;

        if (!DateTime.TryParse(
            txtSalesDate.Text,
            out salesDate))
        {
            ShowMessage(
                "Please enter a valid sale date.",
                "danger"
            );

            return;
        }


        decimal grandTotal = 0;


        foreach (DataRow row in dt.Rows)
        {
            int quantity =
                Convert.ToInt32(
                    row["Quantity"]
                );

            decimal rate =
                Convert.ToDecimal(
                    row["SalePrice"]
                );

            grandTotal +=
                quantity * rate;
        }


        string invoiceNumber =
            txtInvoiceNumber.Text.Trim();


        using (SqlConnection con =
            new SqlConnection(
                Connection.getConnectionString()))
        {
            con.Open();


            SqlTransaction transaction =
                con.BeginTransaction();


            try
            {
                // ---------------------------------------------
                // SALES HEADER
                // ---------------------------------------------

                string insertSale = @"
                    INSERT INTO tbl_Sales
                    (
                        InvoiceNumber,
                        CustomerID,
                        WarehouseID,
                        SalesDate,
                        TotalAmount,
                        CreatedDate
                    )
                    VALUES
                    (
                        @InvoiceNumber,
                        @CustomerID,
                        @WarehouseID,
                        @SalesDate,
                        @TotalAmount,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";


                int salesID;


                using (SqlCommand cmd =
                    new SqlCommand(
                        insertSale,
                        con,
                        transaction))
                {
                    cmd.Parameters.AddWithValue(
                        "@InvoiceNumber",
                        invoiceNumber
                    );

                    cmd.Parameters.AddWithValue(
                        "@CustomerID",
                        customerID
                    );

                    cmd.Parameters.AddWithValue(
                        "@WarehouseID",
                        warehouseID
                    );

                    cmd.Parameters.AddWithValue(
                        "@SalesDate",
                        salesDate
                    );

                    cmd.Parameters.AddWithValue(
                        "@TotalAmount",
                        grandTotal
                    );


                    salesID =
                        Convert.ToInt32(
                            cmd.ExecuteScalar()
                        );
                }


                // ---------------------------------------------
                // EACH ITEM
                // ---------------------------------------------

                foreach (DataRow row in dt.Rows)
                {
                    int productID =
                        Convert.ToInt32(
                            row["ProductID"]
                        );

                    int quantity =
                        Convert.ToInt32(
                            row["Quantity"]
                        );

                    decimal rate =
                        Convert.ToDecimal(
                            row["SalePrice"]
                        );

                    decimal amount =
                        quantity * rate;


                    // -----------------------------------------
                    // WAREHOUSE STOCK
                    // -----------------------------------------

                    string updateWarehouseStock = @"
                        UPDATE tbl_WarehouseStock
                        SET Quantity = Quantity - @Quantity
                        WHERE WarehouseID = @WarehouseID
                            AND ProductID = @ProductID
                            AND Quantity >= @Quantity";


                    int warehouseRows;


                    using (SqlCommand cmd =
                        new SqlCommand(
                            updateWarehouseStock,
                            con,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity
                        );

                        cmd.Parameters.AddWithValue(
                            "@WarehouseID",
                            warehouseID
                        );

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productID
                        );


                        warehouseRows =
                            cmd.ExecuteNonQuery();
                    }


                    if (warehouseRows == 0)
                    {
                        throw new Exception(
                            "Insufficient warehouse stock for one of the selected products."
                        );
                    }


                    // -----------------------------------------
                    // TOTAL PRODUCT STOCK
                    // -----------------------------------------

                    string updateProductStock = @"
                        UPDATE tbl_Products
                        SET CurrentStock =
                            CurrentStock - @Quantity
                        WHERE ProductID = @ProductID
                            AND ISNULL(CurrentStock, 0)
                                >= @Quantity";


                    int productRows;


                    using (SqlCommand cmd =
                        new SqlCommand(
                            updateProductStock,
                            con,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity
                        );

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productID
                        );


                        productRows =
                            cmd.ExecuteNonQuery();
                    }


                    if (productRows == 0)
                    {
                        throw new Exception(
                            "Insufficient total stock for one of the selected products."
                        );
                    }


                    // -----------------------------------------
                    // SALES DETAILS
                    // -----------------------------------------

                    string insertDetail = @"
                        INSERT INTO tbl_SalesDetails
                        (
                            SalesID,
                            ProductID,
                            Quantity,
                            SalePrice,
                            TotalAmount
                        )
                        VALUES
                        (
                            @SalesID,
                            @ProductID,
                            @Quantity,
                            @SalePrice,
                            @TotalAmount
                        )";


                    using (SqlCommand cmd =
                        new SqlCommand(
                            insertDetail,
                            con,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SalesID",
                            salesID
                        );

                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productID
                        );

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity
                        );

                        cmd.Parameters.AddWithValue(
                            "@SalePrice",
                            rate
                        );

                        cmd.Parameters.AddWithValue(
                            "@TotalAmount",
                            amount
                        );


                        cmd.ExecuteNonQuery();
                    }


                    // -----------------------------------------
                    // SALES LEDGER
                    // -----------------------------------------

                    string insertLedger = @"
                        INSERT INTO tbl_SalesLedger
                        (
                            ProductID,
                            QuantitySold,
                            SalePrice,
                            SaleDate
                        )
                        VALUES
                        (
                            @ProductID,
                            @Quantity,
                            @SalePrice,
                            @SaleDate
                        )";


                    using (SqlCommand cmd =
                        new SqlCommand(
                            insertLedger,
                            con,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            productID
                        );

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            quantity
                        );

                        cmd.Parameters.AddWithValue(
                            "@SalePrice",
                            rate
                        );

                        cmd.Parameters.AddWithValue(
                            "@SaleDate",
                            salesDate
                        );


                        cmd.ExecuteNonQuery();
                    }
                }


                transaction.Commit();


                Response.Redirect(
                    "SalesList.aspx?saved=" +
                    Server.UrlEncode(
                        invoiceNumber
                    )
                );
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }


                ShowMessage(
                    "Sale could not be saved. " +
                    ex.Message,
                    "danger"
                );
            }
        }
    }


    // =========================================================
    // VALIDATION
    // =========================================================

    private string ValidateSale()
    {
        if (ddlCustomer.SelectedValue == "")
        {
            return "Please select a customer.";
        }


        if (ddlWarehouse.SelectedValue == "")
        {
            return "Please select a warehouse.";
        }


        DateTime salesDate;

        if (!DateTime.TryParse(
            txtSalesDate.Text,
            out salesDate))
        {
            return "Please enter a valid sale date.";
        }


        if (string.IsNullOrEmpty(
            txtInvoiceNumber.Text.Trim()))
        {
            return "Invoice number could not be generated.";
        }


        DataTable dt =
            ViewState["SalesItems"] as DataTable;


        if (dt == null ||
            dt.Rows.Count == 0)
        {
            return "Please add at least one product.";
        }


        for (int i = 0;
            i < dt.Rows.Count;
            i++)
        {
            int productID =
                Convert.ToInt32(
                    dt.Rows[i]["ProductID"]
                );


            int quantity =
                Convert.ToInt32(
                    dt.Rows[i]["Quantity"]
                );


            decimal rate =
                Convert.ToDecimal(
                    dt.Rows[i]["SalePrice"]
                );


            if (productID <= 0)
            {
                return
                    "Please select a product in row "
                    + (i + 1).ToString()
                    + ".";
            }


            if (quantity <= 0)
            {
                return
                    "Quantity must be greater than zero in row "
                    + (i + 1).ToString()
                    + ".";
            }


            if (rate < 0)
            {
                return "Sale rate cannot be negative.";
            }


            // Duplicate product
            for (int j = i + 1;
                j < dt.Rows.Count;
                j++)
            {
                int otherProductID =
                    Convert.ToInt32(
                        dt.Rows[j]["ProductID"]
                    );


                if (productID ==
                    otherProductID)
                {
                    return
                        "The same product cannot be added twice. "
                        + "Please combine the quantities.";
                }
            }


            int warehouseID =
                Convert.ToInt32(
                    ddlWarehouse.SelectedValue
                );


            int available =
                GetWarehouseStock(
                    warehouseID,
                    productID
                );


            if (quantity > available)
            {
                return
                    "Insufficient stock for product in row "
                    + (i + 1).ToString()
                    + ". Available stock: "
                    + available.ToString()
                    + ".";
            }
        }


        return "";
    }


    // =========================================================
    // CANCEL
    // =========================================================

    protected void btnCancel_Click(
        object sender,
        EventArgs e)
    {
        Response.Redirect(
            "SalesList.aspx"
        );
    }


    // =========================================================
    // MESSAGE
    // =========================================================

    private void ShowMessage(
        string message,
        string type)
    {
        divMessage.Attributes["class"] =
            "alert alert-" + type;

        lblMessage.Text =
            message;
    }
}