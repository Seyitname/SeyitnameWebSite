using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;
using SeyitnameWebSite.Hubs;

// 🔥 BUNU EKLE (EN ÜSTE)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);


// --------------------
// DATABASE (PostgreSQL)
// --------------------
// --------------------
// DATABASE
// --------------------
var connectionString = builder.Configuration.GetConnectionString("database");

builder.Services.AddDbContext<DataContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // LOCAL → SQLite
        options.UseSqlite(connectionString);
    }
    else
    {
        // PRODUCTION → PostgreSQL (Render)
        var pgConnection =
            Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL");

        if (string.IsNullOrEmpty(pgConnection))
            throw new Exception("DATABASE_INTERNAL_URL is not set");

        options.UseNpgsql(pgConnection);
    }
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

    // Email confirmation kaldırıldı (3. madde)
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// --------------------
// SIGNALR (Chat için)
// --------------------
builder.Services.AddSignalR();

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
// --------------------
// MIGRATION + SEED
// --------------------
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
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

var pgConnection = Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL");

// postgresql:// formatını Npgsql'e uygun hale getir
var uri = new Uri(pgConnection!);
var npgsqlConn = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";

options.UseNpgsql(npgsqlConn);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Chat hub'ını map et
app.MapHub<ChatHub>("/chatHub");

app.Run();
