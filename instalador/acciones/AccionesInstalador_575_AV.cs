using Acceso_DAL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private const int SCRIPTS_ESPERADOS = 13;
        private const string INSTANCIA_EXPRESS = @".\SQLEXPRESS";
        private const string ENSAMBLADO_NEGOCIO = "Negocio_BLL.dll";
        private const string CLASE_INTEGRIDAD = "Negocio_BLL.VerificadorIntegridadBLL";

        // Interfaz: llena el combo con las instancias locales y propone la ya configurada, .\SQLEXPRESS o la primera.
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
                        ? "Conexión correcta. ConectAR_DB ya existe: se respalda y se actualiza conservando sus datos."
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
        // Guarda conexion.cfg y crea ConectAR_DB, o la actualiza si ya existe. Nunca hace fallar la instalacion.
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
                    ActualizarBase(sesion, instalador, instancia, Path.GetDirectoryName(carpeta));
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

        // Reinstalacion: respaldo, scripts sin datos de prueba y recalculo de DVH/DVV con la BLL ya instalada.
        // Si algo falla se restaura el respaldo, asi la base queda como estaba.
        private static void ActualizarBase(SesionMsi_575_AV sesion, InstaladorBaseDatos_575_AV instalador, string instancia, string carpetaPrograma)
        {
            sesion.Registrar("ConectAR_DB ya existe en " + instancia + ": se actualiza conservando los datos");
            string respaldo;
            try
            {
                respaldo = instalador.RespaldarBase(instancia);
                sesion.Registrar("respaldo previo a la actualizacion: " + respaldo);
            }
            catch (Exception ex)
            {
                sesion.Registrar("no se pudo respaldar ConectAR_DB: " + ex.Message);
                sesion.Avisar("No se pudo respaldar la base de datos ConectAR_DB, así que no se actualizó. La instalación continúa; " +
                              "vuelva a ejecutar el instalador cuando el respaldo sea posible.");
                return;
            }

            try
            {
                sesion.IniciarAccion(ACCION_DIFERIDA, "Actualizando la base de datos ConectAR_DB", "Ejecutando [1] ([2] de [3])");
                instalador.ActualizarBase(instancia, new AvanceMsi(sesion));
                List<string> falladas = RecalcularDigitosVerificadores(carpetaPrograma);
                if (falladas.Count > 0)
                {
                    throw new InvalidOperationException("tablas con DV incorrecto despues del recalculo: " + string.Join(", ", falladas));
                }
                sesion.Registrar("ConectAR_DB actualizada en " + instancia + "; DVH y DVV recalculados, integridad verificada");
            }
            catch (Exception ex)
            {
                Exception causa = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                sesion.Registrar("fallo la actualizacion de la base: " + causa.Message);
                try
                {
                    instalador.RestaurarRespaldo(instancia, respaldo);
                    sesion.Registrar("se restauro el respaldo " + respaldo);
                    sesion.Avisar("No se pudo actualizar la base de datos ConectAR_DB y se restauró como estaba antes de la instalación.");
                }
                catch (Exception restauracion)
                {
                    sesion.Registrar("no se pudo restaurar el respaldo: " + restauracion.Message);
                    sesion.Avisar("No se pudo actualizar la base de datos ConectAR_DB. Sus datos están en el respaldo " + respaldo +
                                  ", que se puede restaurar desde ConectAR (Backup y restore).");
                }
            }
        }

        // Mismo recalculo que dispara el restore (T07): VerificadorIntegridadBLL del programa instalado
        private static List<string> RecalcularDigitosVerificadores(string carpetaPrograma)
        {
            Assembly negocio = Assembly.LoadFrom(Path.Combine(carpetaPrograma, ENSAMBLADO_NEGOCIO));
            Type tipo = negocio.GetType(CLASE_INTEGRIDAD, true);
            object integridad = Activator.CreateInstance(tipo);
            tipo.GetMethod("RecalcularDVTotal").Invoke(integridad, null);
            return ((IEnumerable<string>)tipo.GetMethod("VerificarIntegridadTotal").Invoke(integridad, null)).ToList();
        }

        private static string InstanciaPropuesta(List<string> instancias)
        {
            string configurada = ConfiguracionConexion_575_AV.ObtenerInstancia();
            if (!string.IsNullOrWhiteSpace(configurada)) { return configurada; }
            string express = instancias.FirstOrDefault(i => string.Equals(i, INSTANCIA_EXPRESS, StringComparison.OrdinalIgnoreCase));
            if (express != null) { return express; }
            return instancias.Count > 0 ? instancias[0] : INSTANCIA_EXPRESS;
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
