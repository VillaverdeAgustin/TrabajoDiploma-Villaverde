using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Negocio_BLL
{
    public class ClienteBLL_575_AV
    {
        private const string TABLA = "Cliente";

        private MP_Cliente_575_AV mpCliente = new MP_Cliente_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        public ClienteBE_575_AV BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) { return null; }

            return mpCliente.BuscarPorDni(dni.Trim());
        }

        public string DesencriptarCorreo(ClienteBE_575_AV cliente, string clave, string usuario)
        {
            if (cliente == null) { return null; }

            UsuarioBE enSesion = SessionManager.Logged()
                ? SessionManager.GetInstance.UsuarioActual()
                : null;

            if (enSesion == null || string.IsNullOrEmpty(clave) ||
                Encriptador.EncriptarIrrev(clave) != enSesion.pass)
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CAJA_MSG_CLAVE_INCORRECTA"));
            }

            string correo = EncriptadorReversible_575_AV.Desencriptar(cliente.correo_electronico);
            bitacora.RegistrarBitacora(usuario, TipoAccion.DesencriptacionCorreo);
            return correo;
        }

        #endregion

        #region Alta

        public int CrearCliente(ClienteBE_575_AV cliente, string usuario)
        {
            Validar(cliente);

            if (mpCliente.BuscarPorDni(cliente.dni) != null)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("CLI_ERR_DNI_EXISTE"),
                        cliente.dni));
            }

            if (!string.IsNullOrWhiteSpace(cliente.cuit) &&
                mpCliente.BuscarPorCuit(cliente.cuit) != null)
            {
                throw new InvalidOperationException(
                    string.Format(GestorDeIdioma.GetInstance.Traducir("CLI_ERR_CUIT_EXISTE"),
                        cliente.cuit));
            }

            cliente.correo_electronico =
                EncriptadorReversible_575_AV.Encriptar(cliente.correo_electronico);

            cliente.activo = true;
            cliente.fecha_actualizacion = DateTime.Now;

            cliente.id_cliente = mpCliente.CrearCliente(cliente);

            ClienteBE_575_AV guardado = mpCliente.BuscarPorDni(cliente.dni);
            if (guardado != null) { cliente.fecha_actualizacion = guardado.fecha_actualizacion; }

            PersistirDigito(cliente);
            bitacora.RegistrarBitacora(usuario, TipoAccion.AltaCliente);

            return cliente.id_cliente;
        }

        #endregion

        #region Integridad

        public bool VerificarIntegridad()
        {
            List<ClienteBE_575_AV> clientes = mpCliente.ListarClientes()
                .OrderBy(c => c.id_cliente)
                .ToList();

            return verificador.VerificarIntegridad(clientes, TABLA);
        }

        public void RecalcularDV()
        {
            List<ClienteBE_575_AV> clientes = mpCliente.ListarClientes()
                .OrderBy(c => c.id_cliente)
                .ToList();

            foreach (ClienteBE_575_AV cliente in clientes)
            {
                cliente.dvh = VerificadorIntegridad.CalcularDVH(cliente);
                mpCliente.ActualizarDVH(cliente.id_cliente, cliente.dvh);
            }

            verificador.ActualizarDVV(TABLA);
        }

        private void PersistirDigito(ClienteBE_575_AV cliente)
        {
            cliente.dvh = VerificadorIntegridad.CalcularDVH(cliente);
            mpCliente.ActualizarDVH(cliente.id_cliente, cliente.dvh);
            verificador.ActualizarDVV(TABLA);
        }

        #endregion

        #region Validacion

        private void Validar(ClienteBE_575_AV cliente)
        {
            if (cliente == null) { throw new ArgumentNullException("cliente"); }

            if (string.IsNullOrWhiteSpace(cliente.dni))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_DNI"));
            }
            if (!Regex.IsMatch(cliente.dni.Trim(), @"^\d{7,8}$"))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_DNI_FORMATO"));
            }
            if (string.IsNullOrWhiteSpace(cliente.nombre))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_NOMBRE"));
            }
            if (string.IsNullOrWhiteSpace(cliente.apellido))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_APELLIDO"));
            }

            if (cliente.condicion_iva != CondicionIva_575_AV.ConsumidorFinal &&
                string.IsNullOrWhiteSpace(cliente.cuit))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_CUIT_REQUERIDO"));
            }

            if (!string.IsNullOrWhiteSpace(cliente.cuit) && !ValidarCuit(cliente.cuit))
            {
                throw new InvalidOperationException(
                    GestorDeIdioma.GetInstance.Traducir("CLI_ERR_CUIT"));
            }

            cliente.dni = cliente.dni.Trim();
        }

        private bool ValidarCuit(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) { return false; }

            string digitos = cuit.Replace("-", string.Empty).Replace(".", string.Empty).Trim();

            if (!Regex.IsMatch(digitos, @"^\d{11}$")) { return false; }

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

        #endregion
    }
}
