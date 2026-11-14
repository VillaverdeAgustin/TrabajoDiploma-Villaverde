using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // A02 - Reporte de compras por proveedor. La pantalla solo pide el reporte a la BLL y lo muestra.
    public partial class frmReporteCompras_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly ReporteComprasBLL_575_AV reporteBLL = new ReporteComprasBLL_575_AV();
        private readonly Font fuenteTotal = new Font(EstiloConectAR_575_AV.FuenteBase, FontStyle.Bold);
        private Func<string> avisoTraducible;
        private ReporteComprasBE_575_AV reporte;

        public frmReporteCompras_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnGenerar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            lblLeyenda.Font = EstiloConectAR_575_AV.FuenteBajada;

            EstiloConectAR_575_AV.FormatoNumero(dgvResumen, "colMontoArs", "colMontoUsd");
            EstiloConectAR_575_AV.FormatoNumero(dgvOrdenes, "colOrdenArs", "colOrdenUsd");
            Alinear(dgvResumen, "colOrdenes", "colRecibido", "colPendientes", "colRechazadas");
            Alinear(dgvOrdenes, "colRenglones", "colOrdenRecibido");
            colFecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            cmbEstado.FormattingEnabled = true;
            cmbEstado.Format += cmbEstado_Format;
            TextoTraducido_575_AV.PrimeraOpcion(cmbProveedor, "COMUN_TODOS");
            dgvResumen.CellFormatting += dgvResumen_CellFormatting;
            dgvOrdenes.CellFormatting += dgvOrdenes_CellFormatting;
            this.FormClosed += (s, a) => fuenteTotal.Dispose();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            CargarFiltros();
            Generar();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("REPC_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("REPC_TITULO");
            gbFiltros.Text = gestorIdioma.Traducir("REPC_GB_FILTROS");
            lblDesde.Text = gestorIdioma.Traducir("COMUN_DESDE");
            lblHasta.Text = gestorIdioma.Traducir("COMUN_HASTA");
            lblProveedor.Text = gestorIdioma.Traducir("REPC_LBL_PROVEEDOR");
            lblEstado.Text = gestorIdioma.Traducir("REPC_LBL_ESTADO");
            btnGenerar.Text = gestorIdioma.Traducir("REPC_BTN_GENERAR");
            gbResumen.Text = gestorIdioma.Traducir("REPC_GB_RESUMEN");
            lblLeyenda.Text = gestorIdioma.Traducir("REPC_LBL_LEYENDA");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            colProveedor.HeaderText = gestorIdioma.Traducir("REPC_COL_PROVEEDOR");
            colOrdenes.HeaderText = gestorIdioma.Traducir("REPC_COL_ORDENES");
            colMontoArs.HeaderText = gestorIdioma.Traducir("REPC_COL_MONTO_ARS");
            colMontoUsd.HeaderText = gestorIdioma.Traducir("REPC_COL_MONTO_USD");
            colRecibido.HeaderText = gestorIdioma.Traducir("REPC_COL_RECIBIDO");
            colPendientes.HeaderText = gestorIdioma.Traducir("REPC_COL_PENDIENTES");
            colRechazadas.HeaderText = gestorIdioma.Traducir("REPC_COL_RECHAZADAS");
            colNro.HeaderText = gestorIdioma.Traducir("REPC_COL_NRO");
            colFecha.HeaderText = gestorIdioma.Traducir("REPC_COL_FECHA");
            colProveedorOrden.HeaderText = gestorIdioma.Traducir("REPC_COL_PROVEEDOR");
            colEstado.HeaderText = gestorIdioma.Traducir("REPC_COL_ESTADO");
            colRenglones.HeaderText = gestorIdioma.Traducir("REPC_COL_RENGLONES");
            colOrdenArs.HeaderText = gestorIdioma.Traducir("REPC_COL_MONTO_ARS");
            colOrdenUsd.HeaderText = gestorIdioma.Traducir("REPC_COL_MONTO_USD");
            colOrdenRecibido.HeaderText = gestorIdioma.Traducir("REPC_COL_RECIBIDO");

            TituloOrdenes();
            TextoTraducido_575_AV.Refrescar(cmbProveedor);
            TextoTraducido_575_AV.Refrescar(cmbEstado);
            dgvResumen.Invalidate();
            dgvOrdenes.Invalidate();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TituloOrdenes()
        {
            ResumenProveedorCompra_575_AV fila = Seleccionada();
            gbOrdenes.Text = fila == null || fila.es_total
                ? gestorIdioma.Traducir("REPC_GB_ORDENES_TODAS")
                : string.Format(gestorIdioma.Traducir("REPC_GB_ORDENES"), fila.razon_social);
        }

        #endregion

        #region Filtros y reporte

        private void CargarFiltros()
        {
            try
            {
                List<ProveedorBE_575_AV> proveedores = new List<ProveedorBE_575_AV> { new ProveedorBE_575_AV() };
                proveedores.AddRange(reporteBLL.ListarProveedores());
                cmbProveedor.DataSource = proveedores;
                cmbProveedor.DisplayMember = "razon_social";

                List<object> estados = new List<object> { new object() };
                estados.AddRange(reporteBLL.ListarEstados().Cast<object>());
                cmbEstado.DataSource = estados;
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void Generar()
        {
            try
            {
                ProveedorBE_575_AV proveedor = cmbProveedor.SelectedIndex > 0 ? cmbProveedor.SelectedItem as ProveedorBE_575_AV : null;
                EstadoOrdenCompra_575_AV? estado = cmbEstado.SelectedIndex > 0 ? (EstadoOrdenCompra_575_AV?)cmbEstado.SelectedItem : null;

                reporte = reporteBLL.GenerarReporte(dtpDesde.Value, dtpHasta.Value,
                    proveedor == null ? (int?)null : proveedor.id_proveedor, estado, UsuarioActual());

                int ordenes = reporte.total.cantidad_ordenes;
                int proveedores = reporte.proveedores.Count;
                dgvResumen.DataSource = ordenes == 0 ? new List<ResumenProveedorCompra_575_AV>() : reporte.filas;
                MostrarOrdenes();

                if (ordenes == 0)
                {
                    Avisar(() => gestorIdioma.Traducir("REPC_MSG_SIN_RESULTADOS"), false);
                }
                else
                {
                    Avisar(() => string.Format(gestorIdioma.Traducir("REPC_MSG_RESULTADO"), ordenes, proveedores), false);
                }
            }
            catch (InvalidOperationException ex)
            {
                Vaciar();
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
            }
            catch (Exception)
            {
                Vaciar();
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void Vaciar()
        {
            reporte = null;
            dgvResumen.DataSource = new List<ResumenProveedorCompra_575_AV>();
            MostrarOrdenes();
        }

        private void MostrarOrdenes()
        {
            ResumenProveedorCompra_575_AV fila = Seleccionada();
            dgvOrdenes.DataSource = fila == null ? new List<OrdenReporteCompra_575_AV>() : fila.ordenes;
            TituloOrdenes();
        }

        // Al cambiar la seleccion, SelectionChanged llega antes de que se actualice CurrentRow: se lee la fila seleccionada
        private ResumenProveedorCompra_575_AV Seleccionada()
        {
            DataGridViewRow fila = dgvResumen.SelectedRows.Count > 0 ? dgvResumen.SelectedRows[0] : dgvResumen.CurrentRow;
            return fila == null ? null : fila.DataBoundItem as ResumenProveedorCompra_575_AV;
        }

        private void btnGenerar_Click(object sender, EventArgs e) => Generar();

        private void dgvResumen_SelectionChanged(object sender, EventArgs e) => MostrarOrdenes();

        #endregion

        #region Formato de grillas y combos

        private void cmbEstado_Format(object sender, ListControlConvertEventArgs e)
        {
            if (cmbEstado.Items.Count > 0 && ReferenceEquals(e.ListItem, cmbEstado.Items[0]))
            {
                e.Value = gestorIdioma.Traducir("COMUN_TODOS");
            }
            else if (e.ListItem is EstadoOrdenCompra_575_AV)
            {
                e.Value = Estado((EstadoOrdenCompra_575_AV)e.ListItem);
            }
        }

        private void dgvResumen_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) { return; }
            ResumenProveedorCompra_575_AV fila = dgvResumen.Rows[e.RowIndex].DataBoundItem as ResumenProveedorCompra_575_AV;
            if (fila == null) { return; }

            string columna = dgvResumen.Columns[e.ColumnIndex].Name;
            if (columna == "colProveedor" && fila.es_total)
            {
                e.Value = gestorIdioma.Traducir("COMUN_TOTAL");
                e.FormattingApplied = true;
            }
            else if (columna == "colRecibido")
            {
                e.Value = Porcentaje(fila.porcentaje_recibido);
                e.FormattingApplied = true;
            }

            if (fila.es_total)
            {
                e.CellStyle.Font = fuenteTotal;
                e.CellStyle.BackColor = EstiloConectAR_575_AV.Borde;
            }
        }

        private void dgvOrdenes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) { return; }
            string columna = dgvOrdenes.Columns[e.ColumnIndex].Name;
            if (columna == "colEstado" && e.Value is EstadoOrdenCompra_575_AV)
            {
                e.Value = Estado((EstadoOrdenCompra_575_AV)e.Value);
                e.FormattingApplied = true;
            }
            else if (columna == "colOrdenRecibido")
            {
                OrdenReporteCompra_575_AV orden = dgvOrdenes.Rows[e.RowIndex].DataBoundItem as OrdenReporteCompra_575_AV;
                e.Value = Porcentaje(orden == null ? null : orden.porcentaje_recibido);
                e.FormattingApplied = true;
            }
        }

        private string Estado(EstadoOrdenCompra_575_AV estado)
        {
            return gestorIdioma.Traducir("OC_EST_" + estado.ToString().ToUpperInvariant());
        }

        private static string Porcentaje(decimal? valor)
        {
            return valor.HasValue ? valor.Value.ToString("N1") + " %" : "—";
        }

        private static void Alinear(DataGridView grilla, params string[] columnas)
        {
            foreach (string columna in columnas)
            {
                grilla.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        #endregion

        #region Apoyo

        private void Avisar(Func<string> mensaje, bool esError)
        {
            avisoTraducible = mensaje;
            lblMensaje.Text = mensaje();
            lblMensaje.ForeColor = esError ? EstiloConectAR_575_AV.Peligro : EstiloConectAR_575_AV.NavySuave;
        }

        private static string UsuarioActual()
        {
            return SessionManager.Logged() ? SessionManager.GetInstance.UsuarioActual().user : "sin sesion";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
