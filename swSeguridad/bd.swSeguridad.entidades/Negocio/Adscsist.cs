using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscsist
    {
        public Adscsist()
        {
            Adscmenu = new HashSet<Adscmenu>();
        }

        public string AdstSistema { get; set; }
        public string AdstDescripcion { get; set; }
        public string AdstTipo { get; set; }
        public string AdstHost { get; set; }
        public string AdstBdd { get; set; }

        public virtual ICollection<Adscmenu> Adscmenu { get; set; }
        public virtual Adscbdd AdstBddNavigation { get; set; }
    }
}
