namespace Presentacion_575_AV
{
    partial class frmBitacoraCambios_575_AV
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.lblArticulo = new System.Windows.Forms.Label();
            this.cmbArticulo = new System.Windows.Forms.ComboBox();
            this.lblOperacion = new System.Windows.Forms.Label();
            this.cmbOperacion = new System.Windows.Forms.ComboBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.gbHistorial = new System.Windows.Forms.GroupBox();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbComparacion = new System.Windows.Forms.GroupBox();
            this.dgvComparacion = new System.Windows.Forms.DataGridView();
            this.colCampo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnterior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosterior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblLeyenda = new System.Windows.Forms.Label();
            this.rbAnterior = new System.Windows.Forms.RadioButton();
            this.rbPosterior = new System.Windows.Forms.RadioButton();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.gbFiltros.SuspendLayout();
            this.gbHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.gbComparacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComparacion)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(220, 17);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Bitácora de cambios de artículos";
            //
            // gbFiltros
            //
            this.gbFiltros.Controls.Add(this.lblArticulo);
            this.gbFiltros.Controls.Add(this.cmbArticulo);
            this.gbFiltros.Controls.Add(this.lblOperacion);
            this.gbFiltros.Controls.Add(this.cmbOperacion);
            this.gbFiltros.Controls.Add(this.lblDesde);
            this.gbFiltros.Controls.Add(this.dtpDesde);
            this.gbFiltros.Controls.Add(this.lblHasta);
            this.gbFiltros.Controls.Add(this.dtpHasta);
            this.gbFiltros.Controls.Add(this.btnBuscar);
            this.gbFiltros.Location = new System.Drawing.Point(16, 40);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(1088, 92);
            this.gbFiltros.TabIndex = 2;
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "Filtros";
            //
            // lblArticulo
            //
            this.lblArticulo.Location = new System.Drawing.Point(14, 26);
            this.lblArticulo.Name = "lblArticulo";
            this.lblArticulo.Size = new System.Drawing.Size(330, 19);
            this.lblArticulo.TabIndex = 0;
            this.lblArticulo.Text = "Artículo:";
            //
            // cmbArticulo
            //
            this.cmbArticulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArticulo.Location = new System.Drawing.Point(16, 50);
            this.cmbArticulo.Name = "cmbArticulo";
            this.cmbArticulo.Size = new System.Drawing.Size(340, 26);
            this.cmbArticulo.TabIndex = 1;
            //
            // lblOperacion
            //
            this.lblOperacion.Location = new System.Drawing.Point(374, 26);
            this.lblOperacion.Name = "lblOperacion";
            this.lblOperacion.Size = new System.Drawing.Size(180, 19);
            this.lblOperacion.TabIndex = 2;
            this.lblOperacion.Text = "Operación:";
            //
            // cmbOperacion
            //
            this.cmbOperacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperacion.Location = new System.Drawing.Point(376, 50);
            this.cmbOperacion.Name = "cmbOperacion";
            this.cmbOperacion.Size = new System.Drawing.Size(180, 26);
            this.cmbOperacion.TabIndex = 3;
            //
            // lblDesde
            //
            this.lblDesde.Location = new System.Drawing.Point(574, 26);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(140, 19);
            this.lblDesde.TabIndex = 4;
            this.lblDesde.Text = "Desde:";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(576, 50);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(140, 25);
            this.dtpDesde.TabIndex = 5;
            //
            // lblHasta
            //
            this.lblHasta.Location = new System.Drawing.Point(734, 26);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(140, 19);
            this.lblHasta.TabIndex = 6;
            this.lblHasta.Text = "Hasta:";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(736, 50);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(140, 25);
            this.dtpHasta.TabIndex = 7;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(904, 46);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(168, 32);
            this.btnBuscar.TabIndex = 8;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // gbHistorial
            //
            this.gbHistorial.Controls.Add(this.dgvHistorial);
            this.gbHistorial.Location = new System.Drawing.Point(16, 142);
            this.gbHistorial.Name = "gbHistorial";
            this.gbHistorial.Size = new System.Drawing.Size(610, 404);
            this.gbHistorial.TabIndex = 3;
            this.gbHistorial.TabStop = false;
            this.gbHistorial.Text = "Cambios registrados";
            //
            // dgvHistorial
            //
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.AllowUserToResizeRows = false;
            this.dgvHistorial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFecha,
            this.colUsuario,
            this.colOperacion,
            this.colCodigo,
            this.colDescripcion});
            this.dgvHistorial.Location = new System.Drawing.Point(12, 28);
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.Size = new System.Drawing.Size(586, 362);
            this.dgvHistorial.TabIndex = 0;
            this.dgvHistorial.SelectionChanged += new System.EventHandler(this.dgvHistorial_SelectionChanged);
            //
            // colFecha
            //
            this.colFecha.HeaderText = "Fecha";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            this.colFecha.Width = 132;
            //
            // colUsuario
            //
            this.colUsuario.HeaderText = "Usuario";
            this.colUsuario.Name = "colUsuario";
            this.colUsuario.ReadOnly = true;
            this.colUsuario.Width = 86;
            //
            // colOperacion
            //
            this.colOperacion.HeaderText = "Operación";
            this.colOperacion.Name = "colOperacion";
            this.colOperacion.ReadOnly = true;
            this.colOperacion.Width = 112;
            //
            // colCodigo
            //
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.ReadOnly = true;
            this.colCodigo.Width = 116;
            //
            // colDescripcion
            //
            this.colDescripcion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.Name = "colDescripcion";
            this.colDescripcion.ReadOnly = true;
            //
            // gbComparacion
            //
            this.gbComparacion.Controls.Add(this.dgvComparacion);
            this.gbComparacion.Controls.Add(this.lblLeyenda);
            this.gbComparacion.Controls.Add(this.rbAnterior);
            this.gbComparacion.Controls.Add(this.rbPosterior);
            this.gbComparacion.Location = new System.Drawing.Point(638, 142);
            this.gbComparacion.Name = "gbComparacion";
            this.gbComparacion.Size = new System.Drawing.Size(466, 404);
            this.gbComparacion.TabIndex = 4;
            this.gbComparacion.TabStop = false;
            this.gbComparacion.Text = "Valor anterior / Valor posterior";
            //
            // dgvComparacion
            //
            this.dgvComparacion.AllowUserToAddRows = false;
            this.dgvComparacion.AllowUserToDeleteRows = false;
            this.dgvComparacion.AllowUserToResizeRows = false;
            this.dgvComparacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCampo,
            this.colAnterior,
            this.colPosterior});
            this.dgvComparacion.Location = new System.Drawing.Point(12, 28);
            this.dgvComparacion.Name = "dgvComparacion";
            this.dgvComparacion.ReadOnly = true;
            this.dgvComparacion.Size = new System.Drawing.Size(442, 296);
            this.dgvComparacion.TabIndex = 0;
            this.dgvComparacion.TabStop = false;
            //
            // colCampo
            //
            this.colCampo.HeaderText = "Campo";
            this.colCampo.Name = "colCampo";
            this.colCampo.ReadOnly = true;
            this.colCampo.Width = 150;
            //
            // colAnterior
            //
            this.colAnterior.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAnterior.HeaderText = "Valor anterior";
            this.colAnterior.Name = "colAnterior";
            this.colAnterior.ReadOnly = true;
            //
            // colPosterior
            //
            this.colPosterior.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPosterior.HeaderText = "Valor posterior";
            this.colPosterior.Name = "colPosterior";
            this.colPosterior.ReadOnly = true;
            //
            // lblLeyenda
            //
            this.lblLeyenda.Location = new System.Drawing.Point(12, 332);
            this.lblLeyenda.Name = "lblLeyenda";
            this.lblLeyenda.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblLeyenda.Size = new System.Drawing.Size(442, 26);
            this.lblLeyenda.TabIndex = 1;
            this.lblLeyenda.Text = "Resaltados: los campos que cambiaron.";
            this.lblLeyenda.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // rbAnterior
            //
            this.rbAnterior.Location = new System.Drawing.Point(14, 366);
            this.rbAnterior.Name = "rbAnterior";
            this.rbAnterior.Size = new System.Drawing.Size(210, 26);
            this.rbAnterior.TabIndex = 2;
            this.rbAnterior.Text = "Versión anterior";
            this.rbAnterior.CheckedChanged += new System.EventHandler(this.rbVersion_CheckedChanged);
            //
            // rbPosterior
            //
            this.rbPosterior.Location = new System.Drawing.Point(236, 366);
            this.rbPosterior.Name = "rbPosterior";
            this.rbPosterior.Size = new System.Drawing.Size(210, 26);
            this.rbPosterior.TabIndex = 3;
            this.rbPosterior.Text = "Versión posterior";
            this.rbPosterior.CheckedChanged += new System.EventHandler(this.rbVersion_CheckedChanged);
            //
            // lblMensaje
            //
            this.lblMensaje.Location = new System.Drawing.Point(16, 560);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(720, 32);
            this.lblMensaje.TabIndex = 5;
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRestaurar
            //
            this.btnRestaurar.Location = new System.Drawing.Point(764, 560);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(180, 34);
            this.btnRestaurar.TabIndex = 6;
            this.btnRestaurar.Text = "Restaurar versión";
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(952, 560);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(152, 34);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmBitacoraCambios_575_AV
            //
            this.ClientSize = new System.Drawing.Size(1120, 610);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.gbHistorial);
            this.Controls.Add(this.gbComparacion);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.btnSalir);
            this.Name = "frmBitacoraCambios_575_AV";
            this.Text = "Bitácora de cambios de artículos";
            this.Load += new System.EventHandler(this.frmBitacoraCambios_Load);
            this.gbFiltros.ResumeLayout(false);
            this.gbHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.gbComparacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComparacion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.Label lblArticulo;
        private System.Windows.Forms.ComboBox cmbArticulo;
        private System.Windows.Forms.Label lblOperacion;
        private System.Windows.Forms.ComboBox cmbOperacion;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.GroupBox gbHistorial;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOperacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.GroupBox gbComparacion;
        private System.Windows.Forms.DataGridView dgvComparacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCampo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnterior;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosterior;
        private System.Windows.Forms.Label lblLeyenda;
        private System.Windows.Forms.RadioButton rbAnterior;
        private System.Windows.Forms.RadioButton rbPosterior;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnSalir;
    }
}
