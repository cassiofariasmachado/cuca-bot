using CucaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CucaAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Cucas")]
    public class CucasController : Controller
    {
        private readonly CucaContext _context;

        public CucasController(CucaContext context)
        {
            _context = context;
        }

        // GET: api/Cucas
        [HttpGet]
        public IEnumerable<Cuca> GetCuca()
        {
            return _context.Cuca;
        }

        // GET: api/Cucas/next
        [HttpGet]
        [Route("next")]
        public IEnumerable<Cuca> GetCuca([FromQuery]int limit = 20, [FromQuery]int skip = 0)
        {
            return _context.Cuca
                .OrderBy(c => c.Date)
                .Where(c => c.Date >= DateTime.UtcNow)
                .Include(c => c.Participants)
                .Skip(skip * limit)
                .Take(limit);
        }

        // GET: api/Cucas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCuca([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cuca = await _context.Cuca.Include(c => c.Participants)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (cuca == null)
            {
                return NotFound();
            }

            return Ok(cuca);
        }

        // PUT: api/Cucas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuca([FromRoute] int id, [FromBody] Cuca cuca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cuca.Id)
            {
                return BadRequest();
            }

            _context.Entry(cuca).State = EntityState.Modified;

            foreach (var participant in cuca.Participants)
            {
                _context.Entry(participant).State = UserExists(participant.Id) ? EntityState.Modified : EntityState.Added;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CucaExists(id))
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

        // PUT: api/Cucas/5/join
        [HttpPut("{id}/join")]
        public async Task<IActionResult> PutCuca([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cuca = _context.Cuca.Find(id);

            if (cuca == null)
            {
                return NotFound();
            }

            cuca.Participants.Add(user);

            _context.Entry(cuca).State = EntityState.Modified;

            foreach (var participant in cuca.Participants)
            {
                _context.Entry(participant).State = UserExists(participant.Id) ? EntityState.Modified : EntityState.Added;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CucaExists(id))
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

        // POST: api/Cucas
        [HttpPost]
        public async Task<IActionResult> PostCuca([FromBody] Cuca cuca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(cuca).State = EntityState.Added;

            foreach (var participant in cuca.Participants)
            {
                _context.Entry(participant).State = UserExists(participant.Id) ? EntityState.Modified : EntityState.Added;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCuca", new { id = cuca.Id }, cuca);
        }

        // DELETE: api/Cucas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuca([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cuca = await _context.Cuca.SingleOrDefaultAsync(m => m.Id == id);
            if (cuca == null)
            {
                return NotFound();
            }

            _context.Cuca.Remove(cuca);
            await _context.SaveChangesAsync();

            return Ok(cuca);
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(u => u.Id == id);
        }

        private bool CucaExists(int id)
        {
            return _context.Cuca.Any(e => e.Id == id);
        }
    }
}