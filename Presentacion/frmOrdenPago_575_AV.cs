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
    public partial class frmOrdenPago_575_AV : Form, IObservadorIdioma
    {
        private CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
        private OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private CarritoBE_575_AV carritoActual;

        public frmOrdenPago_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "carrito");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnActualizar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmOrdenPago_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                CargarCarritos();
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.OrdenPagoAbierta);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("OP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("OP_TITULO");
            gbCarritos.Text = gestorIdioma.Traducir("OP_GB_CARRITOS");
            gbDetalle.Text = gestorIdioma.Traducir("OP_GB_DETALLE");
            lblCliente.Text = gestorIdioma.Traducir("OP_LBL_CLIENTE");
            lblTotalTitulo.Text = gestorIdioma.Traducir("COMUN_TOTAL");
            btnEmitir.Text = gestorIdioma.Traducir("OP_BTN_EMITIR");
            btnActualizar.Text = gestorIdioma.Traducir("COMUN_ACTUALIZAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo(dgvCarritos, "nro_carrito", "OP_COL_CARRITO");
            Titulo(dgvCarritos, "usuario_vendedor", "OP_COL_VENDEDOR");
            Titulo(dgvCarritos, "fecha_apertura", "OP_COL_FECHA");
            Titulo(dgvCarritos, "precio_total", "COMUN_TOTAL");

            Titulo(dgvDetalle, "codigo_articulo", "ART_COL_CODIGO");
            Titulo(dgvDetalle, "descripcion_articulo", "ART_COL_DESCRIPCION");
            Titulo(dgvDetalle, "cantidad", "VTA_COL_CANTIDAD");
            Titulo(dgvDetalle, "abreviatura_unidad", "ART_COL_UNIDAD");
            Titulo(dgvDetalle, "precio_unitario", "ART_COL_PRECIO");
            Titulo(dgvDetalle, "subtotal", "VTA_COL_SUBTOTAL");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Carga

        private void CargarCarritos()
        {
            HashSet<int> conOrden = new HashSet<int>(
                ordenPagoBLL.ListarPendientes().Select(o => o.id_carrito));

            List<CarritoBE_575_AV> carritos = carritoBLL.ListarAbiertos()
                .Where(c => c.estado == EstadoCarrito_575_AV.Confirmado
                         && !conOrden.Contains(c.id_carrito))
                .ToList();

            dgvCarritos.DataSource = carritos;

            EstiloConectAR_575_AV.MostrarColumnas(dgvCarritos,
                "nro_carrito", "usuario_vendedor", "fecha_apertura", "precio_total");
            EstiloConectAR_575_AV.FormatoNumero(dgvCarritos, "precio_total");
            EstiloConectAR_575_AV.ColumnaElastica(dgvCarritos, "usuario_vendedor");
            TraducirColumnas();

            if (carritos.Count == 0)
            {
                carritoActual = null;
                MostrarCarrito(null);
                Avisar(() => gestorIdioma.Traducir("OP_MSG_SIN_CARRITOS"), false);
            }
        }

        private void dgvCarritos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCarritos.CurrentRow == null) { return; }

            carritoActual = dgvCarritos.CurrentRow.DataBoundItem as CarritoBE_575_AV;
            MostrarCarrito(carritoActual);
        }

        private void MostrarCarrito(CarritoBE_575_AV carrito)
        {
            if (carrito == null)
            {
                dgvDetalle.DataSource = null;
                txtCliente.Clear();
                lblTotal.Text = 0m.ToString("N2", CultureInfo.CurrentCulture);
                EstiloConectAR_575_AV.EstadoBoton(btnEmitir, false);
                return;
            }

            dgvDetalle.DataSource = carrito.detalle;
            EstiloConectAR_575_AV.MostrarColumnas(dgvDetalle,
                "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad",
                "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.FormatoNumero(dgvDetalle, "cantidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.ColumnaElastica(dgvDetalle, "descripcion_articulo");
            TraducirColumnas();

            txtCliente.Clear();
            lblTotal.Text = carrito.precio_total.ToString("N2", CultureInfo.CurrentCulture);
            EstiloConectAR_575_AV.EstadoBoton(btnEmitir, true);
            Avisar(string.Empty, false);
        }

        #endregion

        #region Emision

        private void btnEmitir_Click(object sender, EventArgs e)
        {
            if (carritoActual == null)
            {
                Avisar(() => gestorIdioma.Traducir("OP_MSG_ELEGIR"), true);
                return;
            }

            try
            {
                OrdenPagoBE_575_AV orden = ordenPagoBLL.GenerarOrdenPago(
                    carritoActual, txtCliente.Text.Trim(), UsuarioActual());

                Func<string> mensaje = () => string.Format(
                    gestorIdioma.Traducir("OP_MSG_EMITIDA"), orden.nro_orden_pago);

                MessageBox.Show(mensaje(), gestorIdioma.Traducir("OP_TITULO"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCarritos();
                Avisar(mensaje, false);
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

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try { CargarCarritos(); }
            catch (Exception) { Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true); }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Apoyo

        private Func<string> avisoTraducible;

        private void Avisar(string mensaje, bool esError)
        {
            avisoTraducible = null;
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = esError
                ? EstiloConectAR_575_AV.Peligro
                : EstiloConectAR_575_AV.NavySuave;
        }

        private void Avisar(Func<string> mensaje, bool esError)
        {
            Avisar(mensaje(), esError);
            avisoTraducible = mensaje;
        }

        private static string UsuarioActual()
        {
            return SessionManager.Logged()
                ? SessionManager.GetInstance.UsuarioActual().user
                : "sin sesion";
        }

        #endregion
    }
}
