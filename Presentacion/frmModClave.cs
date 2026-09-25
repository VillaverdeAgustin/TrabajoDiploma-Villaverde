using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmModClave : Form, IObservadorIdioma
    {
        public frmModClave()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            gestorIdioma.Suscribir(this);
            this.FormClosed += frmModClave_FormClosed;
        }

        UsuarioBLL usuario = new UsuarioBLL();
        BitacoraBLL bitacora = new BitacoraBLL();
        UsuarioBE us = new UsuarioBE();
        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        int verif = 0;
        string patron = "^[A-Za-z0-9]+$";

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("CLAVE_TITULO");
            lblActual.Text = gestorIdioma.Traducir("CLAVE_LBL_ACTUAL");
            lblNuevo.Text = gestorIdioma.Traducir("CLAVE_LBL_NUEVA");
            lblRep.Text = gestorIdioma.Traducir("CLAVE_LBL_REPETIR");
            btnLogin.Text = gestorIdioma.Traducir("CLAVE_BTN_CONFIRMAR");
            btnAceptar.Text = gestorIdioma.Traducir("COMUN_ACEPTAR");
            btnCancel.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
            if (lblInstrucciones.Text != "")
            {
                lblInstrucciones.Text = gestorIdioma.Traducir("CLAVE_MSG_INSTRUCCIONES");
            }
        }

        private void frmModClave_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #endregion


        #region Modificadores Formulario
        public void CambiarEnabled()
        {
            if (txtClave.Text != "" & txtClaveRep.Text != "")
            {
                btnAceptar.Enabled = true;
            }
            else { btnAceptar.Enabled = false; }
        }

        public void LimpiaClave()
        {
            txtClave.Clear();
            txtClaveRep.Clear();
            txtClave.Focus();
        }

        public void VerificarEnabled()
        {
            if (txtActual.Text != "")
            {
                btnLogin.Enabled = true;
            }
            else { btnLogin.Enabled = false; }
        }

        private void txtActual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
        private void txtActual_TextChanged(object sender, EventArgs e)
        {
            VerificarEnabled();
            lblError.Text = "";
        }

        private void txtClave_TextChanged(object sender, EventArgs e)
        {
            CambiarEnabled();
            lblError.Text = "";
        }

        private void txtClaveRep_TextChanged(object sender, EventArgs e)
        {
            CambiarEnabled();
            lblError.Text = "";
        }

        private void txtClaveRep_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnAceptar_Click(sender, e);
            }
        }

        #endregion

        bool OkText(string text)
        {
            return Regex.IsMatch(text, patron);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (OkText(txtActual.Text))
            {
                us.pass = txtActual.Text;
                verif = usuario.VerifUsuario(us, 0);

                switch (verif)
                {
                    case 0:
                        txtActual.Clear();
                        txtActual.Focus();
                        lblError.Text = string.Format(gestorIdioma.Traducir("CLAVE_ERR_INTENTOS"), usuario.maxIntentos.ToString());
                        bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.LoginFail);
                        //txtActual.Clear();
                        //txtActual.Focus();
                        //lblError.Text = $"Contraseña incorrecta, quedan {usuario.maxIntentos.ToString()} intentos";
                        break;
                    case 1:
                        btnLogin.Visible = false;
                        lblActual.Visible = false;
                        txtActual.Visible = false;
                        lblNuevo.Visible = true;
                        txtClave.Visible = true;
                        lblRep.Visible = true;
                        txtClaveRep.Visible = true;
                        btnAceptar.Visible = true;
                        lblInstrucciones.Text = gestorIdioma.Traducir("CLAVE_MSG_INSTRUCCIONES");
                        txtClave.Focus();
                        break;
                    case 2:
                        MessageBox.Show(gestorIdioma.Traducir("CLAVE_MSG_BLOQUEADO"), gestorIdioma.Traducir("CLAVE_TIT_BLOQUEADO"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.BloqueoUsuario);
                        usuario.Logout();
                        Application.Exit();
                        break;
                }
            }
            else
            {
                txtActual.Clear();
                lblError.Text = gestorIdioma.Traducir("CLAVE_ERR_FORMATO");
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (OkText(txtClave.Text) && OkText(txtClaveRep.Text))
            {
                if (txtClave.Text == txtClaveRep.Text)
                {
                    try
                    {
                        us.pass = txtClave.Text;
                        verif = usuario.VerifUsuario(us, 1);

                        if (verif == 1)
                        {
                            LimpiaClave();
                            gestorIdioma.Traducir("CLAVE_ERR_IGUAL_ANTERIOR");
                        }
                        else
                        {
                            MessageBox.Show(gestorIdioma.Traducir("CLAVE_MSG_OK"), gestorIdioma.Traducir("CLAVE_TIT_OK"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD") + ex.Message);
                        this.Close();
                    }
                }
                else
                {
                    LimpiaClave();
                    lblError.Text = gestorIdioma.Traducir("CLAVE_ERR_DISTINTAS");
                }
            }
            else
            {
                LimpiaClave();
                lblError.Text = gestorIdioma.Traducir("CLAVE_ERR_NO_FORMATO");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
