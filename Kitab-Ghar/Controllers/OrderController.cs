using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public OrderController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return orders.Select(o => ToDTO(o)).ToList();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var emailSender = new EmailSender("smtp.gmail.com", 587, "gprabal505@gmail.com", "N@resh12");
            Console.WriteLine("asd");
            emailSender.SendEmail(
                recipientEmail: "lostzero980@gmail.com",
                subject: "Test Email from C#",
                body: "<h1>Hello!</h1><p>This is a test email sent from a C# app.</p>"
            );

            return ToDTO(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PostOrder(OrderDTO orderDto)
        {
            var order = FromDTO(orderDto);
            order.Date = DateTimeOffset.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, ToDTO(order));
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO orderDto)
        {
            if (id != orderDto.Id)
            {
                return BadRequest();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.UserId = orderDto.UserId;
            order.Status = orderDto.Status;
            order.TotalAmount = orderDto.TotalAmount;
            order.Date = orderDto.Date;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        // Mapping Helpers
        private static OrderDTO ToDTO(Order order) => new OrderDTO
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Date = order.Date
        };

        private static Order FromDTO(OrderDTO dto) => new Order
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Status = dto.Status,
            TotalAmount = dto.TotalAmount,
            Date = dto.Date
        };
    }
}
