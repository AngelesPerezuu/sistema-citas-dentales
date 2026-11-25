using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ClinicaWeb
{
    public partial class Formulario2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Este proyecto no registra jQuery para validación unobtrusive; se usan validadores Web Forms clásicos.
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (Session["User"] == null) { Response.Redirect("Login.aspx"); return; }
            if (!IsPostBack) CargarUbigeos();
        }

        private string CadenaConexion { get { return ConfigurationManager.ConnectionStrings["ClinicaDentalDb"].ConnectionString; } }

        private void CargarUbigeos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(CadenaConexion))
                using (SqlDataAdapter adaptador = new SqlDataAdapter("SELECT Cod_ubigeo, Dis_ubigeo + ' - ' + Dep_ubigeo AS Nombre FROM TB_Ubigeo ORDER BY Dep_ubigeo, Dis_ubigeo", conexion))
                {
                    DataTable datos = new DataTable(); adaptador.Fill(datos);
                    ddlUbigeo.DataSource = datos; ddlUbigeo.DataTextField = "Nombre"; ddlUbigeo.DataValueField = "Cod_ubigeo"; ddlUbigeo.DataBind();
                    ddlUbigeo.Items.Insert(0, new ListItem("Selecciona una ubicación", ""));
                }
            }
            catch (Exception ex) { MostrarMensaje("danger", "No se pudo cargar la lista de ubicaciones: " + Server.HtmlEncode(ex.Message)); }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            DateTime fechaNacimiento;
            if (!DateTime.TryParse(txtFechaNacimiento.Text, out fechaNacimiento) || fechaNacimiento.Date > DateTime.Today) { MostrarMensaje("danger", "La fecha de nacimiento no es válida."); return; }
            try
            {
                using (SqlConnection conexion = new SqlConnection(CadenaConexion))
                {
                    conexion.Open();
                    string idPaciente;
                    using (SqlCommand id = new SqlCommand("SELECT 'PAC' + RIGHT('000' + CAST(ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(ID_paciente, 4, 10))), 0) + 1 AS varchar(10)), 3) FROM TB_Paciente", conexion)) idPaciente = Convert.ToString(id.ExecuteScalar());
                    const string sql = @"INSERT INTO TB_Paciente (ID_paciente, Nom_paciente, Apt_paciente, Apm_paciente, Nac_paciente, Gen_paciente, Civ_paciente, Tel_paciente, Cor_paciente, Dir_paciente, Pai_paciente, His_paciente, Ale_paciente, DNI_paciente, Est_seguro, Cod_ubigeo) VALUES (@id, @nom, @apt, @apm, @nac, @gen, @civ, @tel, @cor, @dir, @pai, @his, @ale, @dni, @seg, @ubi)";
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", idPaciente); comando.Parameters.AddWithValue("@nom", txtNombres.Text.Trim()); comando.Parameters.AddWithValue("@apt", txtApellidoPaterno.Text.Trim()); comando.Parameters.AddWithValue("@apm", txtApellidoMaterno.Text.Trim()); comando.Parameters.AddWithValue("@nac", fechaNacimiento.Date); comando.Parameters.AddWithValue("@gen", ddlGenero.SelectedValue); comando.Parameters.AddWithValue("@civ", ddlEstadoCivil.SelectedValue); comando.Parameters.AddWithValue("@tel", txtTelefono.Text.Trim()); comando.Parameters.AddWithValue("@cor", ValorOBaseDeDatos(txtCorreo.Text)); comando.Parameters.AddWithValue("@dir", ValorOBaseDeDatos(txtDireccion.Text)); comando.Parameters.AddWithValue("@pai", ValorOBaseDeDatos(txtPais.Text)); comando.Parameters.AddWithValue("@his", ValorOBaseDeDatos(txtHistoria.Text)); comando.Parameters.AddWithValue("@ale", txtAlergias.Text.Trim()); comando.Parameters.AddWithValue("@dni", txtDni.Text.Trim()); comando.Parameters.AddWithValue("@seg", ddlSeguro.SelectedValue); comando.Parameters.AddWithValue("@ubi", ddlUbigeo.SelectedValue);
                        comando.ExecuteNonQuery();
                    }
                    LimpiarFormulario(); MostrarMensaje("success", "Paciente registrado correctamente con código " + idPaciente + ".");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) MostrarMensaje("danger", "El DNI o correo ingresado ya está registrado.");
                else MostrarMensaje("danger", "No se pudo registrar el paciente: " + Server.HtmlEncode(ex.Message));
            }
            catch (Exception ex) { MostrarMensaje("danger", "No se pudo registrar el paciente: " + Server.HtmlEncode(ex.Message)); }
        }
        protected void btnLimpiar_Click(object sender, EventArgs e) { LimpiarFormulario(); }
        private object ValorOBaseDeDatos(string valor) { return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim(); }
        private void LimpiarFormulario() { txtNombres.Text = txtApellidoPaterno.Text = txtApellidoMaterno.Text = txtDni.Text = txtFechaNacimiento.Text = txtTelefono.Text = txtCorreo.Text = txtDireccion.Text = txtHistoria.Text = txtAlergias.Text = string.Empty; txtPais.Text = "Perú"; ddlGenero.SelectedIndex = ddlEstadoCivil.SelectedIndex = ddlSeguro.SelectedIndex = ddlUbigeo.SelectedIndex = 0; }
        private void MostrarMensaje(string tipo, string mensaje) { litMensaje.Text = "<div class='alert alert-" + tipo + " alert-dismissible fade show' role='alert'>" + mensaje + "<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>"; }
    }

}
