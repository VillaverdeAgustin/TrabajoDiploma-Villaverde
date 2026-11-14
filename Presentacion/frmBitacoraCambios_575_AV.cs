using Negocio_BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    // T06.B Bitacora de cambios: pantalla de diseno de la Entrega 3 (CUS-016 Consultar y CUS-017 Restaurar).
    // Todavia sin logica de negocio: muestra cambios de ejemplo en memoria. La consulta real (ArticuloHistorialBLL_575_AV)
    // y la restauracion (ArticuloBLL_575_AV.RestaurarVersion) se programan en la Entrega 3. No esta en el menu.
    public partial class frmBitacoraCambios_575_AV : Form, IObservadorIdioma
    {
        private readonly GestorDeIdioma gestorIdioma = GestorDeIdioma.GetInstance;
        private readonly List<CambioEjemplo> cambios = CambiosDeEjemplo();
        private static readonly Color Resaltado = Color.FromArgb(255, 236, 179);
        private Func<string> avisoTraducible;
        private bool cargando;

        public frmBitacoraCambios_575_AV()
        {
            InitializeComponent();
            EstiloConectAR_575_AV.AplicarFormulario(this);
            EstiloConectAR_575_AV.BotonPrimario(btnBuscar);
            EstiloConectAR_575_AV.BotonPrimario(btnRestaurar);
            EstiloConectAR_575_AV.BotonSecundario(btnSalir);
            lblEjemplo.ForeColor = EstiloConectAR_575_AV.TextoSuave;
            lblLeyenda.BackColor = Resaltado;
        }

        private void frmBitacoraCambios_Load(object sender, EventArgs e)
        {
            gestorIdioma.Suscribir(this);
            this.FormClosed += (s, a) => gestorIdioma.Desuscribir(this);

            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            CargarFiltros();
            Buscar();
        }

        #region Patron Observer - Idiomas

        public void ActualizarTextos()
        {
            this.Text = gestorIdioma.Traducir("HIS_TITULO");
            lblTitulo.Text = gestorIdioma.Traducir("HIS_TITULO");
            lblEjemplo.Text = gestorIdioma.Traducir("HIS_LBL_EJEMPLO");
            gbFiltros.Text = gestorIdioma.Traducir("HIS_GB_FILTROS");
            lblArticulo.Text = gestorIdioma.Traducir("HIS_LBL_ARTICULO");
            lblOperacion.Text = gestorIdioma.Traducir("HIS_LBL_OPERACION");
            lblDesde.Text = gestorIdioma.Traducir("COMUN_DESDE");
            lblHasta.Text = gestorIdioma.Traducir("COMUN_HASTA");
            btnBuscar.Text = gestorIdioma.Traducir("COMUN_BUSCAR");
            gbHistorial.Text = gestorIdioma.Traducir("HIS_GB_HISTORIAL");
            gbComparacion.Text = gestorIdioma.Traducir("HIS_GB_COMPARACION");
            lblLeyenda.Text = gestorIdioma.Traducir("HIS_LBL_LEYENDA");
            btnRestaurar.Text = gestorIdioma.Traducir("HIS_BTN_RESTAURAR");
            btnSalir.Text = gestorIdioma.Traducir("COMUN_SALIR");

            colFecha.HeaderText = gestorIdioma.Traducir("COMUN_FECHA");
            colUsuario.HeaderText = gestorIdioma.Traducir("COMUN_USUARIO");
            colOperacion.HeaderText = gestorIdioma.Traducir("HIS_COL_OPERACION");
            colCodigo.HeaderText = gestorIdioma.Traducir("ART_COL_CODIGO");
            colDescripcion.HeaderText = gestorIdioma.Traducir("ART_COL_DESCRIPCION");
            colCampo.HeaderText = gestorIdioma.Traducir("HIS_COL_CAMPO");
            colAnterior.HeaderText = gestorIdioma.Traducir("HIS_COL_ANTERIOR");
            colPosterior.HeaderText = gestorIdioma.Traducir("HIS_COL_POSTERIOR");

            if (cmbArticulo.Items.Count > 0)
            {
                CargarFiltros();
                foreach (DataGridViewRow fila in dgvHistorial.Rows)
                {
                    fila.Cells[colOperacion.Index].Value = Operacion(((CambioEjemplo)fila.Tag).operacion);
                }
                MostrarSeleccionado();
            }
            lblMensaje.Text = avisoTraducible == null ? string.Empty : avisoTraducible();
        }

        #endregion

        #region Filtros y busqueda

        private void CargarFiltros()
        {
            cargando = true;
            int articulo = Math.Max(cmbArticulo.SelectedIndex, 0);
            int operacion = Math.Max(cmbOperacion.SelectedIndex, 0);

            cmbArticulo.Items.Clear();
            cmbArticulo.Items.Add(gestorIdioma.Traducir("COMUN_TODOS"));
            foreach (string codigo in cambios.Select(c => c.posterior.codigo).Distinct().OrderBy(c => c))
            {
                cmbArticulo.Items.Add(codigo + " - " + cambios.Last(c => c.posterior.codigo == codigo).posterior.descripcion);
            }
            cmbOperacion.Items.Clear();
            cmbOperacion.Items.Add(gestorIdioma.Traducir("COMUN_TODOS"));
            foreach (string op in Operaciones) { cmbOperacion.Items.Add(Operacion(op)); }

            cmbArticulo.SelectedIndex = Math.Min(articulo, cmbArticulo.Items.Count - 1);
            cmbOperacion.SelectedIndex = operacion;
            cargando = false;
        }

        private void Buscar()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                Avisar(() => gestorIdioma.Traducir("HIS_ERR_RANGO"), true);
                return;
            }

            string codigo = cmbArticulo.SelectedIndex > 0 ? cmbArticulo.Text.Split(' ')[0] : null;
            string operacion = cmbOperacion.SelectedIndex > 0 ? Operaciones[cmbOperacion.SelectedIndex - 1] : null;
            DateTime desde = dtpDesde.Value.Date, hasta = dtpHasta.Value.Date.AddDays(1);

            List<CambioEjemplo> encontrados = cambios
                .Where(c => (codigo == null || c.posterior.codigo == codigo) && (operacion == null || c.operacion == operacion)
                         && c.fecha >= desde && c.fecha < hasta)
                .OrderByDescending(c => c.fecha).ToList();

            cargando = true;
            dgvHistorial.Rows.Clear();
            foreach (CambioEjemplo c in encontrados)
            {
                int i = dgvHistorial.Rows.Add(c.fecha.ToString("dd/MM/yyyy HH:mm"), c.usuario, Operacion(c.operacion), c.posterior.codigo, c.posterior.descripcion);
                dgvHistorial.Rows[i].Tag = c;
            }
            cargando = false;
            MostrarSeleccionado();

            int cantidad = encontrados.Count;
            if (cantidad == 0) { Avisar(() => gestorIdioma.Traducir("HIS_MSG_SIN_CAMBIOS"), false); }
            else { Avisar(() => string.Format(gestorIdioma.Traducir("HIS_MSG_ENCONTRADOS"), cantidad), false); }
        }

        private void btnBuscar_Click(object sender, EventArgs e) => Buscar();

        #endregion

        #region Comparacion

        private void dgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            if (!cargando) { MostrarSeleccionado(); }
        }

        private void MostrarSeleccionado()
        {
            CambioEjemplo cambio = Seleccionado();
            dgvComparacion.Rows.Clear();
            EstiloConectAR_575_AV.EstadoBoton(btnRestaurar, cambio != null && cambio.anterior != null);
            if (cambio != null) { MostrarComparacion(cambio.anterior, cambio.posterior); }
        }

        // Una fila por campo maestro del articulo; las que cambiaron entre la imagen anterior y la posterior van resaltadas.
        private void MostrarComparacion(VersionEjemplo anterior, VersionEjemplo posterior)
        {
            foreach (var campo in Campos())
            {
                string antes = anterior == null ? string.Empty : campo.Item2(anterior);
                string despues = campo.Item2(posterior);
                int i = dgvComparacion.Rows.Add(gestorIdioma.Traducir(campo.Item1), antes, despues);
                if (anterior != null && antes != despues)
                {
                    DataGridViewCellStyle estilo = dgvComparacion.Rows[i].DefaultCellStyle;
                    estilo.BackColor = Resaltado;
                    estilo.Font = EstiloConectAR_575_AV.FuenteEtiqueta;
                }
            }
            dgvComparacion.ClearSelection();
        }

        private IEnumerable<Tuple<string, Func<VersionEjemplo, string>>> Campos()
        {
            CultureInfo ar = CultureInfo.GetCultureInfo("es-AR");
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_CODIGO", v => v.codigo);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_DESCRIPCION", v => v.descripcion);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_CATEGORIA", v => v.categoria);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_MARCA", v => v.marca);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_UNIDAD", v => v.unidad);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_PRECIO", v => v.precio_unitario.ToString("N2", ar));
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_REPOSICION", v => v.punto_reposicion.ToString("N2", ar));
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("ART_LBL_UBICACION", v => v.deposito_ubicacion);
            yield return Tuple.Create<string, Func<VersionEjemplo, string>>("HIS_CAMPO_ACTIVO", v => gestorIdioma.Traducir(v.activo ? "HIS_VAL_SI" : "HIS_VAL_NO"));
        }

        #endregion

        #region Restaurar version

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            CambioEjemplo cambio = Seleccionado();
            if (cambio == null || cambio.anterior == null) { return; }

            string pregunta = string.Format(gestorIdioma.Traducir("HIS_MSG_CONFIRMA"), cambio.posterior.codigo, cambio.fecha.ToString("dd/MM/yyyy HH:mm"));
            if (MessageBox.Show(this, pregunta, gestorIdioma.Traducir("COMUN_ATENCION"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Avisar(() => gestorIdioma.Traducir("HIS_MSG_DISENO"), false);
            }
        }

        #endregion

        #region Apoyo

        private CambioEjemplo Seleccionado()
        {
            return dgvHistorial.CurrentRow == null ? null : dgvHistorial.CurrentRow.Tag as CambioEjemplo;
        }

        private static readonly string[] Operaciones = { "Alta", "Modificacion", "Baja", "Restauracion" };

        private string Operacion(string operacion) => gestorIdioma.Traducir("HIS_OP_" + operacion);

        private void Avisar(Func<string> mensaje, bool esError)
        {
            avisoTraducible = mensaje;
            lblMensaje.Text = mensaje();
            lblMensaje.ForeColor = esError ? EstiloConectAR_575_AV.Peligro : EstiloConectAR_575_AV.NavySuave;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Datos de ejemplo (en la Entrega 3 salen de ArticuloHistorial_575_AV)

        private sealed class VersionEjemplo
        {
            public string codigo, descripcion, categoria, marca, unidad, deposito_ubicacion;
            public decimal precio_unitario, punto_reposicion;
            public bool activo = true;

            public VersionEjemplo Con(Action<VersionEjemplo> cambio)
            {
                VersionEjemplo copia = (VersionEjemplo)MemberwiseClone();
                cambio(copia);
                return copia;
            }
        }

        private sealed class CambioEjemplo
        {
            public DateTime fecha;
            public string usuario, operacion;
            public VersionEjemplo anterior, posterior;
        }

        private static List<CambioEjemplo> CambiosDeEjemplo()
        {
            DateTime hoy = DateTime.Today;
            var cable = new VersionEjemplo { codigo = "CAB-UTP6-EXT", descripcion = "Cable UTP Cat 6 exterior CMX", categoria = "Cable de red", marca = "Furukawa", unidad = "Metro lineal", precio_unitario = 1350m, punto_reposicion = 200m, deposito_ubicacion = "Pasillo 1 - Est. B" };
            var patchera = new VersionEjemplo { codigo = "PAT-24-C6", descripcion = "Patchera 24 bocas Cat 6", categoria = "Conectividad", marca = "Panduit", unidad = "Unidad", precio_unitario = 92000m, punto_reposicion = 5m, deposito_ubicacion = "Pasillo 2 - Est. A" };
            var jack = new VersionEjemplo { codigo = "JCK-RJ45-C6", descripcion = "Jack RJ45 Cat 6 keystone", categoria = "Conectividad", marca = "Panduit", unidad = "Unidad", precio_unitario = 6250m, punto_reposicion = 50m, deposito_ubicacion = "Pasillo 2 - Est. B" };
            var rack = new VersionEjemplo { codigo = "RCK-12U-MUR", descripcion = "Rack mural 12U de 570 mm", categoria = "Racks y gabinetes", marca = "Legrand", unidad = "Unidad", precio_unitario = 315000m, punto_reposicion = 4m, deposito_ubicacion = "Deposito - Sector G" };
            var canaleta = new VersionEjemplo { codigo = "CAN-40X25", descripcion = "Canaleta PVC 40x25 mm por 2 m", categoria = "Canalizacion", marca = "Generico", unidad = "Unidad", precio_unitario = 7200m, punto_reposicion = 50m, deposito_ubicacion = "Pasillo 4 - Est. B" };

            var jackCaro = jack.Con(v => v.precio_unitario = 6900m);
            var patcheraNueva = patchera.Con(v => { v.descripcion = "Patchera 24 bocas Cat 6 de 19 pulgadas"; v.precio_unitario = 98500m; });
            var cableNuevo = cable.Con(v => { v.precio_unitario = 1480m; v.deposito_ubicacion = "Pasillo 1 - Est. A"; });

            return new List<CambioEjemplo>
            {
                new CambioEjemplo { fecha = hoy.AddDays(-26).AddHours(8.5),  usuario = "admin",    operacion = "Alta",         anterior = null,          posterior = rack },
                new CambioEjemplo { fecha = hoy.AddDays(-22).AddHours(11),   usuario = "admin",    operacion = "Modificacion", anterior = jack,          posterior = jackCaro },
                new CambioEjemplo { fecha = hoy.AddDays(-18).AddHours(18.3), usuario = "admin",    operacion = "Restauracion", anterior = jackCaro,      posterior = jack },
                new CambioEjemplo { fecha = hoy.AddDays(-12).AddHours(9.25), usuario = "admin",    operacion = "Modificacion", anterior = patchera,      posterior = patcheraNueva },
                new CambioEjemplo { fecha = hoy.AddDays(-9).AddHours(12.5),  usuario = "admin",    operacion = "Alta",         anterior = null,          posterior = canaleta },
                new CambioEjemplo { fecha = hoy.AddDays(-5).AddHours(16.1),  usuario = "admin",    operacion = "Baja",         anterior = canaleta,      posterior = canaleta.Con(v => v.activo = false) },
                new CambioEjemplo { fecha = hoy.AddDays(-2).AddHours(10.7),  usuario = "deposito", operacion = "Modificacion", anterior = cable,         posterior = cableNuevo },
            };
        }

        #endregion
    }
}
