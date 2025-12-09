using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Data;

// AI tarafından yapıldı
public class User : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string? Bio { get; set; }
}
