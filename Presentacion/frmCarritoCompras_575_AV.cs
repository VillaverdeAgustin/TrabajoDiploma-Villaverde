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
    public partial class frmCarritoCompras_575_AV : Form, IObservadorIdioma
    {
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private CarritoBLL_575_AV carritoBLL = new CarritoBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private CarritoBE_575_AV carritoActual;

        private HashSet<int> sinDisponibilidad = new HashSet<int>();

        private static readonly Color FondoFaltante = Color.FromArgb(251, 234, 232);

        public frmCarritoCompras_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "carrito");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnBuscar);
            EstiloConectAR_575_AV.BotonSecundario(btnQuitar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            TextoTraducido_575_AV.PrimeraOpcion(cmbCategoria, "COMUN_TODOS");
            lblReferencia.ForeColor = EstiloConectAR_575_AV.Peligro;
        }

        private void frmCarritoCompras_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.CarritoComprasAbierto);
                CargarCategorias();
                Buscar();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }

            MostrarCarrito();
            txtDescripcion.Focus();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("VTA_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("VTA_TITULO");
            gbBuscar.Text = gestorIdioma.Traducir("VTA_GB_BUSCAR");
            lblDescripcion.Text = gestorIdioma.Traducir("ART_LBL_DESCRIPCION");
            lblCategoria.Text = gestorIdioma.Traducir("ART_LBL_CATEGORIA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            lblReferencia.Text = gestorIdioma.Traducir("VTA_LBL_REFERENCIA");
            lblCantidad.Text = gestorIdioma.Traducir("VTA_COL_CANTIDAD");
            btnAgregar.Text = gestorIdioma.Traducir("VTA_BTN_AGREGAR");
            gbCarrito.Text = gestorIdioma.Traducir("VTA_GB_CARRITO");
            btnQuitar.Text = gestorIdioma.Traducir("VTA_BTN_QUITAR");
            lblTotalTitulo.Text = gestorIdioma.Traducir("COMUN_TOTAL");
            btnConfirmar.Text = gestorIdioma.Traducir("VTA_BTN_CONFIRMAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            MostrarNumeroCarrito();
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
            TextoTraducido_575_AV.Refrescar(cmbCategoria);
        }

        private void TraducirColumnas()
        {
            Titulo(dgvArticulos, "codigo", "ART_COL_CODIGO");
            Titulo(dgvArticulos, "descripcion", "ART_COL_DESCRIPCION");
            Titulo(dgvArticulos, "nombre_categoria", "ART_COL_CATEGORIA");
            Titulo(dgvArticulos, "nombre_marca", "ART_COL_MARCA");
            Titulo(dgvArticulos, "precio_unitario", "ART_COL_PRECIO");
            Titulo(dgvArticulos, "cantidad_disponible", "ART_COL_DISPONIBLE");
            Titulo(dgvArticulos, "abreviatura_unidad", "ART_COL_UNIDAD");

            Titulo(dgvCarrito, "codigo_articulo", "ART_COL_CODIGO");
            Titulo(dgvCarrito, "descripcion_articulo", "ART_COL_DESCRIPCION");
            Titulo(dgvCarrito, "cantidad", "VTA_COL_CANTIDAD");
            Titulo(dgvCarrito, "abreviatura_unidad", "ART_COL_UNIDAD");
            Titulo(dgvCarrito, "precio_unitario", "ART_COL_PRECIO");
            Titulo(dgvCarrito, "subtotal", "VTA_COL_SUBTOTAL");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Busqueda de articulos

        private void CargarCategorias()
        {
            List<CategoriaBE_575_AV> categorias = articuloBLL.ListarCategorias()
                .Where(c => c.activo)
                .ToList();

            categorias.Insert(0, new CategoriaBE_575_AV
            {
                id_categoria = 0,
                nombre = gestorIdioma.Traducir("COMUN_TODOS")
            });

            cmbCategoria.DataSource = categorias;
            cmbCategoria.DisplayMember = "nombre";
            cmbCategoria.ValueMember = "id_categoria";
            cmbCategoria.SelectedIndex = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Buscar();
            }
        }

        private void Buscar()
        {
            try
            {
                CategoriaBE_575_AV categoria = cmbCategoria.SelectedItem as CategoriaBE_575_AV;
                int? idCategoria = categoria == null || categoria.id_categoria == 0
                    ? (int?)null
                    : categoria.id_categoria;

                List<ArticuloBE_575_AV> articulos = articuloBLL.BuscarArticulos(
                    txtDescripcion.Text, idCategoria, null, null);

                dgvArticulos.DataSource = articulos;

                EstiloConectAR_575_AV.MostrarColumnas(dgvArticulos,
                    "codigo", "descripcion", "nombre_categoria", "nombre_marca",
                    "precio_unitario", "cantidad_disponible", "abreviatura_unidad");
                EstiloConectAR_575_AV.FormatoNumero(dgvArticulos, "precio_unitario", "cantidad_disponible");
                EstiloConectAR_575_AV.ColumnaElastica(dgvArticulos, "descripcion");
                TraducirColumnas();

                Avisar(() => string.Format(gestorIdioma.Traducir("ART_MSG_ENCONTRADOS"), articulos.Count), false);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void dgvArticulos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            ArticuloBE_575_AV articulo = dgvArticulos.Rows[e.RowIndex].DataBoundItem as ArticuloBE_575_AV;
            if (articulo == null) { return; }

            if (!articulo.se_puede_vender || sinDisponibilidad.Contains(articulo.id_articulo))
            {
                e.CellStyle.ForeColor = EstiloConectAR_575_AV.Peligro;
                e.CellStyle.BackColor = FondoFaltante;
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            ArticuloBE_575_AV articulo = ArticuloSeleccionado();

            nudCantidad.DecimalPlaces = articulo != null && articulo.fraccionable ? 2 : 0;
            nudCantidad.Minimum = articulo != null && articulo.fraccionable ? 0.01m : 1m;
            lblUnidad.Text = articulo == null ? string.Empty : articulo.abreviatura_unidad;
        }

        private ArticuloBE_575_AV ArticuloSeleccionado()
        {
            return dgvArticulos.CurrentRow == null
                ? null
                : dgvArticulos.CurrentRow.DataBoundItem as ArticuloBE_575_AV;
        }

        #endregion

        #region Armado del carrito

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            ArticuloBE_575_AV articulo = ArticuloSeleccionado();
            if (articulo == null)
            {
                Avisar(() => gestorIdioma.Traducir("VTA_MSG_ELEGIR_ARTICULO"), true);
                return;
            }

            decimal cantidad = nudCantidad.Value;

            try
            {
                decimal yaEnCarrito = carritoActual == null
                    ? 0m
                    : carritoActual.detalle
                        .Where(d => d.id_articulo == articulo.id_articulo)
                        .Sum(d => d.cantidad);

                if (!articuloBLL.ValidarDisponibilidad(articulo.id_articulo, yaEnCarrito + cantidad))
                {
                    sinDisponibilidad.Add(articulo.id_articulo);
                    dgvArticulos.Invalidate();
                    Avisar(() => string.Format(gestorIdioma.Traducir("VTA_MSG_SIN_DISPONIBILIDAD"),
                        articulo.descripcion), true);
                    return;
                }

                string usuario = UsuarioActual();

                if (carritoActual == null)
                {
                    carritoActual = carritoBLL.CrearCarrito(usuario);
                }

                carritoActual = carritoBLL.AgregarDetalle(carritoActual,
                    new DetalleCarritoBE_575_AV
                    {
                        id_articulo = articulo.id_articulo,
                        cantidad = cantidad
                    },
                    usuario);

                MostrarCarrito();
                Avisar(() => string.Format(gestorIdioma.Traducir("VTA_MSG_AGREGADO"), articulo.descripcion), false);
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

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            DetalleCarritoBE_575_AV renglon = dgvCarrito.CurrentRow == null
                ? null
                : dgvCarrito.CurrentRow.DataBoundItem as DetalleCarritoBE_575_AV;

            if (carritoActual == null || renglon == null)
            {
                Avisar(() => gestorIdioma.Traducir("VTA_MSG_ELEGIR_RENGLON"), true);
                return;
            }

            try
            {
                carritoActual = carritoBLL.QuitarDetalle(carritoActual, renglon, UsuarioActual());
                MostrarCarrito();
                Avisar(() => string.Format(gestorIdioma.Traducir("VTA_MSG_QUITADO"),
                    renglon.descripcion_articulo), false);
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

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (carritoActual == null || carritoActual.detalle.Count == 0)
            {
                Avisar(() => gestorIdioma.Traducir("VTA_MSG_CARRITO_VACIO"), true);
                return;
            }

            try
            {
                string nro = carritoBLL.ConfirmarCarrito(carritoActual, UsuarioActual());
                Func<string> mensaje = () => string.Format(gestorIdioma.Traducir("VTA_MSG_CONFIRMADO"), nro);

                MessageBox.Show(mensaje(), gestorIdioma.Traducir("VTA_TITULO"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                carritoActual = null;
                sinDisponibilidad.Clear();
                MostrarCarrito();
                Buscar();
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

        private void MostrarCarrito()
        {
            bool hayRenglones = carritoActual != null && carritoActual.detalle.Count > 0;

            dgvCarrito.DataSource = null;
            if (carritoActual != null)
            {
                dgvCarrito.DataSource = carritoActual.detalle;
                EstiloConectAR_575_AV.MostrarColumnas(dgvCarrito,
                    "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad",
                    "precio_unitario", "subtotal");
                EstiloConectAR_575_AV.FormatoNumero(dgvCarrito, "cantidad", "precio_unitario", "subtotal");
                EstiloConectAR_575_AV.ColumnaElastica(dgvCarrito, "descripcion_articulo");
                TraducirColumnas();
            }

            decimal total = carritoActual == null ? 0m : carritoActual.precio_total;
            lblTotal.Text = total.ToString("N2", CultureInfo.CurrentCulture);

            MostrarNumeroCarrito();
            EstiloConectAR_575_AV.EstadoBoton(btnQuitar, hayRenglones);
            EstiloConectAR_575_AV.BotonSecundario(btnQuitar);
            btnQuitar.Enabled = hayRenglones;
            EstiloConectAR_575_AV.EstadoBoton(btnConfirmar, hayRenglones);
        }

        private void MostrarNumeroCarrito()
        {
            lblNroCarrito.Text = carritoActual == null
                ? gestorIdioma.Traducir("VTA_LBL_CARRITO_NUEVO")
                : string.Format(gestorIdioma.Traducir("VTA_LBL_NRO_CARRITO"), carritoActual.nro_carrito);
        }

        #endregion

        #region Salida

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmCarritoCompras_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (carritoActual == null || carritoActual.detalle.Count == 0) { return; }

            DialogResult respuesta = MessageBox.Show(
                string.Format(gestorIdioma.Traducir("VTA_MSG_SALIR_SIN_CONFIRMAR"), carritoActual.nro_carrito),
                gestorIdioma.Traducir("COMUN_ATENCION"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) { e.Cancel = true; }
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
