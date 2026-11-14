namespace Presentacion_575_AV
{
    partial class frmValidarCobrar_575_AV
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
            this.lblNroOrden = new System.Windows.Forms.Label();
            this.txtNroOrden = new System.Windows.Forms.TextBox();
            this.btnBuscarOrden = new System.Windows.Forms.Button();
            this.gbCliente = new System.Windows.Forms.GroupBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.lblClienteNombre = new System.Windows.Forms.Label();
            this.lblClienteIva = new System.Windows.Forms.Label();
            this.lblClienteCuit = new System.Windows.Forms.Label();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.btnVerCorreo = new System.Windows.Forms.Button();
            this.gbDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.gbPago = new System.Windows.Forms.GroupBox();
            this.lblMedioPago = new System.Windows.Forms.Label();
            this.cmbMedioPago = new System.Windows.Forms.ComboBox();
            this.lblDatosPago = new System.Windows.Forms.Label();
            this.txtDatosPago = new System.Windows.Forms.TextBox();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.gbCliente.SuspendLayout();
            this.gbDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.gbPago.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(600, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Validar Pedido y Cobrar";
            //
            // gbOrdenes
            //
            this.gbOrdenes.Controls.Add(this.dgvOrdenes);
            this.gbOrdenes.Controls.Add(this.lblNroOrden);
            this.gbOrdenes.Controls.Add(this.txtNroOrden);
            this.gbOrdenes.Controls.Add(this.btnBuscarOrden);
            this.gbOrdenes.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbOrdenes.Location = new System.Drawing.Point(12, 50);
            this.gbOrdenes.Name = "gbOrdenes";
            this.gbOrdenes.Size = new System.Drawing.Size(460, 230);
            this.gbOrdenes.TabIndex = 1;
            this.gbOrdenes.TabStop = false;
            this.gbOrdenes.Text = "Ordenes de pago pendientes";
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.Location = new System.Drawing.Point(12, 26);
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.Size = new System.Drawing.Size(436, 150);
            this.dgvOrdenes.TabIndex = 0;
            this.dgvOrdenes.SelectionChanged += new System.EventHandler(this.dgvOrdenes_SelectionChanged);
            //
            // lblNroOrden
            //
            this.lblNroOrden.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblNroOrden.Location = new System.Drawing.Point(12, 189);
            this.lblNroOrden.Name = "lblNroOrden";
            this.lblNroOrden.Size = new System.Drawing.Size(105, 20);
            this.lblNroOrden.TabIndex = 1;
            this.lblNroOrden.Text = "Nro. de orden";
            //
            // txtNroOrden
            //
            this.txtNroOrden.Location = new System.Drawing.Point(120, 186);
            this.txtNroOrden.MaxLength = 15;
            this.txtNroOrden.Name = "txtNroOrden";
            this.txtNroOrden.Size = new System.Drawing.Size(200, 25);
            this.txtNroOrden.TabIndex = 2;
            //
            // btnBuscarOrden
            //
            this.btnBuscarOrden.Location = new System.Drawing.Point(330, 183);
            this.btnBuscarOrden.Name = "btnBuscarOrden";
            this.btnBuscarOrden.Size = new System.Drawing.Size(118, 30);
            this.btnBuscarOrden.TabIndex = 3;
            this.btnBuscarOrden.Text = "Buscar";
            this.btnBuscarOrden.Click += new System.EventHandler(this.btnBuscarOrden_Click);
            //
            // gbCliente
            //
            this.gbCliente.Controls.Add(this.lblDni);
            this.gbCliente.Controls.Add(this.txtDni);
            this.gbCliente.Controls.Add(this.btnBuscarCliente);
            this.gbCliente.Controls.Add(this.lblClienteNombre);
            this.gbCliente.Controls.Add(this.lblClienteIva);
            this.gbCliente.Controls.Add(this.lblClienteCuit);
            this.gbCliente.Controls.Add(this.lblCorreo);
            this.gbCliente.Controls.Add(this.txtCorreo);
            this.gbCliente.Controls.Add(this.btnVerCorreo);
            this.gbCliente.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbCliente.Location = new System.Drawing.Point(484, 50);
            this.gbCliente.Name = "gbCliente";
            this.gbCliente.Size = new System.Drawing.Size(460, 230);
            this.gbCliente.TabIndex = 2;
            this.gbCliente.TabStop = false;
            this.gbCliente.Text = "Cliente";
            //
            // lblDni
            //
            this.lblDni.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDni.Location = new System.Drawing.Point(12, 34);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(60, 20);
            this.lblDni.TabIndex = 0;
            this.lblDni.Text = "DNI";
            //
            // txtDni
            //
            this.txtDni.Location = new System.Drawing.Point(80, 31);
            this.txtDni.MaxLength = 8;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(180, 25);
            this.txtDni.TabIndex = 1;
            //
            // btnBuscarCliente
            //
            this.btnBuscarCliente.Location = new System.Drawing.Point(270, 28);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(178, 30);
            this.btnBuscarCliente.TabIndex = 2;
            this.btnBuscarCliente.Text = "Buscar cliente";
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            //
            // lblClienteNombre
            //
            this.lblClienteNombre.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblClienteNombre.Location = new System.Drawing.Point(12, 72);
            this.lblClienteNombre.Name = "lblClienteNombre";
            this.lblClienteNombre.Size = new System.Drawing.Size(436, 24);
            this.lblClienteNombre.TabIndex = 3;
            //
            // lblClienteIva
            //
            this.lblClienteIva.Location = new System.Drawing.Point(12, 100);
            this.lblClienteIva.Name = "lblClienteIva";
            this.lblClienteIva.Size = new System.Drawing.Size(436, 22);
            this.lblClienteIva.TabIndex = 4;
            //
            // lblClienteCuit
            //
            this.lblClienteCuit.Location = new System.Drawing.Point(12, 124);
            this.lblClienteCuit.Name = "lblClienteCuit";
            this.lblClienteCuit.Size = new System.Drawing.Size(436, 22);
            this.lblClienteCuit.TabIndex = 5;
            //
            // lblCorreo
            //
            this.lblCorreo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCorreo.Location = new System.Drawing.Point(12, 168);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new System.Drawing.Size(60, 20);
            this.lblCorreo.TabIndex = 6;
            this.lblCorreo.Text = "Correo";
            //
            // txtCorreo
            //
            this.txtCorreo.Location = new System.Drawing.Point(80, 165);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.ReadOnly = true;
            this.txtCorreo.Size = new System.Drawing.Size(230, 25);
            this.txtCorreo.TabIndex = 7;
            //
            // btnVerCorreo
            //
            this.btnVerCorreo.Location = new System.Drawing.Point(320, 162);
            this.btnVerCorreo.Name = "btnVerCorreo";
            this.btnVerCorreo.Size = new System.Drawing.Size(128, 30);
            this.btnVerCorreo.TabIndex = 8;
            this.btnVerCorreo.Text = "Ver correo";
            this.btnVerCorreo.Click += new System.EventHandler(this.btnVerCorreo_Click);
            //
            // gbDetalle
            //
            this.gbDetalle.Controls.Add(this.dgvDetalle);
            this.gbDetalle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbDetalle.Location = new System.Drawing.Point(12, 290);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(932, 190);
            this.gbDetalle.TabIndex = 3;
            this.gbDetalle.TabStop = false;
            this.gbDetalle.Text = "Detalle del pedido";
            //
            // dgvDetalle
            //
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.Location = new System.Drawing.Point(12, 26);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.Size = new System.Drawing.Size(908, 152);
            this.dgvDetalle.TabIndex = 0;
            //
            // gbPago
            //
            this.gbPago.Controls.Add(this.lblMedioPago);
            this.gbPago.Controls.Add(this.cmbMedioPago);
            this.gbPago.Controls.Add(this.lblDatosPago);
            this.gbPago.Controls.Add(this.txtDatosPago);
            this.gbPago.Controls.Add(this.lblTotalTitulo);
            this.gbPago.Controls.Add(this.lblTotal);
            this.gbPago.Controls.Add(this.btnCobrar);
            this.gbPago.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbPago.Location = new System.Drawing.Point(12, 490);
            this.gbPago.Name = "gbPago";
            this.gbPago.Size = new System.Drawing.Size(932, 112);
            this.gbPago.TabIndex = 4;
            this.gbPago.TabStop = false;
            this.gbPago.Text = "Pago";
            //
            // lblMedioPago
            //
            this.lblMedioPago.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMedioPago.Location = new System.Drawing.Point(12, 34);
            this.lblMedioPago.Name = "lblMedioPago";
            this.lblMedioPago.Size = new System.Drawing.Size(130, 20);
            this.lblMedioPago.TabIndex = 0;
            this.lblMedioPago.Text = "Medio de pago";
            //
            // cmbMedioPago
            //
            this.cmbMedioPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedioPago.Location = new System.Drawing.Point(150, 31);
            this.cmbMedioPago.Name = "cmbMedioPago";
            this.cmbMedioPago.Size = new System.Drawing.Size(240, 25);
            this.cmbMedioPago.TabIndex = 1;
            this.cmbMedioPago.SelectedIndexChanged += new System.EventHandler(this.cmbMedioPago_SelectedIndexChanged);
            //
            // lblDatosPago
            //
            this.lblDatosPago.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDatosPago.Location = new System.Drawing.Point(12, 72);
            this.lblDatosPago.Name = "lblDatosPago";
            this.lblDatosPago.Size = new System.Drawing.Size(130, 20);
            this.lblDatosPago.TabIndex = 2;
            this.lblDatosPago.Text = "Datos del pago";
            //
            // txtDatosPago
            //
            this.txtDatosPago.Location = new System.Drawing.Point(150, 69);
            this.txtDatosPago.MaxLength = 60;
            this.txtDatosPago.Name = "txtDatosPago";
            this.txtDatosPago.Size = new System.Drawing.Size(240, 25);
            this.txtDatosPago.TabIndex = 3;
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitulo.Location = new System.Drawing.Point(560, 30);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(110, 28);
            this.lblTotalTitulo.TabIndex = 4;
            this.lblTotalTitulo.Text = "Total";
            this.lblTotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(676, 24);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(244, 36);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "0,00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnCobrar
            //
            this.btnCobrar.Location = new System.Drawing.Point(676, 66);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(244, 36);
            this.btnCobrar.TabIndex = 6;
            this.btnCobrar.Text = "Cobrar y emitir comprobante";
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(784, 614);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 34);
            this.btnSalir.TabIndex = 5;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(12, 618);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(760, 26);
            this.lblMensaje.TabIndex = 6;
            //
            // frmValidarCobrar_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(956, 660);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.gbOrdenes);
            this.Controls.Add(this.gbCliente);
            this.Controls.Add(this.gbDetalle);
            this.Controls.Add(this.gbPago);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblMensaje);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "frmValidarCobrar_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validar Pedido y Cobrar";
            this.Load += new System.EventHandler(this.frmValidarCobrar_Load);
            this.gbOrdenes.ResumeLayout(false);
            this.gbOrdenes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbCliente.ResumeLayout(false);
            this.gbCliente.PerformLayout();
            this.gbDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.gbPago.ResumeLayout(false);
            this.gbPago.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.Label lblNroOrden;
        private System.Windows.Forms.TextBox txtNroOrden;
        private System.Windows.Forms.Button btnBuscarOrden;
        private System.Windows.Forms.GroupBox gbCliente;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Button btnBuscarCliente;
        private System.Windows.Forms.Label lblClienteNombre;
        private System.Windows.Forms.Label lblClienteIva;
        private System.Windows.Forms.Label lblClienteCuit;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Button btnVerCorreo;
        private System.Windows.Forms.GroupBox gbDetalle;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.GroupBox gbPago;
        private System.Windows.Forms.Label lblMedioPago;
        private System.Windows.Forms.ComboBox cmbMedioPago;
        private System.Windows.Forms.Label lblDatosPago;
        private System.Windows.Forms.TextBox txtDatosPago;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
