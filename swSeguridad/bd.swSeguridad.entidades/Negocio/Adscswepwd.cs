using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscswepwd
    {
        public string AdpsLogin { get; set; }
        public string AdseSw { get; set; }
        public string AdspObs { get; set; }

        public virtual Adscpassw AdpsLoginNavigation { get; set; }
        public virtual Adscswext AdseSwNavigation { get; set; }
    }
}
