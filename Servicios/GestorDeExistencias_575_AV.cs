using Entidad_BE;
using System;

namespace Servicios
{
    public class ArticuloBajoMinimoEventArgs_575_AV : EventArgs
    {
        public ArticuloBajoMinimoEventArgs_575_AV(ArticuloBE_575_AV articulo)
        {
            this.articulo = articulo;
        }

        public ArticuloBE_575_AV articulo { get; private set; }
    }

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

        public void Configurar(IExistencias_575_AV origen)
        {
            if (origen == null) { throw new ArgumentNullException("origen"); }

            lock (candado)
            {
                if (existencias == null) { existencias = origen; }
            }
        }

        public event EventHandler<ArticuloBajoMinimoEventArgs_575_AV> BajoPuntoReposicion;

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

        public void Descontar(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("EXI_ERR_CANTIDAD_DESCONTAR");
            }

            lock (candado)
            {
                if (!HayDisponibilidadSinCandado(id_articulo, cantidad))
                {
                    throw new InvalidOperationException("EXI_ERR_SIN_DISPONIBILIDAD");
                }

                Origen().MoverCantidad(id_articulo, -cantidad, usuario);
                AvisarSiQuedoBajoMinimo(id_articulo);
            }
        }

        public void Reponer(int id_articulo, decimal cantidad, string usuario)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("EXI_ERR_CANTIDAD_REPONER");
            }

            lock (candado)
            {
                Origen().MoverCantidad(id_articulo, cantidad, usuario);
            }
        }

        public void Bloquear(int id_articulo, string usuario)
        {
            lock (candado)
            {
                Origen().CambiarEstado(id_articulo, EstadoArticulo_575_AV.Bloqueado, usuario);
            }
        }

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
                throw new InvalidOperationException("EXI_ERR_SIN_ORIGEN");
            }
            return existencias;
        }

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
