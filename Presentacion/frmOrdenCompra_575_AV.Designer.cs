namespace Presentacion_575_AV
{
    partial class frmOrdenCompra_575_AV
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
            this.gbReponer = new System.Windows.Forms.GroupBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.btnComparar = new System.Windows.Forms.Button();
            this.dgvReponer = new System.Windows.Forms.DataGridView();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblUnidad = new System.Windows.Forms.Label();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.nudPrecio = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.gbBorrador = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbReponer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReponer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).BeginInit();
            this.gbBorrador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(600, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Órdenes de Compra";
            //
            // gbReponer
            //
            this.gbReponer.Controls.Add(this.lblProveedor);
            this.gbReponer.Controls.Add(this.cmbProveedor);
            this.gbReponer.Controls.Add(this.btnComparar);
            this.gbReponer.Controls.Add(this.dgvReponer);
            this.gbReponer.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbReponer.Location = new System.Drawing.Point(12, 50);
            this.gbReponer.Name = "gbReponer";
            this.gbReponer.Size = new System.Drawing.Size(900, 250);
            this.gbReponer.TabIndex = 1;
            this.gbReponer.TabStop = false;
            this.gbReponer.Text = "Artículos a reponer";
            //
            // lblProveedor
            //
            this.lblProveedor.Location = new System.Drawing.Point(12, 30);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(100, 20);
            this.lblProveedor.Text = "Proveedor";
            //
            // cmbProveedor
            //
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProveedor.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbProveedor.Location = new System.Drawing.Point(116, 27);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(400, 25);
            this.cmbProveedor.TabIndex = 0;
            //
            // btnComparar
            //
            this.btnComparar.Location = new System.Drawing.Point(700, 23);
            this.btnComparar.Name = "btnComparar";
            this.btnComparar.Size = new System.Drawing.Size(188, 32);
            this.btnComparar.TabIndex = 1;
            this.btnComparar.Text = "Comparar precios";
            this.btnComparar.Click += new System.EventHandler(this.btnComparar_Click);
            //
            // dgvReponer
            //
            this.dgvReponer.AllowUserToAddRows = false;
            this.dgvReponer.AllowUserToDeleteRows = false;
            this.dgvReponer.Location = new System.Drawing.Point(12, 64);
            this.dgvReponer.MultiSelect = false;
            this.dgvReponer.Name = "dgvReponer";
            this.dgvReponer.ReadOnly = true;
            this.dgvReponer.Size = new System.Drawing.Size(876, 174);
            this.dgvReponer.TabIndex = 2;
            this.dgvReponer.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvReponer_CellFormatting);
            this.dgvReponer.CurrentCellChanged += new System.EventHandler(this.dgvReponer_CurrentCellChanged);
            //
            // lblCantidad
            //
            this.lblCantidad.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCantidad.Location = new System.Drawing.Point(12, 315);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(80, 20);
            this.lblCantidad.TabIndex = 2;
            this.lblCantidad.Text = "Cantidad";
            //
            // nudCantidad
            //
            this.nudCantidad.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.nudCantidad.Location = new System.Drawing.Point(96, 312);
            this.nudCantidad.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(120, 25);
            this.nudCantidad.TabIndex = 3;
            this.nudCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblUnidad
            //
            this.lblUnidad.Location = new System.Drawing.Point(222, 315);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(50, 20);
            this.lblUnidad.TabIndex = 4;
            //
            // lblPrecio
            //
            this.lblPrecio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPrecio.Location = new System.Drawing.Point(280, 315);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Size = new System.Drawing.Size(120, 20);
            this.lblPrecio.TabIndex = 5;
            this.lblPrecio.Text = "Precio manual";
            //
            // nudPrecio
            //
            this.nudPrecio.DecimalPlaces = 2;
            this.nudPrecio.Enabled = false;
            this.nudPrecio.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.nudPrecio.Location = new System.Drawing.Point(404, 312);
            this.nudPrecio.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            this.nudPrecio.Name = "nudPrecio";
            this.nudPrecio.Size = new System.Drawing.Size(140, 25);
            this.nudPrecio.TabIndex = 6;
            this.nudPrecio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPrecio.ThousandsSeparator = true;
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(560, 307);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(220, 34);
            this.btnAgregar.TabIndex = 7;
            this.btnAgregar.Text = "Agregar a la orden";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // gbBorrador
            //
            this.gbBorrador.Controls.Add(this.dgvDetalle);
            this.gbBorrador.Controls.Add(this.lblTotal);
            this.gbBorrador.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbBorrador.Location = new System.Drawing.Point(12, 352);
            this.gbBorrador.Name = "gbBorrador";
            this.gbBorrador.Size = new System.Drawing.Size(900, 256);
            this.gbBorrador.TabIndex = 8;
            this.gbBorrador.TabStop = false;
            this.gbBorrador.Text = "Orden en borrador";
            //
            // dgvDetalle
            //
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.Location = new System.Drawing.Point(12, 26);
            this.dgvDetalle.MultiSelect = false;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.Size = new System.Drawing.Size(876, 186);
            this.dgvDetalle.TabIndex = 0;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(488, 220);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(400, 24);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(582, 620);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(160, 36);
            this.btnGenerar.TabIndex = 9;
            this.btnGenerar.Text = "Generar orden";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 620);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 10;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 666);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 40);
            this.lblMensaje.TabIndex = 11;
            //
            // frmOrdenCompra_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 716);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbReponer);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.nudPrecio);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.gbBorrador);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmOrdenCompra_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Órdenes de Compra";
            this.Load += new System.EventHandler(this.frmOrdenCompra_Load);
            this.gbReponer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReponer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecio)).EndInit();
            this.gbBorrador.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbReponer;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Button btnComparar;
        private System.Windows.Forms.DataGridView dgvReponer;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblUnidad;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.NumericUpDown nudPrecio;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.GroupBox gbBorrador;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
