using Microsoft.AspNetCore.Mvc;

namespace SeyitnameWebSite.Controllers;

public class GameController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}