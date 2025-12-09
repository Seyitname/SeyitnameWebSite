using Microsoft.AspNetCore.Mvc;

// AI tarafından yapıldı - Legal Pages Controller

namespace SeyitnameWebSite.Controllers
{
    public class LegalController : Controller
    {
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult TermsOfService()
        {
            return View();
        }
    }
}
