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
            TextoTraducido_575_AV.Enum(cmbAccion, "ACC_");
            TextoTraducido_575_AV.Enum(dgvHistorial, "accion", "ACC_");
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
                dgvHistorial.EnableHeadersVisualStyles = false;
                TraducirColumnas();
            }
        }

        private void TraducirColumnas()
        {
            Titulo("fecha", "COMUN_FECHA");
            Titulo("accion", "COMUN_ACCION");
            Titulo("usuarioResponsable", "HIST_COL_RESPONSABLE");
            Titulo("nombre", "COMUN_NOMBRE");
            Titulo("apellido", "COMUN_APELLIDO");
            Titulo("dni", "USU_LBL_DNI");
            Titulo("usuario", "COMUN_USUARIO");
            Titulo("rol", "COMUN_ROL");
            Titulo("direccion", "USU_LBL_DIRECCION");
            Titulo("telefono", "USU_LBL_TELEFONO");
            Titulo("email", "USU_LBL_EMAIL");
            Titulo("activo", "USU_CHK_ACTIVO");
            Titulo("bloqueado", "USU_CHK_BLOQUEADO");
        }

        private void Titulo(string columna, string clave)
        {
            if (dgvHistorial.Columns[columna] != null)
            {
                dgvHistorial.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        private void CargarHistorial()
        {
            try
            {
                ConfigDGV(historialBLL.BuscarHistorial((TipoAccion?)cmbAccion.SelectedItem, dtpDesde.Value, dtpHasta.Value));
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => CargarHistorial();

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            var version = dgvHistorial.CurrentRow == null
                ? null
                : dgvHistorial.CurrentRow.DataBoundItem as HistorialUsuarioBE;
            if (version == null)
            {
                MessageBox.Show(gestorIdioma.Traducir("HIST_MSG_ELEGIR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                string.Format(gestorIdioma.Traducir("HIST_MSG_CONFIRMA"), version.usuario, version.fecha),
                gestorIdioma.Traducir("HIST_TIT_CONFIRMA"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    historialBLL.RestaurarVersion(version.idHistorial, SessionManager.GetInstance.UsuarioActual().user);
                    MessageBox.Show(gestorIdioma.Traducir("HIST_MSG_RESTAURADO"));
                    CargarHistorial();
                }
                catch (Exception)
                {
                    MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            lblAccion.Text = gestorIdioma.Traducir("COMUN_ACCION");
            lblDesde.Text = gestorIdioma.Traducir("COMUN_DESDE");
            lblHasta.Text = gestorIdioma.Traducir("COMUN_HASTA");
            label1.Text = gestorIdioma.Traducir("COMUN_NOMBRE");
            label2.Text = gestorIdioma.Traducir("COMUN_APELLIDO");
            label3.Text = gestorIdioma.Traducir("COMUN_USUARIO");
            label4.Text = gestorIdioma.Traducir("COMUN_ROL");
            label5.Text = gestorIdioma.Traducir("USU_LBL_DNI");
            lblTelefono.Text = gestorIdioma.Traducir("USU_LBL_TELEFONO");
            lblDireccion.Text = gestorIdioma.Traducir("USU_LBL_DIRECCION");
            lblEmail.Text = gestorIdioma.Traducir("USU_LBL_EMAIL");
            chkBloqueado.Text = gestorIdioma.Traducir("USU_CHK_BLOQUEADO");
            chkActivo.Text = gestorIdioma.Traducir("USU_CHK_ACTIVO");
            TraducirColumnas();
            TextoTraducido_575_AV.Refrescar(cmbAccion);
            dgvHistorial.Invalidate();
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
            txtDireccion.Text = version.direccion;
            txtTelefono.Text = version.telefono;
            txtEmail.Text = version.email;
            chkActivo.Checked = version.activo;
            chkBloqueado.Checked = version.bloqueado;

            btnRestaurar.Enabled = true;
        }
    }
}
