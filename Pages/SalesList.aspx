<%@ Page Title="Sales List" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="SalesList.aspx.cs" Inherits="Pages_SalesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- ================= HEADER ================= -->

        <div class="d-flex justify-content-between align-items-center mb-4">

            <div>
                <h4 class="fw-bold mb-1">Sales List</h4>

                <p class="text-muted mb-0">
                    View completed sales and invoices.
                </p>
            </div>

            <a href="Sales.aspx" class="btn btn-primary">
                <i class="bx bx-plus me-1"></i>
                New Sale
            </a>

        </div>

        <!-- ================= MESSAGE ================= -->

        <div id="divMessage" runat="server" class="alert d-none">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>

        <!-- ================= SEARCH ================= -->

        <div class="card mb-4">

            <div class="card-body">

                <div class="row g-3 align-items-end">

                    <div class="col-md-5">

                        <label class="form-label">Search </label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Invoice number or customer name"> </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">From Date </label>
                        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"> </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">To Date </label>
                        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"> </asp:TextBox>

                    </div>

                    <div class="col-md-1">
                        <asp:Button ID="btnSearch" runat="server" Text="Go" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                    </div>

                </div>

            </div>

        </div>

        <!-- ================= SALES TABLE ================= -->

        <div class="card">

            <div class="card-header">
                <h5 class="mb-0">Sales Transactions</h5>
            </div>

            <div class="card-body">

                <div class="table-responsive">

                    <%--<asp:GridView ID="gvSales" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover align-middle" GridLines="None" DataKeyNames="SalesID" OnRowCommand="gvSales_RowCommand" EmptyDataText="No sales found.">--%>
                    <asp:GridView ID="gvSales" runat="server" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvSales_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-hover align-middle text-nowrap small m-0" GridLines="None" DataKeyNames="SalesID" OnRowCommand="gvSales_RowCommand" EmptyDataText="No sales found." PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="&laquo;" PagerSettings-LastPageText="&raquo;" PagerSettings-PageButtonCount="5">
                        <PagerStyle CssClass="gridview-pagination p-3 border-top bg-light" />

                        <Columns>

                            <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice No." />
                            <asp:BoundField DataField="SalesDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" />
                            <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                            <asp:BoundField DataField="WarehouseName" HeaderText="Warehouse" />
                            <asp:BoundField DataField="ItemCount" HeaderText="Items" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="₹ {0:N2}" />

                            <asp:TemplateField HeaderText="Payment">
                                <ItemTemplate>
                                    <span class='<%# GetPaymentStatusClass(Eval("PaymentStatus").ToString()) %>'>
                                        <%# Eval("PaymentStatus") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Due">
                                <ItemTemplate>
                                    ₹ <%# Convert.ToDecimal(Eval("AmountDue")).ToString("N2") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Actions">

                                <ItemTemplate>

                                    <asp:LinkButton ID="btnView" runat="server" CommandName="ViewSale" CommandArgument='<%# Eval("SalesID") %>' CssClass="btn btn-sm btn-outline-primary me-1">
                                        <i class="bx bx-show"></i>  View
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnPrint" runat="server" CommandName="PrintSale" CommandArgument='<%# Eval("SalesID") %>' CssClass="btn btn-sm btn-outline-secondary">
                                        <i class="bx bx-printer"></i> Print
                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                         <%--<PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />--%>

                    </asp:GridView>

                </div>

            </div>

        </div>


        <!-- ================= DETAILS PANEL ================= -->

        <asp:Panel ID="pnlDetails" runat="server" Visible="false" CssClass="card mt-4">

            <div class="card-header d-flex justify-content-between">

                <h5 class="mb-0">Sale Details </h5>
                <asp:Button ID="btnCloseDetails" runat="server" Text="Close" CssClass="btn btn-sm btn-outline-secondary" CausesValidation="false" OnClick="btnCloseDetails_Click" />

            </div>


            <div class="card-body">

                <!-- SALE INFORMATION -->

                <div class="row mb-4">

                    <div class="col-md-3">

                        <small class="text-muted">Invoice</small>

                        <h6>
                            <asp:Label ID="lblDetailInvoice" runat="server"> </asp:Label>
                        </h6>

                    </div>


                    <div class="col-md-3">

                        <small class="text-muted">Date</small>

                        <h6>
                            <asp:Label ID="lblDetailDate" runat="server"> </asp:Label>
                        </h6>

                    </div>


                    <div class="col-md-3">
                        <small class="text-muted">Customer</small>
                        <h6>
                            <asp:Label ID="lblDetailCustomer" runat="server"> </asp:Label>
                        </h6>
                    </div>


                    <div class="col-md-3">

                        <small class="text-muted">Warehouse</small>
                        <h6>
                            <asp:Label ID="lblDetailWarehouse" runat="server"> </asp:Label>
                        </h6>

                    </div>

                </div>


                <!-- PRODUCT DETAILS -->

                <div class="table-responsive">

                    <asp:GridView ID="gvSaleDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">

                        <Columns>
                            <asp:BoundField DataField="ProductName" HeaderText="Product" />
                            <asp:BoundField DataField="SKU" HeaderText="SKU" />
                            <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                            <asp:BoundField DataField="SalePrice" HeaderText="Sale Rate" DataFormatString="₹ {0:N2}" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="Amount" DataFormatString="₹ {0:N2}" />
                        </Columns>

                    </asp:GridView>

                </div>


                <!-- PAYMENT SUMMARY -->

                <div class="row justify-content-end mt-4">

                    <div class="col-md-5">

                        <div class="border rounded p-3">

                            <div class="d-flex justify-content-between mb-2">

                                <span>Invoice Total</span>
                                <strong>₹
                                    <asp:Label ID="lblDetailTotal" runat="server"> </asp:Label>
                                </strong>

                            </div>

                            <div class="d-flex justify-content-between mb-2">

                                <span>Amount Paid</span>
                                <strong>₹
                                    <asp:Label ID="lblDetailPaid" runat="server"> </asp:Label>
                                </strong>

                            </div>

                            <div class="d-flex justify-content-between mb-2">

                                <span>Amount Due</span>
                                <strong>₹
                                    <asp:Label ID="lblDetailDue" runat="server"> </asp:Label></strong>

                            </div>

                            <hr />

                            <div class="d-flex justify-content-between align-items-center">

                                <span>Payment Status</span>
                                <asp:Label ID="lblDetailPaymentStatus" runat="server" CssClass="badge bg-label-warning"> </asp:Label>

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </asp:Panel>

        <!-- ================= PRINTABLE INVOICE ================= -->

        <asp:Panel ID="pnlPrintInvoice" runat="server" CssClass="print-invoice-container mt-4" Visible="false">


            <!-- ================================================= -->
            <!-- SELLER COPY -->
            <!-- ================================================= -->

            <div class="invoice-copy border p-4">

                <div class="text-center mb-4">

                    <h3 class="fw-bold">SALES INVOICE
                    </h3>

                    <h5>SELLER COPY
                    </h5>

                    <div>
                        Invoice:
                        <asp:Label ID="lblPrintInvoiceSeller" runat="server"> </asp:Label>
                    </div>

                </div>


                <div class="row mb-4">

                    <div class="col-md-6">

                        <strong>Customer:</strong>

                        <asp:Label
                            ID="lblPrintCustomerSeller"
                            runat="server">
                        </asp:Label>

                    </div>


                    <div class="col-md-6">

                        <strong>Warehouse:</strong>

                        <asp:Label
                            ID="lblPrintWarehouseSeller"
                            runat="server">
                        </asp:Label>

                    </div>

                </div>


                <!-- PRODUCTS -->

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvPrintDetailsSeller"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered">

                        <Columns>

                            <asp:BoundField
                                DataField="ProductName"
                                HeaderText="Product" />

                            <asp:BoundField
                                DataField="Quantity"
                                HeaderText="Qty" />

                            <asp:BoundField
                                DataField="SalePrice"
                                HeaderText="Rate"
                                DataFormatString="₹ {0:N2}" />

                            <asp:BoundField
                                DataField="TotalAmount"
                                HeaderText="Amount"
                                DataFormatString="₹ {0:N2}" />

                        </Columns>

                    </asp:GridView>

                </div>


                <!-- PAYMENT SUMMARY -->

                <div class="payment-summary mt-4">

                    <div class="row justify-content-end">

                        <div class="col-md-5">

                            <div class="border rounded p-3">

                                <div class="d-flex justify-content-between mb-2">

                                    <span>Invoice Total</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintTotalSeller"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <div class="d-flex justify-content-between mb-2">

                                    <span>Amount Paid</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintPaidSeller"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <div class="d-flex justify-content-between mb-2">

                                    <span>Amount Due</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintDueSeller"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <hr />


                                <div class="d-flex justify-content-between">

                                    <strong>Payment Status</strong>

                                    <strong>
                                        <asp:Label
                                            ID="lblPrintPaymentStatusSeller"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>

                            </div>

                        </div>

                    </div>

                </div>


                <div class="row mt-5">

                    <div class="col-6">
                        Seller Signature
                    </div>

                    <div class="col-6 text-end">
                        Customer Signature
                    </div>

                </div>

            </div>


            <!-- ================================================= -->
            <!-- CUSTOMER COPY -->
            <!-- ================================================= -->

            <div class="invoice-copy border p-4">

                <div class="text-center mb-4">

                    <h3 class="fw-bold">SALES INVOICE
                    </h3>

                    <h5>CUSTOMER COPY
                    </h5>

                    <div>
                        Invoice:
                        <asp:Label
                            ID="lblPrintInvoiceCustomer"
                            runat="server">
                        </asp:Label>
                    </div>

                </div>


                <div class="row mb-4">

                    <div class="col-md-6">

                        <strong>Customer:</strong>

                        <asp:Label
                            ID="lblPrintCustomerCustomer"
                            runat="server">
                        </asp:Label>

                    </div>


                    <div class="col-md-6">

                        <strong>Warehouse:</strong>

                        <asp:Label
                            ID="lblPrintWarehouseCustomer"
                            runat="server">
                        </asp:Label>

                    </div>

                </div>


                <!-- PRODUCTS -->

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvPrintDetailsCustomer"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered">

                        <Columns>

                            <asp:BoundField
                                DataField="ProductName"
                                HeaderText="Product" />

                            <asp:BoundField
                                DataField="Quantity"
                                HeaderText="Qty" />

                            <asp:BoundField
                                DataField="SalePrice"
                                HeaderText="Rate"
                                DataFormatString="₹ {0:N2}" />

                            <asp:BoundField
                                DataField="TotalAmount"
                                HeaderText="Amount"
                                DataFormatString="₹ {0:N2}" />

                        </Columns>

                    </asp:GridView>

                </div>


                <!-- PAYMENT SUMMARY -->

                <div class="payment-summary mt-4">

                    <div class="row justify-content-end">

                        <div class="col-md-5">

                            <div class="border rounded p-3">

                                <div class="d-flex justify-content-between mb-2">

                                    <span>Invoice Total</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintTotalCustomer"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <div class="d-flex justify-content-between mb-2">

                                    <span>Amount Paid</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintPaidCustomer"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <div class="d-flex justify-content-between mb-2">

                                    <span>Amount Due</span>

                                    <strong>₹
                                        <asp:Label
                                            ID="lblPrintDueCustomer"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>


                                <hr />


                                <div class="d-flex justify-content-between">

                                    <strong>Payment Status</strong>

                                    <strong>
                                        <asp:Label
                                            ID="lblPrintPaymentStatusCustomer"
                                            runat="server">
                                        </asp:Label>
                                    </strong>

                                </div>

                            </div>

                        </div>

                    </div>

                </div>


                <div class="row mt-5">

                    <div class="col-6">
                        Seller Signature
                    </div>

                    <div class="col-6 text-end">
                        Customer Signature
                    </div>

                </div>

            </div>

        </asp:Panel>

    </div>


    <!-- ================= PRINT CSS ================= -->

    <style type="text/css">
        .print-invoice-container {
            background: white;
        }

        .invoice-copy {
            background: white;
            margin-bottom: 30px;
        }


        @media print {

            body * {
                visibility: hidden;
            }

            .print-invoice-container,
            .print-invoice-container * {
                visibility: visible;
            }

            .print-invoice-container {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                margin: 0;
            }

            .invoice-copy {
                width: 100%;
                margin: 0;
                min-height: 95vh;
            }

                .invoice-copy + .invoice-copy {
                    page-break-before: always;
                }
        }
    </style>

</asp:Content>
