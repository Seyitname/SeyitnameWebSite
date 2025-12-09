// AI tarafından yapıldı: Bu dosyada yapılan değişiklikler AI (GitHub Copilot) tarafından önerildi ve eklendi.
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeyitnameWebSite.Data;
using SeyitnameWebSite.Models;

namespace SeyitnameWebSite.Controllers;

public class BaglantiController : Controller
{
    private readonly DataContext _context;
    public BaglantiController(DataContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        return View(await _context.Baglantilar.ToListAsync());
    }
}