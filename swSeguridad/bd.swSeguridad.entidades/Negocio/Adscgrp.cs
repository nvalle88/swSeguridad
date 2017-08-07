using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscgrp
    {
        public Adscgrp()
        {
            Adscexe = new HashSet<Adscexe>();
            Adscmiem = new HashSet<Adscmiem>();
        }

        public string AdgrBdd { get; set; }
        public string AdgrGrupo { get; set; }
        public string AdgrNombre { get; set; }
        public string AdgrDescripcion { get; set; }

        public virtual ICollection<Adscexe> Adscexe { get; set; }
        public virtual ICollection<Adscmiem> Adscmiem { get; set; }
        public virtual Adscbdd AdgrBddNavigation { get; set; }
    }
}
