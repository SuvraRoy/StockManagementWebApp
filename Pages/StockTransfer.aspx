<%@ Page Title="Stock Transfer" Language="C#" MasterPageFile="~/landing.master" AutoEventWireup="true" CodeFile="StockTransfer.aspx.cs" Inherits="Pages_StockTransfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<div class="container-xxl flex-grow-1 container-p-y">

    <!-- Message Alert -->
    <div id="divMessage" runat="server" visible="false" class="alert alert-dismissible fade show" role="alert">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>

    <!-- Header Information Card (Includes File Upload) -->
    <div class="card mb-4">
        <div class="card-header">
            <h5 class="mb-0">Transfer Information</h5>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3 mb-3">
                    <label class="form-label">Transfer #</label>
                    <asp:TextBox ID="txtTransferNumber" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label">Transfer Date <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTransferDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label">Source Warehouse <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlFromWarehouse" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFromWarehouse_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label">Destination Warehouse <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlToWarehouse" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label">Remarks</label>
                    <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional notes..."></asp:TextBox>
                </div>
                <!-- FILE UPLOAD REMAINS OUTSIDE THE UPDATE PANEL -->
                <div class="col-md-6 mb-3">
                    <label class="form-label">Supporting Document (PDF, JPG, PNG)</label>
                    <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
                    <small class="text-muted">Max file size: 200KB</small>
                </div>
            </div>
        </div>
    </div>

    <!-- WRAP ONLY THE ITEMS AND ROW BUTTONS INSIDE THE UPDATEPANEL -->
    <asp:UpdatePanel ID="upnlTransferItems" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Transfer Items</h5>
                    <asp:Button ID="btnAddRow" runat="server" Text="+ Add Item" CssClass="btn btn-sm btn-primary" OnClick="btnAddRow_Click" />
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-bordered align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th style="width: 45%;">Product</th>
                                    <th style="width: 20%;">Available Stock</th>
                                    <th style="width: 20%;">Transfer Qty</th>
                                    <th style="width: 15%; text-align: center;">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptItems" runat="server" OnItemCommand="rptItems_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-select"
                                                    AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAvailableStock" runat="server" Text="0" CssClass="fw-bold"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" Text="1"></asp:TextBox>
                                            </td>
                                            <td class="text-center">
                                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRow"
                                                    CommandArgument='<%# Container.ItemIndex %>' CssClass="btn btn-sm btn-outline-danger">
                                                    <i class="bx bx-trash"></i> Delete
                                                </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlFromWarehouse" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <!-- ACTION BUTTONS (OUTSIDE UPDATE PANEL) -->
    <div class="d-flex gap-2">
        <asp:Button ID="btnSave" runat="server" Text="Save Transfer" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary" OnClick="btnCancel_Click" />
    </div>

</div>
</asp:Content>