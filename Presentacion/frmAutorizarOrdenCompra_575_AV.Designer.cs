namespace Presentacion_575_AV
{
    partial class frmAutorizarOrdenCompra_575_AV
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
            this.gbPendientes = new System.Windows.Forms.GroupBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.gbDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.btnAutorizar = new System.Windows.Forms.Button();
            this.btnRechazar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbPendientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
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
            this.lblTitulo.Text = "Autorizaciones Pendientes";
            //
            // gbPendientes
            //
            this.gbPendientes.Controls.Add(this.dgvOrdenes);
            this.gbPendientes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbPendientes.Location = new System.Drawing.Point(12, 50);
            this.gbPendientes.Name = "gbPendientes";
            this.gbPendientes.Size = new System.Drawing.Size(900, 220);
            this.gbPendientes.TabIndex = 1;
            this.gbPendientes.TabStop = false;
            this.gbPendientes.Text = "Órdenes pendientes de autorización";
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.Location = new System.Drawing.Point(12, 26);
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.Size = new System.Drawing.Size(876, 182);
            this.dgvOrdenes.TabIndex = 0;
            this.dgvOrdenes.CurrentCellChanged += new System.EventHandler(this.dgvOrdenes_CurrentCellChanged);
            //
            // gbDetalle
            //
            this.gbDetalle.Controls.Add(this.dgvDetalle);
            this.gbDetalle.Controls.Add(this.lblTotal);
            this.gbDetalle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbDetalle.Location = new System.Drawing.Point(12, 280);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(900, 256);
            this.gbDetalle.TabIndex = 2;
            this.gbDetalle.TabStop = false;
            this.gbDetalle.Text = "Detalle de la orden";
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
            // lblMotivo
            //
            this.lblMotivo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMotivo.Location = new System.Drawing.Point(12, 551);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(150, 20);
            this.lblMotivo.TabIndex = 3;
            this.lblMotivo.Text = "Motivo del rechazo";
            //
            // txtMotivo
            //
            this.txtMotivo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtMotivo.Location = new System.Drawing.Point(166, 548);
            this.txtMotivo.MaxLength = 300;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(746, 25);
            this.txtMotivo.TabIndex = 4;
            //
            // btnAutorizar
            //
            this.btnAutorizar.Location = new System.Drawing.Point(412, 592);
            this.btnAutorizar.Name = "btnAutorizar";
            this.btnAutorizar.Size = new System.Drawing.Size(160, 36);
            this.btnAutorizar.TabIndex = 5;
            this.btnAutorizar.Text = "Autorizar";
            this.btnAutorizar.Click += new System.EventHandler(this.btnAutorizar_Click);
            //
            // btnRechazar
            //
            this.btnRechazar.Location = new System.Drawing.Point(582, 592);
            this.btnRechazar.Name = "btnRechazar";
            this.btnRechazar.Size = new System.Drawing.Size(160, 36);
            this.btnRechazar.TabIndex = 6;
            this.btnRechazar.Text = "Rechazar";
            this.btnRechazar.Click += new System.EventHandler(this.btnRechazar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 592);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 638);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 24);
            this.lblMensaje.TabIndex = 8;
            //
            // frmAutorizarOrdenCompra_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 672);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbPendientes);
            this.Controls.Add(this.gbDetalle);
            this.Controls.Add(this.lblMotivo);
            this.Controls.Add(this.txtMotivo);
            this.Controls.Add(this.btnAutorizar);
            this.Controls.Add(this.btnRechazar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmAutorizarOrdenCompra_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autorizaciones Pendientes";
            this.Load += new System.EventHandler(this.frmAutorizarOrdenCompra_Load);
            this.gbPendientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbPendientes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.GroupBox gbDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Button btnAutorizar;
        private System.Windows.Forms.Button btnRechazar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
