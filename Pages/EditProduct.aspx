<%@ Page Title="Edit Product" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="EditProduct.aspx.cs" Inherits="Pages_EditProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


<div class="container-xxl flex-grow-1 container-p-y">

    <div class="row justify-content-center">

        <div class="col-xl-8 col-lg-10 col-md-12">

            <div class="card">

                <!-- HEADER -->
                <div class="card-header">

                    <div class="d-flex align-items-center justify-content-between">

                        <div>
                            <h4 class="mb-1">Edit Product</h4>
                            <small class="text-muted">
                                Update product information and inventory settings.
                            </small>
                        </div>

                        <asp:Label
                            ID="lblProductStatus"
                            runat="server"
                            CssClass="badge bg-label-success">
                        </asp:Label>

                    </div>

                </div>


                <!-- BODY -->
                <div class="card-body">

                    <!-- PRODUCT NAME -->
                    <div class="mb-4">

                        <label class="form-label">
                            Product Name <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtProdName"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Enter product name">
                        </asp:TextBox>

                    </div>


                    <!-- SKU + COST -->
                    <div class="row">

                        <div class="col-md-6 mb-4">

                            <label class="form-label">
                                SKU <span class="text-danger">*</span>
                            </label>

                            <asp:TextBox
                                ID="txtSKU"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Enter SKU">
                            </asp:TextBox>

                        </div>


                        <div class="col-md-6 mb-4">

                            <label class="form-label">
                                Cost Price <span class="text-danger">*</span>
                            </label>

                            <asp:TextBox
                                ID="txtCostPrice"
                                runat="server"
                                CssClass="form-control"
                                TextMode="Number"
                                step="0.01"
                                placeholder="0.00">
                            </asp:TextBox>

                        </div>

                    </div>


                    <!-- STOCK + REORDER -->
                    <div class="row">

                        <div class="col-md-6 mb-4">

                            <label class="form-label">
                                Current Stock
                            </label>

                            <asp:TextBox
                                ID="txtCurrentStock"
                                runat="server"
                                CssClass="form-control"
                                ReadOnly="true">
                            </asp:TextBox>

                            <small class="text-muted">
                                Stock is updated automatically through Purchase and Sales transactions.
                            </small>

                        </div>


                        <div class="col-md-6 mb-4">

                            <label class="form-label">
                                Reorder Level <span class="text-danger">*</span>
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


                    <hr class="my-4" />


                    <!-- ACTIONS -->
                    <div class="d-flex justify-content-end gap-2">

                        <asp:Button
                            ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-label-secondary"
                            PostBackUrl="Products.aspx" />

                        <asp:Button
                            ID="btnUpdate"
                            runat="server"
                            Text="Update Product"
                            CssClass="btn btn-primary"
                            OnClick="btnUpdate_Click" />

                    </div>


                    <!-- PRODUCT ID -->
                    <asp:HiddenField
                        ID="hfProductID"
                        runat="server" />

                </div>

            </div>

        </div>

    </div>


    <!-- TOAST -->
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

</div>


</asp:Content>
