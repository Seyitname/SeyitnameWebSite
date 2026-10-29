using Npgsql;

namespace SeyitnameWebSite.Data;

internal static class DatabaseConnectionString
{
    public static string Resolve(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = configuration.GetConnectionString("database")
                ?? configuration.GetConnectionString("DefaultConnection");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection is not configured. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");
        }

        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException("DATABASE_URL is not a valid PostgreSQL connection URI.");
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        var databaseName = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
        if (userInfo.Length == 0 || string.IsNullOrWhiteSpace(userInfo[0]) || string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("DATABASE_URL must include a username and database name.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = databaseName,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require,
            Pooling = true
        };

        return builder.ConnectionString;
    }
}
