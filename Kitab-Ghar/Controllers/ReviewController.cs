using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly DatabaseHandler _context;

    public ReviewsController(DatabaseHandler context)
    {
        _context = context;
    }

    // GET: api/Reviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
    {
        var reviews = await _context.Reviews.ToListAsync();
        return reviews.Select(ToDTO).ToList();
    }

    // GET: api/Reviews/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDTO>> GetReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        return ToDTO(review);
    }

    // GET: api/Reviews/book/5
    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByBook(int bookId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.BookId == bookId)
            .ToListAsync();

        return reviews.Select(ToDTO).ToList();
    }

    // POST: api/Reviews
    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> PostReview(ReviewDTO dto)
    {
        var review = FromDTO(dto);
        review.ReviewDate = DateTimeOffset.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, ToDTO(review));
    }

    // PUT: api/Reviews/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutReview(int id, ReviewDTO dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        review.Comment = dto.Comment;
        review.Rating = dto.Rating;
        review.ReviewDate = dto.ReviewDate;

        _context.Entry(review).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReviewExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReviewExists(int id) => _context.Reviews.Any(e => e.Id == id);

    private static ReviewDTO ToDTO(Review review) => new ReviewDTO
    {
        Id = review.Id,
        BookId = review.BookId,
        Comment = review.Comment,
        Rating = review.Rating,
        ReviewDate = review.ReviewDate,
        UserId = review.UserId
    };

    private static Review FromDTO(ReviewDTO dto) => new Review
    {
        Id = dto.Id,
        BookId = dto.BookId,
        Comment = dto.Comment,
        Rating = dto.Rating,
        ReviewDate = dto.ReviewDate,
        UserId = dto.UserId
    };
}
