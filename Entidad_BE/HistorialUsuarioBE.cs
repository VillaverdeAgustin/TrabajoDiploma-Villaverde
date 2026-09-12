using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public class HistorialUsuarioBE
    {

        private int _idHistorial;

        public int idHistorial
        {
            get { return _idHistorial; }
            set { _idHistorial = value; }
        }

        private int _usuarioId;

        public int usuarioId
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        private string _nombre;

		public string nombre
		{
			get { return _nombre; }
			set { _nombre = value; }
		}

		private string _apellido;

		public string apellido
		{
			get { return _apellido; }
			set { _apellido = value; }
		}

		private string _dni;

		public string dni
		{
			get { return _dni; }
			set { _dni = value; }
		}

		private string _usuario;

		public string usuario
		{
			get { return _usuario; }
			set { _usuario = value; }
		}

		private string _rol;

		public string rol
		{
			get { return _rol; }
			set { _rol = value; }
		}

		private string _direccion;

		public string direccion
		{
			get { return _direccion; }
			set { _direccion = value; }
		}

		private string _telefono;

		public string telefono
		{
			get { return _telefono; }
			set { _telefono = value; }
		}

		private string _email;

		public string email
		{
			get { return _email; }
			set { _email = value; }
		}

		private bool _activo;

		public bool activo
		{
			get { return _activo; }
			set { _activo = value; }
		}

		private bool _bloqueado;

		public bool bloqueado
		{
			get { return _bloqueado; }
			set { _bloqueado = value; }
		}

        private TipoAccion _accion;

        public TipoAccion accion
        {
            get { return _accion; }
            set { _accion = value; }
        }

        private string _usuarioResponsable;

        public string usuarioResponsable
        {
            get { return _usuarioResponsable; }
            set { _usuarioResponsable = value; }
        }

        private DateTime _fecha;

        public DateTime fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }
    }
}
