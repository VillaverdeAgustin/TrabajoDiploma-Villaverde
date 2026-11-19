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
    public partial class frmBitacoraCambios_575_AV : Form, IObservadorIdioma
    {
        private static readonly Color Resaltado = Color.FromArgb(255, 236, 179);
        private static readonly string[] Operaciones = { "Alta", "Modificacion", "Baja", "Restauracion" };

        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly ArticuloHistorialBLL_575_AV historialBLL = new ArticuloHistorialBLL_575_AV();
        private readonly ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private readonly BitacoraBLL bitacora = new BitacoraBLL();

        private List<ArticuloBE_575_AV> articulos = new List<ArticuloBE_575_AV>();
        private Dictionary<int, string> categorias = new Dictionary<int, string>();
        private Dictionary<int, string> marcas = new Dictionary<int, string>();
        private Dictionary<int, string> unidades = new Dictionary<int, string>();
        private ArticuloHistorialBE_575_AV anterior;
        private ArticuloHistorialBE_575_AV posterior;
        private Func<string> avisoTraducible;
        private bool cargando;

        public frmBitacoraCambios_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "bitacora_cambios");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnBuscar);
            EstiloConectAR_575_AV.BotonPrimario(btnRestaurar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            lblLeyenda.BackColor = Resaltado;
        }

        private void frmBitacoraCambios_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            MostrarCambio(null);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.BitacoraCambiosAbierta);
                articulos = articuloBLL.ListarArticulos().OrderBy(a => a.codigo).ToList();
                categorias = articuloBLL.ListarCategorias().ToDictionary(c => c.id_categoria, c => c.nombre);
                marcas = articuloBLL.ListarMarcas().ToDictionary(m => m.id_marca, m => m.nombre);
                unidades = articuloBLL.ListarUnidadesMedida().ToDictionary(u => u.id_unidad_medida, u => u.nombre);
                CargarFiltros();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("HIS_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("HIS_TITULO");
            gbFiltros.Text = gestorIdioma.Traducir("HIS_GB_FILTROS");
            lblArticulo.Text = gestorIdioma.Traducir("HIS_LBL_ARTICULO");
            lblOperacion.Text = gestorIdioma.Traducir("HIS_LBL_OPERACION");
            lblDesde.Text = gestorIdioma.Traducir("COMUN_DESDE");
            lblHasta.Text = gestorIdioma.Traducir("COMUN_HASTA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            gbHistorial.Text = gestorIdioma.Traducir("HIS_GB_HISTORIAL");
            gbComparacion.Text = gestorIdioma.Traducir("HIS_GB_COMPARACION");
            lblLeyenda.Text = gestorIdioma.Traducir("HIS_LBL_LEYENDA");
            rbAnterior.Text = gestorIdioma.Traducir("HIS_RB_ANTERIOR");
            rbPosterior.Text = gestorIdioma.Traducir("HIS_RB_POSTERIOR");
            btnRestaurar.Text = gestorIdioma.Traducir("HIS_BTN_RESTAURAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            colFecha.HeaderText = gestorIdioma.Traducir("COMUN_FECHA");
            colUsuario.HeaderText = gestorIdioma.Traducir("COMUN_USUARIO");
            colOperacion.HeaderText = gestorIdioma.Traducir("HIS_COL_OPERACION");
            colCodigo.HeaderText = gestorIdioma.Traducir("ART_COL_CODIGO");
            colDescripcion.HeaderText = gestorIdioma.Traducir("ART_COL_DESCRIPCION");
            colCampo.HeaderText = gestorIdioma.Traducir("HIS_COL_CAMPO");
            colAnterior.HeaderText = gestorIdioma.Traducir("HIS_COL_ANTERIOR");
            colPosterior.HeaderText = gestorIdioma.Traducir("HIS_COL_POSTERIOR");

            if (cmbOperacion.Items.Count > 0)
            {
                CargarFiltros();
                foreach (DataGridViewRow fila in dgvHistorial.Rows)
                {
                    fila.Cells[colOperacion.Index].Value = Operacion(((ArticuloHistorialBE_575_AV)fila.Tag).operacion);
                }
                MostrarComparacion();
            }
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        #endregion

        #region Filtros y busqueda

        private void CargarFiltros()
        {
            cargando = true;
            int articulo = Math.Max(cmbArticulo.SelectedIndex, 0);
            int operacion = Math.Max(cmbOperacion.SelectedIndex, 0);

            cmbArticulo.Items.Clear();
            cmbArticulo.Items.Add(gestorIdioma.Traducir("COMUN_TODOS"));
            foreach (ArticuloBE_575_AV a in articulos) { cmbArticulo.Items.Add(a.codigo + " - " + a.descripcion); }
            cmbOperacion.Items.Clear();
            cmbOperacion.Items.Add(gestorIdioma.Traducir("COMUN_TODOS"));
            foreach (string op in Operaciones) { cmbOperacion.Items.Add(Operacion(op)); }

            cmbArticulo.SelectedIndex = Math.Min(articulo, cmbArticulo.Items.Count - 1);
            cmbOperacion.SelectedIndex = operacion;
            cargando = false;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            int? id_articulo = cmbArticulo.SelectedIndex > 0 ? articulos[cmbArticulo.SelectedIndex - 1].id_articulo : (int?)null;
            string operacion = cmbOperacion.SelectedIndex > 0 ? Operaciones[cmbOperacion.SelectedIndex - 1] : null;

            try
            {
                List<ArticuloHistorialBE_575_AV> cambios = historialBLL.BuscarHistorial(id_articulo, operacion, dtpDesde.Value, dtpHasta.Value);

                cargando = true;
                dgvHistorial.Rows.Clear();
                foreach (ArticuloHistorialBE_575_AV c in cambios)
                {
                    int i = dgvHistorial.Rows.Add(c.fecha.ToString("dd/MM/yyyy HH:mm"), c.usuario_modificacion, Operacion(c.operacion), c.codigo, c.descripcion);
                    dgvHistorial.Rows[i].Tag = c;
                }
                cargando = false;
                MostrarCambio(Seleccionado());

                if (cambios.Count == 0) { Avisar(() => gestorIdioma.Traducir("HIS_MSG_SIN_CAMBIOS"), false); }
                else { Avisar(() => string.Format(gestorIdioma.Traducir("HIS_MSG_ENCONTRADOS"), cambios.Count), false); }
            }
            catch (InvalidOperationException ex)
            {
                cargando = false;
                Avisar(ex.Message, true);
            }
            catch (Exception)
            {
                cargando = false;
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #endregion

        #region Comparacion

        private void dgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            if (cargando) { return; }

            try
            {
                MostrarCambio(Seleccionado());
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void MostrarCambio(ArticuloHistorialBE_575_AV cambio)
        {
            List<ArticuloHistorialBE_575_AV> imagenes = cambio == null ? new List<ArticuloHistorialBE_575_AV>() : historialBLL.ObtenerCambio(cambio.id_cambio);
            anterior = imagenes.FirstOrDefault(h => h.imagen == "A");
            posterior = imagenes.FirstOrDefault(h => h.imagen == "P");

            rbAnterior.Enabled = anterior != null;
            rbPosterior.Enabled = posterior != null;
            if (anterior != null) { rbAnterior.Checked = true; }
            else if (posterior != null) { rbPosterior.Checked = true; }
            EstiloConectAR_575_AV.EstadoBoton(btnRestaurar, posterior != null);

            MostrarComparacion();
        }

        private void MostrarComparacion()
        {
            dgvComparacion.Rows.Clear();
            if (posterior == null) { return; }

            foreach (var campo in Campos())
            {
                string antes = anterior == null ? string.Empty : campo.Item2(anterior);
                string despues = campo.Item2(posterior);
                int i = dgvComparacion.Rows.Add(gestorIdioma.Traducir(campo.Item1), antes, despues);
                if (anterior != null && antes != despues)
                {
                    DataGridViewCellStyle estilo = dgvComparacion.Rows[i].DefaultCellStyle;
                    estilo.BackColor = Resaltado;
                    estilo.Font = EstiloConectAR_575_AV.FuenteEtiqueta;
                }
            }
            dgvComparacion.ClearSelection();
        }

        private IEnumerable<Tuple<string, Func<ArticuloHistorialBE_575_AV, string>>> Campos()
        {
            CultureInfo ar = CultureInfo.GetCultureInfo("es-AR");
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_CODIGO", v => v.codigo);
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_DESCRIPCION", v => v.descripcion);
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_CATEGORIA", v => Nombre(categorias, v.id_categoria));
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_MARCA", v => Nombre(marcas, v.id_marca));
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_UNIDAD", v => Nombre(unidades, v.id_unidad_medida));
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_PRECIO", v => v.precio_unitario.ToString("N2", ar));
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_REPOSICION", v => v.punto_reposicion.ToString("N2", ar));
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("ART_LBL_UBICACION", v => v.deposito_ubicacion ?? string.Empty);
            yield return Tuple.Create<string, Func<ArticuloHistorialBE_575_AV, string>>("HIS_CAMPO_ACTIVO", v => gestorIdioma.Traducir(v.activo ? "HIS_VAL_SI" : "HIS_VAL_NO"));
        }

        private void rbVersion_CheckedChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.EstadoBoton(btnRestaurar, VersionElegida() != null);
        }

        #endregion

        #region Restaurar version (CUS-017)

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            ArticuloHistorialBE_575_AV version = VersionElegida();
            if (version == null) { return; }

            string pregunta = string.Format(gestorIdioma.Traducir("HIS_MSG_CONFIRMA"), version.codigo, version.fecha.ToString("dd/MM/yyyy HH:mm"));
            if (MessageBox.Show(this, pregunta, gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                articuloBLL.RestaurarVersion(version, UsuarioActual());
                Buscar();
                string codigo = version.codigo;
                Avisar(() => string.Format(gestorIdioma.Traducir("HIS_MSG_RESTAURADO"), codigo), false);
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

        #endregion

        #region Apoyo

        private ArticuloHistorialBE_575_AV Seleccionado()
        {
            return dgvHistorial.SelectedRows.Count == 0 ? null : dgvHistorial.SelectedRows[0].Tag as ArticuloHistorialBE_575_AV;
        }

        private ArticuloHistorialBE_575_AV VersionElegida()
        {
            if (rbAnterior.Checked && anterior != null) { return anterior; }
            if (rbPosterior.Checked && posterior != null) { return posterior; }
            return null;
        }

        private static string Nombre(Dictionary<int, string> catalogo, int id)
        {
            string nombre;
            return catalogo.TryGetValue(id, out nombre) ? nombre : id.ToString(CultureInfo.InvariantCulture);
        }

        private string Operacion(string operacion)
        {
            return gestorIdioma.Traducir("HIS_OP_" + operacion);
        }

        private void Avisar(Func<string> mensaje, bool esError)
        {
            avisoTraducible = mensaje;
            lblMensaje.Text = mensaje();
            lblMensaje.ForeColor = esError ? EstiloConectAR_575_AV.Peligro : EstiloConectAR_575_AV.NavySuave;
        }

        private void Avisar(string mensaje, bool esError)
        {
            Avisar(() => mensaje, esError);
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
