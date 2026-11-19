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
    public partial class frmLogin : Form, IObservadorIdioma
    {
        public frmLogin()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "login");
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        UsuarioBLL usuario = new UsuarioBLL();
        BitacoraBLL bitacora = new BitacoraBLL();
        VerificadorIntegridadBLL IntegridadBLL = new VerificadorIntegridadBLL();
        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                txtContra.boton = this.btnIniciar;
                lblError.Text = "";
                CargarMenuIdiomas();
                gestorIdioma.Suscribir(this);
                this.FormClosed += frmLogin_FormClosed;
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD"),
                    gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region T08 - Control de integridad al iniciar sesion

        private void IngresarAlSistema()
        {
            UsuarioBE enSesion = SessionManager.GetInstance.UsuarioActual();
            List<string> tablasAfectadas = IntegridadBLL.VerificarIntegridadTotal();

            if (tablasAfectadas.Count > 0)
            {
                bool puedeSeguir = usuario.EsAdministrador(enSesion)
                    ? ResolverIntegridad(enSesion, tablasAfectadas)
                    : InformarSistemaNoDisponible(enSesion);

                if (!puedeSeguir) { return; }
            }

            frmMenu frm = new frmMenu();
            frm.Show();
            this.Hide();
            frm.FormClosing += frm_closing;
        }

        private bool ResolverIntegridad(UsuarioBE enSesion, List<string> tablasAfectadas)
        {
            DialogResult opcion;
            while (true)
            {
                using (frmRecalcular pantalla = new frmRecalcular())
                {
                    pantalla.IndicarTablasAfectadas(tablasAfectadas);
                    opcion = pantalla.ShowDialog(this);
                }
                if (opcion != DialogResult.Retry) { break; }

                // T07 - Restaurar backup: si se restaura, la BLL ya recalculo los DV y cerro la sesion
                bool restaurado;
                using (frmRestore_575_AV restore = new frmRestore_575_AV())
                {
                    restore.StartPosition = FormStartPosition.CenterScreen;
                    restore.ShowDialog(this);
                    restaurado = restore.Restaurado;
                }
                if (restaurado)
                {
                    txtContra.Limpiar();
                    txtUsuario.Enfocar();
                    return false;
                }
            }

            if (opcion != DialogResult.OK)
            {
                CerrarSesionYSalir();
                return false;
            }

            IntegridadBLL.RecalcularDVTotal();
            bitacora.RegistrarBitacora(enSesion.user, TipoAccion.RecalculoDigitoVerificador);

            List<string> siguenAfectadas = IntegridadBLL.VerificarIntegridadTotal();
            if (siguenAfectadas.Count > 0)
            {
                MessageBox.Show(
                    string.Format(gestorIdioma.Traducir("LOGIN_MSG_INTEGRIDAD_NO_RESTAURADA"),
                        string.Join(", ", siguenAfectadas)),
                    gestorIdioma.Traducir("LOGIN_TIT_INTEGRIDAD"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                CerrarSesionYSalir();
                return false;
            }

            MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_INTEGRIDAD_RESTAURADA"),
                gestorIdioma.Traducir("LOGIN_TIT_INTEGRIDAD"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool InformarSistemaNoDisponible(UsuarioBE enSesion)
        {
            MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_SISTEMA_NO_DISPONIBLE"),
                gestorIdioma.Traducir("LOGIN_TIT_INTEGRIDAD"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
            bitacora.RegistrarBitacora(enSesion.user, TipoAccion.SistemaNoDisponible);
            CerrarSesionYSalir();
            return false;
        }

        private void CerrarSesionYSalir()
        {
            usuario.Logout();
            Application.Exit();
        }

        #endregion

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("LOGIN_TITULO");
            lblUsuario.Text = gestorIdioma.Traducir("COMUN_USUARIO");
            lblContra.Text = gestorIdioma.Traducir("COMUN_CONTRASENA");
            btnIniciar.Text = gestorIdioma.Traducir("LOGIN_BTN_INICIAR");
            btnCancelar.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
            lblSinConexion.Text = gestorIdioma.Traducir("LOGIN_LNK_SIN_CONEXION");
            lblPie.Text = gestorIdioma.Traducir("LOGIN_LBL_PIE");
            cambiarIdiomaToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CAMBIAR_IDIOMA");
            MarcarIdiomaActivo();
        }

        private void CargarMenuIdiomas()
        {
            try
            {
                cambiarIdiomaToolStripMenuItem.DropDownItems.Clear();
                foreach (IdiomaBE idioma in gestorIdioma.ObtenerIdiomas())
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(idioma.Nombre);
                    item.Tag = idioma.Codigo;
                    item.Click += itemIdioma_Click;
                    cambiarIdiomaToolStripMenuItem.DropDownItems.Add(item);
                }
            }
            catch { }
        }

        private void MarcarIdiomaActivo()
        {
            foreach (ToolStripMenuItem item in cambiarIdiomaToolStripMenuItem.DropDownItems)
            {
                item.Checked = gestorIdioma.IdiomaActual != null
                    && item.Tag.ToString() == gestorIdioma.IdiomaActual.Codigo;
            }
        }

        private void itemIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                gestorIdioma.CambiarIdioma((sender as ToolStripMenuItem).Tag.ToString());
                bitacora.RegistrarBitacora("null", TipoAccion.CambioIdioma);
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void txtContra_Load(object sender, EventArgs e)
        {
            txtContra.Hide(true);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(gestorIdioma.Traducir("COMUN_CONFIRMA_SALIR_APP"), gestorIdioma.Traducir("COMUN_ATENCION"),
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (SessionManager.Logged()) { usuario.Logout(); }
                    else { bitacora.RegistrarBitacora("null", TipoAccion.AppClose); }
                    Application.Exit();
                }
                catch (Exception)
                {
                    MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else txtUsuario.Enfocar();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            UsuarioBE user;

            if (!txtUsuario.ok || !txtContra.ok)
            {
                MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_DATOS_INVALIDOS"), gestorIdioma.Traducir("LOGIN_TIT_DATOS_INVALIDOS"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    user = new UsuarioBE();
                    user.user = txtUsuario.texto;
                    user.pass = txtContra.texto;

                    LoginResult authOK = usuario.Login(user);
                    if (authOK == LoginResult.LoginOK)
                    {
                        IngresarAlSistema();
                    }
                    else
                    {
                        switch (authOK)
                        {
                            case LoginResult.UserInexistente:
                                lblError.Text = gestorIdioma.Traducir("LOGIN_ERR_USER_INEXISTENTE");
                                break;
                            case LoginResult.UserBloqueado:
                                lblError.Text = gestorIdioma.Traducir("LOGIN_ERR_USER_BLOQUEADO");
                                break;
                            case LoginResult.PassIncorrecta:
                                lblError.Text = gestorIdioma.Traducir("LOGIN_ERR_PASS_INCORRECTA");
                                break;
                            case LoginResult.UserInactivo:
                                MessageBox.Show(string.Format(gestorIdioma.Traducir("LOGIN_MSG_USER_INACTIVO"), user.user), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                break;
                            case LoginResult.FinIntentos:
                                MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_FIN_INTENTOS"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                break;
                            case LoginResult.SesionIniciada:
                                MessageBox.Show(string.Format(gestorIdioma.Traducir("LOGIN_MSG_SESION_INICIADA"), user.user), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                IngresarAlSistema();
                                break;
                            case LoginResult.ExisteSesion:
                                if (MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_EXISTE_SESION"), " ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    usuario.Logout();
                                    MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_SESION_CERRADA"), " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else { MessageBox.Show(gestorIdioma.Traducir("LOGIN_MSG_USUARIO_EN_USO"), " ", MessageBoxButtons.OK, MessageBoxIcon.Hand); }
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(gestorIdioma.Traducir("COMUN_MSG_ERROR"), gestorIdioma.Traducir("COMUN_ATENCION"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            txtUsuario.Limpiar();
            txtContra.Limpiar();
            txtUsuario.Enfocar();
        }

        private void lblSinConexion_Click(object sender, EventArgs e)
        {
            bitacora.RegistrarBitacora("null", TipoAccion.NoSesion);
            usuario.Logout();
            frmMenu frm = new frmMenu();
            frm.Show();
            this.Hide();
        }

        private void lblSinConexion_MouseHover(object sender, EventArgs e)
        {
            ((Label)sender).Font = EstiloConectAR_575_AV.FuenteEnlaceActivo;
            ((Label)sender).ForeColor = EstiloConectAR_575_AV.Cian;
        }

        private void lblSinConexion_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Font = EstiloConectAR_575_AV.FuenteEnlace;
            ((Label)sender).ForeColor = EstiloConectAR_575_AV.NavySuave;
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            txtContra.Limpiar();
            txtUsuario.Limpiar();
            this.Show();
            txtUsuario.Enfocar();
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            lblError.Text = "";
        }
    }
}
