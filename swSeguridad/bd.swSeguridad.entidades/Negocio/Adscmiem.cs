using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscmiem
    {
        [Key]
        public string AdmiEmpleado { get; set; }
        [Key]
        public string AdmiGrupo { get; set; }
        [Key]
        public string AdmiBdd { get; set; }


        public string AdmiTotal { get; set; }

        public string AdmiCodigoEmpleado { get; set; }

        public virtual Adscgrp Admi { get; set; }
    }
}
