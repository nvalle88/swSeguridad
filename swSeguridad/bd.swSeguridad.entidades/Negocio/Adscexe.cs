using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.Negocio
{
    public partial class Adscexe
    {
        public string AdexBdd { get; set; }
        public string AdexGrupo { get; set; }
        public string AdexSistema { get; set; }
        public string AdexAplicacion { get; set; }
        public string AdexSql { get; set; }
        public bool? Ins { get; set; }
        public bool? Sel { get; set; }
        public bool? Upd { get; set; }
        public bool? Del { get; set; }

        public virtual Adscgrp Adex { get; set; }
        public virtual Adscmenu AdexNavigation { get; set; }
    }
}
