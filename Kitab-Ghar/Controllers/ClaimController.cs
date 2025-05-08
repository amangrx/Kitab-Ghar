using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public ClaimController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Claim
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaimDTO>>> GetClaims()
        {
            var claims = await _context.Claims.ToListAsync();
            return claims.Select(c => ToDTO(c)).ToList();
        }

        // GET: api/Claim/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimDTO>> GetClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            return ToDTO(claim);
        }

        // POST: api/Claim
        [HttpPost]
        public async Task<ActionResult<ClaimDTO>> PostClaim(ClaimDTO dto)
        {
            var claim = FromDTO(dto);
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, ToDTO(claim));
        }

        // PUT: api/Claim/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClaim(int id, ClaimDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.ClaimCode = dto.ClaimCode;

            _context.Entry(claim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClaimExists(id))
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

        // DELETE: api/Claim/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClaimExists(int id)
        {
            return _context.Claims.Any(c => c.Id == id);
        }

        // Mapping Methods
        private static ClaimDTO ToDTO(Claims claim) => new ClaimDTO
        {
            Id = claim.Id,
            ClaimCode = claim.ClaimCode
        };

        private static Claims FromDTO(ClaimDTO dto) => new Claims
        {
            Id = dto.Id,
            ClaimCode = dto.ClaimCode
        };
    }
}
