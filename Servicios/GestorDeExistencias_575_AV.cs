using Entidad_BE;
using System;

namespace Servicios
{
    /// <summary>
    /// Datos del aviso que se emite cuando un articulo cae al punto de reposicion.
    /// </summary>
    public class ArticuloBajoMinimoEventArgs_575_AV : EventArgs
    {
        public ArticuloBajoMinimoEventArgs_575_AV(ArticuloBE_575_AV articulo)
        {
            this.articulo = articulo;
        }

        public ArticuloBE_575_AV articulo { get; private set; }
    }

    /// <summary>
    /// Punto unico de verdad sobre la disponibilidad y el bloqueo de articulos.
    /// Patron <b>Singleton</b>.
    ///
    /// <para>Nadie descuenta existencias ni bloquea un articulo por fuera de este
    /// gestor. Las operaciones van bajo un candado, de modo que dos cajas no
    /// puedan vender el mismo saldo de bobina.</para>
    ///
    /// <para>No conoce la DAL: opera contra un <see cref="IExistencias_575_AV"/>
    /// que le entrega la BLL al iniciar, para no romper el orden de capas.</para>
    ///
    /// <para><see cref="BajoPuntoReposicion"/> queda expuesto desde ahora aunque
    /// todavia no haya quien lo escuche: el aviso por umbral con patron Observer
    /// y su tablero pertenecen al RFN2.</para>
    /// </summary>
    public sealed class GestorDeExistencias_575_AV
    {
        private static readonly GestorDeExistencias_575_AV instancia =
            new GestorDeExistencias_575_AV();

        private readonly object candado = new object();
        private IExistencias_575_AV existencias;

        private GestorDeExistencias_575_AV() { }

        public static GestorDeExistencias_575_AV GetInstance
        {
            get { return instancia; }
        }

        /// <summary>
        /// Engancha el acceso a datos. La BLL la llama al construirse; las
        /// llamadas posteriores no hacen nada, para que el gestor conserve
        /// siempre el mismo origen.
        /// </summary>
        public void Configurar(IExistencias_575_AV origen)
        {
            if (origen == null) { throw new ArgumentNullException("origen"); }

            lock (candado)
            {
                if (existencias == null) { existencias = origen; }
            }
        }

        /// <summary>Se emite cuando un movimiento deja al articulo en el punto de reposicion o por debajo.</summary>
        public event EventHandler<ArticuloBajoMinimoEventArgs_575_AV> BajoPuntoReposicion;

        /// <summary>
        /// RFN1.2 - Si el articulo puede cubrir la cantidad pedida. En los
        /// fraccionables no alcanza con la cantidad total: tiene que haber una
        /// bobina que la cubra por si sola.
        /// </summary>
        public bool HayDisponibilidad(int id_articulo, decimal cantidad)
        {
            if (cantidad <= 0) { return false; }

            lock (candado)
            {
                ArticuloBE_575_AV articulo = Origen().ObtenerArticulo(id_articulo);

                if (articulo == null) { return false; }
                if (!articulo.se_puede_vender) { return false; }
                if (articulo.cantidad_disponible < cantidad) { return false; }

                if (articulo.fraccionable)
                {
                    return Origen().HaySaldoDeBobina(id_articulo, cantidad);
                }

                return true;
            }
        }

        /// <summary>RFN1.6 - Descuenta existencias por una venta.</summary>
        public void Descontar(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a descontar tiene que ser mayor a cero.");
            }

            lock (candado)
            {
                if (!HayDisponibilidadSinCandado(id_articulo, cantidad))
                {
                    throw new InvalidOperationException(
                        "No hay disponibilidad suficiente para el articulo solicitado.");
                }

                Origen().MoverCantidad(id_articulo, -cantidad, usuario);
                AvisarSiQuedoBajoMinimo(id_articulo);
            }
        }

        /// <summary>
        /// RFN1.6 - Confirma la salida de mercaderia ya comprometida, al emitir
        /// el comprobante.
        ///
        /// <para>A diferencia de <see cref="Descontar"/>, no vuelve a exigir
        /// saldo de bobina: en los articulos fraccionables la bobina se eligio y
        /// se descontó al armar el carrito, asi que volver a pedirlo contaria
        /// dos veces el mismo metraje.</para>
        /// </summary>
        public void ConfirmarSalida(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a descontar tiene que ser mayor a cero.");
            }

            lock (candado)
            {
                ArticuloBE_575_AV articulo = Origen().ObtenerArticulo(id_articulo);

                if (articulo == null)
                {
                    throw new InvalidOperationException("El articulo no existe.");
                }
                if (articulo.cantidad_disponible < cantidad)
                {
                    throw new InvalidOperationException(
                        "Las existencias de " + articulo.descripcion +
                        " no alcanzan para confirmar la salida.");
                }

                Origen().MoverCantidad(id_articulo, -cantidad, usuario);
                AvisarSiQuedoBajoMinimo(id_articulo);
            }
        }

        /// <summary>Repone existencias: recepcion de mercaderia o anulacion de una venta.</summary>
        public void Reponer(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a reponer tiene que ser mayor a cero.");
            }

            lock (candado)
            {
                Origen().MoverCantidad(id_articulo, cantidad, usuario);
            }
        }

        /// <summary>
        /// Bloquea el articulo. Lo dispara la reserva de un faltante y se
        /// mantiene hasta que la compra se concrete.
        /// </summary>
        public void Bloquear(int id_articulo, string usuario)
        {
            lock (candado)
            {
                Origen().CambiarEstado(id_articulo, EstadoArticulo_575_AV.Bloqueado, usuario);
            }
        }

        /// <summary>Libera el bloqueo y devuelve el articulo al estado que le corresponda.</summary>
        public void Liberar(int id_articulo, string usuario)
        {
            lock (candado)
            {
                ArticuloBE_575_AV articulo = Origen().ObtenerArticulo(id_articulo);
                if (articulo == null) { return; }

                EstadoArticulo_575_AV estado = articulo.cantidad_disponible > 0
                    ? EstadoArticulo_575_AV.Disponible
                    : EstadoArticulo_575_AV.SinStock;

                Origen().CambiarEstado(id_articulo, estado, usuario);
            }
        }

        #region Apoyo

        private IExistencias_575_AV Origen()
        {
            if (existencias == null)
            {
                throw new InvalidOperationException(
                    "El gestor de existencias no tiene configurado su origen de datos.");
            }
            return existencias;
        }

        /// <summary>Misma verificacion que <see cref="HayDisponibilidad"/>, ya dentro del candado.</summary>
        private bool HayDisponibilidadSinCandado(int id_articulo, decimal cantidad)
        {
            ArticuloBE_575_AV articulo = Origen().ObtenerArticulo(id_articulo);

            if (articulo == null) { return false; }
            if (!articulo.se_puede_vender) { return false; }
            if (articulo.cantidad_disponible < cantidad) { return false; }

            return !articulo.fraccionable || Origen().HaySaldoDeBobina(id_articulo, cantidad);
        }

        private void AvisarSiQuedoBajoMinimo(int id_articulo)
        {
            EventHandler<ArticuloBajoMinimoEventArgs_575_AV> suscriptores = BajoPuntoReposicion;
            if (suscriptores == null) { return; }

            ArticuloBE_575_AV articulo = Origen().ObtenerArticulo(id_articulo);
            if (articulo != null && articulo.bajo_punto_reposicion)
            {
                suscriptores(this, new ArticuloBajoMinimoEventArgs_575_AV(articulo));
            }
        }

        #endregion
    }
}
