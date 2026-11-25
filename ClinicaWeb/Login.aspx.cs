using System;
using System.Configuration; 
using System.Web.Security;

namespace ClinicaWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuarioCorrecto = ConfigurationManager.AppSettings["UsuarioAdmin"];
            string passwordCorrecta = ConfigurationManager.AppSettings["PasswordAdmin"];

            string usuarioIngresado = txtUsuario.Text.Trim();
            string passwordIngresada = txtPassword.Text.Trim();

            if (usuarioIngresado == usuarioCorrecto && passwordIngresada == passwordCorrecta)
            {
                Session["User"] = usuarioIngresado;
                Session.Timeout = 20;
                FormsAuthentication.SetAuthCookie(usuarioIngresado, false);
                // Evitar caching para proteger contenido después de logout
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Redirect("Inicio.aspx");
            }
            else
            {
                // Muestra un mensaje de error en la página
                litError.Text = "<div class='alert alert-danger mb-0'>Usuario o contraseña incorrectos</div>";
            }
        }
    }
}
