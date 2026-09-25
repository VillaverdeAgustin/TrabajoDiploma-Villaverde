using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad_BE
{
    public enum LoginResult
    {
        UserInexistente = 0,
        LoginOK = 1,
        UserBloqueado = 2,
        PassIncorrecta = 3,
        UserInactivo = 4,
        FinIntentos = 5,
        SesionIniciada = 6,
        ExisteSesion = 7
    }
}
