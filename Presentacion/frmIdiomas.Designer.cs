namespace Presentacion_575_AV
{
    partial class frmIdiomas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbNuevo = new System.Windows.Forms.GroupBox();
            this.btnCrear = new System.Windows.Forms.Button();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.lblBase = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.gbTraducciones = new System.Windows.Forms.GroupBox();
            this.dgvTraducciones = new System.Windows.Forms.DataGridView();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.cmbIdioma = new System.Windows.Forms.ComboBox();
            this.lblIdioma = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.gbNuevo.SuspendLayout();
            this.gbTraducciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).BeginInit();
            this.SuspendLayout();
            //
            // gbNuevo
            //
            this.gbNuevo.Controls.Add(this.btnCrear);
            this.gbNuevo.Controls.Add(this.lblArchivo);
            this.gbNuevo.Controls.Add(this.txtArchivo);
            this.gbNuevo.Controls.Add(this.btnExaminar);
            this.gbNuevo.Controls.Add(this.lblBase);
            this.gbNuevo.Controls.Add(this.txtNombre);
            this.gbNuevo.Controls.Add(this.lblNombre);
            this.gbNuevo.Controls.Add(this.txtCodigo);
            this.gbNuevo.Controls.Add(this.lblCodigo);
            this.gbNuevo.Location = new System.Drawing.Point(12, 12);
            this.gbNuevo.Name = "gbNuevo";
            this.gbNuevo.Size = new System.Drawing.Size(380, 290);
            this.gbNuevo.TabIndex = 0;
            this.gbNuevo.TabStop = false;
            this.gbNuevo.Text = "Nuevo Idioma";
            //
            // lblCodigo
            //
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Location = new System.Drawing.Point(16, 35);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(50, 15);
            this.lblCodigo.TabIndex = 0;
            this.lblCodigo.Text = "Código";
            //
            // txtCodigo
            //
            this.txtCodigo.Location = new System.Drawing.Point(120, 32);
            this.txtCodigo.MaxLength = 5;
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(80, 23);
            this.txtCodigo.TabIndex = 1;
            this.txtCodigo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(16, 75);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(54, 15);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "Nombre";
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(120, 72);
            this.txtNombre.MaxLength = 50;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(230, 23);
            this.txtNombre.TabIndex = 3;
            //
            // lblArchivo
            //
            this.lblArchivo.Location = new System.Drawing.Point(16, 112);
            this.lblArchivo.Name = "lblArchivo";
            this.lblArchivo.Size = new System.Drawing.Size(348, 18);
            this.lblArchivo.TabIndex = 4;
            this.lblArchivo.Text = "Archivo de traducciones (opcional)";
            //
            // txtArchivo
            //
            this.txtArchivo.Location = new System.Drawing.Point(16, 133);
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.ReadOnly = true;
            this.txtArchivo.Size = new System.Drawing.Size(252, 23);
            this.txtArchivo.TabIndex = 5;
            //
            // btnExaminar
            //
            this.btnExaminar.Location = new System.Drawing.Point(274, 132);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(90, 25);
            this.btnExaminar.TabIndex = 6;
            this.btnExaminar.Text = "Examinar...";
            this.btnExaminar.UseVisualStyleBackColor = true;
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            //
            // lblBase
            //
            this.lblBase.Location = new System.Drawing.Point(16, 168);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(348, 60);
            this.lblBase.TabIndex = 7;
            this.lblBase.Text = "Al crear un idioma se copian las traducciones del idioma base (Español) como punto de partida.";
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(120, 240);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(130, 30);
            this.btnCrear.TabIndex = 8;
            this.btnCrear.Text = "Crear Idioma";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // gbTraducciones
            //
            this.gbTraducciones.Controls.Add(this.dgvTraducciones);
            this.gbTraducciones.Controls.Add(this.btnGuardar);
            this.gbTraducciones.Controls.Add(this.cmbIdioma);
            this.gbTraducciones.Controls.Add(this.lblIdioma);
            this.gbTraducciones.Location = new System.Drawing.Point(410, 12);
            this.gbTraducciones.Name = "gbTraducciones";
            this.gbTraducciones.Size = new System.Drawing.Size(470, 540);
            this.gbTraducciones.TabIndex = 1;
            this.gbTraducciones.TabStop = false;
            this.gbTraducciones.Text = "Traducciones";
            //
            // lblIdioma
            //
            this.lblIdioma.AutoSize = true;
            this.lblIdioma.Location = new System.Drawing.Point(16, 30);
            this.lblIdioma.Name = "lblIdioma";
            this.lblIdioma.Size = new System.Drawing.Size(47, 15);
            this.lblIdioma.TabIndex = 0;
            this.lblIdioma.Text = "Idioma:";
            //
            // cmbIdioma
            //
            this.cmbIdioma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdioma.FormattingEnabled = true;
            this.cmbIdioma.Location = new System.Drawing.Point(90, 27);
            this.cmbIdioma.Name = "cmbIdioma";
            this.cmbIdioma.Size = new System.Drawing.Size(200, 23);
            this.cmbIdioma.TabIndex = 1;
            this.cmbIdioma.SelectedIndexChanged += new System.EventHandler(this.cmbIdioma_SelectedIndexChanged);
            //
            // dgvTraducciones
            //
            this.dgvTraducciones.AllowUserToAddRows = false;
            this.dgvTraducciones.AllowUserToDeleteRows = false;
            this.dgvTraducciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTraducciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTraducciones.Location = new System.Drawing.Point(16, 65);
            this.dgvTraducciones.Name = "dgvTraducciones";
            this.dgvTraducciones.RowHeadersVisible = false;
            this.dgvTraducciones.Size = new System.Drawing.Size(438, 420);
            this.dgvTraducciones.TabIndex = 2;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(334, 495);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 30);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(12, 522);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(120, 30);
            this.btnSalir.TabIndex = 2;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmIdiomas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 566);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.gbTraducciones);
            this.Controls.Add(this.gbNuevo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmIdiomas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestión de Idiomas";
            this.Load += new System.EventHandler(this.frmIdiomas_Load);
            this.gbNuevo.ResumeLayout(false);
            this.gbNuevo.PerformLayout();
            this.gbTraducciones.ResumeLayout(false);
            this.gbTraducciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gbNuevo;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Label lblArchivo;
        private System.Windows.Forms.TextBox txtArchivo;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.Label lblBase;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.GroupBox gbTraducciones;
        private System.Windows.Forms.DataGridView dgvTraducciones;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.ComboBox cmbIdioma;
        private System.Windows.Forms.Label lblIdioma;
        private System.Windows.Forms.Button btnSalir;
    }
}
