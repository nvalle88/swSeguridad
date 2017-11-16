using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscmenu
    {
        public Adscmenu()
        {
            Adscexe = new HashSet<Adscexe>();
        }

        public string AdmeSistema { get; set; }
        public string AdmeAplicacion { get; set; }
        public string AdmeTipo { get; set; }
        public string AdmePadre { get; set; }
        public string AdmeObjetivo { get; set; }
        public string AdmeDescripcion { get; set; }
        public int? AdmeOrden { get; set; }
        public string AdmeTipoObjeto { get; set; }
        public string AdmeUrl { get; set; }
        public string AdmeEnsamblado { get; set; }
        public string AdmeElemento { get; set; }
        public string AdmeEstado { get; set; }
        public string AdmeControlador { get; set; }
        public string AdmeAccionControlador { get; set; }

        public virtual ICollection<Adscexe> Adscexe { get; set; }
        public virtual Adscsist AdmeSistemaNavigation { get; set; }
    }
}
