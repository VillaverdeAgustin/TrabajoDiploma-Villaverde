using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    public static class EncriptadorReversible_575_AV
    {
        private const string CLAVE_AES = "ConectAR-575-AV-2026";
        private const int BYTES_CLAVE = 32;
        private const int BYTES_VI = 16;

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

        private static byte[] DerivarClave()
        {
            byte[] sal = Encoding.UTF8.GetBytes("ConectAR.575_AV");

            using (Rfc2898DeriveBytes derivador = new Rfc2898DeriveBytes(CLAVE_AES, sal, 10000))
            {
                return derivador.GetBytes(BYTES_CLAVE);
            }
        }
    }
}
