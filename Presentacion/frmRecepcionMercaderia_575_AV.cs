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
    public partial class frmRecepcionMercaderia_575_AV : Form, IObservadorIdioma
    {
        private static readonly Color FondoEditable = Color.FromArgb(232, 246, 252);

        private OrdenCompraBLL_575_AV ordenCompraBLL = new OrdenCompraBLL_575_AV();
        private RecepcionBLL_575_AV recepcionBLL = new RecepcionBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        public frmRecepcionMercaderia_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "recepcion");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmRecepcionMercaderia_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.RecepcionAbierta);
                CargarOrdenes();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("RCP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("RCP_TITULO");
            gbOrdenes.Text = gestorIdioma.Traducir("RCP_GB_ORDENES");
            gbRenglones.Text = gestorIdioma.Traducir("RCP_GB_RENGLONES");
            lblRemito.Text = gestorIdioma.Traducir("RCP_LBL_REMITO");
            btnRegistrar.Text = gestorIdioma.Traducir("RCP_BTN_REGISTRAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo(dgvOrdenes, "nro", "RCP_COL_NRO");
            Titulo(dgvOrdenes, "proveedor", "CMP_COL_PROVEEDOR");
            Titulo(dgvOrdenes, "estado", "RCP_COL_ESTADO");
            Titulo(dgvOrdenes, "emitida", "RCP_COL_EMISION");
            Titulo(dgvOrdenes, "total", "RCP_COL_TOTAL");

            Titulo(dgvRenglones, "codigo", "ART_COL_CODIGO");
            Titulo(dgvRenglones, "descripcion", "ART_COL_DESCRIPCION");
            Titulo(dgvRenglones, "unidad", "ART_COL_UNIDAD");
            Titulo(dgvRenglones, "solicitada", "RCP_COL_SOLICITADA");
            Titulo(dgvRenglones, "recibida_antes", "RCP_COL_RECIBIDA_ANTES");
            Titulo(dgvRenglones, "pendiente", "RCP_COL_PENDIENTE");
            Titulo(dgvRenglones, "recibir", "RCP_COL_RECIBIR");
        }

        private void Titulo(DataGridView grilla, string columna, string clave)
        {
            if (grilla.Columns[columna] != null)
            {
                grilla.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Ordenes y renglones

        private void CargarOrdenes()
        {
            List<Orden> ordenes = ordenCompraBLL.ListarParaRecepcion().Select(o => new Orden
            {
                orden = o,
                nro = o.nro_orden_compra,
                proveedor = o.razon_social,
                estado = gestorIdioma.Traducir("OC_EST_" + o.estado.ToString().ToUpperInvariant()),
                emitida = o.fecha_emision,
                total = o.total
            }).ToList();

            dgvOrdenes.DataSource = ordenes;
            EstiloConectAR_575_AV.MostrarColumnas(dgvOrdenes, "nro", "proveedor", "estado", "emitida", "total");
            EstiloConectAR_575_AV.FormatoNumero(dgvOrdenes, "total");
            EstiloConectAR_575_AV.ColumnaElastica(dgvOrdenes, "proveedor");
            if (dgvOrdenes.Columns["emitida"] != null)
            {
                dgvOrdenes.Columns["emitida"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
            TraducirColumnas();

            if (ordenes.Count == 0)
            {
                Avisar(() => gestorIdioma.Traducir("RCP_MSG_SIN_ORDENES"), false);
                MostrarRenglones(null);
            }
        }

        private void dgvOrdenes_CurrentCellChanged(object sender, EventArgs e)
        {
            MostrarRenglones(OrdenSeleccionada());
        }

        private void MostrarRenglones(OrdenCompraBE_575_AV orden)
        {
            List<Renglon> renglones = orden == null ? new List<Renglon>() : orden.detalle.Select(d => new Renglon
            {
                id_articulo = d.id_articulo,
                codigo = d.codigo_articulo,
                descripcion = d.descripcion_articulo,
                unidad = d.abreviatura_unidad,
                solicitada = d.cantidad,
                recibida_antes = d.cantidad_recibida,
                pendiente = d.cantidad_pendiente,
                recibir = d.cantidad_pendiente
            }).ToList();

            dgvRenglones.DataSource = renglones;
            EstiloConectAR_575_AV.MostrarColumnas(dgvRenglones,
                "codigo", "descripcion", "unidad", "solicitada", "recibida_antes", "pendiente", "recibir");
            EstiloConectAR_575_AV.FormatoNumero(dgvRenglones, "solicitada", "recibida_antes", "pendiente", "recibir");
            EstiloConectAR_575_AV.ColumnaElastica(dgvRenglones, "descripcion");
            foreach (DataGridViewColumn columna in dgvRenglones.Columns)
            {
                columna.ReadOnly = columna.Name != "recibir";
            }
            if (dgvRenglones.Columns["recibir"] != null)
            {
                dgvRenglones.Columns["recibir"].DefaultCellStyle.BackColor = FondoEditable;
                dgvRenglones.Columns["recibir"].DefaultCellStyle.Font = EstiloConectAR_575_AV.FuenteEtiqueta;
            }
            TraducirColumnas();

            txtRemito.Text = string.Empty;
            EstiloConectAR_575_AV.EstadoBoton(btnRegistrar, orden != null);
        }

        private void dgvRenglones_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            Avisar(() => gestorIdioma.Traducir("RCP_ERR_CANTIDAD_INVALIDA"), true);
        }

        #endregion

        #region Registro

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            dgvRenglones.EndEdit();

            OrdenCompraBE_575_AV orden = OrdenSeleccionada();
            if (orden == null)
            {
                Avisar(() => gestorIdioma.Traducir("RCP_MSG_ELEGIR"), true);
                return;
            }

            try
            {
                List<DetalleRecepcionBE_575_AV> detalles = ((List<Renglon>)dgvRenglones.DataSource)
                    .Select(r => new DetalleRecepcionBE_575_AV { id_articulo = r.id_articulo, cantidad_recibida = r.recibir })
                    .ToList();

                RecepcionBE_575_AV recepcion = recepcionBLL.RegistrarRecepcion(orden,
                    new RecepcionBE_575_AV { remito_proveedor = txtRemito.Text }, detalles, UsuarioActual());

                CargarOrdenes();
                Avisar(() => string.IsNullOrEmpty(recepcion.diferencias)
                    ? gestorIdioma.Traducir("RCP_MSG_REGISTRADA")
                    : string.Format(gestorIdioma.Traducir("RCP_MSG_PARCIAL"), recepcion.diferencias), false);
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

        private OrdenCompraBE_575_AV OrdenSeleccionada()
        {
            Orden fila = dgvOrdenes.CurrentRow == null ? null : dgvOrdenes.CurrentRow.DataBoundItem as Orden;
            return fila == null ? null : fila.orden;
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

        public class Orden
        {
            public OrdenCompraBE_575_AV orden { get; set; }
            public string nro { get; set; }
            public string proveedor { get; set; }
            public string estado { get; set; }
            public DateTime? emitida { get; set; }
            public decimal total { get; set; }
        }

        public class Renglon
        {
            public int id_articulo { get; set; }
            public string codigo { get; set; }
            public string descripcion { get; set; }
            public string unidad { get; set; }
            public decimal solicitada { get; set; }
            public decimal recibida_antes { get; set; }
            public decimal pendiente { get; set; }
            public decimal recibir { get; set; }
        }

        #endregion
    }
}
