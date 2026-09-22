<%@ Page Title="Create Purchase"
    Language="C#"
    MasterPageFile="~/landing.master"
    AutoEventWireup="true"
    CodeFile="test.aspx.cs"
    Inherits="Pages_test" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="Server">
</asp:Content>


<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <div class="card">

            <!-- HEADER -->
            <div class="card-header d-flex justify-content-between align-items-center">

                <div>
                    <h4 class="mb-1">New Purchase</h4>

                    <small class="text-muted">
                        Record products received from a supplier
                    </small>
                </div>

            </div>


            <div class="card-body">

                <!-- MESSAGE -->
                <div id="divMessage"
                    runat="server"
                    visible="false"
                    class="alert alert-danger mb-4">

                    <asp:Label
                        ID="lblMessage"
                        runat="server">
                    </asp:Label>

                </div>


                <!-- PURCHASE DETAILS -->

                <div class="row g-3 mb-4">

                    <!-- SUPPLIER -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Supplier
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlSupplier"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Text="Select Supplier"
                                Value="">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- WAREHOUSE -->
                    <div class="col-md-4">

                        <label class="form-label">
                            Delivery Warehouse
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList
                            ID="ddlWarehouse"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Text="Select Warehouse"
                                Value="">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- PO NUMBER -->
                    <div class="col-md-2">

                        <label class="form-label">
                            Purchase No.
                        </label>

                        <asp:TextBox
                            ID="txtPurchaseNumber"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- DATE -->
                    <div class="col-md-2">

                        <label class="form-label">
                            Purchase Date
                            <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtPurchaseDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>


                <hr />


                <!-- ITEMS -->

                <div class="d-flex justify-content-between align-items-center mb-3">

                    <h5 class="mb-0">
                        Purchase Items
                    </h5>

                </div>


                <div class="table-responsive">

                    <table class="table table-bordered align-middle">

                        <thead class="table-light">

                            <tr>

                                <th style="width: 35%">
                                    Product
                                </th>

                                <th style="width: 15%">
                                    SKU
                                </th>

                                <th style="width: 12%">
                                    Quantity
                                </th>

                                <th style="width: 15%">
                                    Rate
                                </th>

                                <th style="width: 18%"
                                    class="text-end">

                                    Amount

                                </th>

                                <th style="width: 5%">
                                </th>

                            </tr>

                        </thead>


                        <tbody>

                            <asp:Repeater
                                ID="rptItems"
                                runat="server"
                                OnItemDataBound="rptItems_ItemDataBound">

                                <ItemTemplate>

                                    <tr>

                                        <!-- PRODUCT -->
                                        <td>

                                            <asp:HiddenField
                                                ID="hfRowIndex"
                                                runat="server"
                                                Value='<%# Eval("RowIndex") %>' />


                                            <asp:DropDownList
                                                ID="ddlProduct"
                                                runat="server"
                                                CssClass="form-select"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged">
                                            </asp:DropDownList>

                                        </td>


                                        <!-- SKU -->
                                        <td>

                                            <asp:Label
                                                ID="lblSKU"
                                                runat="server"
                                                Text='<%# Eval("SKU") %>'>
                                            </asp:Label>

                                        </td>


                                        <!-- QUANTITY -->
                                        <td>

                                            <asp:TextBox
                                                ID="txtQuantity"
                                                runat="server"
                                                TextMode="Number"
                                                CssClass="form-control"
                                                Text='<%# Eval("Quantity") %>'
                                                AutoPostBack="true"
                                                OnTextChanged="ItemValueChanged">
                                            </asp:TextBox>

                                        </td>


                                        <!-- RATE -->
                                        <td>

                                            <asp:TextBox
                                                ID="txtRate"
                                                runat="server"
                                                TextMode="Number"
                                                CssClass="form-control"
                                                Text='<%# Eval("Rate") %>'
                                                AutoPostBack="true"
                                                OnTextChanged="ItemValueChanged">
                                            </asp:TextBox>

                                        </td>


                                        <!-- AMOUNT -->
                                        <td class="text-end">

                                            ₹

                                            <asp:Label
                                                ID="lblAmount"
                                                runat="server"
                                                Text='<%# Eval("Amount") %>'
                                                CssClass="fw-semibold">
                                            </asp:Label>

                                        </td>


                                        <!-- DELETE -->
                                        <td class="text-center">

                                            <asp:LinkButton
                                                ID="btnDelete"
                                                runat="server"
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CommandArgument='<%# Eval("RowIndex") %>'
                                                OnClick="btnDelete_Click">

                                                <i class="bx bx-trash"></i>

                                            </asp:LinkButton>

                                        </td>

                                    </tr>

                                </ItemTemplate>

                            </asp:Repeater>

                        </tbody>

                    </table>

                </div>


                <!-- ADD ROW -->

                <div class="mb-4">

                    <asp:Button
                        ID="btnAddRow"
                        runat="server"
                        Text="+ Add Item"
                        CssClass="btn btn-outline-primary"
                        OnClick="btnAddRow_Click" />

                </div>


                <!-- TOTAL -->

                <div class="row justify-content-end">

                    <div class="col-md-4">

                        <div class="bg-light rounded p-3">

                            <div class="d-flex justify-content-between align-items-center">

                                <span class="fw-semibold">
                                    Grand Total
                                </span>

                                <h5 class="mb-0 text-primary">

                                    ₹

                                    <asp:Label
                                        ID="lblGrandTotal"
                                        runat="server"
                                        Text="0.00">
                                    </asp:Label>

                                </h5>

                            </div>

                        </div>

                    </div>

                </div>

            </div>


            <!-- FOOTER -->

            <div class="card-footer">

                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save Purchase"
                    CssClass="btn btn-primary"
                    OnClick="btnSave_Click" />


                <asp:Button
                    ID="btnCancel"
                    runat="server"
                    Text="Cancel"
                    CssClass="btn btn-outline-secondary ms-2"
                    OnClick="btnCancel_Click" />

            </div>

        </div>

    </div>

</asp:Content>