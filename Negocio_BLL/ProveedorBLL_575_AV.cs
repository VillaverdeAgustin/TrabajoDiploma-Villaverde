using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Negocio_BLL
{
    public class ProveedorBLL_575_AV
    {
        private const string TABLA = "Proveedor";

        private MP_Proveedor_575_AV mpProveedor = new MP_Proveedor_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public List<ProveedorBE_575_AV> ListarProveedores()
        {
            return mpProveedor.ListarProveedores();
        }

        public ProveedorBE_575_AV BuscarPorCuit(string cuit)
        {
            string normalizado = Normalizar(cuit);
            return normalizado == null ? null : mpProveedor.BuscarPorCuit(normalizado);
        }

        #endregion

        #region Alta y modificacion

        public int CrearProveedor(ProveedorBE_575_AV proveedor, string usuario)
        {
            Validar(proveedor);

            if (mpProveedor.BuscarPorCuit(proveedor.cuit) != null)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_CUIT_EXISTE"), proveedor.cuit));
            }

            ValidarCorreo(proveedor.correo_electronico);
            proveedor.correo_electronico = EncriptadorReversible_575_AV.Encriptar(proveedor.correo_electronico);
            proveedor.activo = true;

            proveedor.id_proveedor = mpProveedor.CrearProveedor(proveedor);

            PersistirDigito(proveedor);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaProveedor);

            return proveedor.id_proveedor;
        }

        public void ModificarProveedor(ProveedorBE_575_AV proveedor, string usuario)
        {
            Validar(proveedor);

            ProveedorBE_575_AV existente = mpProveedor.ListarProveedores()
                .FirstOrDefault(p => p.id_proveedor == proveedor.id_proveedor);
            if (existente == null)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_NO_EXISTE"));
            }

            ProveedorBE_575_AV mismoCuit = mpProveedor.BuscarPorCuit(proveedor.cuit);
            if (mismoCuit != null && mismoCuit.id_proveedor != proveedor.id_proveedor)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_CUIT_EXISTE"), proveedor.cuit));
            }

            if (proveedor.correo_electronico != existente.correo_electronico)
            {
                ValidarCorreo(proveedor.correo_electronico);
                proveedor.correo_electronico = EncriptadorReversible_575_AV.Encriptar(proveedor.correo_electronico);
            }

            mpProveedor.ActualizarProveedor(proveedor);

            PersistirDigito(proveedor);
            bitacora.RegistrarBitacora(usuario, TipoAccion.ModificacionProveedor);
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<ProveedorBE_575_AV> proveedores = mpProveedor.ListarProveedores()
                .OrderBy(p => p.id_proveedor)
                .ToList();

            return verificador.VerificarIntegridad(proveedores, TABLA);
        }

        public void RecalcularDV()
        {
            foreach (ProveedorBE_575_AV proveedor in mpProveedor.ListarProveedores().OrderBy(p => p.id_proveedor))
            {
                proveedor.dvh = VerificadorIntegridad.CalcularDVH(proveedor);
                mpProveedor.ActualizarDVH(proveedor.id_proveedor, proveedor.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(ProveedorBE_575_AV proveedor)
        {
            proveedor.dvh = VerificadorIntegridad.CalcularDVH(proveedor);
            mpProveedor.ActualizarDVH(proveedor.id_proveedor, proveedor.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Validacion

        private void Validar(ProveedorBE_575_AV proveedor)
        {
            if (proveedor == null) { throw new ArgumentNullException("proveedor"); }

            proveedor.razon_social = Recortar(proveedor.razon_social);
            proveedor.rubro = Recortar(proveedor.rubro);
            proveedor.telefono = Recortar(proveedor.telefono);
            proveedor.direccion = Recortar(proveedor.direccion);
            proveedor.correo_electronico = Recortar(proveedor.correo_electronico);

            if (string.IsNullOrEmpty(proveedor.razon_social))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_RAZON_SOCIAL"));
            }
            if (!ValidarCuit(proveedor.cuit))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_CUIT"));
            }
            if (proveedor.condicion_iva == CondicionIva_575_AV.ConsumidorFinal)
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_CONDICION_IVA"));
            }
            proveedor.cuit = Normalizar(proveedor.cuit);
        }

        private bool ValidarCuit(string cuit)
        {
            string digitos = SoloDigitos(cuit);
            if (digitos == null) { return false; }

            int[] factores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < factores.Length; i++)
            {
                suma += (digitos[i] - '0') * factores[i];
            }

            int resto = suma % 11;
            int esperado = resto == 0 ? 0 : (resto == 1 ? 9 : 11 - resto);

            return esperado == (digitos[10] - '0');
        }

        private static void ValidarCorreo(string correo)
        {
            if (correo != null && !Regex.IsMatch(correo, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                throw new InvalidOperationException(GestorDeIdioma.GetInstance.Traducir("PRV_ERR_CORREO"));
            }
        }

        private static string SoloDigitos(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) { return null; }
            string digitos = cuit.Replace("-", string.Empty).Replace(".", string.Empty).Replace(" ", string.Empty);
            return Regex.IsMatch(digitos, @"^\d{11}$") ? digitos : null;
        }

        private static string Normalizar(string cuit)
        {
            string digitos = SoloDigitos(cuit);
            return digitos == null ? null : digitos.Substring(0, 2) + "-" + digitos.Substring(2, 8) + "-" + digitos.Substring(10, 1);
        }

        private static string Recortar(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        #endregion
    }
}
