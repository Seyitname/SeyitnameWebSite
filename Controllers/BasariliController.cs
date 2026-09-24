using Microsoft.AspNetCore.Mvc;

namespace SeyitnameWebSite.Controllers;

public class BasariliController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Account/Basarili.cshtml");
    }
}
