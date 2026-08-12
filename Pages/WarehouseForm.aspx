<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="WarehouseForm.aspx.cs" Inherits="Pages_WarehouseForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <div class="row justify-content-center">

            <div class="col-lg-12">

                <div class="card">

                    <div class="card-header">
                        <h4 id="lblPageTitle"
                            runat="server"
                            class="mb-0 text-primary">Add Warehouse
                        </h4>
                    </div>

                    <div class="card-body">

                        <asp:HiddenField
                            ID="hfWarehouseID"
                            runat="server" />

                        <!-- Warehouse Code -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Warehouse Code
                            </label>

                            <div class="col-sm-4">

                                <asp:TextBox
                                    ID="txtWarehouseCode"
                                    runat="server"
                                    CssClass="form-control bg-light fw-semibold"
                                    ReadOnly="true">
                                </asp:TextBox>

                            </div>

                        </div>


                        <!-- Warehouse Name -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Warehouse Name
                                <span class="text-danger">*</span>
                            </label>

                            <div class="col-sm-7">

                                <asp:TextBox
                                    ID="txtWarehouseName"
                                    runat="server"
                                    CssClass="form-control"
                                    placeholder="Enter warehouse name">
                                </asp:TextBox>

                            </div>

                        </div>

                        <!-- Warehouse Type -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Warehouse Type
                            <span class="text-danger">*</span>
                            </label>

                            <div class="col-sm-4">

                                <asp:DropDownList
                                    ID="ddlWarehouseType"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem Text="Main Warehouse" Value="Main" />
                                    <asp:ListItem Text="Branch Warehouse" Value="Branch" />

                                </asp:DropDownList>

                            </div>

                        </div>

                        <!-- Phone -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Phone
                            </label>

                            <div class="col-sm-5">

                                <asp:TextBox
                                    ID="txtPhone"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="10"
                                    placeholder="Phone number">
                                </asp:TextBox>

                            </div>

                        </div>

                        <!-- Email -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Email Address
                            </label>

                            <div class="col-sm-6">

                                <asp:TextBox
                                    ID="txtEmail"
                                    runat="server"
                                    CssClass="form-control"
                                    placeholder="Email Address">
                                </asp:TextBox>

                            </div>

                        </div>

                        <!-- Address Section -->
                        <div class="border-top pt-4 mt-4">

                            <h5 class="text-primary mb-4">Warehouse Address
                            </h5>

                            <div class="row mb-3">

                                <div class="col-md-3">

                                    <label class="form-label text-secondary">
                                        Country
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlCountry"
                                        runat="server"
                                        CssClass="form-select">

                                        <asp:ListItem
                                            Text="India"
                                            Value="IN"
                                            Selected="True" />

                                        <asp:ListItem
                                            Text="United States"
                                            Value="US" />

                                    </asp:DropDownList>

                                </div>


                                <div class="col-md-3">

                                    <label class="form-label text-secondary">
                                        State
                                    </label>

                                    <asp:TextBox
                                        ID="txtState"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="State">
                                    </asp:TextBox>

                                </div>


                                <div class="col-md-3">

                                    <label class="form-label text-secondary">
                                        City
                                    </label>

                                    <asp:TextBox
                                        ID="txtCity"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="City">
                                    </asp:TextBox>

                                </div>


                                <div class="col-md-3">

                                    <label class="form-label text-secondary">
                                        Pin Code
                                    </label>

                                    <asp:TextBox
                                        ID="txtPinCode"
                                        runat="server"
                                        CssClass="form-control"
                                        MaxLength="10"
                                        placeholder="Pin Code">
                                    </asp:TextBox>

                                </div>

                            </div>


                            <div class="row mb-3">

                                <div class="col-md-12">

                                    <label class="form-label text-secondary">
                                        Address
                                    </label>

                                    <asp:TextBox
                                        ID="txtAddress"
                                        runat="server"
                                        CssClass="form-control"
                                        TextMode="MultiLine"
                                        Rows="3"
                                        placeholder="Warehouse address">
                                    </asp:TextBox>

                                </div>

                            </div>

                        </div>


                        <!-- BUTTONS -->
                        <div class="border-top pt-4 mt-4 text-end">

                            <asp:Button
                                ID="btnCancel"
                                runat="server"
                                Text="Cancel"
                                CssClass="btn btn-secondary"
                                CausesValidation="false"
                                OnClick="btnCancel_Click" />

                            <asp:Button
                                ID="btnSave"
                                runat="server"
                                Text="Save Warehouse"
                                CssClass="btn btn-primary ms-2"
                                OnClick="btnSave_Click" />

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- TOAST -->

        <div class="toast-container position-fixed top-0 end-0 p-3"
            style="z-index: 9999;">

            <div id="liveToast"
                runat="server"
                class="toast shadow-lg border-0"
                role="alert"
                aria-live="assertive"
                aria-atomic="true">

                <div class="toast-header">

                    <strong class="me-auto">System Alert
                    </strong>

                    <button type="button"
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
