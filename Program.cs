// AI tarafından yapıldı: Program.cs içinde yapılan DB ve routing değişiklikleri AI tarafından eklendi.
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SeyitnameWebSite.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("database");
    options.UseSqlite(connectionString);
});

// AI tarafından yapıldı - Identity konfigürasyonu
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// AI tarafından yapıldı: Veritabanı migration'larını otomatik olarak uygula
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();

    // Ensure roles exist and seed Admin role if missing
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Seed default roles
        var defaultRoles = new[] { "Admin", "Member" };
        foreach (var roleName in defaultRoles)
        {
            var exists = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
            if (!exists)
            {
                logger.LogInformation("Creating '{role}' role in database.", roleName);
                roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }
        }

        // If there is exactly one user, make them Admin for initial setup
        var users = db.Users.ToList();
        if (users.Count == 1)
        {
            var firstUser = users[0];
            if (!userManager.IsInRoleAsync(firstUser, "Admin").GetAwaiter().GetResult())
            {
                logger.LogInformation("Assigning 'Admin' role to first user: {UserId}", firstUser.Id);
                userManager.AddToRoleAsync(firstUser, "Admin").GetAwaiter().GetResult();
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while ensuring roles are created");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// AI tarafından yapıldı - Authentication ve Authorization pipeline'ı
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}");

app.Run();