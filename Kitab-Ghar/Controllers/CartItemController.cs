using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public CartItemController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/CartItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetCartItems()
        {
            var cartItems = await _context.CartItems.ToListAsync();
            var cartItemDTOs = cartItems.Select(ci => new CartItemDTO
            {
                Id = ci.Id,
                BookId = ci.BookId,
                Quantity = ci.Quantity,
                CartId = ci.CartId
            }).ToList();

            return cartItemDTOs;
        }

        // GET: api/CartItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemDTO>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            var cartItemDTO = new CartItemDTO
            {
                Id = cartItem.Id,
                BookId = cartItem.BookId,
                Quantity = cartItem.Quantity
            };

            return cartItemDTO;
        }

        // POST: api/CartItem
        [HttpPost]
        public async Task<ActionResult<CartItemDTO>> PostCartItem(CartItemDTO cartItemDTO)
        {
            var cartItem = new CartItem
            {
                Id = cartItemDTO.Id,
                BookId = cartItemDTO.BookId,
                CartId = cartItemDTO.CartId, 
                Quantity = cartItemDTO.Quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            cartItemDTO.Id = cartItem.Id;

            return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.Id }, cartItemDTO);
        }

        // PUT: api/CartItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItemDTO cartItemDTO)
        {
            if (id != cartItemDTO.Id)
            {
                return BadRequest();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.BookId = cartItemDTO.BookId;
            cartItem.Quantity = cartItemDTO.Quantity;

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
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

        // DELETE: api/CartItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(ci => ci.Id == id);
        }

        private int GetCartIdFromBookId(int bookId)
        {
            return 1;
        }
    }
}