using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeyitnameWebSite.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // LOCAL için geçici connection string
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=seyitname;Username=postgres;Password=1234"
            );

            return new DataContext(optionsBuilder.Options);
        }
    }
}
