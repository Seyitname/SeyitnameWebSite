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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
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

        [HttpGet]
        public async Task<IActionResult> ConfirmDeleteUser(string id)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized(new { message = "Bu sayfaya erişim yetkiniz yok!" });
            }

            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ----------------- Role Management -----------------
        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            if (!await IsAdminAsync()) return Unauthorized();
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRole()
        {
            if (!await IsAdminAsync()) return Unauthorized();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!await IsAdminAsync()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError(string.Empty, "Rol adı boş olamaz.");
                return View();
            }

n            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Bu rol zaten var.");
                return View();
            }

n            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Rol oluşturuldu.";
                return RedirectToAction(nameof(Roles));
            }

n            foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string id)
        {
            if (!await IsAdminAsync()) return Unauthorized();
            if (string.IsNullOrEmpty(id)) return BadRequest();

n            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

n            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = allRoles.Select(r => new RoleSelection { RoleName = r, Selected = userRoles.Contains(r) }).ToList()
            };

n            return View(model);
        }

n        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRoles(ManageUserRolesViewModel model)
        {
            if (!await IsAdminAsync()) return Unauthorized();
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

n            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.RoleName).ToList();

n            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

n            if (rolesToAdd.Any()) await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (rolesToRemove.Any()) await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

n            TempData["SuccessMessage"] = "Kullanıcı rolleri güncellendi.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirm(string id)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized(new { message = "Bu işlemi yapamazsınız!" });
            }

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı id!";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı silindi!";
                return RedirectToAction(nameof(Users));
            }

            TempData["ErrorMessage"] = "Silme işlemi başarısız!";
            return RedirectToAction(nameof(Users));
        }
    }

    // Model for DeleteUser JSON body
    public class DeleteUserModel
    {
        public string? Id { get; set; }
    }

    // ----------------- Role management view models -----------------
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<RoleSelection> Roles { get; set; } = new();
    }

n    public class RoleSelection
    {
        public string RoleName { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }
}


