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
    [Route("api/Adscmenus")]
    public class AdscmenusController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public AdscmenusController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/Adscmenus
        [HttpGet]
        public IEnumerable<Adscmenu> GetAdscmenu()
        {
            return _context.Adscmenu;
        }

        // GET: api/Adscmenus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscmenu([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscmenu = await _context.Adscmenu.SingleOrDefaultAsync(m => m.AdmeSistema == id);

            if (adscmenu == null)
            {
                return NotFound();
            }

            return Ok(adscmenu);
        }

        // PUT: api/Adscmenus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscmenu([FromRoute] string id, [FromBody] Adscmenu adscmenu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscmenu.AdmeSistema)
            {
                return BadRequest();
            }

            _context.Entry(adscmenu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscmenuExists(id))
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

        // POST: api/Adscmenus
        [HttpPost]
        public async Task<IActionResult> PostAdscmenu([FromBody] Adscmenu adscmenu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscmenu.Add(adscmenu);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdscmenuExists(adscmenu.AdmeSistema))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAdscmenu", new { id = adscmenu.AdmeSistema }, adscmenu);
        }

        // DELETE: api/Adscmenus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscmenu([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscmenu = await _context.Adscmenu.SingleOrDefaultAsync(m => m.AdmeSistema == id);
            if (adscmenu == null)
            {
                return NotFound();
            }

            _context.Adscmenu.Remove(adscmenu);
            await _context.SaveChangesAsync();

            return Ok(adscmenu);
        }

        private bool AdscmenuExists(string id)
        {
            return _context.Adscmenu.Any(e => e.AdmeSistema == id);
        }
    }
}