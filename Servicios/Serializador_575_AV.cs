using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Servicios
{
    // A03 - Serializacion a XML con XmlSerializer y SHA-256 del archivo generado.
    // Los errores de formato salen como FormatException con la clave de traduccion; la BLL los traduce.
    public class Serializador_575_AV
    {
        public const string ERROR_FORMATO = "SER_ERR_ARCHIVO";

        public void Serializar<T>(T objeto, string ruta)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(T));
            XmlWriterSettings opciones = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };

            using (XmlWriter escritor = XmlWriter.Create(ruta, opciones))
            {
                serializador.Serialize(escritor, objeto);
            }
        }

        public T Deserializar<T>(string ruta)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(T));
            XmlReaderSettings opciones = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };

            try
            {
                using (XmlReader lector = XmlReader.Create(ruta, opciones))
                {
                    if (!serializador.CanDeserialize(lector))
                    {
                        throw new FormatException(ERROR_FORMATO);
                    }
                    return (T)serializador.Deserialize(lector);
                }
            }
            catch (XmlException ex)
            {
                throw new FormatException(ERROR_FORMATO, ex);
            }
            catch (InvalidOperationException ex)
            {
                // XmlSerializer informa asi los valores que no puede convertir (un numero o una fecha mal escritos)
                throw new FormatException(ERROR_FORMATO, ex);
            }
        }

        public string CalcularHash(string ruta)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream archivo = File.OpenRead(ruta))
            {
                byte[] hash = sha.ComputeHash(archivo);
                StringBuilder texto = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    texto.Append(b.ToString("x2"));
                }
                return texto.ToString();
            }
        }
    }
}
