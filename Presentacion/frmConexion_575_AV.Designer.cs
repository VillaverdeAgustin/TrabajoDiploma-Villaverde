namespace Presentacion_575_AV
{
    partial class frmConexion_575_AV
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
            this.lblAyuda = new System.Windows.Forms.Label();
            this.lblInstancia = new System.Windows.Forms.Label();
            this.cmbInstancia = new System.Windows.Forms.ComboBox();
            this.btnProbar = new System.Windows.Forms.Button();
            this.btnCrear = new System.Windows.Forms.Button();
            this.pbAvance = new System.Windows.Forms.ProgressBar();
            this.lstAvance = new System.Windows.Forms.ListBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(528, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Conexión a la base de datos";
            //
            // lblAyuda
            //
            this.lblAyuda.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblAyuda.Location = new System.Drawing.Point(16, 48);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(528, 36);
            this.lblAyuda.TabIndex = 1;
            this.lblAyuda.Text = "Elija la instancia de SQL Server donde está la base ConectAR_DB o donde se va a crear.";
            //
            // lblInstancia
            //
            this.lblInstancia.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblInstancia.Location = new System.Drawing.Point(16, 96);
            this.lblInstancia.Name = "lblInstancia";
            this.lblInstancia.Size = new System.Drawing.Size(340, 20);
            this.lblInstancia.TabIndex = 2;
            this.lblInstancia.Text = "Instancia de SQL Server";
            //
            // cmbInstancia
            //
            this.cmbInstancia.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmbInstancia.Location = new System.Drawing.Point(16, 120);
            this.cmbInstancia.Name = "cmbInstancia";
            this.cmbInstancia.Size = new System.Drawing.Size(340, 25);
            this.cmbInstancia.TabIndex = 3;
            this.cmbInstancia.TextChanged += new System.EventHandler(this.cmbInstancia_TextChanged);
            //
            // btnProbar
            //
            this.btnProbar.Location = new System.Drawing.Point(370, 116);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(174, 32);
            this.btnProbar.TabIndex = 4;
            this.btnProbar.Text = "Probar conexión";
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(16, 160);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(200, 34);
            this.btnCrear.TabIndex = 5;
            this.btnCrear.Text = "Crear base de datos";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // pbAvance
            //
            this.pbAvance.Location = new System.Drawing.Point(230, 166);
            this.pbAvance.Name = "pbAvance";
            this.pbAvance.Size = new System.Drawing.Size(314, 22);
            this.pbAvance.TabIndex = 6;
            //
            // lstAvance
            //
            this.lstAvance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lstAvance.IntegralHeight = false;
            this.lstAvance.ItemHeight = 17;
            this.lstAvance.Location = new System.Drawing.Point(16, 206);
            this.lstAvance.Name = "lstAvance";
            this.lstAvance.Size = new System.Drawing.Size(528, 170);
            this.lstAvance.TabIndex = 7;
            //
            // lblMensaje
            //
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(16, 384);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(528, 40);
            this.lblMensaje.TabIndex = 8;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(214, 430);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(180, 36);
            this.btnGuardar.TabIndex = 9;
            this.btnGuardar.Text = "Guardar y continuar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(404, 430);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(140, 36);
            this.btnSalir.TabIndex = 10;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmConexion_575_AV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(560, 480);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.lblInstancia);
            this.Controls.Add(this.cmbInstancia);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.pbAvance);
            this.Controls.Add(this.lstAvance);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnSalir);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConexion_575_AV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conexión a la base de datos";
            this.Load += new System.EventHandler(this.frmConexion_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Label lblInstancia;
        private System.Windows.Forms.ComboBox cmbInstancia;
        private System.Windows.Forms.Button btnProbar;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.ProgressBar pbAvance;
        private System.Windows.Forms.ListBox lstAvance;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnSalir;
    }
}
