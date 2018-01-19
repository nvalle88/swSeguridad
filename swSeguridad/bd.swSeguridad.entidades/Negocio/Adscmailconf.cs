using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bd.swseguridad.entidades.Negocio
{
   
    public partial class Adscmailconf
    {
        [Key]
        public int AdcfTipo { get; set; }
        public string AdcfCorreo { get; set; }
        public string AdcfAsunto { get; set; }
    }
}
