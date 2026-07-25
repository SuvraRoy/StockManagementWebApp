<%@ Page Title="" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="Suppliers.aspx.cs" Inherits="Pages_Suppliers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">


<style>

    .supplierHeader th,
    .supplierHeader {
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

    .supplier-action-btn {
        min-width: 95px;
    }

</style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


<div class="container-xxl flex-grow-1 container-p-y">

    <div class="row mb-6 gy-6">

        <div id="divGrid" runat="server" class="col-lg-12">

            <div class="card">

                <!-- HEADER -->
                <div class="row mx-2 px-4">

                    <div class="row align-items-center justify-content-between g-3">

                        <div class="col-auto">

                            <h5 class="mb-0">
                                Supplier List
                            </h5>

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
                                    placeholder="Search suppliers..."
                                    AutoPostBack="true"
                                    OnTextChanged="txtSearch_TextChanged">
                                </asp:TextBox>

                            </div>


                            <!-- ADD -->
                            <asp:Button
                                ID="btnAddSupplier"
                                runat="server"
                                Text="+ Add Supplier"
                                CssClass="btn btn-primary text-nowrap"
                                OnClick="btnAddSupplier_Click" />

                        </div>

                    </div>

                </div>


                
                <div class="card-body">

                    <div class="table-responsive text-nowrap">

                        <asp:GridView
                            ID="gvSuppliers"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            DataKeyNames="SupplierID"
                            AllowPaging="true"
                            PageSize="10"
                            OnPageIndexChanging="gvSuppliers_PageIndexChanging"
                            OnRowCommand="gvSuppliers_RowCommand1">

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
                                CssClass="supplierHeader"
                                Height="40px"
                                HorizontalAlign="Left" />

                            <RowStyle Height="50px" />

                            <AlternatingRowStyle
                                BackColor="#FAFAFA" />


                            <Columns>

                                
                                <asp:BoundField
                                    DataField="SupplierCode"
                                    HeaderText="Supplier Code" />


                                <asp:BoundField
                                    DataField="CompanyName"
                                    HeaderText="Company Name" />


                                <asp:BoundField
                                    DataField="ContactPerson"
                                    HeaderText="Contact Person" />


                                <asp:BoundField
                                    DataField="Phone"
                                    HeaderText="Phone" />


                               
                                <asp:TemplateField
                                    HeaderText="Location">

                                    <ItemTemplate>

                                        <%# GetLocation(
                                            Eval("City"),
                                            Eval("State")
                                        ) %>

                                    </ItemTemplate>

                                </asp:TemplateField>


                              
                                <asp:TemplateField
                                    HeaderText="Status">

                                    <ItemTemplate>

                                        <div class="d-flex align-items-center gap-2">

                                            <asp:Label
                                                ID="lblStatus"
                                                runat="server"
                                                CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-label-success" : "badge bg-label-secondary" %>'
                                                Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>'>
                                            </asp:Label>


                                            <asp:LinkButton
                                                ID="btnToggleStatus"
                                                runat="server"
                                                CommandName="ToggleStatus"
                                                CommandArgument='<%# Eval("SupplierID") %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-sm btn-outline-danger supplier-action-btn" : "btn btn-sm btn-outline-success supplier-action-btn" %>'
                                                Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("IsActive")) ? "return confirm(\"Are you sure you want to deactivate this supplier?\");" : "return confirm(\"Activate this supplier?\");" %>'>
                                            </asp:LinkButton>

                                        </div>

                                    </ItemTemplate>

                                </asp:TemplateField>


                               
                                <asp:TemplateField
                                    HeaderText="Actions">

                                    <ItemTemplate>

                                   
                                        <asp:LinkButton
                                            ID="btnView"
                                            runat="server"
                                            CommandName="ViewSupplier"
                                            CommandArgument='<%# Eval("SupplierID") %>'
                                            CssClass="btn btn-sm btn-outline-secondary me-1"
                                            ToolTip="View Supplier">

                                            <i class="bx bx-show"></i>

                                        </asp:LinkButton>


                                        <!-- EDIT -->
                                        <asp:LinkButton
                                            ID="btnEdit"
                                            runat="server"
                                            CommandName="EditSupplier"
                                            CommandArgument='<%# Eval("SupplierID") %>'
                                            CssClass="btn btn-sm btn-outline-primary"
                                            ToolTip="Edit Supplier">

                                            <i class="bx bx-edit"></i>

                                        </asp:LinkButton>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>


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

                    <strong class="me-auto font-monospace">
                        System Alert
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

</div>


</asp:Content>
