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
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
            return await _context.Announcements.ToListAsync();
        }

        // GET: api/Announcement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Announcement>> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            return announcement;
        }

        // POST: api/Announcement
        [HttpPost]
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
            announcement.AnnouncementTime = DateTime.UtcNow;
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.Id }, announcement);
        }

        // PUT: api/Announcement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, Announcement announcement)
        {
            if (id != announcement.Id)
            {
                return BadRequest();
            }

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
    }
}
