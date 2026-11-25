using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ClinicaWeb
{
    public partial class MasterBootStrap : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null || !Request.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            // Renueva explícitamente la expiración de la cookie en cada petición autenticada.
            FormsAuthentication.SetAuthCookie(Session["User"].ToString(), false);

            if (!IsPostBack)
            {
                lblHola.Text = "Hola, " + Session["User"].ToString();
            }
        }
    }
}
