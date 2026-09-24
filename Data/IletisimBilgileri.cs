using System.ComponentModel.DataAnnotations;

namespace SeyitnameWebSite.Data
{
    public class IletisimBilgileri
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Lütfen bize bir puan veriniz.")]
        [Range(0, 10, ErrorMessage = "Puan 0 ile 10 arasında olmalıdır.")] //AI ekledi
        public int Puan { get; set; }
        [Required(ErrorMessage = "İletişime geçersek isminizle hitap edeceğiz")]
        public string? Ad { get; set; }
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Required(ErrorMessage = "İletişime geçmemiz için önemlidir")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Lütfen mesajınızı açıkça belirtin; böylece sitemizi geliştirelim")]
        public string? Mesaj { get; set; }
    }
}
 
