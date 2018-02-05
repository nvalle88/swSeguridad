using System;
using System.Collections.Generic;
using System.Text;

namespace bd.swseguridad.entidades.Utils
{
  public static class Mensaje
    {
        public static string Excepcion { get { return "Ha ocurrido una Excepción"; } }
        public static string CambioContrasenaExito { get { return "La contraseña se ha cambiado exitosamente"; } }
        public static string NoHabilitadoCambioContrasena { get { return "Los Usuarios internos no pueden cambiar la contraseña en el sistema"; } }
        public static string UsuarioBloqueado { get { return "Usuario Bloqueado contacte con el administrador"; } }
        public static string UsuarioCaducado { get { return "Usuario Caducado contacte con el administrador"; } }
        public static string UsuariooContrasenaIncorrecto { get { return "Usuario o contraseña incorrecto !!!"; } }
        public static string ExisteRegistro { get { return "Existe un registro de igual información"; } }
        public static string Satisfactorio { get { return "La acción se ha realizado satisfactoriamente"; } }
        public static string Error { get { return "Ha ocurrido error inesperado"; } }
        public static string UsuarioSinConfirmar { get { return "Usuario sin Confirmar"; } }
        public static string RegistroNoEncontrado { get { return "El registro solicitado no se ha encontrado"; } }
        public static string ModeloInvalido { get { return "El Módelo es inválido"; } }
        public static string BorradoNoSatisfactorio { get { return "No es posible eliminar el registro, existen relaciones que dependen de él"; } }
    }
}
