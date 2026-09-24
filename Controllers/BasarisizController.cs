using Microsoft.AspNetCore.Mvc;

namespace SeyitnameWebSite.Controllers;

public class BasarisizController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Account/Basarisiz.cshtml");
    }
}
