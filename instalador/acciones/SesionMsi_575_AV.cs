using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InstaladorConectAR_575_AV
{
    // Acceso minimo a la sesion de Windows Installer (msi.dll) para las acciones del instalador.
    internal sealed class SesionMsi_575_AV
    {
        private const uint ERROR_SUCCESS = 0;
        private const uint ERROR_MORE_DATA = 234;
        private const int MSIMODIFY_INSERT_TEMPORARY = 7;

        private const int MENSAJE_AVISO = 0x02000000;
        private const int MENSAJE_INFO = 0x04000000;
        private const int MENSAJE_INICIO_ACCION = 0x08000000;
        private const int MENSAJE_DATOS_ACCION = 0x09000000;
        private const int MENSAJE_PROGRESO = 0x0A000000;
        private const int BOTON_ACEPTAR_ICONO_AVISO = 0x30;

        private readonly uint sesion;

        public SesionMsi_575_AV(uint sesion)
        {
            this.sesion = sesion;
        }

        public string Propiedad(string nombre)
        {
            uint largo = 0;
            StringBuilder valor = new StringBuilder(1);
            uint resultado = MsiGetPropertyW(sesion, nombre, valor, ref largo);
            if (resultado == ERROR_MORE_DATA)
            {
                largo++;
                valor = new StringBuilder((int)largo);
                resultado = MsiGetPropertyW(sesion, nombre, valor, ref largo);
            }
            return resultado == ERROR_SUCCESS ? valor.ToString() : string.Empty;
        }

        public void FijarPropiedad(string nombre, string valor)
        {
            MsiSetPropertyW(sesion, nombre, valor ?? string.Empty);
        }

        public void Registrar(string texto)
        {
            Enviar(MENSAJE_INFO, "[" + DateTime.Now.ToString("HH:mm:ss") + "] ConectAR_575_AV: " + texto);
        }

        public void Avisar(string texto)
        {
            Enviar(MENSAJE_AVISO | BOTON_ACEPTAR_ICONO_AVISO, texto);
        }

        public void IniciarAccion(string accion, string descripcion, string plantilla)
        {
            Enviar(MENSAJE_INICIO_ACCION, accion, descripcion, plantilla);
        }

        public void DatosAccion(params object[] campos)
        {
            Enviar(MENSAJE_DATOS_ACCION, campos);
        }

        public void SumarAlTotalDeProgreso(int pasos)
        {
            Enviar(MENSAJE_PROGRESO, 3, pasos, 0, 0);
        }

        public void AvanzarProgreso(int pasos)
        {
            Enviar(MENSAJE_PROGRESO, 2, pasos, 0, 0);
        }

        // Agrega filas temporales a la tabla ComboBox para el combo ligado a la propiedad indicada.
        public void CargarCombo(string propiedad, string[] valores)
        {
            uint baseDatos = MsiGetActiveDatabase(sesion);
            if (baseDatos == 0) { return; }
            uint vista = 0;
            try
            {
                if (MsiDatabaseOpenViewW(baseDatos, "SELECT `Property`, `Order`, `Value`, `Text` FROM `ComboBox`", out vista) != ERROR_SUCCESS) { return; }
                MsiViewExecute(vista, 0);
                for (int i = 0; i < valores.Length; i++)
                {
                    uint fila = MsiCreateRecord(4);
                    try
                    {
                        MsiRecordSetStringW(fila, 1, propiedad);
                        MsiRecordSetInteger(fila, 2, i + 1);
                        MsiRecordSetStringW(fila, 3, valores[i]);
                        MsiRecordSetStringW(fila, 4, valores[i]);
                        MsiViewModify(vista, MSIMODIFY_INSERT_TEMPORARY, fila);
                    }
                    finally { MsiCloseHandle(fila); }
                }
            }
            finally
            {
                if (vista != 0) { MsiViewClose(vista); MsiCloseHandle(vista); }
                MsiCloseHandle(baseDatos);
            }
        }

        private void Enviar(int tipo, params object[] campos)
        {
            uint registro = MsiCreateRecord((uint)campos.Length);
            try
            {
                for (int i = 0; i < campos.Length; i++)
                {
                    if (campos[i] is int) { MsiRecordSetInteger(registro, (uint)(i + 1), (int)campos[i]); }
                    else { MsiRecordSetStringW(registro, (uint)(i + 1), Convert.ToString(campos[i])); }
                }
                bool conPlantilla = (tipo & 0xFF000000) == MENSAJE_INFO || (tipo & 0xFF000000) == MENSAJE_AVISO;
                if (conPlantilla) { MsiRecordSetStringW(registro, 0, "[1]"); }
                MsiProcessMessage(sesion, tipo, registro);
            }
            finally { MsiCloseHandle(registro); }
        }

        [DllImport("msi.dll", CharSet = CharSet.Unicode)] private static extern uint MsiGetPropertyW(uint hInstall, string szName, StringBuilder szValueBuf, ref uint pchValueBuf);
        [DllImport("msi.dll", CharSet = CharSet.Unicode)] private static extern uint MsiSetPropertyW(uint hInstall, string szName, string szValue);
        [DllImport("msi.dll")] private static extern uint MsiGetActiveDatabase(uint hInstall);
        [DllImport("msi.dll", CharSet = CharSet.Unicode)] private static extern uint MsiDatabaseOpenViewW(uint hDatabase, string szQuery, out uint phView);
        [DllImport("msi.dll")] private static extern uint MsiViewExecute(uint hView, uint hRecord);
        [DllImport("msi.dll")] private static extern uint MsiViewModify(uint hView, int eModifyMode, uint hRecord);
        [DllImport("msi.dll")] private static extern uint MsiViewClose(uint hView);
        [DllImport("msi.dll")] private static extern uint MsiCreateRecord(uint cParams);
        [DllImport("msi.dll", CharSet = CharSet.Unicode)] private static extern uint MsiRecordSetStringW(uint hRecord, uint iField, string szValue);
        [DllImport("msi.dll")] private static extern uint MsiRecordSetInteger(uint hRecord, uint iField, int iValue);
        [DllImport("msi.dll")] private static extern int MsiProcessMessage(uint hInstall, int eMessageType, uint hRecord);
        [DllImport("msi.dll")] private static extern uint MsiCloseHandle(uint hAny);
    }
}
