using System;

using Microsoft.AspNetCore.Http;


namespace bd.swseguridad.entidades.ObjectTranfer
{
  public  class PermisoUsuarioSwExternos
    {
        public string Uri { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string Aplicacion { get; set; }
        public object parametros { get; set; }
    }
}
