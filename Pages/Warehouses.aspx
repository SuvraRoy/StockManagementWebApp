<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="Warehouses.aspx.cs" Inherits="Pages_Warehouses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .warehouseHeader th,
        .warehouseHeader {
            background-color: #5F61E6 !important;
            color: white !important;
            font-weight: bold;
        }

        .pagination-area {
            text-align: center;
        }

        .pagination-area table {
            margin: auto;
            }

        .pagination-area a,
        .pagination-area span {
            padding: 6px 12px;
            margin: 0 2px;
            border: 1px solid #dee2e6;
            border-radius: 6px;
            text-decoration: none;
            }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <div class="row mb-6 gy-6">

            <div class="col-lg-12">

                <div class="card">

                    <!-- HEADER -->
                    <div class="row mx-2 px-4">

                        <div class="row align-items-center justify-content-between g-3">

                            <div class="col-auto">
                                <h5 class="mb-0">Warehouse List</h5>
                            </div>

                            <div class="col-auto d-flex align-items-center gap-2">

                                <!-- SEARCH -->
                                <div class="input-group">

                                    <span class="input-group-text">
                                        <i class="icon-base bx bx-search"></i>
                                    </span>

                                    <asp:TextBox
                                        ID="txtSearch"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="Search warehouse..."
                                        AutoPostBack="true"
                                        OnTextChanged="txtSearch_TextChanged">
                                    </asp:TextBox>

                                </div>

                                <!-- ADD -->
                                <asp:Button
                                    ID="btnAddWarehouse"
                                    runat="server"
                                    Text="+ Add Warehouse"
                                    CssClass="btn btn-primary text-nowrap"
                                    OnClick="btnAddWarehouse_Click" />

                            </div>

                        </div>

                    </div>


                    <!-- TABLE -->
                    <div class="card-body">

                        <div class="table-responsive text-nowrap">

                            <asp:GridView
                                ID="gvWarehouses"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="table table-hover align-middle"
                                DataKeyNames="WarehouseID"
                                AllowPaging="true"
                                PageSize="10"
                                OnPageIndexChanging="gvWarehouses_PageIndexChanging"
                                OnRowCommand="gvWarehouses_RowCommand">

                                <PagerSettings
                                    Mode="NextPreviousFirstLast"
                                    PreviousPageText="◀ Prev"
                                    NextPageText="Next ▶"
                                    FirstPageText="⏮ First"
                                    LastPageText="Last ⏭" />

                                <PagerStyle
                                    CssClass="pagination-area"
                                    HorizontalAlign="Center" />

                                <HeaderStyle
                                    CssClass="warehouseHeader"
                                    Height="40px"
                                    HorizontalAlign="Left" />

                                <RowStyle Height="50px" />

                                <AlternatingRowStyle BackColor="#FAFAFA" />


                                <Columns>
                                    <asp:BoundField DataField="WarehouseCode" HeaderText="Code" />
                                    <asp:BoundField DataField="WarehouseName" HeaderText="Warehouse Name" />

                                    <asp:TemplateField HeaderText="Location">

                                        <ItemTemplate>

                                            <%# GetLocation(
                                                Eval("City"),
                                                Eval("State")
                                            ) %>
                                        </ItemTemplate>

                                    </asp:TemplateField>



                                   <%-- <asp:TemplateField HeaderText="Type">

                                        <ItemTemplate>

                                            <asp:Label
                                                ID="lblWarehouseType"
                                                runat="server"
                                                Text='<%# Convert.ToBoolean(Eval("IsMainWarehouse")) ? "Main Warehouse" : "Warehouse" %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("IsMainWarehouse")) ? "badge bg-label-primary" : "badge bg-label-secondary" %>'>
                                            </asp:Label>

                                        </ItemTemplate>

                                    </asp:TemplateField>--%>



                                    <asp:TemplateField HeaderText="Status">

                                        <ItemTemplate>

                                            <asp:Label
                                                ID="lblStatus"
                                                runat="server"
                                                Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-label-success" : "badge bg-label-danger" %>'>
                                            </asp:Label>

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>

                                            <span class='<%# Convert.ToString(Eval("WarehouseType")) == "Main" ? "badge bg-primary"  : "badge bg-label-secondary" %>'>
                                                <%# Convert.ToString(Eval("WarehouseType")) == "Main"? "Main": "Branch" %>
                                            </span>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Actions">

                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="btnView"
                                                runat="server"
                                                CommandName="ViewWarehouse"
                                                CommandArgument='<%# Eval("WarehouseID") %>'
                                                CssClass="btn btn-sm btn-outline-secondary me-1"
                                                ToolTip="View Warehouse">

                                                <i class="bx bx-show"></i>

                                            </asp:LinkButton>



                                            <asp:LinkButton
                                                ID="btnEdit"
                                                runat="server"
                                                CommandName="EditWarehouse"
                                                CommandArgument='<%# Eval("WarehouseID") %>'
                                                CssClass="btn btn-sm btn-outline-primary me-1"
                                                ToolTip="Edit Warehouse">

                                                <i class="bx bx-edit"></i>

                                            </asp:LinkButton>


                                            <asp:LinkButton
                                                ID="btnToggleStatus"
                                                runat="server"
                                                CommandName="ToggleStatus"
                                                CommandArgument='<%# Eval("WarehouseID") %>'
                                                CssClass="btn btn-sm btn-outline-warning"
                                                ToolTip="Change Status"
                                                OnClientClick="return confirm('Are you sure you want to change this warehouse status?');">

                                                <i class="bx bx-power-off"></i>

                                            </asp:LinkButton>

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>

                    </div>

                </div>

            </div>

        </div>

<%-- Toast Message --%>
        <div class="toast-container position-fixed top-0 end-0 p-4"
            style="z-index: 9999;">

            <div
                id="liveToast"
                runat="server"
                class="toast shadow-lg border-0"
                role="alert"
                aria-live="assertive"
                aria-atomic="true">

                <div class="toast-header bg-transparent border-bottom">

                    <i class="bx bx-bell me-2 text-primary"></i>

                    <strong class="me-auto">System Alert
                    </strong>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="toast">
                    </button>

                </div>

                <div class="toast-body fw-bold">

                    <asp:Label
                        ID="lblToast"
                        runat="server">
                    </asp:Label>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
