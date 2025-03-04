using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Check session to see if user is logged in and display appropriate options.
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View();
        }
    }
}
