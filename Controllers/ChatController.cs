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
    private readonly DataContext _context;

    public ChatController(UserManager<User> userManager, IHubContext<ChatHub> chatHub, DataContext context)
    {
        _userManager = userManager;
        _chatHub = chatHub;
        _context = context;
    }

    // Chat sayfası
    [HttpGet]
    public IActionResult Index()
    {
        // Son 50 mesajı veritabanından yükle
        var messages = _context.ChatMessages
            .OrderByDescending(m => m.CreatedDate)
            .Take(50)
            .OrderBy(m => m.CreatedDate)
            .ToList();

        Console.WriteLine($"Loading {messages.Count} historical messages for chat");
        foreach (var msg in messages)
        {
            Console.WriteLine($"Message: {msg.UserName}: {msg.Message}");
        }

        return View(messages);
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