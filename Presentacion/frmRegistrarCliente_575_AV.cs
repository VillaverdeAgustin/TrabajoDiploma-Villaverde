using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmRegistrarCliente_575_AV : Form, IObservadorIdioma
    {
        private ClienteBLL_575_AV clienteBLL = new ClienteBLL_575_AV();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        public ClienteBE_575_AV ClienteRegistrado { get; private set; }

        public frmRegistrarCliente_575_AV() : this(null) { }

        public frmRegistrarCliente_575_AV(string dni)
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);

            if (!string.IsNullOrWhiteSpace(dni))
            {
                txtDni.Text = dni.Trim();
                txtDni.ReadOnly = true;
                EstiloConectAR_575_AV.CampoBloqueado(txtDni);
            }

            EstiloConectAR_575_AV.BotonPrimario(btnRegistrar);
            EstiloConectAR_575_AV.BotonPeligro(btnCancelar);
        }

        private void frmRegistrarCliente_Load(object sender, EventArgs e)
        {
            CargarCondicionesIva();
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            txtNombre.Focus();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("CLI_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("CLI_TITULO");
            lblDni.Text = gestorIdioma.Traducir("CLI_LBL_DNI");
            lblNombre.Text = gestorIdioma.Traducir("COMUN_NOMBRE");
            lblApellido.Text = gestorIdioma.Traducir("COMUN_APELLIDO");
            lblRazonSocial.Text = gestorIdioma.Traducir("CLI_LBL_RAZON_SOCIAL");
            lblCondicionIva.Text = gestorIdioma.Traducir("CLI_LBL_CONDICION_IVA");
            lblCuit.Text = gestorIdioma.Traducir("CLI_LBL_CUIT");
            lblDireccion.Text = gestorIdioma.Traducir("CLI_LBL_DIRECCION");
            lblTelefono.Text = gestorIdioma.Traducir("CLI_LBL_TELEFONO");
            lblCorreo.Text = gestorIdioma.Traducir("CLI_LBL_CORREO");
            btnRegistrar.Text = gestorIdioma.Traducir("CLI_BTN_REGISTRAR");
            btnCancelar.Text = gestorIdioma.Traducir("COMUN_CANCELAR");
            CargarCondicionesIva();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        #endregion

        private void CargarCondicionesIva()
        {
            object elegida = cmbCondicionIva.SelectedValue;

            cmbCondicionIva.DataSource = Enum.GetValues(typeof(CondicionIva_575_AV))
                .Cast<CondicionIva_575_AV>()
                .Select(c => new { Valor = c, Texto = gestorIdioma.Traducir("CLI_IVA_" + c) })
                .ToList();
            cmbCondicionIva.DisplayMember = "Texto";
            cmbCondicionIva.ValueMember = "Valor";

            if (elegida != null) { cmbCondicionIva.SelectedValue = elegida; }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            ClienteBE_575_AV cliente = new ClienteBE_575_AV
            {
                dni = txtDni.Text.Trim(),
                nombre = txtNombre.Text.Trim(),
                apellido = txtApellido.Text.Trim(),
                razon_social = txtRazonSocial.Text.Trim(),
                condicion_iva = (CondicionIva_575_AV)cmbCondicionIva.SelectedValue,
                cuit = txtCuit.Text.Trim(),
                direccion = txtDireccion.Text.Trim(),
                telefono = txtTelefono.Text.Trim(),
                correo_electronico = txtCorreo.Text.Trim()
            };

            try
            {
                clienteBLL.CrearCliente(cliente, UsuarioActual());

                ClienteRegistrado = clienteBLL.BuscarPorDni(cliente.dni);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                Avisar(ex.Message);
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"));
            }
        }

        private Func<string> avisoTraducible;

        private void Avisar(string mensaje)
        {
            avisoTraducible = null;
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = EstiloConectAR_575_AV.Peligro;
        }

        private void Avisar(Func<string> mensaje)
        {
            Avisar(mensaje());
            avisoTraducible = mensaje;
        }

        private static string UsuarioActual()
        {
            return SessionManager.Logged()
                ? SessionManager.GetInstance.UsuarioActual().user
                : "sin sesion";
        }
    }
}
