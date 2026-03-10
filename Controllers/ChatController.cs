using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;
using SeyitnameWebSite.Models;

namespace SeyitnameWebSite.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public ChatController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Sohbet sayfasını görüntüle
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Mesajları 30 günü geçmişleri sil
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var oldMessages = await _context.Messages
                .Where(m => m.CreatedDate < thirtyDaysAgo)
                .ToListAsync();
            
            if (oldMessages.Any())
            {
                _context.Messages.RemoveRange(oldMessages);
                await _context.SaveChangesAsync();
            }

            // Tüm kullanıcıları al (bot hariç)
            var users = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .OrderBy(u => u.FullName ?? u.UserName)
                .ToListAsync();

            // Kullanıcılara rollerini ekle
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Roles = new List<string>(roles);
            }

            ViewBag.Users = users;
            ViewBag.CurrentUser = currentUser;

            return View();
        }

        // API: Tüm mesajları al
        [HttpGet("api/chat/messages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.Messages
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.CreatedDate)
                .Include(m => m.User)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.CreatedDate,
                    UserId = m.User.Id,
                    UserName = m.User.FullName ?? m.User.UserName,
                    UserImage = m.User.ProfileImage,
                    UserRole = _context.UserRoles
                        .Where(ur => ur.UserId == m.User.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .FirstOrDefault() ?? "User"
                })
                .ToListAsync();

            return Ok(messages);
        }

        // API: Mesaj gönder
        [HttpPost("api/chat/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 5000)
                return BadRequest("Mesaj boş veya çok uzun");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var message = new Message
            {
                UserId = currentUser.Id,
                Content = request.Content.Trim(),
                CreatedDate = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var userRole = userRoles.FirstOrDefault() ?? "User";

            return Ok(new
            {
                message.Id,
                message.Content,
                message.CreatedDate,
                UserId = currentUser.Id,
                UserName = currentUser.FullName ?? currentUser.UserName,
                UserImage = currentUser.ProfileImage,
                UserRole = userRole
            });
        }

        // API: Mesaj sil
        [HttpDelete("api/chat/delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return NotFound();

            if (message.UserId != currentUser.Id && !User.IsInRole("Admin"))
                return Forbid();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
