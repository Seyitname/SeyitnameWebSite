using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeyitnameWebSite.Data
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Silinmeyi kolay hale getirmek için
        public bool IsDeleted { get; set; } = false;
    }
}
