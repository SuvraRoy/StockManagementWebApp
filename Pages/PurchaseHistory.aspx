
<%@ Page Title="Purchase History" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="PurchaseHistory.aspx.cs"
    Inherits="Pages_PurchaseHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .history-card {
            border: 0;
            box-shadow: 0 2px 10px rgba(0,0,0,.05);
        }

        .history-table th {
            background: #f7f7f8;
            font-size: 13px;
        }

        .history-table td {
            vertical-align: middle;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-xxl flex-grow-1 container-p-y py-4">

        <!-- Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">

            <div>
                <h4 class="mb-1">Purchase History</h4>
                <p class="text-muted mb-0">
                    View all recorded purchases
                </p>
            </div>

            <asp:Button
                ID="btnNewPurchase"
                runat="server"
                Text="+ New Purchase"
                CssClass="btn btn-primary"
                OnClick="btnNewPurchase_Click" />

        </div>


        <!-- Search -->
        <div class="card history-card mb-4">

            <div class="card-body">

                <asp:Panel ID="searchPanel" runat="server" DefaultButton="btnSearch">

                    <div class="row g-2">

                        <div class="col-md-6">

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Search purchase no, invoice or supplier">
                            </asp:TextBox>

                        </div>

                        <div class="col-md-2">

                            <asp:Button
                                ID="btnSearch"
                                runat="server"
                                Text="Search"
                                CssClass="btn btn-outline-primary w-100"
                                OnClick="btnSearch_Click" />

                        </div>

                        <div class="col-md-2">

                            <asp:Button
                                ID="btnClear"
                                runat="server"
                                Text="Clear"
                                CssClass="btn btn-outline-secondary w-100"
                                OnClick="btnClear_Click" />

                        </div>

                    </div>

                </asp:Panel>

            </div>

        </div>

        <!-- History Table -->
        <div class="card history-card">

            <div class="card-body p-0">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvPurchaseHistory"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table history-table table-hover mb-0"
                        GridLines="None"
                        EmptyDataText="No purchases found."
                        OnRowCommand="gvPurchaseHistory_RowCommand">

                        <Columns>

                            <%--<asp:BoundField
                                DataField="PurchaseNumber"
                                HeaderText="Purchase #" />--%>

                            <asp:BoundField
                                DataField="PurchaseDate"
                                HeaderText="Date"
                                DataFormatString="{0:dd-MMM-yyyy}" />

                            <asp:BoundField
                                DataField="CompanyName"
                                HeaderText="Supplier" />

                            <asp:BoundField
                                DataField="InvoiceNumber"
                                HeaderText="Invoice No." />

                            <asp:BoundField
                                DataField="ItemCount"
                                HeaderText="Items" />

                            <asp:BoundField
                                DataField="TotalAmount"
                                HeaderText="Total Amount"
                                DataFormatString="₹ {0:N2}" />

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnView"
                                        runat="server"
                                        CommandName="ViewPurchase"
                                        CommandArgument='<%# Eval("PurchaseID") %>'
                                        CssClass="btn btn-sm btn-outline-primary">

                                        <i class="bx bx-show"></i>
                                        View

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>

