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
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-xxl flex-grow-1 container-p-y">


        <div class="row mb-6 gy-6">

            <%-- Product List --%>
            <div id="divGrid" runat="server" class="col-lg-12">
                <div class="card">
                    <div class="row mx-2 px-4">
                        <div class="row align-items-center justify-content-between g-3">
                            <div class="col-auto">
                                <h5 class="mb-0">Supplier List</h5>
                            </div>
                            <div class="col-auto d-flex align-items-center gap-2">
                                <div class="input-group">
                                    <span class="input-group-text"><i class="icon-base bx bx-search"></i></span>
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                                </div>
                                <asp:LinkButton ID="btnAddSupplier" runat="server" CssClass="btn btn-primary text-nowrap" OnClick="btnAddSupplier_Click">Add Suppliers</asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="card-body">
                        <div class="table-responsive text-nowrap">

                            <asp:GridView ID="gvSuppliers" runat="server" AutoGenerateColumns="false" CssClass="table table-sm table-hover" DataKeyNames="SupplierID" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSuppliers_PageIndexChanging" OnRowCommand="gvSuppliers_RowCommand1">

                                <PagerSettings
                                    Mode="NextPreviousFirstLast"
                                    PreviousPageText="◀ Prev"
                                    NextPageText="Next ▶"
                                    FirstPageText="⏮ First"
                                    LastPageText="Last ⏭" />
                                <PagerStyle CssClass="pagination-area" HorizontalAlign="Center" />

                                <HeaderStyle CssClass="supplierHeader" BackColor="#5F61E6" Font-Bold="True" ForeColor="#FFFFFF" Height="40px" HorizontalAlign="Left" />

                                <%-- Alternating Row Colors for Readability --%>
                                <RowStyle BackColor="#F9F9F9" Height="35px" />
                                <AlternatingRowStyle BackColor="White" Height="35px" />


                                <Columns>
                                    <%--<asp:BoundField DataField="SupplierID" HeaderText="Supplier ID" />--%>

                                    <%-- For counter --%>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate><%# Container.DataItemIndex + 1 %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--// For counter --%>

                                    <asp:BoundField DataField="CompanyName" HeaderText="Company Name" />
                                    <asp:BoundField DataField="Phone" HeaderText="Phone" />
                                    <asp:BoundField DataField="Address" HeaderText="Address" />
                                    <%--<asp:TemplateField HeaderText="Status">
                                    <ItemTemplate> <span class='<%# Convert.ToBoolean(Eval("IsActive"))? "badge bg-success": "badge bg-danger" %>'> "Active" : "Inactive" %> </span> </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>

                                            <asp:LinkButton ID="btnEdit"
                                                runat="server"
                                                CssClass="btn btn-sm btn-warning me-1"
                                                Text="Edit"
                                                CommandName="EditSupplier"
                                                CommandArgument='<%# Eval("SupplierID") %>' />

                                            <asp:LinkButton ID="btnToggleStatus"
                                                runat="server"
                                                CommandName="ToggleStatus"
                                                CommandArgument='<%# Eval("SupplierID") %>'
                                                OnClientClick="return confirm('Change supplier status?');"
                                                Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-sm btn-success" : "btn btn-sm btn-danger" %>' />

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <%-- Side Form --%>
            <div id="divForm" runat="server" class="col-lg-4" visible="false">

                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Supplier Details</h5>
                    </div>

                    <div class="card-body">
                        <asp:HiddenField ID="hfSupplierID" runat="server" />

                        <div class="mb-3">
                            <label>Company Name</label>
                            <asp:TextBox ID="txtCompanyName"
                                runat="server"
                                CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label>Phone</label>
                            <asp:TextBox ID="txtPhone"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Enter 10-digit mobile number"
                                MaxLength="10" />
                        </div>


                        <asp:Button ID="btnSaveSupplier" runat="server" Text="Save Supplier" CssClass="btn btn-primary" OnClick="btnSaveSupplier_Click" />
                        <asp:Button ID="btnCancelSupplier" runat="server" Text="Cancel" CssClass="btn btn-gray" OnClick="btnCancelSupplier_Click" />
                        <asp:Button ID="btnResetData" runat="server" Text="Reset" CssClass="btn btn-danger" OnClick="btnResetData_Click" />
                    </div>
                </div>

            </div>

        </div>

        <%-- Toast Message --%>
        <div class="toast-container position-fixed top-0 end-0 p-4" style="z-index: 9999;">
            <div id="liveToast" runat="server" class="toast shadow-lg border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-header bg-transparent border-bottom">
                    <i class="bx bx-bell me-2 text-primary"></i>
                    <strong class="me-auto font-monospace">System Alert</strong>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body fw-bold">
                    <asp:Label ID="lblToast" runat="server"></asp:Label>
                </div>
            </div>
        </div>

    </div>



</asp:Content>

