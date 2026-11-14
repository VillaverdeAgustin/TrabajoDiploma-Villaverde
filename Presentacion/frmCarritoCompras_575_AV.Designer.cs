namespace Presentacion_575_AV
{
    partial class frmCarritoCompras_575_AV
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
            this.lblNroCarrito = new System.Windows.Forms.Label();
            this.gbBuscar = new System.Windows.Forms.GroupBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvArticulos = new System.Windows.Forms.DataGridView();
            this.lblReferencia = new System.Windows.Forms.Label();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblUnidad = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.gbCarrito = new System.Windows.Forms.GroupBox();
            this.dgvCarrito = new System.Windows.Forms.DataGridView();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbBuscar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.gbCarrito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(560, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Carrito de Compras";
            //
            // lblNroCarrito
            //
            this.lblNroCarrito.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNroCarrito.Location = new System.Drawing.Point(582, 16);
            this.lblNroCarrito.Name = "lblNroCarrito";
            this.lblNroCarrito.Size = new System.Drawing.Size(330, 24);
            this.lblNroCarrito.TabIndex = 1;
            this.lblNroCarrito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gbBuscar
            //
            this.gbBuscar.Controls.Add(this.lblDescripcion);
            this.gbBuscar.Controls.Add(this.txtDescripcion);
            this.gbBuscar.Controls.Add(this.lblCategoria);
            this.gbBuscar.Controls.Add(this.cmbCategoria);
            this.gbBuscar.Controls.Add(this.btnBuscar);
            this.gbBuscar.Controls.Add(this.dgvArticulos);
            this.gbBuscar.Controls.Add(this.lblReferencia);
            this.gbBuscar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbBuscar.Location = new System.Drawing.Point(12, 50);
            this.gbBuscar.Name = "gbBuscar";
            this.gbBuscar.Size = new System.Drawing.Size(900, 290);
            this.gbBuscar.TabIndex = 2;
            this.gbBuscar.TabStop = false;
            this.gbBuscar.Text = "Buscar artículos";
            //
            // lblDescripcion
            //
            this.lblDescripcion.Location = new System.Drawing.Point(12, 30);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(90, 20);
            this.lblDescripcion.TabIndex = 0;
            this.lblDescripcion.Text = "Descripción";
            //
            // txtDescripcion
            //
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtDescripcion.Location = new System.Drawing.Point(104, 27);
            this.txtDescripcion.MaxLength = 200;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(320, 25);
            this.txtDescripcion.TabIndex = 1;
            this.txtDescripcion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescripcion_KeyDown);
            //
            // lblCategoria
            //
            this.lblCategoria.Location = new System.Drawing.Point(440, 30);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(84, 20);
            this.lblCategoria.TabIndex = 2;
            this.lblCategoria.Text = "Categoría";
            //
            // cmbCategoria
            //
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbCategoria.Location = new System.Drawing.Point(526, 27);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(226, 25);
            this.cmbCategoria.TabIndex = 3;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(764, 23);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(124, 32);
            this.btnBuscar.TabIndex = 4;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvArticulos
            //
            this.dgvArticulos.AllowUserToAddRows = false;
            this.dgvArticulos.AllowUserToDeleteRows = false;
            this.dgvArticulos.Location = new System.Drawing.Point(12, 64);
            this.dgvArticulos.MultiSelect = false;
            this.dgvArticulos.Name = "dgvArticulos";
            this.dgvArticulos.ReadOnly = true;
            this.dgvArticulos.Size = new System.Drawing.Size(876, 190);
            this.dgvArticulos.TabIndex = 5;
            this.dgvArticulos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvArticulos_CellFormatting);
            this.dgvArticulos.SelectionChanged += new System.EventHandler(this.dgvArticulos_SelectionChanged);
            //
            // lblReferencia
            //
            this.lblReferencia.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblReferencia.Location = new System.Drawing.Point(12, 260);
            this.lblReferencia.Name = "lblReferencia";
            this.lblReferencia.Size = new System.Drawing.Size(876, 20);
            this.lblReferencia.TabIndex = 6;
            this.lblReferencia.Text = "Los artículos resaltados en rojo no tienen disponibilidad.";
            //
            // lblCantidad
            //
            this.lblCantidad.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCantidad.Location = new System.Drawing.Point(12, 356);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(80, 20);
            this.lblCantidad.TabIndex = 3;
            this.lblCantidad.Text = "Cantidad";
            //
            // nudCantidad
            //
            this.nudCantidad.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.nudCantidad.Location = new System.Drawing.Point(96, 353);
            this.nudCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(120, 25);
            this.nudCantidad.TabIndex = 4;
            this.nudCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblUnidad
            //
            this.lblUnidad.Location = new System.Drawing.Point(222, 356);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(60, 20);
            this.lblUnidad.TabIndex = 5;
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(292, 348);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(190, 34);
            this.btnAgregar.TabIndex = 6;
            this.btnAgregar.Text = "Agregar al carrito";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // gbCarrito
            //
            this.gbCarrito.Controls.Add(this.dgvCarrito);
            this.gbCarrito.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbCarrito.Location = new System.Drawing.Point(12, 394);
            this.gbCarrito.Name = "gbCarrito";
            this.gbCarrito.Size = new System.Drawing.Size(900, 200);
            this.gbCarrito.TabIndex = 7;
            this.gbCarrito.TabStop = false;
            this.gbCarrito.Text = "Carrito";
            //
            // dgvCarrito
            //
            this.dgvCarrito.AllowUserToAddRows = false;
            this.dgvCarrito.AllowUserToDeleteRows = false;
            this.dgvCarrito.Location = new System.Drawing.Point(12, 26);
            this.dgvCarrito.MultiSelect = false;
            this.dgvCarrito.Name = "dgvCarrito";
            this.dgvCarrito.ReadOnly = true;
            this.dgvCarrito.Size = new System.Drawing.Size(876, 162);
            this.dgvCarrito.TabIndex = 0;
            //
            // btnQuitar
            //
            this.btnQuitar.Location = new System.Drawing.Point(12, 604);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(140, 36);
            this.btnQuitar.TabIndex = 8;
            this.btnQuitar.Text = "Quitar";
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitulo.Location = new System.Drawing.Point(560, 606);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(90, 28);
            this.lblTotalTitulo.TabIndex = 9;
            this.lblTotalTitulo.Text = "Total";
            this.lblTotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(652, 602);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(260, 34);
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "0,00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnConfirmar
            //
            this.btnConfirmar.Location = new System.Drawing.Point(582, 648);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(160, 36);
            this.btnConfirmar.TabIndex = 11;
            this.btnConfirmar.Text = "Confirmar carrito";
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 648);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 12;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 694);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 24);
            this.lblMensaje.TabIndex = 13;
            //
            // frmCarritoCompras_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 728);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblNroCarrito);
            this.Controls.Add(this.gbBuscar);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.gbCarrito);
            this.Controls.Add(this.btnQuitar);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmCarritoCompras_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Carrito de Compras";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCarritoCompras_FormClosing);
            this.Load += new System.EventHandler(this.frmCarritoCompras_Load);
            this.gbBuscar.ResumeLayout(false);
            this.gbBuscar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.gbCarrito.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNroCarrito;
        private System.Windows.Forms.GroupBox gbBuscar;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvArticulos;
        private System.Windows.Forms.Label lblReferencia;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblUnidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.GroupBox gbCarrito;
        private System.Windows.Forms.DataGridView dgvCarrito;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
