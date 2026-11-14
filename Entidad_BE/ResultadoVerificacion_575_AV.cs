namespace Entidad_BE
{
    // A03 - Una verificacion de la serializacion o la deserializacion. El texto se arma en pantalla con la
    // clave de traduccion y sus argumentos, asi sigue al idioma activo; detalle lleva solo datos.
    public class ResultadoVerificacion_575_AV
    {
        public string clave { get; set; }
        public string[] argumentos { get; set; }
        public bool correcto { get; set; }
        public string detalle { get; set; }

        public ResultadoVerificacion_575_AV()
        {
            argumentos = new string[0];
        }
    }
}
