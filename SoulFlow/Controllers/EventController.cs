using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoulFlow.Data;
using SoulFlow.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SoulFlow.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EventController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.Include(e => e.Host).ToListAsync();
            return View(events);
        }

        [Authorize]
        public async Task<IActionResult> Matches()
        {
            
            var matchedEvents = await _context.Events.Include(e => e.Host).ToListAsync();
            return View(matchedEvents);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Host)
                .Include(e => e.Participants).ThenInclude(p => p.User)
                .Include(e => e.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Event @event)
        {
            if (@event.ImageFile != null)
            {
                var extension = Path.GetExtension(@event.ImageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/events/", newImageName);

                var directory = Path.GetDirectoryName(location);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await @event.ImageFile.CopyToAsync(stream);
                }

                @event.ImageUrl = "/images/events/" + newImageName;
            }

            var user = await _userManager.GetUserAsync(User);
            @event.HostId = user.Id;
            @event.EventCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            ModelState.Remove("Host");
            ModelState.Remove("HostId");
            ModelState.Remove("EventCode");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");
            ModelState.Remove("Duration");

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (@event.HostId != user.Id) return Unauthorized();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Event @event, string tarihVerisi)
        {
            if (id != @event.Id) return NotFound();

            var eventInDb = await _context.Events.FindAsync(id);
            if (eventInDb == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (eventInDb.HostId != user.Id) return Unauthorized();

            if (!string.IsNullOrEmpty(tarihVerisi))
            {
                eventInDb.Date = DateTime.Parse(tarihVerisi);
            }

            if (@event.ImageFile != null)
            {
                var extension = Path.GetExtension(@event.ImageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/events/", newImageName);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await @event.ImageFile.CopyToAsync(stream);
                }
                eventInDb.ImageUrl = "/images/events/" + newImageName;
            }

            eventInDb.Title = @event.Title;
            eventInDb.Description = @event.Description;
            eventInDb.Location = @event.Location;
            eventInDb.Quota = @event.Quota;

            try
            {
                _context.Update(eventInDb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Events.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.Include(e => e.Host).FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (@event.HostId != user.Id) return Unauthorized();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Join(int id)
        {
            var @event = await _context.Events.Include(e => e.Participants).FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);

            if (@event.Participants.Count >= @event.Quota) return RedirectToAction("Details", new { id = id });
            if (@event.Participants.Any(p => p.UserId == user.Id)) return RedirectToAction("Details", new { id = id });

            var participant = new EventParticipant { EventId = id, UserId = user.Id, JoinedAt = DateTime.Now };
            _context.EventParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return RedirectToAction("Details", new { id = id });
            var user = await _userManager.GetUserAsync(User);

            var comment = new Comment { EventId = id, UserId = user.Id, Content = content, CreatedAt = DateTime.Now };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }
    }
}