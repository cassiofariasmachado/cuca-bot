using CucaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CucaAPI.Controllers
{
    [Route("api/Cucas")]
    [Produces("application/json")]
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
            return _context.Cucas;
        }

        // GET: api/Cucas/next
        [HttpGet]
        [Route("next")]
        public IEnumerable<Cuca> GetCuca([FromQuery]int limit = 20, [FromQuery]int skip = 0)
        {
            return _context.Cucas
                .OrderBy(c => c.Date)
                .Where(c => c.Date >= DateTime.UtcNow)
                .Include(c => c.Participants)
                .Skip(skip * limit)
                .Take(limit);
        }

        // GET: api/Cucas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCuca([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuca = await _context.Cucas.Include(c => c.Participants)
                .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (cuca == null)
                return NotFound();

            return Ok(cuca);
        }

        // PUT: api/Cucas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuca(
            [FromRoute] int id,
            [FromBody] Cuca cuca,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != cuca.Id)
                return BadRequest();

            _context.Entry(cuca).State = EntityState.Modified;

            foreach (var participant in cuca.Participants)
            {
                bool userExists = await UserExists(participant.Id, cancellationToken);
                _context.Entry(participant).State = userExists ? EntityState.Modified : EntityState.Added;
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                bool cucaExists = await CucaExists(id, cancellationToken);
                if (!cucaExists)
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // PUT: api/Cucas/5/join
        [HttpPut("{id}/join")]
        public async Task<IActionResult> PutCuca(
            [FromRoute] int id,
            [FromBody] User user,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuca = await _context.Cucas.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (cuca == null)
                return NotFound();

            cuca.Participants.Add(user);

            _context.Entry(cuca).State = EntityState.Modified;

            foreach (var participant in cuca.Participants)
            {
                bool userExists = await UserExists(participant.Id, cancellationToken);
                _context.Entry(participant).State = userExists ? EntityState.Modified : EntityState.Added;
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                bool cucaExists = await CucaExists(id, cancellationToken);
                if (!cucaExists)
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Cucas
        [HttpPost]
        public async Task<IActionResult> PostCuca([FromBody] Cuca cuca, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(cuca).State = EntityState.Added;

            foreach (var participant in cuca.Participants)
            {
                bool userExists = await UserExists(participant.Id, cancellationToken);
                _context.Entry(participant).State = userExists ? EntityState.Modified : EntityState.Added;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction("GetCuca", new { id = cuca.Id }, cuca);
        }

        // DELETE: api/Cucas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuca([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuca = await _context.Cucas.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (cuca == null)
                return NotFound();

            _context.Cucas.Remove(cuca);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(cuca);
        }

        private async Task<bool> UserExists(string id, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
        }

        private async Task<bool> CucaExists(int id, CancellationToken cancellationToken)
        {
            return await _context.Cucas.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}