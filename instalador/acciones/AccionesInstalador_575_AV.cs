using Acceso_DAL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace InstaladorConectAR_575_AV
{
    // Acciones personalizadas del instalador. Cada metodo publico se exporta como funcion nativa
    // (generar_instalador.ps1 agrega las exportaciones) y Windows Installer lo llama con el handle de la sesion.
    // La conexion, el listado de instancias, la ejecucion de los scripts y el archivo conexion.cfg usan
    // las mismas clases de la DAL que el programa (ConfiguracionConexion_575_AV e InstaladorBaseDatos_575_AV).
    public static class AccionesInstalador_575_AV
    {
        private const uint EXITO = 0;
        private const string PROP_INSTANCIA = "CONECTAR_INSTANCIA";
        private const string PROP_PRUEBA = "CONECTAR_PRUEBA";
        private const string PROP_RESULTADO = "CONECTAR_RESULTADO";
        private const string PROP_SIN_BASE = "CONECTAR_SIN_BASE";
        private const string ACCION_DIFERIDA = "CrearBaseDatos_575_AV";
        private const char SEPARADOR = '|';
        private const int PASOS_POR_SCRIPT = 50000;
        private const int SCRIPTS_ESPERADOS = 11;

        // Interfaz: llena el combo con las instancias locales y propone la ya configurada o la primera.
        public static uint ListarInstancias(uint manejador)
        {
            SesionMsi_575_AV sesion = new SesionMsi_575_AV(manejador);
            try
            {
                List<string> instancias = ConfiguracionConexion_575_AV.ListarInstanciasLocales();
                sesion.CargarCombo(PROP_INSTANCIA, instancias.ToArray());
                if (string.IsNullOrWhiteSpace(sesion.Propiedad(PROP_INSTANCIA)))
                {
                    sesion.FijarPropiedad(PROP_INSTANCIA, InstanciaPropuesta(instancias));
                }
                sesion.Registrar("instancias locales: " + string.Join(", ", instancias));
            }
            catch (Exception ex)
            {
                sesion.Registrar("no se pudieron listar las instancias: " + ex.Message);
            }
            return EXITO;
        }

        // Interfaz: boton "Probar conexion" (y Siguiente). Autenticacion de Windows contra master.
        public static uint ProbarConexion(uint manejador)
        {
            SesionMsi_575_AV sesion = new SesionMsi_575_AV(manejador);
            string instancia = sesion.Propiedad(PROP_INSTANCIA).Trim();
            try
            {
                InstaladorBaseDatos_575_AV instalador = new InstaladorBaseDatos_575_AV();
                if (instancia.Length > 0 && instalador.ProbarServidor(instancia))
                {
                    bool existe = instalador.ExisteBase(instancia);
                    sesion.FijarPropiedad(PROP_PRUEBA, "1");
                    sesion.FijarPropiedad(PROP_SIN_BASE, string.Empty);
                    sesion.FijarPropiedad(PROP_RESULTADO, existe
                        ? "Conexión correcta. ConectAR_DB ya existe: se conservan sus datos y no se ejecutan scripts."
                        : "Conexión correcta. Se va a crear la base ConectAR_DB.");
                }
                else
                {
                    sesion.FijarPropiedad(PROP_PRUEBA, "0");
                    sesion.FijarPropiedad(PROP_RESULTADO, "No se pudo conectar con la instancia indicada.");
                }
            }
            catch (Exception ex)
            {
                sesion.FijarPropiedad(PROP_PRUEBA, "0");
                sesion.FijarPropiedad(PROP_RESULTADO, "No se pudo conectar con la instancia indicada.");
                sesion.Registrar("prueba de conexion con '" + instancia + "': " + ex.Message);
            }
            sesion.Registrar("prueba de conexion con '" + instancia + "': " + sesion.Propiedad(PROP_PRUEBA));
            return EXITO;
        }

        // Ejecucion inmediata: arma los datos de la accion diferida y reserva el avance de la barra.
        public static uint PrepararBaseDatos(uint manejador)
        {
            SesionMsi_575_AV sesion = new SesionMsi_575_AV(manejador);
            try
            {
                string instancia = sesion.Propiedad(PROP_INSTANCIA).Trim();
                if (instancia.Length == 0)
                {
                    instancia = InstanciaPropuesta(ConfiguracionConexion_575_AV.ListarInstanciasLocales());
                }
                bool sinBase = sesion.Propiedad(PROP_SIN_BASE) == "1";
                string carpeta = Path.Combine(sesion.Propiedad("APPDIR"), "db");

                sesion.FijarPropiedad(ACCION_DIFERIDA, instancia + SEPARADOR + carpeta + SEPARADOR + (sinBase ? "1" : "0"));
                if (!sinBase) { sesion.SumarAlTotalDeProgreso(PASOS_POR_SCRIPT * SCRIPTS_ESPERADOS); }
                sesion.Registrar("instancia elegida: '" + instancia + "', scripts en " + carpeta + ", sin base: " + sinBase);
            }
            catch (Exception ex)
            {
                sesion.Registrar("no se pudo preparar la creacion de la base: " + ex.Message);
            }
            return EXITO;
        }

        // Ejecucion diferida, con la cuenta del usuario que instala (sin "No Impersonate").
        // Guarda conexion.cfg y crea ConectAR_DB si no existe. Nunca hace fallar la instalacion.
        public static uint CrearBaseDatos(uint manejador)
        {
            SesionMsi_575_AV sesion = new SesionMsi_575_AV(manejador);
            string[] datos = sesion.Propiedad("CustomActionData").Split(SEPARADOR);
            string instancia = datos.Length > 0 ? datos[0].Trim() : string.Empty;
            string carpeta = datos.Length > 1 ? datos[1] : string.Empty;
            bool sinBase = datos.Length > 2 && datos[2] == "1";

            sesion.Registrar("cuenta de ejecucion: " + WindowsIdentity.GetCurrent().Name);
            GuardarConfiguracion(sesion, instancia);

            if (sinBase || instancia.Length == 0)
            {
                sesion.Registrar("se continua sin crear la base: el programa la crea en el primer inicio");
                return EXITO;
            }

            InstaladorBaseDatos_575_AV instalador = new InstaladorBaseDatos_575_AV(carpeta);
            if (!instalador.ProbarServidor(instancia))
            {
                sesion.Avisar("No se pudo conectar con la instancia " + instancia + ". La instalación continúa: " +
                              "ConectAR va a pedir la instancia y crear la base de datos la primera vez que se inicie.");
                return EXITO;
            }

            bool creada = false;
            try
            {
                if (instalador.ExisteBase(instancia))
                {
                    sesion.Registrar("ConectAR_DB ya existe en " + instancia + ": no se ejecuta ningun script");
                    return EXITO;
                }

                sesion.IniciarAccion(ACCION_DIFERIDA, "Creando la base de datos ConectAR_DB", "Ejecutando [1] ([2] de [3])");
                creada = true;
                instalador.EjecutarScripts(instancia, new AvanceMsi(sesion));
                sesion.Registrar("ConectAR_DB creada en " + instancia);
            }
            catch (Exception ex)
            {
                sesion.Registrar("fallo la creacion de la base: " + ex.Message);
                if (creada)
                {
                    try { instalador.BorrarBase(instancia); sesion.Registrar("se borro la base a medio crear"); }
                    catch (Exception borrado) { sesion.Registrar("no se pudo borrar la base a medio crear: " + borrado.Message); }
                }
                sesion.Avisar("No se pudo crear la base de datos ConectAR_DB y se deshizo lo creado. La instalación continúa: " +
                              "ConectAR va a crear la base de datos la primera vez que se inicie.");
            }
            return EXITO;
        }

        private static string InstanciaPropuesta(List<string> instancias)
        {
            string configurada = ConfiguracionConexion_575_AV.ObtenerInstancia();
            if (!string.IsNullOrWhiteSpace(configurada)) { return configurada; }
            return instancias.Count > 0 ? instancias[0] : "localhost";
        }

        private static void GuardarConfiguracion(SesionMsi_575_AV sesion, string instancia)
        {
            if (instancia.Length == 0) { return; }
            try
            {
                ConfiguracionConexion_575_AV.GuardarInstancia(instancia);
                PermitirModificarAUsuarios(Path.GetDirectoryName(ConfiguracionConexion_575_AV.RutaArchivo));
                sesion.Registrar("conexion.cfg guardado en " + ConfiguracionConexion_575_AV.RutaArchivo + " con instancia=" + instancia);
            }
            catch (Exception ex)
            {
                sesion.Registrar("no se pudo guardar conexion.cfg: " + ex.Message);
            }
        }

        // GuardarInstancia da permiso solo cuando crea la carpeta; aca se asegura tambien si ya existia.
        private static void PermitirModificarAUsuarios(string carpeta)
        {
            SecurityIdentifier usuarios = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            DirectorySecurity seguridad = Directory.GetAccessControl(carpeta);
            bool tiene = seguridad.GetAccessRules(true, true, typeof(SecurityIdentifier)).Cast<FileSystemAccessRule>()
                .Any(r => r.IdentityReference == usuarios && r.AccessControlType == AccessControlType.Allow &&
                          (r.FileSystemRights & FileSystemRights.Modify) == FileSystemRights.Modify);
            if (tiene) { return; }
            seguridad.AddAccessRule(new FileSystemAccessRule(usuarios, FileSystemRights.Modify,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(carpeta, seguridad);
        }

        private sealed class AvanceMsi : IProgress<AvanceInstalacion_575_AV>
        {
            private readonly SesionMsi_575_AV sesion;

            public AvanceMsi(SesionMsi_575_AV sesion)
            {
                this.sesion = sesion;
            }

            public void Report(AvanceInstalacion_575_AV avance)
            {
                if (!avance.terminado)
                {
                    // La pantalla de progreso solo muestra ActionText: cada script se informa como un inicio de accion.
                    sesion.IniciarAccion(ACCION_DIFERIDA,
                        "Ejecutando " + avance.script + " (" + avance.numero + " de " + avance.total + ")", string.Empty);
                    sesion.DatosAccion(avance.script, avance.numero, avance.total);
                    sesion.Registrar("ejecutando " + avance.script + " (" + avance.numero + " de " + avance.total + ")");
                }
                else
                {
                    sesion.AvanzarProgreso(PASOS_POR_SCRIPT * SCRIPTS_ESPERADOS / Math.Max(avance.total, 1));
                }
            }
        }
    }
}
