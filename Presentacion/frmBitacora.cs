using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmBitacora : Form, IObservadorIdioma
    {
        public frmBitacora()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        BitacoraBLL bitacora = new BitacoraBLL();
        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            dgvBitacora.DataSource = bitacora.ListarBitacora();
            TraducirColumnas();
            dgvBitacora.EnableHeadersVisualStyles = false;
            dgvBitacora.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBitacora.Font, FontStyle.Bold);
            dgvBitacora.ReadOnly = true;
            gestorIdioma.Suscribir(this);
            this.FormClosed += frmBitacora_FormClosed;
           cmbEvento.DataSource = Enum.GetValues(typeof(TipoAccion));
            LoadDefaultForm();
        }

        private void frmBitacora_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("BIT_TITULO");
            label1.Text = gestorIdioma.Traducir("BIT_LBL_TITULO");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
        }

        private void TraducirColumnas()
        {
            if (dgvBitacora.Columns.Count == 0) { return; }
            dgvBitacora.Columns[0].HeaderText = gestorIdioma.Traducir("BIT_COL_REGISTRO");
            dgvBitacora.Columns[1].HeaderText = gestorIdioma.Traducir("COMUN_USUARIO");
            dgvBitacora.Columns[2].HeaderText = gestorIdioma.Traducir("COMUN_ACCION");
            dgvBitacora.Columns[3].HeaderText = gestorIdioma.Traducir("COMUN_FECHA");
        }

        #endregion


        private void CargarDGV(List<EventoBE> ev)
        {
            dgvBitacora.DataSource = ev;
            dgvBitacora.Columns[0].HeaderText = "ID Registro";
            dgvBitacora.Columns[1].HeaderText = "Usuario";
            dgvBitacora.Columns[2].HeaderText = "Accion";
            dgvBitacora.Columns[3].HeaderText = "Fecha";
            dgvBitacora.EnableHeadersVisualStyles = false;
            dgvBitacora.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBitacora.Font, FontStyle.Bold);
            dgvBitacora.ReadOnly = true;
        }

        private void LoadDefaultForm()
        {
            try
            {
                CargarDGV(bitacora.ListarBitacora());
                dgvBitacora.Columns[0].HeaderText = "ID Registro";
                dgvBitacora.Columns[1].HeaderText = "Usuario";
                dgvBitacora.Columns[2].HeaderText = "Accion";
                dgvBitacora.Columns[3].HeaderText = "Fecha";
                dgvBitacora.EnableHeadersVisualStyles = false;
                dgvBitacora.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBitacora.Font, FontStyle.Bold);
                dgvBitacora.ReadOnly = true;

                dtpDesde.Value = DateTime.Today.AddDays(-30);
                dtpHasta.Value = DateTime.Today;

                cmbEvento.SelectedIndex = -1;
                txtUsuario.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Control control = btnSalir.Parent;
            frmMenu.opcActivo.BackColor = EstiloConectAR_575_AV.Navy;
            this.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LoadDefaultForm();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string us;
                TipoAccion? acc;
                DateTime fIni = dtpDesde.Value;
                DateTime fFin = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

                if (string.IsNullOrEmpty(txtUsuario.Text)) { us = null; }
                else { us = txtUsuario.Text; }

                if (cmbEvento.SelectedIndex == -1) { acc = null; }
                else { acc = (TipoAccion)cmbEvento.SelectedValue; }


                CargarDGV(bitacora.BuscarEventos(us, acc, fIni, fFin));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
