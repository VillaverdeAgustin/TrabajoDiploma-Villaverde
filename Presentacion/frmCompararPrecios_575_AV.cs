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
    public partial class frmCompararPrecios_575_AV : Form, IObservadorIdioma
    {
        private static readonly Color FondoVencida = Color.FromArgb(253, 243, 219);
        private const int ANCHO_PROVEEDOR = 180;

        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private ListaPrecioBLL_575_AV listaPrecioBLL = new ListaPrecioBLL_575_AV();
        private OrdenCompraBLL_575_AV ordenCompraBLL = new OrdenCompraBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private string codigoInicial;

        public frmCompararPrecios_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnBuscar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            TextoTraducido_575_AV.PrimeraOpcion(cmbMarca, "COMUN_TODOS");
        }

        public frmCompararPrecios_575_AV(string codigo) : this()
        {
            codigoInicial = codigo;
        }

        private void frmCompararPrecios_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.CompararPreciosAbierta);
                CargarMarcas();
                cmbMoneda.SelectedIndex = 0;
                txtBuscar.Text = codigoInicial ?? string.Empty;
                Buscar();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("CMP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("CMP_TITULO");
            gbBuscar.Text = gestorIdioma.Traducir("CMP_GB_BUSCAR");
            lblBuscar.Text = gestorIdioma.Traducir("CMP_LBL_BUSCAR");
            lblMarca.Text = gestorIdioma.Traducir("ART_LBL_MARCA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            gbPrecios.Text = gestorIdioma.Traducir("CMP_GB_PRECIOS");
            lblLeyenda.Text = string.Format(gestorIdioma.Traducir("CMP_LBL_LEYENDA"), ListaPrecioBLL_575_AV.DIAS_VIGENCIA);
            lblMoneda.Text = gestorIdioma.Traducir("CMP_LBL_MONEDA");
            lblCantidad.Text = gestorIdioma.Traducir("VTA_COL_CANTIDAD");
            btnAgregar.Text = gestorIdioma.Traducir("CMP_BTN_AGREGAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
            TextoTraducido_575_AV.Refrescar(cmbMarca);
        }

        private void TraducirColumnas()
        {
            Titulo(dgvArticulos, "codigo", "ART_COL_CODIGO");
            Titulo(dgvArticulos, "descripcion", "ART_COL_DESCRIPCION");
            Titulo(dgvArticulos, "nombre_marca", "ART_COL_MARCA");
            Titulo(dgvArticulos, "cantidad_disponible", "ART_COL_DISPONIBLE");
            Titulo(dgvArticulos, "punto_reposicion", "CMP_COL_REPOSICION");
            Titulo(dgvArticulos, "abreviatura_unidad", "ART_COL_UNIDAD");

            Titulo(dgvPrecios, "proveedor", "CMP_COL_PROVEEDOR");
            Titulo(dgvPrecios, "precio", "CMP_COL_PRECIO");
            Titulo(dgvPrecios, "moneda", "CMP_COL_MONEDA");
            Titulo(dgvPrecios, "vigencia", "CMP_COL_VIGENCIA");
            Titulo(dgvPrecios, "plazo", "CMP_COL_PLAZO");
            Titulo(dgvPrecios, "unidad", "CMP_COL_UNIDAD");
            Titulo(dgvPrecios, "codigo", "CMP_COL_CODIGO");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Busqueda

        private void CargarMarcas()
        {
            List<MarcaBE_575_AV> marcas = articuloBLL.ListarMarcas().Where(m => m.activo).ToList();
            marcas.Insert(0, new MarcaBE_575_AV { id_marca = 0, nombre = gestorIdioma.Traducir("COMUN_TODOS") });
            cmbMarca.DataSource = marcas;
            cmbMarca.DisplayMember = "nombre";
            cmbMarca.ValueMember = "id_marca";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
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
                string texto = txtBuscar.Text.Trim();
                MarcaBE_575_AV marca = cmbMarca.SelectedItem as MarcaBE_575_AV;
                int? idMarca = marca == null || marca.id_marca == 0 ? (int?)null : marca.id_marca;

                List<ArticuloBE_575_AV> articulos = articuloBLL.BuscarArticulos(texto, null, idMarca, null);
                if (articulos.Count == 0 && texto.Length > 0)
                {
                    articulos = articuloBLL.BuscarArticulos(null, null, idMarca, texto);
                }

                Avisar(() => string.Format(gestorIdioma.Traducir("ART_MSG_ENCONTRADOS"), articulos.Count), false);
                dgvArticulos.DataSource = articulos;
                EstiloConectAR_575_AV.MostrarColumnas(dgvArticulos,
                    "codigo", "descripcion", "nombre_marca", "cantidad_disponible", "punto_reposicion", "abreviatura_unidad");
                EstiloConectAR_575_AV.FormatoNumero(dgvArticulos, "cantidad_disponible", "punto_reposicion");
                EstiloConectAR_575_AV.ColumnaElastica(dgvArticulos, "descripcion");
                TraducirColumnas();

                if (articulos.Count == 0) { MostrarPrecios(null); }
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            ArticuloBE_575_AV articulo = ArticuloSeleccionado();
            nudCantidad.DecimalPlaces = articulo != null && articulo.fraccionable ? 2 : 0;
            lblUnidad.Text = articulo == null ? string.Empty : articulo.abreviatura_unidad;
            MostrarPrecios(articulo);
        }

        #endregion

        #region Comparacion

        private void MostrarPrecios(ArticuloBE_575_AV articulo)
        {
            if (articulo == null)
            {
                dgvPrecios.DataSource = null;
                EstiloConectAR_575_AV.EstadoBoton(btnAgregar, false);
                return;
            }

            try
            {
                List<ComparacionPrecio_575_AV> precios = listaPrecioBLL.CompararPrecios(articulo.id_articulo, MonedaElegida());
                dgvPrecios.DataSource = precios.Select(p => new Precio
                {
                    id_proveedor = p.id_proveedor,
                    proveedor = p.razon_social,
                    precio = p.precio_lista,
                    moneda = p.moneda,
                    vigencia = p.fecha_vigencia.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture),
                    plazo = p.plazo_entrega,
                    unidad = p.unidad_venta,
                    codigo = p.codigo_proveedor,
                    vencida = p.vencida,
                    observacion = p.vencida
                        ? string.Format(gestorIdioma.Traducir("CMP_OBS_VENCIDA"),
                            p.fecha_vigencia.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture))
                        : string.Empty
                }).ToList();

                EstiloConectAR_575_AV.MostrarColumnas(dgvPrecios,
                    "proveedor", "precio", "moneda", "vigencia", "plazo", "unidad", "codigo");
                EstiloConectAR_575_AV.FormatoNumero(dgvPrecios, "precio");
                EstiloConectAR_575_AV.ColumnaElastica(dgvPrecios, "proveedor");
                dgvPrecios.Columns["proveedor"].MinimumWidth = ANCHO_PROVEEDOR;
                TraducirColumnas();

                EstiloConectAR_575_AV.EstadoBoton(btnAgregar, precios.Count > 0);
                if (precios.Count == 0)
                {
                    Avisar(() => gestorIdioma.Traducir("CMP_MSG_SIN_PROVEEDORES"), true);
                }
                else
                {
                    Avisar(string.Empty, false);
                }
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void cmbMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarPrecios(ArticuloSeleccionado());
        }

        private string MonedaElegida()
        {
            return cmbMoneda.SelectedItem == null ? "ARS" : cmbMoneda.SelectedItem.ToString();
        }

        private void dgvPrecios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Precio precio = dgvPrecios.Rows[e.RowIndex].DataBoundItem as Precio;
            if (precio != null && precio.vencida)
            {
                e.CellStyle.BackColor = FondoVencida;
                dgvPrecios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = precio.observacion;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            ArticuloBE_575_AV articulo = ArticuloSeleccionado();
            Precio precio = dgvPrecios.CurrentRow == null ? null : dgvPrecios.CurrentRow.DataBoundItem as Precio;
            if (articulo == null || precio == null)
            {
                Avisar(() => gestorIdioma.Traducir("CMP_MSG_ELEGIR"), true);
                return;
            }

            try
            {
                OrdenCompraBE_575_AV orden = ordenCompraBLL.AgregarArticuloABorrador(precio.id_proveedor,
                    new DetalleOrdenCompraBE_575_AV { id_articulo = articulo.id_articulo, cantidad = nudCantidad.Value },
                    UsuarioActual());

                Avisar(() => string.Format(gestorIdioma.Traducir("CMP_MSG_AGREGADO"), articulo.descripcion, precio.proveedor,
                    orden.total.ToString("N2", CultureInfo.CurrentCulture)), false);
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

        private ArticuloBE_575_AV ArticuloSeleccionado()
        {
            return dgvArticulos.CurrentRow == null ? null : dgvArticulos.CurrentRow.DataBoundItem as ArticuloBE_575_AV;
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

        public class Precio
        {
            public int id_proveedor { get; set; }
            public string proveedor { get; set; }
            public decimal precio { get; set; }
            public string moneda { get; set; }
            public string vigencia { get; set; }
            public int? plazo { get; set; }
            public string unidad { get; set; }
            public string codigo { get; set; }
            public bool vencida { get; set; }
            public string observacion { get; set; }
        }

        #endregion
    }
}
