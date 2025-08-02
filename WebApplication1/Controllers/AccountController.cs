using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebApplication1.Utilities;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;

        public AccountController(IConfiguration config)
        {
            _db = new DatabaseHelper(config);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var query = "SELECT id, username, role FROM users WHERE username = :username AND password = :password";
            var param = new Dictionary<string, object> {
            { "username", username },
            { "password", password }
        };
            var dt = _db.ExecuteQuery(query);

            if (dt.Rows.Count == 1)
            {
                var role = dt.Rows[0]["role"].ToString();
                var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                return role switch
                {
                    "SuperAdmin" => RedirectToAction("Dashboard", "Admin"),
                    "Patient" => RedirectToAction("Dashboard", "Patient"),
                    "Staff" => RedirectToAction("Dashboard", "Staff"),
                    _ => RedirectToAction("Login")
                };
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
