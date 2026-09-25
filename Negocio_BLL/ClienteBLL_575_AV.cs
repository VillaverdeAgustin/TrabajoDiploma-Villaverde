using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Negocio_BLL
{
    /// <summary>
    /// Logica de cliente.
    ///
    /// <para>En la Entrega 1 el cliente solo se consulta y se da de alta: el
    /// alta entra por DNI desde la caja, como extend de CUN-004. El ABM completo
    /// (modificacion y baja) queda fuera del alcance, por eso no hay metodos ni
    /// stored procedures para eso todavia.</para>
    ///
    /// <para>Es la unica clase que cifra y descifra el correo (T03.2): la DAL lo
    /// mueve tal cual esta guardado.</para>
    /// </summary>
    public class ClienteBLL_575_AV
    {
        private const string TABLA = "Cliente";

        private MP_Cliente_575_AV mpCliente = new MP_Cliente_575_AV();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL verificador = new VerificadorIntegridadBLL();

        #region Consulta

        /// <summary>
        /// Entrada del circuito de caja: se busca al cliente por documento.
        /// Devuelve nulo si no existe, que es lo que dispara el alta.
        /// </summary>
        public ClienteBE_575_AV BuscarPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) { return null; }

            return mpCliente.BuscarPorDni(dni.Trim());
        }

        /// <summary>
        /// Devuelve el correo en claro. Es el unico camino para leerlo: en la
        /// pantalla queda detras del boton protegido por clave.
        /// </summary>
        public string DesencriptarCorreo(ClienteBE_575_AV cliente)
        {
            if (cliente == null) { return null; }

            return EncriptadorReversible_575_AV.Desencriptar(cliente.correo_electronico);
        }

        #endregion

        #region Alta

        /// <summary>
        /// Alta de cliente. El correo se cifra antes de persistirlo, y el
        /// <paramref name="cliente"/> queda con el valor cifrado, que es el que
        /// entra al digito verificador.
        /// </summary>
        public int CrearCliente(ClienteBE_575_AV cliente, string usuario)
        {
            Validar(cliente);

            if (mpCliente.BuscarPorDni(cliente.dni) != null)
            {
                throw new InvalidOperationException(
                    "Ya hay un cliente registrado con el documento " + cliente.dni + ".");
            }

            if (!string.IsNullOrWhiteSpace(cliente.cuit) &&
                mpCliente.BuscarPorCuit(cliente.cuit) != null)
            {
                throw new InvalidOperationException(
                    "Ya hay un cliente registrado con el CUIT " + cliente.cuit + ".");
            }

            // T03.2 - el correo se guarda cifrado con AES-256.
            cliente.correo_electronico =
                EncriptadorReversible_575_AV.Encriptar(cliente.correo_electronico);

            cliente.activo = true;
            cliente.fecha_actualizacion = DateTime.Now;

            cliente.id_cliente = mpCliente.CrearCliente(cliente);

            // Se relee la fila antes de calcular el dvh: la base redondea
            // datetime a unos 3 ms, y si el redondeo cruza el segundo el digito
            // calculado en memoria no coincidiria con el dato guardado.
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
                throw new InvalidOperationException("El documento es obligatorio.");
            }
            if (!Regex.IsMatch(cliente.dni.Trim(), @"^\d{7,8}$"))
            {
                throw new InvalidOperationException("El documento tiene que tener 7 u 8 digitos.");
            }
            if (string.IsNullOrWhiteSpace(cliente.nombre))
            {
                throw new InvalidOperationException("El nombre es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(cliente.apellido))
            {
                throw new InvalidOperationException("El apellido es obligatorio.");
            }

            // Fuera de consumidor final, la condicion frente al IVA exige CUIT.
            if (cliente.condicion_iva != CondicionIva_575_AV.ConsumidorFinal &&
                string.IsNullOrWhiteSpace(cliente.cuit))
            {
                throw new InvalidOperationException(
                    "La condicion frente al IVA elegida requiere informar el CUIT.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.cuit) && !ValidarCuit(cliente.cuit))
            {
                throw new InvalidOperationException("El CUIT no es valido.");
            }

            cliente.dni = cliente.dni.Trim();
        }

        /// <summary>
        /// Valida un CUIT por su digito verificador, con los factores 5 4 3 2 7
        /// 6 5 4 3 2 sobre los primeros diez digitos. Acepta con guiones o sin
        /// ellos.
        /// </summary>
        public bool ValidarCuit(string cuit)
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
