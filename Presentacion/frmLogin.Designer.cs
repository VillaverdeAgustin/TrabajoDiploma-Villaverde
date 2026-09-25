namespace Presentacion_575_AV
{
    partial class frmLogin
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cambiarIdiomaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTarjeta = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblContra = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblSinConexion = new System.Windows.Forms.Label();
            this.lblPie = new System.Windows.Forms.Label();
            this.txtContra = new Presentacion_575_AV.ucAlfaNum();
            this.txtUsuario = new Presentacion_575_AV.ucAlfaNum();
            this.menuStrip1.SuspendLayout();
            this.pnlTarjeta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(59)))), ((int)(((byte)(92)))));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cambiarIdiomaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(400, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            //
            // cambiarIdiomaToolStripMenuItem
            //
            this.cambiarIdiomaToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.cambiarIdiomaToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.cambiarIdiomaToolStripMenuItem.Name = "cambiarIdiomaToolStripMenuItem";
            this.cambiarIdiomaToolStripMenuItem.Size = new System.Drawing.Size(115, 24);
            this.cambiarIdiomaToolStripMenuItem.Text = "Cambiar Idioma";
            //
            // pnlTarjeta
            //
            this.pnlTarjeta.BackColor = System.Drawing.Color.White;
            this.pnlTarjeta.Controls.Add(this.pictureBox1);
            this.pnlTarjeta.Controls.Add(this.lblUsuario);
            this.pnlTarjeta.Controls.Add(this.txtUsuario);
            this.pnlTarjeta.Controls.Add(this.lblContra);
            this.pnlTarjeta.Controls.Add(this.txtContra);
            this.pnlTarjeta.Controls.Add(this.lblError);
            this.pnlTarjeta.Controls.Add(this.btnIniciar);
            this.pnlTarjeta.Controls.Add(this.btnCancelar);
            this.pnlTarjeta.Controls.Add(this.lblSinConexion);
            this.pnlTarjeta.Controls.Add(this.lblPie);
            this.pnlTarjeta.Location = new System.Drawing.Point(0, 28);
            this.pnlTarjeta.Name = "pnlTarjeta";
            this.pnlTarjeta.Size = new System.Drawing.Size(400, 538);
            this.pnlTarjeta.TabIndex = 1;
            //
            // pictureBox1
            //
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::Presentacion_575_AV.Properties.Resources.LogoConectAR;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(85, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(230, 186);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(59)))), ((int)(((byte)(92)))));
            this.lblUsuario.Location = new System.Drawing.Point(56, 222);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(58, 17);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario";
            //
            // txtUsuario
            //
            this.txtUsuario.BackColor = System.Drawing.Color.White;
            this.txtUsuario.isPass = false;
            this.txtUsuario.Location = new System.Drawing.Point(56, 244);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.ok = false;
            this.txtUsuario.Size = new System.Drawing.Size(288, 44);
            this.txtUsuario.TabIndex = 2;
            this.txtUsuario.texto = "";
            this.txtUsuario.Leave += new System.EventHandler(this.txtUsuario_Leave);
            //
            // lblContra
            //
            this.lblContra.AutoSize = true;
            this.lblContra.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblContra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(59)))), ((int)(((byte)(92)))));
            this.lblContra.Location = new System.Drawing.Point(56, 300);
            this.lblContra.Name = "lblContra";
            this.lblContra.Size = new System.Drawing.Size(82, 17);
            this.lblContra.TabIndex = 3;
            this.lblContra.Text = "Contraseña";
            //
            // txtContra
            //
            this.txtContra.BackColor = System.Drawing.Color.White;
            this.txtContra.isPass = false;
            this.txtContra.Location = new System.Drawing.Point(56, 322);
            this.txtContra.Name = "txtContra";
            this.txtContra.ok = false;
            this.txtContra.Size = new System.Drawing.Size(288, 44);
            this.txtContra.TabIndex = 4;
            this.txtContra.texto = "";
            this.txtContra.Load += new System.EventHandler(this.txtContra_Load);
            //
            // lblError
            //
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.lblError.Location = new System.Drawing.Point(56, 374);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(288, 36);
            this.lblError.TabIndex = 5;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnIniciar
            //
            this.btnIniciar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnIniciar.Location = new System.Drawing.Point(56, 416);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(138, 38);
            this.btnIniciar.TabIndex = 6;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.Firebrick;
            this.btnCancelar.Location = new System.Drawing.Point(206, 416);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(138, 38);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // lblSinConexion
            //
            this.lblSinConexion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSinConexion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Underline);
            this.lblSinConexion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(88)))), ((int)(((byte)(116)))));
            this.lblSinConexion.Location = new System.Drawing.Point(56, 470);
            this.lblSinConexion.Name = "lblSinConexion";
            this.lblSinConexion.Size = new System.Drawing.Size(288, 22);
            this.lblSinConexion.TabIndex = 8;
            this.lblSinConexion.Text = "Iniciar sin conexion";
            this.lblSinConexion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSinConexion.Click += new System.EventHandler(this.lblSinConexion_Click);
            this.lblSinConexion.MouseLeave += new System.EventHandler(this.lblSinConexion_MouseLeave);
            this.lblSinConexion.MouseHover += new System.EventHandler(this.lblSinConexion_MouseHover);
            //
            // lblPie
            //
            this.lblPie.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblPie.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(113)))), ((int)(((byte)(131)))));
            this.lblPie.Location = new System.Drawing.Point(56, 502);
            this.lblPie.Name = "lblPie";
            this.lblPie.Size = new System.Drawing.Size(288, 18);
            this.lblPie.TabIndex = 9;
            this.lblPie.Text = "ConectAR S.R.L.  ·  Gestor de Ventas y Compras";
            this.lblPie.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // frmLogin
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 566);
            this.Controls.Add(this.pnlTarjeta);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = global::Presentacion_575_AV.Properties.Resources.IconoConectAR;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConectAR - Iniciar sesion";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.menuStrip1.ResumeLayout(false);
            this.pnlTarjeta.ResumeLayout(false);
            this.pnlTarjeta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cambiarIdiomaToolStripMenuItem;
        private System.Windows.Forms.Panel pnlTarjeta;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Presentacion_575_AV.ucAlfaNum txtUsuario;
        private Presentacion_575_AV.ucAlfaNum txtContra;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblContra;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblSinConexion;
        private System.Windows.Forms.Label lblPie;
    }
}
