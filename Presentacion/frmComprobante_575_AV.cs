using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmComprobante_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly ComprobanteBLL_575_AV comprobanteBLL = new ComprobanteBLL_575_AV();
        private readonly int idComprobante;
        private ComprobanteBE_575_AV comprobante;

        public frmComprobante_575_AV() : this(0) { }

        public frmComprobante_575_AV(int idComprobante)
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "consultar_comprobantes");
            this.idComprobante = idComprobante;
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnCerrar);
            lblEmpresa.Font = EstiloConectAR_575_AV.FuenteTitulo;
            lblTipo.Font = new Font(EstiloConectAR_575_AV.FuenteTitulo.FontFamily, 26F, FontStyle.Bold);
            lblTitulo.Font = new Font(EstiloConectAR_575_AV.FuenteTitulo.FontFamily, 13F, FontStyle.Bold);
            lblTotal.Font = new Font(EstiloConectAR_575_AV.FuenteTitulo.FontFamily, 11F, FontStyle.Bold);
            lblTotalValor.Font = lblTotal.Font;
            lblLeyenda.Font = new Font(EstiloConectAR_575_AV.FuenteBase, FontStyle.Italic);
            lblTipoTitulo.Font = EstiloConectAR_575_AV.FuenteBajada;
            lblEmpresaDatos.Font = EstiloConectAR_575_AV.FuenteBajada;
        }

        private void frmComprobante_Load(object sender, EventArgs e)
        {
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
            try
            {
                comprobante = comprobanteBLL.ObtenerComprobante(idComprobante);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            gestorIdioma.Suscribir(this);
            if (comprobante == null) { BeginInvoke(new Action(Close)); }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("COMPR_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("COMPR_TITULO");
            lblEmpresa.Text = gestorIdioma.Traducir("COMPR_LBL_EMPRESA");
            lblEmpresaDatos.Text = gestorIdioma.Traducir("COMPR_LBL_EMPRESA_DATOS");
            lblTipoTitulo.Text = gestorIdioma.Traducir("COMPR_LBL_TIPO");
            lblNumero.Text = gestorIdioma.Traducir("COMPR_LBL_NUMERO");
            lblFecha.Text = gestorIdioma.Traducir("COMPR_LBL_FECHA");
            lblEstado.Text = gestorIdioma.Traducir("COMPR_LBL_ESTADO");
            lblOrden.Text = gestorIdioma.Traducir("COMPR_LBL_ORDEN");
            gbCliente.Text = gestorIdioma.Traducir("COMPR_GB_CLIENTE");
            lblCliente.Text = gestorIdioma.Traducir("COMPR_LBL_CLIENTE");
            lblDocumento.Text = gestorIdioma.Traducir("COMPR_LBL_DOCUMENTO");
            lblCondicion.Text = gestorIdioma.Traducir("COMPR_LBL_CONDICION");
            gbDetalle.Text = gestorIdioma.Traducir("COMPR_GB_DETALLE");
            lblNeto.Text = gestorIdioma.Traducir("COMPR_LBL_NETO");
            lblIva.Text = gestorIdioma.Traducir("COMPR_LBL_IVA");
            lblTotal.Text = gestorIdioma.Traducir("COMPR_LBL_TOTAL");
            gbPagos.Text = gestorIdioma.Traducir("COMPR_GB_PAGOS");
            gbTransparencia.Text = gestorIdioma.Traducir("COMPR_GB_TRANSPARENCIA");
            lblIvaContenido.Text = gestorIdioma.Traducir("COMPR_LBL_IVA_CONTENIDO");
            lblOtrosImpuestos.Text = gestorIdioma.Traducir("COMPR_LBL_OTROS_IMPUESTOS");
            lblLeyenda.Text = gestorIdioma.Traducir("COMPR_LBL_LEYENDA");
            btnCerrar.Text = gestorIdioma.Traducir("COMPR_BTN_CERRAR");
            MostrarComprobante();
        }

        #endregion

        private void MostrarComprobante()
        {
            gbTransparencia.Visible = comprobante != null && comprobante.es_consumidor_final;
            if (comprobante == null) { return; }

            lblTipo.Text = comprobante.tipo_comprobante;
            lblNumeroValor.Text = comprobante.nro_comprobante;
            lblFechaValor.Text = comprobante.fecha_emision.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            lblEstadoValor.Text = gestorIdioma.Traducir("COMPR_EST_" + comprobante.estado);
            lblOrdenValor.Text = comprobante.nro_orden_pago;

            if (comprobante.cliente != null)
            {
                lblClienteValor.Text = comprobante.cliente.nombre_para_mostrar;
                lblDocumentoValor.Text = comprobante.cliente.documento_para_mostrar;
                lblCondicionValor.Text = gestorIdioma.Traducir("CLI_IVA_" + comprobante.cliente.condicion_iva);
            }

            lblNetoValor.Text = Importe(comprobante.neto);
            lblIvaValor.Text = Importe(comprobante.iva);
            lblTotalValor.Text = Importe(comprobante.precio_total);
            lblIvaContenidoValor.Text = Importe(comprobante.iva);
            lblOtrosImpuestosValor.Text = Importe(comprobante.otros_impuestos_indirectos);

            if (dgvDetalle.DataSource == null)
            {
                dgvDetalle.DataSource = comprobante.detalle;
                EstiloConectAR_575_AV.MostrarColumnas(dgvDetalle,
                    "codigo_articulo", "descripcion_articulo", "cantidad", "abreviatura_unidad", "precio_unitario", "subtotal");
                EstiloConectAR_575_AV.FormatoNumero(dgvDetalle, "cantidad", "precio_unitario", "subtotal");
                EstiloConectAR_575_AV.ColumnaElastica(dgvDetalle, "descripcion_articulo");

                dgvPagos.DataSource = comprobante.pagos;
                EstiloConectAR_575_AV.MostrarColumnas(dgvPagos, "nombre_medio_pago", "monto");
                EstiloConectAR_575_AV.FormatoNumero(dgvPagos, "monto");
                EstiloConectAR_575_AV.ColumnaElastica(dgvPagos, "nombre_medio_pago");
            }

            Titulo(dgvDetalle, "codigo_articulo", "ART_COL_CODIGO");
            Titulo(dgvDetalle, "descripcion_articulo", "ART_COL_DESCRIPCION");
            Titulo(dgvDetalle, "cantidad", "VTA_COL_CANTIDAD");
            Titulo(dgvDetalle, "abreviatura_unidad", "ART_COL_UNIDAD");
            Titulo(dgvDetalle, "precio_unitario", "ORD_COL_PRECIO_UNITARIO");
            Titulo(dgvDetalle, "subtotal", "ORD_COL_SUBTOTAL");
            Titulo(dgvPagos, "nombre_medio_pago", "COMPR_COL_MEDIO");
            Titulo(dgvPagos, "monto", "COMPR_COL_IMPORTE");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null) { grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave); }
        }

        private static string Importe(decimal valor)
        {
            return "$ " + valor.ToString("N2", CultureInfo.CurrentCulture);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
