using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeyitnameWebSite.Data;
public class User : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? Bio { get; set; }

    // Kullanıcı adının yanına eklenecek tag (örneğin [001])
    public string Tag { get; set; } = string.Empty;

    // Roles (runtime'da kullanmak için, veritabanına kaydedilmez)
    [NotMapped]
    public virtual ICollection<string> Roles { get; set; } = new List<string>();
}

