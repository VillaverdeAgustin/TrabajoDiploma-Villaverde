using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmEntregaDeposito_575_AV : Form, IObservadorIdioma
    {
        private EntregaBLL_575_AV entregaBLL = new EntregaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private ComprobanteBE_575_AV comprobanteActual;
        private BindingList<Renglon> renglones = new BindingList<Renglon>();

        public frmEntregaDeposito_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "entregar");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmEntregaDeposito_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            MostrarComprobante(null);

            try { bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.DepositoAbierto); }
            catch (Exception) { Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true); }

            txtNroComprobante.Focus();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("DEP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("DEP_TITULO");
            gbComprobante.Text = gestorIdioma.Traducir("DEP_GB_COMPROBANTE");
            lblNroComprobante.Text = gestorIdioma.Traducir("DEP_LBL_NRO_COMPROBANTE");
            btnValidar.Text = gestorIdioma.Traducir("DEP_BTN_VALIDAR");
            gbArticulos.Text = gestorIdioma.Traducir("DEP_GB_ARTICULOS");
            lblAyuda.Text = gestorIdioma.Traducir("DEP_LBL_AYUDA");
            btnRegistrar.Text = gestorIdioma.Traducir("DEP_BTN_REGISTRAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            MostrarCabecera();
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo("codigo", "ART_COL_CODIGO");
            Titulo("descripcion", "ART_COL_DESCRIPCION");
            Titulo("facturado", "DEP_COL_FACTURADO");
            Titulo("unidad", "ART_COL_UNIDAD");
            Titulo("ubicacion", "ART_COL_UBICACION");
            Titulo("a_entregar", "DEP_COL_A_ENTREGAR");
        }

        private void Titulo(string columna, string clave)
        {
            if (dgvArticulos.Columns[columna] != null)
            {
                dgvArticulos.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Validacion del comprobante

        private void btnValidar_Click(object sender, EventArgs e)
        {
            string nro = txtNroComprobante.Text.Trim();

            try
            {
                List<DetalleComprobanteBE_575_AV> aEntregar = entregaBLL.ValidarComprobante(nro);
                ComprobanteBE_575_AV comprobante = entregaBLL.ObtenerComprobante(nro);

                MostrarComprobante(comprobante);
                CargarRenglones(aEntregar);
                Avisar(() => gestorIdioma.Traducir("DEP_MSG_VALIDO"), false);
            }
            catch (InvalidOperationException ex)
            {
                MostrarComprobante(null);
                Avisar(ex.Message, true);
            }
            catch (Exception)
            {
                MostrarComprobante(null);
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void MostrarComprobante(ComprobanteBE_575_AV comprobante)
        {
            comprobanteActual = comprobante;

            if (comprobante == null)
            {
                renglones = new BindingList<Renglon>();
                dgvArticulos.DataSource = renglones;
                ConfigurarGrilla();
            }

            MostrarCabecera();
            EstiloConectAR_575_AV.EstadoBoton(btnRegistrar, comprobante != null);
        }

        private void MostrarCabecera()
        {
            if (comprobanteActual == null)
            {
                lblComprobanteInfo.Text = string.Empty;
                return;
            }

            lblComprobanteInfo.Text = string.Format(
                gestorIdioma.Traducir("DEP_LBL_INFO"),
                comprobanteActual.nro_comprobante,
                comprobanteActual.fecha_emision.ToString("dd/MM/yyyy HH:mm"),
                comprobanteActual.precio_total.ToString("N2", CultureInfo.CurrentCulture));
        }

        private void CargarRenglones(List<DetalleComprobanteBE_575_AV> detalle)
        {
            renglones = new BindingList<Renglon>(detalle.Select(d => new Renglon
            {
                id_articulo = d.id_articulo,
                id_unidad_medida = d.id_unidad_medida,
                codigo = d.codigo_articulo,
                descripcion = d.descripcion_articulo,
                facturado = d.cantidad,
                unidad = d.abreviatura_unidad,
                ubicacion = d.deposito_ubicacion,
                a_entregar = d.cantidad
            }).ToList());

            dgvArticulos.DataSource = renglones;
            ConfigurarGrilla();
        }

        private void ConfigurarGrilla()
        {
            EstiloConectAR_575_AV.MostrarColumnas(dgvArticulos,
                "codigo", "descripcion", "facturado", "unidad", "ubicacion", "a_entregar");
            EstiloConectAR_575_AV.FormatoNumero(dgvArticulos, "facturado", "a_entregar");
            EstiloConectAR_575_AV.ColumnaElastica(dgvArticulos, "descripcion");

            foreach (DataGridViewColumn columna in dgvArticulos.Columns)
            {
                columna.ReadOnly = columna.Name != "a_entregar";
            }

            dgvArticulos.Columns["a_entregar"].DefaultCellStyle.BackColor = EstiloConectAR_575_AV.Fondo;
            dgvArticulos.Columns["a_entregar"].DefaultCellStyle.Font = EstiloConectAR_575_AV.FuenteEtiqueta;

            TraducirColumnas();
        }

        private void dgvArticulos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Avisar(() => gestorIdioma.Traducir("DEP_MSG_CANTIDAD_INVALIDA"), true);
            e.Cancel = true;
        }

        #endregion

        #region Registro

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (comprobanteActual == null) { return; }

            dgvArticulos.EndEdit();

            List<DetalleEntregaBE_575_AV> detalles = renglones
                .Where(r => r.a_entregar > 0)
                .Select(r => new DetalleEntregaBE_575_AV
                {
                    id_articulo = r.id_articulo,
                    id_unidad_medida = r.id_unidad_medida,
                    cantidad_entregada = r.a_entregar
                })
                .ToList();

            try
            {
                EntregaBE_575_AV entrega = entregaBLL.RegistrarEntrega(
                    comprobanteActual, detalles, UsuarioActual());

                Func<string> mensaje = () => string.Format(
                    gestorIdioma.Traducir(entrega.estado == EstadoEntrega_575_AV.Total
                        ? "DEP_MSG_ENTREGA_TOTAL"
                        : "DEP_MSG_ENTREGA_PARCIAL"),
                    entrega.nro_comprobante);

                MessageBox.Show(mensaje(), gestorIdioma.Traducir("DEP_TITULO"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtNroComprobante.Clear();
                MostrarComprobante(null);
                Avisar(mensaje, false);
                txtNroComprobante.Focus();
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

        public class Renglon
        {
            public int id_articulo { get; set; }
            public int id_unidad_medida { get; set; }
            public string codigo { get; set; }
            public string descripcion { get; set; }
            public decimal facturado { get; set; }
            public string unidad { get; set; }
            public string ubicacion { get; set; }
            public decimal a_entregar { get; set; }
        }

        #endregion
    }
}
