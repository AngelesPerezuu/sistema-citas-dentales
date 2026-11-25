using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.IO;

namespace ClinicaWeb
{
    public partial class Formulario3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null) { Response.Redirect("Login.aspx"); return; }
            if (!IsPostBack) CargarReportes(null, null);
            {
                CargarReportes(null, null);
                CargarCitasPaginadas(1); // <-- AGREGA ESTA LÍNEA EXACTAMENTE AQUÍ
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            DateTime desde, hasta;
            bool tieneDesde = DateTime.TryParse(txtDesde.Text, out desde);
            bool tieneHasta = DateTime.TryParse(txtHasta.Text, out hasta);
            if ((!string.IsNullOrWhiteSpace(txtDesde.Text) && !tieneDesde) || (!string.IsNullOrWhiteSpace(txtHasta.Text) && !tieneHasta)) { MostrarError("Las fechas ingresadas no son válidas."); return; }
            if (tieneDesde && tieneHasta && desde.Date > hasta.Date) { MostrarError("La fecha inicial no puede ser posterior a la fecha final."); return; }
            CargarReportes(tieneDesde ? (DateTime?)desde.Date : null, tieneHasta ? (DateTime?)hasta.Date : null);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDesde.Text = txtHasta.Text = string.Empty;
            CargarReportes(null, null);
        }

        private void CargarReportes(DateTime? desde, DateTime? hasta)
        {
            litIngresosJson.Text = litEstadosJson.Text = "{\"labels\":[],\"values\":[]}";
            litMensaje.Text = string.Empty;
            litPeriodo.Text = FormatoPeriodo(desde, hasta);
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ClinicaDentalDb"].ConnectionString))
                {
                    conexion.Open();
                    string filtro = CrearFiltro(desde, hasta);
                    litTotalCitas.Text = Escalar(conexion, "SELECT COUNT(*) FROM TB_Cita" + filtro, desde, hasta).ToString("N0");
                    litFinalizadas.Text = Escalar(conexion, "SELECT COUNT(*) FROM TB_Cita WHERE Est_cita = 4" + CondicionExtra(filtro), desde, hasta).ToString("N0");
                    litCanceladas.Text = Escalar(conexion, "SELECT COUNT(*) FROM TB_Cita WHERE Est_cita = 3" + CondicionExtra(filtro), desde, hasta).ToString("N0");
                    litIngresos.Text = EscalarDecimal(conexion, "SELECT ISNULL(SUM(Pre_cita), 0) FROM TB_Cita WHERE Est_cita = 4" + CondicionExtra(filtro), desde, hasta).ToString("N2", CultureInfo.GetCultureInfo("es-PE"));
                    litIngresosJson.Text = CrearJson(conexion, "SELECT CONVERT(char(7), Fec_cita, 120) AS Etiqueta, ISNULL(SUM(Pre_cita), 0) AS Total FROM TB_Cita WHERE Est_cita = 4" + CondicionExtra(filtro) + " GROUP BY CONVERT(char(7), Fec_cita, 120) ORDER BY Etiqueta", desde, hasta);
                    litEstadosJson.Text = CrearJson(conexion, "SELECT CASE Est_cita WHEN 1 THEN 'Pendiente' WHEN 2 THEN 'Confirmada' WHEN 3 THEN 'Cancelada' WHEN 4 THEN 'Finalizada' ELSE 'Sin estado' END AS Etiqueta, COUNT(*) AS Total FROM TB_Cita" + filtro + " GROUP BY Est_cita ORDER BY Est_cita", desde, hasta);
                    Enlazar(conexion, "SELECT S.Nom_servicio AS Servicio, COUNT(C.ID_cita) AS Citas, ISNULL(SUM(CASE WHEN C.Est_cita = 4 THEN C.Pre_cita ELSE 0 END), 0) AS Ingresos FROM TB_Cita C INNER JOIN TB_Servicio S ON S.ID_servicio = C.ID_servicio" + ReemplazarPrefijo(filtro, "C.") + " GROUP BY S.Nom_servicio ORDER BY Citas DESC, Servicio", gvServicios, desde, hasta);
                    Enlazar(conexion, "SELECT D.Nom_dentista + ' ' + D.Apt_dentista AS Dentista, COUNT(C.ID_cita) AS Citas, SUM(CASE WHEN C.Est_cita = 4 THEN 1 ELSE 0 END) AS Finalizadas FROM TB_Cita C INNER JOIN TB_Dentista D ON D.ID_dentista = C.ID_dentista" + ReemplazarPrefijo(filtro, "C.") + " GROUP BY D.Nom_dentista, D.Apt_dentista ORDER BY Citas DESC, Dentista", gvDentistas, desde, hasta);
                }
            }
            catch (Exception ex) { MostrarError("No se pudieron generar los reportes: " + Server.HtmlEncode(ex.Message)); }
        }

        private string CrearFiltro(DateTime? desde, DateTime? hasta)
        {
            if (!desde.HasValue && !hasta.HasValue) return string.Empty;
            return " WHERE " + (desde.HasValue ? "Fec_cita >= @desde" : "1=1") + (hasta.HasValue ? (desde.HasValue ? " AND " : "") + "Fec_cita <= @hasta" : "");
        }
        private string CondicionExtra(string filtro) { return string.IsNullOrEmpty(filtro) ? string.Empty : filtro.Replace(" WHERE ", " AND "); }
        private string ReemplazarPrefijo(string filtro, string prefijo) { return filtro.Replace("Fec_cita", prefijo + "Fec_cita"); }
        private int Escalar(SqlConnection c, string sql, DateTime? d, DateTime? h) { using (SqlCommand x = Comando(c, sql, d, h)) return Convert.ToInt32(x.ExecuteScalar()); }
        private decimal EscalarDecimal(SqlConnection c, string sql, DateTime? d, DateTime? h) { using (SqlCommand x = Comando(c, sql, d, h)) return Convert.ToDecimal(x.ExecuteScalar()); }
        private SqlCommand Comando(SqlConnection c, string sql, DateTime? d, DateTime? h)
        {
            SqlCommand x = new SqlCommand(sql, c);
            if (d.HasValue) x.Parameters.AddWithValue("@desde", d.Value);
            if (h.HasValue) x.Parameters.AddWithValue("@hasta", h.Value);
            return x;
        }
        private string CrearJson(SqlConnection c, string sql, DateTime? d, DateTime? h)
        {
            var etiquetas = new List<string>(); var valores = new List<decimal>();
            using (SqlCommand x = Comando(c, sql, d, h)) using (SqlDataReader r = x.ExecuteReader()) { while (r.Read()) { etiquetas.Add(r["Etiqueta"].ToString()); valores.Add(Convert.ToDecimal(r["Total"])); } }
            return new JavaScriptSerializer().Serialize(new { labels = etiquetas, values = valores });
        }
        private void Enlazar(SqlConnection c, string sql, GridView grilla, DateTime? d, DateTime? h)
        {
            using (SqlDataAdapter a = new SqlDataAdapter(Comando(c, sql, d, h))) { DataTable t = new DataTable(); a.Fill(t); grilla.DataSource = t; grilla.DataBind(); }
        }
        private string FormatoPeriodo(DateTime? d, DateTime? h)
        {
            if (!d.HasValue && !h.HasValue) return "Histórico completo";
            return (d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "Inicio") + " — " + (h.HasValue ? h.Value.ToString("dd/MM/yyyy") : "Actualidad");
        }
        private void MostrarError(string texto) { litMensaje.Text = "<div class='alert alert-danger border-0 shadow-sm'>" + texto + "</div>"; }

        protected void btnDescargarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("Servicios");
            dt.Columns.Add("Servicio");
            dt.Columns.Add("Citas");
            dt.Columns.Add("Ingresos (S/)");

            foreach (GridViewRow row in gvServicios.Rows)
            {
                dt.Rows.Add(row.Cells[0].Text, row.Cells[1].Text, row.Cells[2].Text);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Reporte_Servicios.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        protected void btnDescargarDentistas_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("Dentistas");
            dt.Columns.Add("Dentista");
            dt.Columns.Add("Citas");
            dt.Columns.Add("Finalizadas");

            foreach (GridViewRow row in gvDentistas.Rows)
            {
                dt.Rows.Add(row.Cells[0].Text, row.Cells[1].Text, row.Cells[2].Text);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Reporte_Dentistas.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        private void CargarCitasPaginadas(int paginaActual)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ClinicaDentalDb"].ConnectionString))
                {
                    conexion.Open();

                    int totalRegistros = 0;
                    using (SqlCommand cmdCount = new SqlCommand("SELECT COUNT(*) FROM TB_Cita", conexion))
                    {
                        totalRegistros = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_ListarCitasPaginado", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageIndex", paginaActual);
                        cmd.Parameters.AddWithValue("@PageSize", gvCitasPaginado.PageSize);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            gvCitasPaginado.VirtualItemCount = totalRegistros;

                            gvCitasPaginado.DataSource = dt;
                            gvCitasPaginado.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo cargar el historial paginado: " + Server.HtmlEncode(ex.Message));
            }
        }
        protected void gvCitasPaginado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCitasPaginado.PageIndex = e.NewPageIndex;
            CargarCitasPaginadas(e.NewPageIndex + 1);
        }
    }
}


