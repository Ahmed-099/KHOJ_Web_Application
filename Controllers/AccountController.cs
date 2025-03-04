using Microsoft.AspNetCore.Mvc;
using MissingPersonIdentificationSystem.Models;
using MissingPersonIdentificationSystem.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        //public IActionResult Login() => View();

        public IActionResult Login()
        {            
            return View();
        }
        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter both email and password.";
                return View();
            }

            // Validate credentials (this is pseudocode)
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == ComputeSha256Hash(password));
            if (user != null)
            {
                // Store user details in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);      // e.g., "Finder" or "Family"
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserPhone", user.Phone);
                HttpContext.Session.SetString("UserAddress", user.Address);

                                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }
        }


        // GET: /Account/Signup
        public IActionResult Signup() => View();

        // POST: /Account/Signup
        [HttpPost]
        public async Task<IActionResult> Signup(User model)
        {
            if (!ModelState.IsValid)            
                return View(model);
           

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            // Hash the password
            model.PasswordHash = ComputeSha256Hash(model.PasswordHash); // Assume model.PasswordHash holds plain text password temporarily.
            // Role should be set by user (e.g., "Family" or "Finder")
            _context.Users.Add(model);
            await _context.SaveChangesAsync();
            TempData["Signup"] = "Registered Successfully";
            return RedirectToAction("Login");            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //AdminLogin Work below
        public IActionResult AdminLogin()
        {
            if (HttpContext.Session.GetString("AdminName") != null)
            {
                return RedirectToAction("Index","AccountAdmin");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminLogin adminlogin)
        {
            var AdminData = _context.AdminLogins.Where(x => x.Name == adminlogin.Name && x.Password == adminlogin.Password).FirstOrDefault();
            if (AdminData != null)
            {
                HttpContext.Session.SetString("AdminName", AdminData.Name);
                return RedirectToAction("Index", "AccountAdmin");
            }
            else
            {
                TempData["AdminLoginFailed"] = "Login Failed";
            }
            return View();
        }

        public IActionResult AdminLogout()
        {
            if (HttpContext.Session.GetString("AdminName") != null)
            {
                HttpContext.Session.Remove("AdminName");
                return RedirectToAction("AdminLogin");
            }
            return View();
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
