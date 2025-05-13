using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly DatabaseHandler _context;

    public UsersController(DatabaseHandler context)
    {
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(ToDTO).ToList();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? NotFound() : ToDTO(user);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDto)
    {
        var user = FromDTO(userDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ToDTO(user));
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, UserDTO userDto)
    {
        if (id != userDto.Id)
            return BadRequest();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.Name = userDto.Name;
        user.Email = userDto.Email;

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id) => _context.Users.Any(u => u.Id == id);

    private static UserDTO ToDTO(User user) => new UserDTO
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    };

    private static User FromDTO(UserDTO dto) => new User
    {
        Id = dto.Id,
        Name = dto.Name,
        Email = dto.Email
    };
}
