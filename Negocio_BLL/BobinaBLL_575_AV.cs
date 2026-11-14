using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio_BLL
{
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

        public decimal SaldoDisponible(int id_articulo)
        {
            return mpBobina.ListarPorArticulo(id_articulo).Sum(b => b.saldo_bobina);
        }

        public bool ExisteSaldoSuficiente(int id_articulo, decimal cantidad)
        {
            if (cantidad <= 0) { return false; }

            return mpBobina.ListarPorArticulo(id_articulo)
                .Any(b => b.saldo_bobina >= cantidad);
        }

        #endregion

        #region Alta

        public int CrearBobina(BobinaBE_575_AV bobina, string usuario)
        {
            if (bobina == null) { throw new ArgumentNullException("bobina"); }

            if (string.IsNullOrWhiteSpace(bobina.identificador))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("BOB_ERR_IDENTIFICADOR"));
            }
            if (bobina.id_articulo <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("BOB_ERR_ARTICULO"));
            }
            if (bobina.medida_inicial <= 0)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("BOB_ERR_MEDIDA"));
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
                    GestorDeIdioma.GetInstance.Traducir("BOB_ERR_SIN_SALDO"));
            }

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

            BobinaBE_575_AV guardada = mpBobina.ListarBobinas()
                .FirstOrDefault(b => b.id_bobina == bobina.id_bobina) ?? bobina;

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
