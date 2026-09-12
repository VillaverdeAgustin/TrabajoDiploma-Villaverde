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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmUsuario : Form,IObservadorIdioma
    {
        public frmUsuario()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
        }

        UsuarioBE auxUsuario;
        BitacoraBLL bitacora = new BitacoraBLL();
        UsuarioBLL usuarioBLL = new UsuarioBLL();
        PerfilBLL perfilBLL = new PerfilBLL();
        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        int varMod = 0; //Indica si la modificacion esta activa
        List<string> roles = new List<string>();
        //{ "Cajero", "Vendedor", "Administrador" };

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("USU_TITULO");
            label1.Text = gestorIdioma.Traducir("USU_LBL_TITULO");
            label2.Text = gestorIdioma.Traducir("USU_LBL_OPCIONES");
            gbDatos.Text = gestorIdioma.Traducir("USU_GB_DATOS");
            g.Text = gestorIdioma.Traducir("USU_GB_MENSAJES");
            label3.Text = gestorIdioma.Traducir("USU_LBL_DNI");
            label4.Text = gestorIdioma.Traducir("COMUN_NOMBRE");
            label5.Text = gestorIdioma.Traducir("COMUN_APELLIDO");
            label6.Text = gestorIdioma.Traducir("COMUN_USUARIO");
            label7.Text = gestorIdioma.Traducir("COMUN_ROL");
            label8.Text = gestorIdioma.Traducir("USU_LBL_DIRECCION");
            label9.Text = gestorIdioma.Traducir("USU_LBL_TELEFONO");
            label10.Text = gestorIdioma.Traducir("USU_LBL_EMAIL");
            chkActivo.Text = gestorIdioma.Traducir("USU_CHK_ACTIVO");
            chkBloqueado.Text = gestorIdioma.Traducir("USU_CHK_BLOQUEADO");
            btnCrearUs.Text = gestorIdioma.Traducir("USU_BTN_CREAR");
            btnModUs.Text = gestorIdioma.Traducir("USU_BTN_MODIFICAR");
            btnElimUs.Text = gestorIdioma.Traducir("USU_BTN_ELIMINAR");
            btnDesbloquear.Text = gestorIdioma.Traducir("USU_BTN_DESBLOQUEAR");
            btnGuardar.Text = gestorIdioma.Traducir("COMUN_GUARDAR");
            btnCancelar.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            TraducirColumnas();
        }

        private void TraducirColumnas()
        {
            if (dgvUsuarios.Columns.Count == 0) { return; }
            dgvUsuarios.Columns[1].HeaderText = gestorIdioma.Traducir("USU_LBL_DNI");
            dgvUsuarios.Columns[2].HeaderText = gestorIdioma.Traducir("COMUN_NOMBRE");
            dgvUsuarios.Columns[3].HeaderText = gestorIdioma.Traducir("COMUN_APELLIDO");
            dgvUsuarios.Columns[4].HeaderText = gestorIdioma.Traducir("COMUN_USUARIO");
            dgvUsuarios.Columns[5].HeaderText = gestorIdioma.Traducir("COMUN_ROL");
            dgvUsuarios.Columns[10].HeaderText = gestorIdioma.Traducir("USU_CHK_ACTIVO");
            dgvUsuarios.Columns[11].HeaderText = gestorIdioma.Traducir("USU_CHK_BLOQUEADO");
        }

        #endregion

        #region Funciones

        private void ConfigDefaultForm()
        {
            HabilitarBtn(btnGuardar, false);
            HabilitarBtn(btnCancelar, false);
            cmbRol.Enabled = false;
            HabilitarBtn(btnCrearUs, true);
            HabilitarBtn(btnModUs, false);
            HabilitarBtn(btnElimUs, false);
            HabilitarBtn(btnDesbloquear, false);
            dgvUsuarios.Enabled = true;
            chkActivo.Enabled = false;
            varMod = 0;
            foreach (Control c in pUsuario.Controls)
            {
                if (c is Button btn)
                {
                    EstiloConectAR_575_AV.EstadoBoton(btn, btn.Enabled);
                }
            }
            ultimoBoton = null;
            LimpiarDatos();

            foreach (Control control in gbDatos.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Enabled = false;
                }
            }
        }

        private void HabilitarBtn(Button btn, bool tipo)
        {
            EstiloConectAR_575_AV.EstadoBoton(btn, tipo);
        }

        private Button ultimoBoton = null;

        private void ClickBoton(object sender, EventArgs e)
        {
            if (ultimoBoton != null)
            {
                EstiloConectAR_575_AV.EstadoBoton(ultimoBoton, ultimoBoton.Enabled);
            }

            if ((sender as Button).Text != "Cancelar")
            {
                EstiloConectAR_575_AV.BotonSeleccionado(sender as Button);
            }
            ultimoBoton = (sender as Button);
        }

        private void HabilitarCampos()
        {
            foreach (Control control in gbDatos.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Enabled = true;
                    EstiloConectAR_575_AV.CampoEditable(textBox);
                }
                cmbRol.Enabled = true;
            }
        }

        private void LimpiarDatos()
        {
            foreach (Control control in gbDatos.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = "";
                    EstiloConectAR_575_AV.CampoBloqueado(textBox);
                }
            }
            cmbRol.SelectedItem = null;
            chkBloqueado.Checked = false;
            chkActivo.Checked = false;
            txtDoc.Focus();
        }

        private void ConfigDGV(List<UsuarioBE> usuarios)
        {
            dgvUsuarios.DataSource = usuarios;
            dgvUsuarios.Columns["dir"].Visible = false;
            dgvUsuarios.Columns["tel"].Visible = false;
            dgvUsuarios.Columns["email"].Visible = false;
            dgvUsuarios.Columns["cod"].Visible = false;
            dgvUsuarios.Columns["pass"].Visible = false;
            dgvUsuarios.Columns["dvh"].Visible = false;
            dgvUsuarios.Columns["digito"].Visible = false;
            dgvUsuarios.Columns["ape"].HeaderText = "Apellido";
            dgvUsuarios.Columns["nomb"].HeaderText = "Nombre";
            dgvUsuarios.Columns["dni"].HeaderText = "DNI";
            dgvUsuarios.Columns["user"].HeaderText = "Usuario";
            dgvUsuarios.Columns["rol"].HeaderText = "Rol";
            dgvUsuarios.Columns["estado"].HeaderText = "Activo";
            dgvUsuarios.Columns["bloq"].HeaderText = "Bloqueado";
            dgvUsuarios.ReadOnly = true;
        }

        private void ActualizarDGV()
        {
            dgvUsuarios.DataSource = usuarioBLL.ListarUsuarios();
            ConfigDefaultForm();
        }

        private UsuarioBE CargaUsuario()
        {
            if (ValCampos())//Validacion caracteres
            {
                if(varMod == 1) { auxUsuario = ExtraerDatos(dgvUsuarios.SelectedRows[0]); }
                else auxUsuario = new UsuarioBE();

                auxUsuario.dni = Convert.ToInt32(txtDoc.Text);
                auxUsuario.nomb = txtNom.Text;
                auxUsuario.ape = txtApe.Text;
                auxUsuario.rol = cmbRol.SelectedValue.ToString();
                auxUsuario.user = txtUsu.Text;
                auxUsuario.estado = chkActivo.Checked;
                auxUsuario.bloq = chkBloqueado.Checked;
                if (txtDir.Text == string.Empty) { auxUsuario.dir = " "; }
                else { auxUsuario.dir = txtDir.Text; }
                if (txtTel.Text == string.Empty) { auxUsuario.tel = " "; }
                else { auxUsuario.tel = txtTel.Text; }
                if (txtMail.Text == string.Empty) { auxUsuario.email = " "; }
                else { auxUsuario.email = txtMail.Text; }

                return auxUsuario;
            }
            return null;
        }

        private void GuardarEnabled()
        {
            if (txtNom.Text != "" & txtApe.Text != "" & txtDoc.Text != ""
                    & txtUsu.Text != "" & cmbRol.SelectedValue != null)
            {
                HabilitarBtn(btnGuardar, true);
            }
            else
            {
                HabilitarBtn(btnGuardar, false);
            }
        }

        private void CampoError(string campo)
        //Marcar en rojo los campos con validacion no ok
        {
            foreach (Control t in gbDatos.Controls)
            {
                if (t is TextBox & (t.Name == campo))
                {
                    EstiloConectAR_575_AV.CampoConError((TextBox)t);
                    break;
                }
            }
        }

        private UsuarioBE ExtraerDatos(DataGridViewRow fila)
        {
            string[] datos = new string[fila.Cells.Count];

            for (int i = 0; i < fila.Cells.Count; i++)
            {
                datos[i] = fila.Cells[i].Value.ToString();
            }
            auxUsuario = new UsuarioBE();
            return auxUsuario.CrearUsuario(datos);
        }

        private void LlenarMensaje(string linea)
        {
            txtMensaje.Text = linea + Environment.NewLine + Environment.NewLine + txtMensaje.Text;
        }

        #endregion

        #region ValidacionCampos
        private void txtDoc_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtDoc);
            EstiloConectAR_575_AV.CampoEditable(txtDoc);
            GuardarEnabled();
        }

        private void txtNom_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtNom);
            GuardarEnabled();
        }

        private void txtApe_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtApe);
            GuardarEnabled();
        }

        private void txtUsu_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtUsu);
            GuardarEnabled();
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtDir);
            GuardarEnabled();
        }

        private void txtTel_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtTel);
            GuardarEnabled();
        }

        private void txtMail_TextChanged(object sender, EventArgs e)
        {
            EstiloConectAR_575_AV.CampoEditable(txtMail);
            GuardarEnabled();
        }

        private void cmbRol_SelectedValueChanged(object sender, EventArgs e)
        {
            GuardarEnabled();
        }

        private bool ValCampos()
        {
            string patronT = @"^[a-zA-Z\s]+$";
            string patronD = @"^[A-Za-z0-9\s]+$";
            string patronN = "^[0-9]+$";
            string patronS = "^[A-Za-z0-9]+$";
            string patronM = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            bool txtOK, numOK, sensOK, dirOK, mailOK;
            txtOK = numOK = sensOK = dirOK = mailOK = true;

            foreach (Control t in gbDatos.Controls)
            {
                if (t is TextBox)
                {
                    //Validacion Nombre y Apellido
                    if (t.Name == "txtNom" || t.Name == "txtApe")
                    {
                        if (!Regex.IsMatch(t.Text, patronT))
                        {
                            CampoError(t.Name);
                            txtOK = false;
                        }
                    }
                    else
                    {
                        //Validacion Direccion
                        if (t.Name == "txtDir")
                        {
                            if (!Regex.IsMatch(t.Text, patronD) & t.Text != string.Empty)
                            {
                                CampoError(t.Name);
                                dirOK = false;
                            }
                        }
                        else
                        {
                            //Validacion User y Pass
                            if (t.Name == "txtUsu")
                            {
                                if (!Regex.IsMatch(t.Text, patronS))
                                {
                                    CampoError(t.Name);
                                    sensOK = false;
                                }
                            }
                            else
                            {
                                //Validacion DNI y Telefono
                                if ((t.Name == "txtTel" & t.Text != string.Empty) || t.Name == "txtDoc")
                                {
                                    if (!Regex.IsMatch(t.Text, patronN))
                                    {
                                        CampoError(t.Name);
                                        numOK = false;
                                    }
                                }
                                else
                                {
                                    //Validacion Mail
                                    if (t.Name == "txtMail" & t.Text != string.Empty)
                                    {
                                        if (!Regex.IsMatch(t.Text, patronM))
                                        {
                                            CampoError(t.Name);
                                            mailOK = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #region Texto de error en Campo Mensajes
            if (!txtOK)
            {
                LlenarMensaje("Campos Nombre y/o Apellido contienen caracteres no validos." +
                    "Ingrese solo letras por favor.");
            }

            if (!numOK)
            {
                LlenarMensaje("Campos DNI y/o Telefono contienen caracteres no validos." +
                    "Ingrese solo numeros por favor.");
            }

            if (!dirOK)
            {
                LlenarMensaje("Campo Direccion contiene caracteres no validos." +
                    "Ingrese solo letras y numeros por favor.");
            }

            if (!sensOK)
            {
                LlenarMensaje("Campo Usuario contiene caracteres no validos." +
                    "No se permiten espacios ni caracteres especiales.");
            }

            if (!mailOK)
            {
                LlenarMensaje("Campo eMail contiene caracteres no validos." +
                    "Respete el formato xxxx@xxx.xxx, sin espacios ni caracteres especiales.");
            }
            #endregion

            if (!txtOK || !sensOK || !dirOK || !numOK || !mailOK)
            {
                txtDoc.Focus();
                return false;
            }
            else { return true; }
        }
        #endregion

        private void frmUsuario_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigDGV(usuarioBLL.ListarUsuarios());
                try
                {
                    List<Permiso> permisosRol = perfilBLL.ListaPermisos("Rol");
                    roles.Clear();
                    foreach (var permiso in permisosRol)
                    {
                        roles.Add(permiso.Nombre);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(gestorIdioma.Traducir("USU_ERR_ROLES") + ex.Message);
                    roles = new List<string>() { "Cajero", "Vendedor", "Admin" };
                }
                cmbRol.DataSource = new BindingSource(roles, null);
                ConfigDefaultForm();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            gestorIdioma.Suscribir(this);
            this.FormClosed += frmUsuario_FormClosed;
        }

        private void frmUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Carga de datos de fila seleccionada a textbox
            if (e.RowIndex >= 0)
            {

                DataGridViewRow fila = dgvUsuarios.Rows[e.RowIndex];

                txtDoc.Text = fila.Cells["dni"].Value.ToString();
                txtNom.Text = fila.Cells["nomb"].Value.ToString();
                txtApe.Text = fila.Cells["ape"].Value.ToString();
                cmbRol.SelectedIndex = cmbRol.Items.IndexOf(Convert.ToString(fila.Cells["rol"].Value));
                txtUsu.Text = fila.Cells["user"].Value.ToString();
                txtDir.Text = fila.Cells["dir"].Value.ToString();
                txtTel.Text = fila.Cells["tel"].Value.ToString();
                txtMail.Text = fila.Cells["email"].Value.ToString();
                if (Convert.ToBoolean(fila.Cells["estado"].Value) == true)
                {
                    chkActivo.Checked = true;
                    HabilitarBtn(btnElimUs, true);
                }
                else
                {
                    chkActivo.Checked = false;
                    HabilitarBtn(btnElimUs, false);
                }
                if (Convert.ToBoolean(fila.Cells["bloq"].Value) == true)
                {
                    chkBloqueado.Checked = true;
                    HabilitarBtn(btnDesbloquear, true);
                }
                else
                {
                    chkBloqueado.Checked = false;
                    HabilitarBtn(btnDesbloquear, false);
                }
                HabilitarBtn(btnCancelar, true);
                HabilitarBtn(btnModUs, true);
                HabilitarBtn(btnGuardar, false);
            }
            else { ConfigDefaultForm(); }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            auxUsuario = null;
            Control control = btnSalir.Parent;
            frmMenu.opcActivo.BackColor = EstiloConectAR_575_AV.Navy;
            this.Close();
        }

        private void btnCrearUs_Click(object sender, EventArgs e)
        {
            HabilitarBtn(btnCancelar, true);
            HabilitarBtn(btnDesbloquear, false);
            HabilitarBtn(btnCrearUs, false);
            HabilitarBtn(btnElimUs, false);
            HabilitarBtn(btnModUs, false);
            dgvUsuarios.Enabled = false;
            LimpiarDatos();
            HabilitarCampos();
            chkActivo.Checked = true;
            ClickBoton(sender, e);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ConfigDefaultForm();
            ClickBoton(sender, e);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            UsuarioBE us = CargaUsuario();

            if (us != null)
            {
                int error = 0;

                if (varMod == 0) //Guardar Usuario Nuevo
                {
                    foreach (DataGridViewRow fila in dgvUsuarios.Rows)
                    {
                        if (Convert.ToInt32(fila.Cells[3].Value) == us.dni)
                        {
                            LlenarMensaje(gestorIdioma.Traducir("USU_MSG_DNI_EXISTE"));
                            ConfigDefaultForm();
                            error = 1;
                            break;
                        }
                        else
                        {
                            if (fila.Cells[4].Value.ToString() == us.user)
                            {
                                LlenarMensaje(gestorIdioma.Traducir("USU_MSG_USER_EXISTE"));
                                txtUsu.Text = "";
                                error = 1;
                                break;
                            }
                        }
                    }
                    if (error == 0)
                    {
                        try
                        {
                            us.pass = usuarioBLL.GenerarPass(us.ape, us.dni.ToString());
                            usuarioBLL.CrearUsuario(us);
                            ActualizarDGV();
                            LlenarMensaje(string.Format(gestorIdioma.Traducir("USU_MSG_CREADO"), us.user));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(gestorIdioma.Traducir("USU_ERR_CREAR") + ex.Message);
                            ConfigDefaultForm();
                        }
                    }
                }
                else //Guardar Usuario Modificado
                {
                    try
                    {
                        us.cod = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells[0].Value);
                        usuarioBLL.ActualizarUsuario(us);
                        ActualizarDGV();
                        LlenarMensaje(string.Format(gestorIdioma.Traducir("USU_MSG_ACTUALIZADO"), us.user));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(gestorIdioma.Traducir("USU_ERR_ACTUALIZAR") + ex.Message);
                        ConfigDefaultForm();
                    }
                }
            }
        }

        private void btnModUs_Click(object sender, EventArgs e)
        {
            varMod = 1;
            HabilitarBtn(btnCancelar, true);
            HabilitarBtn(btnDesbloquear, false);
            HabilitarBtn(btnCrearUs, false);
            HabilitarBtn(btnElimUs, false);
            HabilitarBtn(btnModUs, false);
            dgvUsuarios.Enabled = false;
            chkActivo.Enabled = true;
            HabilitarBtn(btnGuardar, true);
            HabilitarCampos();
            if (txtTel.Text == " ") { txtTel.Clear(); }
            if (txtDir.Text == " ") { txtDir.Clear(); }
            if (txtMail.Text == " ") { txtMail.Clear(); }
        }

        private void btnElimUs_Click(object sender, EventArgs e)
        {
            try
            {
                UsuarioBE us = ExtraerDatos(dgvUsuarios.SelectedRows[0]);
                if (MessageBox.Show(string.Format(gestorIdioma.Traducir("USU_MSG_CONFIRMA_ELIMINAR"), us.user), gestorIdioma.Traducir("USU_BTN_ELIMINAR"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    us.estado = false;
                    usuarioBLL.EliminarUs(us);
                    ActualizarDGV();
                    LlenarMensaje(string.Format(gestorIdioma.Traducir("USU_MSG_BAJA_OK"), us.user));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(gestorIdioma.Traducir("USU_ERR_ELIMINAR") + ex.Message);
                ConfigDefaultForm();
            }
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(gestorIdioma.Traducir("USU_MSG_CONFIRMA_DESBLOQUEO"), gestorIdioma.Traducir("USU_TIT_DESBLOQUEAR"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UsuarioBE aux = new UsuarioBE();
                    aux = ExtraerDatos(dgvUsuarios.SelectedRows[0]);
                    usuarioBLL.DesbloquearUS(aux);
                    LlenarMensaje(gestorIdioma.Traducir("USU_MSG_DESBLOQUEO_OK"));
                    ActualizarDGV();
                }
            }
            catch (Exception ex) { MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD") + ex.Message); }
        }
    }
}
