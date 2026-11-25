using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Serialization;
using ClosedXML.Excel;
using System.IO;

namespace ClinicaWeb
{
    public partial class Inicio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null) { Response.Redirect("Login.aspx"); return; }
            if (!IsPostBack) CargarDashboard();
        }

        private void CargarDashboard()
        {
            litActualizado.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            litEstadosJson.Text = "{\"labels\":[],\"values\":[]}"; litServiciosJson.Text = "{\"labels\":[],\"values\":[]}";
            try
            {
                string cadena = ConfigurationManager.ConnectionStrings["ClinicaDentalDb"].ConnectionString;
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                    litPacientes.Text = ObtenerEntero(conexion, "SELECT COUNT(*) FROM TB_Paciente").ToString("N0");
                    litCitasHoy.Text = ObtenerEntero(conexion, "SELECT COUNT(*) FROM TB_Cita WHERE Fec_cita = CAST(GETDATE() AS date)").ToString("N0");
                    litFinalizadas.Text = ObtenerEntero(conexion, "SELECT COUNT(*) FROM TB_Cita WHERE Est_cita = 4").ToString("N0");
                    litIngresos.Text = ObtenerDecimal(conexion, "SELECT ISNULL(SUM(Pre_cita), 0) FROM TB_Cita WHERE Est_cita = 4").ToString("N2", CultureInfo.GetCultureInfo("es-PE"));
                    litEstadosJson.Text = CrearJson(conexion, @"SELECT CASE Est_cita WHEN 1 THEN 'Pendiente' WHEN 2 THEN 'Confirmada' WHEN 3 THEN 'Cancelada' WHEN 4 THEN 'Finalizada' ELSE 'Sin estado' END AS Etiqueta, COUNT(*) AS Total FROM TB_Cita GROUP BY Est_cita ORDER BY Est_cita");
                    litServiciosJson.Text = CrearJson(conexion, @"SELECT TOP 5 S.Nom_servicio AS Etiqueta, COUNT(C.ID_cita) AS Total FROM TB_Cita C INNER JOIN TB_Servicio S ON S.ID_servicio = C.ID_servicio GROUP BY S.Nom_servicio ORDER BY Total DESC, Etiqueta");
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(@"SELECT TOP 6 CONVERT(varchar(10), C.Fec_cita, 103) AS Fecha, CONVERT(varchar(5), C.Hor_cita) AS Hora, P.Nom_paciente + ' ' + P.Apt_paciente AS Paciente, D.Nom_dentista + ' ' + D.Apt_dentista AS Dentista, S.Nom_servicio AS Servicio, CASE C.Est_cita WHEN 1 THEN 'Pendiente' WHEN 2 THEN 'Confirmada' WHEN 3 THEN 'Cancelada' WHEN 4 THEN 'Finalizada' END AS Estado FROM TB_Cita C INNER JOIN TB_Paciente P ON P.ID_paciente = C.ID_paciente INNER JOIN TB_Dentista D ON D.ID_dentista = C.ID_dentista INNER JOIN TB_Servicio S ON S.ID_servicio = C.ID_servicio WHERE C.Est_cita IN (1,2) ORDER BY CASE WHEN C.Fec_cita >= CAST(GETDATE() AS date) THEN 0 ELSE 1 END, CASE WHEN C.Fec_cita >= CAST(GETDATE() AS date) THEN C.Fec_cita END, CASE WHEN C.Fec_cita < CAST(GETDATE() AS date) THEN C.Fec_cita END DESC, C.Hor_cita", conexion))
                    { DataTable citas = new DataTable(); adaptador.Fill(citas); gvProximasCitas.DataSource = citas; gvProximasCitas.DataBind(); }
                }
            }
            catch (Exception ex) { pnlError.Visible = true; litError.Text = " Verifica la instancia de SQL Server y la ruta de Web.config. Detalle: " + Server.HtmlEncode(ex.Message); }
        }
        private int ObtenerEntero(SqlConnection conexion, string sql) { using (SqlCommand comando = new SqlCommand(sql, conexion)) return Convert.ToInt32(comando.ExecuteScalar()); }
        private decimal ObtenerDecimal(SqlConnection conexion, string sql) { using (SqlCommand comando = new SqlCommand(sql, conexion)) return Convert.ToDecimal(comando.ExecuteScalar()); }
        private string CrearJson(SqlConnection conexion, string sql)
        {
            var etiquetas = new List<string>(); var valores = new List<int>();
            using (SqlCommand comando = new SqlCommand(sql, conexion)) using (SqlDataReader lector = comando.ExecuteReader()) { while (lector.Read()) { etiquetas.Add(lector["Etiqueta"].ToString()); valores.Add(Convert.ToInt32(lector["Total"])); } }
            return new JavaScriptSerializer().Serialize(new { labels = etiquetas, values = valores });
        }
        protected void btnDescargarCitas_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("ProximasCitas");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Hora");
            dt.Columns.Add("Paciente");
            dt.Columns.Add("Dentista");
            dt.Columns.Add("Servicio");
            dt.Columns.Add("Estado");

            foreach (GridViewRow row in gvProximasCitas.Rows)
            {
                dt.Rows.Add(
                    Server.HtmlDecode(row.Cells[0].Text),
                    Server.HtmlDecode(row.Cells[1].Text),
                    Server.HtmlDecode(row.Cells[2].Text),
                    Server.HtmlDecode(row.Cells[3].Text),
                    Server.HtmlDecode(row.Cells[4].Text),
                    Server.HtmlDecode(row.Cells[5].Text)
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Reporte_ProximasCitas.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}
