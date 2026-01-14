using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// DATABASE (PostgreSQL)
// --------------------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);

    var userInfo = uri.UserInfo.Split(':');

    connectionString =
        $"Host={uri.Host};" +
        $"Port={uri.Port};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};" +
        $"Password={userInfo[1]};" +
        $"SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("database");
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));


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

// --------------------
// MIGRATION + SEED
// --------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<DataContext>();
    db.Database.Migrate();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var roles = new[] { "Admin", "Member" };

        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }

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
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during DB migration or role seeding");
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
