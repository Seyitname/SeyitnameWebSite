using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Data
{
    public class Homework
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        // User ID for assignment
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}