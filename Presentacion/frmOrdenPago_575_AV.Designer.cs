namespace Presentacion_575_AV
{
    partial class frmOrdenPago_575_AV
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
            this.gbCarritos = new System.Windows.Forms.GroupBox();
            this.dgvCarritos = new System.Windows.Forms.DataGridView();
            this.gbDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblCliente = new System.Windows.Forms.Label();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnEmitir = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbCarritos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarritos)).BeginInit();
            this.gbDetalle.SuspendLayout();
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
            this.lblTitulo.Text = "Orden de Pago";
            //
            // gbCarritos
            //
            this.gbCarritos.Controls.Add(this.dgvCarritos);
            this.gbCarritos.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbCarritos.Location = new System.Drawing.Point(12, 50);
            this.gbCarritos.Name = "gbCarritos";
            this.gbCarritos.Size = new System.Drawing.Size(900, 190);
            this.gbCarritos.TabIndex = 1;
            this.gbCarritos.TabStop = false;
            this.gbCarritos.Text = "Carritos confirmados";
            //
            // dgvCarritos
            //
            this.dgvCarritos.AllowUserToAddRows = false;
            this.dgvCarritos.AllowUserToDeleteRows = false;
            this.dgvCarritos.Location = new System.Drawing.Point(12, 26);
            this.dgvCarritos.MultiSelect = false;
            this.dgvCarritos.Name = "dgvCarritos";
            this.dgvCarritos.ReadOnly = true;
            this.dgvCarritos.Size = new System.Drawing.Size(876, 152);
            this.dgvCarritos.TabIndex = 0;
            this.dgvCarritos.SelectionChanged += new System.EventHandler(this.dgvCarritos_SelectionChanged);
            //
            // gbDetalle
            //
            this.gbDetalle.Controls.Add(this.dgvDetalle);
            this.gbDetalle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbDetalle.Location = new System.Drawing.Point(12, 250);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(900, 200);
            this.gbDetalle.TabIndex = 2;
            this.gbDetalle.TabStop = false;
            this.gbDetalle.Text = "Detalle del carrito";
            //
            // dgvDetalle
            //
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.Location = new System.Drawing.Point(12, 26);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.Size = new System.Drawing.Size(876, 162);
            this.dgvDetalle.TabIndex = 0;
            //
            // lblCliente
            //
            this.lblCliente.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCliente.Location = new System.Drawing.Point(12, 466);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(140, 20);
            this.lblCliente.TabIndex = 3;
            this.lblCliente.Text = "Nombre del cliente";
            //
            // txtCliente
            //
            this.txtCliente.Location = new System.Drawing.Point(158, 463);
            this.txtCliente.MaxLength = 100;
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.Size = new System.Drawing.Size(360, 25);
            this.txtCliente.TabIndex = 4;
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitulo.Location = new System.Drawing.Point(620, 462);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(90, 28);
            this.lblTotalTitulo.TabIndex = 5;
            this.lblTotalTitulo.Text = "Total";
            this.lblTotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(712, 458);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(200, 34);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "0,00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnEmitir
            //
            this.btnEmitir.Location = new System.Drawing.Point(582, 506);
            this.btnEmitir.Name = "btnEmitir";
            this.btnEmitir.Size = new System.Drawing.Size(160, 36);
            this.btnEmitir.TabIndex = 7;
            this.btnEmitir.Text = "Emitir";
            this.btnEmitir.Click += new System.EventHandler(this.btnEmitir_Click);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(12, 506);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(140, 36);
            this.btnActualizar.TabIndex = 9;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 506);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 8;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 552);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 24);
            this.lblMensaje.TabIndex = 10;
            //
            // frmOrdenPago_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 586);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbCarritos);
            this.Controls.Add(this.gbDetalle);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.txtCliente);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnEmitir);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmOrdenPago_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Orden de Pago";
            this.Load += new System.EventHandler(this.frmOrdenPago_Load);
            this.gbCarritos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarritos)).EndInit();
            this.gbDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbCarritos;
        private System.Windows.Forms.DataGridView dgvCarritos;
        private System.Windows.Forms.GroupBox gbDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.TextBox txtCliente;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnEmitir;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
