using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    /// <summary>
    /// T03.2 - Encriptacion reversible AES-256 sobre el correo electronico del
    /// cliente. Es el complemento del <see cref="Encriptador"/>, que hace la
    /// irreversible (SHA-256) sobre usuario y contrasena.
    ///
    /// <para>La clave no esta en el codigo fuente: sale de la clave de
    /// aplicacion <c>ClaveAES</c> del archivo de configuracion, y de ahi se
    /// deriva la de 256 bits. Cambiarla deja ilegibles los datos ya cifrados.</para>
    ///
    /// <para>Cada texto se cifra con un vector de inicializacion distinto, que
    /// viaja al principio del resultado. Por eso cifrar dos veces el mismo
    /// correo da cadenas diferentes, y aun asi las dos se descifran bien.</para>
    /// </summary>
    public static class EncriptadorReversible_575_AV
    {
        private const string CLAVE_CONFIG = "ClaveAES";
        private const int BYTES_CLAVE = 32; // 256 bits
        private const int BYTES_VI = 16;

        /// <summary>Cifra un texto y lo devuelve en Base64, con el VI adelante.</summary>
        public static string Encriptar(string texto)
        {
            if (string.IsNullOrEmpty(texto)) { return texto; }

            using (Aes aes = CrearAes())
            {
                aes.GenerateIV();

                using (ICryptoTransform cifrador = aes.CreateEncryptor())
                using (MemoryStream salida = new MemoryStream())
                {
                    salida.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(salida, cifrador, CryptoStreamMode.Write))
                    {
                        byte[] datos = Encoding.UTF8.GetBytes(texto);
                        cs.Write(datos, 0, datos.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(salida.ToArray());
                }
            }
        }

        /// <summary>
        /// Devuelve el texto original. Si el dato no esta cifrado o esta
        /// dañado, se devuelve tal cual en lugar de cortar la operacion: el
        /// correo es un dato de contacto, no puede dejar sin atender a un
        /// cliente en el mostrador.
        /// </summary>
        public static string Desencriptar(string cifrado)
        {
            if (string.IsNullOrEmpty(cifrado)) { return cifrado; }

            try
            {
                byte[] todo = Convert.FromBase64String(cifrado);
                if (todo.Length <= BYTES_VI) { return cifrado; }

                byte[] vi = new byte[BYTES_VI];
                Buffer.BlockCopy(todo, 0, vi, 0, BYTES_VI);

                using (Aes aes = CrearAes())
                {
                    aes.IV = vi;

                    using (ICryptoTransform descifrador = aes.CreateDecryptor())
                    using (MemoryStream entrada = new MemoryStream(todo, BYTES_VI, todo.Length - BYTES_VI))
                    using (CryptoStream cs = new CryptoStream(entrada, descifrador, CryptoStreamMode.Read))
                    using (StreamReader lector = new StreamReader(cs, Encoding.UTF8))
                    {
                        return lector.ReadToEnd();
                    }
                }
            }
            catch (FormatException) { return cifrado; }
            catch (CryptographicException) { return cifrado; }
        }

        private static Aes CrearAes()
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = DerivarClave();
            return aes;
        }

        /// <summary>
        /// Lleva la clave de configuracion a los 32 bytes que necesita AES-256.
        /// La sal es fija para que el mismo texto de clave derive siempre en la
        /// misma llave y los datos ya cifrados se sigan pudiendo leer.
        /// </summary>
        private static byte[] DerivarClave()
        {
            string clave = ConfigurationManager.AppSettings[CLAVE_CONFIG];

            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ConfigurationErrorsException(
                    "No esta definida la clave '" + CLAVE_CONFIG +
                    "' en el archivo de configuracion de la aplicacion.");
            }

            byte[] sal = Encoding.UTF8.GetBytes("ConectAR.575_AV");

            using (Rfc2898DeriveBytes derivador = new Rfc2898DeriveBytes(clave, sal, 10000))
            {
                return derivador.GetBytes(BYTES_CLAVE);
            }
        }
    }
}
