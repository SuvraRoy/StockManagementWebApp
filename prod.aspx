<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="prod.aspx.cs" Inherits="prod" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-xxl flex-grow-1 container-p-y">
        <div class="row mb-6 gy-6">
            <div class="col-xl-8">
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
                                <asp:LinkButton ID="btnAddProduct" runat="server" CssClass="btn btn-primary btn-sm text-nowrap" data-bs-toggle="modal" data-bs-target="#backDropModal">Add Product</asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="card-body">
                        <div class="table-responsive text-nowrap">
                            <table class="table table-sm table-hover">
                                <thead>
                                    <tr>
                                        <th>Sl.No</th>
                                        <th>Product Name</th>
                                        <th>SKU</th>
                                        <th>Cost Price</th>
                                        <th>Current Stock</th>
                                        <th>Reorder Level</th>
                                    </tr>
                                </thead>
                                <tbody class="table-border-bottom-0">
                                    </tbody>
                            </table>
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
                                        <div class="col mb-0">
                                            <label class="form-label">SKU</label>
                                            <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col mb-0">
                                            <label class="form-label">Cost Price</label>
                                            <asp:TextBox ID="txtCost" runat="server" CssClass="form-control" TextMode="Number" />
                                        </div>
                                        <div class="col mb-0">
                                            <label class="form-label">Reorder Level</label>
                                            <asp:TextBox ID="txtReorder" runat="server" CssClass="form-control" TextMode="Number" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-label-secondary" data-bs-dismiss="modal">Close</button>
                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                                    <asp:Label ID="lblMessage" runat="server" Text="" cssClass="text-danger"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-xl-4">
                <div class="card">
                    <div class="card-header"><h5 class="mb-0">Product details</h5></div>
                    <div class="card-body">
                        <p><strong>Product Name:</strong> <asp:Label ID="lblDetailName" runat="server" /></p>
                        <p><strong>Product SKU:</strong> <asp:Label ID="lblDetailSKU" runat="server" /></p>
                        <p><strong>Cost Price:</strong> <asp:Label ID="lblDetailCost" runat="server" /></p>
                        <p><strong>Current Stock:</strong> <asp:Label ID="lblDetailStock" runat="server" /></p>
                        <p><strong>Reorder Level:</strong> <asp:Label ID="lblDetailReorder" runat="server" /></p>
                        
                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
