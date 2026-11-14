using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Acceso_DAL
{
    public static class ConfiguracionConexion_575_AV
    {
        public const string BASE_DATOS = "ConectAR_DB";

        private const string CARPETA = "ConectAR_575_AV";
        private const string ARCHIVO = "conexion.cfg";
        private const string CLAVE_INSTANCIA = "instancia";
        private const string CLAVE_REGISTRO = @"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL";
        private const string INSTANCIA_PREDETERMINADA = "MSSQLSERVER";

        private static readonly object candado = new object();
        private static string instancia;
        private static bool leida;

        public static string RutaArchivo
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CARPETA, ARCHIVO);
            }
        }

        public static string ObtenerInstancia()
        {
            lock (candado)
            {
                if (!leida)
                {
                    instancia = Leer();
                    leida = true;
                }
                return instancia;
            }
        }

        public static bool HayInstanciaConfigurada()
        {
            return !string.IsNullOrWhiteSpace(ObtenerInstancia());
        }

        public static void GuardarInstancia(string nueva)
        {
            if (string.IsNullOrWhiteSpace(nueva)) { throw new ArgumentException("instancia"); }

            string carpeta = Path.GetDirectoryName(RutaArchivo);
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
                PermitirModificarAUsuarios(carpeta);
            }

            File.WriteAllText(RutaArchivo, CLAVE_INSTANCIA + "=" + nueva.Trim() + Environment.NewLine, new UTF8Encoding(false));

            lock (candado)
            {
                instancia = nueva.Trim();
                leida = true;
            }
        }

        public static string CadenaConexion()
        {
            string actual = ObtenerInstancia();
            if (string.IsNullOrWhiteSpace(actual))
            {
                throw new InvalidOperationException("CONEX_ERR_SIN_CONFIGURAR");
            }
            return ArmarCadena(actual, BASE_DATOS, null);
        }

        public static string ArmarCadena(string servidor, string baseDatos, int? segundosEspera)
        {
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder
            {
                DataSource = servidor,
                InitialCatalog = baseDatos,
                IntegratedSecurity = true,
                TrustServerCertificate = true
            };
            if (segundosEspera.HasValue) { cadena.ConnectTimeout = segundosEspera.Value; }
            return cadena.ConnectionString;
        }

        public static List<string> ListarInstanciasLocales()
        {
            HashSet<string> nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (RegistryView vista in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (RegistryKey raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, vista))
                    using (RegistryKey clave = raiz.OpenSubKey(CLAVE_REGISTRO))
                    {
                        if (clave == null) { continue; }
                        foreach (string nombre in clave.GetValueNames()) { nombres.Add(nombre); }
                    }
                }
                catch (System.Security.SecurityException) { }
                catch (UnauthorizedAccessException) { }
            }

            return nombres
                .OrderBy(n => string.Equals(n, INSTANCIA_PREDETERMINADA, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(n => n, StringComparer.OrdinalIgnoreCase)
                .Select(n => string.Equals(n, INSTANCIA_PREDETERMINADA, StringComparison.OrdinalIgnoreCase) ? "localhost" : @".\" + n)
                .ToList();
        }

        private static string Leer()
        {
            try
            {
                if (!File.Exists(RutaArchivo)) { return null; }

                foreach (string linea in File.ReadAllLines(RutaArchivo, Encoding.UTF8))
                {
                    int igual = linea.IndexOf('=');
                    if (igual > 0 && string.Equals(linea.Substring(0, igual).Trim(), CLAVE_INSTANCIA, StringComparison.OrdinalIgnoreCase))
                    {
                        string valor = linea.Substring(igual + 1).Trim();
                        return valor.Length == 0 ? null : valor;
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return null;
        }

        private static void PermitirModificarAUsuarios(string carpeta)
        {
            try
            {
                DirectorySecurity seguridad = Directory.GetAccessControl(carpeta);
                seguridad.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));
                Directory.SetAccessControl(carpeta, seguridad);
            }
            catch (UnauthorizedAccessException) { }
            catch (InvalidOperationException) { }
        }
    }
}
