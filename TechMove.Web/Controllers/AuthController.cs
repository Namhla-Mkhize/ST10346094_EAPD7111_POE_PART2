using Microsoft.AspNetCore.Mvc;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var token = await _apiService.LoginAsync(username, password);

            if (token == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // Store JWT token in session
            HttpContext.Session.SetString("JwtToken", token);
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}