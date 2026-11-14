using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // A03 - Serializacion de ordenes de compra a XML. La pantalla elige la orden y el archivo;
    // la BLL serializa, deserializa y verifica.
    public partial class frmSerializacion_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly SerializacionOrdenCompraBLL_575_AV serializacionBLL = new SerializacionOrdenCompraBLL_575_AV();
        private Func<string> avisoTraducible;
        private ResultadoSerializacion_575_AV serializado;
        private ResultadoSerializacion_575_AV deserializado;

        public frmSerializacion_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnSerializar);
            EstiloConectAR_575_AV.BotonPrimario(btnAbrir);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            tpSerializar.BackColor = EstiloConectAR_575_AV.Fondo;
            tpDeserializar.BackColor = EstiloConectAR_575_AV.Fondo;
            EstiloConectAR_575_AV.CampoBloqueado(txtHashSerializado);
            EstiloConectAR_575_AV.CampoBloqueado(txtHashDeserializado);

            EstiloConectAR_575_AV.FormatoNumero(dgvOrdenes, "colTotal");
            EstiloConectAR_575_AV.FormatoNumero(dgvRenglones, "colCantidad", "colPrecio", "colSubtotal");
            colFecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            colRenglones.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (DataGridView grilla in new[] { dgvOrdenes, dgvRenglones, dgvVerificacion, dgvVerificacionBase })
            {
                Compacta(grilla);
            }

            dgvOrdenes.CellFormatting += dgvOrdenes_CellFormatting;
            dgvVerificacion.CellFormatting += Verificacion_CellFormatting;
            dgvVerificacionBase.CellFormatting += Verificacion_CellFormatting;

            string documentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfdXml.InitialDirectory = documentos;
            sfdXml.DefaultExt = "xml";
            sfdXml.AddExtension = true;
            sfdXml.OverwritePrompt = true;
            ofdXml.InitialDirectory = documentos;
            ofdXml.CheckFileExists = true;
        }

        private void frmSerializacion_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
            CargarOrdenes();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("SER_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("SER_TITULO");
            tpSerializar.Text = gestorIdioma.Traducir("SER_TAB_SERIALIZAR");
            tpDeserializar.Text = gestorIdioma.Traducir("SER_TAB_DESERIALIZAR");
            gbOrdenes.Text = gestorIdioma.Traducir("SER_GB_ORDENES");
            btnSerializar.Text = gestorIdioma.Traducir("SER_BTN_SERIALIZAR");
            btnAbrir.Text = gestorIdioma.Traducir("SER_BTN_ABRIR");
            lblHashSerializado.Text = gestorIdioma.Traducir("SER_LBL_HASH");
            lblHashDeserializado.Text = gestorIdioma.Traducir("SER_LBL_HASH");
            gbVerificacion.Text = gestorIdioma.Traducir("SER_GB_VERIFICACION");
            gbVerificacionBase.Text = gestorIdioma.Traducir("SER_GB_VERIFICACION_BD");
            gbRecuperada.Text = gestorIdioma.Traducir("SER_GB_RECUPERADA");
            lblOrden.Text = gestorIdioma.Traducir("SER_LBL_ORDEN");
            lblProveedorRecuperado.Text = gestorIdioma.Traducir("SER_LBL_PROVEEDOR");
            lblEmision.Text = gestorIdioma.Traducir("SER_LBL_EMISION");
            lblEstadoRecuperado.Text = gestorIdioma.Traducir("SER_LBL_ESTADO");
            lblTotalRecuperado.Text = gestorIdioma.Traducir("SER_LBL_TOTAL");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            colNro.HeaderText = gestorIdioma.Traducir("SER_COL_NRO");
            colFecha.HeaderText = gestorIdioma.Traducir("COMUN_FECHA");
            colProveedor.HeaderText = gestorIdioma.Traducir("SER_COL_PROVEEDOR");
            colEstado.HeaderText = gestorIdioma.Traducir("SER_COL_ESTADO");
            colRenglones.HeaderText = gestorIdioma.Traducir("SER_COL_RENGLONES");
            colTotal.HeaderText = gestorIdioma.Traducir("COMUN_TOTAL");
            colCodigo.HeaderText = gestorIdioma.Traducir("SER_COL_CODIGO");
            colDescripcion.HeaderText = gestorIdioma.Traducir("SER_COL_DESCRIPCION");
            colCantidad.HeaderText = gestorIdioma.Traducir("SER_COL_CANTIDAD");
            colUnidad.HeaderText = gestorIdioma.Traducir("SER_COL_UNIDAD");
            colPrecio.HeaderText = gestorIdioma.Traducir("SER_COL_PRECIO");
            colSubtotal.HeaderText = gestorIdioma.Traducir("SER_COL_SUBTOTAL");
            foreach (string s in new[] { string.Empty, "Base" })
            {
                DataGridView grilla = s.Length == 0 ? dgvVerificacion : dgvVerificacionBase;
                grilla.Columns["colVerificacion" + s].HeaderText = gestorIdioma.Traducir("SER_COL_VERIFICACION");
                grilla.Columns["colResultado" + s].HeaderText = gestorIdioma.Traducir("SER_COL_RESULTADO");
                grilla.Columns["colDetalle" + s].HeaderText = gestorIdioma.Traducir("SER_COL_DETALLE");
            }

            sfdXml.Title = gestorIdioma.Traducir("SER_DLG_GUARDAR");
            sfdXml.Filter = gestorIdioma.Traducir("SER_FILTRO");
            ofdXml.Title = gestorIdioma.Traducir("SER_DLG_ABRIR");
            ofdXml.Filter = gestorIdioma.Traducir("SER_FILTRO");

            lblArchivoSerializado.Text = Archivo(serializado);
            lblArchivoDeserializado.Text = Archivo(deserializado);
            lblEstadoValor.Text = deserializado == null ? string.Empty : Estado(deserializado.documento.orden.estado);
            dgvOrdenes.Invalidate();
            dgvVerificacion.Invalidate();
            dgvVerificacionBase.Invalidate();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private string Archivo(ResultadoSerializacion_575_AV resultado)
        {
            return resultado == null ? string.Empty
                : string.Format(gestorIdioma.Traducir("SER_LBL_ARCHIVO"), Path.GetFileName(resultado.ruta));
        }

        #endregion

        #region Serializar

        private void CargarOrdenes()
        {
            try
            {
                List<OrdenCompraBE_575_AV> ordenes = serializacionBLL.ListarOrdenes();
                dgvOrdenes.DataSource = ordenes;
                if (ordenes.Count == 0)
                {
                    Avisar(() => gestorIdioma.Traducir("SER_MSG_SIN_ORDENES"), false);
                }
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void btnSerializar_Click(object sender, EventArgs e)
        {
            OrdenCompraBE_575_AV orden = OrdenSeleccionada();
            if (orden == null)
            {
                Avisar(() => gestorIdioma.Traducir("SER_MSG_ELEGIR"), true);
                return;
            }

            sfdXml.FileName = orden.nro_orden_compra + ".xml";
            if (sfdXml.ShowDialog(this) == DialogResult.OK)
            {
                Serializar(sfdXml.FileName);
            }
        }

        private void Serializar(string ruta)
        {
            OrdenCompraBE_575_AV orden = OrdenSeleccionada();
            if (orden == null)
            {
                Avisar(() => gestorIdioma.Traducir("SER_MSG_ELEGIR"), true);
                return;
            }

            try
            {
                serializado = serializacionBLL.Serializar(orden.id_orden_compra, ruta, UsuarioActual());
                dgvVerificacion.DataSource = serializado.verificaciones;
                dgvVerificacion.ClearSelection();
                txtHashSerializado.Text = serializado.hash;
                lblArchivoSerializado.Text = Archivo(serializado);

                string nro = orden.nro_orden_compra;
                bool correcto = serializado.correcto;
                Avisar(() => string.Format(gestorIdioma.Traducir(correcto ? "SER_MSG_SERIALIZADA" : "SER_MSG_SER_FALLA"), nro), !correcto);
            }
            catch (InvalidOperationException ex)
            {
                LimpiarSerializacion();
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
            }
            catch (Exception)
            {
                LimpiarSerializacion();
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void LimpiarSerializacion()
        {
            serializado = null;
            dgvVerificacion.DataSource = null;
            txtHashSerializado.Clear();
            lblArchivoSerializado.Text = string.Empty;
        }

        private OrdenCompraBE_575_AV OrdenSeleccionada()
        {
            DataGridViewRow fila = dgvOrdenes.SelectedRows.Count > 0 ? dgvOrdenes.SelectedRows[0] : dgvOrdenes.CurrentRow;
            return fila == null ? null : fila.DataBoundItem as OrdenCompraBE_575_AV;
        }

        #endregion

        #region Deserializar

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            if (ofdXml.ShowDialog(this) == DialogResult.OK)
            {
                Deserializar(ofdXml.FileName);
            }
        }

        private void Deserializar(string ruta)
        {
            try
            {
                deserializado = serializacionBLL.Deserializar(ruta, UsuarioActual());
                OrdenCompraBE_575_AV orden = deserializado.documento.orden;

                lblArchivoDeserializado.Text = Archivo(deserializado);
                txtHashDeserializado.Text = deserializado.hash;
                lblOrdenValor.Text = orden.nro_orden_compra;
                lblProveedorValor.Text = deserializado.documento.proveedor.razon_social;
                lblEmisionValor.Text = orden.fecha_emision.HasValue ? orden.fecha_emision.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                lblEstadoValor.Text = Estado(orden.estado);
                lblTotalValor.Text = orden.total.ToString("N2");
                dgvRenglones.DataSource = orden.detalle;
                dgvRenglones.ClearSelection();
                dgvVerificacionBase.DataSource = deserializado.verificaciones;
                dgvVerificacionBase.ClearSelection();

                string nro = orden.nro_orden_compra;
                bool correcto = deserializado.correcto;
                Avisar(() => string.Format(gestorIdioma.Traducir(correcto ? "SER_MSG_DESERIALIZADA" : "SER_MSG_DES_FALLA"), nro), !correcto);
            }
            catch (InvalidOperationException ex)
            {
                LimpiarDeserializacion();
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
            }
            catch (Exception)
            {
                LimpiarDeserializacion();
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void LimpiarDeserializacion()
        {
            deserializado = null;
            lblArchivoDeserializado.Text = string.Empty;
            txtHashDeserializado.Clear();
            lblOrdenValor.Text = string.Empty;
            lblProveedorValor.Text = string.Empty;
            lblEmisionValor.Text = string.Empty;
            lblEstadoValor.Text = string.Empty;
            lblTotalValor.Text = string.Empty;
            dgvRenglones.DataSource = null;
            dgvVerificacionBase.DataSource = null;
        }

        #endregion

        #region Formato de grillas

        private void dgvOrdenes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvOrdenes.Columns[e.ColumnIndex].Name == "colEstado" && e.Value is EstadoOrdenCompra_575_AV)
            {
                e.Value = Estado((EstadoOrdenCompra_575_AV)e.Value);
                e.FormattingApplied = true;
            }
        }

        // La descripcion se arma con la clave y sus argumentos para que siga al idioma activo
        private void Verificacion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grilla = (DataGridView)sender;
            if (e.RowIndex < 0) { return; }
            ResultadoVerificacion_575_AV verificacion = grilla.Rows[e.RowIndex].DataBoundItem as ResultadoVerificacion_575_AV;
            if (verificacion == null) { return; }

            string columna = grilla.Columns[e.ColumnIndex].Name;
            if (columna.StartsWith("colVerificacion"))
            {
                e.Value = string.Format(gestorIdioma.Traducir(verificacion.clave), verificacion.argumentos);
                e.FormattingApplied = true;
            }
            else if (columna.StartsWith("colResultado"))
            {
                e.Value = gestorIdioma.Traducir(verificacion.correcto ? "SER_RES_OK" : "SER_RES_FALLA");
                e.CellStyle.ForeColor = verificacion.correcto ? EstiloConectAR_575_AV.Exito : EstiloConectAR_575_AV.Peligro;
                e.CellStyle.Font = EstiloConectAR_575_AV.FuenteEtiqueta;
                e.FormattingApplied = true;
            }

            if (!verificacion.correcto)
            {
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(250, 235, 233);
            }
        }

        private string Estado(EstadoOrdenCompra_575_AV estado)
        {
            return gestorIdioma.Traducir("OC_EST_" + estado.ToString().ToUpperInvariant());
        }

        // Filas mas bajas que las del estilo general, para que entren todas las verificaciones sin desplazar
        private static void Compacta(DataGridView grilla)
        {
            grilla.RowTemplate.Height = 22;
            grilla.DefaultCellStyle.Padding = new Padding(6, 1, 6, 1);
            grilla.ColumnHeadersHeight = 30;
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
