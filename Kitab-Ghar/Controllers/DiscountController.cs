using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<DiscountDTO>>> GetDiscounts()
    {
        var discounts = await _context.Discounts
            .Include(d => d.Book)
            .ToListAsync();

        return discounts.Select(ToDTO).ToList();
    }

    // GET: api/Discount/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountDTO>> GetDiscount(int id)
    {
        var discount = await _context.Discounts
            .Include(d => d.Book)
            .FirstOrDefaultAsync(d => d.Id == id);

        return discount == null ? NotFound() : ToDTO(discount);
    }

    // POST: api/Discount
    [HttpPost]
    public async Task<ActionResult<DiscountDTO>> PostDiscount(DiscountDTO dto)
    {
        var discount = FromDTO(dto);
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, ToDTO(discount));
    }

    // PUT: api/Discount/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDiscount(int id, DiscountDTO dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null)
            return NotFound();

        // Update properties
        discount.BookId = dto.BookId;
        discount.DiscountPercent = dto.DiscountPercent;
        discount.OnSale = dto.OnSale;
        discount.DiscountStart = dto.DiscountStart;
        discount.DiscountEnd = dto.DiscountEnd;

        _context.Entry(discount).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DiscountExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Discount/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null)
            return NotFound();

        _context.Discounts.Remove(discount);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DiscountExists(int id) =>
        _context.Discounts.Any(e => e.Id == id);

    private static DiscountDTO ToDTO(Discount d) => new DiscountDTO
    {
        Id = d.Id,
        BookId = d.BookId,
        DiscountPercent = d.DiscountPercent,
        OnSale = d.OnSale,
        DiscountStart = d.DiscountStart,
        DiscountEnd = d.DiscountEnd
    };

    private static Discount FromDTO(DiscountDTO dto) => new Discount
    {
        Id = dto.Id,
        BookId = dto.BookId,
        DiscountPercent = dto.DiscountPercent,
        OnSale = dto.OnSale,
        DiscountStart = dto.DiscountStart,
        DiscountEnd = dto.DiscountEnd
    };
}
