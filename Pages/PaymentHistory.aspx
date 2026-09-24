<%@ Page Title="Payment History" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="PaymentHistory.aspx.cs"
    Inherits="Pages_PaymentHistory" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">

            <div>
                <h4 class="fw-bold mb-1">Payment History</h4>

                <p class="text-muted mb-0">
                    View and search payments received against sales invoices.
                </p>
            </div>

            <a href="Payments.aspx"
                class="btn btn-primary">

                <i class="bx bx-plus me-1"></i>
                Record Payment

            </a>

        </div>


        <!-- Message -->
        <div id="divMessage"
            runat="server"
            class="alert d-none">

            <asp:Label
                ID="lblMessage"
                runat="server">
            </asp:Label>

        </div>


        <!-- Search and Filters -->
        <div class="card mb-4">

            <div class="card-header">

                <h5 class="mb-0">Search & Filter
                </h5>

            </div>


            <div class="card-body">

                <div class="row g-3 align-items-end">

                    <!-- Search -->
                    <div class="col-md-5">

                        <label class="form-label">
                            Search
                        </label>

                        <asp:TextBox
                            ID="txtSearch"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Invoice, customer or reference number">
                        </asp:TextBox>

                    </div>


                    <!-- From Date -->
                    <div class="col-md-3">

                        <label class="form-label">
                            From Date
                        </label>

                        <asp:TextBox
                            ID="txtFromDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- To Date -->
                    <div class="col-md-3">

                        <label class="form-label">
                            To Date
                        </label>

                        <asp:TextBox
                            ID="txtToDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Search Button -->
                    <div class="col-md-1">

                        <asp:Button
                            ID="btnSearch"
                            runat="server"
                            Text="Go"
                            CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- Payment Summary -->
        <div class="row mb-4">

            <!-- Total Payments -->
            <div class="col-md-4">

                <div class="card">

                    <div class="card-body">

                        <div class="d-flex justify-content-between">

                            <div>
                                <span class="text-muted">Payments Found
                                </span>

                                <h4 class="mb-0">
                                    <asp:Label
                                        ID="lblPaymentCount"
                                        runat="server">
                                        0
                                    </asp:Label>
                                </h4>
                            </div>

                            <div>
                                <i class="bx bx-receipt fs-2 text-primary"></i>
                            </div>

                        </div>

                    </div>

                </div>

            </div>


            <!-- Total Received -->
            <div class="col-md-4">

                <div class="card">

                    <div class="card-body">

                        <div class="d-flex justify-content-between">

                            <div>
                                <span class="text-muted">Total Received
                                </span>

                                <h4 class="mb-0">₹
                                    <asp:Label
                                        ID="lblTotalReceived"
                                        runat="server">
                                        0.00
                                    </asp:Label>
                                </h4>
                            </div>

                            <div>
                                <i class="bx bx-money fs-2 text-success"></i>
                            </div>

                        </div>

                    </div>

                </div>

            </div>


            <!-- Current Filter -->
            <div class="col-md-4">

                <div class="card">

                    <div class="card-body">

                        <div class="d-flex justify-content-between">

                            <div>
                                <span class="text-muted">Payment Modes
                                </span>

                                <h4 class="mb-0">
                                    <asp:Label
                                        ID="lblPaymentModes"
                                        runat="server">
                                        0
                                    </asp:Label>
                                </h4>
                            </div>

                            <div>
                                <i class="bx bx-wallet fs-2 text-info"></i>
                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- Payment Table -->
        <div class="card">

            <div class="card-header">

                <h5 class="mb-0">Payment Transactions
                </h5>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvPayments"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover align-middle"
                        GridLines="None"
                        EmptyDataText="No payments found.">

                        <Columns>

                            <asp:BoundField
                                DataField="PaymentID"
                                HeaderText="Payment ID" />


                            <asp:BoundField
                                DataField="PaymentDate"
                                HeaderText="Date"
                                DataFormatString="{0:dd-MM-yyyy}" />

                            <asp:TemplateField HeaderText="Invoice No.">
                                <ItemTemplate>
                                    <div class="d-flex align-items-center gap-1.5">
                                        <span><%# Eval("InvoiceNumber") %></span>

                                        <!-- Direct redirect with SalesID -->
                                        <a href='<%# "SalesList.aspx?SalesID=" + Eval("SalesID") %>'
                                            class="btn btn-link p-2 text-decoration-none rotate-share-icon"
                                            title="Open Invoice Details">
                                            <i class="bx bx-link-external fs-6"></i>
                                        </a>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:BoundField
                                DataField="CustomerName"
                                HeaderText="Customer" />


                            <asp:BoundField
                                DataField="AmountPaid"
                                HeaderText="Amount"
                                DataFormatString="₹ {0:N2}" />


                            <asp:BoundField
                                DataField="PaymentMode"
                                HeaderText="Payment Mode" />


                            <asp:BoundField
                                DataField="ReferenceNumber"
                                HeaderText="Reference" />


                            <asp:BoundField
                                DataField="Remarks"
                                HeaderText="Remarks" />

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
