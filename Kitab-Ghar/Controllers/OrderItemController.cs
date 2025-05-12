using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public OrderItemController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItems()
        {
            var orderItems = await _context.OrderItems.ToListAsync();
            return orderItems.Select(oi => ToDTO(oi)).ToList();
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDTO>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return ToDTO(orderItem);
        }

        // POST: api/OrderItem
        [HttpPost]
        public async Task<ActionResult<OrderItemDTO>> PostOrderItem(OrderItemDTO orderItemDto)
        {
            var orderItem = FromDTO(orderItemDto);
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, ToDTO(orderItem));
        }

        // PUT: api/OrderItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItemDTO orderItemDto)
        {
            if (id != orderItemDto.Id)
            {
                return BadRequest();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.BookId = orderItemDto.BookId;
            orderItem.Quantity = orderItemDto.Quantity;

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
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

        // DELETE: api/OrderItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }

        // Mapping Helpers
        private static OrderItemDTO ToDTO(OrderItem orderItem) => new OrderItemDTO
        {
            Id = orderItem.Id,
            BookId = orderItem.BookId,
            Quantity = orderItem.Quantity
        };

        private static OrderItem FromDTO(OrderItemDTO dto) => new OrderItem
        {
            Id = dto.Id,
            BookId = dto.BookId,
            Quantity = dto.Quantity
        };
    }
}
