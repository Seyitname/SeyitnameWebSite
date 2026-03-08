using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeyitnameWebSite.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SeyitnameWebSite.Controllers
{
    [Authorize]
    public class SpecialGuestController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public SpecialGuestController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Sadece "özel misafir" rolüne sahip kullanıcılar ve adminler erişebilir
        private async Task<bool> CanAccessAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return false;

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isSpecialGuest = await _userManager.IsInRoleAsync(currentUser, "özel misafir");

            return isAdmin || isSpecialGuest;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await CanAccessAsync())
            {
                return Unauthorized("Bu sayfaya erişim yetkiniz yok!");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var homeworks = await _context.Homeworks
                .Where(h => h.UserId == currentUser.Id)
                .OrderByDescending(h => h.CreatedDate)
                .ToListAsync();

            return View(homeworks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHomework(string title, string? subject, string? description, DateTime? dueDate)
        {
            if (!await CanAccessAsync())
            {
                return Unauthorized("Bu işlemi yapamazsınız!");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                TempData["Error"] = "Ödev başlığı zorunludur!";
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var homework = new Homework
            {
                Title = title.Trim(),
                Subject = subject?.Trim(),
                Description = description?.Trim(),
                DueDate = dueDate,
                UserId = currentUser.Id
            };

            _context.Homeworks.Add(homework);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ödev başarıyla eklendi!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHomework(int id)
        {
            if (!await CanAccessAsync())
            {
                return Unauthorized("Bu işlemi yapamazsınız!");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var homework = await _context.Homeworks
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (homework == null)
            {
                return NotFound("Ödev bulunamadı!");
            }

            homework.IsCompleted = !homework.IsCompleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHomework(int id)
        {
            if (!await CanAccessAsync())
            {
                return Unauthorized("Bu işlemi yapamazsınız!");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var homework = await _context.Homeworks
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == currentUser.Id);

            if (homework == null)
            {
                return NotFound("Ödev bulunamadı!");
            }

            _context.Homeworks.Remove(homework);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ödev başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }
    }
}