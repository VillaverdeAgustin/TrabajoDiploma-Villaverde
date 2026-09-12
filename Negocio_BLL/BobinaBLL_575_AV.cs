using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Circuito de bobina: la particularidad del rubro. Los articulos que se
    /// venden por metro lineal salen de una caja o rollo, y hay que llevar el
    /// saldo remanente de cada bobina abierta.
    /// </summary>
    public class BobinaBLL_575_AV
    {
        private const string TABLA = "Bobina";

        private MP_Bobina_575_AV mpBobina = new MP_Bobina_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public List<BobinaBE_575_AV> ListarPorProducto(int idProducto)
        {
            return mpBobina.ListarPorProducto(idProducto);
        }

        /// <summary>Saldo total disponible de un producto, sumando sus bobinas.</summary>
        public decimal SaldoDisponible(int idProducto)
        {
            return mpBobina.ListarPorProducto(idProducto).Sum(b => b.SaldoActual);
        }

        /// <summary>
        /// Si alguna bobina puede cubrir la cantidad pedida por si sola. Se
        /// exige de una sola bobina porque un tramo de cable no se entrega
        /// empalmado: 200 m no se cubren con dos bobinas de 100 m.
        /// </summary>
        public bool ExisteSaldoSuficiente(int idProducto, decimal cantidad)
        {
            if (cantidad <= 0) { return false; }

            return mpBobina.ListarPorProducto(idProducto)
                .Any(b => b.SaldoActual >= cantidad);
        }

        #endregion

        #region Alta

        /// <summary>
        /// Registra una bobina nueva, cerrada y con el saldo completo.
        /// Es lo que ocurre al recibir mercaderia.
        /// </summary>
        public int CrearBobina(BobinaBE_575_AV bobina, string usuario)
        {
            if (bobina == null) { throw new ArgumentNullException("bobina"); }

            if (string.IsNullOrWhiteSpace(bobina.Identificador))
            {
                throw new InvalidOperationException("El identificador de la bobina es obligatorio.");
            }
            if (bobina.IdProducto <= 0)
            {
                throw new InvalidOperationException("La bobina tiene que pertenecer a un producto.");
            }
            if (bobina.MedidaInicial <= 0)
            {
                throw new InvalidOperationException("La medida inicial tiene que ser mayor a cero.");
            }

            bobina.SaldoActual = bobina.MedidaInicial;
            bobina.Estado = EstadoBobina_575_AV.Cerrada;
            bobina.FechaApertura = null;

            bobina.IdBobina = mpBobina.CrearBobina(bobina);

            PersistirDigito(bobina);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaBobina);

            return bobina.IdBobina;
        }

        #endregion

        #region Fraccionamiento

        /// <summary>
        /// RFN1.8 - Descuenta <paramref name="cantidad"/> del saldo de una bobina
        /// del producto y devuelve la bobina afectada.
        ///
        /// Elige la bobina abierta con menor saldo que alcance para cubrir el
        /// pedido: asi se termina de consumir lo ya abierto antes de romper una
        /// bobina nueva. Si no hay ninguna abierta que alcance, abre la de menor
        /// saldo entre las cerradas que sirva.
        /// </summary>
        public BobinaBE_575_AV Fraccionar(int idProducto, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a fraccionar tiene que ser mayor a cero.");
            }

            List<BobinaBE_575_AV> disponibles = mpBobina.ListarPorProducto(idProducto)
                .Where(b => b.SaldoActual >= cantidad)
                .ToList();

            if (disponibles.Count == 0)
            {
                throw new InvalidOperationException(
                    "No hay ninguna bobina con saldo suficiente para cubrir la cantidad pedida.");
            }

            // Primero las ya abiertas, y dentro de cada grupo la de menor saldo.
            BobinaBE_575_AV bobina = disponibles
                .OrderBy(b => b.Estado == EstadoBobina_575_AV.Abierta ? 0 : 1)
                .ThenBy(b => b.SaldoActual)
                .ThenBy(b => b.IdBobina)
                .First();

            if (bobina.Estado == EstadoBobina_575_AV.Cerrada)
            {
                bobina.FechaApertura = DateTime.Now;
            }

            bobina.SaldoActual -= cantidad;
            bobina.Estado = bobina.SaldoActual == 0
                ? EstadoBobina_575_AV.Agotada
                : EstadoBobina_575_AV.Abierta;

            mpBobina.ActualizarSaldo(bobina);

            PersistirDigito(bobina);
            bitacora.RegistrarBitacora(usuario, TipoAccion.FraccionamientoBobina);

            return bobina;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<BobinaBE_575_AV> bobinas = mpBobina.ListarBobinas()
                .OrderBy(b => b.IdBobina)
                .ToList();

            return verificador.VerificarIntegridad(bobinas, TABLA);
        }

        public void RecalcularDV()
        {
            List<BobinaBE_575_AV> bobinas = mpBobina.ListarBobinas()
                .OrderBy(b => b.IdBobina)
                .ToList();

            foreach (BobinaBE_575_AV bobina in bobinas)
            {
                bobina.DVH = VerificadorIntegridad.CalcularDVH(bobina);
                mpBobina.ActualizarDVH(bobina.IdBobina, bobina.DVH);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(BobinaBE_575_AV bobina)
        {
            bobina.DVH = VerificadorIntegridad.CalcularDVH(bobina);
            mpBobina.ActualizarDVH(bobina.IdBobina, bobina.DVH);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion
    }
}
