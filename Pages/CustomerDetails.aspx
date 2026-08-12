<%@ Page Title="Customer Details"
    Language="C#"
    MasterPageFile="~/landing.master"
    AutoEventWireup="true"
    CodeFile="CustomerDetails.aspx.cs"
    Inherits="Pages_CustomerDetails" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="Server">


    <style>
        .customer-details-title {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 5px;
        }

        .customer-details-subtitle {
            color: #6c757d;
            margin-bottom: 0;
        }

        .profile-header {
            display: flex;
            align-items: center;
            gap: 20px;
        }

        .customer-avatar {
            width: 70px;
            height: 70px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: #696cff;
            color: #ffffff;
            font-size: 28px;
            font-weight: 600;
            flex-shrink: 0;
        }

        .customer-name {
            font-size: 22px;
            font-weight: 600;
            margin-bottom: 5px;
        }

        .customer-code {
            color: #697a8d;
            font-size: 14px;
        }

        .status-badge {
            display: inline-block;
            padding: 5px 12px;
            border-radius: 50px;
            font-size: 12px;
            font-weight: 500;
        }

        .status-active {
            background-color: #e8fadf;
            color: #71dd37;
        }

        .status-inactive {
            background-color: #ffe7e7;
            color: #ff3e1d;
        }

        .detail-section-title {
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 20px;
        }

        .detail-item {
            margin-bottom: 20px;
        }

        .detail-label {
            display: block;
            color: #697a8d;
            font-size: 13px;
            margin-bottom: 5px;
        }

        .detail-value {
            display: block;
            font-size: 15px;
            font-weight: 500;
            color: #384551;
            word-break: break-word;
        }

        .empty-value {
            color: #a1acb8;
            font-weight: 400;
        }

        .page-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
    </style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">

            <div>

                <h4 class="customer-details-title">Customer Details
                </h4>

                <p class="customer-details-subtitle">
                    View complete customer information.
                </p>

            </div>

            <div class="page-actions">

                <asp:HyperLink
                    ID="lnkBack"
                    runat="server"
                    NavigateUrl="~/Pages/Customers.aspx"
                    CssClass="btn btn-outline-secondary">

                <i class="bx bx-arrow-back me-1"></i>
                Back to Customers

                </asp:HyperLink>

                <asp:HyperLink
                    ID="lnkEdit"
                    runat="server"
                    CssClass="btn btn-primary">

                <i class="bx bx-edit me-1"></i>
                Edit Customer

                </asp:HyperLink>

            </div>

        </div>


        <!-- Customer Profile -->
        <div class="card mb-4">

            <div class="card-body">

                <div class="profile-header">

                    <!-- Customer Name / Code / Status -->
                    <div>

                        <div class="customer-name">

                            <asp:Label
                                ID="lblCustomerName"
                                runat="server">
                            </asp:Label>

                        </div>

                        <div class="customer-code">

                            <asp:Label
                                ID="lblCustomerCode"
                                runat="server">
                            </asp:Label>

                        </div>

                        <div class="mt-2">

                            <asp:Label
                                ID="lblStatus"
                                runat="server"
                                CssClass="status-badge">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- Contact Information -->
        <div class="card mb-4">

            <div class="card-body">

                <h5 class="detail-section-title">Contact Information
                </h5>

                <div class="row">

                    <!-- Phone -->
                    <div class="col-md-6">

                        <div class="detail-item">

                            <span class="detail-label">Phone Number
                            </span>

                            <asp:Label
                                ID="lblPhone"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- Email -->
                    <div class="col-md-6">

                        <div class="detail-item">

                            <span class="detail-label">Email Address
                            </span>

                            <asp:Label
                                ID="lblEmail"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- Address -->
                    <div class="col-12">

                        <div class="detail-item">

                            <span class="detail-label">Address
                            </span>

                            <asp:Label
                                ID="lblAddress"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- City -->
                    <div class="col-md-4">

                        <div class="detail-item">

                            <span class="detail-label">City
                            </span>

                            <asp:Label
                                ID="lblCity"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- State -->
                    <div class="col-md-4">

                        <div class="detail-item">

                            <span class="detail-label">State
                            </span>

                            <asp:Label
                                ID="lblState"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- PIN Code -->
                    <div class="col-md-4">

                        <div class="detail-item">

                            <span class="detail-label">PIN Code
                            </span>

                            <asp:Label
                                ID="lblPinCode"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- Business Information -->
        <div class="card mb-4">

            <div class="card-body">

                <h5 class="detail-section-title">Business Information
                </h5>

                <div class="row">

                    <!-- GSTIN -->
                    <div class="col-md-6">

                        <div class="detail-item">

                            <span class="detail-label">GSTIN
                            </span>

                            <asp:Label
                                ID="lblGSTIN"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- Created Date -->
                    <div class="col-md-6">

                        <div class="detail-item">

                            <span class="detail-label">Customer Since
                            </span>

                            <asp:Label
                                ID="lblCreatedDate"
                                runat="server"
                                CssClass="detail-value">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- Error Message -->
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

    </div>


</asp:Content>
