using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArmyGrievances.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly ICommonGenericFunction _commonGeneric;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, ICommonGenericFunction commonGeneric, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _commonGeneric = commonGeneric;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult DashBoard()
        {
            HttpContext.Session.SetString("User","Admin SNP");
            HttpContext.Session.SetInt32("IsAdmin", 1);
            string DevId = "";           
           
            ViewData["Message"] = TempData["Message"];

            ViewData["showGrid"] = !string.IsNullOrEmpty(DevId) ? "show" : "hide";
            return View();
        }

        public IActionResult Logout()
        {
            AuthenticationHttpContextExtensions.SignOutAsync(HttpContext
           , CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            HttpResponse response = HttpContext.Response;
            response.Clear();
            return Redirect("/Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
