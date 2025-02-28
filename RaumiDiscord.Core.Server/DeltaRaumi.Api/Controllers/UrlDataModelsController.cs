using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlDataModelsController : ControllerBase
    {
        private readonly DeltaRaumiDbContext _context;

        public UrlDataModelsController(DeltaRaumiDbContext context)
        {
            _context = context;
        }

        // GET: api/UrlDataModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlDataModel>>> GetUrlDataModels()
        {
            return await _context.UrlDataModels.ToListAsync();
        }

        // GET: api/UrlDataModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UrlDataModel>> GetUrlDataModel(uint id)
        {
            var urlDataModel = await _context.UrlDataModels.FindAsync(id);

            if (urlDataModel == null)
            {
                return NotFound();
            }

            return urlDataModel;
        }

        // PUT: api/UrlDataModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUrlDataModel(uint id, UrlDataModel urlDataModel)
        {
            if (id != urlDataModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(urlDataModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrlDataModelExists(id))
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

        // POST: api/UrlDataModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UrlDataModel>> PostUrlDataModel(UrlDataModel urlDataModel)
        {
            _context.UrlDataModels.Add(urlDataModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUrlDataModel", new { id = urlDataModel.Id }, urlDataModel);
        }

        // DELETE: api/UrlDataModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrlDataModel(uint id)
        {
            var urlDataModel = await _context.UrlDataModels.FindAsync(id);
            if (urlDataModel == null)
            {
                return NotFound();
            }

            _context.UrlDataModels.Remove(urlDataModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UrlDataModelExists(uint id)
        {
            return _context.UrlDataModels.Any(e => e.Id == id);
        }
    }
}
