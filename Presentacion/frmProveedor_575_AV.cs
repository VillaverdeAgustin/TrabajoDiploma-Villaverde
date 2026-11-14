using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmProveedor_575_AV : Form, IObservadorIdioma
    {
        private const string CORREO_OCULTO = "••••••••";

        private ProveedorBLL_575_AV proveedorBLL = new ProveedorBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private ProveedorBE_575_AV proveedorActual;
        private bool correoModificado;
        private bool cargando;

        public ProveedorBE_575_AV ProveedorRegistrado { get; private set; }

        public frmProveedor_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonSecundario(btnNuevo);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
        }

        private void frmProveedor_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            CargarCondicionesIva();
            try
            {
                bitacora.RegistrarBitacora(UsuarioActual(), TipoAccion.GestionProveedoresAbierta);
                CargarProveedores();
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
            PrepararAlta();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("PRV_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("PRV_TITULO");
            gbLista.Text = gestorIdioma.Traducir("PRV_GB_LISTA");
            gbDatos.Text = gestorIdioma.Traducir("PRV_GB_DATOS");
            lblRazonSocial.Text = gestorIdioma.Traducir("PRV_LBL_RAZON_SOCIAL");
            lblCuit.Text = gestorIdioma.Traducir("PRV_LBL_CUIT");
            lblCondicionIva.Text = gestorIdioma.Traducir("PRV_LBL_CONDICION_IVA");
            lblRubro.Text = gestorIdioma.Traducir("PRV_LBL_RUBRO");
            lblTelefono.Text = gestorIdioma.Traducir("PRV_LBL_TELEFONO");
            lblCorreo.Text = gestorIdioma.Traducir("PRV_LBL_CORREO");
            lblDireccion.Text = gestorIdioma.Traducir("PRV_LBL_DIRECCION");
            chkActivo.Text = gestorIdioma.Traducir("PRV_CHK_ACTIVO");
            btnNuevo.Text = gestorIdioma.Traducir("COMUN_NUEVO");
            btnGuardar.Text = gestorIdioma.Traducir("COMUN_GUARDAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");
            CargarCondicionesIva();
            MostrarModo();
            TraducirColumnas();
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        private void TraducirColumnas()
        {
            Titulo("razon_social", "PRV_LBL_RAZON_SOCIAL");
            Titulo("cuit", "PRV_LBL_CUIT");
            Titulo("condicion_iva", "PRV_LBL_CONDICION_IVA");
            Titulo("rubro", "PRV_LBL_RUBRO");
            Titulo("telefono", "PRV_LBL_TELEFONO");
            Titulo("activo", "PRV_CHK_ACTIVO");
        }

        private void Titulo(string columna, string clave)
        {
            if (dgvProveedores.Columns[columna] != null)
            {
                dgvProveedores.Columns[columna].HeaderText = gestorIdioma.Traducir(clave);
            }
        }

        #endregion

        #region Carga

        private void CargarProveedores()
        {
            dgvProveedores.DataSource = proveedorBLL.ListarProveedores();
            EstiloConectAR_575_AV.MostrarColumnas(dgvProveedores,
                "razon_social", "cuit", "condicion_iva", "rubro", "telefono", "activo");
            EstiloConectAR_575_AV.ColumnaElastica(dgvProveedores, "razon_social");
            TraducirColumnas();
        }

        private void CargarCondicionesIva()
        {
            object elegida = cmbCondicionIva.SelectedValue;
            cmbCondicionIva.DataSource = Enum.GetValues(typeof(CondicionIva_575_AV))
                .Cast<CondicionIva_575_AV>()
                .Where(c => c != CondicionIva_575_AV.ConsumidorFinal)
                .Select(c => new { Valor = c, Texto = gestorIdioma.Traducir("CLI_IVA_" + c) })
                .ToList();
            cmbCondicionIva.DisplayMember = "Texto";
            cmbCondicionIva.ValueMember = "Valor";
            if (elegida != null) { cmbCondicionIva.SelectedValue = elegida; }
        }

        private void dgvProveedores_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProveedores.Columns[e.ColumnIndex].Name == "condicion_iva" && e.Value != null)
            {
                e.Value = gestorIdioma.Traducir("CLI_IVA_" + e.Value);
                e.FormattingApplied = true;
            }
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) { return; }
            ProveedorBE_575_AV proveedor = dgvProveedores.Rows[e.RowIndex].DataBoundItem as ProveedorBE_575_AV;
            if (proveedor != null)
            {
                PrepararModificacion(proveedor);
                Avisar(string.Empty, false);
            }
        }

        #endregion

        #region Alta y modificacion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararAlta();
            Avisar(string.Empty, false);
            txtRazonSocial.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ProveedorBE_575_AV proveedor = new ProveedorBE_575_AV
            {
                id_proveedor = proveedorActual == null ? 0 : proveedorActual.id_proveedor,
                razon_social = txtRazonSocial.Text,
                cuit = txtCuit.Text,
                condicion_iva = cmbCondicionIva.SelectedValue is CondicionIva_575_AV
                    ? (CondicionIva_575_AV)cmbCondicionIva.SelectedValue
                    : CondicionIva_575_AV.ResponsableInscripto,
                rubro = txtRubro.Text,
                telefono = txtTelefono.Text,
                direccion = txtDireccion.Text,
                activo = proveedorActual == null || chkActivo.Checked,
                correo_electronico = proveedorActual != null && !correoModificado
                    ? proveedorActual.correo_electronico
                    : txtCorreo.Text.Trim()
            };

            try
            {
                if (proveedorActual == null)
                {
                    proveedorBLL.CrearProveedor(proveedor, UsuarioActual());
                    ProveedorRegistrado = proveedor;
                    TerminarGuardado(proveedor.cuit, "PRV_MSG_REGISTRADO");
                }
                else
                {
                    proveedorBLL.ModificarProveedor(proveedor, UsuarioActual());
                    TerminarGuardado(proveedor.cuit, "PRV_MSG_ACTUALIZADO");
                }
            }
            catch (InvalidOperationException ex)
            {
                ProveedorBE_575_AV existente = proveedorActual == null ? proveedorBLL.BuscarPorCuit(txtCuit.Text) : null;
                if (existente != null &&
                    ex.Message == string.Format(gestorIdioma.Traducir("PRV_ERR_CUIT_EXISTE"), existente.cuit))
                {
                    PrepararModificacion(existente);
                    Avisar(() => string.Format(gestorIdioma.Traducir("PRV_MSG_CUIT_REGISTRADO"), existente.razon_social), true);
                }
                else
                {
                    Avisar(ex.Message, true);
                }
            }
            catch (Exception)
            {
                Avisar(() => gestorIdioma.Traducir("COMUN_MSG_ERROR"), true);
            }
        }

        private void TerminarGuardado(string cuit, string clave)
        {
            CargarProveedores();
            ProveedorBE_575_AV guardado = proveedorBLL.BuscarPorCuit(cuit);
            if (guardado != null)
            {
                PrepararModificacion(guardado);
                SeleccionarFila(guardado.id_proveedor);
            }
            Avisar(() => gestorIdioma.Traducir(clave), false);
        }

        private void SeleccionarFila(int id_proveedor)
        {
            foreach (DataGridViewRow fila in dgvProveedores.Rows)
            {
                ProveedorBE_575_AV proveedor = fila.DataBoundItem as ProveedorBE_575_AV;
                if (proveedor != null && proveedor.id_proveedor == id_proveedor)
                {
                    dgvProveedores.CurrentCell = fila.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
                    return;
                }
            }
        }

        private void PrepararAlta()
        {
            cargando = true;
            proveedorActual = null;
            txtRazonSocial.Clear();
            txtCuit.Clear();
            txtRubro.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtDireccion.Clear();
            if (cmbCondicionIva.Items.Count > 0) { cmbCondicionIva.SelectedIndex = 0; }
            chkActivo.Checked = true;
            chkActivo.Enabled = false;
            correoModificado = true;
            cargando = false;
            MostrarModo();
        }

        private void PrepararModificacion(ProveedorBE_575_AV proveedor)
        {
            cargando = true;
            proveedorActual = proveedor;
            txtRazonSocial.Text = proveedor.razon_social;
            txtCuit.Text = proveedor.cuit;
            cmbCondicionIva.SelectedValue = proveedor.condicion_iva;
            txtRubro.Text = proveedor.rubro;
            txtTelefono.Text = proveedor.telefono;
            txtCorreo.Text = string.IsNullOrEmpty(proveedor.correo_electronico) ? string.Empty : CORREO_OCULTO;
            txtDireccion.Text = proveedor.direccion;
            chkActivo.Checked = proveedor.activo;
            chkActivo.Enabled = true;
            correoModificado = false;
            cargando = false;
            MostrarModo();
        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            if (!cargando) { correoModificado = true; }
        }

        private void MostrarModo()
        {
            lblModo.Text = proveedorActual == null
                ? gestorIdioma.Traducir("PRV_LBL_MODO_ALTA")
                : string.Format(gestorIdioma.Traducir("PRV_LBL_MODO_MODIFICACION"), proveedorActual.razon_social);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Apoyo

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

        private static string UsuarioActual()
        {
            return SessionManager.Logged() ? SessionManager.GetInstance.UsuarioActual().user : "sin sesion";
        }

        #endregion
    }
}
