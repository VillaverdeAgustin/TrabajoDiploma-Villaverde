namespace Presentacion_575_AV
{
    partial class frmUsuario
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUsuario));
            this.pUsuario = new System.Windows.Forms.Panel();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnDesbloquear = new System.Windows.Forms.Button();
            this.btnElimUs = new System.Windows.Forms.Button();
            this.btnModUs = new System.Windows.Forms.Button();
            this.btnCrearUs = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.g = new System.Windows.Forms.GroupBox();
            this.txtMensaje = new System.Windows.Forms.TextBox();
            this.gbDatos = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.txtTel = new System.Windows.Forms.TextBox();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.txtUsu = new System.Windows.Forms.TextBox();
            this.txtApe = new System.Windows.Forms.TextBox();
            this.txtNom = new System.Windows.Forms.TextBox();
            this.txtDoc = new System.Windows.Forms.TextBox();
            this.chkBloqueado = new System.Windows.Forms.CheckBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.pUsuario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.panel1.SuspendLayout();
            this.g.SuspendLayout();
            this.gbDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // pUsuario
            // 
            this.pUsuario.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pUsuario.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pUsuario.Controls.Add(this.btnSalir);
            this.pUsuario.Controls.Add(this.btnCancelar);
            this.pUsuario.Controls.Add(this.btnGuardar);
            this.pUsuario.Controls.Add(this.btnDesbloquear);
            this.pUsuario.Controls.Add(this.btnElimUs);
            this.pUsuario.Controls.Add(this.btnModUs);
            this.pUsuario.Controls.Add(this.btnCrearUs);
            this.pUsuario.Controls.Add(this.label2);
            this.pUsuario.Controls.Add(this.label1);
            this.pUsuario.Controls.Add(this.dgvUsuarios);
            this.pUsuario.Location = new System.Drawing.Point(12, 11);
            this.pUsuario.Name = "pUsuario";
            this.pUsuario.Size = new System.Drawing.Size(1004, 426);
            this.pUsuario.TabIndex = 17;
            // 
            // btnSalir
            // 
            this.btnSalir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSalir.Location = new System.Drawing.Point(867, 338);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(118, 29);
            this.btnSalir.TabIndex = 9;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancelar.Location = new System.Drawing.Point(867, 302);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(118, 29);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnGuardar.Location = new System.Drawing.Point(867, 267);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(118, 29);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnDesbloquear
            // 
            this.btnDesbloquear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDesbloquear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnDesbloquear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDesbloquear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesbloquear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDesbloquear.Location = new System.Drawing.Point(867, 232);
            this.btnDesbloquear.Name = "btnDesbloquear";
            this.btnDesbloquear.Size = new System.Drawing.Size(118, 29);
            this.btnDesbloquear.TabIndex = 6;
            this.btnDesbloquear.Text = "Desbloquear";
            this.btnDesbloquear.UseVisualStyleBackColor = false;
            this.btnDesbloquear.Click += new System.EventHandler(this.btnDesbloquear_Click);
            // 
            // btnElimUs
            // 
            this.btnElimUs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnElimUs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnElimUs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnElimUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElimUs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnElimUs.Location = new System.Drawing.Point(867, 197);
            this.btnElimUs.Name = "btnElimUs";
            this.btnElimUs.Size = new System.Drawing.Size(118, 29);
            this.btnElimUs.TabIndex = 5;
            this.btnElimUs.Text = "Eliminar Usuario";
            this.btnElimUs.UseVisualStyleBackColor = false;
            this.btnElimUs.Click += new System.EventHandler(this.btnElimUs_Click);
            // 
            // btnModUs
            // 
            this.btnModUs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModUs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnModUs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnModUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModUs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnModUs.Location = new System.Drawing.Point(867, 162);
            this.btnModUs.Name = "btnModUs";
            this.btnModUs.Size = new System.Drawing.Size(118, 29);
            this.btnModUs.TabIndex = 4;
            this.btnModUs.Text = "Modificar Usuario";
            this.btnModUs.UseVisualStyleBackColor = false;
            this.btnModUs.Click += new System.EventHandler(this.btnModUs_Click);
            // 
            // btnCrearUs
            // 
            this.btnCrearUs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrearUs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCrearUs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCrearUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearUs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCrearUs.Location = new System.Drawing.Point(867, 127);
            this.btnCrearUs.Name = "btnCrearUs";
            this.btnCrearUs.Size = new System.Drawing.Size(118, 29);
            this.btnCrearUs.TabIndex = 3;
            this.btnCrearUs.Text = "Crear Usuario";
            this.btnCrearUs.UseVisualStyleBackColor = false;
            this.btnCrearUs.Click += new System.EventHandler(this.btnCrearUs_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(888, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Opciones";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label1.Location = new System.Drawing.Point(336, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "GESTIÓN DE USUARIOS";
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.AllowUserToResizeColumns = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvUsuarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvUsuarios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvUsuarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Firebrick;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Firebrick;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUsuarios.ColumnHeadersHeight = 40;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvUsuarios.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvUsuarios.EnableHeadersVisualStyles = false;
            this.dgvUsuarios.Location = new System.Drawing.Point(15, 83);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.RowHeadersWidth = 51;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9F);
            this.dgvUsuarios.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvUsuarios.RowTemplate.Height = 24;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(832, 327);
            this.dgvUsuarios.TabIndex = 0;
            this.dgvUsuarios.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUsuarios_CellClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.g);
            this.panel1.Controls.Add(this.gbDatos);
            this.panel1.Location = new System.Drawing.Point(12, 444);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1004, 206);
            this.panel1.TabIndex = 18;
            // 
            // g
            // 
            this.g.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.g.Controls.Add(this.txtMensaje);
            this.g.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.g.Location = new System.Drawing.Point(662, 15);
            this.g.Name = "g";
            this.g.Size = new System.Drawing.Size(256, 177);
            this.g.TabIndex = 1;
            this.g.TabStop = false;
            this.g.Text = "Mensajes";
            // 
            // txtMensaje
            // 
            this.txtMensaje.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtMensaje.Location = new System.Drawing.Point(6, 22);
            this.txtMensaje.Multiline = true;
            this.txtMensaje.Name = "txtMensaje";
            this.txtMensaje.ReadOnly = true;
            this.txtMensaje.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMensaje.Size = new System.Drawing.Size(245, 149);
            this.txtMensaje.TabIndex = 0;
            // 
            // gbDatos
            // 
            this.gbDatos.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.gbDatos.Controls.Add(this.label10);
            this.gbDatos.Controls.Add(this.label9);
            this.gbDatos.Controls.Add(this.label8);
            this.gbDatos.Controls.Add(this.label7);
            this.gbDatos.Controls.Add(this.label6);
            this.gbDatos.Controls.Add(this.label5);
            this.gbDatos.Controls.Add(this.label4);
            this.gbDatos.Controls.Add(this.label3);
            this.gbDatos.Controls.Add(this.cmbRol);
            this.gbDatos.Controls.Add(this.txtMail);
            this.gbDatos.Controls.Add(this.txtTel);
            this.gbDatos.Controls.Add(this.txtDir);
            this.gbDatos.Controls.Add(this.txtUsu);
            this.gbDatos.Controls.Add(this.txtApe);
            this.gbDatos.Controls.Add(this.txtNom);
            this.gbDatos.Controls.Add(this.txtDoc);
            this.gbDatos.Controls.Add(this.chkBloqueado);
            this.gbDatos.Controls.Add(this.chkActivo);
            this.gbDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.gbDatos.Location = new System.Drawing.Point(86, 15);
            this.gbDatos.Name = "gbDatos";
            this.gbDatos.Size = new System.Drawing.Size(569, 177);
            this.gbDatos.TabIndex = 0;
            this.gbDatos.TabStop = false;
            this.gbDatos.Text = "Datos Usuario";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(306, 151);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 15);
            this.label10.TabIndex = 17;
            this.label10.Text = "E-mail";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(306, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 15);
            this.label9.TabIndex = 16;
            this.label9.Text = "Telefono";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(306, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 15);
            this.label8.TabIndex = 15;
            this.label8.Text = "Direccion";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(306, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Rol";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(28, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Usuario";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(28, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Apellido";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(28, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Nombre";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(28, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "DNI";
            // 
            // cmbRol
            // 
            this.cmbRol.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRol.DropDownWidth = 188;
            this.cmbRol.Enabled = false;
            this.cmbRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cmbRol.FormattingEnabled = true;
            this.cmbRol.Location = new System.Drawing.Point(375, 55);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(188, 24);
            this.cmbRol.TabIndex = 9;
            this.cmbRol.SelectedValueChanged += new System.EventHandler(this.cmbRol_SelectedValueChanged);
            // 
            // txtMail
            // 
            this.txtMail.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtMail.Enabled = false;
            this.txtMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtMail.Location = new System.Drawing.Point(375, 145);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(188, 23);
            this.txtMail.TabIndex = 8;
            this.txtMail.TextChanged += new System.EventHandler(this.txtMail_TextChanged);
            // 
            // txtTel
            // 
            this.txtTel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtTel.Enabled = false;
            this.txtTel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtTel.Location = new System.Drawing.Point(375, 115);
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(188, 23);
            this.txtTel.TabIndex = 7;
            this.txtTel.TextChanged += new System.EventHandler(this.txtTel_TextChanged);
            // 
            // txtDir
            // 
            this.txtDir.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtDir.Enabled = false;
            this.txtDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtDir.Location = new System.Drawing.Point(375, 86);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(188, 23);
            this.txtDir.TabIndex = 6;
            this.txtDir.TextChanged += new System.EventHandler(this.txtDir_TextChanged);
            // 
            // txtUsu
            // 
            this.txtUsu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtUsu.Enabled = false;
            this.txtUsu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtUsu.Location = new System.Drawing.Point(93, 145);
            this.txtUsu.Name = "txtUsu";
            this.txtUsu.Size = new System.Drawing.Size(188, 23);
            this.txtUsu.TabIndex = 5;
            this.txtUsu.TextChanged += new System.EventHandler(this.txtUsu_TextChanged);
            // 
            // txtApe
            // 
            this.txtApe.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtApe.Enabled = false;
            this.txtApe.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtApe.Location = new System.Drawing.Point(93, 114);
            this.txtApe.Name = "txtApe";
            this.txtApe.Size = new System.Drawing.Size(188, 23);
            this.txtApe.TabIndex = 4;
            this.txtApe.TextChanged += new System.EventHandler(this.txtApe_TextChanged);
            // 
            // txtNom
            // 
            this.txtNom.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtNom.Enabled = false;
            this.txtNom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtNom.Location = new System.Drawing.Point(93, 84);
            this.txtNom.Name = "txtNom";
            this.txtNom.Size = new System.Drawing.Size(188, 23);
            this.txtNom.TabIndex = 3;
            this.txtNom.TextChanged += new System.EventHandler(this.txtNom_TextChanged);
            // 
            // txtDoc
            // 
            this.txtDoc.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtDoc.Enabled = false;
            this.txtDoc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtDoc.Location = new System.Drawing.Point(93, 55);
            this.txtDoc.Name = "txtDoc";
            this.txtDoc.Size = new System.Drawing.Size(188, 23);
            this.txtDoc.TabIndex = 2;
            this.txtDoc.TextChanged += new System.EventHandler(this.txtDoc_TextChanged);
            // 
            // chkBloqueado
            // 
            this.chkBloqueado.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkBloqueado.AutoSize = true;
            this.chkBloqueado.Enabled = false;
            this.chkBloqueado.Location = new System.Drawing.Point(167, 24);
            this.chkBloqueado.Margin = new System.Windows.Forms.Padding(2);
            this.chkBloqueado.Name = "chkBloqueado";
            this.chkBloqueado.Size = new System.Drawing.Size(104, 21);
            this.chkBloqueado.TabIndex = 1;
            this.chkBloqueado.Text = "Bloqueado";
            this.chkBloqueado.UseVisualStyleBackColor = true;
            // 
            // chkActivo
            // 
            this.chkActivo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkActivo.AutoSize = true;
            this.chkActivo.Enabled = false;
            this.chkActivo.Location = new System.Drawing.Point(93, 24);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(71, 21);
            this.chkActivo.TabIndex = 0;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            // 
            // frmUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1028, 661);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pUsuario);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(1028, 661);
            this.Name = "frmUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Gestion Usuarios";
            this.Load += new System.EventHandler(this.frmUsuario_Load);
            this.pUsuario.ResumeLayout(false);
            this.pUsuario.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.panel1.ResumeLayout(false);
            this.g.ResumeLayout(false);
            this.g.PerformLayout();
            this.gbDatos.ResumeLayout(false);
            this.gbDatos.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pUsuario;
        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnDesbloquear;
        private System.Windows.Forms.Button btnElimUs;
        private System.Windows.Forms.Button btnModUs;
        private System.Windows.Forms.Button btnCrearUs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox g;
        private System.Windows.Forms.GroupBox gbDatos;
        private System.Windows.Forms.CheckBox chkBloqueado;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.TextBox txtMensaje;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.TextBox txtTel;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.TextBox txtUsu;
        private System.Windows.Forms.TextBox txtApe;
        private System.Windows.Forms.TextBox txtNom;
        private System.Windows.Forms.TextBox txtDoc;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.Label label10;
    }
}