using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // T07 - Backup de ConectAR_DB. La pantalla elige la carpeta del servidor; BackupBLL_575_AV hace el resto.
    public partial class frmBackup_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly BackupBLL_575_AV backupBLL = new BackupBLL_575_AV();
        private Func<string> avisoTraducible;
        private BackupBE_575_AV ultimo;

        public frmBackup_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnGenerar);
            EstiloConectAR_575_AV.BotonSecundario(btnPredeterminada);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            EstiloConectAR_575_AV.CampoBloqueado(txtRuta);
            lblAyuda.Font = EstiloConectAR_575_AV.FuenteBajada;
        }

        private void frmBackup_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
            CargarCarpetaPredeterminada();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("BKP_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("BKP_TITULO");
            lblAyuda.Text = gestorIdioma.Traducir("BKP_LBL_AYUDA");
            gbDestino.Text = gestorIdioma.Traducir("BKP_GB_DESTINO");
            lblCarpeta.Text = gestorIdioma.Traducir("BKP_LBL_CARPETA");
            btnPredeterminada.Text = gestorIdioma.Traducir("BKP_BTN_PREDETERMINADA");
            lblNombre.Text = gestorIdioma.Traducir("BKP_LBL_NOMBRE");
            lblNombreValor.Text = gestorIdioma.Traducir("BKP_PATRON_NOMBRE");
            btnGenerar.Text = gestorIdioma.Traducir("BKP_BTN_GENERAR");
            gbResultado.Text = gestorIdioma.Traducir("BKP_GB_RESULTADO");
            lblRuta.Text = gestorIdioma.Traducir("BKP_LBL_ARCHIVO");
            lblFecha.Text = gestorIdioma.Traducir("BKP_LBL_FECHA");
            lblTamano.Text = gestorIdioma.Traducir("BKP_LBL_TAMANO");
            lblVerificacion.Text = gestorIdioma.Traducir("BKP_LBL_VERIFICACION");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            MostrarVerificacion();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        #endregion

        #region Backup

        private void CargarCarpetaPredeterminada()
        {
            try
            {
                txtCarpeta.Text = backupBLL.CarpetaPredeterminada();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void btnPredeterminada_Click(object sender, EventArgs e) => CargarCarpetaPredeterminada();

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                ultimo = backupBLL.GenerarBackup(txtCarpeta.Text, UsuarioActual());
                txtRuta.Text = ultimo.ruta;
                lblFechaValor.Text = ultimo.fecha == DateTime.MinValue ? string.Empty : ultimo.fecha.ToString("dd/MM/yyyy HH:mm:ss");
                lblTamanoValor.Text = ultimo.tamano_mb.ToString("N2") + " MB";
                MostrarVerificacion();

                string archivo = ultimo.archivo;
                bool verificado = ultimo.verificado;
                Avisar(() => string.Format(gestorIdioma.Traducir(verificado ? "BKP_MSG_GENERADO" : "BKP_MSG_SIN_VERIFICAR"), archivo), !verificado);
            }
            catch (InvalidOperationException ex)
            {
                string mensaje = ex.Message;
                Avisar(() => mensaje, true);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void MostrarVerificacion()
        {
            if (ultimo == null)
            {
                lblVerificacionValor.Text = string.Empty;
                return;
            }
            lblVerificacionValor.Text = gestorIdioma.Traducir(ultimo.verificado ? "BKP_VER_OK" : "BKP_VER_FALLA");
            lblVerificacionValor.ForeColor = ultimo.verificado ? EstiloConectAR_575_AV.Exito : EstiloConectAR_575_AV.Peligro;
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
