using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bd.swseguridad.entidades.Utils;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Codificar")]
    public class CodificarController : Controller
    {
        [HttpPost]
        [Route("SHA512")]
        public  Codificar SHA512([FromBody]Codificar codificar)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(codificar.Entrada);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(256);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                var resultado = new Codificar
                {
                    Entrada = codificar.Entrada,
                    Salida = hashedInputStringBuilder.ToString(),
                };

                return resultado;
            }; 
        }
    }
}
