using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SeyitnameWebSite.Data;
using SeyitnameWebSite.Hubs;
using System.Threading.Tasks;

namespace SeyitnameWebSite.Controllers;

// Chat sistemi ve admin kontrolleri için controller
[Authorize]
public class ChatController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IHubContext<ChatHub> _chatHub;

    public ChatController(UserManager<User> userManager, IHubContext<ChatHub> chatHub)
    {
        _userManager = userManager;
        _chatHub = chatHub;
    }

    // Chat sayfası
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // Admin: Kullanıcıyı sustur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MuteUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            user.IsMuted = true;
            await _userManager.UpdateAsync(user);
            // Kullanıcıya bildirim gönder
            await _chatHub.Clients.User(user.Id).SendAsync("ReceiveSystemMessage", "Admin tarafından susturuldunuz.");
        }
        return RedirectToAction("Index");
    }

    // Admin: Kullanıcıyı banla
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            user.IsBanned = true;
            await _userManager.UpdateAsync(user);
            // Kullanıcı bağlantısını kes
            await _chatHub.Clients.User(user.Id).SendAsync("ReceiveSystemMessage", "Admin tarafından banlandınız.");
        }
        return RedirectToAction("Index");
    }

    // Admin: Susturmayı kaldır
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnmuteUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            user.IsMuted = false;
            await _userManager.UpdateAsync(user);
            await _chatHub.Clients.User(user.Id).SendAsync("ReceiveSystemMessage", "Susturma kaldırıldı.");
        }
        return RedirectToAction("Index");
    }

    // Admin: Ban kaldır
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnbanUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            user.IsBanned = false;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Index");
    }
}