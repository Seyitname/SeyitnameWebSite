using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Data;

// AI tarafından yapıldı
public class User : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? Bio { get; set; }

    // Kullanıcı adı yanına eklenecek tag (örneğin [001])
    public string Tag { get; set; } = string.Empty;

    // Chat için mute/ban durumları
    public bool IsMuted { get; set; } = false;
    public bool IsBanned { get; set; } = false;
}
