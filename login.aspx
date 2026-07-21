<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>


<!DOCTYPE html>
<html lang="en" class="light-style customizer-hide" dir="ltr" data-theme="theme-default">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title>Login - Stock Management System</title>

    <link rel="stylesheet" href="~/assets/vendor/fonts/boxicons.css" runat="server" />
    <link rel="stylesheet" href="~/assets/vendor/css/core.css" class="template-customizer-core-css" runat="server" />
    <link rel="stylesheet" href="~/assets/vendor/css/theme-default.css" class="template-customizer-theme-css" runat="server" />
    <link rel="stylesheet" href="~/assets/css/demo.css" runat="server" />
    <link rel="stylesheet" href="~/assets/vendor/css/pages/page-auth.css" runat="server" />
</head>
<body>
    <div class="container-xxl">
        <div class="authentication-wrapper authentication-basic container-p-y">
            <div class="authentication-inner">
                
                <div class="card">
                    <div class="card-body">
                        <div class="app-brand justify-content-center">
                            <span class="app-brand-logo demo">
                                <i class="bx bx-package text-primary fs-1"></i>
                            </span>
                            <span class="app-brand-text demo text-body fw-bolder text-capitalize ms-2" style="font-size:1.4rem;">StockMaster</span>
                        </div>
                        
                        <h4 class="mb-2">Welcome!</h4>

                        <form id="form1" runat="server" class="mb-3">
                            
                            <asp:Panel ID="pnlAlert" runat="server" CssClass="alert alert-danger alert-dismissible fade show" Visible="false" role="alert">
                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                            </asp:Panel>

                            <div class="mb-3">
                                <label for="txtUsername" class="form-label">Username</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username" autofocus="true" />
                            </div>

                            <div class="mb-3 form-password-toggle">
                                <label for="txtPassword" class="form-label">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;" />
                            </div>

                           

                            <div class="mb-3">
                                <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary d-grid w-100" onClick="btnLogin_Click" />
                            </div>
                        </form>

                        <p class="text-center">
                            <span>New on our platform?</span>
                            <a href="Register.aspx"><span>Create an account</span></a>
                        </p>
                    </div>
                </div>
                </div>
        </div>
    </div>

   <script src="/assets/vendor/libs/jquery/jquery.js"></script>
<script src="/assets/vendor/libs/popper/popper.js"></script>
<script src="/assets/vendor/js/bootstrap.js"></script>

</body>
</html>
