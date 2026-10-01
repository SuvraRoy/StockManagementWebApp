using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
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
            lblAdminName.Text = Session["FullName"] != null ? Session["FullName"].ToString() : Session["Username"].ToString();
            LoadDashboardMetrics();
        }
    }

    private void LoadDashboardMetrics()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();

            // 1. Core Summary Metrics
            string metricsQuery = @"
                SELECT 
                    (SELECT COUNT(*) FROM tbl_Products WHERE IsActive = 1) AS TotalProducts,
                    (SELECT ISNULL(SUM(Quantity), 0) FROM tbl_WarehouseStock) AS TotalStock,
                    (SELECT COUNT(*) FROM tbl_Warehouses WHERE IsActive = 1) AS TotalWarehouses,
                    (SELECT COUNT(*) FROM tbl_Suppliers WHERE IsActive = 1) AS TotalSuppliers,
                    (SELECT COUNT(*) 
                     FROM tbl_WarehouseStock ws
                     INNER JOIN tbl_Products p ON ws.ProductID = p.ProductID
                     INNER JOIN tbl_Warehouses w ON ws.WarehouseID = w.WarehouseID
                     WHERE p.IsActive = 1 
                       AND w.IsActive = 1
                       AND ISNULL(ws.Quantity, 0) <= ISNULL(p.ReorderLevel, 0)) AS LowStockCount;";

            using (SqlCommand cmd = new SqlCommand(metricsQuery, conn))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblProducts.Text = Convert.ToInt32(dr["TotalProducts"]).ToString("N0");
                        lblTotalStock.Text = Convert.ToInt32(dr["TotalStock"]).ToString("N0");
                        lblTotalWarehouses.Text = Convert.ToInt32(dr["TotalWarehouses"]).ToString("N0");
                        lblSuppliers.Text = Convert.ToInt32(dr["TotalSuppliers"]).ToString("N0");
                        lblLowStock.Text = Convert.ToInt32(dr["LowStockCount"]).ToString("N0");
                    }
                }
            }

            // 2. Today's Business Activity (Simplified direct WHERE queries)
            string todayQuery = @"
                SELECT 
                    ISNULL(SUM(TotalAmount), 0) AS TodayPurchasesAmt,
                    COUNT(*) AS TodayPurchasesCount
                FROM tbl_Purchases
                WHERE CAST(PurchaseDate AS DATE) = CAST(GETDATE() AS DATE);

                SELECT 
                    ISNULL(SUM(TotalAmount), 0) AS TodaySalesAmt,
                    COUNT(*) AS TodaySalesCount
                FROM tbl_Sales
                WHERE CAST(SalesDate AS DATE) = CAST(GETDATE() AS DATE);

                SELECT 
                    COUNT(*) AS TodayTransfersCount 
                FROM tbl_StockTransfers 
                WHERE CAST(TransferDate AS DATE) = CAST(GETDATE() AS DATE);";

            using (SqlCommand cmd = new SqlCommand(todayQuery, conn))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    // Purchases
                    if (dr.Read())
                    {
                        lblTodayPurchasesAmt.Text = Convert.ToDecimal(dr["TodayPurchasesAmt"]).ToString("N2");
                        lblTodayPurchasesCount.Text = dr["TodayPurchasesCount"].ToString();
                    }

                    // Sales
                    if (dr.NextResult() && dr.Read())
                    {
                        lblTodaySalesAmt.Text = Convert.ToDecimal(dr["TodaySalesAmt"]).ToString("N2");
                        lblTodaySalesCount.Text = dr["TodaySalesCount"].ToString();
                    }

                    // Stock Transfers
                    if (dr.NextResult() && dr.Read())
                    {
                        lblTodayTransfersCount.Text = dr["TodayTransfersCount"].ToString();
                    }
                }
            }

            // 3. Recent 5 Purchases
            string recentPurchasesQuery = @"
                SELECT TOP 5 
                    p.InvoiceNumber, 
                    s.CompanyName, 
                    p.PurchaseDate, 
                    p.TotalAmount
                FROM tbl_Purchases p
                INNER JOIN tbl_Suppliers s ON p.SupplierID = s.SupplierID
                ORDER BY p.PurchaseDate DESC, p.PurchaseID DESC;";

            using (SqlDataAdapter da = new SqlDataAdapter(recentPurchasesQuery, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvRecentPurchases.DataSource = dt;
                gvRecentPurchases.DataBind();
            }

            // 4. Recent 5 Sales
            string recentSalesQuery = @"
                SELECT TOP 5 
                    SalesID,  
                    SalesDate, 
                    TotalAmount
                FROM tbl_Sales
                ORDER BY SalesDate DESC, SalesID DESC;";

            using (SqlDataAdapter da = new SqlDataAdapter(recentSalesQuery, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvRecentSales.DataSource = dt;
                gvRecentSales.DataBind();
            }
        }

        // 5. Populate Location-Specific Low Stock Grid
        BindLowStockGrid();
    }

    private void BindLowStockGrid()
    {
        string connStr = Connection.getConnectionString();

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string lowStockQuery = @"
                SELECT 
                    p.ProductID,
                    p.ProductName, 
                    p.SKU, 
                    w.WarehouseID,
                    w.WarehouseName, 
                    ISNULL(ws.Quantity, 0) AS Quantity, 
                    ISNULL(p.ReorderLevel, 0) AS ReorderLevel
                FROM tbl_WarehouseStock ws
                INNER JOIN tbl_Products p ON ws.ProductID = p.ProductID
                INNER JOIN tbl_Warehouses w ON ws.WarehouseID = w.WarehouseID
                WHERE p.IsActive = 1 
                  AND w.IsActive = 1
                  AND ISNULL(ws.Quantity, 0) <= ISNULL(p.ReorderLevel, 0)
                ORDER BY ws.Quantity ASC, p.ProductName ASC;";

            using (SqlDataAdapter da = new SqlDataAdapter(lowStockQuery, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLowStock.DataSource = dt;
                gvLowStock.DataBind();
            }
        }
    }

    protected void gvLowStock_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvLowStock.PageIndex = e.NewPageIndex;
        BindLowStockGrid();
    }
}