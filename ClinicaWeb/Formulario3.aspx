<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/MasterBootStrap.Master" AutoEventWireup="true" CodeBehind="Formulario3.aspx.cs" Inherits="ClinicaWeb.Formulario3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.4/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="reportes">
        <h2 class="titulo-dental">📊 Reportes y Estadísticas</h2>
        <p class="text-muted">Análisis y balance de rendimiento de la clínica dental.</p>

        <!-- Filtros y Botones -->
        <div class="card border-0 shadow-sm mb-4 filtro-reportes">
            <div class="card-body">
                <div class="row align-items-end g-3">
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Desde</label>
                        <asp:TextBox ID="txtDesde" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Hasta</label>
                        <asp:TextBox ID="txtHasta" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="col-md-6 d-flex gap-2">
                        <asp:Button ID="btnFiltrar" runat="server" Text="Aplicar filtros" CssClass="btn btn-primary" OnClick="btnFiltrar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Ver histórico" CausesValidation="false" CssClass="btn btn-outline-secondary" OnClick="btnLimpiar_Click" />
                        
                        <!-- BOTÓN DE DESCARGA A EXCEL AGREGADO -->
                        <asp:Button ID="btnDescargarExcel" runat="server" Text="⬇ Descargar Excel" CssClass="btn btn-success fw-bold" OnClick="btnDescargarExcel_Click" />
                    </div>
                </div>
                <p class="text-muted small mb-0 mt-3">Periodo mostrado: <asp:Literal ID="litPeriodo" runat="server" /></p>
            </div>
        </div>

        <asp:Literal ID="litMensaje" runat="server" />

        <!-- Tarjetas de Indicadores -->
        <div class="row g-3 mb-4">
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-azul h-100">
                    <div class="card-body">
                        <span>Total de citas</span>
                        <strong><asp:Literal ID="litTotalCitas" runat="server" /></strong>
                        <small>En el periodo seleccionado</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-verde h-100">
                    <div class="card-body">
                        <span>Citas finalizadas</span>
                        <strong><asp:Literal ID="litFinalizadas" runat="server" /></strong>
                        <small>Atenciones completadas</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-naranja h-100">
                    <div class="card-body">
                        <span>Citas canceladas</span>
                        <strong><asp:Literal ID="litCanceladas" runat="server" /></strong>
                        <small>Requieren seguimiento</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-celeste h-100">
                    <div class="card-body">
                        <span>Ingresos realizados</span>
                        <strong>S/ <asp:Literal ID="litIngresos" runat="server" /></strong>
                        <small>Solo atenciones finalizadas</small>
                    </div>
                </div>
            </div>
        </div>

        <!-- Gráficos -->
        <div class="row g-4 mb-4">
            <div class="col-lg-7">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="fw-bold">Ingresos por mes</h5>
                        <p class="text-muted small">Monto de las citas finalizadas.</p>
                        <div class="chart-box">
                            <canvas id="chartIngresos"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-5">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="fw-bold">Estado de las citas</h5>
                        <p class="text-muted small">Distribución en el periodo consultado.</p>
                        <div class="chart-box">
                            <canvas id="chartEstadosReporte"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Tablas de Datos (GridViews) -->
       <div class="row g-4">
            <div class="col-lg-6">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body p-0">
                        <div class="p-3 border-bottom">
                            <h5 class="fw-bold mb-0">Servicios más solicitados</h5>
                        </div>
                        <div class="table-responsive">
                            <asp:GridView ID="gvServicios" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0" GridLines="None" EmptyDataText="No hay datos para el periodo.">
                                <Columns>
                                    <asp:BoundField DataField="Servicio" HeaderText="Servicio" />
                                    <asp:BoundField DataField="Citas" HeaderText="Citas" />
                                    <asp:BoundField DataField="Ingresos" HeaderText="Ingresos (S/)" DataFormatString="{0:N2}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body p-0">
                        <div class="p-3 border-bottom d-flex justify-content-between align-items-center">
                            <h5 class="fw-bold mb-0">Rendimiento por dentista</h5>
                            <asp:Button ID="btnDescargarDentistas" runat="server" Text="⬇ Excel Dentistas" CssClass="btn btn-success btn-sm fw-bold me-2" OnClick="btnDescargarDentistas_Click" />
                        </div>
                        <div class="table-responsive">
                            <asp:GridView ID="gvDentistas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0" GridLines="None" EmptyDataText="No hay datos para el periodo.">
                                <Columns>
                                    <asp:BoundField DataField="Dentista" HeaderText="Dentista" />
                                    <asp:BoundField DataField="Citas" HeaderText="Citas" />
                                    <asp:BoundField DataField="Finalizadas" HeaderText="Finalizadas" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mt-4 mb-4">
            <div class="col-12">
                <div class="card tarjeta-dental border-0 shadow-sm">
                    <div class="card-body p-0">
                        <div class="p-3 border-bottom">
                            <h5 class="fw-bold mb-0">📋 Historial General de Citas</h5>
                            <p class="text-muted small mb-0">Muestra el listado completo de la agenda.</p> 
                        </div>
                        <div class="table-responsive p-3">
                            <asp:GridView ID="gvCitasPaginado" runat="server" AutoGenerateColumns="false" 
                                AllowPaging="true" AllowCustomPaging="true" PageSize="5" OnPageIndexChanging="gvCitasPaginado_PageIndexChanging"
                                CssClass="table table-hover align-middle mb-0" GridLines="None"
                                EmptyDataText="No se encontraron citas registradas.">
                                <Columns>
                                    <asp:BoundField DataField="IdCita" HeaderText="ID Cita" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Hora" HeaderText="Hora" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="DNI" HeaderText="DNI Paciente" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Paciente" HeaderText="Paciente" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Dentista" HeaderText="Dentista" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Servicio" HeaderText="Servicio" HeaderStyle-CssClass="fw-bold text-secondary" />
                                    <asp:BoundField DataField="Estado" HeaderText="Estado" HeaderStyle-CssClass="fw-bold text-secondary" />
                                </Columns>
                                <PagerStyle HorizontalAlign="Center" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts de Chart.js -->
    <script>
        const ingresos = <asp:Literal ID="litIngresosJson" runat="server" />;
        const estados = <asp:Literal ID="litEstadosJson" runat="server" />;

        new Chart(document.getElementById('chartIngresos'), {
            type: 'line',
            data: {
                labels: ingresos.labels,
                datasets: [{
                    label: 'Ingresos (S/)',
                    data: ingresos.values,
                    borderColor: '#2563eb',
                    backgroundColor: 'rgba(37,99,235,.15)',
                    fill: true,
                    tension: .35,
                    pointBackgroundColor: '#2563eb'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: { y: { beginAtZero: true } }
            }
        });

        new Chart(document.getElementById('chartEstadosReporte'), {
            type: 'doughnut',
            data: {
                labels: estados.labels,
                datasets: [{
                    data: estados.values,
                    backgroundColor: ['#f59e0b', '#0ea5e9', '#ef4444', '#10b981'],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '64%',
                plugins: { legend: { position: 'bottom' } }
            }
        });
    </script>
</asp:Content>