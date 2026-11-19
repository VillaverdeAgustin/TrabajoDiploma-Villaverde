using Negocio_BLL;
using Servicios;
using System;
using System.Windows.Forms;

namespace Presentacion_575_AV
{
    public static class AyudaFormulario_575_AV
    {
        public static void Asociar(Form formulario, string tema)
        {
            formulario.HelpRequested += (s, e) =>
            {
                MostrarTema(tema);
                e.Handled = true;
            };
        }

        public static void MostrarTema(string tema)
        {
            Mostrar(() => AyudaEnLinea_575_AV.MostrarTema(tema, IdiomaActual()));
        }

        public static void MostrarIndice()
        {
            Mostrar(() => AyudaEnLinea_575_AV.MostrarIndice(IdiomaActual()));
        }

        private static void Mostrar(Func<bool> abrir)
        {
            bool abierta;
            try
            {
                abierta = abrir();
            }
            catch (Exception)
            {
                abierta = false;
            }

            if (!abierta)
            {
                GestorDeIdioma idioma = GestorDeIdioma.GetInstance;
                MessageBox.Show(idioma.Traducir("AYUDA_ERR_NO_DISPONIBLE"), idioma.Traducir("COMUN_ATENCION"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static string IdiomaActual()
        {
            var idioma = GestorDeIdioma.GetInstance.IdiomaActual;
            return idioma == null || string.IsNullOrWhiteSpace(idioma.Codigo) ? GestorDeIdioma.IdiomaPorDefecto : idioma.Codigo;
        }
    }
}
