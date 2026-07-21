<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestConn.aspx.cs" Inherits="TestConn" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Database Connection Test</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />
</head>
<body class="bg-light d-flex align-items-center justify-content-center vh-100">
    <form id="form1" runat="server">
        <div class="card shadow p-4 text-center" style="max-width: 400px; width: 100%;">
            <h5 class="card-title text-secondary mb-3">Database Connection Diagnostic</h5>
            <div class="mb-3">
                <asp:Button ID="btnTest" runat="server" Text="Test Connection Now" 
                            CssClass="btn btn-primary w-100" OnClick="btnTest_Click" />
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" CssClass="fw-bold d-block p-2 rounded"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>