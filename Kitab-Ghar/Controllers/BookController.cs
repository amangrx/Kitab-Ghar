using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly DatabaseHandler _context;

    public BooksController(DatabaseHandler context)
    {
        _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
    {
        var books = await _context.Books.ToListAsync();
        return books.Select(b => ToDTO(b)).ToList();
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return ToDTO(book);
    }

    // PUT: api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookDTO bookDto)
    {
        if (id != bookDto.BookId)
        {
            return BadRequest();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        book.Title = bookDto.Title;
        book.Price = bookDto.Price;
        book.Availability = bookDto.Availability;
        book.ISBN = bookDto.ISBN;

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
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

    // POST: api/Books
    [HttpPost]
    public async Task<ActionResult<BookDTO>> PostBook(BookDTO bookDto)
    {
        var book = FromDTO(bookDto);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, ToDTO(book));
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }

    // Mapping Helpers
    private static BookDTO ToDTO(Book book) => new BookDTO
    {
        BookId = book.BookId,
        Title = book.Title,
        Price = book.Price,
        Availability = book.Availability,
        ISBN = book.ISBN
    };

    private static Book FromDTO(BookDTO dto) => new Book
    {
        BookId = dto.BookId,
        Title = dto.Title,
        Price = dto.Price,
        Availability = dto.Availability,
        ISBN = dto.ISBN
    };

    // GET: api/Books/search
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks(string title = null, string isbn = null, bool? availability = null)
    {
        var query = _context.Books.AsQueryable();

        // Apply filters if parameters are provided
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }
        if (!string.IsNullOrEmpty(isbn))
        {
            query = query.Where(b => b.ISBN.Contains(isbn));
        }
        if (availability.HasValue)
        {
            query = query.Where(b => b.Availability == availability);
        }

        var books = await query.ToListAsync();

        return books.Select(b => ToDTO(b)).ToList();
    }
}
