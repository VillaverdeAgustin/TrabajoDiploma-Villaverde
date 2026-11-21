using Entidad_BE;
using Negocio_BLL;
using Servicios;
using System.Drawing;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public partial class frmMenu : Form, IObservadorIdioma
    {
        public frmMenu()
        {
            InitializeComponent();
            AyudaFormulario_575_AV.Asociar(this, "menu");
            EstiloConectAR_575_AV.AplicarFormulario(this);
            AplicarIdentidad();
        }

        private void AplicarIdentidad()
        {
            lblUsuario.BackColor = EstiloConectAR_575_AV.Navy;
            lblUsuario.ForeColor = Color.White;
            lblUsuario.Font = EstiloConectAR_575_AV.FuenteEtiqueta;

            foreach (Control ctrl in this.Controls)
            {
                MdiClient areaMdi = ctrl as MdiClient;
                if (areaMdi == null) { continue; }

                areaMdi.BackColor = EstiloConectAR_575_AV.Fondo;
                areaMdi.BackgroundImage = Properties.Resources.MarcaAguaConectAR;
                areaMdi.BackgroundImageLayout = ImageLayout.Center;
            }
        }

        public static ToolStripMenuItem opcActivo = null;
        public static Form formActivo = null;
        UsuarioBLL usuario = new UsuarioBLL();
        BitacoraBLL bitacora = new BitacoraBLL();
        VerificadorIntegridadBLL verIntegridad = new VerificadorIntegridadBLL();
        PerfilBLL perfilBLL = new PerfilBLL();
        GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;

        private void frmMenu_Load(object sender, System.EventArgs e)
        {
            if (SessionManager.Logged())
            {
                FormConectado();
            }
            else
            {
                FormDesconectado();
            }
            CargarMenuIdiomas();
            gestorIdioma.Suscribir(this);
            this.FormClosed += frmMenu_FormClosed;
        }

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            gestorIdioma.Desuscribir(this);
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            tsInicio.Text = gestorIdioma.Traducir("MENU_TS_INICIO");
            iniciarSesionToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_INICIAR_SESION");
            cerrarSesionToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CERRAR_SESION");
            salirToolStripMenuItem.Text = gestorIdioma.Traducir("COMUN_SALIR");
            tsAdmin.Text = gestorIdioma.Traducir("MENU_TS_ADMIN");
            gestionDeUsuariosToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_GESTION_USUARIOS");
            tsPerfiles.Text = gestorIdioma.Traducir("MENU_PERFILES");
            idiomasToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_IDIOMAS");
            backupToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_BACKUP");
            restoreToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_RESTORE");
            bitacoraToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_BITACORA");
            bitacoraCambiosToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_BITACORA_CAMBIOS");
            conexionToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CONEXION");
            tsUsuario.Text = gestorIdioma.Traducir("COMUN_USUARIO");
            cambiarClaveToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CAMBIAR_CLAVE");
            cambiarIdiomaToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CAMBIAR_IDIOMA");
            tsVentas.Text = gestorIdioma.Traducir("MENU_TS_VENTAS");
            tsCaja.Text = gestorIdioma.Traducir("MENU_TS_CAJA");
            tsDeposito.Text = gestorIdioma.Traducir("MENU_TS_DEPOSITO");
            ordenPagoToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_ORDEN_PAGO");
            entregaDepositoToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_ENTREGA_DEPOSITO");
            carritoComprasToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CARRITO_COMPRAS");
            validarCobrarToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_VALIDAR_COBRAR");
            consultarComprobantesToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CONSULTAR_COMPROBANTES");
            tsGestion.Text = gestorIdioma.Traducir("MENU_TS_GESTION");
            clienteToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_CLIENTE");
            articulosToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_ARTICULOS");
            proveedoresToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_PROVEEDORES");
            tsCompras.Text = gestorIdioma.Traducir("MENU_TS_COMPRAS");
            importarListaToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_IMPORTAR_LISTA");
            compararPreciosToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_COMPARAR_PRECIOS");
            ordenesCompraToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_ORDENES_COMPRA");
            recepcionToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_RECEPCION");
            reporteComprasToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_REPORTE_COMPRAS");
            serializacionToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_SERIALIZACION");
            tsReportes.Text = gestorIdioma.Traducir("MENU_TS_REPORTES");
            tsAyuda.Text = gestorIdioma.Traducir("MENU_TS_AYUDA");
            contenidoAyudaToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_AYUDA_CONTENIDO");
            historialToolStripMenuItem.Text = gestorIdioma.Traducir("MENU_HISTORIAL");

            if (SessionManager.Logged() && SessionManager.GetInstance.UsuarioActual() != null)
            {
                lblUsuario.Text = gestorIdioma.Traducir("MENU_LBL_USUARIO") + SessionManager.GetInstance.UsuarioActual().user;
            }
            else
            {
                lblUsuario.Text = gestorIdioma.Traducir("MENU_LBL_SIN_CONEXION");
            }
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

        private void itemIdioma_Click(object sender, System.EventArgs e)
        {
            try
            {
                gestorIdioma.CambiarIdioma((sender as ToolStripMenuItem).Tag.ToString());
                string user = SessionManager.Logged() ? SessionManager.GetInstance.UsuarioActual().user : "null";
                bitacora.RegistrarBitacora(user, TipoAccion.CambioIdioma);
            }
            catch (System.Exception)
            {
                MessageBox.Show(gestorIdioma.Traducir("COMUN_ERROR_BD"), gestorIdioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Set Formulario
        public void FormConectado()
        {
            UsuarioBE usActual = SessionManager.GetInstance.UsuarioActual();
            lblUsuario.Text = gestorIdioma.Traducir("MENU_LBL_USUARIO") + usActual.user;
            iniciarSesionToolStripMenuItem.Enabled = false;
            cerrarSesionToolStripMenuItem.Enabled = true;
            tsAdmin.Enabled = true;
            tsGestion.Enabled = true;
            tsVentas.Enabled = true;
            cambiarClaveToolStripMenuItem.Enabled = true;

            AplicarPermisos(usActual.rol);
            conexionToolStripMenuItem.Enabled = usuario.EsAdministrador(usActual);
        }

        private void AplicarPermisos(string rol)
        {
            gestionDeUsuariosToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionUsuarios.ToString());
            historialToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionUsuarios.ToString());
            tsPerfiles.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionPerfiles.ToString());
            backupToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionBackup.ToString());
            restoreToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionBackup.ToString());
            bitacoraToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionBitacora.ToString());
            bitacoraCambiosToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.BitacoraCambios.ToString());
            carritoComprasToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.CompletarCarrito.ToString());
            validarCobrarToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.ValidarPedidoYCobrar.ToString());
            consultarComprobantesToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.ValidarPedidoYCobrar.ToString());
            articulosToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GestionArticulos.ToString());
            ordenPagoToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GenerarOrdenPago.ToString());
            entregaDepositoToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.EntregarArticulos.ToString());
            tsCaja.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.ValidarPedidoYCobrar.ToString());
            tsDeposito.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.EntregarArticulos.ToString());
            proveedoresToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.RegistrarProveedor.ToString());
            importarListaToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.ImportarListaPrecios.ToString());
            compararPreciosToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.CompararPrecios.ToString());
            ordenesCompraToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.GenerarOrdenCompra.ToString());
            recepcionToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.RegistrarRecepcion.ToString());
            reporteComprasToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.ReporteCompras.ToString());
            serializacionToolStripMenuItem.Enabled = perfilBLL.TienePermiso(rol, TipoPermiso.SerializarOrdenCompra.ToString());
            tsCompras.Enabled = proveedoresToolStripMenuItem.Enabled || importarListaToolStripMenuItem.Enabled
                || compararPreciosToolStripMenuItem.Enabled || ordenesCompraToolStripMenuItem.Enabled
                || recepcionToolStripMenuItem.Enabled || serializacionToolStripMenuItem.Enabled;
            tsReportes.Enabled = reporteComprasToolStripMenuItem.Enabled;

            clienteToolStripMenuItem.Visible = false;
        }

        public void FormDesconectado()
        {
            lblUsuario.Text = gestorIdioma.Traducir("MENU_LBL_SIN_CONEXION");
            iniciarSesionToolStripMenuItem.Enabled = true;
            cerrarSesionToolStripMenuItem.Visible = false;
            tsAdmin.Visible = false;
            tsReportes.Visible = false;
            tsGestion.Visible = false;
            tsVentas.Visible = false;
            tsCaja.Visible = false;
            tsDeposito.Visible = false;
            tsCompras.Visible = false;
            cambiarClaveToolStripMenuItem.Visible = false;
        }

        #endregion

        private void OpenForm(ToolStripMenuItem opc, Form frm)
        {
            if (opcActivo != null)
            {
                opcActivo.BackColor = EstiloConectAR_575_AV.Navy;
            }
            opc.BackColor = EstiloConectAR_575_AV.Cian;
            opcActivo = opc;

            if (formActivo != null) { formActivo.Close(); }
            formActivo = frm;
            frm.MdiParent = this;
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            frm.Show();
        }

        private void gestionDeUsuariosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.GestionUsuariosAbierta);
            OpenForm(tsAdmin, new frmUsuario());
        }

        private void salirToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(gestorIdioma.Traducir("COMUN_CONFIRMA_SALIR_APP"), gestorIdioma.Traducir("COMUN_ATENCION"),
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.AppClose);
                if (SessionManager.Logged())
                {
                    verIntegridad.ActualizarDVV();
                }            
                SessionManager.GetInstance.Logout();
                Application.Exit();
            }
        } 

        private void cerrarSesionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(gestorIdioma.Traducir("COMUN_CONFIRMA_CERRAR_SESION"), gestorIdioma.Traducir("COMUN_ATENCION"),
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UsuarioBLL usuario = new UsuarioBLL();
                usuario.Logout();

                frmLogin frm = new frmLogin();
                frm.Show();
                this.Close();
            }
        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.Show();
            this.Close();
        }

        private void cambiarClaveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmModClave frmClave = new frmModClave();
            frmClave.ShowDialog();
        }

        private void bitacoraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.BitacoraAbierta);
            OpenForm(tsAdmin, new frmBitacora());
        }

        private void contenidoAyudaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AyudaFormulario_575_AV.MostrarIndice();
        }

        private void bitacoraCambiosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsGestion, new frmBitacoraCambios_575_AV());
        }

        private void perfilesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmGestionPerfiles frmperfil = new frmGestionPerfiles();
            frmperfil.ShowDialog();
        }

        private void historialToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmHistorialUsuario frmhistorial = new frmHistorialUsuario();
            frmhistorial.ShowDialog();
        }

        #region RFN1 - Gestion de Venta por mostrador

        private void articulosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsGestion, new frmGestionArticulos_575_AV());
        }

        private void carritoComprasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsVentas, new frmCarritoCompras_575_AV());
        }

        private void ordenPagoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsVentas, new frmOrdenPago_575_AV());
        }

        private void validarCobrarToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCaja, new frmValidarCobrar_575_AV());
        }

        private void consultarComprobantesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCaja, new frmConsultaComprobantes_575_AV());
        }

        private void proveedoresToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmProveedor_575_AV());
        }

        private void importarListaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmImportarListaPrecios_575_AV());
        }

        private void compararPreciosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmCompararPrecios_575_AV());
        }

        private void conexionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            using (frmConexion_575_AV conexion = new frmConexion_575_AV(true))
            {
                conexion.ShowDialog(this);
            }
        }

        private void ordenesCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmOrdenCompra_575_AV());
        }

        private void recepcionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmRecepcionMercaderia_575_AV());
        }

        private void backupToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsAdmin, new frmBackup_575_AV());
        }

        // T07 - Despues de un restore la sesion ya esta cerrada: se cierra el menu y se vuelve al login
        private void restoreToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmRestore_575_AV frm = new frmRestore_575_AV();
            frm.RestauracionRealizada += (s, a) => this.BeginInvoke(new System.Action(this.Close));
            OpenForm(tsAdmin, frm);
        }

        private void reporteComprasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsReportes, new frmReporteCompras_575_AV());
        }

        private void serializacionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsCompras, new frmSerializacion_575_AV());
        }

        private void entregaDepositoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenForm(tsDeposito, new frmEntregaDeposito_575_AV());
        }

        #endregion

        private void idiomasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            frmIdiomas frmIdiomas = new frmIdiomas();
            frmIdiomas.ShowDialog();
            CargarMenuIdiomas();
            MarcarIdiomaActivo();
        }
    }
}
