using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace SeyitnameWebSite.Data
{
    // AI tarafından yapıldı - Identity entegrasyonu
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<Baglanti> Baglantilar => Set<Baglanti>();
        public DbSet<IletisimBilgileri> IBilgiler => Set<IletisimBilgileri>();
    }
}