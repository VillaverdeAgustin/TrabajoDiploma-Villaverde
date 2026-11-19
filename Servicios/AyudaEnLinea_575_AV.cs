using System;
using System.Diagnostics;
using System.IO;

namespace Servicios
{
    public static class AyudaEnLinea_575_AV
    {
        public const string CARPETA = "Ayuda";
        public const string IDIOMA_RESPALDO = "es";
        public const string INDICE = "index";
        public const string EXTENSION = ".htm";

        public static string CarpetaBase { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static Action<string> Abridor { get; set; } = AbrirConNavegador;

        public static string RutaTema(string tema, string idioma)
        {
            if (string.IsNullOrWhiteSpace(tema)) { return null; }

            string archivo = tema.Trim() + EXTENSION;
            string codigo = string.IsNullOrWhiteSpace(idioma) ? IDIOMA_RESPALDO : idioma.Trim().ToLowerInvariant();

            string propia = Path.Combine(CarpetaBase, CARPETA, codigo, archivo);
            if (File.Exists(propia)) { return propia; }

            string respaldo = Path.Combine(CarpetaBase, CARPETA, IDIOMA_RESPALDO, archivo);
            return File.Exists(respaldo) ? respaldo : null;
        }

        public static string RutaIndice(string idioma)
        {
            return RutaTema(INDICE, idioma);
        }

        public static bool MostrarTema(string tema, string idioma)
        {
            return Abrir(RutaTema(tema, idioma) ?? RutaIndice(idioma));
        }

        public static bool MostrarIndice(string idioma)
        {
            return Abrir(RutaIndice(idioma));
        }

        private static bool Abrir(string ruta)
        {
            if (ruta == null) { return false; }
            Abridor(ruta);
            return true;
        }

        private static void AbrirConNavegador(string ruta)
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
    }
}
