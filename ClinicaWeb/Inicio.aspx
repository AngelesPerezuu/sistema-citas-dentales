<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/MasterBootStrap.Master" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="ClinicaWeb.Inicio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.4/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="dashboard">
        
        <!-- Encabezado -->
        <div class="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-4">
            <div>
                <h2 class="titulo-dental mb-2">🦷 Dashboard clínico</h2>
                <p class="lead text-muted mb-0">Resumen operativo actualizado desde la base de datos.</p>
            </div>
            <span class="badge rounded-pill text-bg-light border px-3 py-2">
                Actualizado: <asp:Literal ID="litActualizado" runat="server" />
            </span>
        </div>

        <!-- Panel de Error -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-warning border-0 shadow-sm">
            <strong>No se pudieron cargar los datos del dashboard.</strong>
            <asp:Literal ID="litError" runat="server" />
        </asp:Panel>

        <!-- 4 Tarjetas Indicadoras -->
        <div class="row g-3 mb-4">
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-azul h-100">
                    <div class="card-body">
                        <span>Pacientes registrados</span>
                        <strong><asp:Literal ID="litPacientes" runat="server" /></strong>
                        <small>Total en el sistema</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-celeste h-100">
                    <div class="card-body">
                        <span>Citas de hoy</span>
                        <strong><asp:Literal ID="litCitasHoy" runat="server" /></strong>
                        <small>Agenda diaria</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-verde h-100">
                    <div class="card-body">
                        <span>Atenciones finalizadas</span>
                        <strong><asp:Literal ID="litFinalizadas" runat="server" /></strong>
                        <small>Histórico acumulado</small>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-xl-3">
                <div class="card indicador indicador-naranja h-100">
                    <div class="card-body">
                        <span>Ingresos realizados</span>
                        <strong>S/ <asp:Literal ID="litIngresos" runat="server" /></strong>
                        <small>Solo citas finalizadas</small>
                    </div>
                </div>
            </div>
        </div>

        <!-- Gráficos -->
        <div class="row g-4">
            <div class="col-lg-7">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="fw-bold mb-1">Citas por estado</h5>
                        <p class="text-muted small">Distribución de toda la agenda clínica.</p>
                        <div class="chart-box">
                            <canvas id="chartEstados"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-5">
                <div class="card tarjeta-dental border-0 shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="fw-bold mb-1">Servicios más solicitados</h5>
                        <p class="text-muted small">Principales tratamientos registrados.</p>
                        <div class="chart-box">
                            <canvas id="chartServicios"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tabla de Próximas Citas -->
            <div class="col-12">
                <div class="card tarjeta-dental border-0 shadow-sm">
                    <div class="card-body p-0">
                        <!-- Cabecera de la tabla con el botón de Excel -->
                        <div class="p-3 border-bottom d-flex justify-content-between align-items-center">
                            <div>
                                <h5 class="fw-bold mb-1">Próximas citas</h5>
                                <p class="text-muted small mb-0">Agenda pendiente o confirmada; si no hay futuras, se muestran las más recientes.</p>
                            </div>
                            <asp:Button ID="btnDescargarCitas" runat="server" Text="⬇ Descargar Excel" CssClass="btn btn-success btn-sm fw-bold" OnClick="btnDescargarCitas_Click" />
                        </div>
                        
                        <!-- GridView -->
                        <div class="table-responsive">
                            <asp:GridView ID="gvProximasCitas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0" GridLines="None" EmptyDataText="No hay citas programadas.">
                                <Columns>
                                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" />
                                    <asp:BoundField DataField="Hora" HeaderText="Hora" />
                                    <asp:BoundField DataField="Paciente" HeaderText="Paciente" />
                                    <asp:BoundField DataField="Dentista" HeaderText="Dentista" />
                                    <asp:BoundField DataField="Servicio" HeaderText="Servicio" />
                                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts de Gráficos -->
    <script>
        const estadoData = <asp:Literal ID="litEstadosJson" runat="server" />;
        const servicioData = <asp:Literal ID="litServiciosJson" runat="server" />;
        const colores = ['#2563eb', '#0ea5e9', '#f59e0b', '#10b981'];

        new Chart(document.getElementById('chartEstados'), {
            type: 'doughnut',
            data: {
                labels: estadoData.labels,
                datasets: [{
                    data: estadoData.values,
                    backgroundColor: colores,
                    borderWidth: 0
                }]
            },
            options: {
                plugins: { legend: { position: 'bottom' } },
                cutout: '64%'
            }
        });

        new Chart(document.getElementById('chartServicios'), {
            type: 'bar',
            data: {
                labels: servicioData.labels,
                datasets: [{
                    data: servicioData.values,
                    backgroundColor: '#2563eb',
                    borderRadius: 7,
                    maxBarThickness: 34
                }]
            },
            options: {
                indexAxis: 'y',
                plugins: { legend: { display: false } },
                scales: {
                    x: { beginAtZero: true, ticks: { precision: 0 } },
                    y: { grid: { display: false } }
                }
            }
        });
    </script>
</asp:Content>