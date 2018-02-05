using System;
using System.Collections.Generic;
using System.Text;

namespace bd.swseguridad.entidades.ViewModels
{
  public class CambiarContrasenaViewModel
    {
        public string Usuario { get; set; }
        public string ContrasenaActual { get; set; }
        public string NuevaContrasena { get; set; }
        public string ConfirmacionContrasena { get; set; }
    }
}
