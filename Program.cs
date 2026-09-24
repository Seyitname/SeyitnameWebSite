using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --------------------
// DATABASE (PostgreSQL)
// --------------------
builder.Services.AddDbContext<DataContext>(options =>
{
    // 1. Render üzerindeki DATABASE_URL ortam değişkenini kontrol et
    var pgRaw = Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL")
        ?? Environment.GetEnvironmentVariable("DATABASE_URL");

    string connectionString;

    if (!string.IsNullOrWhiteSpace(pgRaw))
    {
        // Render (Production) Bağlantısı
        var uri = new Uri(pgRaw);
        var port = uri.Port > 0 ? uri.Port : 5432;
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;
        var databaseName = uri.AbsolutePath.TrimStart('/');

        connectionString = $"Host={uri.Host};" +
                           $"Port={port};" +
                           $"Database={databaseName};" +
                           $"Username={username};" +
                           $"Password={password};" +
                           $"SSL Mode=Require;Trust Server Certificate=true;";
    }
    else
    {
        // Lokal (appsettings.Development.json veya appsettings.json) Bağlantısı
        connectionString = builder.Configuration.GetConnectionString("database") 
            ?? builder.Configuration.GetConnectionString("DefaultConnection");
    }

    // Hem Lokal hem Production için PostgreSQL Kullan
    options.UseNpgsql(connectionString);
});

// --------------------
// IDENTITY
// --------------------
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// --------------------
// MVC
// --------------------
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DataContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    try
    {
        logger.LogInformation("Applying pending migrations...");
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        throw;
    }

    try
    {
        var roles = new[] { "Admin", "Member", "özel misafir" };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during role seeding.");
    }

    try
    {
        if (!app.Environment.IsDevelopment())
        {
            var users = db.Users.ToList();
            if (users.Count == 1)
            {
                var user = users.First();
                if (!userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during admin role assignment.");
    }
}

// --------------------
// PIPELINE
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();