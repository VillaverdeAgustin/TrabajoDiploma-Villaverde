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
    public partial class frmOrdenCompra_575_AV : Form, IObservadorIdioma
    {
        private static readonly Color FondoSinPrecio = Color.FromArgb(253, 243, 219);
        private const int ANCHO_DESCRIPCION = 150;

        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private ListaPrecioBLL_575_AV listaPrecioBLL = new ListaPrecioBLL_575_AV();
        private OrdenCompraBLL_575_AV ordenCompraBLL = new OrdenCompraBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private OrdenCompraBE_575_AV borrador;

        public frmOrdenCompra_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnComparar);
            EstiloConectAR_575_AV.BotonSecundario(btnAgregar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmOrdenCompra_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.OrdenesCompraAbierta);
                CargarProveedores();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("ORD_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("ORD_TITULO");
            gbReponer.Text = gestorIdioma.Traducir("ORD_GB_REPONER");
            lblProveedor.Text = gestorIdioma.Traducir("ORD_LBL_PROVEEDOR");
            btnComparar.Text = gestorIdioma.Traducir("ORD_BTN_COMPARAR");
            lblCantidad.Text = gestorIdioma.Traducir("VTA_COL_CANTIDAD");
            lblPrecio.Text = gestorIdioma.Traducir("ORD_LBL_PRECIO");
            btnAgregar.Text = gestorIdioma.Traducir("ORD_BTN_AGREGAR");
            gbBorrador.Text = gestorIdioma.Traducir("ORD_GB_BORRADOR");
            btnGenerar.Text = gestorIdioma.Traducir("ORD_BTN_GENERAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            MostrarTotal();
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo(dgvReponer, "codigo", "ART_COL_CODIGO");
            Titulo(dgvReponer, "descripcion", "ART_COL_DESCRIPCION");
            Titulo(dgvReponer, "disponible", "ART_COL_DISPONIBLE");
            Titulo(dgvReponer, "punto_reposicion", "CMP_COL_REPOSICION");
            Titulo(dgvReponer, "unidad", "ART_COL_UNIDAD");
            Titulo(dgvReponer, "precio", "ORD_COL_PRECIO");
            Titulo(dgvReponer, "unidad_venta", "CMP_COL_UNIDAD");

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

        #region Proveedor y articulos a reponer

        private void CargarProveedores()
        {
            cmbProveedor.SelectedIndexChanged -= cmbProveedor_SelectedIndexChanged;
            cmbProveedor.DataSource = proveedorBLL.ListarProveedores().Where(p => p.activo).ToList();
            cmbProveedor.DisplayMember = "razon_social";
            cmbProveedor.ValueMember = "id_proveedor";
            cmbProveedor.SelectedIndexChanged += cmbProveedor_SelectedIndexChanged;
            Refrescar();
        }

        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Refrescar();
        }

        private void Refrescar()
        {
            try
            {
                CargarReponer();
                MostrarBorrador();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void CargarReponer()
        {
            int idProveedor = ProveedorElegido();
            List<Reponer> articulos = articuloBLL.ListarParaReponer().Select(a =>
            {
                ItemListaPrecioBE_575_AV vigente = idProveedor > 0
                    ? listaPrecioBLL.ObtenerPrecioVigente(idProveedor, a.id_articulo)
                    : null;
                return new Reponer
                {
                    id_articulo = a.id_articulo,
                    codigo = a.codigo,
                    descripcion = a.descripcion,
                    disponible = a.cantidad_disponible,
                    punto_reposicion = a.punto_reposicion,
                    unidad = a.abreviatura_unidad,
                    fraccionable = a.fraccionable,
                    precio = vigente == null ? (decimal?)null : vigente.precio_lista,
                    unidad_venta = vigente == null ? null : vigente.unidad_venta
                };
            }).ToList();

            dgvReponer.DataSource = articulos;
            EstiloConectAR_575_AV.MostrarColumnas(dgvReponer,
                "codigo", "descripcion", "disponible", "punto_reposicion", "unidad", "precio", "unidad_venta");
            EstiloConectAR_575_AV.FormatoNumero(dgvReponer, "disponible", "punto_reposicion", "precio");
            EstiloConectAR_575_AV.ColumnaElastica(dgvReponer, "descripcion");
            dgvReponer.Columns["descripcion"].MinimumWidth = ANCHO_DESCRIPCION;
            TraducirColumnas();

            if (articulos.Count == 0)
            {
                Avisar(() => gestorIdioma.Traducir("ORD_MSG_SIN_ARTICULOS"), false);
                PrepararRenglon(null);
            }
        }

        private void dgvReponer_CurrentCellChanged(object sender, EventArgs e)
        {
            PrepararRenglon(ArticuloSeleccionado());
        }

        private void PrepararRenglon(Reponer articulo)
        {
            nudCantidad.DecimalPlaces = articulo != null && articulo.fraccionable ? 2 : 0;
            nudCantidad.Value = articulo == null ? 1 : Math.Max(1, Math.Ceiling(articulo.punto_reposicion - articulo.disponible));
            lblUnidad.Text = articulo == null ? string.Empty : articulo.unidad;

            bool manual = articulo != null && !articulo.precio.HasValue;
            nudPrecio.Enabled = manual;
            nudPrecio.Value = 0;
            EstiloConectAR_575_AV.EstadoBoton(btnAgregar, articulo != null);

            if (manual)
            {
                Avisar(() => gestorIdioma.Traducir("ORD_MSG_PRECIO_MANUAL"), true);
            }
            else if (articulo != null)
            {
                Avisar(string.Empty, false);
            }
        }

        private void dgvReponer_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Reponer articulo = dgvReponer.Rows[e.RowIndex].DataBoundItem as Reponer;
            if (articulo != null && !articulo.precio.HasValue)
            {
                e.CellStyle.BackColor = FondoSinPrecio;
            }
        }

        private void btnComparar_Click(object sender, EventArgs e)
        {
            Reponer articulo = ArticuloSeleccionado();
            using (frmCompararPrecios_575_AV comparar = new frmCompararPrecios_575_AV(articulo == null ? null : articulo.codigo))
            {
                comparar.ShowDialog(this);
            }
            Refrescar();
        }

        #endregion

        #region Borrador y generacion

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Reponer articulo = ArticuloSeleccionado();
            int idProveedor = ProveedorElegido();
            if (articulo == null || idProveedor <= 0)
            {
                Avisar(() => gestorIdioma.Traducir("ORD_MSG_ELEGIR"), true);
                return;
            }

            try
            {
                decimal pedida = nudCantidad.Value;
                DetalleOrdenCompraBE_575_AV detalle = new DetalleOrdenCompraBE_575_AV
                {
                    id_articulo = articulo.id_articulo,
                    cantidad = pedida,
                    precio_unitario = articulo.precio.HasValue ? 0 : nudPrecio.Value
                };

                OrdenCompraBE_575_AV orden = ordenCompraBLL.AgregarArticuloABorrador(idProveedor, detalle, UsuarioActual());
                string total = orden.total.ToString("N2", CultureInfo.CurrentCulture);

                MostrarBorrador();
                if (detalle.cantidad != pedida)
                {
                    Avisar(() => string.Format(gestorIdioma.Traducir("ORD_MSG_AJUSTADO"),
                        detalle.cantidad.ToString("N2", CultureInfo.CurrentCulture), articulo.unidad,
                        detalle.unidad_venta, total), false);
                }
                else
                {
                    Avisar(() => string.Format(gestorIdioma.Traducir("ORD_MSG_AGREGADO"), articulo.descripcion, total), false);
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

        private void MostrarBorrador()
        {
            int idProveedor = ProveedorElegido();
            borrador = idProveedor > 0 ? ordenCompraBLL.ObtenerBorrador(idProveedor) : null;

            dgvDetalle.DataSource = borrador == null ? new List<DetalleOrdenCompraBE_575_AV>() : borrador.detalle;
            EstiloConectAR_575_AV.MostrarColumnas(dgvDetalle,
                "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.FormatoNumero(dgvDetalle, "cantidad", "precio_unitario", "subtotal");
            EstiloConectAR_575_AV.ColumnaElastica(dgvDetalle, "descripcion_articulo");
            TraducirColumnas();

            EstiloConectAR_575_AV.EstadoBoton(btnGenerar, borrador != null && borrador.detalle.Count > 0);
            MostrarTotal();
        }

        private void MostrarTotal()
        {
            decimal total = borrador == null ? 0 : borrador.total;
            lblTotal.Text = string.Format(gestorIdioma.Traducir("ORD_LBL_TOTAL"), total.ToString("N2", CultureInfo.CurrentCulture));
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                OrdenCompraBE_575_AV actual = ordenCompraBLL.ObtenerBorrador(ProveedorElegido());
                if (actual == null)
                {
                    Avisar(() => gestorIdioma.Traducir("OC_ERR_SIN_BORRADOR"), true);
                    return;
                }

                OrdenCompraBE_575_AV generada = ordenCompraBLL.GenerarOrdenCompra(actual, actual.detalle, UsuarioActual());
                Refrescar();
                Avisar(() => string.Format(gestorIdioma.Traducir("ORD_MSG_GENERADA"), generada.nro_orden_compra), false);
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

        private int ProveedorElegido()
        {
            return cmbProveedor.SelectedValue is int ? (int)cmbProveedor.SelectedValue : 0;
        }

        private Reponer ArticuloSeleccionado()
        {
            return dgvReponer.CurrentRow == null ? null : dgvReponer.CurrentRow.DataBoundItem as Reponer;
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

        public class Reponer
        {
            public int id_articulo { get; set; }
            public string codigo { get; set; }
            public string descripcion { get; set; }
            public decimal disponible { get; set; }
            public decimal punto_reposicion { get; set; }
            public string unidad { get; set; }
            public bool fraccionable { get; set; }
            public decimal? precio { get; set; }
            public string unidad_venta { get; set; }
        }

        #endregion
    }
}
