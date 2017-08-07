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
    [Route("api/Adscmiems")]
    public class AdscmiemsController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public AdscmiemsController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/Adscmiems
        [HttpGet]
        public IEnumerable<Adscmiem> GetAdscmiem()
        {
            return _context.Adscmiem;
        }

        // GET: api/Adscmiems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscmiem([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscmiem = await _context.Adscmiem.SingleOrDefaultAsync(m => m.AdmiEmpleado == id);

            if (adscmiem == null)
            {
                return NotFound();
            }

            return Ok(adscmiem);
        }

        // PUT: api/Adscmiems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscmiem([FromRoute] string id, [FromBody] Adscmiem adscmiem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscmiem.AdmiEmpleado)
            {
                return BadRequest();
            }

            _context.Entry(adscmiem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscmiemExists(id))
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

        // POST: api/Adscmiems
        [HttpPost]
        public async Task<IActionResult> PostAdscmiem([FromBody] Adscmiem adscmiem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscmiem.Add(adscmiem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdscmiemExists(adscmiem.AdmiEmpleado))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAdscmiem", new { id = adscmiem.AdmiEmpleado }, adscmiem);
        }

        // DELETE: api/Adscmiems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscmiem([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscmiem = await _context.Adscmiem.SingleOrDefaultAsync(m => m.AdmiEmpleado == id);
            if (adscmiem == null)
            {
                return NotFound();
            }

            _context.Adscmiem.Remove(adscmiem);
            await _context.SaveChangesAsync();

            return Ok(adscmiem);
        }

        private bool AdscmiemExists(string id)
        {
            return _context.Adscmiem.Any(e => e.AdmiEmpleado == id);
        }
    }
}