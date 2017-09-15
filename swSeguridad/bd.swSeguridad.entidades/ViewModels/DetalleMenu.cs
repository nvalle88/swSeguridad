using bd.swseguridad.entidades.Negocio;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.ViewModels
{
    public class DetalleMenu
    {
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
        public List<Adscmenu> ListaHijos { get; set; }
    }
}
