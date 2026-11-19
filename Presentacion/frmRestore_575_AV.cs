using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // T07 - Restore de ConectAR_DB. La pantalla elige el archivo y pide la confirmacion; BackupBLL_575_AV
    // restaura, recalcula los DV, registra el evento y cierra la sesion. Quien abrio la pantalla vuelve al login.
    public partial class frmRestore_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly BackupBLL_575_AV backupBLL = new BackupBLL_575_AV();
        private Func<string> avisoTraducible;

        // Avisa al menu (o al login) que la base se reemplazo y la sesion ya se cerro
        public event EventHandler RestauracionRealizada;

        public bool Restaurado { get; private set; }

        public frmRestore_575_AV()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "backup_restore");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPeligro(btnRestaurar);
            EstiloConectAR_575_AV.BotonSecundario(btnExaminar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            EstiloConectAR_575_AV.FormatoNumero(dgvBackups, "colTamano");
            colFecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            lblAdvertencia.ForeColor = EstiloConectAR_575_AV.Peligro;

            ofdBackup.CheckFileExists = true;
            ofdBackup.DefaultExt = "bak";
        }

        private void frmRestore_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
            CargarBackups();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("RST_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("RST_TITULO");
            lblAdvertencia.Text = gestorIdioma.Traducir("RST_LBL_ADVERTENCIA");
            gbBackups.Text = gestorIdioma.Traducir("RST_GB_BACKUPS");
            colFecha.HeaderText = gestorIdioma.Traducir("COMUN_FECHA");
            colTamano.HeaderText = gestorIdioma.Traducir("RST_COL_TAMANO");
            colRuta.HeaderText = gestorIdioma.Traducir("RST_COL_ARCHIVO");
            gbArchivo.Text = gestorIdioma.Traducir("RST_GB_ARCHIVO");
            btnExaminar.Text = gestorIdioma.Traducir("RST_BTN_EXAMINAR");
            btnRestaurar.Text = gestorIdioma.Traducir("RST_BTN_RESTAURAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            ofdBackup.Title = gestorIdioma.Traducir("RST_DLG_ABRIR");
            ofdBackup.Filter = gestorIdioma.Traducir("RST_FILTRO");
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        #endregion

        #region Eleccion del backup

        private void CargarBackups()
        {
            try
            {
                List<BackupBE_575_AV> backups = backupBLL.ListarBackups();
                dgvBackups.DataSource = backups;
                if (backups.Count == 0)
                {
                    Avisar(() => gestorIdioma.Traducir("RST_MSG_SIN_BACKUPS"), false);
                }
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void dgvBackups_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow fila = dgvBackups.SelectedRows.Count > 0 ? dgvBackups.SelectedRows[0] : dgvBackups.CurrentRow;
            BackupBE_575_AV backup = fila == null ? null : fila.DataBoundItem as BackupBE_575_AV;
            if (backup != null) { txtArchivo.Text = backup.ruta; }
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtArchivo.Text))
            {
                try { ofdBackup.InitialDirectory = Path.GetDirectoryName(txtArchivo.Text.Trim()); }
                catch (ArgumentException) { }
            }
            if (ofdBackup.ShowDialog(this) == DialogResult.OK)
            {
                dgvBackups.ClearSelection();
                txtArchivo.Text = ofdBackup.FileName;
            }
        }

        #endregion

        #region Restore

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            BackupBE_575_AV backup;
            try
            {
                backup = backupBLL.ValidarArchivo(txtArchivo.Text);
            }
            catch (InvalidOperationException ex)
            {
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
                return;
            }

            string confirmacion = string.Format(gestorIdioma.Traducir("RST_MSG_CONFIRMA"),
                backup.archivo, backup.fecha.ToString("dd/MM/yyyy HH:mm:ss"));
            if (MessageBox.Show(confirmacion, gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                Avisar(() => gestorIdioma.Traducir("RST_MSG_CANCELADO"), false);
                return;
            }

            ResultadoRestore_575_AV resultado;
            Cursor = Cursors.WaitCursor;
            try
            {
                resultado = backupBLL.Restaurar(backup.ruta, UsuarioActual());
            }
            catch (InvalidOperationException ex)
            {
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
                return;
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            string texto = resultado.integridad_ok
                ? string.Format(gestorIdioma.Traducir("RST_MSG_RESTAURADO"), resultado.backup.archivo)
                : string.Format(gestorIdioma.Traducir("RST_MSG_RESTAURADO_CON_ERRORES"), resultado.backup.archivo,
                    string.Join(", ", resultado.tablas_con_error));
            MessageBox.Show(texto, gestorIdioma.Traducir("RST_TITULO"), MessageBoxButtons.OK,
                resultado.integridad_ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            Restaurado = true;
            if (RestauracionRealizada != null) { RestauracionRealizada(this, EventArgs.Empty); }
            if (Modal) { DialogResult = DialogResult.OK; }
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
