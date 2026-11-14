namespace Presentacion_575_AV
{
    partial class frmEntregaDeposito_575_AV
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
            this.gbComprobante = new System.Windows.Forms.GroupBox();
            this.lblNroComprobante = new System.Windows.Forms.Label();
            this.txtNroComprobante = new System.Windows.Forms.TextBox();
            this.btnValidar = new System.Windows.Forms.Button();
            this.lblComprobanteInfo = new System.Windows.Forms.Label();
            this.gbArticulos = new System.Windows.Forms.GroupBox();
            this.dgvArticulos = new System.Windows.Forms.DataGridView();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbComprobante.SuspendLayout();
            this.gbArticulos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(600, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Entrega en Deposito";
            //
            // gbComprobante
            //
            this.gbComprobante.Controls.Add(this.lblNroComprobante);
            this.gbComprobante.Controls.Add(this.txtNroComprobante);
            this.gbComprobante.Controls.Add(this.btnValidar);
            this.gbComprobante.Controls.Add(this.lblComprobanteInfo);
            this.gbComprobante.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbComprobante.Location = new System.Drawing.Point(12, 50);
            this.gbComprobante.Name = "gbComprobante";
            this.gbComprobante.Size = new System.Drawing.Size(900, 100);
            this.gbComprobante.TabIndex = 1;
            this.gbComprobante.TabStop = false;
            this.gbComprobante.Text = "Comprobante";
            //
            // lblNroComprobante
            //
            this.lblNroComprobante.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNroComprobante.Location = new System.Drawing.Point(12, 34);
            this.lblNroComprobante.Name = "lblNroComprobante";
            this.lblNroComprobante.Size = new System.Drawing.Size(160, 20);
            this.lblNroComprobante.TabIndex = 0;
            this.lblNroComprobante.Text = "Nro. de comprobante";
            //
            // txtNroComprobante
            //
            this.txtNroComprobante.Location = new System.Drawing.Point(180, 31);
            this.txtNroComprobante.MaxLength = 20;
            this.txtNroComprobante.Name = "txtNroComprobante";
            this.txtNroComprobante.Size = new System.Drawing.Size(240, 25);
            this.txtNroComprobante.TabIndex = 1;
            //
            // btnValidar
            //
            this.btnValidar.Location = new System.Drawing.Point(430, 28);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(140, 30);
            this.btnValidar.TabIndex = 2;
            this.btnValidar.Text = "Validar";
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            //
            // lblComprobanteInfo
            //
            this.lblComprobanteInfo.Location = new System.Drawing.Point(12, 66);
            this.lblComprobanteInfo.Name = "lblComprobanteInfo";
            this.lblComprobanteInfo.Size = new System.Drawing.Size(876, 22);
            this.lblComprobanteInfo.TabIndex = 3;
            //
            // gbArticulos
            //
            this.gbArticulos.Controls.Add(this.dgvArticulos);
            this.gbArticulos.Controls.Add(this.lblAyuda);
            this.gbArticulos.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbArticulos.Location = new System.Drawing.Point(12, 160);
            this.gbArticulos.Name = "gbArticulos";
            this.gbArticulos.Size = new System.Drawing.Size(900, 300);
            this.gbArticulos.TabIndex = 2;
            this.gbArticulos.TabStop = false;
            this.gbArticulos.Text = "Articulos a entregar";
            //
            // dgvArticulos
            //
            this.dgvArticulos.AllowUserToAddRows = false;
            this.dgvArticulos.AllowUserToDeleteRows = false;
            this.dgvArticulos.Location = new System.Drawing.Point(12, 26);
            this.dgvArticulos.Name = "dgvArticulos";
            this.dgvArticulos.Size = new System.Drawing.Size(876, 236);
            this.dgvArticulos.TabIndex = 0;
            this.dgvArticulos.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvArticulos_DataError);
            //
            // lblAyuda
            //
            this.lblAyuda.Location = new System.Drawing.Point(12, 268);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(876, 22);
            this.lblAyuda.TabIndex = 1;
            this.lblAyuda.Text = "Si se entrega menos de lo facturado, corregir la columna A entregar.";
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(582, 472);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(160, 36);
            this.btnRegistrar.TabIndex = 3;
            this.btnRegistrar.Text = "Registrar entrega";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(752, 472);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 478);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(560, 26);
            this.lblMensaje.TabIndex = 5;
            //
            // frmEntregaDeposito_575_AV
            //
            this.AcceptButton = this.btnValidar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(924, 520);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbComprobante);
            this.Controls.Add(this.gbArticulos);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmEntregaDeposito_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Entrega en Deposito";
            this.Load += new System.EventHandler(this.frmEntregaDeposito_Load);
            this.gbComprobante.ResumeLayout(false);
            this.gbComprobante.PerformLayout();
            this.gbArticulos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvArticulos)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbComprobante;
        private System.Windows.Forms.Label lblNroComprobante;
        private System.Windows.Forms.TextBox txtNroComprobante;
        private System.Windows.Forms.Button btnValidar;
        private System.Windows.Forms.Label lblComprobanteInfo;
        private System.Windows.Forms.GroupBox gbArticulos;
        private System.Windows.Forms.DataGridView dgvArticulos;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
