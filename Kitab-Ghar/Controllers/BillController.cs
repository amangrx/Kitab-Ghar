using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public BillController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Bill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDTO>>> GetBills()
        {
            var bills = await _context.Bills
                .Include(b => b.Order)
                .ToListAsync();

            return bills.Select(b => ToDTO(b)).ToList();
        }

        // GET: api/Bill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillDTO>> GetBill(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Order)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return ToDTO(bill);
        }

        // POST: api/Bill
        [HttpPost]
        public async Task<ActionResult<BillDTO>> PostBill(BillDTO billDto)
        {
            var bill = FromDTO(billDto);
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, ToDTO(bill));
        }

        // PUT: api/Bill/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, BillDTO billDto)
        {
            if (id != billDto.Id)
            {
                return BadRequest();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            bill.OrderId = billDto.OrderId;
            bill.Amount = billDto.Amount;

            _context.Entry(bill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
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

        // DELETE: api/Bill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(b => b.Id == id);
        }

        // Mapping Helpers
        private static BillDTO ToDTO(Bill bill) => new BillDTO
        {
            Id = bill.Id,
            OrderId = bill.OrderId,
            Amount = bill.Amount
        };

        private static Bill FromDTO(BillDTO dto) => new Bill
        {
            Id = dto.Id,
            OrderId = dto.OrderId,
            Amount = dto.Amount
        };
    }
}
