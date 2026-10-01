<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .metric-card {
            border: 1px solid #e7e7e8;
            border-radius: 0.375rem;
            background: #fff;
            box-shadow: none !important;
        }
        .metric-card .card-body {
            padding: 1.15rem 1.25rem;
        }
        .metric-label {
            font-size: 0.8rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #8592a3;
            font-weight: 600;
        }
        .metric-value {
            font-size: 1.65rem;
            font-weight: 700;
            line-height: 1.2;
            color: #566a7f;
            margin-top: 0.35rem;
            margin-bottom: 0.35rem;
        }
        .metric-sub {
            font-size: 0.78rem;
        }
        .alert-card-warning {
            border: 1px solid #ffab00 !important;
            background-color: #fffbf3;
        }
        .alert-card-danger {
            border: 1px solid #ff3e1d !important;
            background-color: #fff7f6;
        }

        /* Spacing and layout for the GridView pager table */
    .gridview-pager table {
        margin: 1rem 1rem 0.5rem auto !important;
    }

    /* Creates distinct gaps between buttons */
    .gridview-pager td {
        padding: 0 4px !important;
        border: none !important;
    }

    /* Button styling matching Sneat theme */
    .gridview-pager a, 
    .gridview-pager span {
        display: inline-block;
        padding: 5px 12px;
        border-radius: 6px;
        font-size: 0.85rem;
        font-weight: 500;
        text-decoration: none;
        transition: all 0.2s ease-in-out;
    }

    /* Inactive / Clickable numbers and Next/Prev/First/Last links */
    .gridview-pager a {
        background-color: #f5f5f9;
        color: #697a8d;
        border: 1px solid #d9dee3;
    }

    .gridview-pager a:hover {
        background-color: #e1e4e8;
        color: #566a7f;
    }

    /* Current / Active Page Number */
    .gridview-pager span {
        background-color: #696cff;
        color: #ffffff !important;
        border: 1px solid #696cff;
        box-shadow: 0 2px 4px rgba(105, 108, 255, 0.4);
    }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- 1. HEADER & QUICK ACTIONS -->
        <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
            <div>
                <h4 class="fw-bold mb-1">
                    Good Day, <asp:Label ID="lblAdminName" runat="server" Text="Admin"></asp:Label>
                </h4>
                <p class="text-muted mb-0">Here is what is happening with your inventory today.</p>
            </div>
            <div class="d-flex gap-2">
                <a href="Pages/Products.aspx" class="btn btn-sm btn-outline-secondary">
                    <i class="bx bx-plus me-1"></i> Add Product
                </a>
                <a href="Pages/Purchases.aspx" class="btn btn-sm btn-outline-primary">
                    <i class="bx bx-cart-add me-1"></i> New Purchase
                </a>
                <a href="Pages/Sales.aspx" class="btn btn-sm btn-primary">
                    <i class="bx bx-receipt me-1"></i> New Sale
                </a>
                <a href="Pages/StockTransfer.aspx" class="btn btn-sm btn-outline-info">
                    <i class="bx bx-transfer-alt me-1"></i> Stock Transfer
                </a>
            </div>
        </div>

        <!-- 2. TOP SUMMARY CARDS -->
        <div class="row g-3 mb-4">
            <div class="col-sm-6 col-lg-3">
                <div class="card metric-card h-100">
                    <div class="card-body">
                        <span class="metric-label">Total Products</span>
                        <div class="metric-value">
                            <asp:Label ID="lblProducts" runat="server" Text="0"></asp:Label>
                        </div>
                        <small class="text-success fw-semibold"><i class="bx bx-check-circle"></i> Active in Catalog</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-lg-3">
                <div class="card metric-card h-100">
                    <div class="card-body">
                        <span class="metric-label">Total Stock</span>
                        <div class="metric-value">
                            <asp:Label ID="lblTotalStock" runat="server" Text="0"></asp:Label>
                        </div>
                        <small class="text-muted">Total units stored</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-lg-3">
                <div class="card metric-card h-100 alert-card-danger">
                    <div class="card-body">
                        <span class="metric-label text-danger">Low Stock Items</span>
                        <div class="metric-value text-danger">
                            <asp:Label ID="lblLowStock" runat="server" Text="0"></asp:Label>
                        </div>
                        <small class="text-danger fw-semibold"><i class="bx bx-error"></i> Needs Attention</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-lg-3">
                <div class="card metric-card h-100">
                    <div class="card-body">
                        <span class="metric-label">Warehouses</span>
                        <div class="metric-value">
                            <asp:Label ID="lblTotalWarehouses" runat="server" Text="0"></asp:Label>
                        </div>
                        <small class="text-muted">Operational locations</small>
                    </div>
                </div>
            </div>
        </div>

        <!-- 3. TODAY'S BUSINESS & OPERATIONAL ALERTS (What happened today? What needs attention?) -->
        <div class="row g-3 mb-4">
            <div class="col-lg-8">
                <div class="card metric-card h-100">
                    <div class="card-header border-bottom py-3 d-flex justify-content-between align-items-center">
                        <h6 class="mb-0 fw-bold">Today's Business Activity</h6>
                        <span class="badge bg-label-secondary"><%= DateTime.Now.ToString("dd MMM yyyy") %></span>
                    </div>
                    <div class="card-body py-4">
                        <div class="row text-center gy-3">
                            <div class="col-6 col-sm-3 border-end">
                                <span class="metric-label d-block mb-1">Today's Sales</span>
                                <h4 class="text-primary mb-1 fw-bold">
                                    ₹ <asp:Label ID="lblTodaySalesAmt" runat="server" Text="0.00"></asp:Label>
                                </h4>
                                <small class="text-muted"><asp:Label ID="lblTodaySalesCount" runat="server" Text="0"></asp:Label> Invoices</small>
                            </div>
                            <div class="col-6 col-sm-3 border-end">
                                <span class="metric-label d-block mb-1">Today's Purchases</span>
                                <h4 class="text-secondary mb-1 fw-bold">
                                    ₹ <asp:Label ID="lblTodayPurchasesAmt" runat="server" Text="0.00"></asp:Label>
                                </h4>
                                <small class="text-muted"><asp:Label ID="lblTodayPurchasesCount" runat="server" Text="0"></asp:Label> Orders</small>
                            </div>
                            <div class="col-6 col-sm-3 border-end">
                                <span class="metric-label d-block mb-1">Stock Transfers</span>
                                <h4 class="text-info mb-1 fw-bold">
                                    <asp:Label ID="lblTodayTransfersCount" runat="server" Text="0"></asp:Label>
                                </h4>
                                <small class="text-muted">Inter-branch logs</small>
                            </div>
                            <div class="col-6 col-sm-3">
                                <span class="metric-label d-block mb-1">Total Suppliers</span>
                                <h4 class="text-dark mb-1 fw-bold">
                                    <asp:Label ID="lblSuppliers" runat="server" Text="0"></asp:Label>
                                </h4>
                                <small class="text-muted">Registered vendors</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="card metric-card h-100 alert-card-warning">
                    <div class="card-header border-bottom py-3">
                        <span class="badge bg-warning text-dark mb-1">Inventory Health</span>
                        <h6 class="mb-0 fw-bold text-dark">Stock Alert Summary</h6>
                    </div>
                    <div class="card-body d-flex flex-column justify-content-between py-3">
                        <p class="text-muted small mb-3">
                            Items are evaluated per warehouse quantity. Any item falling below its reorder threshold is flagged below.
                        </p>
                        <div>
                            <a href="#low-stock-grid" class="btn btn-sm btn-outline-dark w-100">
                                View Shortage List &darr;
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- 4. RECENT ACTIVITY (Purchases & Sales side-by-side) -->
        <div class="row g-3 mb-4">
            <div class="col-lg-6">
                <div class="card metric-card h-100">
                    <div class="card-header border-bottom py-3 d-flex justify-content-between align-items-center">
                        <h6 class="mb-0 fw-bold">Recent Purchases</h6>
                        <a href="Pages/PurchaseHistory.aspx" class="btn btn-xs btn-outline-secondary">View All</a>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvRecentPurchases" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-hover table-sm text-nowrap mb-0" GridLines="None" 
                            EmptyDataText="<div class='p-3 text-muted text-center'>No recent purchases recorded.</div>">
                            <Columns>
                                <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice" ItemStyle-CssClass="fw-semibold" />
                                <asp:BoundField DataField="CompanyName" HeaderText="Supplier" />
                                <asp:BoundField DataField="PurchaseDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                                <asp:BoundField DataField="TotalAmount" HeaderText="Amount (₹)" DataFormatString="{0:N2}" ItemStyle-CssClass="fw-bold text-end" HeaderStyle-CssClass="text-end" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card metric-card h-100">
                    <div class="card-header border-bottom py-3 d-flex justify-content-between align-items-center">
                        <h6 class="mb-0 fw-bold">Recent Sales</h6>
                        <a href="Pages/SalesList.aspx" class="btn btn-xs btn-outline-secondary">View All</a>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvRecentSales" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-hover table-sm text-nowrap mb-0" GridLines="None" 
                            EmptyDataText="<div class='p-3 text-muted text-center'>No recent sales recorded.</div>">
                            <Columns>
                                <asp:BoundField DataField="SalesID" HeaderText="Order #" DataFormatString="SO-{0:D5}" ItemStyle-CssClass="fw-semibold" />
                                <%--<asp:BoundField DataField="CustomerName" HeaderText="Customer" />--%>
                                <asp:BoundField DataField="SalesDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                                <asp:BoundField DataField="TotalAmount" HeaderText="Amount (₹)" DataFormatString="{0:N2}" ItemStyle-CssClass="fw-bold text-end" HeaderStyle-CssClass="text-end" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <!-- 5. LOW STOCK PRODUCTS BY WAREHOUSE LOCATION -->
        <div id="low-stock-grid" class="card metric-card">
            <div class="card-header border-bottom py-3 d-flex justify-content-between align-items-center">
                <div>
                    <h6 class="mb-0 fw-bold text-danger">
                        <i class="bx bx-error-circle me-1"></i> Low Stock Products (Location-Specific)
                    </h6>
                    <small class="text-muted">Shortages based strictly on individual warehouse quantities vs product reorder levels.</small>
                </div>
            </div>
            <div class="table-responsive">

                <asp:GridView ID="gvLowStock" runat="server" AutoGenerateColumns="False" AllowPaging="true" OnPageIndexChanging="gvLowStock_PageIndexChanging" PageSize="10" CssClass="table table-hover align-middle mb-0" GridLines="None" EmptyDataText="<div class='p-4 text-center text-success'><i class='bx bx-check-shield fs-4 d-block mb-1'></i>All items across all warehouses are operating above safety reorder thresholds.</div>">

                    
                    <PagerStyle CssClass ="gridview-pager" HorizontalAlign="Justify" />
                    <PagerSettings Mode="NextPreviousFirstLast" FirstPageText="« First" PreviousPageText="‹ Prev" NextPageText="Next ›" LastPageText="Last »"  PageButtonCount="5" Position="Bottom"/>


                    <Columns>

                        <asp:TemplateField HeaderText ="Sl.No." ItemStyle-CssClass="text-center text-muted fw-semibold" HeaderStyle-CssClass="text-center" HeaderStyle-Width="50px">
                            <ItemTemplate>
                                <%# (gvLowStock.PageIndex * gvLowStock.PageSize) + Container.DataItemIndex + 1 %>
                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:BoundField DataField="ProductName" HeaderText="Product Name" ItemStyle-CssClass="fw-semibold text-dark" />
                        <asp:BoundField DataField="SKU" HeaderText="SKU" />
                        <asp:BoundField DataField="WarehouseName" HeaderText="Warehouse Location" />
                        <asp:TemplateField HeaderText="Current Stock" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <span class="badge bg-label-danger fw-bold"><%# Eval("Quantity") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" ItemStyle-CssClass="text-center text-muted" HeaderStyle-CssClass="text-center" />
                        
                        <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
    <ItemTemplate>
        <a href='Pages/Reorder.aspx?pid=<%# Eval("ProductID") %>&wid=<%# Eval("WarehouseID") %>' 
           class="btn btn-xs btn-outline-primary">
            <i class="bx bx-cart-add me-1"></i> Reorder
        </a>
    </ItemTemplate>
</asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>