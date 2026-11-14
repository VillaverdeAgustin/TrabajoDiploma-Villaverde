namespace Presentacion_575_AV
{
    partial class frmRecalcular
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstTablas = new System.Windows.Forms.ListBox();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.btnRecalcular = new System.Windows.Forms.Button();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblRestaurar = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(496, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Integridad de la base de datos";
            //
            // label1
            //
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(14, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(494, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Se detectaron diferencias en las siguientes tablas:";
            //
            // lstTablas
            //
            this.lstTablas.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lstTablas.IntegralHeight = false;
            this.lstTablas.ItemHeight = 17;
            this.lstTablas.Location = new System.Drawing.Point(14, 78);
            this.lstTablas.Name = "lstTablas";
            this.lstTablas.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstTablas.Size = new System.Drawing.Size(494, 120);
            this.lstTablas.TabIndex = 2;
            //
            // lblAyuda
            //
            this.lblAyuda.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblAyuda.Location = new System.Drawing.Point(14, 204);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(494, 36);
            this.lblAyuda.TabIndex = 3;
            this.lblAyuda.Text = "Recalcular vuelve a calcular los dígitos verificadores de todas las tablas.";
            //
            // btnRecalcular
            //
            this.btnRecalcular.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnRecalcular.Location = new System.Drawing.Point(14, 250);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new System.Drawing.Size(160, 36);
            this.btnRecalcular.TabIndex = 4;
            this.btnRecalcular.Text = "Recalcular";
            //
            // btnRestaurar
            //
            this.btnRestaurar.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.btnRestaurar.Location = new System.Drawing.Point(181, 250);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(160, 36);
            this.btnRestaurar.TabIndex = 5;
            this.btnRestaurar.Text = "Restaurar backup";
            //
            // btnSalir
            //
            this.btnSalir.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnSalir.Location = new System.Drawing.Point(348, 250);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(160, 36);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            //
            // lblRestaurar
            //
            this.lblRestaurar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblRestaurar.Location = new System.Drawing.Point(14, 294);
            this.lblRestaurar.Name = "lblRestaurar";
            this.lblRestaurar.Size = new System.Drawing.Size(494, 20);
            this.lblRestaurar.TabIndex = 7;
            this.lblRestaurar.Text = "Restaurar backup se habilita con el módulo de Backup y Restore (T07).";
            //
            // frmRecalcular
            //
            this.AcceptButton = this.btnRecalcular;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnSalir;
            this.ClientSize = new System.Drawing.Size(522, 324);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstTablas);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.btnRecalcular);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblRestaurar);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecalcular";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Integridad de la base de datos";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstTablas;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Button btnRecalcular;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblRestaurar;
    }
}
