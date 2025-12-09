using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Models;
using SeyitnameWebSite.Data;

namespace SeyitnameWebSite.Controllers;

// AI tarafından yapıldı - Rastgele bağlantı göster
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;

    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Rastgele 3 bağlantı seç
        var randomBaglanti = await _context.Baglantilar
            .OrderBy(x => EF.Functions.Random())
            .Take(3)
            .ToListAsync();
        
        return View(randomBaglanti);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
