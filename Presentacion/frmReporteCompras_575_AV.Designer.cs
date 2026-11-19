namespace Presentacion_575_AV
{
    partial class frmReporteCompras_575_AV
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
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.gbResumen = new System.Windows.Forms.GroupBox();
            this.dgvResumen = new System.Windows.Forms.DataGridView();
            this.colProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrdenes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMontoArs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMontoUsd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRecibido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPendientes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblLeyenda = new System.Windows.Forms.Label();
            this.gbOrdenes = new System.Windows.Forms.GroupBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.colNro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProveedorOrden = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRenglones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrdenArs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrdenUsd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrdenRecibido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnExportarPdf = new System.Windows.Forms.Button();
            this.sfdPdf = new System.Windows.Forms.SaveFileDialog();
            this.gbFiltros.SuspendLayout();
            this.gbResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).BeginInit();
            this.gbOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Reporte de compras por proveedor";
            //
            // gbFiltros
            //
            this.gbFiltros.Controls.Add(this.lblDesde);
            this.gbFiltros.Controls.Add(this.dtpDesde);
            this.gbFiltros.Controls.Add(this.lblHasta);
            this.gbFiltros.Controls.Add(this.dtpHasta);
            this.gbFiltros.Controls.Add(this.lblProveedor);
            this.gbFiltros.Controls.Add(this.cmbProveedor);
            this.gbFiltros.Controls.Add(this.lblEstado);
            this.gbFiltros.Controls.Add(this.cmbEstado);
            this.gbFiltros.Controls.Add(this.btnGenerar);
            this.gbFiltros.Location = new System.Drawing.Point(16, 44);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(1088, 76);
            this.gbFiltros.TabIndex = 1;
            this.gbFiltros.Text = "Filtros";
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(14, 33);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.TabIndex = 2;
            this.lblDesde.Text = "Desde:";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(70, 29);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(118, 25);
            this.dtpDesde.TabIndex = 3;
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(204, 33);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.TabIndex = 4;
            this.lblHasta.Text = "Hasta:";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(256, 29);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(118, 25);
            this.dtpHasta.TabIndex = 5;
            //
            // lblProveedor
            //
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Location = new System.Drawing.Point(392, 33);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.TabIndex = 6;
            this.lblProveedor.Text = "Proveedor:";
            //
            // cmbProveedor
            //
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.Location = new System.Drawing.Point(474, 29);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(262, 25);
            this.cmbProveedor.TabIndex = 7;
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(752, 33);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.TabIndex = 8;
            this.lblEstado.Text = "Estado:";
            //
            // cmbEstado
            //
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.Location = new System.Drawing.Point(810, 29);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(160, 25);
            this.cmbEstado.TabIndex = 9;
            //
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(982, 25);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(94, 34);
            this.btnGenerar.TabIndex = 10;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // gbResumen
            //
            this.gbResumen.Controls.Add(this.dgvResumen);
            this.gbResumen.Controls.Add(this.lblLeyenda);
            this.gbResumen.Location = new System.Drawing.Point(16, 128);
            this.gbResumen.Name = "gbResumen";
            this.gbResumen.Size = new System.Drawing.Size(1088, 262);
            this.gbResumen.TabIndex = 11;
            this.gbResumen.Text = "Compras por proveedor";
            //
            // dgvResumen
            //
            this.dgvResumen.AllowUserToAddRows = false;
            this.dgvResumen.AllowUserToDeleteRows = false;
            this.dgvResumen.AllowUserToResizeRows = false;
            this.dgvResumen.AutoGenerateColumns = false;
            this.dgvResumen.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProveedor,
            this.colOrdenes,
            this.colMontoArs,
            this.colMontoUsd,
            this.colRecibido,
            this.colPendientes});
            this.dgvResumen.MultiSelect = false;
            this.dgvResumen.ReadOnly = true;
            this.dgvResumen.Location = new System.Drawing.Point(12, 26);
            this.dgvResumen.Name = "dgvResumen";
            this.dgvResumen.Size = new System.Drawing.Size(1064, 200);
            this.dgvResumen.TabIndex = 12;
            this.dgvResumen.SelectionChanged += new System.EventHandler(this.dgvResumen_SelectionChanged);
            //
            // colProveedor
            //
            this.colProveedor.DataPropertyName = "razon_social";
            this.colProveedor.HeaderText = "Proveedor";
            this.colProveedor.Name = "colProveedor";
            this.colProveedor.ReadOnly = true;
            this.colProveedor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // colOrdenes
            //
            this.colOrdenes.DataPropertyName = "cantidad_ordenes";
            this.colOrdenes.HeaderText = "Órdenes";
            this.colOrdenes.Name = "colOrdenes";
            this.colOrdenes.ReadOnly = true;
            this.colOrdenes.Width = 84;
            //
            // colMontoArs
            //
            this.colMontoArs.DataPropertyName = "monto_ars";
            this.colMontoArs.HeaderText = "Monto ARS";
            this.colMontoArs.Name = "colMontoArs";
            this.colMontoArs.ReadOnly = true;
            this.colMontoArs.Width = 140;
            //
            // colMontoUsd
            //
            this.colMontoUsd.DataPropertyName = "monto_usd";
            this.colMontoUsd.HeaderText = "Monto USD";
            this.colMontoUsd.Name = "colMontoUsd";
            this.colMontoUsd.ReadOnly = true;
            this.colMontoUsd.Width = 120;
            //
            // colRecibido
            //
            this.colRecibido.DataPropertyName = "porcentaje_recibido";
            this.colRecibido.HeaderText = "% recibido";
            this.colRecibido.Name = "colRecibido";
            this.colRecibido.ReadOnly = true;
            this.colRecibido.Width = 104;
            //
            // colPendientes
            //
            this.colPendientes.DataPropertyName = "pendientes_recepcion";
            this.colPendientes.HeaderText = "Pend. recepción";
            this.colPendientes.Name = "colPendientes";
            this.colPendientes.ReadOnly = true;
            this.colPendientes.Width = 150;
            //
            // lblLeyenda
            //
            this.lblLeyenda.Location = new System.Drawing.Point(12, 230);
            this.lblLeyenda.Name = "lblLeyenda";
            this.lblLeyenda.Size = new System.Drawing.Size(1064, 26);
            this.lblLeyenda.TabIndex = 13;
            this.lblLeyenda.Text = "";
            //
            // gbOrdenes
            //
            this.gbOrdenes.Controls.Add(this.dgvOrdenes);
            this.gbOrdenes.Location = new System.Drawing.Point(16, 398);
            this.gbOrdenes.Name = "gbOrdenes";
            this.gbOrdenes.Size = new System.Drawing.Size(1088, 188);
            this.gbOrdenes.TabIndex = 14;
            this.gbOrdenes.Text = "Órdenes de todos los proveedores";
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.AllowUserToResizeRows = false;
            this.dgvOrdenes.AutoGenerateColumns = false;
            this.dgvOrdenes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNro,
            this.colFecha,
            this.colProveedorOrden,
            this.colEstado,
            this.colRenglones,
            this.colOrdenArs,
            this.colOrdenUsd,
            this.colOrdenRecibido});
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.Location = new System.Drawing.Point(12, 26);
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.Size = new System.Drawing.Size(1064, 150);
            this.dgvOrdenes.TabIndex = 15;
            //
            // colNro
            //
            this.colNro.DataPropertyName = "nro_orden_compra";
            this.colNro.HeaderText = "Nro. orden";
            this.colNro.Name = "colNro";
            this.colNro.ReadOnly = true;
            this.colNro.Width = 110;
            //
            // colFecha
            //
            this.colFecha.DataPropertyName = "fecha_emision";
            this.colFecha.HeaderText = "Fecha de emisión";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            this.colFecha.Width = 140;
            //
            // colProveedorOrden
            //
            this.colProveedorOrden.DataPropertyName = "razon_social";
            this.colProveedorOrden.HeaderText = "Proveedor";
            this.colProveedorOrden.Name = "colProveedorOrden";
            this.colProveedorOrden.ReadOnly = true;
            this.colProveedorOrden.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // colEstado
            //
            this.colEstado.DataPropertyName = "estado";
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            this.colEstado.Width = 170;
            //
            // colRenglones
            //
            this.colRenglones.DataPropertyName = "renglones";
            this.colRenglones.HeaderText = "Renglones";
            this.colRenglones.Name = "colRenglones";
            this.colRenglones.ReadOnly = true;
            this.colRenglones.Width = 90;
            //
            // colOrdenArs
            //
            this.colOrdenArs.DataPropertyName = "monto_ars";
            this.colOrdenArs.HeaderText = "Monto ARS";
            this.colOrdenArs.Name = "colOrdenArs";
            this.colOrdenArs.ReadOnly = true;
            this.colOrdenArs.Width = 140;
            //
            // colOrdenUsd
            //
            this.colOrdenUsd.DataPropertyName = "monto_usd";
            this.colOrdenUsd.HeaderText = "Monto USD";
            this.colOrdenUsd.Name = "colOrdenUsd";
            this.colOrdenUsd.ReadOnly = true;
            this.colOrdenUsd.Width = 120;
            //
            // colOrdenRecibido
            //
            this.colOrdenRecibido.DataPropertyName = "porcentaje_recibido";
            this.colOrdenRecibido.HeaderText = "% recibido";
            this.colOrdenRecibido.Name = "colOrdenRecibido";
            this.colOrdenRecibido.ReadOnly = true;
            this.colOrdenRecibido.Width = 104;
            //
            // lblMensaje
            //
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje.Location = new System.Drawing.Point(16, 596);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(752, 34);
            this.lblMensaje.TabIndex = 16;
            this.lblMensaje.Text = "";
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(952, 596);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(152, 34);
            this.btnSalir.TabIndex = 17;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // btnExportarPdf
            //
            this.btnExportarPdf.Location = new System.Drawing.Point(784, 596);
            this.btnExportarPdf.Name = "btnExportarPdf";
            this.btnExportarPdf.Size = new System.Drawing.Size(152, 34);
            this.btnExportarPdf.TabIndex = 18;
            this.btnExportarPdf.Text = "Exportar PDF";
            this.btnExportarPdf.Click += new System.EventHandler(this.btnExportarPdf_Click);
            //
            // sfdPdf
            //
            this.sfdPdf.AddExtension = true;
            this.sfdPdf.DefaultExt = "pdf";
            this.sfdPdf.OverwritePrompt = true;
            //
            // frmReporteCompras_575_AV
            //
            this.ClientSize = new System.Drawing.Size(1120, 642);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.gbResumen);
            this.Controls.Add(this.gbOrdenes);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnExportarPdf);
            this.Controls.Add(this.btnSalir);
            this.Name = "frmReporteCompras_575_AV";
            this.Text = "Reporte de compras por proveedor";
            this.Load += new System.EventHandler(this.frmReporteCompras_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbOrdenes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).EndInit();
            this.gbResumen.ResumeLayout(false);
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.GroupBox gbResumen;
        private System.Windows.Forms.DataGridView dgvResumen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrdenes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMontoArs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMontoUsd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRecibido;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPendientes;
        private System.Windows.Forms.Label lblLeyenda;
        private System.Windows.Forms.GroupBox gbOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNro;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProveedorOrden;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRenglones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrdenArs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrdenUsd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrdenRecibido;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnExportarPdf;
        private System.Windows.Forms.SaveFileDialog sfdPdf;
    }
}
