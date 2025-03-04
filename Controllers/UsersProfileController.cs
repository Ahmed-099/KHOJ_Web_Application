using Microsoft.AspNetCore.Mvc;
using MissingPersonIdentificationSystem.Data;
using MissingPersonIdentificationSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class UsersProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string userRole = HttpContext.Session.GetString("UserRole"); // "Family" or "Finder"
            string userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";

            if (userId == null || string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userEmail))
            {
                ViewBag.Error = "User not logged in properly.";
                return RedirectToAction("Login", "Account");
            }

            if (userRole == "Family")
            {
                var familyMember = await _context.FamilyMembers.FirstOrDefaultAsync(f => f.Email == userEmail);
                if (familyMember == null)
                {
                    ViewBag.Error = "Family member profile not found.";
                    return View();
                }
                var reports = await _context.MissingPersons.Where(m => m.FamilyMemberId == familyMember.Id).ToListAsync();
                ViewBag.ProfileType = "Family";
                ViewBag.UserName = familyMember.Name;
                ViewBag.Email = familyMember.Email;
                ViewBag.Phone = familyMember.Phone;
                return View(reports); // View will display list of MissingPersons with Edit/Delete links.
            }
            else if (userRole == "Finder")
            {
                var finder = await _context.Finders.FirstOrDefaultAsync(f => f.Email == userEmail);
                if (finder == null)
                {
                    ViewBag.Error = "Finder profile not found.";
                    return View();
                }
                var reports = await _context.FoundPersons.Where(f => f.FinderId == finder.Id).ToListAsync();
                ViewBag.ProfileType = "Finder";
                ViewBag.UserName = finder.Name;
                ViewBag.Email = finder.Email;
                ViewBag.Phone = finder.Phone;
                return View(reports); // View will display list of FoundPersons with Edit/Delete links.
            }
            else
            {
                ViewBag.Error = "Invalid user role.";
                return View();
            }
        }
    }
}
