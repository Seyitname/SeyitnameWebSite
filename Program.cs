using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration["PORT"] ?? "80";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var useLocalSqlite = builder.Environment.IsDevelopment();
var localDatabasePath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "local.db");

if (useLocalSqlite)
{
    Directory.CreateDirectory(Path.GetDirectoryName(localDatabasePath)!);
}

// ------------------------------------------------------------------
// DATABASE & EF CORE CONFIGURATION (Vercel & Supabase Destekli)
// ------------------------------------------------------------------
builder.Services.AddDbContext<DataContext>(options =>
{
    if (useLocalSqlite)
    {
        options.UseSqlite($"Data Source={localDatabasePath}");
    }
    else
    {
        options.UseNpgsql(DatabaseConnectionString.Resolve(builder.Configuration));
    }
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
var initializationResult = new TaskCompletionSource<Exception?>(
    TaskCreationOptions.RunContinuationsAsynchronously);

// Start accepting connections before database initialization so the container
// can pass Vercel's startup probe while migrations are running.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    var initializationError = await initializationResult.Task;
    if (initializationError is not null)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsync("Application initialization failed.");
        return;
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.StartAsync();

try
{
    // ------------------------------------------------------------------
    // DATABASE MIGRATIONS & SEED DATA
    // ------------------------------------------------------------------
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DataContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<SeyitnameWebSite.Data.User>>();

    try
    {
        if (useLocalSqlite)
        {
            logger.LogInformation("Creating local SQLite database...");
            db.Database.EnsureCreated();
        }
        else
        {
            logger.LogInformation("Applying pending migrations...");
            db.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        if (app.Environment.IsDevelopment())
        {
            logger.LogWarning("Local database is unavailable; continuing in Development.");
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

    initializationResult.SetResult(null);
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application initialization failed.");
    initializationResult.TrySetResult(ex);
    await app.StopAsync();
    throw;
}

await app.WaitForShutdownAsync();
