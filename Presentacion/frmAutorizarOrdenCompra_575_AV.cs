using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmAutorizarOrdenCompra_575_AV : Form, IObservadorIdioma
    {
        private OrdenCompraBLL_575_AV ordenCompraBLL = new OrdenCompraBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        public frmAutorizarOrdenCompra_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPeligro(btnRechazar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmAutorizarOrdenCompra_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.AutorizacionesAbierta);
                CargarPendientes();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("AUT_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("AUT_TITULO");
            gbPendientes.Text = gestorIdioma.Traducir("AUT_GB_PENDIENTES");
            gbDetalle.Text = gestorIdioma.Traducir("AUT_GB_DETALLE");
            lblMotivo.Text = gestorIdioma.Traducir("AUT_LBL_MOTIVO");
            btnAutorizar.Text = gestorIdioma.Traducir("AUT_BTN_AUTORIZAR");
            btnRechazar.Text = gestorIdioma.Traducir("AUT_BTN_RECHAZAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            MostrarTotal(OrdenSeleccionada());
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo(dgvOrdenes, "nro_orden_compra", "AUT_COL_NRO");
            Titulo(dgvOrdenes, "razon_social", "CMP_COL_PROVEEDOR");
            Titulo(dgvOrdenes, "fecha_emision", "AUT_COL_FECHA");
            Titulo(dgvOrdenes, "usuario_emisor", "AUT_COL_EMISOR");
            Titulo(dgvOrdenes, "cantidad_renglones", "AUT_COL_RENGLONES");
            Titulo(dgvOrdenes, "total", "AUT_COL_TOTAL");

            Titulo(dgvDetalle, "codigo_articulo", "ART_COL_CODIGO");
            Titulo(dgvDetalle, "descripcion_articulo", "ART_COL_DESCRIPCION");
            Titulo(dgvDetalle, "cantidad", "VTA_COL_CANTIDAD");
            Titulo(dgvDetalle, "abreviatura_unidad", "ART_COL_UNIDAD");
            Titulo(dgvDetalle, "precio_unitario", "ORD_COL_PRECIO_UNITARIO");
            Titulo(dgvDetalle, "subtotal", "ORD_COL_SUBTOTAL");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Ordenes pendientes

        private void CargarPendientes()
        {
            List<OrdenCompraBE_575_AV> ordenes = ordenCompraBLL.ListarPendientes();

            dgvOrdenes.DataSource = ordenes;
            EstiloConectAR_575_AV.MostrarColumnas(dgvOrdenes,
                "nro_orden_compra", "razon_social", "fecha_emision", "usuario_emisor", "cantidad_renglones", "total");
            EstiloConectAR_575_AV.FormatoNumero(dgvOrdenes, "total");
            EstiloConectAR_575_AV.ColumnaElastica(dgvOrdenes, "razon_social");
            if (dgvOrdenes.Columns["fecha_emision"] != null)
            {
                dgvOrdenes.Columns["fecha_emision"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
            TraducirColumnas();

            if (ordenes.Count == 0)
            {
                Avisar(() => gestorIdioma.Traducir("AUT_MSG_SIN_PENDIENTES"), false);
                MostrarDetalle(null);
            }
        }

        private void dgvOrdenes_CurrentCellChanged(object sender, EventArgs e)
        {
            MostrarDetalle(OrdenSeleccionada());
        }

        private void MostrarDetalle(OrdenCompraBE_575_AV orden)
        {
            dgvDetalle.DataSource = orden == null ? new List<DetalleOrdenCompraBE_575_AV>() : orden.detalle;
            EstiloConectAR_575_AV.MostrarColumnas(dgvDetalle,
                "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.FormatoNumero(dgvDetalle, "cantidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.ColumnaElastica(dgvDetalle, "descripcion_articulo");
            TraducirColumnas();

            MostrarTotal(orden);
            txtMotivo.Text = string.Empty;
            EstiloConectAR_575_AV.EstadoBoton(btnAutorizar, orden != null);
            btnRechazar.Enabled = orden != null;
        }

        private void MostrarTotal(OrdenCompraBE_575_AV orden)
        {
            decimal total = orden == null ? 0 : orden.total;
            lblTotal.Text = string.Format(gestorIdioma.Traducir("ORD_LBL_TOTAL"), total.ToString("N2", CultureInfo.CurrentCulture));
        }

        #endregion

        #region Autorizar y rechazar

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            OrdenCompraBE_575_AV orden = OrdenSeleccionada();
            if (orden == null)
            {
                Avisar(() => gestorIdioma.Traducir("AUT_MSG_ELEGIR"), true);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                string.Format(gestorIdioma.Traducir("AUT_CONFIRMAR"), orden.nro_orden_compra),
                gestorIdioma.Traducir("AUT_TITULO"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes)
            {
                Avisar(() => gestorIdioma.Traducir("AUT_MSG_CANCELADA"), false);
                return;
            }

            Ejecutar(() => ordenCompraBLL.Autorizar(orden, UsuarioActual()),
                () => gestorIdioma.Traducir("AUT_MSG_AUTORIZADA"));
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            OrdenCompraBE_575_AV orden = OrdenSeleccionada();
            if (orden == null)
            {
                Avisar(() => gestorIdioma.Traducir("AUT_MSG_ELEGIR"), true);
                return;
            }

            string motivo = txtMotivo.Text;
            Ejecutar(() => ordenCompraBLL.Rechazar(orden, motivo, UsuarioActual()),
                () => string.Format(gestorIdioma.Traducir("AUT_MSG_RECHAZADA"), orden.nro_orden_compra));
        }

        private void Ejecutar(Action accion, Func<string> mensajeOk)
        {
            try
            {
                accion();
                CargarPendientes();
                Avisar(mensajeOk, false);
            }
            catch (InvalidOperationException ex)
            {
                Avisar(ex.Message, true);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Apoyo

        private OrdenCompraBE_575_AV OrdenSeleccionada()
        {
            return dgvOrdenes.CurrentRow == null ? null : dgvOrdenes.CurrentRow.DataBoundItem as OrdenCompraBE_575_AV;
        }

        private Func<string> avisoTraducible;

        private void Avisar(string mensaje, bool esError)
        {
            avisoTraducible = null;
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = esError ? EstiloConectAR_575_AV.Peligro : EstiloConectAR_575_AV.NavySuave;
        }

        private void Avisar(Func<string> mensaje, bool esError)
        {
            Avisar(mensaje(), esError);
            avisoTraducible = mensaje;
        }

        private static string UsuarioActual()
        {
            return SessionManager.Logged() ? SessionManager.GetInstance.UsuarioActual().user : "sin sesion";
        }

        #endregion
    }
}
