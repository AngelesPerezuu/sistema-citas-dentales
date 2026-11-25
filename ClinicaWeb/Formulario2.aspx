<%@ Page Title="Registros" Language="C#" MasterPageFile="~/MasterBootStrap.Master" AutoEventWireup="true" CodeBehind="Formulario2.aspx.cs" Inherits="ClinicaWeb.Formulario2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="registro-paciente">
        <h2 class="titulo-dental">📋 Registro de Pacientes</h2>
        <p class="text-muted">Ingresa la información clínica y de contacto del nuevo paciente.</p>

        <asp:Literal ID="litMensaje" runat="server" />
        <asp:ValidationSummary ID="vsRegistro" runat="server" CssClass="alert alert-danger" HeaderText="Revisa los siguientes campos:" />

        <div class="card tarjeta-dental border-0 shadow-sm mt-4">
            <div class="card-body p-4">
                <h5 class="fw-bold mb-3">Datos personales</h5>
                <div class="row g-3">
                    <div class="col-md-4"><label class="form-label">Nombres *</label><asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" MaxLength="30" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtNombres" ErrorMessage="Ingresa los nombres." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Apellido paterno *</label><asp:TextBox ID="txtApellidoPaterno" runat="server" CssClass="form-control" MaxLength="25" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellidoPaterno" ErrorMessage="Ingresa el apellido paterno." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Apellido materno *</label><asp:TextBox ID="txtApellidoMaterno" runat="server" CssClass="form-control" MaxLength="25" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellidoMaterno" ErrorMessage="Ingresa el apellido materno." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">DNI *</label><asp:TextBox ID="txtDni" runat="server" CssClass="form-control" MaxLength="8" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtDni" ErrorMessage="Ingresa el DNI." CssClass="text-danger small" Display="Dynamic" /><asp:RegularExpressionValidator runat="server" ControlToValidate="txtDni" ValidationExpression="\d{8}" ErrorMessage="El DNI debe tener 8 dígitos." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Fecha de nacimiento *</label><asp:TextBox ID="txtFechaNacimiento" runat="server" TextMode="Date" CssClass="form-control" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtFechaNacimiento" ErrorMessage="Ingresa la fecha de nacimiento." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Género *</label><asp:DropDownList ID="ddlGenero" runat="server" CssClass="form-select"><asp:ListItem Value="">Selecciona</asp:ListItem><asp:ListItem Value="F">Femenino</asp:ListItem><asp:ListItem Value="M">Masculino</asp:ListItem><asp:ListItem Value="O">Otro</asp:ListItem></asp:DropDownList><asp:RequiredFieldValidator runat="server" ControlToValidate="ddlGenero" ErrorMessage="Selecciona el género." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Estado civil *</label><asp:DropDownList ID="ddlEstadoCivil" runat="server" CssClass="form-select"><asp:ListItem Value="">Selecciona</asp:ListItem><asp:ListItem Value="S">Soltero(a)</asp:ListItem><asp:ListItem Value="C">Casado(a)</asp:ListItem><asp:ListItem Value="D">Divorciado(a)</asp:ListItem><asp:ListItem Value="V">Viudo(a)</asp:ListItem></asp:DropDownList><asp:RequiredFieldValidator runat="server" ControlToValidate="ddlEstadoCivil" ErrorMessage="Selecciona el estado civil." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Seguro *</label><asp:DropDownList ID="ddlSeguro" runat="server" CssClass="form-select"><asp:ListItem Value="">Selecciona</asp:ListItem><asp:ListItem Value="P">Particular</asp:ListItem><asp:ListItem Value="S">Con seguro</asp:ListItem></asp:DropDownList><asp:RequiredFieldValidator runat="server" ControlToValidate="ddlSeguro" ErrorMessage="Selecciona el tipo de seguro." CssClass="text-danger small" Display="Dynamic" /></div>
                </div>
                <hr class="my-4" />
                <h5 class="fw-bold mb-3">Contacto y antecedentes</h5>
                <div class="row g-3">
                    <div class="col-md-4"><label class="form-label">Teléfono *</label><asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="15" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtTelefono" ErrorMessage="Ingresa el teléfono." CssClass="text-danger small" Display="Dynamic" /><asp:RegularExpressionValidator runat="server" ControlToValidate="txtTelefono" ValidationExpression="\d{7,15}" ErrorMessage="El teléfono solo debe contener dígitos." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-4"><label class="form-label">Correo electrónico</label><asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" CssClass="form-control" MaxLength="100" /></div>
                    <div class="col-md-4"><label class="form-label">Ubigeo *</label><asp:DropDownList ID="ddlUbigeo" runat="server" CssClass="form-select" /><asp:RequiredFieldValidator runat="server" ControlToValidate="ddlUbigeo" InitialValue="" ErrorMessage="Selecciona el ubigeo." CssClass="text-danger small" Display="Dynamic" /></div>
                    <div class="col-md-6"><label class="form-label">Dirección</label><asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" MaxLength="100" /></div>
                    <div class="col-md-6"><label class="form-label">País</label><asp:TextBox ID="txtPais" runat="server" CssClass="form-control" MaxLength="20" Text="Perú" /></div>
                    <div class="col-md-6"><label class="form-label">Historia clínica / antecedentes</label><asp:TextBox ID="txtHistoria" runat="server" CssClass="form-control" MaxLength="100" TextMode="MultiLine" Rows="3" /></div>
                    <div class="col-md-6"><label class="form-label">Alergias *</label><asp:TextBox ID="txtAlergias" runat="server" CssClass="form-control" MaxLength="100" TextMode="MultiLine" Rows="3" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtAlergias" ErrorMessage="Indica las alergias o escribe 'Ninguna'." CssClass="text-danger small" Display="Dynamic" /></div>
                </div>
                <div class="d-flex justify-content-end gap-2 mt-4"><asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CausesValidation="false" CssClass="btn btn-outline-secondary" OnClick="btnLimpiar_Click" /><asp:Button ID="btnGuardar" runat="server" Text="Guardar paciente" CssClass="btn btn-primary px-4" OnClick="btnGuardar_Click" /></div>
            </div>
        </div>
    </div>
</asp:Content>
