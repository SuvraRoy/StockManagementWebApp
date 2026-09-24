<%@ Page Title="Stock Transfer History" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="StockTransferHistory.aspx.cs"
    Inherits="Pages_StockTransferHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        /* Sneat / Bootstrap-styled GridView pagination */
        .gridview-pager table {
            margin: 1rem auto 0.5rem auto !important;
        }

        .gridview-pager td {
            padding: 0 4px !important;
            border: none !important;
        }

        .gridview-pager a, 
        .gridview-pager span {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            text-decoration: none;
            transition: all 0.2s ease-in-out;
        }

        .gridview-pager a {
            background-color: #f5f5f9;
            color: #697a8d;
            border: 1px solid #d9dee3;
        }

        .gridview-pager a:hover {
            background-color: #e1e4e8;
            color: #566a7f;
        }

        .gridview-pager span {
            background-color: #696cff;
            color: #ffffff !important;
            border: 1px solid #696cff;
            box-shadow: 0 2px 4px rgba(105, 108, 255, 0.4);
        }
    </style>

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- HEADER -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h4 class="fw-bold mb-1">
                    Stock Transfer History
                </h4>
                <p class="text-muted mb-0">
                    View stock movements between warehouses.
                </p>
            </div>

            <a href="StockTransfer.aspx" class="btn btn-primary">
                <i class="bx bx-transfer me-1"></i>
                New Transfer
            </a>
        </div>

        <!-- MESSAGE -->
        <div id="divMessage" runat="server" class="alert d-none">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>

        <!-- SEARCH -->
        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch" CssClass="card mb-4">
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-md-5">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Transfer number or warehouse"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">From Date</label>
                        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">To Date</label>
                        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="col-md-1">
                        <asp:Button ID="btnSearch" runat="server" Text="Go" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- HISTORY -->
        <div class="card">
            <div class="card-header">
                <h5 class="mb-0">Transfer Transactions</h5>
            </div>

            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView
                        ID="gvTransfers"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover align-middle mb-0"
                        GridLines="None"
                        DataKeyNames="TransferID"
                        OnRowCommand="gvTransfers_RowCommand"
                        AllowPaging="True"
                        PageSize="10"
                        OnPageIndexChanging="gvTransfers_PageIndexChanging"
                        EmptyDataText="No stock transfers found.">

                        <HeaderStyle CssClass="table-light" />

                        <Columns>
                            <asp:BoundField DataField="TransferNumber" HeaderText="Transfer No." />
                            <asp:BoundField DataField="TransferDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" />
                            <asp:BoundField DataField="FromWarehouse" HeaderText="From Warehouse" />
                            <asp:BoundField DataField="ToWarehouse" HeaderText="To Warehouse" />
                            <asp:BoundField DataField="ItemCount" HeaderText="Items" />

                            <asp:TemplateField HeaderText="Document">
                                <ItemTemplate>
                                    <asp:HyperLink
                                        ID="lnkDocument"
                                        runat="server"
                                        NavigateUrl='<%# Eval("DocumentFilePath") %>'
                                        Target="_blank"
                                        CssClass="btn btn-sm btn-outline-secondary"
                                        Visible='<%# !string.IsNullOrEmpty(Eval("DocumentFilePath") != null ? Eval("DocumentFilePath").ToString() : "") %>'>
                                        <i class="bx bx-file"></i> View
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="btnView"
                                        runat="server"
                                        CommandName="ViewTransfer"
                                        CommandArgument='<%# Eval("TransferID") %>'
                                        CssClass="btn btn-sm btn-outline-primary">
                                        <i class="bx bx-show"></i> View
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <PagerStyle CssClass="gridview-pager" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="« First" LastPageText="Last »" PageButtonCount="5" Position="Bottom" />

                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- DETAILS -->
        <asp:Panel ID="pnlDetails" runat="server" Visible="false" CssClass="card mt-4">
            <div class="card-header d-flex justify-content-between">
                <h5 class="mb-0">Transfer Details</h5>
                <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnClose_Click" />
            </div>

            <div class="card-body">
                <div class="row mb-4">
                    <div class="col-md-3">
                        <small class="text-muted">Transfer Number</small>
                        <h6><asp:Label ID="lblDetailTransferNumber" runat="server"></asp:Label></h6>
                    </div>

                    <div class="col-md-3">
                        <small class="text-muted">Date</small>
                        <h6><asp:Label ID="lblDetailDate" runat="server"></asp:Label></h6>
                    </div>

                    <div class="col-md-3">
                        <small class="text-muted">From</small>
                        <h6><asp:Label ID="lblDetailFrom" runat="server"></asp:Label></h6>
                    </div>

                    <div class="col-md-3">
                        <small class="text-muted">To</small>
                        <h6><asp:Label ID="lblDetailTo" runat="server"></asp:Label></h6>
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvTransferDetails" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                        <Columns>
                            <asp:BoundField DataField="ProductName" HeaderText="Product" />
                            <asp:BoundField DataField="SKU" HeaderText="SKU" />
                            <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="mt-3">
                    <strong>Remarks:</strong>
                    <asp:Label ID="lblDetailRemarks" runat="server"></asp:Label>
                </div>
            </div>
        </asp:Panel>

    </div>

</asp:Content>