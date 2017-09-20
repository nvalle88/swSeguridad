using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bd.swseguridad.entidades.ObjectTranfer
{
   public class DetallePermisoGrupo
    {
        [Display(Name = "Base de datos")]
        public string Adgrbdd { get; set; }

        [Display(Name = "Grupo")]
        public string Adgrgrupo { get; set; }

        [Display(Name = "Sistema")]
        public string AdmeSistema { get; set; }
        [Display(Name = "Aplicación")]
        public string AdmeAplicacion { get; set; }
        [Display(Name = "Tipo")]
        public string AdmeTipo { get; set; }
        [Display(Name = "Menú Padre")]
        public string AdmePadre { get; set; }
        [Display(Name = "Objetivo")]
        public string AdmeObjetivo { get; set; }
        [Display(Name = "Descripción")]
        public string AdmeDescripcion { get; set; }
        [Display(Name = "Orden")]
        public int? AdmeOrden { get; set; }
        [Display(Name = "Tipo de objeto")]
        public string AdmeTipoObjeto { get; set; }
        [Display(Name = "URL")]
        public string AdmeUrl { get; set; }
        [Display(Name = "Ensamblado")]
        public string AdmeEnsamblado { get; set; }
        [Display(Name = "Elemento")]
        public string AdmeElemento { get; set; }
        [Display(Name = "Estado")]
        public string AdmeEstado { get; set; }
       
    }
}
