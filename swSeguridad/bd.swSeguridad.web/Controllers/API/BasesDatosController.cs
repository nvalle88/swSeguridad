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

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/BasesDatos")]
    public class BasesDatosController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public BasesDatosController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/BasesDatos
        [HttpGet]
        [Route("ListarBasesDatos")]
        public IEnumerable<Adscbdd> GetAdscbdd()
        {
            return db.Adscbdd;
        }

        // GET: api/BasesDatos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscbdd([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscbdd = await db.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == id);

            if (adscbdd == null)
            {
                return NotFound();
            }

            return Ok(adscbdd);
        }

        // PUT: api/BasesDatos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscbdd([FromRoute] string id, [FromBody] Adscbdd adscbdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscbdd.AdbdBdd)
            {
                return BadRequest();
            }

            db.Entry(adscbdd).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscbddExists(id))
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

        // POST: api/BasesDatos
        [HttpPost]
        [Route("InsertarBaseDatos")]
        public async Task<Response> PostAdscbdd([FromBody] Adscbdd adscbdd)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess=false,
                        Message="Módelo inválido"
                    };
                }

                var respuesta = Existe(adscbdd);
                if (!respuesta.IsSuccess)
                {
                    db.Adscbdd.Add(adscbdd);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "OK"
                    };
                }
               
                return new Response
                {
                    IsSuccess = false,
                    Message = "OK"
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };

                throw;
            }
        }

        // DELETE: api/BasesDatos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscbdd([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscbdd = await db.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == id);
            if (adscbdd == null)
            {
                return NotFound();
            }

            db.Adscbdd.Remove(adscbdd);
            await db.SaveChangesAsync();

            return Ok(adscbdd);
        }

        private bool AdscbddExists(string id)
        {
            return db.Adscbdd.Any(e => e.AdbdBdd == id);
        }

        public Response Existe(Adscbdd adscbdd)
        {
            var bdd = adscbdd.AdbdBdd.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscbdd.Where(p => p.AdbdBdd.ToUpper().TrimStart().TrimEnd() == bdd).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Existe una base de datos de igual nombre",
                    Resultado = null,
                };

            }

            return new Response
            {
                IsSuccess = false,
                Resultado = loglevelrespuesta,
            };
        }
    }
}