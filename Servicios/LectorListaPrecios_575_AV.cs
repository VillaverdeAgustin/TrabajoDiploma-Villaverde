using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace Servicios
{
    public class LectorListaPrecios_575_AV
    {
        public const string ERROR_FORMATO = "LPR_ERR_FORMATO";
        public const string MOTIVO_CODIGO_VACIO = "LPR_MOT_CODIGO_VACIO";
        public const string MOTIVO_CODIGO_LARGO = "LPR_MOT_CODIGO_LARGO";
        public const string MOTIVO_PRECIO_INVALIDO = "LPR_MOT_PRECIO_INVALIDO";
        public const string MOTIVO_PLAZO_INVALIDO = "LPR_MOT_PLAZO_INVALIDO";

        private static readonly Dictionary<string, string[]> Columnas = new Dictionary<string, string[]>
        {
            { "codigo", new[] { "codigo", "cod", "codigoproveedor", "codigoarticulo", "sku", "articulo" } },
            { "descripcion", new[] { "descripcion", "detalle", "producto", "nombre" } },
            { "precio", new[] { "precio", "preciolista", "preciounitario", "importe", "valor" } },
            { "unidad", new[] { "unidad", "unidadventa", "unidaddeventa", "presentacion", "um" } },
            { "plazo", new[] { "plazo", "plazoentrega", "plazodeentrega", "entrega", "dias" } }
        };

        public List<ItemListaPrecioBE_575_AV> Leer(string ruta)
        {
            string extension = (Path.GetExtension(ruta) ?? string.Empty).ToLowerInvariant();
            List<Fila> filas;

            if (extension == ".xlsx")
            {
                filas = LeerXlsx(ruta);
            }
            else if (extension == ".csv" || extension == ".txt")
            {
                filas = LeerCsv(ruta);
            }
            else
            {
                throw new FormatException(ERROR_FORMATO);
            }

            return Mapear(filas, extension == ".xlsx");
        }

        private List<Fila> LeerXlsx(string ruta)
        {
            try
            {
                using (FileStream archivo = File.OpenRead(ruta))
                using (ZipArchive zip = new ZipArchive(archivo, ZipArchiveMode.Read))
                {
                    List<string> compartidos = new List<string>();
                    XmlDocument cadenas = CargarXml(zip, "xl/sharedStrings.xml");
                    if (cadenas != null)
                    {
                        foreach (XmlNode si in Hijos(cadenas.DocumentElement, "si"))
                        {
                            compartidos.Add(string.Concat(Descendientes(si, "t").Select(t => t.InnerText)));
                        }
                    }

                    XmlDocument hoja = CargarXml(zip, RutaPrimeraHoja(zip));
                    if (hoja == null) { throw new FormatException(ERROR_FORMATO); }

                    List<Fila> filas = new List<Fila>();
                    foreach (XmlNode fila in Descendientes(hoja.DocumentElement, "row"))
                    {
                        int numero;
                        XmlAttribute r = fila.Attributes["r"];
                        numero = r != null && int.TryParse(r.Value, out numero) ? numero : filas.Count + 1;

                        SortedDictionary<int, string> celdas = new SortedDictionary<int, string>();
                        int siguiente = 0;
                        foreach (XmlNode c in Hijos(fila, "c"))
                        {
                            XmlAttribute referencia = c.Attributes["r"];
                            int columna = referencia != null ? IndiceColumna(referencia.Value) : siguiente;
                            siguiente = columna + 1;
                            celdas[columna] = ValorCelda(c, compartidos);
                        }

                        string[] valores = new string[celdas.Count == 0 ? 0 : celdas.Keys.Max() + 1];
                        foreach (KeyValuePair<int, string> celda in celdas) { valores[celda.Key] = celda.Value; }
                        filas.Add(new Fila(numero, valores));
                    }
                    return filas;
                }
            }
            catch (InvalidDataException)
            {
                throw new FormatException(ERROR_FORMATO);
            }
            catch (XmlException)
            {
                throw new FormatException(ERROR_FORMATO);
            }
        }

        private List<Fila> LeerCsv(string ruta)
        {
            byte[] bytes = File.ReadAllBytes(ruta);
            string texto;
            try
            {
                texto = new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                texto = Encoding.GetEncoding(1252).GetString(bytes);
            }
            texto = texto.TrimStart('﻿');

            string[] lineas = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            string primera = lineas.FirstOrDefault(l => l.Trim().Length > 0) ?? string.Empty;
            char separador = new[] { ';', ',', '\t' }
                .OrderByDescending(s => primera.Count(ch => ch == s))
                .First();

            List<Fila> filas = new List<Fila>();
            for (int i = 0; i < lineas.Length; i++)
            {
                filas.Add(new Fila(i + 1, PartirLinea(lineas[i], separador)));
            }
            return filas;
        }

        private static List<ItemListaPrecioBE_575_AV> Mapear(List<Fila> filas, bool desdeXlsx)
        {
            List<Fila> conDatos = filas.Where(f => f.Valores.Any(v => !string.IsNullOrWhiteSpace(v))).ToList();
            if (conDatos.Count < 2) { throw new FormatException(ERROR_FORMATO); }

            Fila encabezado = conDatos[0];
            Dictionary<string, int> indices = new Dictionary<string, int>();
            for (int i = 0; i < encabezado.Valores.Length; i++)
            {
                string nombre = Normalizar(encabezado.Valores[i]);
                foreach (KeyValuePair<string, string[]> columna in Columnas)
                {
                    if (!indices.ContainsKey(columna.Key) && columna.Value.Contains(nombre))
                    {
                        indices[columna.Key] = i;
                    }
                }
            }
            if (!indices.ContainsKey("codigo") || !indices.ContainsKey("precio"))
            {
                throw new FormatException(ERROR_FORMATO);
            }

            List<ItemListaPrecioBE_575_AV> items = new List<ItemListaPrecioBE_575_AV>();
            foreach (Fila fila in conDatos.Skip(1))
            {
                ItemListaPrecioBE_575_AV item = new ItemListaPrecioBE_575_AV
                {
                    numero_renglon = fila.Numero,
                    codigo_proveedor = Valor(fila, indices, "codigo"),
                    descripcion = Recortar(Valor(fila, indices, "descripcion"), 200),
                    unidad_venta = Recortar(Valor(fila, indices, "unidad"), 30),
                    resultado = ResultadoImportacion_575_AV.Actualizado
                };

                decimal precio;
                string plazoTexto = Valor(fila, indices, "plazo");
                int plazo = 0;

                if (string.IsNullOrEmpty(item.codigo_proveedor))
                {
                    Rechazar(item, MOTIVO_CODIGO_VACIO);
                }
                else if (item.codigo_proveedor.Length > 25)
                {
                    Rechazar(item, MOTIVO_CODIGO_LARGO);
                }
                else if (!ParsearPrecio(Valor(fila, indices, "precio"), desdeXlsx, out precio) || precio <= 0)
                {
                    Rechazar(item, MOTIVO_PRECIO_INVALIDO);
                }
                else if (!string.IsNullOrEmpty(plazoTexto) && !ParsearPlazo(plazoTexto, out plazo))
                {
                    item.precio_lista = precio;
                    Rechazar(item, MOTIVO_PLAZO_INVALIDO);
                }
                else
                {
                    item.precio_lista = precio;
                    item.plazo_entrega = string.IsNullOrEmpty(plazoTexto) ? (int?)null : plazo;
                }

                items.Add(item);
            }
            return items;
        }

        private static void Rechazar(ItemListaPrecioBE_575_AV item, string motivo)
        {
            item.resultado = ResultadoImportacion_575_AV.Rechazado;
            item.motivo = motivo;
        }

        private static bool ParsearPrecio(string texto, bool desdeXlsx, out decimal precio)
        {
            precio = 0;
            if (string.IsNullOrWhiteSpace(texto)) { return false; }

            string limpio = texto.Replace("$", string.Empty).Replace("ARS", string.Empty)
                                 .Replace("USD", string.Empty).Replace(" ", string.Empty)
                                 .Replace(" ", string.Empty).Trim();

            if (!desdeXlsx)
            {
                int coma = limpio.LastIndexOf(',');
                int punto = limpio.LastIndexOf('.');
                if (coma >= 0 && punto >= 0)
                {
                    limpio = coma > punto
                        ? limpio.Replace(".", string.Empty).Replace(',', '.')
                        : limpio.Replace(",", string.Empty);
                }
                else if (coma >= 0)
                {
                    limpio = limpio.Replace(',', '.');
                }
                else if (limpio.Count(ch => ch == '.') > 1)
                {
                    limpio = limpio.Replace(".", string.Empty);
                }
            }

            if (!decimal.TryParse(limpio, NumberStyles.Float, CultureInfo.InvariantCulture, out precio))
            {
                return false;
            }
            precio = decimal.Round(precio, 2);
            return true;
        }

        private static bool ParsearPlazo(string texto, out int plazo)
        {
            plazo = 0;
            decimal valor;
            if (!decimal.TryParse(texto.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out valor)) { return false; }
            if (valor < 0 || valor != decimal.Truncate(valor) || valor > int.MaxValue) { return false; }
            plazo = (int)valor;
            return true;
        }

        private static string Valor(Fila fila, Dictionary<string, int> indices, string columna)
        {
            int i;
            if (!indices.TryGetValue(columna, out i) || i >= fila.Valores.Length) { return null; }
            string valor = fila.Valores[i];
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        private static string Recortar(string valor, int largo)
        {
            return valor != null && valor.Length > largo ? valor.Substring(0, largo) : valor;
        }

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) { return string.Empty; }
            string descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder salida = new StringBuilder();
            foreach (char ch in descompuesto)
            {
                if (char.IsLetterOrDigit(ch) && CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    salida.Append(ch);
                }
            }
            return salida.ToString();
        }

        private static string[] PartirLinea(string linea, char separador)
        {
            List<string> campos = new List<string>();
            StringBuilder actual = new StringBuilder();
            bool entreComillas = false;

            for (int i = 0; i < linea.Length; i++)
            {
                char ch = linea[i];
                if (ch == '"')
                {
                    if (entreComillas && i + 1 < linea.Length && linea[i + 1] == '"')
                    {
                        actual.Append('"');
                        i++;
                    }
                    else
                    {
                        entreComillas = !entreComillas;
                    }
                }
                else if (ch == separador && !entreComillas)
                {
                    campos.Add(actual.ToString());
                    actual.Clear();
                }
                else
                {
                    actual.Append(ch);
                }
            }
            campos.Add(actual.ToString());
            return campos.ToArray();
        }

        private static string RutaPrimeraHoja(ZipArchive zip)
        {
            XmlDocument libro = CargarXml(zip, "xl/workbook.xml");
            XmlDocument relaciones = CargarXml(zip, "xl/_rels/workbook.xml.rels");
            if (libro == null) { throw new FormatException(ERROR_FORMATO); }

            XmlNode hoja = Descendientes(libro.DocumentElement, "sheet").FirstOrDefault();
            if (hoja == null) { throw new FormatException(ERROR_FORMATO); }

            XmlAttribute id = hoja.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.LocalName == "id");
            if (id != null && relaciones != null)
            {
                foreach (XmlNode relacion in Descendientes(relaciones.DocumentElement, "Relationship"))
                {
                    if (relacion.Attributes["Id"] != null && relacion.Attributes["Id"].Value == id.Value)
                    {
                        string destino = relacion.Attributes["Target"].Value.Replace('\\', '/');
                        return destino.StartsWith("/") ? destino.TrimStart('/') : "xl/" + destino;
                    }
                }
            }
            return "xl/worksheets/sheet1.xml";
        }

        private static XmlDocument CargarXml(ZipArchive zip, string ruta)
        {
            ZipArchiveEntry entrada = zip.Entries.FirstOrDefault(
                e => string.Equals(e.FullName, ruta, StringComparison.OrdinalIgnoreCase));
            if (entrada == null) { return null; }

            XmlDocument documento = new XmlDocument { XmlResolver = null };
            using (Stream flujo = entrada.Open())
            {
                documento.Load(flujo);
            }
            return documento;
        }

        private static string ValorCelda(XmlNode celda, List<string> compartidos)
        {
            string tipo = celda.Attributes["t"] != null ? celda.Attributes["t"].Value : "n";
            XmlNode v = Hijos(celda, "v").FirstOrDefault();

            switch (tipo)
            {
                case "s":
                    int indice;
                    return v != null && int.TryParse(v.InnerText, out indice) && indice < compartidos.Count
                        ? compartidos[indice] : null;
                case "inlineStr":
                    return string.Concat(Descendientes(celda, "t").Select(t => t.InnerText));
                default:
                    return v != null ? v.InnerText : null;
            }
        }

        private static int IndiceColumna(string referencia)
        {
            int indice = 0;
            foreach (char ch in referencia)
            {
                if (!char.IsLetter(ch)) { break; }
                indice = indice * 26 + (char.ToUpperInvariant(ch) - 'A' + 1);
            }
            return indice - 1;
        }

        private static IEnumerable<XmlNode> Hijos(XmlNode nodo, string nombre)
        {
            return nodo.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == nombre);
        }

        private static IEnumerable<XmlNode> Descendientes(XmlNode nodo, string nombre)
        {
            foreach (XmlNode hijo in nodo.ChildNodes)
            {
                if (hijo.LocalName == nombre) { yield return hijo; }
                foreach (XmlNode nieto in Descendientes(hijo, nombre)) { yield return nieto; }
            }
        }

        private class Fila
        {
            public Fila(int numero, string[] valores)
            {
                Numero = numero;
                Valores = valores ?? new string[0];
            }

            public int Numero { get; private set; }

            public string[] Valores { get; private set; }
        }
    }
}
