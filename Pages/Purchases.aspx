<%@ Page Title="Purchase Entry" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="Purchases.aspx.cs"
    Inherits="Pages_Purchases" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h4 class="fw-bold mb-1">Purchase Entry</h4>
                <p class="text-muted mb-0">
                    Add purchased products and increase warehouse stock.
                </p>
            </div>

            <a href="Purchases.aspx" class="btn btn-outline-secondary">
                <i class="bx bx-refresh me-1"></i>
                New Purchase
            </a>
        </div>

        <!-- Message -->
        <div id="divMessage" runat="server"
            visible="false"
            class="alert alert-danger alert-dismissible fade show"
            role="alert">

            <asp:Label ID="lblMessage" runat="server"></asp:Label>

            <button type="button"
                class="btn-close"
                data-bs-dismiss="alert">
            </button>

        </div>

        <!-- Purchase Information -->
        <div class="card mb-4">

            <div class="card-header">
                <h5 class="mb-0">Purchase Information</h5>
            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Supplier -->
                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Supplier <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlSupplier"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <!-- Warehouse -->
                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Warehouse <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlWarehouse"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <!-- Invoice Number -->
                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Supplier Invoice Number
                            <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtInvoiceNumber"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Enter invoice number">
                        </asp:TextBox>

                    </div>

                    <!-- Purchase Date -->
                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Purchase Date <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtPurchaseDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <!-- Remarks -->
                    <div class="col-md-8 mb-3">

                        <label class="form-label">
                            Remarks
                        </label>

                        <asp:TextBox
                            ID="txtRemarks"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Optional remarks">
                        </asp:TextBox>

                    </div>

                </div>

            </div>

        </div>

        <!-- Purchase Items -->
        <div class="card">

            <div class="card-header d-flex justify-content-between align-items-center">

                <h5 class="mb-0">Purchase Items</h5>

                <asp:Button
                    ID="btnAddRow"
                    runat="server"
                    Text="+ Add Item"
                    CssClass="btn btn-sm btn-primary"
                    OnClick="btnAddRow_Click" />

            </div>

            <div class="card-body">

                <div class="table-responsive">

                    <table class="table table-bordered align-middle">

                        <thead class="table-light">

                            <tr>

                                <th style="width:25%">
                                    Product
                                </th>

                                <th style="width:15%">
                                    SKU
                                </th>

                                <th style="width:15%">
                                    Quantity
                                </th>

                                <th style="width:15%">
                                    Purchase Rate
                                </th>

                                <th style="width:15%">
                                    Amount
                                </th>

                                <th style="width:5%">
                                </th>

                            </tr>

                        </thead>

                        <tbody id="itemRowsContainer">

                            <asp:Repeater
                                ID="rptItems"
                                runat="server"
                                OnItemCommand="rptItems_ItemCommand">

                                <ItemTemplate>

                                    <tr>

                                        <!-- Product -->
                                        <td>

                                            <asp:DropDownList
                                                ID="ddlProduct"
                                                runat="server"
                                                CssClass="form-select form-select-sm"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged">
                                            </asp:DropDownList>

                                        </td>

                                        <!-- SKU -->
                                        <td>

                                            <asp:Label
                                                ID="lblSKU"
                                                runat="server"
                                                CssClass="fw-semibold text-muted">
                                            </asp:Label>

                                        </td>

                                        <!-- Quantity -->
                                        <td>

                                            <asp:TextBox
                                                ID="txtQuantity"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                Text="1"
                                                TextMode="Number"
                                                min="1">
                                            </asp:TextBox>

                                        </td>

                                        <!-- Rate -->
                                        <td>

                                            <asp:TextBox
                                                ID="txtRate"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                TextMode="Number"
                                                step="0.01">
                                            </asp:TextBox>

                                        </td>

                                        <!-- Amount -->
                                        <td>

                                            <asp:Label
                                                ID="lblAmount"
                                                runat="server"
                                                CssClass="fw-bold">
                                                0.00
                                            </asp:Label>

                                        </td>

                                        <!-- Delete -->
                                        <td class="text-center">

                                            <asp:LinkButton
                                                ID="btnDelete"
                                                runat="server"
                                                CommandName="DeleteRow"
                                                CommandArgument='<%# Container.ItemIndex %>'
                                                CssClass="btn btn-sm btn-outline-danger">

                                                <i class="bx bx-trash"></i>

                                            </asp:LinkButton>

                                        </td>

                                    </tr>

                                </ItemTemplate>

                            </asp:Repeater>

                        </tbody>

                    </table>

                </div>

                <!-- Total -->
                <div class="row justify-content-end mt-4">

                    <div class="col-md-4">

                        <div class="d-flex justify-content-between border-top pt-3">

                            <strong>Grand Total</strong>

                            <strong class="fs-5">
                                ₹
                                <asp:Label
                                    ID="lblGrandTotal"
                                    runat="server">
                                    0.00
                                </asp:Label>
                            </strong>

                        </div>

                    </div>

                </div>

                <!-- Buttons -->
                <div class="text-end mt-4">

                    <asp:Button
                        ID="btnCancel"
                        runat="server"
                        Text="Cancel"
                        CssClass="btn btn-outline-secondary me-2"
                        OnClick="btnCancel_Click" />

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save Purchase"
                        CssClass="btn btn-primary"
                        OnClick="btnSave_Click" />

                </div>

            </div>

        </div>

    </div>

<script type="text/javascript">

    function calculatePurchaseRow(row) {
        var quantityBox = row.querySelector("input[id*='txtQuantity']");
        var rateBox = row.querySelector("input[id*='txtRate']");
        var amountLabel = row.querySelector("[id*='lblAmount']");

        if (!quantityBox || !rateBox || !amountLabel) {
            return 0;
        }

        var quantity = parseFloat(quantityBox.value) || 0;
        var rate = parseFloat(rateBox.value) || 0;
        var amount = quantity * rate;

        amountLabel.innerText = amount.toFixed(2);
        return amount;
    }

    function calculateGrandTotal() {
        var rows = document.querySelectorAll("#itemRowsContainer tr");
        var grandTotal = 0;

        for (var i = 0; i < rows.length; i++) {
            grandTotal += calculatePurchaseRow(rows[i]);
        }

        var totalLabel = document.getElementById("<%= lblGrandTotal.ClientID %>");
        if (totalLabel) {
            totalLabel.innerText = grandTotal.toFixed(2);
        }
    }

    function attachPurchaseCalculation() {
        var rows = document.querySelectorAll("#itemRowsContainer tr");

        for (var i = 0; i < rows.length; i++) {
            var quantityBox = rows[i].querySelector("input[id*='txtQuantity']");
            var rateBox = rows[i].querySelector("input[id*='txtRate']");

            if (quantityBox && !quantityBox.dataset.bound) {
                quantityBox.dataset.bound = "true";
                quantityBox.addEventListener("input", calculateGrandTotal);
            }

            if (rateBox && !rateBox.dataset.bound) {
                rateBox.dataset.bound = "true";
                rateBox.addEventListener("input", calculateGrandTotal);
            }
        }

        calculateGrandTotal();
    }

    document.addEventListener("DOMContentLoaded", function () {
        attachPurchaseCalculation();
    });

    // Re-bind script after ASP.NET postbacks if needed
    if (typeof Sys !== 'undefined' && Sys.WebForms) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            attachPurchaseCalculation();
        });
    }

</script>

</asp:Content>