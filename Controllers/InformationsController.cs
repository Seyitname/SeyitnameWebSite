using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SeyitnameWebSite.Models;

namespace SeyitnameWebSite.Controllers;

public class InformationsController : Controller
{

    public IActionResult Anlami()
    {
        return View();
    }
    public IActionResult Kimdir()
    {
        return View();
    }
    public IActionResult Tarihi()
    {
        return View();
    }
    public IActionResult Ozellikleri()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
