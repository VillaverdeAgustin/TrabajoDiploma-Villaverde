namespace Presentacion_575_AV
{
    partial class frmGestionPerfiles
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
            this.labelTitulo = new System.Windows.Forms.Label();
            this.pPerfiles = new System.Windows.Forms.Panel();
            this.gbDatos = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.rBRol = new System.Windows.Forms.RadioButton();
            this.rBFamilia = new System.Windows.Forms.RadioButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.labelNombre = new System.Windows.Forms.Label();
            this.comboFamilias = new System.Windows.Forms.ComboBox();
            this.labelRol = new System.Windows.Forms.Label();
            this.gbPermisos = new System.Windows.Forms.GroupBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.gbArbol = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.pPerfiles.SuspendLayout();
            this.gbDatos.SuspendLayout();
            this.gbPermisos.SuspendLayout();
            this.gbArbol.SuspendLayout();
            this.SuspendLayout();
            //
            // labelTitulo
            //
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Verdana", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.labelTitulo.Location = new System.Drawing.Point(330, 15);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(330, 24);
            this.labelTitulo.TabIndex = 0;
            this.labelTitulo.Text = "GESTIÓN DE PERFILES";
            //
            // pPerfiles
            //
            this.pPerfiles.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pPerfiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pPerfiles.Controls.Add(this.gbDatos);
            this.pPerfiles.Controls.Add(this.gbPermisos);
            this.pPerfiles.Controls.Add(this.gbArbol);
            this.pPerfiles.Location = new System.Drawing.Point(12, 55);
            this.pPerfiles.Name = "pPerfiles";
            this.pPerfiles.Size = new System.Drawing.Size(976, 550);
            this.pPerfiles.TabIndex = 1;
            //
            // gbDatos
            //
            this.gbDatos.Controls.Add(this.button1);
            this.gbDatos.Controls.Add(this.btnEliminar);
            this.gbDatos.Controls.Add(this.button7);
            this.gbDatos.Controls.Add(this.button5);
            this.gbDatos.Controls.Add(this.rBRol);
            this.gbDatos.Controls.Add(this.rBFamilia);
            this.gbDatos.Controls.Add(this.textBox2);
            this.gbDatos.Controls.Add(this.labelNombre);
            this.gbDatos.Controls.Add(this.comboFamilias);
            this.gbDatos.Controls.Add(this.labelRol);
            this.gbDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.gbDatos.Location = new System.Drawing.Point(16, 16);
            this.gbDatos.Name = "gbDatos";
            this.gbDatos.Size = new System.Drawing.Size(280, 300);
            this.gbDatos.TabIndex = 0;
            this.gbDatos.TabStop = false;
            this.gbDatos.Text = "Datos del Perfil";
            //
            // button1
            //
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.button1.Location = new System.Drawing.Point(14, 258);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(250, 32);
            this.button1.TabIndex = 9;
            this.button1.Text = "Cancelar";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnEliminar.Location = new System.Drawing.Point(14, 218);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(250, 32);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // button7
            //
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.button7.Location = new System.Drawing.Point(144, 178);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(120, 32);
            this.button7.TabIndex = 7;
            this.button7.Text = "Guardar";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            //
            // button5
            //
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.button5.Location = new System.Drawing.Point(14, 178);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(120, 32);
            this.button5.TabIndex = 6;
            this.button5.Text = "Crear";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            //
            // rBRol
            //
            this.rBRol.AutoSize = true;
            this.rBRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.rBRol.Location = new System.Drawing.Point(140, 148);
            this.rBRol.Name = "rBRol";
            this.rBRol.Size = new System.Drawing.Size(43, 21);
            this.rBRol.TabIndex = 5;
            this.rBRol.TabStop = true;
            this.rBRol.Text = "Rol";
            this.rBRol.UseVisualStyleBackColor = true;
            //
            // rBFamilia
            //
            this.rBFamilia.AutoSize = true;
            this.rBFamilia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.rBFamilia.Location = new System.Drawing.Point(14, 148);
            this.rBFamilia.Name = "rBFamilia";
            this.rBFamilia.Size = new System.Drawing.Size(65, 21);
            this.rBFamilia.TabIndex = 4;
            this.rBFamilia.TabStop = true;
            this.rBFamilia.Text = "Familia";
            this.rBFamilia.UseVisualStyleBackColor = true;
            //
            // textBox2
            //
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.textBox2.Location = new System.Drawing.Point(14, 108);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(250, 25);
            this.textBox2.TabIndex = 3;
            //
            // labelNombre
            //
            this.labelNombre.AutoSize = true;
            this.labelNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.labelNombre.Location = new System.Drawing.Point(12, 88);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(64, 15);
            this.labelNombre.TabIndex = 2;
            this.labelNombre.Text = "Nombre:";
            //
            // comboFamilias
            //
            this.comboFamilias.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.comboFamilias.Location = new System.Drawing.Point(14, 48);
            this.comboFamilias.Name = "comboFamilias";
            this.comboFamilias.Size = new System.Drawing.Size(250, 25);
            this.comboFamilias.TabIndex = 1;
            this.comboFamilias.SelectedIndexChanged += new System.EventHandler(this.comboFamilias_SelectedIndexChanged);
            //
            // labelRol
            //
            this.labelRol.AutoSize = true;
            this.labelRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.labelRol.Location = new System.Drawing.Point(12, 28);
            this.labelRol.Name = "labelRol";
            this.labelRol.Size = new System.Drawing.Size(101, 15);
            this.labelRol.TabIndex = 0;
            this.labelRol.Text = "Rol / Familia:";
            //
            // gbPermisos
            //
            this.gbPermisos.Controls.Add(this.checkedListBox1);
            this.gbPermisos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.gbPermisos.Location = new System.Drawing.Point(310, 16);
            this.gbPermisos.Name = "gbPermisos";
            this.gbPermisos.Size = new System.Drawing.Size(310, 504);
            this.gbPermisos.TabIndex = 1;
            this.gbPermisos.TabStop = false;
            this.gbPermisos.Text = "Permisos y Familias Disponibles";
            //
            // checkedListBox1
            //
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.checkedListBox1.Location = new System.Drawing.Point(12, 25);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(286, 465);
            this.checkedListBox1.TabIndex = 0;
            //
            // gbArbol
            //
            this.gbArbol.Controls.Add(this.treeView1);
            this.gbArbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.gbArbol.Location = new System.Drawing.Point(634, 16);
            this.gbArbol.Name = "gbArbol";
            this.gbArbol.Size = new System.Drawing.Size(326, 504);
            this.gbArbol.TabIndex = 2;
            this.gbArbol.TabStop = false;
            this.gbArbol.Text = "Árbol de Jerarquía de Permisos";
            //
            // treeView1
            //
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(12, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(302, 465);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            //
            // frmGestionPerfiles
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1000, 630);
            this.Controls.Add(this.pPerfiles);
            this.Controls.Add(this.labelTitulo);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGestionPerfiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestión de Perfiles";
            this.Load += new System.EventHandler(this.frmGestionPerfiles_Load);
            this.pPerfiles.ResumeLayout(false);
            this.gbDatos.ResumeLayout(false);
            this.gbDatos.PerformLayout();
            this.gbPermisos.ResumeLayout(false);
            this.gbArbol.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Panel pPerfiles;
        private System.Windows.Forms.GroupBox gbDatos;
        private System.Windows.Forms.Label labelRol;
        private System.Windows.Forms.ComboBox comboFamilias;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.RadioButton rBFamilia;
        private System.Windows.Forms.RadioButton rBRol;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gbPermisos;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.GroupBox gbArbol;
        private System.Windows.Forms.TreeView treeView1;
    }
}
