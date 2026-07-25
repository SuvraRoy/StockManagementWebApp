<%@ Page Title="Products" Language="C#" MasterPageFile="../landing.master" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="Pages_Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


<div class="container-xxl flex-grow-1 container-p-y">

    <div class="row mb-6 gy-6">

        <!-- PRODUCT LIST -->
        <div class="col-xl-9">

            <div class="card">

                <!-- HEADER -->
                <div class="card-header">

                    <div class="row align-items-center g-3">

                        <div class="col-md-4">
                            <h5 class="mb-0">Product List</h5>
                        </div>

                        <div class="col-md-8">

                            <div class="d-flex justify-content-md-end gap-2">

                                <!-- SEARCH -->
                                <div class="input-group">

                                    <span class="input-group-text">
                                        <i class="icon-base bx bx-search"></i>
                                    </span>

                                    <asp:TextBox
                                        ID="txtSearch"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="Search product or SKU..."
                                        AutoPostBack="true"
                                        OnTextChanged="txtSearch_TextChanged">
                                    </asp:TextBox>

                                </div>

                                <!-- ADD PRODUCT -->
                                <asp:LinkButton
                                    ID="btnAddProduct"
                                    runat="server"
                                    CssClass="btn btn-primary text-nowrap"
                                    OnClick="btnAddProduct_Click">

                                    <i class="bx bx-plus me-1"></i>
                                    Add Product

                                </asp:LinkButton>

                            </div>

                        </div>

                    </div>

                </div>


                <!-- PRODUCT TABLE -->
                <div class="card-body">

                    <div class="table-responsive">

                        <asp:GridView
                            ID="gvProducts"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="table table-sm table-hover align-middle"
                            DataKeyNames="ProductID"
                            EmptyDataText="No products found."
                            OnRowCommand="gvProducts_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="ProductID"
                                    HeaderText="ID" />

                                <asp:BoundField
                                    DataField="ProductName"
                                    HeaderText="Product Name" />

                                <asp:BoundField
                                    DataField="SKU"
                                    HeaderText="SKU" />

                                <asp:BoundField
                                    DataField="CostPrice"
                                    HeaderText="Cost Price"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField
                                    DataField="CurrentStock"
                                    HeaderText="Stock" />

                                <asp:BoundField
                                    DataField="ReorderLevel"
                                    HeaderText="Reorder Level" />

                                
                                <asp:TemplateField HeaderText="Status">

                                    <ItemTemplate>

                                        <asp:Label
                                            ID="lblStatus"
                                            runat="server"
                                            CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-label-success" : "badge bg-label-secondary" %>'
                                            Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>'>
                                        </asp:Label>

                                    </ItemTemplate>

                                </asp:TemplateField>


                                
                                <asp:TemplateField HeaderText="Action">

                                    <ItemTemplate>

                                        <asp:Button
                                            ID="btnView"
                                            runat="server"
                                            Text="View"
                                            CssClass="btn btn-sm btn-primary"
                                            CommandName="ViewProduct"
                                            CommandArgument='<%# Eval("ProductID") %>' />

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>


                <!-- ADD PRODUCT MODAL -->
                <div
                    class="modal fade"
                    id="backDropModal"
                    data-bs-backdrop="static"
                    tabindex="-1"
                    aria-hidden="true">

                    <div class="modal-dialog">

                        <div class="modal-content">

                            <div class="modal-header">

                                <h5 class="modal-title">
                                    Add Product
                                </h5>

                                <button
                                    type="button"
                                    class="btn-close"
                                    data-bs-dismiss="modal"
                                    aria-label="Close">
                                </button>

                            </div>


                            <div class="modal-body">

                                <!-- PRODUCT NAME -->
                                <div class="row">

                                    <div class="col mb-4">

                                        <label class="form-label">
                                            Product Name
                                        </label>

                                        <asp:TextBox
                                            ID="txtProdName"
                                            runat="server"
                                            CssClass="form-control"
                                            placeholder="Enter product name">
                                        </asp:TextBox>

                                    </div>

                                </div>


                                <!-- PRODUCT FIELDS -->
                                <div class="row g-4">

                                    <!-- SKU -->
                                    <div class="col-md-4">

                                        <label class="form-label">
                                            SKU
                                        </label>

                                        <asp:TextBox
                                            ID="txtSKU"
                                            runat="server"
                                            CssClass="form-control"
                                            placeholder="Enter SKU">
                                        </asp:TextBox>

                                    </div>


                                    <!-- COST PRICE -->
                                    <div class="col-md-4">

                                        <label class="form-label">
                                            Cost Price
                                        </label>

                                        <asp:TextBox
                                            ID="txtCostPrice"
                                            runat="server"
                                            CssClass="form-control"
                                            placeholder="0.00">
                                        </asp:TextBox>

                                    </div>


                                    <!-- REORDER LEVEL -->
                                    <div class="col-md-4">

                                        <label class="form-label">
                                            Reorder Level
                                        </label>

                                        <asp:TextBox
                                            ID="txtReorderLevel"
                                            runat="server"
                                            CssClass="form-control"
                                            TextMode="Number"
                                            placeholder="0">
                                        </asp:TextBox>

                                    </div>

                                </div>

                            </div>


                            <div class="modal-footer">

                                <button
                                    type="button"
                                    class="btn btn-label-secondary"
                                    data-bs-dismiss="modal">

                                    Close

                                </button>

                                <asp:Button
                                    ID="btnSave"
                                    runat="server"
                                    Text="Save"
                                    CssClass="btn btn-primary"
                                    OnClick="btnSave_Click" />

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- PRODUCT DETAILS -->
        <div class="col-xl-3">

            <asp:Panel
                ID="pnlProductDetails"
                runat="server"
                Visible="false">

                <div class="card">

                    <div class="card-header">

                        <h5 class="mb-0">
                            Product Details
                        </h5>

                    </div>


                    <div class="card-body">

                        <!-- PRODUCT NAME -->
                        <p>
                            <strong>Product Name:</strong><br />

                            <asp:Label
                                ID="lblDetailName"
                                runat="server">
                            </asp:Label>

                        </p>


                        <!-- SKU -->
                        <p>
                            <strong>Product SKU:</strong><br />

                            <asp:Label
                                ID="lblDetailSKU"
                                runat="server">
                            </asp:Label>

                        </p>


                        <!-- COST -->
                        <p>
                            <strong>Cost Price:</strong><br />

                            <asp:Label
                                ID="lblDetailCost"
                                runat="server">
                            </asp:Label>

                        </p>


                        <!-- STOCK -->
                        <p>
                            <strong>Current Stock:</strong><br />

                            <asp:Label
                                ID="lblDetailStock"
                                runat="server">
                            </asp:Label>

                        </p>


                        <!-- REORDER -->
                        <p>
                            <strong>Reorder Level:</strong><br />

                            <asp:Label
                                ID="lblDetailReorder"
                                runat="server">
                            </asp:Label>

                        </p>


                        
                        <p>
                            <strong>Status:</strong><br />

                            <asp:Label
                                ID="lblDetailStatus"
                                runat="server">
                            </asp:Label>

                        </p>


                        <!-- ACTION BUTTONS -->
                        <div class="d-flex gap-2">

                            <asp:Button
                                ID="btnEdit"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-primary"
                                OnClick="btnEdit_Click" />

                            <asp:Button
                                ID="btnToggleStatus"
                                runat="server"
                                Text="Deactivate"
                                CssClass="btn btn-warning"
                                OnClick="btnToggleStatus_Click"
                                OnClientClick="return confirm('Are you sure you want to change this product status?');" />

                        </div>

                    </div>

                </div>

            </asp:Panel>

        </div>

    </div>

</div>


<!-- TOAST NOTIFICATION -->
<div
    class="toast-container position-fixed top-0 end-0 p-3"
    style="z-index: 9999;">

    <div
        id="liveToast"
        class="toast shadow-lg"
        role="alert"
        aria-live="assertive"
        aria-atomic="true"
        runat="server"
        clientidmode="Static">

        <div class="toast-header">

            <strong class="me-auto">
                Notification
            </strong>

            <button
                type="button"
                class="btn-close"
                data-bs-dismiss="toast">
            </button>

        </div>

        <div class="toast-body">

            <asp:Label
                ID="lblToast"
                runat="server">
            </asp:Label>

        </div>

    </div>

</div>


</asp:Content>
