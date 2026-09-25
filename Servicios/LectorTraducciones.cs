using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Servicios
{
    /// <summary>
    /// Servicio tecnico para leer archivos de traducciones.
    /// Formato esperado (UTF-8): una traduccion por linea "CLAVE;Texto".
    /// Las lineas vacias o que comienzan con # se ignoran.
    /// </summary>
    public static class LectorTraducciones
    {
        public static Dictionary<string, string> LeerArchivo(string ruta)
        {
            Dictionary<string, string> traducciones = new Dictionary<string, string>();
            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();
                if (linea == "" || linea.StartsWith("#")) { continue; }

                int separador = linea.IndexOf(';');
                if (separador <= 0)
                {
                    throw new FormatException($"Linea {i + 1}: se esperaba el formato CLAVE;Texto");
                }

                string clave = linea.Substring(0, separador).Trim().ToUpper();
                string texto = linea.Substring(separador + 1).Trim();
                if (clave != "" && texto != "")
                {
                    traducciones[clave] = texto;
                }
            }
            return traducciones;
        }
    }
}
