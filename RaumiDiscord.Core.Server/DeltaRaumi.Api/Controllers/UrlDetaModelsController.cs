using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;

namespace RaumiDiscord.Core.Server.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlDetaModelsController : ControllerBase
    {
        private readonly DeltaRaumiDbContext _context;

        public UrlDetaModelsController(DeltaRaumiDbContext context)
        {
            _context = context;
        }

        // GET: api/UrlDetaModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlDetaModel>>> GeturlDetaModels()
        {
            return await _context.UrlDetaModels.ToListAsync();
        }

        // GET: api/UrlDetaModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UrlDetaModel>> GetUrlDetaModel(uint id)
        {
            var urlDetaModel = await _context.UrlDetaModels.FindAsync(id);

            if (urlDetaModel == null)
            {
                return NotFound();
            }

            return urlDetaModel;
        }

        // PUT: api/UrlDetaModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUrlDetaModel(uint id, UrlDetaModel urlDetaModel)
        {
            if (id != urlDetaModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(urlDetaModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrlDetaModelExists(id))
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

        // POST: api/UrlDetaModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UrlDetaModel>> PostUrlDetaModel(UrlDetaModel urlDetaModel)
        {
            _context.UrlDetaModels.Add(urlDetaModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUrlDetaModel", new { id = urlDetaModel.Id }, urlDetaModel);
        }

        // DELETE: api/UrlDetaModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrlDetaModel(uint id)
        {
            var urlDetaModel = await _context.UrlDetaModels.FindAsync(id);
            if (urlDetaModel == null)
            {
                return NotFound();
            }

            _context.UrlDetaModels.Remove(urlDetaModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UrlDetaModelExists(uint id)
        {
            return _context.UrlDetaModels.Any(e => e.Id == id);
        }
    }
}
