﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace bd.swseguridad.entidades.Utils
{
    public  class Response
    {
        public  bool IsSuccess { get; set; }
        public  string Message { get; set; }
        public object Resultado { get; set; }
        public List<object> Resultados { get; set; } 


    }
}
