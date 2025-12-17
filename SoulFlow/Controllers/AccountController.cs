using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFlow.Data;
using SoulFlow.Models;

namespace SoulFlow.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return RedirectToAction("Login");

            var model = new ProfileViewModel
            {
                Username = user.UserName,
                UserInterests = user.Interests,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,

                MyEvents = await _context.Events
                    .Where(e => e.HostId == userId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync(),

                JoinedEvents = await _context.EventParticipants
                    .Where(p => p.UserId == userId)
                    .Include(p => p.Event)
                    .ThenInclude(e => e.Host)
                    .Select(p => p.Event)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(string username)
        {
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Böyle bir kullanıcı bulunamadı.");

            var model = new ProfileViewModel
            {
                Username = user.UserName,
                UserInterests = user.Interests,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,

                MyEvents = await _context.Events
                    .Where(e => e.HostId == user.Id)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync(),

                JoinedEvents = await _context.EventParticipants
                    .Where(p => p.UserId == user.Id)
                    .Include(p => p.Event)
                    .ThenInclude(e => e.Host)
                    .Select(p => p.Event)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string interestsToSave = "";

                if (model.SelectedInterests != null && model.SelectedInterests.Count > 0)
                {
                    interestsToSave = string.Join(", ", model.SelectedInterests);
                }

                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Interests = interestsToSave
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var model = new UserEditViewModel
            {
                Bio = user.Bio,
                Interests = user.Interests,
                ExistingImage = user.ProfileImage
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                if (model.ProfilePicture != null)
                {
                    var extension = Path.GetExtension(model.ProfilePicture.FileName);
                    var newImageName = $"{user.Id}_{Guid.NewGuid()}{extension}";

                    var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");

                    if (!Directory.Exists(location)) Directory.CreateDirectory(location);

                    var path = Path.Combine(location, newImageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfileImage = newImageName;
                }

                user.Bio = model.Bio;
                user.Interests = model.Interests;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            model.ExistingImage = user.ProfileImage;
            return View(model);
        }
    }
}