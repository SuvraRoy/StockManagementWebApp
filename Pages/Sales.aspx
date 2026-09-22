<%@ Page Title="Sales & Billing" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="Sales.aspx.cs" Inherits="Pages_Sales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h4 class="fw-bold mb-1">Sales & Billing (Stock-Out)</h4>
                <p class="text-muted mb-0">
                    Create sales invoices and deduct physical stock.
                </p>
            </div>

            <a href="SalesList.aspx" class="btn btn-outline-primary">
                <i class="bx bx-list-ul me-1"></i>
                Sales List
            </a>
        </div>


        <!-- Message -->
        <div id="divMessage" runat="server" class="alert d-none">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>


        <!-- SALE INFORMATION -->
        <div class="card mb-4">

            <div class="card-header">
                <h5 class="mb-0">Sale Information</h5>
            </div>

            <div class="card-body">

                <div class="row g-3">

                    <!-- Invoice -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Invoice Number
                        </label>

                        <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" ReadOnly="true"> </asp:TextBox>

                    </div>


                    <!-- Customer -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Customer <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlCustomer"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>


                    <!-- Warehouse -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Warehouse <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList ID="ddlWarehouse" runat="server" CssClass="form-select"  AutoPostBack="true" OnSelectedIndexChanged="ddlWarehouse_SelectedIndexChanged">  </asp:DropDownList>
                        <small class="text-muted"> Select warehouse before selecting products.</small>

                    </div>


                    <!-- Date -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Sale Date <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtSalesDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>

            </div>

        </div>


        <!-- SALE ITEMS -->
        <div class="card mb-4">

            <div class="card-header d-flex justify-content-between align-items-center">

                <div>
                    <h5 class="mb-1">Sale Items</h5>

                    <small class="text-muted">
                        Products are loaded according to warehouse stock.
                    </small>
                </div>

                <asp:Button
                    ID="btnAddRow"
                    runat="server"
                    Text="+ Add Item"
                    CssClass="btn btn-primary btn-sm"
                    CausesValidation="false"
                    OnClick="btnAddRow_Click" />

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <table class="table table-bordered align-middle">

                        <thead class="table-light">

                            <tr>

                                <th style="min-width:260px;">
                                    Product
                                </th>

                                <th style="width:150px;">
                                    Available Stock
                                </th>

                                <th style="width:140px;">
                                    Quantity
                                </th>

                                <th style="width:160px;">
                                    Sale Rate
                                </th>

                                <th style="width:160px;">
                                    Amount
                                </th>

                                <th style="width:70px;">
                                    Action
                                </th>

                            </tr>

                        </thead>


                        <tbody>

                            <asp:Repeater
                                ID="rptItems"
                                runat="server"
                                OnItemCommand="rptItems_ItemCommand">

                                <ItemTemplate>

                                    <tr>

                                        <!-- PRODUCT -->

                                        <td>

                                            <asp:DropDownList
                                                ID="ddlProduct"
                                                runat="server"
                                                CssClass="form-select product-dropdown"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged">
                                            </asp:DropDownList>

                                        </td>


                                        <!-- AVAILABLE STOCK -->

                                        <td>

                                            <asp:Label
                                                ID="lblAvailableStock"
                                                runat="server"
                                                CssClass="fw-semibold text-primary available-stock">
                                                -
                                            </asp:Label>

                                        </td>


                                        <!-- QUANTITY -->

                                        <td>

                                            <asp:TextBox
                                                ID="txtQuantity"
                                                runat="server"
                                                CssClass="form-control sale-quantity"
                                                TextMode="Number"
                                                min="1"
                                                Text="1">
                                            </asp:TextBox>

                                        </td>


                                        <!-- SALE RATE -->

                                        <td>

                                            <asp:TextBox
                                                ID="txtRate"
                                                runat="server"
                                                CssClass="form-control sale-rate"
                                                TextMode="Number"
                                                step="0.01"
                                                min="0">
                                            </asp:TextBox>

                                        </td>


                                        <!-- AMOUNT -->

                                        <td>

                                            <span class="fw-semibold sale-amount">
                                                0.00
                                            </span>

                                        </td>


                                        <!-- DELETE -->

                                        <td class="text-center">

                                            <asp:LinkButton
                                                ID="btnDelete"
                                                runat="server"
                                                CommandName="DeleteRow"
                                                CommandArgument='<%# Container.ItemIndex %>'
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CausesValidation="false">

                                                <i class="bx bx-trash"></i>

                                            </asp:LinkButton>

                                        </td>

                                    </tr>

                                </ItemTemplate>

                            </asp:Repeater>

                        </tbody>

                    </table>

                </div>


                <!-- GRAND TOTAL -->

                <div class="row justify-content-end mt-4">

                    <div class="col-md-4">

                        <div class="d-flex justify-content-between border-top pt-3">

                            <h5 class="mb-0">
                                Grand Total
                            </h5>

                            <h4 class="fw-bold mb-0">
                                ₹
                                <span id="grandTotalDisplay">0.00</span>
                            </h4>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- BUTTONS -->

        <div class="d-flex justify-content-end gap-2">

            <asp:Button
                ID="btnCancel"
                runat="server"
                Text="Cancel"
                CssClass="btn btn-outline-secondary"
                CausesValidation="false"
                OnClick="btnCancel_Click" />

            <asp:Button
                ID="btnSave"
                runat="server"
                Text="Complete Sale"
                CssClass="btn btn-success"
                OnClick="btnSave_Click" />

        </div>

    </div>


    <!-- REAL TIME CALCULATION -->

 <script type="text/javascript">
     function calculateSalesTotal() {
         var grandTotal = 0;

         // Query all table rows inside the Repeater's table body
         var rows = document.querySelectorAll("table.table tbody tr");

         rows.forEach(function (row) {
             // Match input fields using wildcard attributes generated by ASP.NET ClientIDs
             var quantityBox = row.querySelector("input[id*='txtQuantity']");
             var rateBox = row.querySelector("input[id*='txtRate']");
             var amountDisplay = row.querySelector(".sale-amount");

             if (!quantityBox || !rateBox || !amountDisplay) return;

             var quantity = parseFloat(quantityBox.value) || 0;
             var rate = parseFloat(rateBox.value) || 0;
             var amount = quantity * rate;

             amountDisplay.textContent = amount.toFixed(2);
             grandTotal += amount;
         });

         var grandTotalDisplay = document.getElementById("grandTotalDisplay");
         if (grandTotalDisplay) {
             grandTotalDisplay.textContent = grandTotal.toFixed(2);
         }
     }

     // Attach delegated input event to capture real-time typing across all dynamically generated rows
     document.addEventListener("input", function (event) {
         if (event.target && (event.target.id.indexOf("txtQuantity") !== -1 || event.target.id.indexOf("txtRate") !== -1)) {
             calculateSalesTotal();
         }
     });

     // Run on initial DOM render
     document.addEventListener("DOMContentLoaded", calculateSalesTotal);

     // Re-run execution after ASP.NET Partial PostBacks (if AJAX UpdatePanels are active)
     if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
         Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
             calculateSalesTotal();
         });
     }
 </script>

</asp:Content>