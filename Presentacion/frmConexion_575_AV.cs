using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmConexion_575_AV : Form, IObservadorIdioma
    {
        private const string MARCA_TERMINADO = "✓ ";
        private const string MARCA_EN_CURSO = "… ";

        private static readonly Dictionary<string, string> textosIniciales = new Dictionary<string, string>
        {
            { "CONEX_MSG_PROBANDO", "Probando la conexión..." },
            { "CONEX_MSG_LISTA", "Conexión correcta. La base ConectAR_DB está lista: guarde para continuar." },
            { "CONEX_MSG_SIN_BASE", "Conexión correcta, pero la base ConectAR_DB no existe. Presione Crear base de datos." },
            { "CONEX_MSG_SIN_SERVIDOR", "No se pudo conectar con la instancia indicada." },
            { "CONEX_MSG_CREANDO", "Creando la base de datos..." },
            { "CONEX_MSG_SCRIPT", "Ejecutando {0} ({1} de {2})" },
            { "CONEX_MSG_CREADA", "Base de datos creada. Guarde para continuar." },
            { "CONEX_MSG_REINICIO", "Se guardó la conexión. El programa se reinicia para conectarse a la nueva instancia." },
            { "CONEX_ERR_INSTANCIA", "Ingrese o elija una instancia de SQL Server." },
            { "CONEX_ERR_SIN_SCRIPTS", "No se encontraron los scripts de la base de datos junto al programa." },
            { "CONEX_ERR_CREAR", "No se pudo crear la base de datos. Se deshizo la creación: revise la instancia e intente de nuevo." },
            { "CONEX_ERR_BASE_EXISTE", "La base ConectAR_DB ya existe en esa instancia." },
            { "CONEX_ERR_GUARDAR", "No se pudo guardar la configuración de la conexión." },
            { "CONEX_ERR_NO_LISTA", "Antes de guardar, pruebe la conexión y cree la base si hace falta." },
            { "COMUN_MSG_ERROR", "Ocurrió un error. Intente nuevamente." }
        };

        private ConexionBLL_575_AV conexionBLL = new ConexionBLL_575_AV();
        private GestorDeIdioma gestorIdioma;
        private readonly bool desdeMenu;
        private string instanciaOriginal;
        private bool ocupado;

        public frmConexion_575_AV() : this(false) { }

        public frmConexion_575_AV(bool desdeMenu)
        {
            InitializeComponent();
            this.desdeMenu = desdeMenu;
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnProbar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmConexion_Load(object sender, EventArgs e)
        {
            if (desdeMenu)
            {
                gestorIdioma = GestorDeIdioma.GetInstance;
                gestorIdioma.Suscribir(this);
                this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);
            }

            try
            {
                instanciaOriginal = conexionBLL.ObtenerInstanciaActual();
                foreach (string instancia in conexionBLL.ListarInstancias()) { cmbInstancia.Items.Add(instancia); }

                if (!string.IsNullOrWhiteSpace(instanciaOriginal)) { cmbInstancia.Text = instanciaOriginal; }
                else if (cmbInstancia.Items.Count > 0) { cmbInstancia.SelectedIndex = 0; }
            }
            catch (Exception)
            {
                Avisar(() => Texto("COMUN_MSG_ERROR"), true);
            }

            Habilitar(EstadoConexion_575_AV.ServidorNoDisponible, false);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = Texto("CONEX_TITULO", this.Text);
            lblTitulo.Text = Texto("CONEX_TITULO", lblTitulo.Text);
            lblAyuda.Text = Texto("CONEX_LBL_AYUDA", lblAyuda.Text);
            lblInstancia.Text = Texto("CONEX_LBL_INSTANCIA", lblInstancia.Text);
            btnProbar.Text = Texto("CONEX_BTN_PROBAR", btnProbar.Text);
            btnCrear.Text = Texto("CONEX_BTN_CREAR", btnCrear.Text);
            btnGuardar.Text = Texto("CONEX_BTN_GUARDAR", btnGuardar.Text);
            btnSalir.Text = Texto("COMUN_SALIR", btnSalir.Text);
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private string Texto(string clave, string porDefecto = null)
        {
            if (gestorIdioma != null)
            {
                string traducido = gestorIdioma.Traducir(clave);
                if (traducido != clave) { return traducido; }
            }

            string inicial;
            if (textosIniciales.TryGetValue(clave, out inicial)) { return inicial; }
            return porDefecto ?? textosIniciales["COMUN_MSG_ERROR"];
        }

        #endregion

        #region Probar, crear y guardar

        private void cmbInstancia_TextChanged(object sender, EventArgs e)
        {
            if (ocupado) { return; }
            Habilitar(EstadoConexion_575_AV.ServidorNoDisponible, false);
            Avisar(string.Empty, false);
        }

        private async void btnProbar_Click(object sender, EventArgs e)
        {
            string instancia = cmbInstancia.Text;
            Ocupar(true);
            Avisar(() => Texto("CONEX_MSG_PROBANDO"), false);

            try
            {
                EstadoConexion_575_AV estado = await Task.Run(() => conexionBLL.ProbarConexion(instancia));
                MostrarEstado(estado);
            }
            catch (InvalidOperationException ex)
            {
                Avisar(() => Texto(ex.Message), true);
            }
            catch (Exception)
            {
                Avisar(() => Texto("COMUN_MSG_ERROR"), true);
            }
            finally
            {
                Ocupar(false);
            }
        }

        private async void btnCrear_Click(object sender, EventArgs e)
        {
            string instancia = cmbInstancia.Text;
            Ocupar(true);
            lstAvance.Items.Clear();
            pbAvance.Value = 0;
            Avisar(() => Texto("CONEX_MSG_CREANDO"), false);

            Progress<AvanceInstalacion_575_AV> avance = new Progress<AvanceInstalacion_575_AV>(MostrarAvance);
            try
            {
                await Task.Run(() => conexionBLL.CrearBaseDatos(instancia, avance));
                Habilitar(EstadoConexion_575_AV.Lista, true);
                Avisar(() => Texto("CONEX_MSG_CREADA"), false);
            }
            catch (InvalidOperationException ex)
            {
                Avisar(() => Texto(ex.Message), true);
            }
            catch (Exception)
            {
                Avisar(() => Texto("COMUN_MSG_ERROR"), true);
            }
            finally
            {
                Ocupar(false);
            }
        }

        private void MostrarAvance(AvanceInstalacion_575_AV avance)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MostrarAvance(avance)));
                return;
            }

            while (lstAvance.Items.Count < avance.numero) { lstAvance.Items.Add(string.Empty); }

            string actual = lstAvance.Items[avance.numero - 1].ToString();
            if (avance.terminado)
            {
                lstAvance.Items[avance.numero - 1] = MARCA_TERMINADO + avance.script;
            }
            else if (!actual.StartsWith(MARCA_TERMINADO))
            {
                lstAvance.Items[avance.numero - 1] = MARCA_EN_CURSO + avance.script;
                Avisar(() => string.Format(Texto("CONEX_MSG_SCRIPT"), avance.script, avance.numero, avance.total), false);
            }

            int terminados = lstAvance.Items.Cast<object>().Count(i => i.ToString().StartsWith(MARCA_TERMINADO));
            pbAvance.Maximum = avance.total + 1;
            pbAvance.Value = terminados + 1;
            pbAvance.Value = terminados;
            if (terminados == avance.total) { pbAvance.Maximum = avance.total; }
            lstAvance.TopIndex = avance.numero - 1;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string instancia = cmbInstancia.Text.Trim();
            try
            {
                bool cambia = desdeMenu && !string.Equals(instancia, instanciaOriginal, StringComparison.OrdinalIgnoreCase);
                if (cambia)
                {
                    if (conexionBLL.ProbarConexion(instancia) != EstadoConexion_575_AV.Lista)
                    {
                        Avisar(() => Texto("CONEX_ERR_NO_LISTA"), true);
                        return;
                    }
                    if (SessionManager.Logged()) { new UsuarioBLL().Logout(); }
                }

                conexionBLL.GuardarInstancia(instancia);

                if (cambia)
                {
                    MessageBox.Show(Texto("CONEX_MSG_REINICIO"), Texto("CONEX_TITULO", this.Text),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                Avisar(() => Texto(ex.Message), true);
            }
            catch (Exception)
            {
                Avisar(() => Texto("COMUN_MSG_ERROR"), true);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Apoyo

        private void MostrarEstado(EstadoConexion_575_AV estado)
        {
            Habilitar(estado, estado == EstadoConexion_575_AV.Lista);
            switch (estado)
            {
                case EstadoConexion_575_AV.Lista:
                    Avisar(() => Texto("CONEX_MSG_LISTA"), false);
                    break;
                case EstadoConexion_575_AV.BaseInexistente:
                    Avisar(() => Texto("CONEX_MSG_SIN_BASE"), false);
                    break;
                default:
                    Avisar(() => Texto("CONEX_MSG_SIN_SERVIDOR"), true);
                    break;
            }
        }

        private void Habilitar(EstadoConexion_575_AV estado, bool lista)
        {
            EstiloConectAR_575_AV.EstadoBoton(btnCrear, estado == EstadoConexion_575_AV.BaseInexistente);
            EstiloConectAR_575_AV.EstadoBoton(btnGuardar, lista);
        }

        private void Ocupar(bool valor)
        {
            ocupado = valor;
            cmbInstancia.Enabled = !valor;
            btnProbar.Enabled = !valor;
            btnSalir.Enabled = !valor;
            if (valor)
            {
                btnCrear.Enabled = false;
                btnGuardar.Enabled = false;
            }
            UseWaitCursor = valor;
        }

        private Func<string> avisoTraducible;

        private void Avisar(string mensaje, bool esError)
        {
            avisoTraducible = null;
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = esError ? EstiloConectAR_575_AV.Peligro : EstiloConectAR_575_AV.NavySuave;
        }

        private void Avisar(Func<string> mensaje, bool esError)
        {
            Avisar(mensaje(), esError);
            avisoTraducible = mensaje;
        }

        #endregion
    }
}
