<%@ Page Title="Supplier Details" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="SupplierDetails.aspx.cs" Inherits="Pages_SupplierDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">


<style>

    .detail-label {
        font-size: 0.78rem;
        color: #8592a3;
        font-weight: 600;
        text-transform: uppercase;
        margin-bottom: 4px;
    }

    .detail-value {
        font-size: 0.95rem;
        color: #566a7f;
        font-weight: 500;
        min-height: 24px;
    }

    .detail-section-title {
        font-size: 1rem;
        font-weight: 600;
        color: #5F61E6;
        border-bottom: 1px solid #e9ecef;
        padding-bottom: 10px;
        margin-bottom: 20px;
    }

    .supplier-status {
        font-size: 0.85rem;
        padding: 7px 12px;
    }

</style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


<div class="container-xxl flex-grow-1 container-p-y">

    <!-- PAGE HEADER -->
    <div class="d-flex justify-content-between align-items-center mb-4">

        <div>
            <h4 class="fw-bold mb-1">
                Supplier Details
            </h4>

            <p class="text-muted mb-0">
                View complete supplier information
            </p>
        </div>

        <div class="d-flex gap-2">

            <asp:Button
                ID="btnBack"
                runat="server"
                Text="← Back"
                CssClass="btn btn-outline-secondary"
                CausesValidation="false"
                OnClick="btnBack_Click" />

            <asp:Button
                ID="btnEdit"
                runat="server"
                Text="Edit Supplier"
                CssClass="btn btn-primary"
                OnClick="btnEdit_Click" />

        </div>

    </div>


    <!-- ERROR MESSAGE -->
    <asp:Panel
        ID="pnlError"
        runat="server"
        Visible="false"
        CssClass="alert alert-danger">

        <asp:Label
            ID="lblError"
            runat="server">
        </asp:Label>

    </asp:Panel>


    <!-- SUPPLIER CONTENT -->
    <asp:Panel
        ID="pnlSupplier"
        runat="server"
        Visible="false">


        <!-- BASIC INFORMATION -->
        <div class="card mb-4">

            <div class="card-header">

                <h5 class="mb-0">
                    Basic Information
                </h5>

            </div>


            <div class="card-body">

                <div class="row g-4">

                    <div class="col-md-3">

                        <div class="detail-label">
                            Supplier ID
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblSupplierID"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            Supplier Code
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblSupplierCode"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            Company Name
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblCompanyName"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            Status
                        </div>

                        <asp:Label
                            ID="lblStatus"
                            runat="server"
                            CssClass="badge supplier-status">
                        </asp:Label>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            Created Date
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblCreatedDate"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- CONTACT INFORMATION -->
        <div class="card mb-4">

            <div class="card-body">

                <h6 class="detail-section-title">
                    Contact Information
                </h6>


                <div class="row g-4">

                    <div class="col-md-4">

                        <div class="detail-label">
                            Contact Person
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblContactPerson"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            Contact Person Number
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblContactPersonNo"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            Company Phone
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblPhone"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-6">

                        <div class="detail-label">
                            Email
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblEmail"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- ADDRESS INFORMATION -->
        <div class="card mb-4">

            <div class="card-body">

                <h6 class="detail-section-title">
                    Company Address
                </h6>


                <div class="row g-4">

                    <div class="col-md-3">

                        <div class="detail-label">
                            Country / Region
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblCountry"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            State
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblState"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            City
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblCity"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-3">

                        <div class="detail-label">
                            PIN Code
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblPinCode"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-12">

                        <div class="detail-label">
                            Address
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblAddress"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- TAX INFORMATION -->
        <div class="card mb-4">

            <div class="card-body">

                <h6 class="detail-section-title">
                    Tax Information
                </h6>


                <div class="row g-4">

                    <div class="col-md-4">

                        <div class="detail-label">
                            GSTIN
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblGSTIN"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            PAN
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblPAN"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- BANKING INFORMATION -->
        <div class="card mb-4">

            <div class="card-body">

                <h6 class="detail-section-title">
                    Banking Information
                </h6>


                <div class="row g-4">

                    <div class="col-md-4">

                        <div class="detail-label">
                            Bank Name
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblBankName"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            Account Holder Name
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblAccountHolderName"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            Account Type
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblAccountType"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            Account Number
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblAccountNumber"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>


                    <div class="col-md-4">

                        <div class="detail-label">
                            IFSC Code
                        </div>

                        <div class="detail-value">
                            <asp:Label
                                ID="lblIFSCCode"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </asp:Panel>

</div>


</asp:Content>
