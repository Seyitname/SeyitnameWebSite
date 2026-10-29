using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration["PORT"] ?? "80";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ------------------------------------------------------------------
// DATABASE & EF CORE CONFIGURATION (Vercel & Supabase Destekli)
// ------------------------------------------------------------------
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(DatabaseConnectionString.Resolve(builder.Configuration));
});

// ------------------------------------------------------------------
// IDENTITY CONFIGURATION
// ------------------------------------------------------------------
// Hata Çözümü: Tip karmaşasını çözmek için DataContext'e bağlı gerçek User sınıfı açıkça yazıldı.
builder.Services.AddIdentity<SeyitnameWebSite.Data.User, IdentityRole>(options =>
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

// ------------------------------------------------------------------
// MVC & COOKIE CONFIGURATION
// ------------------------------------------------------------------
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// ------------------------------------------------------------------
// DATABASE MIGRATIONS & SEED DATA (Otomatik Tablo ve Rol Oluşturma)
// ------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DataContext>();

    // Hata Çözümü: GetRequiredService tipleri eksiksiz belirtildi.
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<SeyitnameWebSite.Data.User>>();

    try
    {
        logger.LogInformation("Applying pending migrations...");
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        if (app.Environment.IsDevelopment())
        {
            logger.LogWarning("Lokal veritabanına bağlanılamadı, ancak hot reload aktif.");
        }
        else
        {
            throw;
        }
    }

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

    try
    {
        if (!app.Environment.IsDevelopment())
        {
            var users = db.Users.ToList();
            if (users.Count == 1)
            {
                var user = users.First();
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during admin role assignment.");
    }
}

// ------------------------------------------------------------------
// MIDDLEWARES & PIPELINE
// ------------------------------------------------------------------
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
