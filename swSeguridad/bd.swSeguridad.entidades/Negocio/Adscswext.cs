using System;
using System.Collections.Generic;
using System.Text;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscswext
    {
        public Adscswext()
        {
            Adscswepwd = new HashSet<Adscswepwd>();
        }

        public string AdseSw { get; set; }
        public string AdseDesc { get; set; }
        public string AdseUri { get; set; }

        public virtual ICollection<Adscswepwd> Adscswepwd { get; set; }
    }
}
