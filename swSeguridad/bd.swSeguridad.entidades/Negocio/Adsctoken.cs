using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adsctoken
    {
        public int AdtoId { get; set; }
        public string AdpsLogin { get; set; }
        public string AdtoToken { get; set; }
        public string AdtoNombreServicio { get; set; }
    }
}
