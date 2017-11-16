using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscmiem
    {
        public string AdmiEmpleado { get; set; }
        public string AdmiGrupo { get; set; }
        public string AdmiBdd { get; set; }
        public string AdmiTotal { get; set; }
        public string AdmiCodigoEmpleado { get; set; }

        public virtual Adscgrp Admi { get; set; }
    }
}
