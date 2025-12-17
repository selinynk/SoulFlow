using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFlow.Data;
using System.Threading.Tasks;
using System.Linq;

namespace SoulFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchKeyword, string searchLocation)
        {
            var eventsQuery = _context.Events.Include(e => e.Host).AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                eventsQuery = eventsQuery.Where(e => e.Title.Contains(searchKeyword) || e.Description.Contains(searchKeyword));
            }

            if (!string.IsNullOrEmpty(searchLocation))
            {
                eventsQuery = eventsQuery.Where(e => e.Location.Contains(searchLocation));
            }

            var events = await eventsQuery.OrderBy(e => e.Date).ToListAsync();
            return View(events);
        }

        public IActionResult Communities()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult JoinCommunity(string communityName)
        {
            if (string.IsNullOrEmpty(communityName))
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, message = communityName + " topluluðuna baþarýyla katýldýn!" });
        }
    }
}