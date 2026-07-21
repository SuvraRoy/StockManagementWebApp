<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <!-- Content -->
    <div class="container-xxl flex-grow-1 container-p-y">
        <div class="row">

            <div class="col-xxl-12 col-lg-12 col-md-4 order-1">
                <div class="row">
                    <div class="col-lg-3 col-md-4 col-6 mb-6">
                        <div class="card h-100">
                            <div class="card-body">
                                <div class="card-title d-flex align-items-start justify-content-between mb-4">
                                    <div class="avatar flex-shrink-0">
                                        <img
                                            src="../assets/img/icons/unicons/chart-success.png"
                                            alt="chart success"
                                            class="rounded" />
                                    </div>
                                    <div class="dropdown">
                                        <button
                                            class="btn p-0"
                                            type="button"
                                            id="cardOpt3"
                                            data-bs-toggle="dropdown"
                                            aria-haspopup="true"
                                            aria-expanded="false">
                                            <i class="icon-base bx bx-dots-vertical-rounded text-body-secondary"></i>
                                        </button>
                                        <div class="dropdown-menu dropdown-menu-end" aria-labelledby="cardOpt3">
                                            <a class="dropdown-item" href="javascript:void(0);">View More</a>
                                        </div>
                                    </div>
                                </div>
                                <p class="mb-1">Total Products</p>
                                <h4 class="card-title mb-3">
                                    <asp:Label ID="lblProducts" runat="server" Text="0"></asp:Label>
                                </h4>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-4 col-6 mb-6">
                        <div class="card h-100">
                            <div class="card-body">
                                <div class="card-title d-flex align-items-start justify-content-between mb-4">
                                    <div class="avatar flex-shrink-0">
                                        <img
                                            src="../assets/img/icons/unicons/wallet-info.png"
                                            alt="wallet info"
                                            class="rounded" />
                                    </div>
                                    <div class="dropdown">
                                        <button
                                            class="btn p-0"
                                            type="button"
                                            id="cardOpt6"
                                            data-bs-toggle="dropdown"
                                            aria-haspopup="true"
                                            aria-expanded="false">
                                            <i class="icon-base bx bx-dots-vertical-rounded text-body-secondary"></i>
                                        </button>
                                        <div class="dropdown-menu dropdown-menu-end" aria-labelledby="cardOpt6">
                                            <a class="dropdown-item" href="javascript:void(0);">View More</a>
                                        </div>
                                    </div>
                                </div>
                                <p class="mb-1">Total Suppliers</p>
                                <h4 class="card-title mb-3">
                                    <asp:Label ID="lblSuppliers" runat="server" Text="0"></asp:Label>
                                </h4>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-md-4 col-6 mb-6">
                        <div class="card h-100">
                            <div class="card-body">
                                <div class="card-title d-flex align-items-start justify-content-between mb-4">
                                    <div class="avatar flex-shrink-0">
                                        <img
                                            src="../assets/img/icons/unicons/wallet-info.png"
                                            alt="wallet info"
                                            class="rounded" />
                                    </div>
                                    <div class="dropdown">
                                        <button
                                            class="btn p-0"
                                            type="button"
                                            id="cardOpt6"
                                            data-bs-toggle="dropdown"
                                            aria-haspopup="true"
                                            aria-expanded="false">
                                            <i class="icon-base bx bx-dots-vertical-rounded text-body-secondary"></i>
                                        </button>
                                        <div class="dropdown-menu dropdown-menu-end" aria-labelledby="cardOpt6">
                                            <a class="dropdown-item" href="javascript:void(0);">View More</a>
                                        </div>
                                    </div>
                                </div>
                                <p class="mb-1">Total Purchases</p>
                                <h4 class="card-title mb-3">
                                    <asp:Label ID="lblPurchases" runat="server" Text="0"></asp:Label>
                                </h4>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-md-4 col-6 mb-6">
                        <div class="card h-100">
                            <div class="card-body">
                                <div class="card-title d-flex align-items-start justify-content-between mb-4">
                                    <div class="avatar flex-shrink-0">
                                        <img
                                            src="../assets/img/icons/unicons/wallet-info.png"
                                            alt="wallet info"
                                            class="rounded" />
                                    </div>
                                    <div class="dropdown">
                                        <button
                                            class="btn p-0"
                                            type="button"
                                            id="cardOpt6"
                                            data-bs-toggle="dropdown"
                                            aria-haspopup="true"
                                            aria-expanded="false">
                                            <i class="icon-base bx bx-dots-vertical-rounded text-body-secondary"></i>
                                        </button>
                                        <div class="dropdown-menu dropdown-menu-end" aria-labelledby="cardOpt6">
                                            <a class="dropdown-item" href="javascript:void(0);">View More</a>
                                        </div>
                                    </div>
                                </div>
                                <p class="mb-1">Low Stock Items</p>
                                <h4 class="card-title mb-3">
                               <asp:Label ID="lblLowStock" runat="server" Text="0"></asp:Label>
                              </h4>
                            </div>
                        </div>
                    </div>


                </div>
            </div>

        </div>
    </div>
    <!-- / Content -->

</asp:Content>

