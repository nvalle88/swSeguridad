using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/BasesDatos")]
    public class BasesDatosController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public BasesDatosController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/BasesDatos
        [HttpGet]
        [Route("ListarBasesDatos")]
        public IEnumerable<Adscbdd> GetAdscbdd()
        {
            return _context.Adscbdd;
        }

        // GET: api/BasesDatos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscbdd([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscbdd = await _context.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == id);

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

            _context.Entry(adscbdd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> PostAdscbdd([FromBody] Adscbdd adscbdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscbdd.Add(adscbdd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdscbdd", new { id = adscbdd.AdbdBdd }, adscbdd);
        }

        // DELETE: api/BasesDatos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscbdd([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscbdd = await _context.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == id);
            if (adscbdd == null)
            {
                return NotFound();
            }

            _context.Adscbdd.Remove(adscbdd);
            await _context.SaveChangesAsync();

            return Ok(adscbdd);
        }

        private bool AdscbddExists(string id)
        {
            return _context.Adscbdd.Any(e => e.AdbdBdd == id);
        }
    }
}