using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Armado del carrito de compras (CUN-001).
    ///
    /// <para>El carrito es un borrador de mostrador: mientras esta abierto se
    /// le agregan y quitan renglones. No descuenta existencias, solo las
    /// verifica; el descuento ocurre al emitir el comprobante.</para>
    ///
    /// <para>Carrito y DetalleCarrito son dos tablas, con su propio dvh y su
    /// propio DVV. Cada escritura pasa por un procedimiento distinto, asi que
    /// las encadena esta clase.</para>
    /// </summary>
    public class CarritoBLL_575_AV
    {
        private const string TABLA = "Carrito";
        private const string TABLA_DETALLE = "DetalleCarrito";

        private MP_Carrito_575_AV mpCarrito = new MP_Carrito_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BobinaBLL_575_AV bobinaBLL = new BobinaBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        /// <summary>Los carritos que el cajero puede tomar, con su detalle cargado.</summary>
        public List<CarritoBE_575_AV> ListarAbiertos()
        {
            List<CarritoBE_575_AV> carritos = mpCarrito.ListarAbiertos();

            foreach (CarritoBE_575_AV carrito in carritos)
            {
                carrito.detalle = ObtenerDetalle(carrito.id_carrito);
            }

            return carritos;
        }

        /// <summary>Renglones del carrito, con los datos de articulo ya resueltos.</summary>
        public List<DetalleCarritoBE_575_AV> ObtenerDetalle(int id_carrito)
        {
            return Completar(mpCarrito.ListarDetalle(id_carrito));
        }

        #endregion

        #region Armado

        /// <summary>Abre un carrito vacio a nombre del vendedor.</summary>
        public CarritoBE_575_AV CrearCarrito(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new InvalidOperationException("No hay un vendedor identificado.");
            }

            CarritoBE_575_AV carrito = new CarritoBE_575_AV
            {
                nro_carrito = mpCarrito.ProximoNumero(),
                usuario_vendedor = usuario,
                fecha_apertura = DateTime.Now,
                estado = EstadoCarrito_575_AV.Abierto,
                precio_total = 0m,
                activo = true
            };

            carrito.id_carrito = mpCarrito.CrearCarrito(carrito);

            // La base fija la fecha con su propio redondeo: se relee antes de
            // calcular el dvh.
            carrito = Releer(carrito.id_carrito);

            PersistirDigito(carrito);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaCarrito);

            return carrito;
        }

        /// <summary>
        /// Agrega un renglon. Verifica la disponibilidad contra el gestor de
        /// existencias y, si el articulo es fraccionable, reserva el metraje
        /// sobre una bobina concreta y lo deja anotado en el renglon.
        /// </summary>
        public CarritoBE_575_AV AgregarDetalle(CarritoBE_575_AV carrito,
                                               DetalleCarritoBE_575_AV detalle,
                                               string usuario)
        {
            Validar(carrito);

            if (detalle == null) { throw new ArgumentNullException("detalle"); }
            if (detalle.cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad tiene que ser mayor a cero.");
            }

            ArticuloBE_575_AV articulo = articuloBLL.ObtenerPorId(detalle.id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException("El articulo no existe.");
            }

            if (!GestorDeExistencias_575_AV.GetInstance
                    .HayDisponibilidad(articulo.id_articulo, detalle.cantidad))
            {
                throw new InvalidOperationException(
                    "No hay disponibilidad de " + articulo.descripcion +
                    " para la cantidad pedida.");
            }

            // El renglon toma el precio y la unidad del articulo, no de la pantalla.
            detalle.id_carrito = carrito.id_carrito;
            detalle.id_unidad_medida = articulo.id_unidad_medida;
            detalle.precio_unitario = articulo.precio_unitario;
            detalle.subtotal = decimal.Round(articulo.precio_unitario * detalle.cantidad, 2);

            if (articulo.fraccionable)
            {
                BobinaBE_575_AV bobina = bobinaBLL.Fraccionar(
                    articulo.id_articulo, detalle.cantidad, usuario);
                detalle.id_bobina = bobina.id_bobina;
            }

            detalle.id_detalle_carrito = mpCarrito.AgregarDetalle(detalle);

            PersistirDigitoDetalle(detalle);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaDetalleCarrito);

            return RecalcularTotal(carrito.id_carrito, usuario);
        }

        /// <summary>
        /// Quita un renglon y devuelve a la bobina el metraje que tenia
        /// comprometido, si lo habia.
        /// </summary>
        public CarritoBE_575_AV QuitarDetalle(CarritoBE_575_AV carrito,
                                              DetalleCarritoBE_575_AV detalle,
                                              string usuario)
        {
            Validar(carrito);

            if (detalle == null || detalle.id_detalle_carrito <= 0)
            {
                throw new InvalidOperationException("El renglon no esta identificado.");
            }

            if (detalle.id_bobina.HasValue)
            {
                bobinaBLL.Devolver(detalle.id_bobina.Value, detalle.cantidad, usuario);
            }

            mpCarrito.QuitarDetalle(detalle.id_detalle_carrito);

            // DetalleCarrito perdio una fila: su DVV cambia.
            verificador.ActualizarDVV(TABLA_DETALLE);
            bitacora.RegistrarBitacora(usuario, TipoAccion.BajaDetalleCarrito);

            return RecalcularTotal(carrito.id_carrito, usuario);
        }

        /// <summary>
        /// Cierra el carrito y lo deja a la espera de la caja. Devuelve el
        /// numero de carrito, que es lo que se le informa al cliente.
        /// </summary>
        public string ConfirmarCarrito(CarritoBE_575_AV carrito, string usuario)
        {
            Validar(carrito);

            if (mpCarrito.ListarDetalle(carrito.id_carrito).Count == 0)
            {
                throw new InvalidOperationException("El carrito no tiene articulos.");
            }

            mpCarrito.ConfirmarCarrito(carrito.id_carrito, carrito.nombre_cliente);

            CarritoBE_575_AV guardado = Releer(carrito.id_carrito);
            PersistirDigito(guardado);
            bitacora.RegistrarBitacora(usuario, TipoAccion.ConfirmacionCarrito);

            return guardado.nro_carrito;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            bool carritosOk = verificador.VerificarIntegridad(
                mpCarrito.ListarCarritos().OrderBy(c => c.id_carrito).ToList(), TABLA);

            bool detallesOk = verificador.VerificarIntegridad(
                mpCarrito.ListarDetalles().OrderBy(d => d.id_detalle_carrito).ToList(), TABLA_DETALLE);

            return carritosOk && detallesOk;
        }

        public void RecalcularDV()
        {
            foreach (CarritoBE_575_AV carrito in mpCarrito.ListarCarritos().OrderBy(c => c.id_carrito))
            {
                carrito.dvh = VerificadorIntegridad.CalcularDVH(carrito);
                mpCarrito.ActualizarDVH(carrito.id_carrito, carrito.dvh);
            }
            verificador.ActualizarDVV(TABLA);

            foreach (DetalleCarritoBE_575_AV detalle in mpCarrito.ListarDetalles()
                        .OrderBy(d => d.id_detalle_carrito))
            {
                detalle.dvh = VerificadorIntegridad.CalcularDVH(detalle);
                mpCarrito.ActualizarDVHDetalle(detalle.id_detalle_carrito, detalle.dvh);
            }
            verificador.ActualizarDVV(TABLA_DETALLE);
        }

        private void PersistirDigito(CarritoBE_575_AV carrito)
        {
            carrito.dvh = VerificadorIntegridad.CalcularDVH(carrito);
            mpCarrito.ActualizarDVH(carrito.id_carrito, carrito.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(DetalleCarritoBE_575_AV detalle)
        {
            detalle.dvh = VerificadorIntegridad.CalcularDVH(detalle);
            mpCarrito.ActualizarDVHDetalle(detalle.id_detalle_carrito, detalle.dvh);
            verificador.ActualizarDVV(TABLA_DETALLE);
        }

        /// <summary>Alias legible del anterior, para no confundir las dos sobrecargas.</summary>
        private void PersistirDigitoDetalle(DetalleCarritoBE_575_AV detalle)
        {
            PersistirDigito(detalle);
        }

        #endregion

        #region Apoyo

        /// <summary>Recalcula el total del carrito desde su detalle y lo persiste.</summary>
        private CarritoBE_575_AV RecalcularTotal(int id_carrito, string usuario)
        {
            decimal total = mpCarrito.ListarDetalle(id_carrito).Sum(d => d.subtotal);
            mpCarrito.ActualizarTotal(id_carrito, total);

            CarritoBE_575_AV carrito = Releer(id_carrito);
            PersistirDigito(carrito);

            carrito.detalle = ObtenerDetalle(id_carrito);
            return carrito;
        }

        private CarritoBE_575_AV Releer(int id_carrito)
        {
            CarritoBE_575_AV carrito = mpCarrito.ListarCarritos()
                .FirstOrDefault(c => c.id_carrito == id_carrito);

            if (carrito == null)
            {
                throw new InvalidOperationException("El carrito no existe.");
            }

            return carrito;
        }

        /// <summary>Completa los datos de articulo que la pantalla necesita mostrar.</summary>
        private List<DetalleCarritoBE_575_AV> Completar(List<DetalleCarritoBE_575_AV> detalles)
        {
            if (detalles.Count == 0) { return detalles; }

            Dictionary<int, ArticuloBE_575_AV> articulos =
                articuloBLL.ListarArticulos().ToDictionary(a => a.id_articulo);

            foreach (DetalleCarritoBE_575_AV detalle in detalles)
            {
                ArticuloBE_575_AV articulo;
                if (articulos.TryGetValue(detalle.id_articulo, out articulo))
                {
                    detalle.codigo_articulo = articulo.codigo;
                    detalle.descripcion_articulo = articulo.descripcion;
                    detalle.abreviatura_unidad = articulo.abreviatura_unidad;
                }
            }

            return detalles;
        }

        private void Validar(CarritoBE_575_AV carrito)
        {
            if (carrito == null) { throw new ArgumentNullException("carrito"); }

            if (carrito.id_carrito <= 0)
            {
                throw new InvalidOperationException("El carrito no esta identificado.");
            }

            if (!Releer(carrito.id_carrito).se_puede_editar)
            {
                throw new InvalidOperationException(
                    "El carrito ya fue confirmado y no admite cambios.");
            }
        }

        #endregion
    }
}
