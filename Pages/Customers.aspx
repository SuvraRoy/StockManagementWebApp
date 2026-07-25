<%@ Page Title="Customers"
    Language="C#"
    MasterPageFile="~/landing.master"
    AutoEventWireup="true"
    CodeFile="Customers.aspx.cs"
    Inherits="Pages_Customers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .customer-page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 25px;
        }

        .customer-page-title {
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }

        .customer-page-subtitle {
            color: #6c757d;
            margin-top: 5px;
            margin-bottom: 0;
        }

        .customer-search-box {
            max-width: 350px;
        }

        .customer-table th {
            white-space: nowrap;
        }

        .customer-table td {
            vertical-align: middle;
        }

        .customer-code {
            font-weight: 600;
        }

        .customer-name {
            font-weight: 500;
        }

        .action-buttons {
            display: flex;
            gap: 6px;
            justify-content: center;
        }

        .empty-message {
            text-align: center;
            padding: 30px !important;
            color: #6c757d;
        }

        .status-badge {
            padding: 5px 10px;
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
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Page Header -->
        <div class="customer-page-header">

            <div>
                <h4 class="customer-page-title">
                    Customers
                </h4>

                <p class="customer-page-subtitle">
                    Manage your customers and their contact information.
                </p>
            </div>

            <div>
                <asp:HyperLink
                    ID="lnkAddCustomer"
                    runat="server"
                    NavigateUrl="~/Pages/CustomerForm.aspx"
                    CssClass="btn btn-primary">

                    <i class="bx bx-plus me-1"></i>
                    Add Customer

                </asp:HyperLink>
            </div>

        </div>


        <!-- Customer Card -->
        <div class="card">

            <!-- Card Header / Search -->
            <div class="card-header">

                <div class="row align-items-center g-3">

                    <div class="col-md-6">

                        <h5 class="mb-0">
                            Customer List
                        </h5>

                    </div>

                    <div class="col-md-6">

                        <div class="input-group customer-search-box ms-md-auto">

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Search by name, phone or code..."
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                            <asp:LinkButton
                                ID="btnSearch"
                                runat="server"
                                CssClass="btn btn-outline-primary"
                                OnClick="btnSearch_Click">

                                <i class="bx bx-search"></i>

                            </asp:LinkButton>

                            <asp:LinkButton
                                ID="btnClearSearch"
                                runat="server"
                                CssClass="btn btn-outline-secondary"
                                OnClick="btnClearSearch_Click">

                                <i class="bx bx-x"></i>

                            </asp:LinkButton>

                        </div>

                    </div>

                </div>

            </div>


            <!-- Customer Table -->
            <div class="table-responsive">

                <asp:GridView
                    ID="gvCustomers"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-hover customer-table mb-0"
                    GridLines="None"
                    AllowPaging="True"
                    PageSize="10"
                    OnPageIndexChanging="gvCustomers_PageIndexChanging"
                    OnRowCommand="gvCustomers_RowCommand"
                    EmptyDataText="No customers found.">

                    <Columns>

                        <asp:BoundField
                            DataField="CustomerCode"
                            HeaderText="Customer Code">
                            <ItemStyle CssClass="customer-code" />
                        </asp:BoundField>


                        <asp:BoundField
                            DataField="CustomerName"
                            HeaderText="Customer Name">
                            <ItemStyle CssClass="customer-name" />
                        </asp:BoundField>


                        <asp:BoundField
                            DataField="Phone"
                            HeaderText="Phone" />


                        <asp:BoundField
                            DataField="Email"
                            HeaderText="Email" />


                        <asp:BoundField
                            DataField="City"
                            HeaderText="City" />


                        <asp:TemplateField
                            HeaderText="Status">

                            <ItemTemplate>

                                <span class='<%# Convert.ToBoolean(Eval("IsActive")) ? "status-badge status-active" : "status-badge status-inactive" %>'>

                                    <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>

                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:TemplateField
                            HeaderText="Actions"
                            ItemStyle-HorizontalAlign="Center">

                            <ItemTemplate>

                                <div class="action-buttons">

                                    <asp:LinkButton
                                        ID="btnView"
                                        runat="server"
                                        CommandName="ViewCustomer"
                                        CommandArgument='<%# Eval("CustomerID") %>'
                                        CssClass="btn btn-sm btn-icon btn-outline-info"
                                        ToolTip="View Customer">

                                        <i class="bx bx-show"></i>

                                    </asp:LinkButton>


                                    <asp:LinkButton
                                        ID="btnEdit"
                                        runat="server"
                                        CommandName="EditCustomer"
                                        CommandArgument='<%# Eval("CustomerID") %>'
                                        CssClass="btn btn-sm btn-icon btn-outline-primary"
                                        ToolTip="Edit Customer">

                                        <i class="bx bx-edit"></i>

                                    </asp:LinkButton>

                                </div>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>


                    <HeaderStyle
                        CssClass="table-light" />


                    <PagerStyle
                        CssClass="pagination-container" />

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>