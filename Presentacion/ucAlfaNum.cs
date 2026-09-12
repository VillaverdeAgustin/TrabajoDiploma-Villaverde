using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class ucAlfaNum : UserControl
    {
        public ucAlfaNum()
        {
            InitializeComponent();
        }

        private static string patron = @"^[A-Za-z0-9\s]+$";

        private bool _ok;

        public bool ok
        {
            get { return _ok; }
            set { _ok = value; }
        }

        private string _texto;

        public string texto
        {
            get { return _texto; }
            set { _texto = txtTexto.Text; }
        }

        private bool _isPass;

        public bool isPass
        {
            get { return _isPass; }
            set { _isPass = value; }
        }

        public Button boton;

        /// <summary>
        /// Alinea el control con la identidad visual de ConectAR: caja sin borde
        /// sobre fondo blanco y subrayado institucional que marca la validacion.
        /// </summary>
        public void AplicarEstilo()
        {
            this.BackColor = EstiloConectAR_575_AV.Superficie;
            txtTexto.BorderStyle = BorderStyle.None;
            txtTexto.BackColor = EstiloConectAR_575_AV.Superficie;
            txtTexto.ForeColor = EstiloConectAR_575_AV.Navy;
            txtTexto.Font = EstiloConectAR_575_AV.FuenteCampo;
            ptxtTexto.BackColor = EstiloConectAR_575_AV.Borde;
        }

        private void ucUsuario_Load(object sender, EventArgs e)
        {
            txtTexto.Clear();
            ptxtTexto.BackColor = EstiloConectAR_575_AV.Borde;
            ok = false;
            texto = "";
            isPass = false;
        }

        private void txtTexto_Enter(object sender, EventArgs e)
        {
            ptxtTexto.BackColor = EstiloConectAR_575_AV.Cian;
        }

        private void txtTexto_Leave(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(txtTexto.Text))
            {
                texto = txtTexto.Text;
                if(Regex.IsMatch(texto, patron))
                {

                    ok = true;
                    ptxtTexto.BackColor = EstiloConectAR_575_AV.Borde;
                }
                else
                {
                    ptxtTexto.BackColor = EstiloConectAR_575_AV.Peligro;
                    ok = false;
                }
            }
            else
            {
                ptxtTexto.BackColor = EstiloConectAR_575_AV.Peligro;
                texto = "";
                ok = false;
            }
        }

        public void Limpiar()
        {
            txtTexto.Clear();
        }

        public void Enfocar()
        {
            txtTexto.Focus();
        }

        public void Hide(bool pass)
        {
            if (pass) { isPass = true; txtTexto.UseSystemPasswordChar = true; }
        }

        public void txtContra_KeyPress(object sender, KeyPressEventArgs e)
        //btn Iniciar al presion enter en campo Contraseña
        {
            if (isPass)
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    boton.PerformClick();
                }
            }
        }
    }
}
