using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public CartController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDTO>>> GetCarts()
        {
            var carts = await _context.Carts.ToListAsync();
            var cartDTOs = carts.Select(c => new CartDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                Date = c.Date
            }).ToList();

            return cartDTOs;
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> GetCart(int id)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            var cartDTO = new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Date = cart.Date
            };

            return cartDTO;
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<CartDTO>> PostCart(CartDTO cartDTO)
        {
            var cart = new Cart
            {
                UserId = cartDTO.UserId,
                Date = cartDTO.Date
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            cartDTO.Id = cart.Id;

            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cartDTO);
        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, CartDTO cartDTO)
        {
            if (id != cartDTO.Id)
            {
                return BadRequest();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            cart.UserId = cartDTO.UserId;
            cart.Date = cartDTO.Date;

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(c => c.Id == id);
        }
    }

    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}