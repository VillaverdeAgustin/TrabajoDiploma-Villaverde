namespace Presentacion_575_AV
{
    partial class frmConsultaComprobantes_575_AV
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
            this.gbBusqueda = new System.Windows.Forms.GroupBox();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblDesde = new System.Windows.Forms.Label();
            this.chkFecha = new System.Windows.Forms.CheckBox();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblNumero = new System.Windows.Forms.Label();
            this.gbResultados = new System.Windows.Forms.GroupBox();
            this.dgvComprobantes = new System.Windows.Forms.DataGridView();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.btnVer = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.gbBusqueda.SuspendLayout();
            this.gbResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprobantes)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(160, 17);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Consultar comprobantes";
            //
            // gbBusqueda
            //
            this.gbBusqueda.Controls.Add(this.btnLimpiar);
            this.gbBusqueda.Controls.Add(this.btnBuscar);
            this.gbBusqueda.Controls.Add(this.dtpHasta);
            this.gbBusqueda.Controls.Add(this.lblHasta);
            this.gbBusqueda.Controls.Add(this.dtpDesde);
            this.gbBusqueda.Controls.Add(this.lblDesde);
            this.gbBusqueda.Controls.Add(this.chkFecha);
            this.gbBusqueda.Controls.Add(this.txtCliente);
            this.gbBusqueda.Controls.Add(this.lblCliente);
            this.gbBusqueda.Controls.Add(this.txtNumero);
            this.gbBusqueda.Controls.Add(this.lblNumero);
            this.gbBusqueda.Location = new System.Drawing.Point(16, 40);
            this.gbBusqueda.Name = "gbBusqueda";
            this.gbBusqueda.Size = new System.Drawing.Size(900, 116);
            this.gbBusqueda.TabIndex = 1;
            this.gbBusqueda.TabStop = false;
            this.gbBusqueda.Text = "Búsqueda";
            //
            // lblNumero
            //
            this.lblNumero.Location = new System.Drawing.Point(14, 28);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.Size = new System.Drawing.Size(200, 19);
            this.lblNumero.TabIndex = 0;
            this.lblNumero.Text = "Número:";
            //
            // txtNumero
            //
            this.txtNumero.Location = new System.Drawing.Point(16, 50);
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(200, 26);
            this.txtNumero.TabIndex = 1;
            //
            // lblCliente
            //
            this.lblCliente.Location = new System.Drawing.Point(234, 28);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(280, 19);
            this.lblCliente.TabIndex = 2;
            this.lblCliente.Text = "Cliente (nombre, DNI o CUIT):";
            //
            // txtCliente
            //
            this.txtCliente.Location = new System.Drawing.Point(236, 50);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.Size = new System.Drawing.Size(280, 26);
            this.txtCliente.TabIndex = 3;
            //
            // chkFecha
            //
            this.chkFecha.Location = new System.Drawing.Point(536, 26);
            this.chkFecha.Name = "chkFecha";
            this.chkFecha.Size = new System.Drawing.Size(240, 22);
            this.chkFecha.TabIndex = 4;
            this.chkFecha.Text = "Filtrar por fecha de emisión";
            this.chkFecha.CheckedChanged += new System.EventHandler(this.chkFecha_CheckedChanged);
            //
            // lblDesde
            //
            this.lblDesde.Location = new System.Drawing.Point(536, 54);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(56, 19);
            this.lblDesde.TabIndex = 5;
            this.lblDesde.Text = "Desde";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(594, 51);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(120, 25);
            this.dtpDesde.TabIndex = 6;
            //
            // lblHasta
            //
            this.lblHasta.Location = new System.Drawing.Point(536, 84);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(56, 19);
            this.lblHasta.TabIndex = 7;
            this.lblHasta.Text = "Hasta";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(594, 81);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(120, 25);
            this.dtpHasta.TabIndex = 8;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(756, 26);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(128, 34);
            this.btnBuscar.TabIndex = 9;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // btnLimpiar
            //
            this.btnLimpiar.Location = new System.Drawing.Point(756, 68);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(128, 34);
            this.btnLimpiar.TabIndex = 10;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // gbResultados
            //
            this.gbResultados.Controls.Add(this.dgvComprobantes);
            this.gbResultados.Location = new System.Drawing.Point(16, 166);
            this.gbResultados.Name = "gbResultados";
            this.gbResultados.Padding = new System.Windows.Forms.Padding(8);
            this.gbResultados.Size = new System.Drawing.Size(900, 330);
            this.gbResultados.TabIndex = 2;
            this.gbResultados.TabStop = false;
            this.gbResultados.Text = "Comprobantes emitidos";
            //
            // dgvComprobantes
            //
            this.dgvComprobantes.AllowUserToAddRows = false;
            this.dgvComprobantes.AllowUserToDeleteRows = false;
            this.dgvComprobantes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvComprobantes.MultiSelect = false;
            this.dgvComprobantes.Name = "dgvComprobantes";
            this.dgvComprobantes.ReadOnly = true;
            this.dgvComprobantes.TabIndex = 0;
            this.dgvComprobantes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComprobantes_CellDoubleClick);
            //
            // lblAyuda
            //
            this.lblAyuda.Location = new System.Drawing.Point(16, 504);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(500, 19);
            this.lblAyuda.TabIndex = 3;
            this.lblAyuda.Text = "Doble clic en un comprobante para abrirlo.";
            //
            // btnVer
            //
            this.btnVer.Location = new System.Drawing.Point(640, 504);
            this.btnVer.Name = "btnVer";
            this.btnVer.Size = new System.Drawing.Size(150, 36);
            this.btnVer.TabIndex = 4;
            this.btnVer.Text = "Ver comprobante";
            this.btnVer.Click += new System.EventHandler(this.btnVer_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(796, 504);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(120, 36);
            this.btnSalir.TabIndex = 5;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.Location = new System.Drawing.Point(16, 528);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(600, 19);
            this.lblMensaje.TabIndex = 6;
            //
            // frmConsultaComprobantes_575_AV
            //
            this.ClientSize = new System.Drawing.Size(934, 556);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnVer);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.gbResultados);
            this.Controls.Add(this.gbBusqueda);
            this.Controls.Add(this.lblTitulo);
            this.Name = "frmConsultaComprobantes_575_AV";
            this.Text = "Consultar comprobantes";
            this.Load += new System.EventHandler(this.frmConsultaComprobantes_Load);
            this.gbBusqueda.ResumeLayout(false);
            this.gbBusqueda.PerformLayout();
            this.gbResultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprobantes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbBusqueda;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.TextBox txtCliente;
        private System.Windows.Forms.CheckBox chkFecha;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.GroupBox gbResultados;
        private System.Windows.Forms.DataGridView dgvComprobantes;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Button btnVer;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblMensaje;
    }
}
