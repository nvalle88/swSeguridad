using System;
using System.Collections.Generic;
using System.Text;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adsctoken
    {
        public string AdstSistema { get; set; }
        public string AdpsLogin { get; set; }
        public string AdtoToken { get; set; }

        public virtual Adscpassw AdpsLoginNavigation { get; set; }
        public virtual Adscsist AdstSistemaNavigation { get; set; }
    }
}
