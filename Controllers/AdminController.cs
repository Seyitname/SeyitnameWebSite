using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeyitnameWebSite.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// AI tarafından yapıldı - Admin Paneli Kontrolörü

namespace SeyitnameWebSite.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public AdminController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Admin kontrol - sadece ilk kullanıcı (admin) erişebilir
        private async Task<bool> IsAdminAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return false;

            // Check role membership instead of relying on user order
            return await _userManager.IsInRoleAsync(currentUser, "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            // Admin kontrol
            if (!await IsAdminAsync())
            {
                return Unauthorized(new { message = "Bu sayfaya erişim yetkiniz yok!" });
            }

            var users = _userManager.Users
                .OrderByDescending(u => u.CreatedDate)
                .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserModel model)
        {
            var id = model?.Id;

            // Admin kontrol
            if (!await IsAdminAsync())
            {
                return Unauthorized(new { message = "Bu işlemi yapamazsınız!" });
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { success = false, message = "Geçersiz kullanıcı id!" });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı!" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Kullanıcı silindi!" });
            }

            return BadRequest(new { success = false, message = "Silme işlemi başarısız!" });
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            // Admin kontrol
            if (!await IsAdminAsync())
            {
                return Unauthorized(new { message = "Bu sayfaya erişim yetkiniz yok!" });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }

    // Model for DeleteUser JSON body
    public class DeleteUserModel
    {
        public string? Id { get; set; }
    }
}

