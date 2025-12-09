using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Data
{
    public class Baglanti
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Picture { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
    }
}