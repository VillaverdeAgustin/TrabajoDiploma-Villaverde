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
    public partial class frmGestionArticulos_575_AV : Form, IObservadorIdioma
    {
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private ArticuloBE_575_AV articuloActual;

        private enum Modo { Consulta, Alta, Modificacion }
        private Modo modo = Modo.Consulta;

        public frmGestionArticulos_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            TextoTraducido_575_AV.Enum(dgvArticulos, "estado_articulo", "ART_EST_");
            TextoTraducido_575_AV.PrimeraOpcion(cmbBuscarCategoria, "COMUN_TODOS");
            TextoTraducido_575_AV.PrimeraOpcion(cmbBuscarMarca, "COMUN_TODOS");
        }

        private void frmGestionArticulos_Load(object sender, EventArgs e)
        {
            try
            {
                CargarCatalogos();
                Buscar();
                ModoConsulta();

                gestorIdioma.Suscribir(this);
                this.FormClosed += frmGestionArticulos_FormClosed;

                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.GestionArticulosAbierta);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void frmGestionArticulos_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("ART_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("ART_TITULO");
            gbBuscar.Text = gestorIdioma.Traducir("ART_GB_BUSQUEDA");
            lblDescripcion.Text = gestorIdioma.Traducir("ART_LBL_DESCRIPCION");
            lblCategoria.Text = gestorIdioma.Traducir("ART_LBL_CATEGORIA");
            lblMarca.Text = gestorIdioma.Traducir("ART_LBL_MARCA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            btnLimpiar.Text = gestorIdioma.Traducir("COMUN_LIMPIAR");

            gbDatos.Text = gestorIdioma.Traducir("ART_GB_DATOS");
            lblCodigo.Text = gestorIdioma.Traducir("ART_LBL_CODIGO");
            lblDescripcionArt.Text = gestorIdioma.Traducir("ART_LBL_DESCRIPCION");
            lblCategoriaArt.Text = gestorIdioma.Traducir("ART_LBL_CATEGORIA");
            lblMarcaArt.Text = gestorIdioma.Traducir("ART_LBL_MARCA");
            lblUnidad.Text = gestorIdioma.Traducir("ART_LBL_UNIDAD");
            lblPrecio.Text = gestorIdioma.Traducir("ART_LBL_PRECIO");
            lblStock.Text = gestorIdioma.Traducir("ART_LBL_DISPONIBLE");
            lblReposicion.Text = gestorIdioma.Traducir("ART_LBL_REPOSICION");
            lblUbicacion.Text = gestorIdioma.Traducir("ART_LBL_UBICACION");

            lblOpciones.Text = gestorIdioma.Traducir("COMUN_OPCIONES");
            btnNuevo.Text = gestorIdioma.Traducir("COMUN_NUEVO");
            btnModificar.Text = gestorIdioma.Traducir("COMUN_MODIFICAR");
            btnEliminar.Text = gestorIdioma.Traducir("COMUN_ELIMINAR");
            btnGuardar.Text = gestorIdioma.Traducir("COMUN_GUARDAR");
            btnCancelar.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
            TextoTraducido_575_AV.Refrescar(cmbBuscarCategoria);
            TextoTraducido_575_AV.Refrescar(cmbBuscarMarca);
        }

        private void TraducirColumnas()
        {
            if (dgvArticulos.Columns.Count == 0) { return; }

            dgvArticulos.Columns["codigo"].HeaderText = gestorIdioma.Traducir("ART_COL_CODIGO");
            dgvArticulos.Columns["descripcion"].HeaderText = gestorIdioma.Traducir("ART_COL_DESCRIPCION");
            dgvArticulos.Columns["nombre_categoria"].HeaderText = gestorIdioma.Traducir("ART_COL_CATEGORIA");
            dgvArticulos.Columns["nombre_marca"].HeaderText = gestorIdioma.Traducir("ART_COL_MARCA");
            dgvArticulos.Columns["precio_unitario"].HeaderText = gestorIdioma.Traducir("ART_COL_PRECIO");
            dgvArticulos.Columns["cantidad_disponible"].HeaderText = gestorIdioma.Traducir("ART_COL_DISPONIBLE");
            dgvArticulos.Columns["abreviatura_unidad"].HeaderText = gestorIdioma.Traducir("ART_COL_UNIDAD");
            dgvArticulos.Columns["estado_articulo"].HeaderText = gestorIdioma.Traducir("ART_COL_ESTADO");
            dgvArticulos.Columns["deposito_ubicacion"].HeaderText = gestorIdioma.Traducir("ART_COL_UBICACION");
            dgvArticulos.Invalidate();
        }

        #endregion

        #region Busqueda y grilla

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarDescripcion.Clear();
            cmbBuscarCategoria.SelectedIndex = 0;
            cmbBuscarMarca.SelectedIndex = 0;
            Buscar();
        }

        private void Buscar()
        {
            try
            {
                int? idCategoria = IdSeleccionado(cmbBuscarCategoria);
                int? idMarca = IdSeleccionado(cmbBuscarMarca);

                List<ArticuloBE_575_AV> articulos = articuloBLL.BuscarArticulos(
                    txtBuscarDescripcion.Text, idCategoria, idMarca, null);

                dgvArticulos.DataSource = articulos;
                ConfigurarGrilla();

                Avisar(() => string.Format(gestorIdioma.Traducir("ART_MSG_ENCONTRADOS"),
                    articulos.Count), false);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void ConfigurarGrilla()
        {
            if (dgvArticulos.Columns.Count == 0) { return; }

            foreach (DataGridViewColumn columna in dgvArticulos.Columns)
            {
                columna.Visible = false;
            }

            string[] visibles = { "codigo", "descripcion", "nombre_categoria", "nombre_marca",
                                  "precio_unitario", "cantidad_disponible", "abreviatura_unidad",
                                  "estado_articulo", "deposito_ubicacion" };

            for (int i = 0; i < visibles.Length; i++)
            {
                dgvArticulos.Columns[visibles[i]].Visible = true;
                dgvArticulos.Columns[visibles[i]].DisplayIndex = i;
            }

            dgvArticulos.Columns["descripcion"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            dgvArticulos.Columns["precio_unitario"].DefaultCellStyle.Format = "N2";
            dgvArticulos.Columns["precio_unitario"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvArticulos.Columns["cantidad_disponible"].DefaultCellStyle.Format = "N2";
            dgvArticulos.Columns["cantidad_disponible"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            TraducirColumnas();
            ResaltarFaltantes();
        }

        private void ResaltarFaltantes()
        {
            foreach (DataGridViewRow fila in dgvArticulos.Rows)
            {
                ArticuloBE_575_AV articulo = fila.DataBoundItem as ArticuloBE_575_AV;
                if (articulo == null) { continue; }

                if (!articulo.se_puede_vender)
                {
                    fila.DefaultCellStyle.ForeColor = EstiloConectAR_575_AV.Peligro;
                }
                else if (articulo.bajo_punto_reposicion)
                {
                    fila.DefaultCellStyle.ForeColor = EstiloConectAR_575_AV.Cian;
                }
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (modo != Modo.Consulta) { return; }
            if (dgvArticulos.CurrentRow == null) { return; }

            articuloActual = dgvArticulos.CurrentRow.DataBoundItem as ArticuloBE_575_AV;
            MostrarArticulo(articuloActual);
        }

        #endregion

        #region Alta, modificacion y baja

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            articuloActual = null;
            LimpiarDatos();
            ModoEdicion(Modo.Alta);
            txtCodigo.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (articuloActual == null)
            {
                Avisar(() => gestorIdioma.Traducir("ART_MSG_ELEGIR"), true);
                return;
            }

            ModoEdicion(Modo.Modificacion);
            txtDescripcion.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (articuloActual == null)
            {
                Avisar(() => gestorIdioma.Traducir("ART_MSG_ELEGIR"), true);
                return;
            }

            if (MessageBox.Show(
                    string.Format(gestorIdioma.Traducir("ART_MSG_CONFIRMA_BAJA"), articuloActual.codigo),
                    gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                articuloBLL.EliminarArticulo(articuloActual.id_articulo, UsuarioActual());
                Buscar();
                ModoConsulta();
                Avisar(() => gestorIdioma.Traducir("ART_MSG_BAJA_OK"), false);
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                ArticuloBE_575_AV articulo = LeerDatos();

                if (modo == Modo.Alta)
                {
                    articuloBLL.CrearArticulo(articulo, UsuarioActual());
                    Avisar(() => gestorIdioma.Traducir("ART_MSG_ALTA_OK"), false);
                }
                else
                {
                    articulo.id_articulo = articuloActual.id_articulo;
                    articulo.cantidad_disponible = articuloActual.cantidad_disponible;
                    articulo.estado_articulo = articuloActual.estado_articulo;
                    articulo.activo = articuloActual.activo;

                    articuloBLL.ActualizarArticulo(articulo, UsuarioActual());
                    Avisar(() => gestorIdioma.Traducir("ART_MSG_MOD_OK"), false);
                }

                Buscar();
                ModoConsulta();
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MostrarArticulo(articuloActual);
            ModoConsulta();
            Avisar(string.Empty, false);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Estado del formulario

        private void ModoConsulta()
        {
            modo = Modo.Consulta;

            HabilitarDatos(false);
            dgvArticulos.Enabled = true;
            gbBuscar.Enabled = true;

            EstiloConectAR_575_AV.EstadoBoton(btnNuevo, true);
            EstiloConectAR_575_AV.EstadoBoton(btnModificar, articuloActual != null);
            EstiloConectAR_575_AV.EstadoBoton(btnEliminar, articuloActual != null);
            EstiloConectAR_575_AV.EstadoBoton(btnGuardar, false);
            EstiloConectAR_575_AV.EstadoBoton(btnCancelar, false);
            EstiloConectAR_575_AV.EstadoBoton(btnSalir, true);
        }

        private void ModoEdicion(Modo nuevo)
        {
            modo = nuevo;

            HabilitarDatos(true);
            dgvArticulos.Enabled = false;
            gbBuscar.Enabled = false;

            txtStock.ReadOnly = true;
            EstiloConectAR_575_AV.CampoBloqueado(txtStock);

            EstiloConectAR_575_AV.EstadoBoton(btnNuevo, false);
            EstiloConectAR_575_AV.EstadoBoton(btnModificar, false);
            EstiloConectAR_575_AV.EstadoBoton(btnEliminar, false);
            EstiloConectAR_575_AV.EstadoBoton(btnGuardar, true);
            EstiloConectAR_575_AV.EstadoBoton(btnCancelar, true);
            EstiloConectAR_575_AV.EstadoBoton(btnSalir, false);
        }

        private void HabilitarDatos(bool habilitado)
        {
            foreach (Control control in gbDatos.Controls)
            {
                TextBox caja = control as TextBox;
                if (caja != null)
                {
                    caja.ReadOnly = !habilitado;
                    if (habilitado) { EstiloConectAR_575_AV.CampoEditable(caja); }
                    else { EstiloConectAR_575_AV.CampoBloqueado(caja); }
                }

                ComboBox combo = control as ComboBox;
                if (combo != null) { combo.Enabled = habilitado; }
            }
        }

        #endregion

        #region Carga y lectura de datos

        private void CargarCatalogos()
        {
            CargarCombo(cmbCategoria, articuloBLL.ListarCategorias()
                .Select(c => new Opcion(c.id_categoria, c.nombre)).ToList(), false);
            CargarCombo(cmbMarca, articuloBLL.ListarMarcas()
                .Select(m => new Opcion(m.id_marca, m.nombre)).ToList(), false);
            CargarCombo(cmbUnidad, articuloBLL.ListarUnidadesMedida()
                .Select(u => new Opcion(u.id_unidad_medida, u.nombre)).ToList(), false);

            CargarCombo(cmbBuscarCategoria, articuloBLL.ListarCategorias()
                .Select(c => new Opcion(c.id_categoria, c.nombre)).ToList(), true);
            CargarCombo(cmbBuscarMarca, articuloBLL.ListarMarcas()
                .Select(m => new Opcion(m.id_marca, m.nombre)).ToList(), true);
        }

        private void CargarCombo(ComboBox combo, List<Opcion> opciones, bool conTodos)
        {
            if (conTodos)
            {
                opciones.Insert(0, new Opcion(0, gestorIdioma.Traducir("COMUN_TODOS")));
            }

            combo.DataSource = opciones;
            combo.DisplayMember = "Texto";
            combo.ValueMember = "Id";
            combo.SelectedIndex = 0;
        }

        private static int? IdSeleccionado(ComboBox combo)
        {
            Opcion opcion = combo.SelectedItem as Opcion;
            if (opcion == null || opcion.Id == 0) { return null; }
            return opcion.Id;
        }

        private void MostrarArticulo(ArticuloBE_575_AV articulo)
        {
            if (articulo == null) { LimpiarDatos(); return; }

            txtCodigo.Text = articulo.codigo;
            txtDescripcion.Text = articulo.descripcion;
            cmbCategoria.SelectedValue = articulo.id_categoria;
            cmbMarca.SelectedValue = articulo.id_marca;
            cmbUnidad.SelectedValue = articulo.id_unidad_medida;
            txtPrecio.Text = articulo.precio_unitario.ToString("N2", CultureInfo.CurrentCulture);
            txtStock.Text = articulo.cantidad_disponible.ToString("N2", CultureInfo.CurrentCulture);
            txtReposicion.Text = articulo.punto_reposicion.ToString("N2", CultureInfo.CurrentCulture);
            txtUbicacion.Text = articulo.deposito_ubicacion;

            if (modo == Modo.Consulta)
            {
                EstiloConectAR_575_AV.EstadoBoton(btnModificar, true);
                EstiloConectAR_575_AV.EstadoBoton(btnEliminar, true);
            }
        }

        private void LimpiarDatos()
        {
            txtCodigo.Clear();
            txtDescripcion.Clear();
            txtPrecio.Text = "0,00";
            txtStock.Text = "0,00";
            txtReposicion.Text = "0,00";
            txtUbicacion.Clear();

            if (cmbCategoria.Items.Count > 0) { cmbCategoria.SelectedIndex = 0; }
            if (cmbMarca.Items.Count > 0) { cmbMarca.SelectedIndex = 0; }
            if (cmbUnidad.Items.Count > 0) { cmbUnidad.SelectedIndex = 0; }
        }

        private ArticuloBE_575_AV LeerDatos()
        {
            return new ArticuloBE_575_AV
            {
                codigo = txtCodigo.Text.Trim(),
                descripcion = txtDescripcion.Text.Trim(),
                id_categoria = ValorCombo(cmbCategoria),
                id_marca = ValorCombo(cmbMarca),
                id_unidad_medida = ValorCombo(cmbUnidad),
                precio_unitario = LeerDecimal(txtPrecio, "ART_LBL_PRECIO"),
                punto_reposicion = LeerDecimal(txtReposicion, "ART_LBL_REPOSICION"),
                deposito_ubicacion = txtUbicacion.Text.Trim()
            };
        }

        private static int ValorCombo(ComboBox combo)
        {
            Opcion opcion = combo.SelectedItem as Opcion;
            return opcion == null ? 0 : opcion.Id;
        }

        private decimal LeerDecimal(TextBox caja, string claveCampo)
        {
            decimal valor;
            if (!decimal.TryParse(caja.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
            {
                throw new InvalidOperationException(string.Format(
                    gestorIdioma.Traducir("COMUN_MSG_NUMERO_INVALIDO"),
                    gestorIdioma.Traducir(claveCampo)));
            }
            return valor;
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

        private class Opcion
        {
            public Opcion(int id, string texto) { Id = id; Texto = texto; }

            public int Id { get; private set; }
            public string Texto { get; private set; }

            public override string ToString() { return Texto; }
        }

        #endregion
    }
}
