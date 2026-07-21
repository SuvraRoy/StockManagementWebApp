<%@ Page Title="" Language="C#" MasterPageFile="../landing.master" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="Pages_Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-xxl flex-grow-1 container-p-y">
        <div class="row mb-6 gy-6">
            <div class="col-xl-9">
                <div class="card">
                    <div class="row mx-2">
                        <div class="row align-items-center justify-content-between g-3">
                            <div class="col-auto">
                                <h5 class="mb-0">Product List</h5>
                            </div>
                            <div class="col-auto d-flex align-items-center gap-2">
                                <div class="input-group">
                                    <span class="input-group-text"><i class="icon-base bx bx-search"></i></span>
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                                </div>
                                <asp:LinkButton ID="btnAddProduct" runat="server" CssClass="btn btn-primary btn-sm text-nowrap" OnCLick="btnAddProduct_Click">Add Product</asp:LinkButton>
                            </div>
                        </div>
                    </div>



                    <div class="card-body">
                        <div class="table-responsive text-nowrap">

                            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" CssClass="table table-sm table-hover" DataKeyNames="ProductID" OnRowCommand="gvProducts_RowCommand">

                                <Columns>

                                    <asp:BoundField DataField="ProductID" HeaderText="ID" />

                                    <asp:BoundField DataField="ProductName" HeaderText="Product Name" />

                                    <asp:BoundField DataField="SKU" HeaderText="SKU" />

                                    <asp:BoundField DataField="CostPrice" HeaderText="Cost Price" />

                                    <asp:BoundField DataField="CurrentStock" HeaderText="Stock" />

                                    <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" />

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>

                                            <asp:Button ID="btnView"
                                                runat="server"
                                                Text="View"
                                                CssClass="btn btn-sm btn-primary"
                                                CommandName="ViewProduct"
                                                OnClick="btnView_Click"
                                                CommandArgument='<%# Eval("ProductID") %>' />

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>
                    </div>


                    <div class="modal fade" id="backDropModal" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title">Add Product</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="row">
                                        <div class="col mb-6">
                                            <label class="form-label">Product Name</label>
                                            <asp:TextBox ID="txtProdName" runat="server" CssClass="form-control" placeholder="Enter Name" />
                                        </div>
                                    </div>
                                    <div class="row g-6">
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">SKU</label>
                                            <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Cost Price</label>
                                            <asp:TextBox ID="txtCostPrice" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-4 mb-3">
                                            <label class="form-label">Reorder Level</label>
                                            <asp:TextBox ID="txtReorderLevel" runat="server" CssClass="form-control" TextMode="Number" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-label-secondary" data-bs-dismiss="modal">Close</button>
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-xl-3">

                <asp:Panel ID="pnlProductDetails" runat="server" Visible="false">
                                    <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Product details</h5>
                    </div>
                    <div class="card-body">
                        <p>
                            <strong>Product Name:</strong>
                            <asp:Label ID="lblDetailName" runat="server" />
                        </p>
                        <p>
                            <strong>Product SKU:</strong>
                            <asp:Label ID="lblDetailSKU" runat="server" />
                        </p>
                        <p>
                            <strong>Cost Price:</strong>
                            <asp:Label ID="lblDetailCost" runat="server" />
                        </p>
                        <p>
                            <strong>Current Stock:</strong>
                            <asp:Label ID="lblDetailStock" runat="server" />
                        </p>
                        <p>
                            <strong>Reorder Level:</strong>
                            <asp:Label ID="lblDetailReorder" runat="server" />
                        </p>

                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this record? ')" />
                    </div>
                </div>

                </asp:Panel>

            </div>
        </div>
    </div>

    <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;">
        <div id="liveToast"
            class="toast shadow-lg"
            role="alert"
            aria-live="assertive"
            aria-atomic="true"
            runat="server"
            clientidmode="Static">

            <div class="toast-header">
                <strong class="me-auto">Notification</strong>
                <button type="button"
                    class="btn-close"
                    data-bs-dismiss="toast">
                </button>
            </div>

            <div class="toast-body">
                <asp:Label ID="lblToast" runat="server"></asp:Label>
            </div>

        </div>

    </div>


</asp:Content>
