using System;

using Microsoft.AspNetCore.Http;


namespace bd.swseguridad.entidades.ObjectTranfer
{
  public  class PermisoUsuario
    {
        public string Contexto { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
    }
}
