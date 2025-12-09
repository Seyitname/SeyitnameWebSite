using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeyitnameWebSite.Data;

namespace SeyitnameWebSite.Controllers;

// AI tarafından yapıldı - User Profile Controller
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly DataContext _context;

    public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var model = new EditProfileModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Bio = user.Bio,
            UserName = user.UserName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Bio = model.Bio;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

        // Şifreyi kontrol et
        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
            return Json(new { success = false, message = "❌ Şifre yanlış! Hesap silinmedi." });

        // Hesabı sil
        var deleteResult = await _userManager.DeleteAsync(user);
        if (deleteResult.Succeeded)
        {
            // Oturumu kapat
            await _signInManager.SignOutAsync();
            return Json(new { success = true, message = "Hesab başarıyla silindi!" });
        }

        return Json(new { success = false, message = "Silme işlemi başarısız!" });
    }
}

// AI tarafından yapıldı - Edit Profile Model
public class EditProfileModel
{
    public string? UserName { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string? Email { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(500)]
    public string? Bio { get; set; }
}

// AI tarafından yapıldı - Delete Account Model
public class DeleteAccountModel
{
    public string Password { get; set; } = string.Empty;
}
