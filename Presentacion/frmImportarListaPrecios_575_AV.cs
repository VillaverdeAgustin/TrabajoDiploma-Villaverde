using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmImportarListaPrecios_575_AV : Form, IObservadorIdioma
    {
        private static readonly Color FondoRechazado = Color.FromArgb(251, 234, 232);
        private const int ANCHO_DESCRIPCION = 180;

        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private ListaPrecioBLL_575_AV listaPrecioBLL = new ListaPrecioBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private ResumenImportacion_575_AV resumenActual;
        private string rutaArchivo;
        private bool cargando;

        public frmImportarListaPrecios_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnNuevoProveedor);
            EstiloConectAR_575_AV.BotonSecundario(btnExaminar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmImportarListaPrecios_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            cargando = true;
            cmbMoneda.DataSource = new List<string> { "ARS", "USD" };
            dtpVigencia.Value = DateTime.Today;
            cargando = false;

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.ImportarListaAbierta);
                CargarProveedores(0);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
            Invalidar();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("LPR_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("LPR_TITULO");
            gbOrigen.Text = gestorIdioma.Traducir("LPR_GB_ORIGEN");
            lblProveedor.Text = gestorIdioma.Traducir("LPR_LBL_PROVEEDOR");
            btnNuevoProveedor.Text = gestorIdioma.Traducir("LPR_BTN_NUEVO_PROVEEDOR");
            lblVigencia.Text = gestorIdioma.Traducir("LPR_LBL_VIGENCIA");
            lblMoneda.Text = gestorIdioma.Traducir("LPR_LBL_MONEDA");
            lblArchivo.Text = gestorIdioma.Traducir("LPR_LBL_ARCHIVO");
            btnExaminar.Text = gestorIdioma.Traducir("LPR_BTN_EXAMINAR");
            btnPrevisualizar.Text = gestorIdioma.Traducir("LPR_BTN_PREVISUALIZAR");
            gbRenglones.Text = gestorIdioma.Traducir("LPR_GB_RENGLONES");
            btnImportar.Text = gestorIdioma.Traducir("LPR_BTN_IMPORTAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            if (resumenActual != null) { MostrarResumen(resumenActual); }
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo("renglon", "LPR_COL_RENGLON");
            Titulo("codigo", "LPR_COL_CODIGO");
            Titulo("descripcion", "LPR_COL_DESCRIPCION");
            Titulo("precio", "LPR_COL_PRECIO");
            Titulo("unidad", "LPR_COL_UNIDAD");
            Titulo("plazo", "LPR_COL_PLAZO");
            Titulo("estado", "LPR_COL_ESTADO");
            Titulo("observacion", "LPR_COL_OBSERVACION");
        }

        private void Titulo(string columna, string clave)
        {
            if (dgvRenglones.Columns[columna] != null)
            {
                dgvRenglones.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Origen de la lista

        private void CargarProveedores(int id_seleccionado)
        {
            cargando = true;
            List<ProveedorBE_575_AV> proveedores = proveedorBLL.ListarProveedores().Where(p => p.activo).ToList();
            cmbProveedor.DataSource = proveedores;
            cmbProveedor.DisplayMember = "razon_social";
            cmbProveedor.ValueMember = "id_proveedor";
            if (id_seleccionado > 0) { cmbProveedor.SelectedValue = id_seleccionado; }
            cargando = false;
        }

        private void btnNuevoProveedor_Click(object sender, EventArgs e)
        {
            using (frmProveedor_575_AV alta = new frmProveedor_575_AV())
            {
                alta.StartPosition = FormStartPosition.CenterParent;
                alta.ShowDialog(this);
                try
                {
                    CargarProveedores(alta.ProveedorRegistrado != null ? alta.ProveedorRegistrado.id_proveedor : 0);
                }
                catch (Exception)
                {
                    Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
                }
            }
            Invalidar();
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Filter = gestorIdioma.Traducir("LPR_FILTRO");
                if (dialogo.ShowDialog(this) == DialogResult.OK)
                {
                    rutaArchivo = dialogo.FileName;
                    txtArchivo.Text = System.IO.Path.GetFileName(rutaArchivo);
                }
            }
        }

        private void Origen_Cambio(object sender, EventArgs e)
        {
            if (!cargando) { Invalidar(); }
        }

        #endregion

        #region Previsualizar e importar

        private void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            Invalidar();
            try
            {
                ResumenImportacion_575_AV resumen = listaPrecioBLL.Previsualizar(rutaArchivo, ProveedorElegido());
                resumenActual = resumen;
                MostrarResumen(resumen);
                EstiloConectAR_575_AV.EstadoBoton(btnImportar, resumen.validos.Count > 0);
                Avisar(string.Empty, false);
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

        private void btnImportar_Click(object sender, EventArgs e)
        {
            if (resumenActual == null) { return; }

            ListaPrecioBE_575_AV lista = new ListaPrecioBE_575_AV
            {
                id_proveedor = ProveedorElegido(),
                archivo = rutaArchivo,
                fecha_vigencia = dtpVigencia.Value.Date,
                moneda = cmbMoneda.SelectedItem as string
            };

            try
            {
                ResumenImportacion_575_AV resultado = listaPrecioBLL.Importar(lista, resumenActual.renglones, UsuarioActual());
                string proveedor = cmbProveedor.SelectedItem is ProveedorBE_575_AV
                    ? ((ProveedorBE_575_AV)cmbProveedor.SelectedItem).razon_social : string.Empty;
                Func<string> mensaje = () => string.Format(gestorIdioma.Traducir("LPR_MSG_IMPORTADA"),
                    proveedor, resultado.actualizados, resultado.incorporados, resultado.rechazados);

                MessageBox.Show(mensaje(), gestorIdioma.Traducir("LPR_TITULO"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                EstiloConectAR_575_AV.EstadoBoton(btnImportar, false);
                resumenActual = null;
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

        private void MostrarResumen(ResumenImportacion_575_AV resumen)
        {
            dgvRenglones.DataSource = resumen.renglones.Select(r => new Renglon
            {
                renglon = r.numero_renglon,
                codigo = r.codigo_proveedor,
                descripcion = r.descripcion,
                precio = r.resultado == ResultadoImportacion_575_AV.Rechazado && r.precio_lista <= 0 ? (decimal?)null : r.precio_lista,
                unidad = r.unidad_venta,
                plazo = r.plazo_entrega,
                resultado = r.resultado,
                estado = gestorIdioma.Traducir("LPR_EST_" + r.resultado.ToString().ToUpperInvariant()),
                observacion = r.resultado == ResultadoImportacion_575_AV.Rechazado ? r.motivo
                    : r.resultado == ResultadoImportacion_575_AV.Incorporado ? gestorIdioma.Traducir("LPR_OBS_NUEVO")
                    : r.descripcion_articulo
            }).ToList();

            EstiloConectAR_575_AV.MostrarColumnas(dgvRenglones,
                "renglon", "estado", "observacion", "codigo", "descripcion", "precio", "unidad", "plazo");
            EstiloConectAR_575_AV.FormatoNumero(dgvRenglones, "precio");
            EstiloConectAR_575_AV.ColumnaElastica(dgvRenglones, "descripcion");
            dgvRenglones.Columns["descripcion"].MinimumWidth = ANCHO_DESCRIPCION;
            TraducirColumnas();

            lblResumen.Text = string.Format(gestorIdioma.Traducir("LPR_LBL_RESUMEN"),
                resumen.actualizados, resumen.incorporados, resumen.rechazados);
        }

        private void dgvRenglones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Renglon renglon = dgvRenglones.Rows[e.RowIndex].DataBoundItem as Renglon;
            if (renglon != null && renglon.resultado == ResultadoImportacion_575_AV.Rechazado)
            {
                e.CellStyle.ForeColor = EstiloConectAR_575_AV.Peligro;
                e.CellStyle.BackColor = FondoRechazado;
            }
        }

        private void Invalidar()
        {
            resumenActual = null;
            dgvRenglones.DataSource = null;
            lblResumen.Text = string.Empty;
            EstiloConectAR_575_AV.EstadoBoton(btnImportar, false);
        }

        private int ProveedorElegido()
        {
            return cmbProveedor.SelectedValue is int ? (int)cmbProveedor.SelectedValue : 0;
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

        public class Renglon
        {
            public int renglon { get; set; }
            public string codigo { get; set; }
            public string descripcion { get; set; }
            public decimal? precio { get; set; }
            public string unidad { get; set; }
            public int? plazo { get; set; }
            public ResultadoImportacion_575_AV resultado { get; set; }
            public string estado { get; set; }
            public string observacion { get; set; }
        }

        #endregion
    }
}
