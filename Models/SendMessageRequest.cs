using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Models
{
    public class SendMessageRequest
    {
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;
    }
}