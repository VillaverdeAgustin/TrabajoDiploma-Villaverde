using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public class EventoBE
    {
		private int _registro;

		public int registro
		{
			get { return _registro; }
			set { _registro = value; }
		}

		private string _usuario;

		public string usuario
		{
			get { return _usuario; }
			set { _usuario = value; }
		}

		private TipoAccion _accion;

		public TipoAccion accion
		{
			get { return _accion; }
			set { _accion = value; }
		}

		private DateTime _fecha;

		public DateTime fecha
		{
			get { return _fecha; }
			set { _fecha = value; }
		}

		public EventoBE() { }

		public EventoBE CrearRegistro(string[] datos)
		{
			this.registro = Convert.ToInt32(datos[0]);
			this.usuario = datos[3];
			this.accion = (TipoAccion)Convert.ToInt32(datos[1]);
			this.fecha = Convert.ToDateTime(datos[2]);
			return this;
		}
	}
}
