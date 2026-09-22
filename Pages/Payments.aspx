<%@ Page Title="Payments" Language="C#" MasterPageFile="~/landing.master"
    AutoEventWireup="true" CodeFile="Payments.aspx.cs"
    Inherits="Pages_Payments" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-xxl flex-grow-1 container-p-y">

        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">

            <div>
                <h4 class="fw-bold mb-1">Payments</h4>

                <p class="text-muted mb-0">
                    Record payments received against sales invoices.
                </p>
            </div>

            <a href="PaymentHistory.aspx"
               class="btn btn-outline-primary">
                <i class="bx bx-history me-1"></i>
                Payment History
            </a>

        </div>


        <!-- Message -->
        <div id="divMessage"
             runat="server"
             class="alert d-none">

            <asp:Label ID="lblMessage"
                runat="server">
            </asp:Label>

        </div>


        <!-- Payment Form -->
        <div class="card">

            <div class="card-header">
                <h5 class="mb-0">Record Payment</h5>
            </div>


            <div class="card-body">

                <!-- Invoice -->
                <div class="row g-3">

                    <div class="col-md-6">

                        <label class="form-label">
                            Sales Invoice
                            <span class="text-danger">*</span>
                        </label>

                        <asp:DropDownList ID="ddlInvoice"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlInvoice_SelectedIndexChanged">

                        </asp:DropDownList>

                    </div>

                </div>


                <!-- Invoice Information -->
                <div class="row g-3 mt-1">

                    <div class="col-md-3">

                        <label class="form-label">
                            Customer
                        </label>

                        <asp:TextBox
                            ID="txtCustomer"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3">

                        <label class="form-label">
                            Invoice Total
                        </label>

                        <asp:TextBox
                            ID="txtInvoiceTotal"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3">

                        <label class="form-label">
                            Already Paid
                        </label>

                        <asp:TextBox
                            ID="txtAlreadyPaid"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3">

                        <label class="form-label">
                            Outstanding
                        </label>

                        <asp:TextBox
                            ID="txtOutstanding"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>

                </div>


                <hr class="my-4" />


                <!-- Payment Details -->
                <div class="row g-3">

                    <div class="col-md-4">

                        <label class="form-label">
                            Payment Date
                            <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtPaymentDate"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Date">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-4">

                        <label class="form-label">
                            Amount Paid
                            <span class="text-danger">*</span>
                        </label>

                        <asp:TextBox
                            ID="txtAmountPaid"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Enter payment amount">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-4">

                        <label class="form-label">
                            Payment Mode
                        </label>

                        <asp:DropDownList
                            ID="ddlPaymentMode"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem Text="Select Payment Mode"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem Text="Cash"
                                Value="Cash">
                            </asp:ListItem>

                            <asp:ListItem Text="UPI"
                                Value="UPI">
                            </asp:ListItem>

                            <asp:ListItem Text="Bank Transfer"
                                Value="Bank Transfer">
                            </asp:ListItem>

                            <asp:ListItem Text="Card"
                                Value="Card">
                            </asp:ListItem>

                            <asp:ListItem Text="Cheque"
                                Value="Cheque">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <div class="col-md-6">

                        <label class="form-label">
                            Reference Number
                        </label>

                        <asp:TextBox
                            ID="txtReferenceNumber"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Transaction / cheque reference">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-6">

                        <label class="form-label">
                            Remarks
                        </label>

                        <asp:TextBox
                            ID="txtRemarks"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Optional remarks">
                        </asp:TextBox>

                    </div>

                </div>


                <!-- Buttons -->
                <div class="mt-4">

                    <asp:Button
                        ID="btnSavePayment"
                        runat="server"
                        Text="Save Payment"
                        CssClass="btn btn-primary me-2" OnClick="btnSavePayment_Click" />

                    <asp:Button
                        ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="btn btn-outline-secondary"
                        CausesValidation="false" OnClick="btnClear_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:Content>