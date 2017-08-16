using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;
using bd.swseguridad.entidades.Utils;
using bd.swseguridad.entidades.ObjectTranfer;
using bd.log.guardar.Servicios;
using bd.swseguridad.entidades.Enumeradores;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Enumeradores;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Adscpassws")]
    public class AdscpasswsController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscpasswsController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscpassws
        [HttpGet]
        public IEnumerable<Adscpassw> GetAdscpassw()
        {
            return db.Adscpassw;
        }

        [Route("Login")]
        public async Task<Response> Login([FromBody] Login login)
        {

            try
            {


                var usuario = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No existe"
                    };
                }

                if (usuario.AdpsFechaVencimiento<DateTime.Now)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Usuario Caducado"
                    };
                }

                if (usuario.AdpsIntentos>=3)
                {
                    return new Response
                    {
                        IsSuccess=false,
                        Message="Usuario Bloqueado contacte con el administrador"
                    }; 
                }

               
                
                var existeLogin = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario && x.AdpsPassword == login.Contrasena).FirstOrDefault();
                if (existeLogin == null)
                {
                    usuario.AdpsIntentos = usuario.AdpsIntentos +1;
                    db.Entry(usuario).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No existe",
                        Resultado = "",
                    };
                }
                return new Response
                {
                    IsSuccess=true,
                    Message="Ok",
                    Resultado=existeLogin,
                };
            }
            catch (Exception ex)
            {

                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName =Convert.ToString(Aplicacion.Seguridad),
                    ExceptionTrace=ex,
                    LogCategoryParametre=Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName=Convert.ToString(LogLevelParameter.ERR),
                    Message=ex.Message,
                    UserName=login.Usuario,
                    
                },new Uri(""),"");
                return new Response
                {
                    IsSuccess=false,
                    Message=ex.Message,

                };
                throw;
            }


        }

        // GET: api/Adscpassws/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscpassw([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscpassw = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);

            if (adscpassw == null)
            {
                return NotFound();
            }

            return Ok(adscpassw);
        }

        // PUT: api/Adscpassws/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscpassw([FromRoute] string id, [FromBody] Adscpassw adscpassw)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscpassw.AdpsLogin)
            {
                return BadRequest();
            }

            db.Entry(adscpassw).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscpasswExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Adscpassws
        [HttpPost]
        public async Task<IActionResult> PostAdscpassw([FromBody] Adscpassw adscpassw)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Adscpassw.Add(adscpassw);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetAdscpassw", new { id = adscpassw.AdpsLogin }, adscpassw);
        }

        // DELETE: api/Adscpassws/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscpassw([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscpassw = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);
            if (adscpassw == null)
            {
                return NotFound();
            }

            db.Adscpassw.Remove(adscpassw);
            await db.SaveChangesAsync();

            return Ok(adscpassw);
        }

        private bool AdscpasswExists(string id)
        {
            return db.Adscpassw.Any(e => e.AdpsLogin == id);
        }
    }
}