using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmConsultaComprobantes_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly ComprobanteBLL_575_AV comprobanteBLL = new ComprobanteBLL_575_AV();
        private Func<string> avisoTraducible;

        public frmConsultaComprobantes_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "consultar_comprobantes");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnBuscar);
            EstiloConectAR_575_AV.BotonSecundario(btnLimpiar);
            EstiloConectAR_575_AV.BotonPrimario(btnVer);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            TextoTraducido_575_AV.Enum(dgvComprobantes, "estado", "COMPR_EST_");
        }

        private void frmConsultaComprobantes_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            try { comprobanteBLL.RegistrarConsulta(UsuarioActual()); }
            catch (Exception) { Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true); }

            Limpiar();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("CCOMP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("CCOMP_TITULO");
            gbBusqueda.Text = gestorIdioma.Traducir("CCOMP_GB_BUSQUEDA");
            lblNumero.Text = gestorIdioma.Traducir("CCOMP_LBL_NUMERO");
            lblCliente.Text = gestorIdioma.Traducir("CCOMP_LBL_CLIENTE");
            chkFecha.Text = gestorIdioma.Traducir("CCOMP_CHK_FECHA");
            lblDesde.Text = gestorIdioma.Traducir("COMUN_DESDE");
            lblHasta.Text = gestorIdioma.Traducir("COMUN_HASTA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            btnLimpiar.Text = gestorIdioma.Traducir("COMUN_LIMPIAR");
            gbResultados.Text = gestorIdioma.Traducir("CCOMP_GB_RESULTADOS");
            lblAyuda.Text = gestorIdioma.Traducir("CCOMP_LBL_AYUDA");
            btnVer.Text = gestorIdioma.Traducir("CCOMP_BTN_VER");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
            dgvComprobantes.Invalidate();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo("nro_comprobante", "CCOMP_COL_NUMERO");
            Titulo("tipo_comprobante", "CCOMP_COL_TIPO");
            Titulo("fecha_emision", "COMUN_FECHA");
            Titulo("nombre_cliente", "CCOMP_COL_CLIENTE");
            Titulo("precio_total", "COMUN_TOTAL");
            Titulo("estado", "CCOMP_COL_ESTADO");
        }

        private void Titulo(string columna, string clave)
        {
            if (dgvComprobantes.Columns[columna] != null)
            {
                dgvComprobantes.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Busqueda

        private void Buscar()
        {
            try
            {
                List<ComprobanteBE_575_AV> comprobantes = comprobanteBLL.BuscarComprobantes(
                    txtNumero.Text, txtCliente.Text,
                    chkFecha.Checked ? dtpDesde.Value : (DateTime?)null,
                    chkFecha.Checked ? dtpHasta.Value : (DateTime?)null);

                dgvComprobantes.DataSource = comprobantes;
                EstiloConectAR_575_AV.MostrarColumnas(dgvComprobantes,
                    "nro_comprobante", "tipo_comprobante", "fecha_emision", "nombre_cliente", "precio_total", "estado");
                EstiloConectAR_575_AV.FormatoNumero(dgvComprobantes, "precio_total");
                EstiloConectAR_575_AV.ColumnaElastica(dgvComprobantes, "nombre_cliente");
                TraducirColumnas();

                int cantidad = comprobantes.Count;
                Avisar(() => string.Format(gestorIdioma.Traducir("CCOMP_MSG_ENCONTRADOS"), cantidad), false);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void Limpiar()
        {
            txtNumero.Clear();
            txtCliente.Clear();
            chkFecha.Checked = false;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            HabilitarFechas();
            Buscar();
        }

        private void HabilitarFechas()
        {
            dtpDesde.Enabled = chkFecha.Checked;
            dtpHasta.Enabled = chkFecha.Checked;
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();

        private void btnLimpiar_Click(object sender, EventArgs e) => Limpiar();

        private void chkFecha_CheckedChanged(object sender, EventArgs e) => HabilitarFechas();

        #endregion

        #region Apertura del comprobante

        private void dgvComprobantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) { AbrirSeleccionado(); }
        }

        private void btnVer_Click(object sender, EventArgs e) => AbrirSeleccionado();

        private void AbrirSeleccionado()
        {
            ComprobanteBE_575_AV seleccionado = dgvComprobantes.CurrentRow == null
                ? null : dgvComprobantes.CurrentRow.DataBoundItem as ComprobanteBE_575_AV;
            if (seleccionado == null)
            {
                Avisar(() => gestorIdioma.Traducir("CCOMP_MSG_ELEGIR"), true);
                return;
            }

            using (frmComprobante_575_AV frm = new frmComprobante_575_AV(seleccionado.id_comprobante))
            {
                frm.ShowDialog(this);
            }
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
