using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmIdiomas : Form, IObservadorIdioma
    {
        public frmIdiomas()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        BitacoraBLL bitacora = new BitacoraBLL();
        bool cargando;

        private void frmIdiomas_Load(object sender, EventArgs e)
        {
            CargarIdiomas();
            gestorIdioma.Suscribir(this);
            this.FormClosed += frmIdiomas_FormClosed;
        }

        private void frmIdiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("IDI_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("IDI_TITULO");
            lblIdioma.Text = gestorIdioma.Traducir("IDI_LBL_IDIOMA");
            lblAyuda.Text = gestorIdioma.Traducir("IDI_LBL_AYUDA");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
        }

        #endregion

        private void CargarIdiomas()
        {
            cargando = true;
            try
            {
                List<IdiomaBE> idiomas = gestorIdioma.ObtenerIdiomas();
                cmbIdioma.DataSource = idiomas;
                cmbIdioma.DisplayMember = "Nombre";

                IdiomaBE actual = gestorIdioma.IdiomaActual;
                if (actual != null)
                {
                    cmbIdioma.SelectedItem = idiomas.Find(i => i.Codigo == actual.Codigo);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargando = false;
            }
        }

        private void cmbIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargando) { return; }

            IdiomaBE idioma = cmbIdioma.SelectedItem as IdiomaBE;
            if (idioma == null) { return; }
            if (gestorIdioma.IdiomaActual != null && gestorIdioma.IdiomaActual.Codigo == idioma.Codigo) { return; }

            try
            {
                gestorIdioma.CambiarIdioma(idioma.Codigo);
                string usuario = SessionManager.Logged() ? SessionManager.GetInstance.UsuarioActual().user : "null";
                bitacora.RegistrarBitacora(usuario, TipoAccion.CambioIdioma);
            }
            catch (Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
