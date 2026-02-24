using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SeyitnameWebSite.Controllers;

// Kullanıcıların birbirinin profillerini bulabilmesi için arama controller'ı
[Authorize]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly DataContext _context;

    public UserController(UserManager<User> userManager, DataContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // Kullanıcı listesi ve arama sayfası
    [HttpGet]
    public async Task<IActionResult> Index(string searchTerm)
    {
        var usersQuery = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            // Kullanıcı adı veya tam isimde arama
            usersQuery = usersQuery.Where(u =>
                u.UserName.Contains(searchTerm) ||
                u.FullName.Contains(searchTerm) ||
                (u.UserName + u.Tag).Contains(searchTerm));
        }

        var users = await usersQuery
            .OrderBy(u => u.UserName)
            .Take(50) // İlk 50 kullanıcıyı göster
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        return View(users);
    }

    // Belirli bir kullanıcının profilini görüntüleme
    [HttpGet]
    public async Task<IActionResult> Profile(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
            return NotFound();

        return View(user);
    }
}