using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    public class CarritoBLL_575_AV
    {
        private const string TABLA = "Carrito";
        private const string TABLA_DETALLE = "DetalleCarrito";

        private MP_Carrito_575_AV mpCarrito = new MP_Carrito_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public List<CarritoBE_575_AV> ListarAbiertos()
        {
            List<CarritoBE_575_AV> carritos = mpCarrito.ListarAbiertos();

            foreach (CarritoBE_575_AV carrito in carritos)
            {
                carrito.detalle = ObtenerDetalle(carrito.id_carrito);
            }

            return carritos;
        }

        public List<DetalleCarritoBE_575_AV> ObtenerDetalle(int id_carrito)
        {
            return Completar(mpCarrito.ListarDetalle(id_carrito));
        }

        #endregion

        #region Armado

        public CarritoBE_575_AV CrearCarrito(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_SIN_VENDEDOR"));
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

            carrito = Releer(carrito.id_carrito);

            PersistirDigito(carrito);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaCarrito);

            return carrito;
        }

        public CarritoBE_575_AV AgregarDetalle(CarritoBE_575_AV carrito,
                                               DetalleCarritoBE_575_AV detalle,
                                               string usuario)
        {
            Validar(carrito);

            if (detalle == null) { throw new ArgumentNullException("detalle"); }
            if (detalle.cantidad <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_CANTIDAD"));
            }

            ArticuloBE_575_AV articulo = articuloBLL.ObtenerPorId(detalle.id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("ART_ERR_NO_EXISTE"));
            }

            detalle.id_carrito = carrito.id_carrito;
            detalle.id_unidad_medida = articulo.id_unidad_medida;
            detalle.precio_unitario = articulo.precio_unitario;
            detalle.subtotal = decimal.Round(articulo.precio_unitario * detalle.cantidad, 2);

            detalle.id_detalle_carrito = mpCarrito.AgregarDetalle(detalle);
            PersistirDigitoDetalle(detalle);

            CarritoBE_575_AV actualizado = RecalcularTotal(carrito.id_carrito);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaDetalleCarrito);

            return actualizado;
        }

        public CarritoBE_575_AV QuitarDetalle(CarritoBE_575_AV carrito,
                                              DetalleCarritoBE_575_AV detalle,
                                              string usuario)
        {
            Validar(carrito);

            if (detalle == null || detalle.id_detalle_carrito <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_RENGLON"));
            }

            mpCarrito.QuitarDetalle(detalle.id_detalle_carrito);

            verificador.ActualizarDVV(TABLA_DETALLE);

            CarritoBE_575_AV actualizado = RecalcularTotal(carrito.id_carrito);
            bitacora.RegistrarBitacora(usuario, TipoAccion.BajaDetalleCarrito);

            return actualizado;
        }

        public string ConfirmarCarrito(CarritoBE_575_AV carrito, string usuario)
        {
            Validar(carrito);

            if (mpCarrito.ListarDetalle(carrito.id_carrito).Count == 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_VACIO"));
            }

            mpCarrito.ActualizarEstado(carrito.id_carrito, EstadoCarrito_575_AV.Confirmado);

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

        private void PersistirDigitoDetalle(DetalleCarritoBE_575_AV detalle)
        {
            PersistirDigito(detalle);
        }

        #endregion

        #region Apoyo

        private CarritoBE_575_AV RecalcularTotal(int id_carrito)
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
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_NO_EXISTE"));
            }

            return carrito;
        }

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
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_NO_IDENTIFICADO"));
            }

            if (!Releer(carrito.id_carrito).se_puede_editar)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAR_ERR_CONFIRMADO"));
            }
        }

        #endregion
    }
}
