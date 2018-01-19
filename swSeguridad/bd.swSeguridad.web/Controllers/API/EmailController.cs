using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bd.swseguridad.entidades.Negocio;
using bd.swseguridad.entidades.Utils;
using bd.swseguridad.datos;
using Microsoft.EntityFrameworkCore;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Email")]
    public class EmailController : Controller
    {

        private readonly SwSeguridadDbContext db;

        public EmailController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Route("obtenerCorreo")]
        public async Task<Response>  GetMenusDistinct([FromBody] Adscmailconf adscmailconf)
        {
            try
            {

                var config = await db.Adscmailconf.Where(x => x.AdcfTipo == adscmailconf.AdcfTipo).FirstOrDefaultAsync();
                if (config == null)
                {
                    return new Response { IsSuccess = false };
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Resultado = config
                    };
                }

            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false, Message = Mensaje.Excepcion };
                throw;
            }
        }
    }
}