using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscpassw
    {
        public string AdpsLogin { get; set; }
        public string AdpsPassword { get; set; }
        public DateTime? AdpsFechaCambio { get; set; }
        public DateTime? AdpsFechaVencimiento { get; set; }
        public string AdpsLoginAdm { get; set; }
        public int? AdpsIntentos { get; set; }
        public string AdpsPasswCg { get; set; }
        public string AdpsTipoUso { get; set; }
        public string AdpsIdContacto { get; set; }
        public string AdpsIdEntidad { get; set; }
        public string AdpsPreguntaRecuperacion { get; set; }
        public string AdpsRespuestaRecuperacion { get; set; }
        public string AdpsCodigoEmpleado { get; set; }
    }
}
