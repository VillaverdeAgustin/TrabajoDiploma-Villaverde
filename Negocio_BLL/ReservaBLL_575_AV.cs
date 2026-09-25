using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
    /// <summary>
    /// Reserva del faltante (RFN1.9).
    ///
    /// <para>Cuando la disponibilidad no cubre lo que pide el cliente, se
    /// compromete el articulo y se le informa el plazo estimado de reposicion.
    /// Al registrarse, el articulo queda <b>bloqueado</b> hasta que la compra se
    /// concrete, y el movimiento de reposicion pasa al RFN2.</para>
    ///
    /// <para>El bloqueo va por <c>GestorDeExistencias_575_AV</c>, que es el
    /// unico que cambia el estado de un articulo. Son dos tablas y dos
    /// procedimientos, encadenados aca: se bloquea primero y se registra
    /// despues, y si el alta falla se libera el articulo, para no dejarlo
    /// bloqueado por una reserva que no existe.</para>
    /// </summary>
    public class ReservaBLL_575_AV
    {
        private const string TABLA = "Reserva";

        /// <summary>Plazo por defecto, en dias: la reposicion es de importacion.</summary>
        public const int PLAZO_ENTREGA_POR_DEFECTO = 45;

        private MP_Reserva_575_AV mpReserva = new MP_Reserva_575_AV();
        private ArticuloBLL_575_AV articuloBLL = new ArticuloBLL_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public List<ReservaBE_575_AV> ListarPorArticulo(int id_articulo)
        {
            return Completar(mpReserva.ListarPorArticulo(id_articulo));
        }

        #endregion

        #region Registro

        /// <summary>
        /// Registra la reserva y bloquea el articulo. Devuelve la reserva con su
        /// id y su fecha estimada de entrega ya resueltos.
        /// </summary>
        public ReservaBE_575_AV RegistrarReserva(ReservaBE_575_AV reserva, string usuario)
        {
            Validar(reserva);

            ArticuloBE_575_AV articulo = articuloBLL.ObtenerPorId(reserva.id_articulo);
            if (articulo == null)
            {
                throw new InvalidOperationException("El articulo no existe.");
            }

            if (reserva.plazo_entrega <= 0)
            {
                reserva.plazo_entrega = PLAZO_ENTREGA_POR_DEFECTO;
            }

            reserva.fecha_reserva = DateTime.Now;
            reserva.estado = EstadoReserva_575_AV.Pendiente;

            // El articulo se bloquea primero: si el alta falla, se libera.
            GestorDeExistencias_575_AV.GetInstance.Bloquear(reserva.id_articulo, usuario);

            try
            {
                reserva.id_reserva = mpReserva.CrearReserva(reserva);
            }
            catch
            {
                GestorDeExistencias_575_AV.GetInstance.Liberar(reserva.id_articulo, usuario);
                throw;
            }

            // La base fija la fecha con su propio redondeo: se relee antes de
            // calcular el dvh.
            ReservaBE_575_AV guardada = mpReserva.ListarReservas()
                .FirstOrDefault(r => r.id_reserva == reserva.id_reserva) ?? reserva;

            PersistirDigito(guardada);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaReserva);

            guardada.descripcion_articulo = articulo.descripcion;
            return guardada;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<ReservaBE_575_AV> reservas = mpReserva.ListarReservas()
                .OrderBy(r => r.id_reserva)
                .ToList();

            return verificador.VerificarIntegridad(reservas, TABLA);
        }

        public void RecalcularDV()
        {
            List<ReservaBE_575_AV> reservas = mpReserva.ListarReservas()
                .OrderBy(r => r.id_reserva)
                .ToList();

            foreach (ReservaBE_575_AV reserva in reservas)
            {
                reserva.dvh = VerificadorIntegridad.CalcularDVH(reserva);
                mpReserva.ActualizarDVH(reserva.id_reserva, reserva.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(ReservaBE_575_AV reserva)
        {
            reserva.dvh = VerificadorIntegridad.CalcularDVH(reserva);
            mpReserva.ActualizarDVH(reserva.id_reserva, reserva.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Apoyo

        private List<ReservaBE_575_AV> Completar(List<ReservaBE_575_AV> reservas)
        {
            if (reservas.Count == 0) { return reservas; }

            Dictionary<int, ArticuloBE_575_AV> articulos =
                articuloBLL.ListarArticulos().ToDictionary(a => a.id_articulo);

            foreach (ReservaBE_575_AV reserva in reservas)
            {
                ArticuloBE_575_AV articulo;
                if (articulos.TryGetValue(reserva.id_articulo, out articulo))
                {
                    reserva.descripcion_articulo = articulo.descripcion;
                }
            }

            return reservas;
        }

        private void Validar(ReservaBE_575_AV reserva)
        {
            if (reserva == null) { throw new ArgumentNullException("reserva"); }

            if (reserva.id_articulo <= 0)
            {
                throw new InvalidOperationException("Hay que indicar el articulo a reservar.");
            }
            if (reserva.cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad tiene que ser mayor a cero.");
            }
            if (reserva.id_cliente == null && string.IsNullOrWhiteSpace(reserva.nombre_cliente))
            {
                throw new InvalidOperationException(
                    "Hay que identificar al cliente que retira la reserva.");
            }
        }

        #endregion
    }
}
