using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public ReviewController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Review
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .ToListAsync();

            var reviewDTOs = reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                BookId = r.BookId,
                Comment = r.Comment,
                Rating = r.Rating,
                ReviewDate = r.ReviewDate
            }).ToList();

            return Ok(reviewDTOs);
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                BookId = review.BookId,
                Comment = review.Comment,
                Rating = review.Rating,
                ReviewDate = review.ReviewDate
            };

            return Ok(reviewDTO);
        }

        // POST: api/Review
        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> PostReview(Review review)
        {
            review.ReviewDate = DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                BookId = review.BookId,
                Comment = review.Comment,
                Rating = review.Rating,
                ReviewDate = review.ReviewDate
            };

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDTO);
        }

        // PUT: api/Review/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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

        // DELETE: api/Review/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(r => r.Id == id);
        }
    }
}
