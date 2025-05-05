using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public DiscountController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Discount
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscounts()
        {
            return await _context.Discounts
                .Include(d => d.Book)
                .ToListAsync();
        }

        // GET: api/Discount/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Discount>> GetDiscount(int id)
        {
            var discount = await _context.Discounts
                .Include(d => d.Book)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount == null)
            {
                return NotFound();
            }

            return discount;
        }

        // POST: api/Discount
        [HttpPost]
        public async Task<ActionResult<Discount>> PostDiscount(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, discount);
        }

        // PUT: api/Discount/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscount(int id, Discount discount)
        {
            if (id != discount.Id)
            {
                return BadRequest();
            }

            _context.Entry(discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(id))
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

        // DELETE: api/Discount/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(d => d.Id == id);
        }
    }
}
