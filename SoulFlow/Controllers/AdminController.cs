using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFlow.Models;
using System.Threading.Tasks;

namespace SoulFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.UserName == User.Identity.Name)
            {
                TempData["Error"] = "Kendi yöneticisi hesabınızı silemezsiniz.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı ve ilişkili tüm veriler başarıyla temizlendi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinemedi. Sistemsel bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}