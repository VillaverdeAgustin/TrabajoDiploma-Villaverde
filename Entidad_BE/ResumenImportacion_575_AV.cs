using System.Collections.Generic;
using System.Linq;

namespace Entidad_BE
{
    public class ResumenImportacion_575_AV
    {
        public int id_lista_precio { get; set; }
        public string archivo { get; set; }
        public List<ItemListaPrecioBE_575_AV> renglones { get; set; }

        public ResumenImportacion_575_AV()
        {
            renglones = new List<ItemListaPrecioBE_575_AV>();
        }

        public List<ItemListaPrecioBE_575_AV> validos
        {
            get { return renglones.Where(r => r.resultado != ResultadoImportacion_575_AV.Rechazado).ToList(); }
        }

        public int actualizados
        {
            get { return renglones.Count(r => r.resultado == ResultadoImportacion_575_AV.Actualizado); }
        }

        public int incorporados
        {
            get { return renglones.Count(r => r.resultado == ResultadoImportacion_575_AV.Incorporado); }
        }

        public int rechazados
        {
            get { return renglones.Count(r => r.resultado == ResultadoImportacion_575_AV.Rechazado); }
        }
    }
}
