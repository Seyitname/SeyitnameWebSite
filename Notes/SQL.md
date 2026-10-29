PostgreSQL Geçiş ve Mimari Düzenleme Notları
Bu doküman, projede SQLite altyapısından PostgreSQL altyapısına geçiş sürecinde karşılaşılan sorunları, nedenlerini ve uygulanan çözümleri içeren teknik referans rehberidir.

1. Sorunun Kök Nedeni
Projedeki temel hatalar üç ana etkenden kaynaklanmıştır:

DataContextFactory.cs Yapılandırması (Design-Time Conflict):

Program.cs içerisinde PostgreSQL tanımlanmış olsa bile, terminalden çalıştırılan dotnet ef migrations komutları EF Core tasarımı gereği doğrudan DataContextFactory.cs dosyasına bakar. Bu dosyada UseSqlite("mydb.db") kaldığı için terminal sürekli SQLite mimarisine uygun migration dosyaları üretmiştir.

Veri Tipi Uyumsuzluğu (integer vs boolean):

SQLite üzerinde boolean veri tipi bulunmaz; bunun yerine 0 ve 1 (integer) tutulur. SQLite formatında üretilen migration dosyaları PostgreSQL veritabanına uygulandığında, EmailConfirmed gibi alanlar integer olarak oluşturulmuş ve uygulama içi boolean sorguları çalıştığında veritabanı seviyesinde hatalar oluşmuştur.

Geliştirme Ortamı (dotnet watch) Çökme Sorunu:

Program.cs içerisinde açılış esnasında çalışan db.Database.Migrate() bloğu, yerel veritabanı bağlantısı kurulamadığında veya şifre hatalı olduğunda tüm uygulamayı durdurmakta ve dotnet watch (Hot Reload) sürecini kesmekteydi.

2. Veritabanı ve Konfigürasyon Değişiklikleri
A. DataContextFactory.cs Düzenlemesi
EF CLI araçlarının tasarım anında projeyi PostgreSQL olarak algılaması için DataContextFactory.cs aşağıdaki şekilde dinamik hale getirilmiştir:

C#
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SeyitnameWebSite.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // 1. Ortam konfigürasyonunu oku
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // 2. Connection string bilgisini al
            var connectionString = configuration.GetConnectionString("database") 
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=LOKAL_SIFRENIZ";

            // 3. PostgreSQL (Npgsql) sürücüsünü zorunlu kıl
            optionsBuilder.UseNpgsql(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}
B. Connection String (Bağlantı Cümlesi) Standartı
PostgreSQL parametre bazlı bağlantı yapısı kullanmaktadır.

Npgsql Standart Formatı:

Plaintext
Host=[SUNUCU];Port=[PORT];Database=[VERITABANI];Username=[KULLANICI];Password=[SIFRE];
appsettings.Development.json (Yerel Geliştirme Ortamı):

JSON
{
  "ConnectionStrings": {
    "database": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=GERCEK_LOKAL_SIFRENIZ"
  }
}
appsettings.json (Sürüm Kontrol Sistemine Gidecek Şablon):

JSON
{
  "ConnectionStrings": {
    "database": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=YOUR_PASSWORD"
  }
}
3. Migration ve Veritabanı Sıfırlama Adımları
Eski SQLite bağımlılıklarını temizlemek için uygulanan işlem adımları:

Projedeki mevcut Migrations klasörü tamamen silindi.

Derleme önbelleğini temizlemek için dotnet clean çalıştırıldı.

dotnet ef migrations add InitialPostgreSQL komutu ile PostgreSQL veri tiplerine tam uyumlu ilk migration oluşturuldu.

Render üzerindeki veritabanında şema çakışmalarını önlemek için pgAdmin üzerinden DROP SCHEMA public CASCADE; CREATE SCHEMA public; komutu ile veritabanı şeması sıfırlandı.

4. Program.cs Değişiklikleri ve Teknik Detayları
1. Dinamik Çift Ortam Yapılandırması (Production & Development)
C#
builder.Services.AddDbContext<DataContext>(options =>
{
    // Render üzerindeki DATABASE_URL veya DATABASE_INTERNAL_URL ortam değişkenlerini kontrol et
    var pgRaw = Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL")
        ?? Environment.GetEnvironmentVariable("DATABASE_URL");

    string connectionString;

    if (!string.IsNullOrWhiteSpace(pgRaw))
    {
        // === RENDER (PRODUCTION) ===
        // postgres://user:pass@host:port/db biçimindeki adresi Npgsql formatına ayrıştırır
        var uri = new Uri(pgRaw);
        var port = uri.Port > 0 ? uri.Port : 5432;
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
        var databaseName = uri.AbsolutePath.TrimStart('/');

        connectionString = $"Host={uri.Host};Port={port};Database={databaseName};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
    else
    {
        // === LOKAL (DEVELOPMENT) ===
        connectionString = builder.Configuration.GetConnectionString("database") 
            ?? builder.Configuration.GetConnectionString("DefaultConnection");
    }

    options.UseNpgsql(connectionString);
});
İşlevi: Uygulama Render sunucusuna dağıtıldığında ortam değişkenlerinden canlı veritabanına bağlanır. Yerel ortamda çalışırken appsettings.Development.json içerisindeki yerel veritabanı adresini kullanır.

2. Hata Toleranslı Migration Yönetimi
C#
try
{
    if (!app.Environment.IsDevelopment())
    {
        // Production (Render): Otomatik Migration zorunludur
        logger.LogInformation("Applying pending migrations...");
        db.Database.Migrate();
    }
    else
    {
        // Development: Yerel DB hatasında uygulamanın durmasını engeller
        try
        {
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Yerel veritabanına bağlanılamadı. Hot reload çalışmaya devam ediyor.");
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Database migration failed.");
    throw;
}
İşlevi: Yerel geliştirme sırasında veritabanına erişilemese dahi uygulamanın çökmesini engeller, Hot Reload (dotnet watch) sürecinin kesintisiz devam etmesini sağlar.

3. Asenkron Identity Tohumlama (Seeding)
C#
// Önceki Hatalı Kullanım (Thread Deadlock Riski):
// roleManager.RoleExistsAsync(role).Result 

// Doğru Asenkron Kullanım:
try
{
    var roles = new[] { "Admin", "Member", "özel misafir" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Error during role seeding.");
}
İşlevi: .Result ve .Wait() gibi bloklayıcı metotlar yerine await anahtar kelimesi kullanılarak kilitlenme (deadlock) riskleri ortadan kaldırılmıştır.

5. Özet Mimari Akış
Plaintext
[Lokal Geliştirme] 
   ├── appsettings.Development.json (Yerel PostgreSQL Bağlantısı)
   ├── DataContextFactory.cs (Npgsql ile Migration Üretimi)
   └── dotnet watch run (Kesintisiz Hot Reload)

[Dağıtım - Git Push / Render Deploy]
   ├── Program.cs (DATABASE_URL Algılama ve Ayrıştırma)
   ├── db.Database.Migrate() (Canlı Veritabanı Şema Güncellemesi)
   └── Asenkron Identity Seeding (Roller ve Admin Tanımlaması)