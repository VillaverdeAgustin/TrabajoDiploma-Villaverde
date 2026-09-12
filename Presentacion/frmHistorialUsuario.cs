using Negocio_BLL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Servicios;

namespace Presentacion_575_AV
{
    public partial class frmHistorialUsuario : Form, IObservadorIdioma
    {
        private readonly HistorialUsuarioBLL historialBLL = new HistorialUsuarioBLL();
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        public frmHistorialUsuario()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        private void frmHistorialUsuario_Load(object sender, EventArgs e)
        {
            cmbAccion.DataSource = Enum.GetValues(typeof(TipoAccion));
            gestorIdioma.Suscribir(this);
            ActualizarTextos();
            ConfigDGV(historialBLL.ListarHistorial());
        }

        private void frmHistorialUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        void ConfigDGV(List<HistorialUsuarioBE> lista)
        {
            dgvHistorial.DataSource = lista;

            if (dgvHistorial.Columns.Count > 0)
            {
                dgvHistorial.Columns["idHistorial"].Visible = false;
                dgvHistorial.Columns["usuarioId"].Visible = false;
                dgvHistorial.Columns["fecha"].HeaderText = "Fecha";
                dgvHistorial.Columns["accion"].HeaderText = "Acción";
                dgvHistorial.Columns["usuarioResponsable"].HeaderText = "Responsable";
                dgvHistorial.EnableHeadersVisualStyles = false;
            }
        }

        private void CargarHistorial()
        {
            var lista = historialBLL.BuscarHistorial((TipoAccion?)cmbAccion.SelectedItem, dtpDesde.Value, dtpHasta.Value);
            dgvHistorial.DataSource = lista;

            if (dgvHistorial.Columns.Count > 0)
            {
                dgvHistorial.Columns["idHistorial"].Visible = false;
                dgvHistorial.Columns["usuarioId"].Visible = false;
                dgvHistorial.Columns["fecha"].HeaderText = "Fecha";
                dgvHistorial.Columns["accion"].HeaderText = "Acción";
                dgvHistorial.Columns["usuarioResponsable"].HeaderText = "Responsable";
                dgvHistorial.EnableHeadersVisualStyles = false;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => CargarHistorial();

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            var version = (HistorialUsuarioBE)dgvHistorial.CurrentRow.DataBoundItem;

            var confirmacion = MessageBox.Show(
                $"¿Confirma restaurar al usuario '{version.usuario}' al estado del {version.fecha}?",
                "Confirmar restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                historialBLL.RestaurarVersion(version.idHistorial, SessionManager.GetInstance.UsuarioActual().user);
                MessageBox.Show("Usuario restaurado correctamente.");
                CargarHistorial();
            }
        }

        #region Patrón Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("HIST_TITULO");
            btnBuscar.Text = gestorIdioma.Traducir("HIST_BTN_BUSCAR");
            btnRestaurar.Text = gestorIdioma.Traducir("HIST_BTN_RESTAURAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            gbDetalle.Text = gestorIdioma.Traducir("HIST_GB_DETALLE");
        }

        #endregion

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvHistorial_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvHistorial.CurrentRow == null) { return; }

            var version = (HistorialUsuarioBE)dgvHistorial.CurrentRow.DataBoundItem;
            txtDni.Text = version.dni.ToString();
            txtNombre.Text = version.nombre;
            txtApellido.Text = version.apellido;
            txtUsuario.Text = version.usuario;
            txtRol.Text = version.rol;
            lblDireccion.Text = version.direccion;
            lblTelefono.Text = version.telefono;
            lblEmail.Text = version.email;
            chkActivo.Checked = version.activo;
            chkBloqueado.Checked = version.bloqueado;

            btnRestaurar.Enabled = true;
        }
    }
}
