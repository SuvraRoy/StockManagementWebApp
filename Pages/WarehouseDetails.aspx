<%@ Page Title="Warehouse Details" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="WarehouseDetails.aspx.cs"
    Inherits="Pages_WarehouseDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .detail-label {
            font-size: 13px;
            font-weight: 600;
            color: #697a8d;
            margin-bottom: 5px;
        }

        .detail-value {
            font-size: 15px;
            color: #333;
        }

        .warehouse-title {
            font-size: 24px;
            font-weight: 600;
        }

        .badge-main {
            background-color: #696cff;
            color: white;
        }

        .badge-branch {
            background-color: #8592a3;
            color: white;
        }

        .badge-active {
            background-color: #71dd37;
            color: white;
        }

        .badge-inactive {
            background-color: #ff3e1d;
            color: white;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <div class="row">

            <div class="col-lg-12">

                <div class="card">

                    <!-- HEADER -->
                    <div class="card-header">

                        <div class="d-flex justify-content-between align-items-center">

                            <div>

                                <div class="d-flex align-items-center gap-2 flex-wrap">

                                    <h4 class="warehouse-title mb-0">
                                        <asp:Label
                                            ID="lblWarehouseName"
                                            runat="server">
                                        </asp:Label>
                                    </h4>

                                    <asp:Label
                                        ID="lblWarehouseType"
                                        runat="server"
                                        CssClass="badge">
                                    </asp:Label>

                                    <asp:Label
                                        ID="lblStatus"
                                        runat="server"
                                        CssClass="badge">
                                    </asp:Label>

                                </div>

                                <small class="text-muted">Warehouse Details
                                </small>

                            </div>

                            <div>

                                <asp:Button
                                    ID="btnEdit"
                                    runat="server"
                                    Text="Edit"
                                    CssClass="btn btn-primary"
                                    OnClick="btnEdit_Click" />

                                <asp:Button
                                    ID="btnBack"
                                    runat="server"
                                    Text="Back"
                                    CssClass="btn btn-secondary ms-2"
                                    CausesValidation="false"
                                    OnClick="btnBack_Click" />

                            </div>

                        </div>

                    </div>


                    <!-- BODY -->
                    <div class="card-body">

                        <!-- BASIC INFORMATION -->
                        <h5 class="text-primary mb-4">Basic Information
                        </h5>

                        <div class="row mb-4">

                            <div class="col-md-3 mb-3">
                                <div class="detail-label">
                                    Warehouse Code
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblWarehouseCode"
                                        runat="server">
                                    </asp:Label>
                                </div>
                            </div>


                            <div class="col-md-3 mb-3">
                                <div class="detail-label">
                                    Warehouse Type
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblTypeDetail"
                                        runat="server">
                                    </asp:Label>
                                </div>
                            </div>


                            <div class="col-md-3 mb-3">
                                <div class="detail-label">
                                    Phone
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblPhone"
                                        runat="server">
                                    </asp:Label>
                                </div>
                            </div>

                            <div class="col-md-3 mb-3">
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


                        <hr class="my-4" />


                        <!-- ADDRESS -->
                        <h5 class="text-primary mb-4">Address
                        </h5>

                        <div class="row mb-4">

                            <div class="col-md-3 mb-3">
                                <div class="detail-label">
                                    Country
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblCountry"
                                        runat="server">
                                    </asp:Label>
                                </div>
                            </div>


                            <div class="col-md-3 mb-3">
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


                            <div class="col-md-3 mb-3">
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


                            <div class="col-md-3 mb-3">
                                <div class="detail-label">
                                    Pin Code
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblPinCode"
                                        runat="server">
                                    </asp:Label>
                                </div>
                            </div>

                        </div>


                        <div class="row mb-4">

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


                        <hr class="my-4" />


                        <!-- STATUS -->
                        <h5 class="text-primary mb-4">Status
                        </h5>

                        <div class="row">

                            <div class="col-md-4">

                                <div class="detail-label">
                                    Current Status
                                </div>

                                <div class="detail-value">
                                    <asp:Label
                                        ID="lblStatusDetail"
                                        runat="server"
                                        CssClass="badge">
                                    </asp:Label>
                                </div>

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
