using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bd.swseguridad.entidades.Utils;

namespace bd.swseguridad.web.Controllers.API
{
    /// <summary>
    /// Controladores API de los servicios web estos son los que trabajan directamente con la base de datos
    /// Tiene algunos aspectos que explicar como:
    /// [Produces("application/json")]: filtro especifica los formatos de 
    /// respuesta para una acción específica (o controlador). 
    /// Al igual que la mayoría de los filtros , 
    /// esto se puede aplicar en la acción, el controlador o el alcance global.
    /// para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting
    /// [Route("api/Codificar")]:es la ruta del recurso del controlador en general
    /// public class NombreController : Controller
    /// [Route("Nombre")] en los métodos es la ruta del recurso.
    /// para acceder a estos recursos es: Host + ruta concatenados 
    /// , ruta del controlador + ruta del método
    /// [FromBody] es para capturar el objeto que se envia en el body 
    /// para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
    /// </summary>
    [Produces("application/json")]
    [Route("api/Codificar")]
    public class CodificarController : Controller
    {
        /// <summary>
        /// Servicio web para realizar el hash de una entrada determinada
        /// </summary>
        /// <param name="codificar">
        /// Objeto que tiene un atributo entrada que es donde se introduce lo que se desea codificar.
        /// y el atributo salida que es donde se obtiene la salida codificada.
        /// </param>
        /// <returns></returns>
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
