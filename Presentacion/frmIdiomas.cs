using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmIdiomas : Form, IObservadorIdioma
    {
        public frmIdiomas()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        BitacoraBLL bitacora = new BitacoraBLL();
        DataTable traducciones;

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
            gbNuevo.Text = gestorIdioma.Traducir("IDI_GB_NUEVO");
            lblCodigo.Text = gestorIdioma.Traducir("IDI_LBL_CODIGO");
            lblNombre.Text = gestorIdioma.Traducir("COMUN_NOMBRE");
            lblBase.Text = gestorIdioma.Traducir("IDI_LBL_BASE");
            lblArchivo.Text = gestorIdioma.Traducir("IDI_LBL_ARCHIVO");
            btnExaminar.Text = gestorIdioma.Traducir("IDI_BTN_EXAMINAR");
            btnCrear.Text = gestorIdioma.Traducir("IDI_BTN_CREAR");
            gbTraducciones.Text = gestorIdioma.Traducir("IDI_GB_TRADUCCIONES");
            lblIdioma.Text = gestorIdioma.Traducir("IDI_LBL_IDIOMA");
            btnGuardar.Text = gestorIdioma.Traducir("COMUN_GUARDAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
        }

        private void TraducirColumnas()
        {
            if (dgvTraducciones.Columns.Count == 0) { return; }
            dgvTraducciones.Columns["Clave"].HeaderText = gestorIdioma.Traducir("IDI_COL_CLAVE");
            dgvTraducciones.Columns["Texto"].HeaderText = gestorIdioma.Traducir("IDI_COL_TEXTO");
        }

        #endregion

        private void CargarIdiomas()
        {
            cmbIdioma.DataSource = gestorIdioma.ObtenerIdiomas();
        }

        private void SeleccionarIdioma(string codigo)
        {
            //Deja seleccionado en el combo el idioma recien creado,
            //listo para editar sus traducciones en la grilla
            foreach (IdiomaBE idioma in (List<IdiomaBE>)cmbIdioma.DataSource)
            {
                if (idioma.Codigo == codigo)
                {
                    cmbIdioma.SelectedItem = idioma;
                    break;
                }
            }
        }

        private void cmbIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTraducciones();
        }

        private void CargarTraducciones()
        {
            IdiomaBE idioma = cmbIdioma.SelectedItem as IdiomaBE;
            if (idioma == null) { return; }

            traducciones = gestorIdioma.ObtenerTablaTraducciones(idioma.Id);
            dgvTraducciones.DataSource = traducciones;
            dgvTraducciones.Columns["Clave"].ReadOnly = true;
            dgvTraducciones.Columns["Texto"].ReadOnly = false;
            TraducirColumnas();
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Filter = "Archivos de traducciones (*.csv;*.txt)|*.csv;*.txt|Todos los archivos (*.*)|*.*";
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txtArchivo.Text = dialogo.FileName;
                }
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string codigo = txtCodigo.Text.Trim().ToUpper();
                string rutaArchivo = txtArchivo.Text.Trim() == "" ? null : txtArchivo.Text.Trim();

                int importadas = gestorIdioma.AgregarIdioma(codigo, nombre, rutaArchivo);
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user,
                    TipoAccion.AltaIdioma);

                if (importadas > 0)
                {
                    MessageBox.Show(string.Format(gestorIdioma.Traducir("IDI_MSG_IMPORTADAS"), nombre, importadas));
                }
                else
                {
                    MessageBox.Show(string.Format(gestorIdioma.Traducir("IDI_MSG_CREADO"), nombre));
                }
                txtCodigo.Clear();
                txtNombre.Clear();
                txtArchivo.Clear();
                CargarIdiomas();
                SeleccionarIdioma(codigo);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(gestorIdioma.Traducir("IDI_ERR_ARCHIVO") + ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(gestorIdioma.Traducir("IDI_ERR_ARCHIVO") + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD") + ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            IdiomaBE idioma = cmbIdioma.SelectedItem as IdiomaBE;
            if (idioma == null || traducciones == null) { return; }

            try
            {
                //Confirma la edicion pendiente de celda Y de fila antes de leer
                //los RowState, sino la fila en edicion no figura como Modified
                dgvTraducciones.EndEdit();
                this.Validate();
                this.BindingContext[traducciones].EndCurrentEdit();

                Dictionary<string, string> cambios = new Dictionary<string, string>();
                foreach (DataRow fila in traducciones.Rows)
                {
                    if (fila.RowState == DataRowState.Modified)
                    {
                        cambios[fila["Clave"].ToString()] = fila["Texto"].ToString();
                    }
                }
                if (cambios.Count == 0) { return; }

                gestorIdioma.GuardarTraducciones(idioma.Id, cambios);
                traducciones.AcceptChanges();
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user,
                    TipoAccion.ModificacionIdioma);

                MessageBox.Show(gestorIdioma.Traducir("IDI_MSG_GUARDADO"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD") + ex.Message);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
