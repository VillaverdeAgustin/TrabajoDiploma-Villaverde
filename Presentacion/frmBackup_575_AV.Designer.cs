namespace Presentacion_575_AV
{
    partial class frmBackup_575_AV
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
            this.gbDestino = new System.Windows.Forms.GroupBox();
            this.lblCarpeta = new System.Windows.Forms.Label();
            this.txtCarpeta = new System.Windows.Forms.TextBox();
            this.btnPredeterminada = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblNombreValor = new System.Windows.Forms.Label();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.gbResultado = new System.Windows.Forms.GroupBox();
            this.lblRuta = new System.Windows.Forms.Label();
            this.txtRuta = new System.Windows.Forms.TextBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblFechaValor = new System.Windows.Forms.Label();
            this.lblTamano = new System.Windows.Forms.Label();
            this.lblTamanoValor = new System.Windows.Forms.Label();
            this.lblVerificacion = new System.Windows.Forms.Label();
            this.lblVerificacionValor = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.gbDestino.SuspendLayout();
            this.gbResultado.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Backup de la base de datos";
            //
            // lblAyuda
            //
            this.lblAyuda.Location = new System.Drawing.Point(16, 40);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(868, 40);
            this.lblAyuda.TabIndex = 1;
            this.lblAyuda.Text = "El archivo lo escribe el servicio de SQL Server, no el usuario: por eso se guarda en una carpeta del servidor. Por defecto, la carpeta de backups de la instancia.";
            //
            // gbDestino
            //
            this.gbDestino.Controls.Add(this.lblCarpeta);
            this.gbDestino.Controls.Add(this.txtCarpeta);
            this.gbDestino.Controls.Add(this.btnPredeterminada);
            this.gbDestino.Controls.Add(this.lblNombre);
            this.gbDestino.Controls.Add(this.lblNombreValor);
            this.gbDestino.Location = new System.Drawing.Point(16, 86);
            this.gbDestino.Name = "gbDestino";
            this.gbDestino.Size = new System.Drawing.Size(868, 108);
            this.gbDestino.TabIndex = 2;
            this.gbDestino.Text = "Destino";
            //
            // lblCarpeta
            //
            this.lblCarpeta.AutoSize = true;
            this.lblCarpeta.Location = new System.Drawing.Point(14, 34);
            this.lblCarpeta.Name = "lblCarpeta";
            this.lblCarpeta.TabIndex = 3;
            this.lblCarpeta.Text = "Carpeta:";
            //
            // txtCarpeta
            //
            this.txtCarpeta.Location = new System.Drawing.Point(150, 30);
            this.txtCarpeta.Name = "txtCarpeta";
            this.txtCarpeta.Size = new System.Drawing.Size(560, 25);
            this.txtCarpeta.TabIndex = 4;
            //
            // btnPredeterminada
            //
            this.btnPredeterminada.Location = new System.Drawing.Point(720, 26);
            this.btnPredeterminada.Name = "btnPredeterminada";
            this.btnPredeterminada.Size = new System.Drawing.Size(136, 32);
            this.btnPredeterminada.TabIndex = 5;
            this.btnPredeterminada.Text = "Predeterminada";
            this.btnPredeterminada.Click += new System.EventHandler(this.btnPredeterminada_Click);
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(14, 72);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.TabIndex = 6;
            this.lblNombre.Text = "Nombre del archivo:";
            //
            // lblNombreValor
            //
            this.lblNombreValor.Location = new System.Drawing.Point(150, 72);
            this.lblNombreValor.Name = "lblNombreValor";
            this.lblNombreValor.Size = new System.Drawing.Size(560, 20);
            this.lblNombreValor.TabIndex = 7;
            this.lblNombreValor.Text = "";
            //
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(16, 204);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(200, 36);
            this.btnGenerar.TabIndex = 8;
            this.btnGenerar.Text = "Generar backup";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // gbResultado
            //
            this.gbResultado.Controls.Add(this.lblRuta);
            this.gbResultado.Controls.Add(this.txtRuta);
            this.gbResultado.Controls.Add(this.lblFecha);
            this.gbResultado.Controls.Add(this.lblFechaValor);
            this.gbResultado.Controls.Add(this.lblTamano);
            this.gbResultado.Controls.Add(this.lblTamanoValor);
            this.gbResultado.Controls.Add(this.lblVerificacion);
            this.gbResultado.Controls.Add(this.lblVerificacionValor);
            this.gbResultado.Location = new System.Drawing.Point(16, 252);
            this.gbResultado.Name = "gbResultado";
            this.gbResultado.Size = new System.Drawing.Size(868, 150);
            this.gbResultado.TabIndex = 9;
            this.gbResultado.Text = "Último backup generado";
            //
            // lblRuta
            //
            this.lblRuta.AutoSize = true;
            this.lblRuta.Location = new System.Drawing.Point(14, 34);
            this.lblRuta.Name = "lblRuta";
            this.lblRuta.TabIndex = 10;
            this.lblRuta.Text = "Archivo:";
            //
            // txtRuta
            //
            this.txtRuta.ReadOnly = true;
            this.txtRuta.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtRuta.Location = new System.Drawing.Point(150, 30);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Size = new System.Drawing.Size(706, 25);
            this.txtRuta.TabIndex = 11;
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(14, 70);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.TabIndex = 12;
            this.lblFecha.Text = "Fecha:";
            //
            // lblFechaValor
            //
            this.lblFechaValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblFechaValor.Location = new System.Drawing.Point(150, 70);
            this.lblFechaValor.Name = "lblFechaValor";
            this.lblFechaValor.Size = new System.Drawing.Size(250, 20);
            this.lblFechaValor.TabIndex = 13;
            this.lblFechaValor.Text = "";
            //
            // lblTamano
            //
            this.lblTamano.AutoSize = true;
            this.lblTamano.Location = new System.Drawing.Point(420, 70);
            this.lblTamano.Name = "lblTamano";
            this.lblTamano.TabIndex = 14;
            this.lblTamano.Text = "Tamaño:";
            //
            // lblTamanoValor
            //
            this.lblTamanoValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTamanoValor.Location = new System.Drawing.Point(520, 70);
            this.lblTamanoValor.Name = "lblTamanoValor";
            this.lblTamanoValor.Size = new System.Drawing.Size(200, 20);
            this.lblTamanoValor.TabIndex = 15;
            this.lblTamanoValor.Text = "";
            //
            // lblVerificacion
            //
            this.lblVerificacion.AutoSize = true;
            this.lblVerificacion.Location = new System.Drawing.Point(14, 106);
            this.lblVerificacion.Name = "lblVerificacion";
            this.lblVerificacion.TabIndex = 16;
            this.lblVerificacion.Text = "Verificación:";
            //
            // lblVerificacionValor
            //
            this.lblVerificacionValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblVerificacionValor.Location = new System.Drawing.Point(150, 106);
            this.lblVerificacionValor.Name = "lblVerificacionValor";
            this.lblVerificacionValor.Size = new System.Drawing.Size(706, 20);
            this.lblVerificacionValor.TabIndex = 17;
            this.lblVerificacionValor.Text = "";
            //
            // lblMensaje
            //
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje.Location = new System.Drawing.Point(16, 412);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(700, 34);
            this.lblMensaje.TabIndex = 18;
            this.lblMensaje.Text = "";
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(732, 412);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(152, 34);
            this.btnSalir.TabIndex = 19;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmBackup_575_AV
            //
            this.ClientSize = new System.Drawing.Size(900, 460);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.gbDestino);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.gbResultado);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnSalir);
            this.Name = "frmBackup_575_AV";
            this.Text = "Backup de la base de datos";
            this.Load += new System.EventHandler(this.frmBackup_Load);
            this.gbResultado.ResumeLayout(false);
            this.gbResultado.PerformLayout();
            this.gbDestino.ResumeLayout(false);
            this.gbDestino.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.GroupBox gbDestino;
        private System.Windows.Forms.Label lblCarpeta;
        private System.Windows.Forms.TextBox txtCarpeta;
        private System.Windows.Forms.Button btnPredeterminada;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblNombreValor;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.GroupBox gbResultado;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.TextBox txtRuta;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblFechaValor;
        private System.Windows.Forms.Label lblTamano;
        private System.Windows.Forms.Label lblTamanoValor;
        private System.Windows.Forms.Label lblVerificacion;
        private System.Windows.Forms.Label lblVerificacionValor;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnSalir;
    }
}
