<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClinicaWeb.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Clínica Dental - Iniciar Sesión</title>
    
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    
    <link href="CSS/ClinicaEstilos.css" rel="stylesheet" type="text/css" />
</head>
<body class="bg-light d-flex align-items-center justify-content-center vh-100">
    
    <form id="form1" runat="server" class="contenedor-login">
        
        <div class="card shadow border-0 rounded-4 p-4 text-center bg-white">
            
            <div class="mb-4">
                <span class="logo-dental">🦷</span>
                <h3 class="fw-bold mt-2 text-primary">Clínica Dental</h3>
                <small class="text-muted d-block">Sistema de Gestión de Citas</small>
            </div>

            <div class="form-floating mb-3 text-start">
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Admin"></asp:TextBox>
                <label for="txtUsuario">Usuario</label>
            </div>

            <div class="form-floating mb-4 text-start">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="12345"></asp:TextBox>
                <label for="txtPassword">Contraseña</label>
            </div>

            <div class="mb-3">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </div>

            <div class="d-grid">
                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar al Sistema" OnClick="btnIngresar_Click" CssClass="btn btn-primary btn-lg fw-bold rounded-3" />
            </div>
            
        </div> </form> <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>