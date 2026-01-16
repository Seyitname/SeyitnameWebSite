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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Baglanti>().HasData(
                new Baglanti { Id = 1, Name = "YouTube", Picture = "YouTube.png", Description = "YouTube kanalımdır ve aktif değildir.", Link = "https://www.youtube.com/@seyitname" },
                new Baglanti { Id = 2, Name = "Chesscom", Picture = "Chesscom.png", Description = "Satrançta kendimi geliştirmeye çalışıyorum.", Link = "https://www.chess.com/member/seyitnames" },
                new Baglanti { Id = 3, Name = "Chesscom Kulüp", Picture = "Chesscomkulup.png", Description = "Kulübe istek atan herkesi alabiliriz.", Link = "https://www.chess.com/club/chessname-1/join/757085" },
                new Baglanti { Id = 4, Name = "GitHub", Picture = "GitHub.png", Description = "Bazen yazdığım kodları paylaşırım.", Link = "https://github.com/seyitname" },
                new Baglanti { Id = 5, Name = "Instagram", Picture = "Instagram.jpeg", Description = "Genelde paylaşım yapmam.", Link = "https://www.instagram.com/seyitname26" },
                new Baglanti { Id = 6, Name = "Kick", Picture = "Kick.png", Description = "Yayın açmam ama takip ederseniz sevinirim.", Link = "https://www.kick.com/seyitname" },
                new Baglanti { Id = 7, Name = "Laby", Picture = "Laby.png", Description = "Minecraft karakterim.", Link = "https://laby.net/@seyitname" },
                new Baglanti { Id = 9, Name = "Lichess", Picture = "Lichess.png", Description = "Bazen burada da satranç oynarım.", Link = "https://lichess.org/@/seyitname" },
                new Baglanti { Id = 10, Name = "Steam", Picture = "Steam.jpeg", Description = "Steam profilim.", Link = "https://steamcommunity.com/id/seyitname/" },
                new Baglanti { Id = 11, Name = "TruckersMP", Picture = "TruckersMP.png", Description = "ETS2 oynarım.", Link = "https://truckersmp.com/user/5941855" },
                new Baglanti { Id = 12, Name = "Twitch", Picture = "Twitch.png", Description = "Yayın açmam ama takip ederseniz sevinirim.", Link = "https://www.twitch.tv/seyitname" },
                new Baglanti { Id = 13, Name = "WorldOfTrucks", Picture = "WorldOfTrucks.jpeg", Description = "World of Trucks profilim.", Link = "https://www.worldoftrucks.com/en/profile/10601441" },
                new Baglanti { Id = 14, Name = "LinkedIn", Picture = "LinkedIn.png", Description = "LinkedIn profilim.", Link = "https://www.linkedin.com/in/seyitname" }
            );
        }

    }


}