using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeyitnameWebSite.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // LOCAL için SQLite
            optionsBuilder.UseSqlite("Data Source = mydb.db");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
