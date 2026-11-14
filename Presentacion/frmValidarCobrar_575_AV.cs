using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmValidarCobrar_575_AV : Form, IObservadorIdioma
    {
        private const string CORREO_OCULTO = "••••••••";

        private OrdenPagoBLL_575_AV ordenPagoBLL = new OrdenPagoBLL_575_AV();
        private CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
        private ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private VentaBLL_575_AV ventaBLL = new VentaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private OrdenPagoBE_575_AV ordenActual;
        private ClienteBE_575_AV clienteActual;
        private List<DetalleCarritoBE_575_AV> detalleActual = new List<DetalleCarritoBE_575_AV>();

        public frmValidarCobrar_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnVerCorreo);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            EstiloConectAR_575_AV.CampoBloqueado(txtCorreo);
        }

        private void frmValidarCobrar_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                CargarMediosPago();
                CargarOrdenes();
                MostrarCliente(null);
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.CajaAbierta);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("CAJA_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("CAJA_TITULO");
            gbOrdenes.Text = gestorIdioma.Traducir("CAJA_GB_ORDENES");
            lblNroOrden.Text = gestorIdioma.Traducir("CAJA_LBL_NRO_ORDEN");
            btnBuscarOrden.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            gbCliente.Text = gestorIdioma.Traducir("CAJA_GB_CLIENTE");
            lblDni.Text = gestorIdioma.Traducir("CLI_LBL_DNI");
            btnBuscarCliente.Text = gestorIdioma.Traducir("CAJA_BTN_BUSCAR_CLIENTE");
            lblCorreo.Text = gestorIdioma.Traducir("CAJA_LBL_CORREO");
            btnVerCorreo.Text = gestorIdioma.Traducir("CAJA_BTN_VER_CORREO");
            gbDetalle.Text = gestorIdioma.Traducir("CAJA_GB_DETALLE");
            gbPago.Text = gestorIdioma.Traducir("CAJA_GB_PAGO");
            lblMedioPago.Text = gestorIdioma.Traducir("CAJA_LBL_MEDIO_PAGO");
            lblDatosPago.Text = gestorIdioma.Traducir("CAJA_LBL_DATOS_PAGO");
            lblTotalTitulo.Text = gestorIdioma.Traducir("COMUN_TOTAL");
            btnCobrar.Text = gestorIdioma.Traducir("CAJA_BTN_COBRAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            MostrarCliente(clienteActual);
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo(dgvOrdenes, "nro_orden_pago", "CAJA_COL_ORDEN");
            Titulo(dgvOrdenes, "nombre_cliente", "OP_COL_CLIENTE");
            Titulo(dgvOrdenes, "fecha_emision", "OP_COL_FECHA");
            Titulo(dgvOrdenes, "precio_total", "COMUN_TOTAL");

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

        #region Orden de pago

        private void CargarOrdenes()
        {
            dgvOrdenes.DataSource = ordenPagoBLL.ListarPendientes();

            EstiloConectAR_575_AV.MostrarColumnas(dgvOrdenes,
                "nro_orden_pago", "nombre_cliente", "fecha_emision", "precio_total");
            EstiloConectAR_575_AV.FormatoNumero(dgvOrdenes, "precio_total");
            EstiloConectAR_575_AV.ColumnaElastica(dgvOrdenes, "nombre_cliente");
            TraducirColumnas();

            if (dgvOrdenes.Rows.Count == 0)
            {
                MostrarOrden(null);
                Avisar(() => gestorIdioma.Traducir("CAJA_MSG_SIN_ORDENES"), false);
            }
        }

        private void dgvOrdenes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrdenes.CurrentRow == null) { return; }
            MostrarOrden(dgvOrdenes.CurrentRow.DataBoundItem as OrdenPagoBE_575_AV);
        }

        private void btnBuscarOrden_Click(object sender, EventArgs e)
        {
            try
            {
                OrdenPagoBE_575_AV orden = ordenPagoBLL.BuscarOrdenPago(txtNroOrden.Text);

                if (orden == null)
                {
                    Avisar(() => gestorIdioma.Traducir("CAJA_MSG_ORDEN_INEXISTENTE"), true);
                    return;
                }
                if (!orden.se_puede_cobrar)
                {
                    Avisar(() => gestorIdioma.Traducir("CAJA_MSG_ORDEN_COBRADA"), true);
                    return;
                }

                MostrarOrden(orden);
                Avisar(string.Empty, false);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void MostrarOrden(OrdenPagoBE_575_AV orden)
        {
            ordenActual = orden;

            if (orden == null)
            {
                detalleActual = new List<DetalleCarritoBE_575_AV>();
                dgvDetalle.DataSource = null;
                lblTotal.Text = 0m.ToString("N2", CultureInfo.CurrentCulture);
                ActualizarBotonCobrar();
                return;
            }

            txtNroOrden.Text = orden.nro_orden_pago;

            detalleActual = carritoBLL.ObtenerDetalle(orden.id_carrito);
            dgvDetalle.DataSource = detalleActual;
            EstiloConectAR_575_AV.MostrarColumnas(dgvDetalle,
                "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad",
                "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.FormatoNumero(dgvDetalle, "cantidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.ColumnaElastica(dgvDetalle, "descripcion_articulo");
            TraducirColumnas();

            lblTotal.Text = orden.precio_total.ToString("N2", CultureInfo.CurrentCulture);
            ActualizarBotonCobrar();
        }

        #endregion

        #region Cliente

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                Avisar(() => gestorIdioma.Traducir("CAJA_MSG_INGRESAR_DNI"), true);
                return;
            }

            try
            {
                ClienteBE_575_AV cliente = clienteBLL.BuscarPorDni(dni);

                if (cliente == null)
                {
                    DialogResult respuesta = MessageBox.Show(
                        string.Format(gestorIdioma.Traducir("CAJA_MSG_CLIENTE_INEXISTENTE"), dni),
                        gestorIdioma.Traducir("CAJA_GB_CLIENTE"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (respuesta == DialogResult.Yes)
                    {
                        using (frmRegistrarCliente_575_AV alta = new frmRegistrarCliente_575_AV(dni))
                        {
                            if (alta.ShowDialog(this) == DialogResult.OK)
                            {
                                cliente = alta.ClienteRegistrado;
                            }
                        }
                    }
                }

                MostrarCliente(cliente);
                Avisar(string.Empty, false);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void MostrarCliente(ClienteBE_575_AV cliente)
        {
            clienteActual = cliente;

            if (cliente == null)
            {
                lblClienteNombre.Text = gestorIdioma.Traducir("CAJA_LBL_SIN_CLIENTE");
                lblClienteIva.Text = string.Empty;
                lblClienteCuit.Text = string.Empty;
                txtCorreo.Text = string.Empty;
                EstiloConectAR_575_AV.EstadoBoton(btnVerCorreo, false);
                ActualizarBotonCobrar();
                return;
            }

            txtDni.Text = cliente.dni;
            lblClienteNombre.Text = cliente.nombre_para_mostrar;
            lblClienteIva.Text = gestorIdioma.Traducir("CLI_LBL_CONDICION_IVA") + ": " +
                                 gestorIdioma.Traducir("CLI_IVA_" + cliente.condicion_iva);
            lblClienteCuit.Text = string.IsNullOrWhiteSpace(cliente.cuit)
                ? string.Empty
                : gestorIdioma.Traducir("CLI_LBL_CUIT") + ": " + cliente.cuit;

            bool tieneCorreo = !string.IsNullOrWhiteSpace(cliente.correo_electronico);
            txtCorreo.Text = tieneCorreo ? CORREO_OCULTO : string.Empty;
            EstiloConectAR_575_AV.EstadoBoton(btnVerCorreo, tieneCorreo);
            EstiloConectAR_575_AV.BotonSecundario(btnVerCorreo);
            btnVerCorreo.Enabled = tieneCorreo;

            ActualizarBotonCobrar();
        }

        private void btnVerCorreo_Click(object sender, EventArgs e)
        {
            if (clienteActual == null) { return; }

            string clave = PedirClave();
            if (clave == null) { return; }

            try
            {
                txtCorreo.Text = clienteBLL.DesencriptarCorreo(clienteActual, clave, UsuarioActual());
                Avisar(string.Empty, false);
            }
            catch (InvalidOperationException)
            {
                Avisar(() => gestorIdioma.Traducir("CAJA_MSG_CLAVE_INCORRECTA"), true);
            }
        }

        private string PedirClave()
        {
            using (Form dialogo = new Form())
            using (Label etiqueta = new Label())
            using (TextBox caja = new TextBox())
            using (Button aceptar = new Button())
            using (Button cancelar = new Button())
            {
                dialogo.Text = gestorIdioma.Traducir("CAJA_BTN_VER_CORREO");
                dialogo.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialogo.StartPosition = FormStartPosition.CenterParent;
                dialogo.MinimizeBox = false;
                dialogo.MaximizeBox = false;
                dialogo.ShowInTaskbar = false;
                dialogo.ClientSize = new Size(340, 130);

                etiqueta.Text = gestorIdioma.Traducir("CAJA_LBL_INGRESE_CLAVE");
                etiqueta.Location = new Point(16, 14);
                etiqueta.Size = new Size(308, 22);

                caja.UseSystemPasswordChar = true;
                caja.Location = new Point(16, 42);
                caja.Size = new Size(308, 25);

                aceptar.Text = gestorIdioma.Traducir("COMUN_ACEPTAR");
                aceptar.DialogResult = DialogResult.OK;
                aceptar.Location = new Point(118, 84);
                aceptar.Size = new Size(100, 32);

                cancelar.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
                cancelar.DialogResult = DialogResult.Cancel;
                cancelar.Location = new Point(224, 84);
                cancelar.Size = new Size(100, 32);

                dialogo.Controls.AddRange(new Control[] { etiqueta, caja, aceptar, cancelar });
                dialogo.AcceptButton = aceptar;
                dialogo.CancelButton = cancelar;

                EstiloConectAR_575_AV.AplicarFormulario(dialogo);
                EstiloConectAR_575_AV.BotonPrimario(aceptar);
                EstiloConectAR_575_AV.BotonPeligro(cancelar);

                return dialogo.ShowDialog(this) == DialogResult.OK ? caja.Text : null;
            }
        }

        #endregion

        #region Pago y cobro

        private void CargarMediosPago()
        {
            cmbMedioPago.DataSource = articuloBLL.ListarMediosPago().Where(m => m.activo).ToList();
            cmbMedioPago.DisplayMember = "nombre";
        }

        private void cmbMedioPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            MedioPagoBE_575_AV medio = cmbMedioPago.SelectedItem as MedioPagoBE_575_AV;
            bool pideDatos = medio != null && medio.requiere_autorizacion;

            txtDatosPago.ReadOnly = !pideDatos;
            if (pideDatos) { EstiloConectAR_575_AV.CampoEditable(txtDatosPago); }
            else
            {
                txtDatosPago.Clear();
                EstiloConectAR_575_AV.CampoBloqueado(txtDatosPago);
            }
        }

        private void ActualizarBotonCobrar()
        {
            EstiloConectAR_575_AV.EstadoBoton(btnCobrar,
                ordenActual != null && clienteActual != null && detalleActual.Count > 0);
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            MedioPagoBE_575_AV medio = cmbMedioPago.SelectedItem as MedioPagoBE_575_AV;

            try
            {
                ComprobanteBE_575_AV comprobante = ventaBLL.Cobrar(
                    ordenActual, clienteActual, detalleActual, medio,
                    txtDatosPago.Text.Trim(), UsuarioActual());

                Func<string> mensaje = () => string.Format(
                    gestorIdioma.Traducir("CAJA_MSG_COMPROBANTE_EMITIDO"),
                    comprobante.nro_comprobante,
                    comprobante.precio_total.ToString("N2", CultureInfo.CurrentCulture));

                LimpiarPantalla();
                Avisar(mensaje, false);

                using (frmComprobante_575_AV frm = new frmComprobante_575_AV(comprobante.id_comprobante))
                {
                    frm.ShowDialog(this);
                }
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

        private void LimpiarPantalla()
        {
            txtNroOrden.Clear();
            txtDni.Clear();
            txtDatosPago.Clear();
            MostrarCliente(null);
            CargarOrdenes();
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
