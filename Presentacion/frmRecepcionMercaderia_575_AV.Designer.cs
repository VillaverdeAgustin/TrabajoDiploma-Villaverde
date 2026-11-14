namespace Presentacion_575_AV
{
    partial class frmRecepcionMercaderia_575_AV
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
            this.gbOrdenes = new System.Windows.Forms.GroupBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.gbRenglones = new System.Windows.Forms.GroupBox();
            this.lblRemito = new System.Windows.Forms.Label();
            this.txtRemito = new System.Windows.Forms.TextBox();
            this.dgvRenglones = new System.Windows.Forms.DataGridView();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
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
            this.lblTitulo.Text = "Recepción de Mercadería";
            //
            // gbOrdenes
            //
            this.gbOrdenes.Controls.Add(this.dgvOrdenes);
            this.gbOrdenes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbOrdenes.Location = new System.Drawing.Point(12, 50);
            this.gbOrdenes.Name = "gbOrdenes";
            this.gbOrdenes.Size = new System.Drawing.Size(900, 200);
            this.gbOrdenes.TabIndex = 1;
            this.gbOrdenes.TabStop = false;
            this.gbOrdenes.Text = "Órdenes para recibir";
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.Location = new System.Drawing.Point(12, 26);
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.Size = new System.Drawing.Size(876, 162);
            this.dgvOrdenes.TabIndex = 0;
            this.dgvOrdenes.CurrentCellChanged += new System.EventHandler(this.dgvOrdenes_CurrentCellChanged);
            //
            // gbRenglones
            //
            this.gbRenglones.Controls.Add(this.lblRemito);
            this.gbRenglones.Controls.Add(this.txtRemito);
            this.gbRenglones.Controls.Add(this.dgvRenglones);
            this.gbRenglones.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbRenglones.Location = new System.Drawing.Point(12, 260);
            this.gbRenglones.Name = "gbRenglones";
            this.gbRenglones.Size = new System.Drawing.Size(900, 290);
            this.gbRenglones.TabIndex = 2;
            this.gbRenglones.TabStop = false;
            this.gbRenglones.Text = "Renglones de la orden";
            //
            // lblRemito
            //
            this.lblRemito.Location = new System.Drawing.Point(12, 30);
            this.lblRemito.Name = "lblRemito";
            this.lblRemito.Size = new System.Drawing.Size(170, 20);
            this.lblRemito.Text = "Remito del proveedor";
            //
            // txtRemito
            //
            this.txtRemito.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtRemito.Location = new System.Drawing.Point(186, 27);
            this.txtRemito.MaxLength = 30;
            this.txtRemito.Name = "txtRemito";
            this.txtRemito.Size = new System.Drawing.Size(260, 25);
            this.txtRemito.TabIndex = 0;
            //
            // dgvRenglones
            //
            this.dgvRenglones.AllowUserToAddRows = false;
            this.dgvRenglones.AllowUserToDeleteRows = false;
            this.dgvRenglones.Location = new System.Drawing.Point(12, 64);
            this.dgvRenglones.MultiSelect = false;
            this.dgvRenglones.Name = "dgvRenglones";
            this.dgvRenglones.Size = new System.Drawing.Size(876, 214);
            this.dgvRenglones.TabIndex = 1;
            this.dgvRenglones.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvRenglones_DataError);
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(552, 562);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(190, 36);
            this.btnRegistrar.TabIndex = 3;
            this.btnRegistrar.Text = "Registrar recepción";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 562);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 608);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(900, 40);
            this.lblMensaje.TabIndex = 5;
            //
            // frmRecepcionMercaderia_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 658);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbOrdenes);
            this.Controls.Add(this.gbRenglones);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmRecepcionMercaderia_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recepción de Mercadería";
            this.Load += new System.EventHandler(this.frmRecepcionMercaderia_Load);
            this.gbOrdenes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbRenglones.ResumeLayout(false);
            this.gbRenglones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRenglones)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.GroupBox gbRenglones;
        private System.Windows.Forms.Label lblRemito;
        private System.Windows.Forms.TextBox txtRemito;
        private System.Windows.Forms.DataGridView dgvRenglones;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
