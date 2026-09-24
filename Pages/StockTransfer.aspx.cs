using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_StockTransfer : System.Web.UI.Page
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
            // Clear any stale cached transfer file from previous visits
            Session.Remove("TransferDoc_Bytes");
            Session.Remove("TransferDoc_FileName");

            GenerateTransferNumber();
            LoadWarehouses();
            txtTransferDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            CreateInitialRows();
        }
        else
        {
            PreserveDocumentIfPresent();
        }
    }

    // ============================================
    // DOCUMENT PRESERVATION (ACROSS POSTBACKS)
    // ============================================
    private void PreserveDocumentIfPresent()
    {
        if (fuDocument.HasFile)
        {
            Session["TransferDoc_Bytes"] = fuDocument.FileBytes;
            Session["TransferDoc_FileName"] = fuDocument.FileName;
        }
    }

    // ============================================
    // TRANSFER NUMBER
    // ============================================
    private void GenerateTransferNumber()
    {
        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            string query = @"SELECT ISNULL(MAX(TransferID), 0) + 1 FROM tbl_StockTransfers";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                int nextID = Convert.ToInt32(cmd.ExecuteScalar());
                txtTransferNumber.Text = "ST-" + nextID.ToString("D6");
            }
        }
    }

    // ============================================
    // LOAD WAREHOUSES
    // ============================================
    private void LoadWarehouses()
    {
        string query = @"
            SELECT
                WarehouseID,
                WarehouseName
            FROM tbl_Warehouses
            ORDER BY WarehouseName";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlFromWarehouse.DataSource = dt;
                    ddlFromWarehouse.DataTextField = "WarehouseName";
                    ddlFromWarehouse.DataValueField = "WarehouseID";
                    ddlFromWarehouse.DataBind();

                    ddlToWarehouse.DataSource = dt;
                    ddlToWarehouse.DataTextField = "WarehouseName";
                    ddlToWarehouse.DataValueField = "WarehouseID";
                    ddlToWarehouse.DataBind();
                }
            }
        }

        ddlFromWarehouse.Items.Insert(0, new ListItem("-- Select Source --", ""));
        ddlToWarehouse.Items.Insert(0, new ListItem("-- Select Destination --", ""));
    }

    // ============================================
    // INITIAL ROW & TABLE SCHEMA
    // ============================================
    private void CreateInitialRows()
    {
        DataTable dt = CreateItemTable();

        DataRow row = dt.NewRow();
        row["ProductID"] = 0;
        row["Quantity"] = 1;
        row["AvailableStock"] = 0;

        dt.Rows.Add(row);
        ViewState["TransferItems"] = dt;
        BindItems();
    }

    private DataTable CreateItemTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ProductID", typeof(int));
        dt.Columns.Add("Quantity", typeof(decimal));
        dt.Columns.Add("AvailableStock", typeof(int));
        return dt;
    }

    // ============================================
    // BIND ITEMS
    // ============================================
    private void BindItems()
    {
        DataTable dt = ViewState["TransferItems"] as DataTable;

        if (dt == null)
        {
            CreateInitialRows();
            return;
        }

        rptItems.DataSource = dt;
        rptItems.DataBind();

        for (int i = 0; i < rptItems.Items.Count && i < dt.Rows.Count; i++)
        {
            RepeaterItem item = rptItems.Items[i];
            DataRow row = dt.Rows[i];

            DropDownList ddlProduct = item.FindControl("ddlProduct") as DropDownList;
            Label lblAvailableStock = item.FindControl("lblAvailableStock") as Label;
            TextBox txtQuantity = item.FindControl("txtQuantity") as TextBox;

            int productID = Convert.ToInt32(row["ProductID"]);
            decimal available = Convert.ToDecimal(row["AvailableStock"]);
            decimal quantity = Convert.ToDecimal(row["Quantity"]);

            if (ddlProduct != null)
            {
                LoadProducts(ddlProduct, productID);
            }

            if (lblAvailableStock != null)
            {
                lblAvailableStock.Text = available.ToString("0");
            }

            if (txtQuantity != null)
            {
                txtQuantity.Text = quantity.ToString("G29");
            }
        }
    }

    // ============================================
    // LOAD PRODUCTS FOR ONE ROW
    // ============================================
    private void LoadProducts(DropDownList ddlProduct, int selectedProductID)
    {
        string query = @"
            SELECT DISTINCT
                p.ProductID,
                p.ProductName,
                p.SKU
            FROM tbl_Products p
            INNER JOIN tbl_WarehouseStock ws
                ON p.ProductID = ws.ProductID
            WHERE
                ws.WarehouseID = @WarehouseID
            ORDER BY
                p.ProductName";

        ddlProduct.Items.Clear();
        ddlProduct.Items.Add(new ListItem("-- Select Product --", "0"));

        if (string.IsNullOrEmpty(ddlFromWarehouse.SelectedValue))
        {
            return;
        }

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@WarehouseID", Convert.ToInt32(ddlFromWarehouse.SelectedValue));

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = row["ProductName"].ToString() + " (" + row["SKU"].ToString() + ")";
                        item.Value = row["ProductID"].ToString();
                        ddlProduct.Items.Add(item);
                    }
                }
            }
        }

        if (selectedProductID > 0 && ddlProduct.Items.FindByValue(selectedProductID.ToString()) != null)
        {
            ddlProduct.SelectedValue = selectedProductID.ToString();
        }
    }

    // ============================================
    // SOURCE WAREHOUSE CHANGED
    // ============================================
    protected void ddlFromWarehouse_SelectedIndexChanged(object sender, EventArgs e)
    {
        PreserveDocumentIfPresent();

        DataTable dt = ReadItemsFromControls();

        foreach (DataRow row in dt.Rows)
        {
            row["ProductID"] = 0;
            row["AvailableStock"] = 0;
        }

        ViewState["TransferItems"] = dt;
        BindItems();
    }

    // ============================================
    // ADD ROW
    // ============================================
    protected void btnAddRow_Click(object sender, EventArgs e)
    {
        PreserveDocumentIfPresent();

        DataTable dt = ReadItemsFromControls();

        DataRow row = dt.NewRow();
        row["ProductID"] = 0;
        row["Quantity"] = 1;
        row["AvailableStock"] = 0;

        dt.Rows.Add(row);
        ViewState["TransferItems"] = dt;
        BindItems();
    }

    // ============================================
    // DELETE ROW
    // ============================================
    protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "DeleteRow")
        {
            PreserveDocumentIfPresent();

            int index;
            if (int.TryParse(e.CommandArgument.ToString(), out index))
            {
                DataTable dt = ReadItemsFromControls();

                if (dt.Rows.Count > 1 && index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(index);
                }

                ViewState["TransferItems"] = dt;
                BindItems();
            }
        }
    }

    // ============================================
    // PRODUCT CHANGED
    // ============================================
    protected void ddlProduct_SelectedIndexChanged(object sender, EventArgs e)
    {
        PreserveDocumentIfPresent();

        DropDownList ddlProduct = sender as DropDownList;
        RepeaterItem item = ddlProduct.NamingContainer as RepeaterItem;

        if (item == null)
        {
            return;
        }

        Label lblAvailableStock = item.FindControl("lblAvailableStock") as Label;

        int productID;
        if (!int.TryParse(ddlProduct.SelectedValue, out productID) || productID == 0)
        {
            lblAvailableStock.Text = "0";
            return;
        }

        if (string.IsNullOrEmpty(ddlFromWarehouse.SelectedValue))
        {
            lblAvailableStock.Text = "0";
            return;
        }

        decimal stock = GetWarehouseStock(Convert.ToInt32(ddlFromWarehouse.SelectedValue), productID);
        lblAvailableStock.Text = stock.ToString("0");

        SaveControlsToViewState();
    }

    // ============================================
    // GET WAREHOUSE STOCK
    // ============================================
    private decimal GetWarehouseStock(int warehouseID, int productID)
    {
        string query = @"
            SELECT
                ISNULL(Quantity, 0)
            FROM tbl_WarehouseStock
            WHERE
                WarehouseID = @WarehouseID
                AND ProductID = @ProductID";

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@WarehouseID", warehouseID);
                cmd.Parameters.AddWithValue("@ProductID", productID);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToDecimal(result);
            }
        }
    }

    // ============================================
    // READ / SAVE REPEATER CONTROLS
    // ============================================
    private DataTable ReadItemsFromControls()
    {
        DataTable dt = CreateItemTable();

        foreach (RepeaterItem item in rptItems.Items)
        {
            DropDownList ddlProduct = item.FindControl("ddlProduct") as DropDownList;
            TextBox txtQuantity = item.FindControl("txtQuantity") as TextBox;
            Label lblAvailableStock = item.FindControl("lblAvailableStock") as Label;

            DataRow row = dt.NewRow();

            int productID = 0;
            decimal quantity = 1;
            decimal available = 0;

            if (ddlProduct != null)
            {
                int.TryParse(ddlProduct.SelectedValue, out productID);
            }

            if (txtQuantity != null)
            {
                decimal.TryParse(txtQuantity.Text, out quantity);
            }

            if (lblAvailableStock != null)
            {
                decimal.TryParse(lblAvailableStock.Text, out available);
            }

            row["ProductID"] = productID;
            row["Quantity"] = quantity;
            row["AvailableStock"] = available;

            dt.Rows.Add(row);
        }

        return dt;
    }

    private void SaveControlsToViewState()
    {
        DataTable dt = ReadItemsFromControls();
        ViewState["TransferItems"] = dt;
    }

    // ============================================
    // SAVE TRANSFER
    // ============================================
    protected void btnSave_Click(object sender, EventArgs e)
    {
        divMessage.Visible = false;

        if (string.IsNullOrEmpty(ddlFromWarehouse.SelectedValue))
        {
            ShowMessage("Please select the source warehouse.", "danger");
            return;
        }

        if (string.IsNullOrEmpty(ddlToWarehouse.SelectedValue))
        {
            ShowMessage("Please select the destination warehouse.", "danger");
            return;
        }

        int fromWarehouse = Convert.ToInt32(ddlFromWarehouse.SelectedValue);
        int toWarehouse = Convert.ToInt32(ddlToWarehouse.SelectedValue);

        if (fromWarehouse == toWarehouse)
        {
            ShowMessage("Source and destination warehouses cannot be the same.", "danger");
            return;
        }

        DateTime transferDate;
        if (!DateTime.TryParse(txtTransferDate.Text, out transferDate))
        {
            ShowMessage("Please enter a valid transfer date.", "danger");
            return;
        }

        DataTable items = ReadItemsFromControls();

        if (items.Rows.Count == 0)
        {
            ShowMessage("Please add at least one product.", "danger");
            return;
        }

        // VALIDATE ITEMS
        foreach (DataRow row in items.Rows)
        {
            int productID = Convert.ToInt32(row["ProductID"]);
            decimal quantity = Convert.ToDecimal(row["Quantity"]);

            if (productID <= 0)
            {
                ShowMessage("Please select a product for every row.", "danger");
                return;
            }

            if (quantity <= 0)
            {
                ShowMessage("Transfer quantity must be greater than zero.", "danger");
                return;
            }
        }

        // CHECK DUPLICATE PRODUCTS
        for (int i = 0; i < items.Rows.Count; i++)
        {
            int productID = Convert.ToInt32(items.Rows[i]["ProductID"]);

            for (int j = i + 1; j < items.Rows.Count; j++)
            {
                int secondProductID = Convert.ToInt32(items.Rows[j]["ProductID"]);

                if (productID == secondProductID)
                {
                    ShowMessage("The same product cannot be added more than once.", "danger");
                    return;
                }
            }
        }

        // ============================================
        // DOCUMENT PROCESSING (FU CONTROL OR SESSION)
        // ============================================
        PreserveDocumentIfPresent();

        string documentFileName = null;
        string documentFilePath = null;
        byte[] fileBytes = null;
        string originalFileName = null;

        if (fuDocument.HasFile)
        {
            fileBytes = fuDocument.FileBytes;
            originalFileName = fuDocument.FileName;
        }
        else if (Session["TransferDoc_Bytes"] != null)
        {
            fileBytes = (byte[])Session["TransferDoc_Bytes"];
            originalFileName = Session["TransferDoc_FileName"].ToString();
        }

        if (fileBytes != null && fileBytes.Length > 0)
        {
            string extension = Path.GetExtension(originalFileName).ToLower();

            if (extension != ".pdf" && extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                ShowMessage("Only PDF, JPG, JPEG and PNG files are allowed.", "danger");
                return;
            }

            if (fileBytes.Length > 200 * 1024)
            {
                ShowMessage("Supporting document must be 200KB or smaller.", "danger");
                return;
            }

            string folder = Server.MapPath("~/Uploads/StockTransfers/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            documentFileName = string.Format("{0}_{1:yyyyMMddHHmmss}{2}",
                txtTransferNumber.Text.Trim(),
                DateTime.Now,
                extension);

            string physicalPath = Path.Combine(folder, documentFileName);
            File.WriteAllBytes(physicalPath, fileBytes);
            documentFilePath = "~/Uploads/StockTransfers/" + documentFileName;
        }

        // ============================================
        // DATABASE TRANSACTION
        // ============================================
        bool isSuccess = false;

        using (SqlConnection con = new SqlConnection(Connection.getConnectionString()))
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            try
            {
                // 1. INSERT HEADER
                string headerQuery = @"
                    INSERT INTO tbl_StockTransfers
                    (
                        TransferNumber,
                        TransferDate,
                        FromWarehouseID,
                        ToWarehouseID,
                        Remarks,
                        DocumentFileName,
                        DocumentFilePath
                    )
                    VALUES
                    (
                        @TransferNumber,
                        @TransferDate,
                        @FromWarehouseID,
                        @ToWarehouseID,
                        @Remarks,
                        @DocumentFileName,
                        @DocumentFilePath
                    );

                    SELECT SCOPE_IDENTITY();";

                int transferID;

                using (SqlCommand cmd = new SqlCommand(headerQuery, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@TransferNumber", txtTransferNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@TransferDate", transferDate);
                    cmd.Parameters.AddWithValue("@FromWarehouseID", fromWarehouse);
                    cmd.Parameters.AddWithValue("@ToWarehouseID", toWarehouse);
                    cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text.Trim());
                    cmd.Parameters.AddWithValue("@DocumentFileName", string.IsNullOrEmpty(documentFileName) ? (object)DBNull.Value : documentFileName);
                    cmd.Parameters.AddWithValue("@DocumentFilePath", string.IsNullOrEmpty(documentFilePath) ? (object)DBNull.Value : documentFilePath);

                    transferID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. PROCESS EACH PRODUCT
                foreach (DataRow row in items.Rows)
                {
                    int productID = Convert.ToInt32(row["ProductID"]);
                    decimal quantity = Convert.ToDecimal(row["Quantity"]);

                    // LOCK SOURCE STOCK AND CHECK AVAILABILITY
                    string stockCheckQuery = @"
                        SELECT Quantity
                        FROM tbl_WarehouseStock WITH (UPDLOCK)
                        WHERE
                            WarehouseID = @WarehouseID
                            AND ProductID = @ProductID";

                    decimal availableStock;

                    using (SqlCommand cmd = new SqlCommand(stockCheckQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@WarehouseID", fromWarehouse);
                        cmd.Parameters.AddWithValue("@ProductID", productID);

                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            throw new Exception("Product stock record was not found in the source warehouse.");
                        }

                        availableStock = Convert.ToDecimal(result);
                    }

                    if (availableStock < quantity)
                    {
                        throw new Exception("Insufficient stock for one of the selected products.");
                    }

                    // DECREASE SOURCE
                    string decreaseQuery = @"
                        UPDATE tbl_WarehouseStock
                        SET Quantity = Quantity - @Quantity
                        WHERE
                            WarehouseID = @WarehouseID
                            AND ProductID = @ProductID";

                    using (SqlCommand cmd = new SqlCommand(decreaseQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@WarehouseID", fromWarehouse);
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.ExecuteNonQuery();
                    }

                    // CHECK DESTINATION
                    string destinationCheckQuery = @"
                        SELECT COUNT(*)
                        FROM tbl_WarehouseStock
                        WHERE
                            WarehouseID = @WarehouseID
                            AND ProductID = @ProductID";

                    int destinationExists;

                    using (SqlCommand cmd = new SqlCommand(destinationCheckQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@WarehouseID", toWarehouse);
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        destinationExists = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // INSERT / UPDATE DESTINATION
                    if (destinationExists == 0)
                    {
                        string insertStockQuery = @"
                            INSERT INTO tbl_WarehouseStock
                            (
                                WarehouseID,
                                ProductID,
                                Quantity
                            )
                            VALUES
                            (
                                @WarehouseID,
                                @ProductID,
                                @Quantity
                            )";

                        using (SqlCommand cmd = new SqlCommand(insertStockQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@WarehouseID", toWarehouse);
                            cmd.Parameters.AddWithValue("@ProductID", productID);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string updateStockQuery = @"
                            UPDATE tbl_WarehouseStock
                            SET Quantity = Quantity + @Quantity
                            WHERE
                                WarehouseID = @WarehouseID
                                AND ProductID = @ProductID";

                        using (SqlCommand cmd = new SqlCommand(updateStockQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@WarehouseID", toWarehouse);
                            cmd.Parameters.AddWithValue("@ProductID", productID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // INSERT DETAIL
                    string detailQuery = @"
                        INSERT INTO tbl_StockTransferDetails
                        (
                            TransferID,
                            ProductID,
                            Quantity
                        )
                        VALUES
                        (
                            @TransferID,
                            @ProductID,
                            @Quantity
                        )";

                    using (SqlCommand cmd = new SqlCommand(detailQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TransferID", transferID);
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.ExecuteNonQuery();
                    }
                }

                // COMMIT
                transaction.Commit();
                isSuccess = true;

                // Clear session cache once saved
                Session.Remove("TransferDoc_Bytes");
                Session.Remove("TransferDoc_FileName");
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // Transaction already aborted or invalid
                    }
                }

                ShowMessage(ex.Message, "danger");
            }
        }

        // REDIRECT SAFELY OUTSIDE TRY/CATCH/USING
        if (isSuccess)
        {
            Response.Redirect("StockTransferHistory.aspx?saved=" + Server.UrlEncode(txtTransferNumber.Text), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    // ============================================
    // CANCEL
    // ============================================
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session.Remove("TransferDoc_Bytes");
        Session.Remove("TransferDoc_FileName");
        Response.Redirect("StockTransferHistory.aspx");
    }

    // ============================================
    // MESSAGE
    // ============================================
    private void ShowMessage(string message, string type)
    {
        divMessage.Visible = true;
        divMessage.Attributes["class"] = "alert alert-" + type;
        lblMessage.Text = Server.HtmlEncode(message);
    }
}