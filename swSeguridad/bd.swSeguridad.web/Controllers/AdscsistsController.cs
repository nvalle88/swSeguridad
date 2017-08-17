using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;

namespace bd.swseguridad.web.Controllers
{
    [Produces("application/json")]
    [Route("api/Adscsists")]
    public class AdscsistsController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public AdscsistsController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/Adscsists
        [HttpGet]
        public IEnumerable<Adscsist> GetAdscsist()
        {
            return _context.Adscsist;
        }

        // GET: api/Adscsists/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscsist([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscsist = await _context.Adscsist.SingleOrDefaultAsync(m => m.AdstSistema == id);

            if (adscsist == null)
            {
                return NotFound();
            }

            return Ok(adscsist);
        }

        // PUT: api/Adscsists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscsist([FromRoute] string id, [FromBody] Adscsist adscsist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscsist.AdstSistema)
            {
                return BadRequest();
            }

            _context.Entry(adscsist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscsistExists(id))
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

        // POST: api/Adscsists
        [HttpPost]
        public async Task<IActionResult> PostAdscsist([FromBody] Adscsist adscsist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscsist.Add(adscsist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdscsist", new { id = adscsist.AdstSistema }, adscsist);
        }

        // DELETE: api/Adscsists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscsist([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscsist = await _context.Adscsist.SingleOrDefaultAsync(m => m.AdstSistema == id);
            if (adscsist == null)
            {
                return NotFound();
            }

            _context.Adscsist.Remove(adscsist);
            await _context.SaveChangesAsync();

            return Ok(adscsist);
        }

        private bool AdscsistExists(string id)
        {
            return _context.Adscsist.Any(e => e.AdstSistema == id);
        }
    }
}