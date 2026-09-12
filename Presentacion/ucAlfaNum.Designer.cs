namespace Presentacion_575_AV
{
    partial class ucAlfaNum
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTexto = new System.Windows.Forms.TextBox();
            this.ptxtTexto = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            //
            // txtTexto
            //
            this.txtTexto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTexto.BackColor = System.Drawing.Color.White;
            this.txtTexto.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTexto.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.txtTexto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(59)))), ((int)(((byte)(92)))));
            this.txtTexto.Location = new System.Drawing.Point(2, 10);
            this.txtTexto.Name = "txtTexto";
            this.txtTexto.Size = new System.Drawing.Size(284, 26);
            this.txtTexto.TabIndex = 1;
            this.txtTexto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContra_KeyPress);
            this.txtTexto.Enter += new System.EventHandler(this.txtTexto_Enter);
            this.txtTexto.Leave += new System.EventHandler(this.txtTexto_Leave);
            //
            // ptxtTexto
            //
            this.ptxtTexto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ptxtTexto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.ptxtTexto.Location = new System.Drawing.Point(0, 36);
            this.ptxtTexto.Name = "ptxtTexto";
            this.ptxtTexto.Size = new System.Drawing.Size(288, 2);
            this.ptxtTexto.TabIndex = 20;
            //
            // ucAlfaNum
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ptxtTexto);
            this.Controls.Add(this.txtTexto);
            this.Name = "ucAlfaNum";
            this.Size = new System.Drawing.Size(288, 44);
            this.Load += new System.EventHandler(this.ucUsuario_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtTexto;
        private System.Windows.Forms.Panel ptxtTexto;
    }
}
