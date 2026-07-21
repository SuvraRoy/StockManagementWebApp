<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="EditProduct.aspx.cs" Inherits="Pages_EditProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="container-xxl flex-grow-1 container-p-y">

    <div class="row justify-content-center">

        <div class="col-md-12">

            <div class="card">

                <div class="card-header">
                    <h4 class="mb-0">Edit Product</h4>
                </div>

                <div class="card-body">

                    <div class="row">

                        <div class="col-md-6 mb-3">
                            <label class="form-label">Product Name</label>
                            <asp:TextBox ID="txtProdName"
                                runat="server"
                                CssClass="form-control" />
                        </div>

                        <div class="col-md-6 mb-3">
                            <label class="form-label">SKU</label>
                            <asp:TextBox ID="txtSKU"
                                runat="server"
                                CssClass="form-control" />
                        </div>

                    </div>

                    <div class="row">

                        <div class="col-md-6 mb-3">
                            <label class="form-label">Cost Price</label>
                            <asp:TextBox ID="txtCostPrice"
                                runat="server"
                                CssClass="form-control"
                                TextMode="Number" />
                        </div>

                        <div class="col-md-6 mb-3">
                            <label class="form-label">Reorder Level</label>
                            <asp:TextBox ID="txtReorderLevel"
                                runat="server"
                                CssClass="form-control"
                                TextMode="Number" />
                        </div>

                    </div>

                    <hr />

                    <div class="d-flex justify-content-end gap-2">

                        <asp:Button ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-secondary"
                            PostBackUrl="Products.aspx" />

                        <asp:Button ID="btnUpdate"
                            runat="server"
                            Text="Update Product"
                            CssClass="btn btn-primary"
                            OnClick="btnUpdate_Click" />

                    </div>

                    <asp:HiddenField ID="hfProductID"
                        runat="server" />

                </div>

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


</div></asp:Content>

