using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --------------------
// DATABASE
// --------------------
var connectionString = builder.Configuration.GetConnectionString("database");

builder.Services.AddDbContext<DataContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        var pgRaw = Environment.GetEnvironmentVariable("DATABASE_INTERNAL_URL");

        if (string.IsNullOrEmpty(pgRaw))
            throw new Exception("DATABASE_INTERNAL_URL is not set");

        var uri = new Uri(pgRaw);
        var port = uri.Port > 0 ? uri.Port : 5432;
        var npgsqlConn = $"Host={uri.Host};Port={port};" +
                         $"Database={uri.AbsolutePath.TrimStart('/')};" +
                         $"Username={uri.UserInfo.Split(':')[0]};" +
                         $"Password={uri.UserInfo.Split(':')[1]};" +
                         $"SSL Mode=Require;Trust Server Certificate=true";

        options.UseNpgsql(npgsqlConn);
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

// --------------------
// MIGRATION + SEED
// --------------------
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DataContext>();

    try
    {
        var applied = db.Database.GetAppliedMigrations().ToList();
        var all = db.Database.GetMigrations().ToList();
        var pending = all.Except(applied).ToList();

        logger.LogInformation($"Applied: {applied.Count}, Pending: {pending.Count}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration check failed.");
    }

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        var roles = new[] { "Admin", "Member", "özel misafir" };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
        }

        var users = db.Users.ToList();
        if (users.Count == 1)
        {
            var user = users.First();
            if (!userManager.IsInRoleAsync(user, "Admin").Result)
                userManager.AddToRoleAsync(user, "Admin").Wait();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during role seeding.");
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