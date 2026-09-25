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

        public List<BobinaBE_575_AV> ListarPorArticulo(int id_articulo)
        {
            return mpBobina.ListarPorArticulo(id_articulo);
        }

        /// <summary>Saldo total disponible de un articulo, sumando sus bobinas.</summary>
        public decimal SaldoDisponible(int id_articulo)
        {
            return mpBobina.ListarPorArticulo(id_articulo).Sum(b => b.saldo_bobina);
        }

        /// <summary>
        /// Si alguna bobina puede cubrir la cantidad pedida por si sola. Se exige
        /// de una sola bobina porque un tramo de cable no se entrega empalmado:
        /// 200 m no se cubren con dos bobinas de 100 m.
        /// </summary>
        public bool ExisteSaldoSuficiente(int id_articulo, decimal cantidad)
        {
            if (cantidad <= 0) { return false; }

            return mpBobina.ListarPorArticulo(id_articulo)
                .Any(b => b.saldo_bobina >= cantidad);
        }

        #endregion

        #region Alta

        /// <summary>
        /// Registra una bobina nueva, cerrada y con el saldo completo. Es lo que
        /// ocurre al recibir mercaderia.
        /// </summary>
        public int CrearBobina(BobinaBE_575_AV bobina, string usuario)
        {
            if (bobina == null) { throw new ArgumentNullException("bobina"); }

            if (string.IsNullOrWhiteSpace(bobina.identificador))
            {
                throw new InvalidOperationException("El identificador de la bobina es obligatorio.");
            }
            if (bobina.id_articulo <= 0)
            {
                throw new InvalidOperationException("La bobina tiene que pertenecer a un articulo.");
            }
            if (bobina.medida_inicial <= 0)
            {
                throw new InvalidOperationException("La medida inicial tiene que ser mayor a cero.");
            }

            bobina.saldo_bobina = bobina.medida_inicial;
            bobina.estado = EstadoBobina_575_AV.Cerrada;
            bobina.fecha_apertura = null;

            bobina.id_bobina = mpBobina.CrearBobina(bobina);

            PersistirDigito(bobina);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaBobina);

            return bobina.id_bobina;
        }

        #endregion

        #region Fraccionamiento

        /// <summary>
        /// RFN1.8 - Descuenta <paramref name="cantidad"/> del saldo de una bobina
        /// del articulo y devuelve la bobina afectada.
        ///
        /// <para>Elige la bobina abierta con menor saldo que alcance para cubrir
        /// el pedido: asi se termina de consumir lo ya abierto antes de romper una
        /// bobina nueva. Si no hay ninguna abierta que alcance, abre la de menor
        /// saldo entre las cerradas que sirva.</para>
        /// </summary>
        public BobinaBE_575_AV Fraccionar(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a fraccionar tiene que ser mayor a cero.");
            }

            List<BobinaBE_575_AV> disponibles = mpBobina.ListarPorArticulo(id_articulo)
                .Where(b => b.saldo_bobina >= cantidad)
                .ToList();

            if (disponibles.Count == 0)
            {
                throw new InvalidOperationException(
                    "No hay ninguna bobina con saldo suficiente para cubrir la cantidad pedida.");
            }

            // Primero las ya abiertas, y dentro de cada grupo la de menor saldo.
            BobinaBE_575_AV bobina = disponibles
                .OrderBy(b => b.estado == EstadoBobina_575_AV.Abierta ? 0 : 1)
                .ThenBy(b => b.saldo_bobina)
                .ThenBy(b => b.id_bobina)
                .First();

            if (bobina.estado == EstadoBobina_575_AV.Cerrada)
            {
                bobina.fecha_apertura = DateTime.Now;
            }

            bobina.saldo_bobina -= cantidad;
            bobina.estado = bobina.saldo_bobina == 0
                ? EstadoBobina_575_AV.Agotada
                : EstadoBobina_575_AV.Abierta;

            mpBobina.ActualizarSaldo(bobina);

            // Se relee la fila antes de calcular el dvh: la base redondea
            // datetime a unos 3 ms, y si el redondeo cruza el segundo el digito
            // calculado en memoria no coincidiria con el dato guardado.
            BobinaBE_575_AV guardada = mpBobina.ListarBobinas()
                .FirstOrDefault(b => b.id_bobina == bobina.id_bobina) ?? bobina;

            PersistirDigito(guardada);
            bitacora.RegistrarBitacora(usuario, TipoAccion.FraccionamientoBobina);

            return guardada;
        }

        /// <summary>
        /// Devuelve metraje a una bobina. Es la vuelta atras de
        /// <see cref="Fraccionar"/>: se usa cuando se quita del carrito un
        /// renglon que ya habia comprometido saldo.
        /// </summary>
        public BobinaBE_575_AV Devolver(int id_bobina, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a devolver tiene que ser mayor a cero.");
            }

            BobinaBE_575_AV bobina = mpBobina.ListarBobinas()
                .FirstOrDefault(b => b.id_bobina == id_bobina);

            if (bobina == null)
            {
                throw new InvalidOperationException("La bobina no existe.");
            }

            if (bobina.saldo_bobina + cantidad > bobina.medida_inicial)
            {
                throw new InvalidOperationException(
                    "La devolucion supera la medida original de la bobina.");
            }

            bobina.saldo_bobina += cantidad;

            // Vuelve a Abierta salvo que quede entera, en cuyo caso nunca se
            // llego a consumir nada de ella.
            bobina.estado = bobina.saldo_bobina == bobina.medida_inicial
                ? EstadoBobina_575_AV.Cerrada
                : EstadoBobina_575_AV.Abierta;

            mpBobina.ActualizarSaldo(bobina);

            BobinaBE_575_AV guardada = mpBobina.ListarBobinas()
                .FirstOrDefault(b => b.id_bobina == id_bobina) ?? bobina;

            PersistirDigito(guardada);
            bitacora.RegistrarBitacora(usuario, TipoAccion.FraccionamientoBobina);

            return guardada;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<BobinaBE_575_AV> bobinas = mpBobina.ListarBobinas()
                .OrderBy(b => b.id_bobina)
                .ToList();

            return verificador.VerificarIntegridad(bobinas, TABLA);
        }

        public void RecalcularDV()
        {
            List<BobinaBE_575_AV> bobinas = mpBobina.ListarBobinas()
                .OrderBy(b => b.id_bobina)
                .ToList();

            foreach (BobinaBE_575_AV bobina in bobinas)
            {
                bobina.dvh = VerificadorIntegridad.CalcularDVH(bobina);
                mpBobina.ActualizarDVH(bobina.id_bobina, bobina.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(BobinaBE_575_AV bobina)
        {
            bobina.dvh = VerificadorIntegridad.CalcularDVH(bobina);
            mpBobina.ActualizarDVH(bobina.id_bobina, bobina.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion
    }
}
