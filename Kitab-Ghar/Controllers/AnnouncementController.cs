using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kitab_Ghar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly DatabaseHandler _context;

        public AnnouncementController(DatabaseHandler context)
        {
            _context = context;
        }

        // GET: api/Announcement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAnnouncements()
        {
            var announcements = await _context.Announcements.ToListAsync();
            return announcements.Select(a => ToDTO(a)).ToList();
        }

        // GET: api/Announcement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDTO>> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            return ToDTO(announcement);
        }

        // POST: api/Announcement
        [HttpPost]
        public async Task<ActionResult<AnnouncementDTO>> PostAnnouncement(AnnouncementDTO dto)
        {
            var announcement = FromDTO(dto);
            announcement.AnnouncementTime = DateTimeOffset.UtcNow;

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.Id }, ToDTO(announcement));
        }

        // PUT: api/Announcement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, AnnouncementDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            announcement.Message = dto.Message;
            announcement.Type = dto.Type;
            announcement.AnnouncementTime = dto.AnnouncementTime;

            _context.Entry(announcement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnouncementExists(id))
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

        // DELETE: api/Announcement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Id == id);
        }

        // Mapping Helpers
        private static AnnouncementDTO ToDTO(Announcement a) => new AnnouncementDTO
        {
            Id = a.Id,
            Message = a.Message,
            Type = a.Type,
            AnnouncementTime = a.AnnouncementTime
        };

        private static Announcement FromDTO(AnnouncementDTO dto) => new Announcement
        {
            Id = dto.Id,
            Message = dto.Message,
            Type = dto.Type,
            AnnouncementTime = dto.AnnouncementTime
        };
    }
}
