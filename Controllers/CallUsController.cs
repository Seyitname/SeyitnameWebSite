// AI tarafından yapıldı: Bu dosyada yapılan değişiklikler AI (GitHub Copilot) tarafından önerildi ve eklendi.
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SeyitnameWebSite.Data;
using SeyitnameWebSite.Models;

namespace SeyitnameWebSite.Controllers;

public class CallUsController : Controller
{

    private readonly DataContext? _context;
    public CallUsController(DataContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Index(IletisimBilgileri model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        // AI tarafından eklenen kod bloğu
        // Ensure context is available
        if (_context == null)
        {
            ModelState.AddModelError(string.Empty, "Veritabanı bağlantısı yok.");
            return View(model);
        }

        _context.IBilgiler.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Thanks", new { id = model.Id });
    }

    public async Task<IActionResult> Thanks(int? id)
    {
        if (id == null)
        {
            return View();
        }

        if (_context == null)
        {
            return View();
        }

        var item = await _context.IBilgiler.FindAsync(id.Value);
        return View(item);
    }
}
