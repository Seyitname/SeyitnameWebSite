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
            // appsettings.json veya appsettings.Development.json'dan bağlantı cümlesini oku
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // appsettings içindeki 'database' veya 'DefaultConnection' anahtarını al
            var connectionString = configuration.GetConnectionString("database") 
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5071;Database=mydb;Username=postgres;Password=LOKAL_SIFRENIZ";

            // Artık Tasarım Anında da (Design-Time) PostgreSQL Kullanılacak
            optionsBuilder.UseNpgsql(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}