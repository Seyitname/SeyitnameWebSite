using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeyitnameWebSite.Data;

// AI tarafından yapıldı
public class User : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? Bio { get; set; }

    // Profil resmi
    public string? ProfileImage { get; set; }

    // Kullanıcı adı yanına eklenecek tag (örneğin [001])
    public string Tag { get; set; } = string.Empty;

    // Chat için mute/ban durumları
    public bool IsMuted { get; set; } = false;
    public bool IsBanned { get; set; } = false;

    // Roles (runtime'da kullanmak için, veritabanına kaydedilmez)
    [NotMapped]
    public virtual ICollection<string> Roles { get; set; } = new List<string>();
}
