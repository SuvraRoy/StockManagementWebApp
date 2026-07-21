<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="SupplierForm.aspx.cs" Inherits="Pages_SupplierForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <div class="row mb-6 gy-6">

            <%-- Main Form --%>
            <div id="divForm" runat="server" class="col-lg-12">

                <div class="card">
                    <asp:HiddenField ID="hfSupplierID" runat="server" />
                    <div class="card-header">
                        <h4
                            id="lblPageTitle"
                            runat="server"
                            class="mb-0 text-primary">Add Supplier
                        </h4>
                    </div>

                    <div id="divMessage" runat="server" visible="false"
                        class="alert alert-danger alert-dismissible fade show mx-3"
                        role="alert">

                        <asp:Label ID="lblMessage" runat="server"></asp:Label>

                        <button type="button"
                            class="btn-close"
                            data-bs-dismiss="alert">
                        </button>
                    </div>

                    <div class="card-body mt-4">

                        <!-- Supplier Code -->
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">
                                Supplier Code 
                            </label>

                            <div class="col-sm-3">
                                <asp:TextBox
                                    ID="txtSupplierCode"
                                    runat="server"
                                    CssClass="form-control bg-light fw-semibold"
                                    ReadOnly="true">
                                </asp:TextBox>
                            </div>

                        </div>
                        <!-- Company Name Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary" aria-required="true">Company Name <span class="text-danger">*</span></label>
                            <div class="col-sm-9">

                                <%-- <asp:RequiredFieldValidator ID="rfcompany" runat="server" ErrorMessage="Please Enter the company name" ControlToValidate="txtCompanyName" ValidationGroup="Supplier" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>--%>

                                <asp:TextBox ID="txtCompanyName" runat="server" CssClass="form-control" placeholder="Company Name"></asp:TextBox>
                            </div>
                        </div>


                        <!-- Primary Contact Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">Contact Person</label>

                            <div class="col-sm-5 mb-2">
                                <asp:TextBox ID="txtContactName" runat="server" CssClass="form-control" placeholder="Contact Person Name"></asp:TextBox>
                            </div>
                        </div>

                        <%-- Contact Person Number --%>
                        <div class="row mb-3 align-items-center">

                            <label class="col-sm-3 col-form-label text-secondary">Contact Person Number </label>
                            <div class="col-sm-3">
                                <asp:TextBox ID="txtContMobile" runat="server" MaxLength="10" CssClass="form-control" placeholder="Mobile"></asp:TextBox>
                            </div>
                        </div>


                        <!-- Email Address Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary" aria-required="true">Email Address <span class="text-danger">*</span></label>
                            <div class="col-sm-9">
                                <%--<asp:RequiredFieldValidator ID="rfEmail" runat="server" ErrorMessage="Enter Proper Email ID" CssClass="text-danger" ControlToValidate="txtEmail" Display="Dynamic" ValidationGroup="Supplier"></asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator
                                    ID="revEmail"
                                    runat="server"
                                    ControlToValidate="txtEmail"
                                    Display="Dynamic"
                                    CssClass="text-danger"
                                    ValidationGroup="Supplier" />

                                <div class="input-group">
                                    <span class="input-group-text bg-light">✉</span>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter email address"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <%-- Phone Number Row --%>
                        <div class="row mb-4 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary" aria-required="true">Company Phone Number <span class="text-danger">*</span></label>
                            <div class="col-sm-4">
                                <%--<asp:RequiredFieldValidator ID="rfMobile" runat="server" MaxLength="10" ErrorMessage="Please enter a valid mobile number" ControlToValidate="txtWorkPhone" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>--%>

                                <div class="input-group">
                                    <span class="input-group-text bg-light">📞</span>
                                    <asp:TextBox ID="txtWorkPhone" runat="server" CssClass="form-control" placeholder="Work Phone" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <!-- SECTION 2: ADDRESSES -->
                        <div class="mb-5 border-top pt-4">
                            <div class="row g-4">
                                <!-- Billing Address Column -->

                                <h5 class="text-primary mb-3">Company Address</h5>


                                <div class="row mb-3 align-items-center">

                                    <div class="col-sm-3 mb-2">
                                        <label class="form-label text-secondary small fw-bold">Country/Region</label>
                                        <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-select">
                                            <%--<asp:ListItem Text="Select Country" Value="" />--%>
                                            <asp:ListItem Text="India" Value="IN" Selected="True" />
                                            <asp:ListItem Text="United States" Value="US" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-sm-3 mb-2">
                                        <label class="form-label text-secondary small fw-bold">State</label>
                                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" placeholder="State"></asp:TextBox>
                                    </div>

                                    <div class="col-sm-3 mb-2">
                                        <label class="form-label text-secondary small fw-bold">City</label>
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="City"></asp:TextBox>
                                    </div>

                                    <div class="col-sm-3 mb-2">
                                        <label class="form-label text-secondary small fw-bold">Pin Code</label>
                                        <asp:TextBox ID="txtPincode" runat="server" CssClass="form-control" placeholder="Pin Code"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="mb-3">
                                    <label class="form-label text-secondary small fw-bold">Address</label>
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control mb-2" placeholder="Address Line "></asp:TextBox>
                                </div>

                            </div>
                        </div>

                        <!-- SECTION 4: Finance -->
                        <!-- GSTIN Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">GSTIN <span class="text-danger">* </span></label>
                            <div class="col-sm-4">
                                <%--<asp:RequiredFieldValidator ID="rfGSTIN" runat="server" ErrorMessage="Enter Proper GSTIN No. " ControlToValidate="txtGstIn" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>--%>
                                <asp:TextBox ID="txtGstIn" runat="server" CssClass="form-control" placeholder="15-Digit GSTIN" MaxLength="15"></asp:TextBox>
                            </div>
                        </div>

                        <!-- PAN Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">PAN</label>
                            <div class="col-sm-3">
                                <asp:TextBox ID="txtPan" runat="server" CssClass="form-control" placeholder="10-Digit PAN" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Bank Name Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">Bank Name</label>
                            <div class="col-sm-6">
                                <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" placeholder="Bank Name"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Account Holder Name Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">Account Holder Name</label>
                            <div class="col-sm-4">
                                <asp:TextBox ID="txtAccountHolder" runat="server" CssClass="form-control" placeholder="Account Holder Name"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Account Type Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">Account Type</label>
                            <div class="col-sm-3">
                                <asp:DropDownList ID="ddlAccountType" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Current" Value="Current" />
                                    <asp:ListItem Text="Savings" Value="Savings" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Account Number Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">Account Number </label>
                            <div class="col-sm-4">
                                <%--<asp:RequiredFieldValidator ID="rfAccNo" ControlToValidate="txtAccountNumber" runat="server" ErrorMessage="Enter Account Number"></asp:RequiredFieldValidator>--%>
                                <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder="Account Number"></asp:TextBox>
                            </div>
                        </div>

                        <!-- IFSC Code Row -->
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-3 col-form-label text-secondary">IFSC Code</label>
                            <div class="col-sm-4">
                                <%--<asp:RequiredFieldValidator ID="rfIfscCode" ControlToValidate="txtIfscCode" Display="Dynamic" runat="server" ErrorMessage="Enter IFSC Code"></asp:RequiredFieldValidator>--%>
                                <asp:TextBox ID="txtIfscCode" runat="server" CssClass="form-control" placeholder="IFSC Code"></asp:TextBox>
                            </div>
                        </div>
                    </div>



                    <%--</div>--%>
                    <div class="card-footer text-end">

                        <asp:Button
                            ID="btnSave"
                            runat="server"
                            CssClass="btn btn-primary"
                            Text="Save"
                            OnClick="btnSave_Click" />

                        <%--                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-warning ms-2"
                            CausesValidation="false"
                            OnClick="btnReset_Click" />--%>

                        <asp:Button
                            ID="btnCancel"
                            runat="server"
                            Text="Cancel"
                            CssClass="btn btn-secondary ms-2"
                            CausesValidation="false"
                            OnClick="btnCancel_Click" />

                    </div>
                </div>
            </div>


        </div>
    </div>

    <%-- Toast --%>
    <div class="toast-container position-fixed top-0 end-0 p-3">

        <div id="liveToast"
            runat="server"
            class="toast shadow-lg"
            role="alert"
            aria-live="assertive"
            aria-atomic="true">

            <div class="toast-body">

                <asp:Label
                    ID="lblToast"
                    runat="server">
                </asp:Label>

            </div>

        </div>

    </div>
    <%--</div>--%>
</asp:Content>

