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
    [Route("api/Adscgrps")]
    public class AdscgrpsController : Controller
    {
        private readonly SwSeguridadDbContext _context;

        public AdscgrpsController(SwSeguridadDbContext context)
        {
            _context = context;
        }

        // GET: api/Adscgrps
        [HttpGet]
        public IEnumerable<Adscgrp> GetAdscgrp()
        {
            return _context.Adscgrp;
        }

        // GET: api/Adscgrps/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdscgrp([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscgrp = await _context.Adscgrp.SingleOrDefaultAsync(m => m.AdgrBdd == id);

            if (adscgrp == null)
            {
                return NotFound();
            }

            return Ok(adscgrp);
        }

        // PUT: api/Adscgrps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdscgrp([FromRoute] string id, [FromBody] Adscgrp adscgrp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != adscgrp.AdgrBdd)
            {
                return BadRequest();
            }

            _context.Entry(adscgrp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdscgrpExists(id))
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

        // POST: api/Adscgrps
        [HttpPost]
        public async Task<IActionResult> PostAdscgrp([FromBody] Adscgrp adscgrp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Adscgrp.Add(adscgrp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdscgrpExists(adscgrp.AdgrBdd))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAdscgrp", new { id = adscgrp.AdgrBdd }, adscgrp);
        }

        // DELETE: api/Adscgrps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdscgrp([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adscgrp = await _context.Adscgrp.SingleOrDefaultAsync(m => m.AdgrBdd == id);
            if (adscgrp == null)
            {
                return NotFound();
            }

            _context.Adscgrp.Remove(adscgrp);
            await _context.SaveChangesAsync();

            return Ok(adscgrp);
        }

        private bool AdscgrpExists(string id)
        {
            return _context.Adscgrp.Any(e => e.AdgrBdd == id);
        }
    }
}