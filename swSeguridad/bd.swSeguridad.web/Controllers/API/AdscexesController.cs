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
    [Route("api/Adscexes")]
    public class AdscexesController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public AdscexesController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/Adscexes
        [HttpGet]
        public IEnumerable<Adscexe> GetAdscexe()
        {
            return _context.Adscexe;
        }

        // GET: api/Adscexes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscexe([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscexe = await _context.Adscexe.SingleOrDefaultAsync(m => m.AdexBdd == id);

            if (adscexe == null)
            {
                return NotFound();
            }

            return Ok(adscexe);
        }

        // PUT: api/Adscexes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscexe([FromRoute] string id, [FromBody] Adscexe adscexe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscexe.AdexBdd)
            {
                return BadRequest();
            }

            _context.Entry(adscexe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscexeExists(id))
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

        // POST: api/Adscexes
        [HttpPost]
        public async Task<IActionResult> PostAdscexe([FromBody] Adscexe adscexe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscexe.Add(adscexe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdscexeExists(adscexe.AdexBdd))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAdscexe", new { id = adscexe.AdexBdd }, adscexe);
        }

        // DELETE: api/Adscexes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscexe([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscexe = await _context.Adscexe.SingleOrDefaultAsync(m => m.AdexBdd == id);
            if (adscexe == null)
            {
                return NotFound();
            }

            _context.Adscexe.Remove(adscexe);
            await _context.SaveChangesAsync();

            return Ok(adscexe);
        }

        private bool AdscexeExists(string id)
        {
            return _context.Adscexe.Any(e => e.AdexBdd == id);
        }
    }
}