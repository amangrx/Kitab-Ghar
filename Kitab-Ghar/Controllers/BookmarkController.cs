using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public BookmarkController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Bookmark
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmarks()
        {
            return await _context.Bookmarks
                .Include(b => b.User)
                .Include(b => b.Book)
                .ToListAsync();
        }

        // GET: api/Bookmark/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
            var bookmark = await _context.Bookmarks
                .Include(b => b.User)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return bookmark;
        }

        // POST: api/Bookmark
        [HttpPost]
        public async Task<ActionResult<Bookmark>> PostBookmark(Bookmark bookmark)
        {
            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmark);
        }

        // PUT: api/Bookmark/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookmark(int id, Bookmark bookmark)
        {
            if (id != bookmark.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookmark).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookmarkExists(id))
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

        // DELETE: api/Bookmark/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            var bookmark = await _context.Bookmarks.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookmarkExists(int id)
        {
            return _context.Bookmarks.Any(b => b.Id == id);
        }
    }

