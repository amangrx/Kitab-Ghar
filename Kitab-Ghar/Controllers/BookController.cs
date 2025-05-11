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
        return books.Select(ToDTO).ToList();
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        return book == null ? NotFound() : ToDTO(book);
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

    // PUT: api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookDTO bookDto)
    {
        if (id != bookDto.BookId)
            return BadRequest();

        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        // Update all fields
        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Genre = bookDto.Genre;
        book.Price = bookDto.Price;
        book.Language = bookDto.Language;
        book.Publishers = bookDto.Publishers;
        book.Description = bookDto.Description;
        book.Availability = bookDto.Availability;
        book.ISBN = bookDto.ISBN;
        book.PublicationDate = bookDto.PublicationDate;
        book.Format = bookDto.Format;
        book.Tags = bookDto.Tags;
        book.Image = bookDto.Image;

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Books/search?title=xyz&isbn=123
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks(string title = null, string isbn = null)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(isbn))
            query = query.Where(b => b.ISBN.Contains(isbn));

        var books = await query.ToListAsync();
        return books.Select(ToDTO).ToList();
    }

    // GET: api/Books/filter?minPrice=10&maxPrice=30&availability=true
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> FilterBooks(
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? availability = null)
    {
        var query = _context.Books.AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(b => b.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(b => b.Price <= maxPrice);

        if (availability.HasValue)
            query = query.Where(b => b.Availability == availability);

        var books = await query.ToListAsync();
        return books.Select(ToDTO).ToList();
    }

    private bool BookExists(int id) => _context.Books.Any(e => e.BookId == id);

    private static BookDTO ToDTO(Book book) => new BookDTO
    {
        BookId = book.BookId,
        Title = book.Title,
        Author = book.Author,
        Genre = book.Genre,
        Price = book.Price,
        Language = book.Language,
        Publishers = book.Publishers,
        Description = book.Description,
        Availability = book.Availability,
        ISBN = book.ISBN,
        PublicationDate = book.PublicationDate,
        Format = book.Format,
        Tags = book.Tags,
        Image = book.Image
    };

    private static Book FromDTO(BookDTO dto) => new Book
    {
        BookId = dto.BookId,
        Title = dto.Title,
        Author = dto.Author,
        Genre = dto.Genre,
        Price = dto.Price,
        Language = dto.Language,
        Publishers = dto.Publishers,
        Description = dto.Description,
        Availability = dto.Availability,
        ISBN = dto.ISBN,
        PublicationDate = dto.PublicationDate,
        Format = dto.Format,
        Tags = dto.Tags,
        Image = dto.Image
    };
}
