
<%@ Page Title="Purchase Details" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="PurchaseDetails.aspx.cs"
    Inherits="Pages_PurchaseDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .details-card {
            border: 0;
            box-shadow: 0 2px 10px rgba(0,0,0,.05);
        }

        .details-table th {
            background: #f7f7f8;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-xxl flex-grow-1 container-p-y py-4">

        <!-- Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">

            <div>
                <h4 class="mb-1">Purchase Details</h4>

                <%--<asp:Label
                    ID="lblPurchaseNumber"
                    runat="server"
                    CssClass="text-muted">
                </asp:Label>--%>

            </div>

            <asp:Button
                ID="btnBack"
                runat="server"
                Text="← Purchase History"
                CssClass="btn btn-outline-secondary"
                OnClick="btnBack_Click" />

        </div>


        <!-- Purchase Information -->
        <div class="card details-card mb-4">

            <div class="card-header">
                <h5 class="mb-0">Purchase Information</h5>
            </div>

            <div class="card-body">

                <div class="row g-4">

                    <div class="col-md-3">

                        <small class="text-muted">
                            Supplier
                        </small>

                        <div class="fw-semibold">
                            <asp:Label
                                ID="lblSupplier"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <small class="text-muted">
                            Purchase Date
                        </small>

                        <div class="fw-semibold">
                            <asp:Label
                                ID="lblPurchaseDate"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <small class="text-muted">
                            Invoice Number
                        </small>

                        <div class="fw-semibold">
                            <asp:Label
                                ID="lblInvoiceNumber"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <small class="text-muted">
                            Total Amount
                        </small>

                        <div class="fw-semibold fs-5">
                            <asp:Label
                                ID="lblTotalAmount"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- Items -->
        <div class="card details-card">

            <div class="card-header">
                <h5 class="mb-0">Purchased Products</h5>
            </div>

            <div class="card-body p-0">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvPurchaseDetails"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table details-table table-hover mb-0"
                        GridLines="None">

                        <Columns>

                            <asp:BoundField
                                DataField="ProductName"
                                HeaderText="Product" />

                            <asp:BoundField
                                DataField="SKU"
                                HeaderText="SKU" />

                            <asp:BoundField
                                DataField="Quantity"
                                HeaderText="Quantity" />

                           <%-- <asp:BoundField
                                DataField="PurchaseRate"
                                HeaderText="Purchase Rate"
                                DataFormatString="₹ {0:N2}" />--%>

                            <%--<asp:BoundField
                                DataField="Amount"
                                HeaderText="Amount"
                                DataFormatString="₹ {0:N2}" />--%>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>