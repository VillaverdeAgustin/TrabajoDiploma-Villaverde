-- =============================================================================
-- ConectAR S.R.L. - Trabajo de Diploma - Villaverde, Agustin (575_AV)
-- 05 - Esquema de negocio, fase 1: catalogos, articulo y bobina (RFN1)
--
-- Cubre los catalogos del dominio, el articulo y el circuito de bobina, que es
-- la particularidad del rubro: los articulos fraccionables se venden por metro
-- lineal a partir de una caja o rollo y hay que llevar el saldo remanente de
-- cada bobina abierta. Por eso cantidad_disponible es decimal y no int.
--
-- Convencion de nombres: columnas en minuscula con guion bajo. El termino del
-- dominio es Articulo, no Producto.
--
-- estado_articulo sostiene el bloqueo: cuando se reserva un articulo faltante
-- queda bloqueado hasta que la compra se concrete. Se persiste 'SinStock' sin
-- espacio para que el enum EstadoArticulo_575_AV mapee directo; el texto que ve
-- el usuario sale de la tabla Traduccion (T05).
--
-- Toda tabla de negocio lleva dvh y se controla por DVV (T08), y toda alta,
-- modificacion y baja se registra en bitacora desde la BLL (T06).
--
-- El usuario es un servicio, no una entidad de negocio: ninguna tabla de aca
-- lleva FK a Usuarios.
--
-- Orden de ejecucion: 01 -> 02 -> 03 -> 04 -> 05 -> 06
-- =============================================================================

USE [ConectAR_DB];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- -----------------------------------------------------------------------------
-- Catalogos
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
CREATE TABLE [dbo].[Categoria](
    [id_categoria] [int] IDENTITY(1,1) NOT NULL,
    [nombre]       [varchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [activo]       [bit]         NOT NULL CONSTRAINT [DF_Categoria_activo] DEFAULT (1),
    [dvh]          [varchar](64) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([id_categoria] ASC),
    CONSTRAINT [UQ_Categoria_nombre] UNIQUE ([nombre])
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.Marca', 'U') IS NULL
CREATE TABLE [dbo].[Marca](
    [id_marca] [int] IDENTITY(1,1) NOT NULL,
    [nombre]   [varchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [activo]   [bit]         NOT NULL CONSTRAINT [DF_Marca_activo] DEFAULT (1),
    [dvh]      [varchar](64) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([id_marca] ASC),
    CONSTRAINT [UQ_Marca_nombre] UNIQUE ([nombre])
) ON [PRIMARY];
GO

-- fraccionable habilita el circuito de bobina: "Unidad" en 0, "Metro" en 1.
IF OBJECT_ID('dbo.UnidadMedida', 'U') IS NULL
CREATE TABLE [dbo].[UnidadMedida](
    [id_unidad_medida] [int] IDENTITY(1,1) NOT NULL,
    [nombre]           [varchar](30) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [abreviatura]      [varchar](5)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [fraccionable]     [bit]         NOT NULL CONSTRAINT [DF_UnidadMedida_fraccionable] DEFAULT (0),
    [dvh]              [varchar](64) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_UnidadMedida] PRIMARY KEY CLUSTERED ([id_unidad_medida] ASC),
    CONSTRAINT [UQ_UnidadMedida_nombre] UNIQUE ([nombre])
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.MedioPago', 'U') IS NULL
CREATE TABLE [dbo].[MedioPago](
    [id_medio_pago]         [int] IDENTITY(1,1) NOT NULL,
    [nombre]                [varchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [requiere_autorizacion] [bit]         NOT NULL CONSTRAINT [DF_MedioPago_req_aut] DEFAULT (0),
    [activo]                [bit]         NOT NULL CONSTRAINT [DF_MedioPago_activo] DEFAULT (1),
    [dvh]                   [varchar](64) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_MedioPago] PRIMARY KEY CLUSTERED ([id_medio_pago] ASC),
    CONSTRAINT [UQ_MedioPago_nombre] UNIQUE ([nombre])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Articulo. cantidad_disponible y punto_reposicion son decimal, no int: un
-- articulo fraccionable tiene N bobinas y su disponibilidad es la suma de los
-- saldos.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Articulo', 'U') IS NULL
CREATE TABLE [dbo].[Articulo](
    [id_articulo]         [int] IDENTITY(1,1) NOT NULL,
    [codigo]              [varchar](25)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [descripcion]         [varchar](200) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [id_categoria]        [int]          NOT NULL,
    [id_marca]            [int]          NOT NULL,
    [id_unidad_medida]    [int]          NOT NULL,
    [precio_unitario]     [decimal](12,2) NOT NULL CONSTRAINT [DF_Articulo_precio] DEFAULT (0),
    [cantidad_disponible] [decimal](12,2) NOT NULL CONSTRAINT [DF_Articulo_cantidad] DEFAULT (0),
    [punto_reposicion]    [decimal](12,2) NOT NULL CONSTRAINT [DF_Articulo_pto_rep] DEFAULT (0),
    [deposito_ubicacion]  [varchar](30)  COLLATE Modern_Spanish_CI_AS NULL,
    [estado_articulo]     [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                          CONSTRAINT [DF_Articulo_estado] DEFAULT ('Disponible'),
    [activo]              [bit]          NOT NULL CONSTRAINT [DF_Articulo_activo] DEFAULT (1),
    [dvh]                 [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Articulo] PRIMARY KEY CLUSTERED ([id_articulo] ASC),
    CONSTRAINT [UQ_Articulo_codigo] UNIQUE ([codigo]),
    CONSTRAINT [CK_Articulo_estado] CHECK ([estado_articulo] IN ('Disponible', 'Bloqueado', 'SinStock'))
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Bobina. identificador es la etiqueta fisica de la bobina o caja.
-- estado: Cerrada (sin abrir) / Abierta (con saldo) / Agotada (saldo 0).
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Bobina', 'U') IS NULL
CREATE TABLE [dbo].[Bobina](
    [id_bobina]      [int] IDENTITY(1,1) NOT NULL,
    [id_articulo]    [int]          NOT NULL,
    [identificador]  [varchar](25)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [medida_inicial] [decimal](12,2) NOT NULL,
    [saldo_bobina]   [decimal](12,2) NOT NULL,
    [fecha_apertura] [datetime]     NULL,
    [estado]         [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                     CONSTRAINT [DF_Bobina_estado] DEFAULT ('Cerrada'),
    [dvh]            [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Bobina] PRIMARY KEY CLUSTERED ([id_bobina] ASC),
    CONSTRAINT [UQ_Bobina_identificador] UNIQUE ([identificador]),
    CONSTRAINT [CK_Bobina_estado] CHECK ([estado] IN ('Cerrada', 'Abierta', 'Agotada')),
    CONSTRAINT [CK_Bobina_saldo] CHECK ([saldo_bobina] >= 0 AND [saldo_bobina] <= [medida_inicial])
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Cliente.
--
-- correo_electronico es el campo del requisito T03.2: se guarda cifrado con
-- AES-256 reversible (Servicios.EncriptadorReversible_575_AV) y se descifra solo
-- al mostrarlo, con el boton protegido por clave. Por eso el campo es ancho: el
-- texto cifrado en Base64 ocupa bastante mas que el correo original.
--
-- El dni entra al circuito de caja: es por donde se busca al cliente (CUN-004),
-- y el alta va como extend cuando no existe.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Cliente', 'U') IS NULL
CREATE TABLE [dbo].[Cliente](
    [id_cliente]          [int] IDENTITY(1,1) NOT NULL,
    [razon_social]        [varchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    [nombre]              [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [apellido]            [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [dni]                 [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [cuit]                [varchar](13)  COLLATE Modern_Spanish_CI_AS NULL,
    [condicion_iva]       [varchar](30)  COLLATE Modern_Spanish_CI_AS NOT NULL
                          CONSTRAINT [DF_Cliente_condicion_iva] DEFAULT ('ConsumidorFinal'),
    [direccion]           [varchar](150) COLLATE Modern_Spanish_CI_AS NULL,
    [telefono]            [varchar](30)  COLLATE Modern_Spanish_CI_AS NULL,
    [correo_electronico]  [varchar](256) COLLATE Modern_Spanish_CI_AS NULL,
    [fecha_actualizacion] [datetime]     NOT NULL CONSTRAINT [DF_Cliente_fecha] DEFAULT (GETDATE()),
    [activo]              [bit]          NOT NULL CONSTRAINT [DF_Cliente_activo] DEFAULT (1),
    [dvh]                 [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED ([id_cliente] ASC),
    CONSTRAINT [UQ_Cliente_dni] UNIQUE ([dni]),
    CONSTRAINT [CK_Cliente_condicion_iva] CHECK ([condicion_iva] IN
        ('ConsumidorFinal', 'ResponsableInscripto', 'Monotributista', 'Exento'))
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Carrito y su detalle (CUN-001).
--
-- El carrito lo arma el vendedor en el salon, antes de que el cajero
-- identifique al cliente: por eso guarda nombre_cliente como texto y todavia
-- no hay id_cliente.
--
-- id_bobina en el detalle queda en nulo salvo que el articulo se haya
-- fraccionado de una bobina concreta.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Carrito', 'U') IS NULL
CREATE TABLE [dbo].[Carrito](
    [id_carrito]       [int] IDENTITY(1,1) NOT NULL,
    [nro_carrito]      [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [usuario_vendedor] [varchar](50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [nombre_cliente]   [varchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    [fecha_apertura]   [datetime]     NOT NULL,
    [estado]           [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                       CONSTRAINT [DF_Carrito_estado] DEFAULT ('Abierto'),
    [precio_total]     [decimal](12,2) NOT NULL CONSTRAINT [DF_Carrito_total] DEFAULT (0),
    [activo]           [bit]          NOT NULL CONSTRAINT [DF_Carrito_activo] DEFAULT (1),
    [dvh]              [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Carrito] PRIMARY KEY CLUSTERED ([id_carrito] ASC),
    CONSTRAINT [UQ_Carrito_nro] UNIQUE ([nro_carrito]),
    CONSTRAINT [CK_Carrito_estado] CHECK ([estado] IN
        ('Abierto', 'Confirmado', 'Facturado', 'Anulado'))
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.DetalleCarrito', 'U') IS NULL
CREATE TABLE [dbo].[DetalleCarrito](
    [id_detalle_carrito] [int] IDENTITY(1,1) NOT NULL,
    [id_carrito]         [int]          NOT NULL,
    [id_articulo]        [int]          NOT NULL,
    [cantidad]           [decimal](12,2) NOT NULL,
    [id_unidad_medida]   [int]          NOT NULL,
    [precio_unitario]    [decimal](12,2) NOT NULL,
    [subtotal]           [decimal](12,2) NOT NULL,
    [id_bobina]          [int]          NULL,
    [dvh]                [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_DetalleCarrito] PRIMARY KEY CLUSTERED ([id_detalle_carrito] ASC),
    CONSTRAINT [CK_DetalleCarrito_cantidad] CHECK ([cantidad] > 0)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Reserva del faltante (CUN-001).
--
-- Compromete un articulo que no alcanza a cubrir el pedido e informa el plazo
-- estimado de reposicion. Al registrarse, el articulo queda BLOQUEADO hasta que
-- la compra se concrete, y el movimiento de reposicion pasa al RFN2.
--
-- id_cliente queda en nulo cuando el cliente todavia no esta registrado: el
-- alta entra recien en la caja.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Reserva', 'U') IS NULL
CREATE TABLE [dbo].[Reserva](
    [id_reserva]     [int] IDENTITY(1,1) NOT NULL,
    [id_articulo]    [int]          NOT NULL,
    [id_cliente]     [int]          NULL,
    [nombre_cliente] [varchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    [cantidad]       [decimal](12,2) NOT NULL,
    [fecha_reserva]  [datetime]     NOT NULL,
    [plazo_entrega]  [int]          NOT NULL,
    [estado]         [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                     CONSTRAINT [DF_Reserva_estado] DEFAULT ('Pendiente'),
    [dvh]            [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Reserva] PRIMARY KEY CLUSTERED ([id_reserva] ASC),
    CONSTRAINT [CK_Reserva_cantidad] CHECK ([cantidad] > 0),
    CONSTRAINT [CK_Reserva_estado] CHECK ([estado] IN
        ('Pendiente', 'EnCompra', 'Cumplida', 'Anulada'))
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Orden de pago (CUN-002).
--
-- Es el documento que el vendedor emite a partir del carrito confirmado y con
-- el que el cliente se presenta en la caja. Todavia no es un comprobante: no
-- descuenta existencias ni tiene valor fiscal.
--
-- Copia nombre_cliente y precio_total del carrito en lugar de leerlos por la
-- FK, porque la orden tiene que conservar lo que se emitio aunque despues
-- cambie el precio del articulo.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.OrdenPago', 'U') IS NULL
CREATE TABLE [dbo].[OrdenPago](
    [id_orden_pago]  [int] IDENTITY(1,1) NOT NULL,
    [nro_orden_pago] [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [id_carrito]     [int]          NOT NULL,
    [nombre_cliente] [varchar](100) COLLATE Modern_Spanish_CI_AS NULL,
    [fecha_emision]  [datetime]     NOT NULL,
    [precio_total]   [decimal](12,2) NOT NULL,
    [estado]         [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                     CONSTRAINT [DF_OrdenPago_estado] DEFAULT ('Pendiente'),
    [activo]         [bit]          NOT NULL CONSTRAINT [DF_OrdenPago_activo] DEFAULT (1),
    [dvh]            [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_OrdenPago] PRIMARY KEY CLUSTERED ([id_orden_pago] ASC),
    CONSTRAINT [UQ_OrdenPago_nro] UNIQUE ([nro_orden_pago]),
    CONSTRAINT [UQ_OrdenPago_carrito] UNIQUE ([id_carrito]),
    CONSTRAINT [CK_OrdenPago_estado] CHECK ([estado] IN ('Pendiente', 'Pagada', 'Anulada'))
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Comprobante, su detalle y el pago (CUN-004).
--
-- El comprobante se emite como COMPROBANTE NO VALIDO COMO FACTURA: no hay
-- integracion con ARCA, ni CAE, ni servicios web del organismo.
--
-- tipo_comprobante sale de la condicion del cliente frente al IVA: 'A' para
-- responsable inscripto, con el impuesto discriminado, y 'B' para el resto.
--
-- El detalle copia cantidad y precio en lugar de leerlos del carrito: el
-- comprobante tiene que conservar lo que se emitio.
--
-- estado impide que un mismo comprobante se use dos veces para retirar
-- mercaderia: pasa a Entregado cuando el deposito lo despacha.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Comprobante', 'U') IS NULL
CREATE TABLE [dbo].[Comprobante](
    [id_comprobante]   [int] IDENTITY(1,1) NOT NULL,
    [nro_comprobante]  [varchar](20)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [tipo_comprobante] [varchar](5)   COLLATE Modern_Spanish_CI_AS NOT NULL,
    [id_orden_pago]    [int]          NOT NULL,
    [id_cliente]       [int]          NOT NULL,
    [fecha_emision]    [datetime]     NOT NULL,
    [neto]             [decimal](12,2) NOT NULL,
    [iva]              [decimal](12,2) NOT NULL,
    [precio_total]     [decimal](12,2) NOT NULL,
    [estado]           [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                       CONSTRAINT [DF_Comprobante_estado] DEFAULT ('Emitido'),
    [dvh]              [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Comprobante] PRIMARY KEY CLUSTERED ([id_comprobante] ASC),
    CONSTRAINT [UQ_Comprobante_nro] UNIQUE ([nro_comprobante]),
    CONSTRAINT [UQ_Comprobante_orden] UNIQUE ([id_orden_pago]),
    CONSTRAINT [CK_Comprobante_tipo] CHECK ([tipo_comprobante] IN ('A', 'B')),
    CONSTRAINT [CK_Comprobante_estado] CHECK ([estado] IN ('Emitido', 'Entregado'))
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.DetalleComprobante', 'U') IS NULL
CREATE TABLE [dbo].[DetalleComprobante](
    [id_detalle_comprobante] [int] IDENTITY(1,1) NOT NULL,
    [id_comprobante]         [int]          NOT NULL,
    [id_articulo]            [int]          NOT NULL,
    [cantidad]               [decimal](12,2) NOT NULL,
    [id_unidad_medida]       [int]          NOT NULL,
    [precio_unitario]        [decimal](12,2) NOT NULL,
    [dvh]                    [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_DetalleComprobante] PRIMARY KEY CLUSTERED ([id_detalle_comprobante] ASC),
    CONSTRAINT [CK_DetalleComprobante_cantidad] CHECK ([cantidad] > 0)
) ON [PRIMARY];
GO

-- El codigo de autorizacion lo devuelve el autorizador de pagos, que en el
-- alcance de la entrega simula la respuesta de la entidad: no hay integracion
-- bancaria posible.
IF OBJECT_ID('dbo.Pago', 'U') IS NULL
CREATE TABLE [dbo].[Pago](
    [id_pago]             [int] IDENTITY(1,1) NOT NULL,
    [id_comprobante]      [int]          NOT NULL,
    [id_medio_pago]       [int]          NOT NULL,
    [monto]               [decimal](12,2) NOT NULL,
    [fecha_hora]          [datetime]     NOT NULL,
    [codigo_autorizacion] [varchar](30)  COLLATE Modern_Spanish_CI_AS NULL,
    [estado]              [varchar](15)  COLLATE Modern_Spanish_CI_AS NOT NULL
                          CONSTRAINT [DF_Pago_estado] DEFAULT ('Acreditado'),
    [dvh]                 [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Pago] PRIMARY KEY CLUSTERED ([id_pago] ASC),
    CONSTRAINT [CK_Pago_estado] CHECK ([estado] IN ('Acreditado', 'Rechazado'))
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Entrega en deposito y su detalle (CUN-005).
--
-- El cliente presenta el comprobante y el encargado despacha la mercaderia.
-- No mueve existencias: el stock ya bajo al emitirse el comprobante.
--
-- id_comprobante es UNIQUE: un comprobante se despacha una sola vez. Si lo que
-- se entrega es menos que lo facturado, la entrega queda marcada como Parcial,
-- pero el comprobante se consume igual.
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.Entrega', 'U') IS NULL
CREATE TABLE [dbo].[Entrega](
    [id_entrega]       [int] IDENTITY(1,1) NOT NULL,
    [id_comprobante]   [int]         NOT NULL,
    [usuario_deposito] [varchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [fecha_entrega]    [datetime]    NOT NULL,
    [estado]           [varchar](15) COLLATE Modern_Spanish_CI_AS NOT NULL
                       CONSTRAINT [DF_Entrega_estado] DEFAULT ('Total'),
    [dvh]              [varchar](64) COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_Entrega] PRIMARY KEY CLUSTERED ([id_entrega] ASC),
    CONSTRAINT [UQ_Entrega_comprobante] UNIQUE ([id_comprobante]),
    CONSTRAINT [CK_Entrega_estado] CHECK ([estado] IN ('Total', 'Parcial'))
) ON [PRIMARY];
GO

IF OBJECT_ID('dbo.DetalleEntrega', 'U') IS NULL
CREATE TABLE [dbo].[DetalleEntrega](
    [id_detalle_entrega] [int] IDENTITY(1,1) NOT NULL,
    [id_entrega]         [int]          NOT NULL,
    [id_articulo]        [int]          NOT NULL,
    [cantidad_entregada] [decimal](12,2) NOT NULL,
    [id_unidad_medida]   [int]          NOT NULL,
    [dvh]                [varchar](64)  COLLATE Modern_Spanish_CI_AS NULL,
    CONSTRAINT [PK_DetalleEntrega] PRIMARY KEY CLUSTERED ([id_detalle_entrega] ASC),
    CONSTRAINT [CK_DetalleEntrega_cantidad] CHECK ([cantidad_entregada] > 0)
) ON [PRIMARY];
GO

-- -----------------------------------------------------------------------------
-- Integridad referencial
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.FK_Articulo_Categoria', 'F') IS NULL
    ALTER TABLE [dbo].[Articulo] WITH CHECK ADD CONSTRAINT [FK_Articulo_Categoria]
        FOREIGN KEY([id_categoria]) REFERENCES [dbo].[Categoria]([id_categoria]);
GO

IF OBJECT_ID('dbo.FK_Articulo_Marca', 'F') IS NULL
    ALTER TABLE [dbo].[Articulo] WITH CHECK ADD CONSTRAINT [FK_Articulo_Marca]
        FOREIGN KEY([id_marca]) REFERENCES [dbo].[Marca]([id_marca]);
GO

IF OBJECT_ID('dbo.FK_Articulo_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[Articulo] WITH CHECK ADD CONSTRAINT [FK_Articulo_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

IF OBJECT_ID('dbo.FK_Bobina_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[Bobina] WITH CHECK ADD CONSTRAINT [FK_Bobina_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleCarrito_Carrito', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleCarrito] WITH CHECK ADD CONSTRAINT [FK_DetalleCarrito_Carrito]
        FOREIGN KEY([id_carrito]) REFERENCES [dbo].[Carrito]([id_carrito]);
GO

IF OBJECT_ID('dbo.FK_DetalleCarrito_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleCarrito] WITH CHECK ADD CONSTRAINT [FK_DetalleCarrito_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleCarrito_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleCarrito] WITH CHECK ADD CONSTRAINT [FK_DetalleCarrito_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

IF OBJECT_ID('dbo.FK_DetalleCarrito_Bobina', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleCarrito] WITH CHECK ADD CONSTRAINT [FK_DetalleCarrito_Bobina]
        FOREIGN KEY([id_bobina]) REFERENCES [dbo].[Bobina]([id_bobina]);
GO

IF OBJECT_ID('dbo.FK_Reserva_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[Reserva] WITH CHECK ADD CONSTRAINT [FK_Reserva_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_Reserva_Cliente', 'F') IS NULL
    ALTER TABLE [dbo].[Reserva] WITH CHECK ADD CONSTRAINT [FK_Reserva_Cliente]
        FOREIGN KEY([id_cliente]) REFERENCES [dbo].[Cliente]([id_cliente]);
GO

IF OBJECT_ID('dbo.FK_OrdenPago_Carrito', 'F') IS NULL
    ALTER TABLE [dbo].[OrdenPago] WITH CHECK ADD CONSTRAINT [FK_OrdenPago_Carrito]
        FOREIGN KEY([id_carrito]) REFERENCES [dbo].[Carrito]([id_carrito]);
GO

IF OBJECT_ID('dbo.FK_Comprobante_OrdenPago', 'F') IS NULL
    ALTER TABLE [dbo].[Comprobante] WITH CHECK ADD CONSTRAINT [FK_Comprobante_OrdenPago]
        FOREIGN KEY([id_orden_pago]) REFERENCES [dbo].[OrdenPago]([id_orden_pago]);
GO

IF OBJECT_ID('dbo.FK_Comprobante_Cliente', 'F') IS NULL
    ALTER TABLE [dbo].[Comprobante] WITH CHECK ADD CONSTRAINT [FK_Comprobante_Cliente]
        FOREIGN KEY([id_cliente]) REFERENCES [dbo].[Cliente]([id_cliente]);
GO

IF OBJECT_ID('dbo.FK_DetalleComprobante_Comprobante', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleComprobante] WITH CHECK ADD CONSTRAINT [FK_DetalleComprobante_Comprobante]
        FOREIGN KEY([id_comprobante]) REFERENCES [dbo].[Comprobante]([id_comprobante]);
GO

IF OBJECT_ID('dbo.FK_DetalleComprobante_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleComprobante] WITH CHECK ADD CONSTRAINT [FK_DetalleComprobante_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleComprobante_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleComprobante] WITH CHECK ADD CONSTRAINT [FK_DetalleComprobante_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

IF OBJECT_ID('dbo.FK_Pago_Comprobante', 'F') IS NULL
    ALTER TABLE [dbo].[Pago] WITH CHECK ADD CONSTRAINT [FK_Pago_Comprobante]
        FOREIGN KEY([id_comprobante]) REFERENCES [dbo].[Comprobante]([id_comprobante]);
GO

IF OBJECT_ID('dbo.FK_Pago_MedioPago', 'F') IS NULL
    ALTER TABLE [dbo].[Pago] WITH CHECK ADD CONSTRAINT [FK_Pago_MedioPago]
        FOREIGN KEY([id_medio_pago]) REFERENCES [dbo].[MedioPago]([id_medio_pago]);
GO

IF OBJECT_ID('dbo.FK_Entrega_Comprobante', 'F') IS NULL
    ALTER TABLE [dbo].[Entrega] WITH CHECK ADD CONSTRAINT [FK_Entrega_Comprobante]
        FOREIGN KEY([id_comprobante]) REFERENCES [dbo].[Comprobante]([id_comprobante]);
GO

IF OBJECT_ID('dbo.FK_DetalleEntrega_Entrega', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleEntrega] WITH CHECK ADD CONSTRAINT [FK_DetalleEntrega_Entrega]
        FOREIGN KEY([id_entrega]) REFERENCES [dbo].[Entrega]([id_entrega]);
GO

IF OBJECT_ID('dbo.FK_DetalleEntrega_Articulo', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleEntrega] WITH CHECK ADD CONSTRAINT [FK_DetalleEntrega_Articulo]
        FOREIGN KEY([id_articulo]) REFERENCES [dbo].[Articulo]([id_articulo]);
GO

IF OBJECT_ID('dbo.FK_DetalleEntrega_UnidadMedida', 'F') IS NULL
    ALTER TABLE [dbo].[DetalleEntrega] WITH CHECK ADD CONSTRAINT [FK_DetalleEntrega_UnidadMedida]
        FOREIGN KEY([id_unidad_medida]) REFERENCES [dbo].[UnidadMedida]([id_unidad_medida]);
GO

-- -----------------------------------------------------------------------------
-- Datos minimos de catalogo. Idempotente: solo inserta lo que falta.
-- El dvh queda en NULL y se calcula desde Recalcular Digitos (CUS-012).
-- -----------------------------------------------------------------------------
INSERT INTO [dbo].[UnidadMedida] ([nombre], [abreviatura], [fraccionable])
SELECT v.[nombre], v.[abreviatura], v.[fraccionable]
FROM (VALUES
    ('Unidad',       'u',   CONVERT(BIT, 0)),
    ('Metro lineal', 'm',   CONVERT(BIT, 1)),
    ('Caja 305 m',   'cj',  CONVERT(BIT, 1)),
    ('Rollo',        'rll', CONVERT(BIT, 1))
) AS v([nombre], [abreviatura], [fraccionable])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UnidadMedida] u WHERE u.[nombre] = v.[nombre]);
GO

INSERT INTO [dbo].[Categoria] ([nombre])
SELECT v.[nombre]
FROM (VALUES
    ('Cable de red'), ('Fibra optica'), ('Conectividad'),
    ('Racks y gabinetes'), ('Canalizacion'), ('Herramientas'), ('Instrumental')
) AS v([nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Categoria] c WHERE c.[nombre] = v.[nombre]);
GO

INSERT INTO [dbo].[Marca] ([nombre])
SELECT v.[nombre]
FROM (VALUES
    ('Furukawa'), ('Panduit'), ('Belden'), ('Commscope'),
    ('Legrand'), ('Fluke Networks'), ('Nexxt Solutions'), ('Generico')
) AS v([nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Marca] m WHERE m.[nombre] = v.[nombre]);
GO

INSERT INTO [dbo].[MedioPago] ([nombre], [requiere_autorizacion])
SELECT v.[nombre], v.[requiere_autorizacion]
FROM (VALUES
    ('Efectivo',         CONVERT(BIT, 0)),
    ('Transferencia',    CONVERT(BIT, 0)),
    ('Tarjeta debito',   CONVERT(BIT, 1)),
    ('Tarjeta credito',  CONVERT(BIT, 1)),
    ('Cuenta corriente', CONVERT(BIT, 1))
) AS v([nombre], [requiere_autorizacion])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[MedioPago] m WHERE m.[nombre] = v.[nombre]);
GO

-- =============================================================================
-- T08 - Digito verificador de los datos sembrados
--
-- Mismo criterio que 03_datos_seguridad.sql: los valores van como literales, ya
-- calculados, para que una base recien instalada pase el control del login sin
-- pedir Recalcular Digitos.
--
-- Cada dvh es el SHA-256 de ObtenerCamposDV() de su entidad, con los campos
-- unidos por '|' y los bit renderizados como 'True'/'False'. Por ejemplo, para
-- Categoria:  id_categoria|nombre|activo
-- El DVV de cada tabla es el SHA-256 de sus dvh unidos por '|'; en una tabla
-- vacia equivale al SHA-256 de la cadena vacia.
--
-- Se aplican solo donde dvh quedo en NULL, es decir sobre la siembra recien
-- insertada: nunca pisan un valor ya recalculado en una base en uso.
--
-- Si se agrega, quita o edita una fila sembrada, estos literales dejan de valer
-- y hay que regenerarlos con Recalcular Digitos (CUS-012).
-- =============================================================================

UPDATE c SET c.[dvh] = v.[dvh]
FROM [dbo].[Categoria] c
JOIN (VALUES
    (N'Cable de red',      N'fa47febf477a058742d90e8814ef94e7a17cf69f31954f307e1db63b5c3a0c2d'),
    (N'Fibra optica',      N'105e7770ee69bb738f7f68a704bfe00189c00a126dda9eb80211b8c12cdabb1f'),
    (N'Conectividad',      N'26d4d572886ba6a686417c268082590aaef78cfa34d9ca4006c956a7f0488128'),
    (N'Racks y gabinetes', N'8ec58f1da728ee4f9e2928f717cd7d4f235c60d8b7627ad633f6ed94a984650e'),
    (N'Canalizacion',      N'b019ced40f15a22314984e3fa8f5298d908c63fe61ac1fad55b06946a8935264'),
    (N'Herramientas',      N'b1bf69e3aeabc54d6c84864d42993c39be23c8ec8fe8032f8fa046790916abe9'),
    (N'Instrumental',      N'4f89ef794d827d158efc1659cdb90a3c11469636efc94e8605b6544a0db18bf1')
) AS v([nombre], [dvh]) ON v.[nombre] = c.[nombre]
WHERE c.[dvh] IS NULL;
GO

UPDATE m SET m.[dvh] = v.[dvh]
FROM [dbo].[Marca] m
JOIN (VALUES
    (N'Furukawa',        N'0cb2b2c95a0db3acd227faa79c889926016bbdf1a390812e525c39e3a9338d96'),
    (N'Panduit',         N'7ae083fadaeee2615439b3ea8fefb2f622107a8e5dab6c9b1be1f30cbcb3dab0'),
    (N'Belden',          N'eba8eeeddd034691a44a585e01f2b8cc57b55e59af0d57a31de874f13bf2f2b0'),
    (N'Commscope',       N'1e233b0f7ed738998ab79e9bfb22484e250356ff3c161ae6189821214e677281'),
    (N'Legrand',         N'ef38fb9fe40761b3ed1a6cd9c906b992aebd2cc280e0298526389650de19992d'),
    (N'Fluke Networks',  N'75aea80f37a1c78b9941945ee174c576d4055e734a4912aabb167aa21e06789c'),
    (N'Nexxt Solutions', N'b9cd68bdb98afa3ff2b316538d7961d8c846868a63738ab6d9e7fbd832969805'),
    (N'Generico',        N'7a365972ffa440a3f11d117332d313a4da1f12e7cb56939e7ffa36e460c55500')
) AS v([nombre], [dvh]) ON v.[nombre] = m.[nombre]
WHERE m.[dvh] IS NULL;
GO

UPDATE u SET u.[dvh] = v.[dvh]
FROM [dbo].[UnidadMedida] u
JOIN (VALUES
    (N'Unidad',       N'eef8c31d863c95d4467dbddecdff64f43150e66c1d88c30593a33c804c1e04e3'),
    (N'Metro lineal', N'3136c01de6a6f4f0849574f0ad8f2cff3f40fff298858f48384be3c208c4edb6'),
    (N'Caja 305 m',   N'e9f6085c35c0cff59efafe3b3d2d37034f2ececca614f7d364689a5a707d4f42'),
    (N'Rollo',        N'dfd76153dfe08c8cc46a98ae92dcf4cade21407f1e996b000b798087a28bbace')
) AS v([nombre], [dvh]) ON v.[nombre] = u.[nombre]
WHERE u.[dvh] IS NULL;
GO

UPDATE p SET p.[dvh] = v.[dvh]
FROM [dbo].[MedioPago] p
JOIN (VALUES
    (N'Efectivo',         N'57ecfa0f8ee6fa986de4b8ce0f8fead6d068dbd30832a34d36c5bdb707fb749e'),
    (N'Transferencia',    N'5e176fc694f8fb2c11acef9b133aed4f20d402b12fd82da5b40e08592502626e'),
    (N'Tarjeta debito',   N'4b037d7c4f703f8727b6e2dccb18d376f40b595c1f3718568bb48b24e7d91839'),
    (N'Tarjeta credito',  N'8bd1cb4bddb8a1b44ebfc7d754f9349f1d7d15a67d238cd2dcf900ae59cb884f'),
    (N'Cuenta corriente', N'a4f6c9b24e5e6b31e8018419086cb0d4762ae742c107c1e12a5683880af390a6')
) AS v([nombre], [dvh]) ON v.[nombre] = p.[nombre]
WHERE p.[dvh] IS NULL;
GO

-- DVV por tabla. Articulo y Bobina nacen vacias: su DVV es el SHA-256 de la
-- cadena vacia, y cambia con la primera alta.
MERGE INTO [dbo].[DigitoVertical] AS destino
USING (VALUES
    (N'Categoria',    N'207a428cc6b7ada718b491d1512f9ee918e58c2138a643deab2128c50ca25399'),
    (N'Marca',        N'120d700fc9ada6a380b73fcbf6eef9a09bfbbe102130c8751f09787e78022991'),
    (N'UnidadMedida', N'848c086fa90c4ecff360aa82f58555e23b33033703e9a85f897690ea962d0307'),
    (N'MedioPago',    N'd150abf193df2dd07656bbe6a8932eeba28785a3a3c0741b89e630cf4ded78e5'),
    (N'Articulo',     N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Bobina',       N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Cliente',        N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Carrito',        N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'DetalleCarrito', N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Reserva',        N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'OrdenPago',          N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Comprobante',        N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'DetalleComprobante', N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Pago',               N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'Entrega',            N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'),
    (N'DetalleEntrega',     N'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')
) AS origen ([Tabla], [DVV])
    ON destino.[Tabla] = origen.[Tabla]
WHEN NOT MATCHED THEN
    INSERT ([Tabla], [DVV]) VALUES (origen.[Tabla], origen.[DVV]);
GO

PRINT '05 - Esquema de negocio fase 1 (catalogos, articulo y bobina) creado.';
GO
