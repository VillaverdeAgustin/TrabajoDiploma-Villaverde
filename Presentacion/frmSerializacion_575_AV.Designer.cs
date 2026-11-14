namespace Presentacion_575_AV
{
    partial class frmSerializacion_575_AV
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
            this.tabOperacion = new System.Windows.Forms.TabControl();
            this.tpSerializar = new System.Windows.Forms.TabPage();
            this.gbOrdenes = new System.Windows.Forms.GroupBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.colNro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRenglones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSerializar = new System.Windows.Forms.Button();
            this.lblArchivoSerializado = new System.Windows.Forms.Label();
            this.lblHashSerializado = new System.Windows.Forms.Label();
            this.txtHashSerializado = new System.Windows.Forms.TextBox();
            this.gbVerificacion = new System.Windows.Forms.GroupBox();
            this.dgvVerificacion = new System.Windows.Forms.DataGridView();
            this.colVerificacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResultado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetalle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpDeserializar = new System.Windows.Forms.TabPage();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.lblArchivoDeserializado = new System.Windows.Forms.Label();
            this.lblHashDeserializado = new System.Windows.Forms.Label();
            this.txtHashDeserializado = new System.Windows.Forms.TextBox();
            this.gbRecuperada = new System.Windows.Forms.GroupBox();
            this.lblOrden = new System.Windows.Forms.Label();
            this.lblOrdenValor = new System.Windows.Forms.Label();
            this.lblProveedorRecuperado = new System.Windows.Forms.Label();
            this.lblProveedorValor = new System.Windows.Forms.Label();
            this.lblEmision = new System.Windows.Forms.Label();
            this.lblEmisionValor = new System.Windows.Forms.Label();
            this.lblEstadoRecuperado = new System.Windows.Forms.Label();
            this.lblEstadoValor = new System.Windows.Forms.Label();
            this.lblTotalRecuperado = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.dgvRenglones = new System.Windows.Forms.DataGridView();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubtotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbVerificacionBase = new System.Windows.Forms.GroupBox();
            this.dgvVerificacionBase = new System.Windows.Forms.DataGridView();
            this.colVerificacionBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResultadoBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetalleBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.sfdXml = new System.Windows.Forms.SaveFileDialog();
            this.ofdXml = new System.Windows.Forms.OpenFileDialog();
            this.tabOperacion.SuspendLayout();
            this.tpSerializar.SuspendLayout();
            this.gbOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.gbVerificacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacion)).BeginInit();
            this.tpDeserializar.SuspendLayout();
            this.gbRecuperada.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRenglones)).BeginInit();
            this.gbVerificacionBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacionBase)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Serialización de órdenes de compra (XML)";
            //
            // tabOperacion
            //
            this.tabOperacion.Controls.Add(this.tpSerializar);
            this.tabOperacion.Controls.Add(this.tpDeserializar);
            this.tabOperacion.Location = new System.Drawing.Point(16, 44);
            this.tabOperacion.Name = "tabOperacion";
            this.tabOperacion.Size = new System.Drawing.Size(1088, 622);
            this.tabOperacion.TabIndex = 1;
            //
            // tpSerializar
            //
            this.tpSerializar.Controls.Add(this.gbOrdenes);
            this.tpSerializar.Controls.Add(this.btnSerializar);
            this.tpSerializar.Controls.Add(this.lblArchivoSerializado);
            this.tpSerializar.Controls.Add(this.lblHashSerializado);
            this.tpSerializar.Controls.Add(this.txtHashSerializado);
            this.tpSerializar.Controls.Add(this.gbVerificacion);
            this.tpSerializar.Padding = new System.Windows.Forms.Padding(3);
            this.tpSerializar.UseVisualStyleBackColor = true;
            this.tpSerializar.Name = "tpSerializar";
            this.tpSerializar.Location = new System.Drawing.Point(4, 26);
            this.tpSerializar.Size = new System.Drawing.Size(1080, 592);
            this.tpSerializar.TabIndex = 2;
            this.tpSerializar.Text = "Serializar";
            //
            // gbOrdenes
            //
            this.gbOrdenes.Controls.Add(this.dgvOrdenes);
            this.gbOrdenes.Location = new System.Drawing.Point(8, 6);
            this.gbOrdenes.Name = "gbOrdenes";
            this.gbOrdenes.Size = new System.Drawing.Size(1062, 184);
            this.gbOrdenes.TabIndex = 3;
            this.gbOrdenes.Text = "Órdenes de compra emitidas";
            //
            // dgvOrdenes
            //
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.AllowUserToResizeRows = false;
            this.dgvOrdenes.AutoGenerateColumns = false;
            this.dgvOrdenes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNro,
            this.colFecha,
            this.colProveedor,
            this.colEstado,
            this.colRenglones,
            this.colTotal});
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.Location = new System.Drawing.Point(12, 24);
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.Size = new System.Drawing.Size(1038, 150);
            this.dgvOrdenes.TabIndex = 4;
            //
            // colNro
            //
            this.colNro.DataPropertyName = "nro_orden_compra";
            this.colNro.HeaderText = "Nro. orden";
            this.colNro.Name = "colNro";
            this.colNro.ReadOnly = true;
            this.colNro.Width = 110;
            //
            // colFecha
            //
            this.colFecha.DataPropertyName = "fecha_emision";
            this.colFecha.HeaderText = "Fecha";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            this.colFecha.Width = 140;
            //
            // colProveedor
            //
            this.colProveedor.DataPropertyName = "razon_social";
            this.colProveedor.HeaderText = "Proveedor";
            this.colProveedor.Name = "colProveedor";
            this.colProveedor.ReadOnly = true;
            this.colProveedor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // colEstado
            //
            this.colEstado.DataPropertyName = "estado";
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            this.colEstado.Width = 200;
            //
            // colRenglones
            //
            this.colRenglones.DataPropertyName = "cantidad_renglones";
            this.colRenglones.HeaderText = "Renglones";
            this.colRenglones.Name = "colRenglones";
            this.colRenglones.ReadOnly = true;
            this.colRenglones.Width = 90;
            //
            // colTotal
            //
            this.colTotal.DataPropertyName = "total";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.Width = 140;
            //
            // btnSerializar
            //
            this.btnSerializar.Location = new System.Drawing.Point(8, 198);
            this.btnSerializar.Name = "btnSerializar";
            this.btnSerializar.Size = new System.Drawing.Size(196, 34);
            this.btnSerializar.TabIndex = 5;
            this.btnSerializar.Text = "Serializar a XML...";
            this.btnSerializar.Click += new System.EventHandler(this.btnSerializar_Click);
            //
            // lblArchivoSerializado
            //
            this.lblArchivoSerializado.Location = new System.Drawing.Point(216, 196);
            this.lblArchivoSerializado.Name = "lblArchivoSerializado";
            this.lblArchivoSerializado.Size = new System.Drawing.Size(854, 18);
            this.lblArchivoSerializado.TabIndex = 6;
            this.lblArchivoSerializado.Text = "";
            //
            // lblHashSerializado
            //
            this.lblHashSerializado.AutoSize = true;
            this.lblHashSerializado.Location = new System.Drawing.Point(216, 218);
            this.lblHashSerializado.Name = "lblHashSerializado";
            this.lblHashSerializado.TabIndex = 7;
            this.lblHashSerializado.Text = "Hash SHA-256:";
            //
            // txtHashSerializado
            //
            this.txtHashSerializado.ReadOnly = true;
            this.txtHashSerializado.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtHashSerializado.Location = new System.Drawing.Point(330, 215);
            this.txtHashSerializado.Name = "txtHashSerializado";
            this.txtHashSerializado.Size = new System.Drawing.Size(740, 23);
            this.txtHashSerializado.TabIndex = 8;
            //
            // gbVerificacion
            //
            this.gbVerificacion.Controls.Add(this.dgvVerificacion);
            this.gbVerificacion.Location = new System.Drawing.Point(8, 244);
            this.gbVerificacion.Name = "gbVerificacion";
            this.gbVerificacion.Size = new System.Drawing.Size(1062, 340);
            this.gbVerificacion.TabIndex = 9;
            this.gbVerificacion.Text = "Verificación del archivo generado";
            //
            // dgvVerificacion
            //
            this.dgvVerificacion.AllowUserToAddRows = false;
            this.dgvVerificacion.AllowUserToDeleteRows = false;
            this.dgvVerificacion.AllowUserToResizeRows = false;
            this.dgvVerificacion.AutoGenerateColumns = false;
            this.dgvVerificacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVerificacion,
            this.colResultado,
            this.colDetalle});
            this.dgvVerificacion.MultiSelect = false;
            this.dgvVerificacion.ReadOnly = true;
            this.dgvVerificacion.Location = new System.Drawing.Point(12, 24);
            this.dgvVerificacion.Name = "dgvVerificacion";
            this.dgvVerificacion.Size = new System.Drawing.Size(1038, 304);
            this.dgvVerificacion.TabIndex = 10;
            //
            // colVerificacion
            //
            this.colVerificacion.DataPropertyName = "clave";
            this.colVerificacion.HeaderText = "Verificación";
            this.colVerificacion.Name = "colVerificacion";
            this.colVerificacion.ReadOnly = true;
            this.colVerificacion.Width = 430;
            //
            // colResultado
            //
            this.colResultado.DataPropertyName = "correcto";
            this.colResultado.HeaderText = "Resultado";
            this.colResultado.Name = "colResultado";
            this.colResultado.ReadOnly = true;
            this.colResultado.Width = 120;
            //
            // colDetalle
            //
            this.colDetalle.DataPropertyName = "detalle";
            this.colDetalle.HeaderText = "Detalle";
            this.colDetalle.Name = "colDetalle";
            this.colDetalle.ReadOnly = true;
            this.colDetalle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // tpDeserializar
            //
            this.tpDeserializar.Controls.Add(this.btnAbrir);
            this.tpDeserializar.Controls.Add(this.lblArchivoDeserializado);
            this.tpDeserializar.Controls.Add(this.lblHashDeserializado);
            this.tpDeserializar.Controls.Add(this.txtHashDeserializado);
            this.tpDeserializar.Controls.Add(this.gbRecuperada);
            this.tpDeserializar.Controls.Add(this.gbVerificacionBase);
            this.tpDeserializar.Padding = new System.Windows.Forms.Padding(3);
            this.tpDeserializar.UseVisualStyleBackColor = true;
            this.tpDeserializar.Name = "tpDeserializar";
            this.tpDeserializar.Location = new System.Drawing.Point(4, 26);
            this.tpDeserializar.Size = new System.Drawing.Size(1080, 592);
            this.tpDeserializar.TabIndex = 11;
            this.tpDeserializar.Text = "Deserializar";
            //
            // btnAbrir
            //
            this.btnAbrir.Location = new System.Drawing.Point(8, 6);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(196, 34);
            this.btnAbrir.TabIndex = 12;
            this.btnAbrir.Text = "Abrir archivo XML...";
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            //
            // lblArchivoDeserializado
            //
            this.lblArchivoDeserializado.Location = new System.Drawing.Point(216, 4);
            this.lblArchivoDeserializado.Name = "lblArchivoDeserializado";
            this.lblArchivoDeserializado.Size = new System.Drawing.Size(854, 18);
            this.lblArchivoDeserializado.TabIndex = 13;
            this.lblArchivoDeserializado.Text = "";
            //
            // lblHashDeserializado
            //
            this.lblHashDeserializado.AutoSize = true;
            this.lblHashDeserializado.Location = new System.Drawing.Point(216, 26);
            this.lblHashDeserializado.Name = "lblHashDeserializado";
            this.lblHashDeserializado.TabIndex = 14;
            this.lblHashDeserializado.Text = "Hash SHA-256:";
            //
            // txtHashDeserializado
            //
            this.txtHashDeserializado.ReadOnly = true;
            this.txtHashDeserializado.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtHashDeserializado.Location = new System.Drawing.Point(330, 23);
            this.txtHashDeserializado.Name = "txtHashDeserializado";
            this.txtHashDeserializado.Size = new System.Drawing.Size(740, 23);
            this.txtHashDeserializado.TabIndex = 15;
            //
            // gbRecuperada
            //
            this.gbRecuperada.Controls.Add(this.lblOrden);
            this.gbRecuperada.Controls.Add(this.lblOrdenValor);
            this.gbRecuperada.Controls.Add(this.lblProveedorRecuperado);
            this.gbRecuperada.Controls.Add(this.lblProveedorValor);
            this.gbRecuperada.Controls.Add(this.lblEmision);
            this.gbRecuperada.Controls.Add(this.lblEmisionValor);
            this.gbRecuperada.Controls.Add(this.lblEstadoRecuperado);
            this.gbRecuperada.Controls.Add(this.lblEstadoValor);
            this.gbRecuperada.Controls.Add(this.lblTotalRecuperado);
            this.gbRecuperada.Controls.Add(this.lblTotalValor);
            this.gbRecuperada.Controls.Add(this.dgvRenglones);
            this.gbRecuperada.Location = new System.Drawing.Point(8, 50);
            this.gbRecuperada.Name = "gbRecuperada";
            this.gbRecuperada.Size = new System.Drawing.Size(1062, 226);
            this.gbRecuperada.TabIndex = 16;
            this.gbRecuperada.Text = "Orden recuperada del archivo";
            //
            // lblOrden
            //
            this.lblOrden.AutoSize = true;
            this.lblOrden.Location = new System.Drawing.Point(12, 24);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.TabIndex = 17;
            this.lblOrden.Text = "Orden:";
            //
            // lblOrdenValor
            //
            this.lblOrdenValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblOrdenValor.Location = new System.Drawing.Point(96, 24);
            this.lblOrdenValor.Name = "lblOrdenValor";
            this.lblOrdenValor.Size = new System.Drawing.Size(120, 18);
            this.lblOrdenValor.TabIndex = 18;
            this.lblOrdenValor.Text = "";
            //
            // lblProveedorRecuperado
            //
            this.lblProveedorRecuperado.AutoSize = true;
            this.lblProveedorRecuperado.Location = new System.Drawing.Point(230, 24);
            this.lblProveedorRecuperado.Name = "lblProveedorRecuperado";
            this.lblProveedorRecuperado.TabIndex = 19;
            this.lblProveedorRecuperado.Text = "Proveedor:";
            //
            // lblProveedorValor
            //
            this.lblProveedorValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblProveedorValor.Location = new System.Drawing.Point(314, 24);
            this.lblProveedorValor.Name = "lblProveedorValor";
            this.lblProveedorValor.Size = new System.Drawing.Size(330, 18);
            this.lblProveedorValor.TabIndex = 20;
            this.lblProveedorValor.Text = "";
            //
            // lblEmision
            //
            this.lblEmision.AutoSize = true;
            this.lblEmision.Location = new System.Drawing.Point(660, 24);
            this.lblEmision.Name = "lblEmision";
            this.lblEmision.TabIndex = 21;
            this.lblEmision.Text = "Emisión:";
            //
            // lblEmisionValor
            //
            this.lblEmisionValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblEmisionValor.Location = new System.Drawing.Point(730, 24);
            this.lblEmisionValor.Name = "lblEmisionValor";
            this.lblEmisionValor.Size = new System.Drawing.Size(150, 18);
            this.lblEmisionValor.TabIndex = 22;
            this.lblEmisionValor.Text = "";
            //
            // lblEstadoRecuperado
            //
            this.lblEstadoRecuperado.AutoSize = true;
            this.lblEstadoRecuperado.Location = new System.Drawing.Point(12, 46);
            this.lblEstadoRecuperado.Name = "lblEstadoRecuperado";
            this.lblEstadoRecuperado.TabIndex = 23;
            this.lblEstadoRecuperado.Text = "Estado:";
            //
            // lblEstadoValor
            //
            this.lblEstadoValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblEstadoValor.Location = new System.Drawing.Point(96, 46);
            this.lblEstadoValor.Name = "lblEstadoValor";
            this.lblEstadoValor.Size = new System.Drawing.Size(200, 18);
            this.lblEstadoValor.TabIndex = 24;
            this.lblEstadoValor.Text = "";
            //
            // lblTotalRecuperado
            //
            this.lblTotalRecuperado.AutoSize = true;
            this.lblTotalRecuperado.Location = new System.Drawing.Point(660, 46);
            this.lblTotalRecuperado.Name = "lblTotalRecuperado";
            this.lblTotalRecuperado.TabIndex = 25;
            this.lblTotalRecuperado.Text = "Total:";
            //
            // lblTotalValor
            //
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotalValor.Location = new System.Drawing.Point(730, 46);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(150, 18);
            this.lblTotalValor.TabIndex = 26;
            this.lblTotalValor.Text = "";
            //
            // dgvRenglones
            //
            this.dgvRenglones.AllowUserToAddRows = false;
            this.dgvRenglones.AllowUserToDeleteRows = false;
            this.dgvRenglones.AllowUserToResizeRows = false;
            this.dgvRenglones.AutoGenerateColumns = false;
            this.dgvRenglones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodigo,
            this.colDescripcion,
            this.colCantidad,
            this.colUnidad,
            this.colPrecio,
            this.colSubtotal});
            this.dgvRenglones.MultiSelect = false;
            this.dgvRenglones.ReadOnly = true;
            this.dgvRenglones.Location = new System.Drawing.Point(12, 70);
            this.dgvRenglones.Name = "dgvRenglones";
            this.dgvRenglones.Size = new System.Drawing.Size(1038, 146);
            this.dgvRenglones.TabIndex = 27;
            //
            // colCodigo
            //
            this.colCodigo.DataPropertyName = "codigo_articulo";
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.ReadOnly = true;
            this.colCodigo.Width = 130;
            //
            // colDescripcion
            //
            this.colDescripcion.DataPropertyName = "descripcion_articulo";
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.Name = "colDescripcion";
            this.colDescripcion.ReadOnly = true;
            this.colDescripcion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // colCantidad
            //
            this.colCantidad.DataPropertyName = "cantidad";
            this.colCantidad.HeaderText = "Cantidad";
            this.colCantidad.Name = "colCantidad";
            this.colCantidad.ReadOnly = true;
            this.colCantidad.Width = 110;
            //
            // colUnidad
            //
            this.colUnidad.DataPropertyName = "abreviatura_unidad";
            this.colUnidad.HeaderText = "Unidad";
            this.colUnidad.Name = "colUnidad";
            this.colUnidad.ReadOnly = true;
            this.colUnidad.Width = 80;
            //
            // colPrecio
            //
            this.colPrecio.DataPropertyName = "precio_unitario";
            this.colPrecio.HeaderText = "Precio unitario";
            this.colPrecio.Name = "colPrecio";
            this.colPrecio.ReadOnly = true;
            this.colPrecio.Width = 140;
            //
            // colSubtotal
            //
            this.colSubtotal.DataPropertyName = "subtotal";
            this.colSubtotal.HeaderText = "Subtotal";
            this.colSubtotal.Name = "colSubtotal";
            this.colSubtotal.ReadOnly = true;
            this.colSubtotal.Width = 140;
            //
            // gbVerificacionBase
            //
            this.gbVerificacionBase.Controls.Add(this.dgvVerificacionBase);
            this.gbVerificacionBase.Location = new System.Drawing.Point(8, 280);
            this.gbVerificacionBase.Name = "gbVerificacionBase";
            this.gbVerificacionBase.Size = new System.Drawing.Size(1062, 306);
            this.gbVerificacionBase.TabIndex = 28;
            this.gbVerificacionBase.Text = "Verificación contra la base de datos";
            //
            // dgvVerificacionBase
            //
            this.dgvVerificacionBase.AllowUserToAddRows = false;
            this.dgvVerificacionBase.AllowUserToDeleteRows = false;
            this.dgvVerificacionBase.AllowUserToResizeRows = false;
            this.dgvVerificacionBase.AutoGenerateColumns = false;
            this.dgvVerificacionBase.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVerificacionBase,
            this.colResultadoBase,
            this.colDetalleBase});
            this.dgvVerificacionBase.MultiSelect = false;
            this.dgvVerificacionBase.ReadOnly = true;
            this.dgvVerificacionBase.Location = new System.Drawing.Point(12, 24);
            this.dgvVerificacionBase.Name = "dgvVerificacionBase";
            this.dgvVerificacionBase.Size = new System.Drawing.Size(1038, 274);
            this.dgvVerificacionBase.TabIndex = 29;
            //
            // colVerificacionBase
            //
            this.colVerificacionBase.DataPropertyName = "clave";
            this.colVerificacionBase.HeaderText = "Verificación";
            this.colVerificacionBase.Name = "colVerificacionBase";
            this.colVerificacionBase.ReadOnly = true;
            this.colVerificacionBase.Width = 430;
            //
            // colResultadoBase
            //
            this.colResultadoBase.DataPropertyName = "correcto";
            this.colResultadoBase.HeaderText = "Resultado";
            this.colResultadoBase.Name = "colResultadoBase";
            this.colResultadoBase.ReadOnly = true;
            this.colResultadoBase.Width = 120;
            //
            // colDetalleBase
            //
            this.colDetalleBase.DataPropertyName = "detalle";
            this.colDetalleBase.HeaderText = "Detalle";
            this.colDetalleBase.Name = "colDetalleBase";
            this.colDetalleBase.ReadOnly = true;
            this.colDetalleBase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //
            // lblMensaje
            //
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje.Location = new System.Drawing.Point(16, 674);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(920, 34);
            this.lblMensaje.TabIndex = 30;
            this.lblMensaje.Text = "";
            //
            // btnSalir
            //
            this.btnSalir.Location = new System.Drawing.Point(952, 674);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(152, 34);
            this.btnSalir.TabIndex = 31;
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            //
            // frmSerializacion_575_AV
            //
            this.ClientSize = new System.Drawing.Size(1120, 718);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.tabOperacion);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnSalir);
            this.Name = "frmSerializacion_575_AV";
            this.Text = "Serialización de órdenes de compra (XML)";
            this.Load += new System.EventHandler(this.frmSerializacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacionBase)).EndInit();
            this.gbVerificacionBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRenglones)).EndInit();
            this.gbRecuperada.ResumeLayout(false);
            this.gbRecuperada.PerformLayout();
            this.tpDeserializar.ResumeLayout(false);
            this.tpDeserializar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacion)).EndInit();
            this.gbVerificacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbOrdenes.ResumeLayout(false);
            this.tpSerializar.ResumeLayout(false);
            this.tpSerializar.PerformLayout();
            this.tabOperacion.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TabControl tabOperacion;
        private System.Windows.Forms.TabPage tpSerializar;
        private System.Windows.Forms.GroupBox gbOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNro;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRenglones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.Button btnSerializar;
        private System.Windows.Forms.Label lblArchivoSerializado;
        private System.Windows.Forms.Label lblHashSerializado;
        private System.Windows.Forms.TextBox txtHashSerializado;
        private System.Windows.Forms.GroupBox gbVerificacion;
        private System.Windows.Forms.DataGridView dgvVerificacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVerificacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResultado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetalle;
        private System.Windows.Forms.TabPage tpDeserializar;
        private System.Windows.Forms.Button btnAbrir;
        private System.Windows.Forms.Label lblArchivoDeserializado;
        private System.Windows.Forms.Label lblHashDeserializado;
        private System.Windows.Forms.TextBox txtHashDeserializado;
        private System.Windows.Forms.GroupBox gbRecuperada;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.Label lblOrdenValor;
        private System.Windows.Forms.Label lblProveedorRecuperado;
        private System.Windows.Forms.Label lblProveedorValor;
        private System.Windows.Forms.Label lblEmision;
        private System.Windows.Forms.Label lblEmisionValor;
        private System.Windows.Forms.Label lblEstadoRecuperado;
        private System.Windows.Forms.Label lblEstadoValor;
        private System.Windows.Forms.Label lblTotalRecuperado;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.DataGridView dgvRenglones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubtotal;
        private System.Windows.Forms.GroupBox gbVerificacionBase;
        private System.Windows.Forms.DataGridView dgvVerificacionBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVerificacionBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResultadoBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetalleBase;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.SaveFileDialog sfdXml;
        private System.Windows.Forms.OpenFileDialog ofdXml;
    }
}
