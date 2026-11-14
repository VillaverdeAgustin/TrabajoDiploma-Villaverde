namespace Presentacion_575_AV
{
    partial class frmImportarListaPrecios_575_AV
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
            this.gbOrigen = new System.Windows.Forms.GroupBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.btnNuevoProveedor = new System.Windows.Forms.Button();
            this.lblVigencia = new System.Windows.Forms.Label();
            this.dtpVigencia = new System.Windows.Forms.DateTimePicker();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.cmbMoneda = new System.Windows.Forms.ComboBox();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.btnPrevisualizar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.gbRenglones = new System.Windows.Forms.GroupBox();
            this.dgvRenglones = new System.Windows.Forms.DataGridView();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbOrigen.SuspendLayout();
            this.gbRenglones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRenglones)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(600, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Importar Lista de Precios";
            //
            // gbOrigen
            //
            this.gbOrigen.Controls.Add(this.lblProveedor);
            this.gbOrigen.Controls.Add(this.cmbProveedor);
            this.gbOrigen.Controls.Add(this.btnNuevoProveedor);
            this.gbOrigen.Controls.Add(this.lblVigencia);
            this.gbOrigen.Controls.Add(this.dtpVigencia);
            this.gbOrigen.Controls.Add(this.lblMoneda);
            this.gbOrigen.Controls.Add(this.cmbMoneda);
            this.gbOrigen.Controls.Add(this.lblArchivo);
            this.gbOrigen.Controls.Add(this.txtArchivo);
            this.gbOrigen.Controls.Add(this.btnExaminar);
            this.gbOrigen.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbOrigen.Location = new System.Drawing.Point(12, 50);
            this.gbOrigen.Name = "gbOrigen";
            this.gbOrigen.Size = new System.Drawing.Size(900, 140);
            this.gbOrigen.TabIndex = 1;
            this.gbOrigen.TabStop = false;
            this.gbOrigen.Text = "Lista a importar";
            //
            // lblProveedor
            //
            this.lblProveedor.Location = new System.Drawing.Point(12, 34);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(130, 20);
            this.lblProveedor.Text = "Proveedor";
            //
            // cmbProveedor
            //
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbProveedor.Location = new System.Drawing.Point(146, 31);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(500, 25);
            this.cmbProveedor.TabIndex = 0;
            this.cmbProveedor.SelectedIndexChanged += new System.EventHandler(this.Origen_Cambio);
            //
            // btnNuevoProveedor
            //
            this.btnNuevoProveedor.Location = new System.Drawing.Point(656, 27);
            this.btnNuevoProveedor.Name = "btnNuevoProveedor";
            this.btnNuevoProveedor.Size = new System.Drawing.Size(232, 32);
            this.btnNuevoProveedor.TabIndex = 1;
            this.btnNuevoProveedor.Text = "Nuevo proveedor";
            this.btnNuevoProveedor.Click += new System.EventHandler(this.btnNuevoProveedor_Click);
            //
            // lblVigencia
            //
            this.lblVigencia.Location = new System.Drawing.Point(12, 68);
            this.lblVigencia.Name = "lblVigencia";
            this.lblVigencia.Size = new System.Drawing.Size(130, 20);
            this.lblVigencia.Text = "Fecha de vigencia";
            //
            // dtpVigencia
            //
            this.dtpVigencia.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.dtpVigencia.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVigencia.Location = new System.Drawing.Point(146, 65);
            this.dtpVigencia.Name = "dtpVigencia";
            this.dtpVigencia.Size = new System.Drawing.Size(160, 25);
            this.dtpVigencia.TabIndex = 2;
            //
            // lblMoneda
            //
            this.lblMoneda.Location = new System.Drawing.Point(456, 68);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(100, 20);
            this.lblMoneda.Text = "Moneda";
            //
            // cmbMoneda
            //
            this.cmbMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoneda.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbMoneda.Location = new System.Drawing.Point(560, 65);
            this.cmbMoneda.Name = "cmbMoneda";
            this.cmbMoneda.Size = new System.Drawing.Size(86, 25);
            this.cmbMoneda.TabIndex = 3;
            //
            // lblArchivo
            //
            this.lblArchivo.Location = new System.Drawing.Point(12, 102);
            this.lblArchivo.Name = "lblArchivo";
            this.lblArchivo.Size = new System.Drawing.Size(130, 20);
            this.lblArchivo.Text = "Archivo";
            //
            // txtArchivo
            //
            this.txtArchivo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtArchivo.Location = new System.Drawing.Point(146, 99);
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.ReadOnly = true;
            this.txtArchivo.Size = new System.Drawing.Size(500, 25);
            this.txtArchivo.TabIndex = 4;
            this.txtArchivo.TextChanged += new System.EventHandler(this.Origen_Cambio);
            //
            // btnExaminar
            //
            this.btnExaminar.Location = new System.Drawing.Point(656, 95);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(232, 32);
            this.btnExaminar.TabIndex = 5;
            this.btnExaminar.Text = "Examinar...";
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            //
            // btnPrevisualizar
            //
            this.btnPrevisualizar.Location = new System.Drawing.Point(12, 200);
            this.btnPrevisualizar.Name = "btnPrevisualizar";
            this.btnPrevisualizar.Size = new System.Drawing.Size(160, 36);
            this.btnPrevisualizar.TabIndex = 2;
            this.btnPrevisualizar.Text = "Previsualizar";
            this.btnPrevisualizar.Click += new System.EventHandler(this.btnPrevisualizar_Click);
            //
            // lblResumen
            //
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblResumen.Location = new System.Drawing.Point(186, 206);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(726, 24);
            this.lblResumen.TabIndex = 3;
            //
            // gbRenglones
            //
            this.gbRenglones.Controls.Add(this.dgvRenglones);
            this.gbRenglones.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbRenglones.Location = new System.Drawing.Point(12, 244);
            this.gbRenglones.Name = "gbRenglones";
            this.gbRenglones.Size = new System.Drawing.Size(900, 340);
            this.gbRenglones.TabIndex = 4;
            this.gbRenglones.TabStop = false;
            this.gbRenglones.Text = "Renglones del archivo";
            //
            // dgvRenglones
            //
            this.dgvRenglones.AllowUserToAddRows = false;
            this.dgvRenglones.AllowUserToDeleteRows = false;
            this.dgvRenglones.Location = new System.Drawing.Point(12, 26);
            this.dgvRenglones.MultiSelect = false;
            this.dgvRenglones.Name = "dgvRenglones";
            this.dgvRenglones.ReadOnly = true;
            this.dgvRenglones.Size = new System.Drawing.Size(876, 302);
            this.dgvRenglones.TabIndex = 0;
            this.dgvRenglones.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRenglones_CellFormatting);
            //
            // btnImportar
            //
            this.btnImportar.Location = new System.Drawing.Point(582, 596);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(160, 36);
            this.btnImportar.TabIndex = 5;
            this.btnImportar.Text = "Importar";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 596);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 642);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 24);
            this.lblMensaje.TabIndex = 7;
            //
            // frmImportarListaPrecios_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 676);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbOrigen);
            this.Controls.Add(this.btnPrevisualizar);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.gbRenglones);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmImportarListaPrecios_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar Lista de Precios";
            this.Load += new System.EventHandler(this.frmImportarListaPrecios_Load);
            this.gbOrigen.ResumeLayout(false);
            this.gbOrigen.PerformLayout();
            this.gbRenglones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRenglones)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbOrigen;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Button btnNuevoProveedor;
        private System.Windows.Forms.Label lblVigencia;
        private System.Windows.Forms.DateTimePicker dtpVigencia;
        private System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.ComboBox cmbMoneda;
        private System.Windows.Forms.Label lblArchivo;
        private System.Windows.Forms.TextBox txtArchivo;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.Button btnPrevisualizar;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.GroupBox gbRenglones;
        private System.Windows.Forms.DataGridView dgvRenglones;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
