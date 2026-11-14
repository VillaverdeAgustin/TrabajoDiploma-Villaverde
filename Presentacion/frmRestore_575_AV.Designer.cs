namespace Presentacion_575_AV
{
    partial class frmRestore_575_AV
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
            this.lblAdvertencia = new System.Windows.Forms.Label();
            this.gbBackups = new System.Windows.Forms.GroupBox();
            this.dgvBackups = new System.Windows.Forms.DataGridView();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTamano = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRuta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbArchivo = new System.Windows.Forms.GroupBox();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.ofdBackup = new System.Windows.Forms.OpenFileDialog();
            this.gbBackups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).BeginInit();
            this.gbArchivo.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Restaurar un backup";
            //
            // lblAdvertencia
            //
            this.lblAdvertencia.ForeColor = System.Drawing.Color.Firebrick;
            this.lblAdvertencia.Location = new System.Drawing.Point(16, 40);
            this.lblAdvertencia.Name = "lblAdvertencia";
            this.lblAdvertencia.Size = new System.Drawing.Size(968, 40);
            this.lblAdvertencia.TabIndex = 1;
            this.lblAdvertencia.Text = "Restaurar reemplaza todos los datos actuales de ConectAR_DB por los del backup. Después se recalculan los dígitos verificadores y se cierra la sesión.";
            //
            // gbBackups
            //
            this.gbBackups.Controls.Add(this.dgvBackups);
            this.gbBackups.Location = new System.Drawing.Point(16, 86);
            this.gbBackups.Name = "gbBackups";
            this.gbBackups.Size = new System.Drawing.Size(968, 268);
            this.gbBackups.TabIndex = 2;
            this.gbBackups.Text = "Backups registrados en el servidor";
            //
            // dgvBackups
            //
            this.dgvBackups.AllowUserToAddRows = false;
            this.dgvBackups.AllowUserToDeleteRows = false;
            this.dgvBackups.AllowUserToResizeRows = false;
            this.dgvBackups.AutoGenerateColumns = false;
            this.dgvBackups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFecha,
            this.colTamano,
            this.colRuta});
            this.dgvBackups.MultiSelect = false;
            this.dgvBackups.ReadOnly = true;
            this.dgvBackups.Location = new System.Drawing.Point(12, 26);
            this.dgvBackups.Name = "dgvBackups";
            this.dgvBackups.Size = new System.Drawing.Size(944, 230);
            this.dgvBackups.TabIndex = 3;
            this.dgvBackups.SelectionChanged += new System.EventHandler(this.dgvBackups_SelectionChanged);
            //
            // colFecha
            //
            this.colFecha.DataPropertyName = "fecha";
            this.colFecha.HeaderText = "Fecha";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            this.colFecha.Width = 150;
            //
            // colTamano
            //
            this.colTamano.DataPropertyName = "tamano_mb";
            this.colTamano.HeaderText = "Tamaño (MB)";
            this.colTamano.Name = "colTamano";
            this.colTamano.ReadOnly = true;
            this.colTamano.Width = 120;
            //
            // colRuta
            //
            this.colRuta.DataPropertyName = "ruta";
            this.colRuta.HeaderText = "Archivo";
            this.colRuta.Name = "colRuta";
            this.colRuta.ReadOnly = true;
            this.colRuta.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // gbArchivo
            //
            this.gbArchivo.Controls.Add(this.txtArchivo);
            this.gbArchivo.Controls.Add(this.btnExaminar);
            this.gbArchivo.Location = new System.Drawing.Point(16, 362);
            this.gbArchivo.Name = "gbArchivo";
            this.gbArchivo.Size = new System.Drawing.Size(968, 76);
            this.gbArchivo.TabIndex = 4;
            this.gbArchivo.Text = "Archivo a restaurar";
            //
            // txtArchivo
            //
            this.txtArchivo.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtArchivo.Location = new System.Drawing.Point(14, 32);
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.Size = new System.Drawing.Size(780, 25);
            this.txtArchivo.TabIndex = 5;
            //
            // btnExaminar
            //
            this.btnExaminar.Location = new System.Drawing.Point(806, 28);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(150, 32);
            this.btnExaminar.TabIndex = 6;
            this.btnExaminar.Text = "Examinar...";
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            //
            // btnRestaurar
            //
            this.btnRestaurar.Location = new System.Drawing.Point(16, 448);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(200, 36);
            this.btnRestaurar.TabIndex = 7;
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            //
            // lblMensaje
            //
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje.Location = new System.Drawing.Point(230, 448);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(580, 36);
            this.lblMensaje.TabIndex = 8;
            this.lblMensaje.Text = "";
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(832, 448);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(152, 36);
            this.btnSalir.TabIndex = 9;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmRestore_575_AV
            //
            this.ClientSize = new System.Drawing.Size(1000, 500);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblAdvertencia);
            this.Controls.Add(this.gbBackups);
            this.Controls.Add(this.gbArchivo);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnSalir);
            this.Name = "frmRestore_575_AV";
            this.Text = "Restaurar un backup";
            this.Load += new System.EventHandler(this.frmRestore_Load);
            this.gbArchivo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).EndInit();
            this.gbBackups.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblAdvertencia;
        private System.Windows.Forms.GroupBox gbBackups;
        private System.Windows.Forms.DataGridView dgvBackups;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTamano;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRuta;
        private System.Windows.Forms.GroupBox gbArchivo;
        private System.Windows.Forms.TextBox txtArchivo;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.OpenFileDialog ofdBackup;
    }
}
