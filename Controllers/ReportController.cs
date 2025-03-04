using Microsoft.AspNetCore.Mvc;
using MissingPersonIdentificationSystem.Models;
using MissingPersonIdentificationSystem.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report/ReportMissingPerson
        public IActionResult ReportMissingPerson()
        {
            // Retrieve reporter details from session (set during login)
            var familyMemberName = HttpContext.Session.GetString("UserName") ?? "";
            var familyMemberEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            var familyMemberPhone = HttpContext.Session.GetString("UserPhone") ?? "";
            var familyMemberAddress = HttpContext.Session.GetString("UserAddress") ?? "";

            // Create a new view model and pre-populate the reporter fields
            var model = new MissingPersonViewModel
            {
                FamilyMemberName = familyMemberName,
                FamilyMemberEmail = familyMemberEmail,
                FamilyMemberPhone = familyMemberPhone,
                FamilyMemberAddress = familyMemberAddress
            };

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }


        // POST: Report/ReportMissingPerson
        [HttpPost]
        public async Task<IActionResult> ReportMissingPerson(MissingPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the uploaded image in the "uploads" folder.
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate filename with the "Missing_" prefix.
                string fileName = $"Missing_{model.MissingPersonName.Trim().Replace(" ", "")}_{model.FamilyMemberEmail.Trim()}_{model.FamilyMemberPhone.Trim()}.jpg";
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MissingPersonPhoto.CopyToAsync(stream);
                }

                // Save or retrieve the FamilyMember record.
                var familyMember = _context.FamilyMembers.FirstOrDefault(f => f.Email == model.FamilyMemberEmail);
                if (familyMember == null)
                {
                    familyMember = new FamilyMember
                    {
                        Name = model.FamilyMemberName,
                        Email = model.FamilyMemberEmail,
                        Phone = model.FamilyMemberPhone,
                        Address = model.FamilyMemberAddress
                    };
                    _context.FamilyMembers.Add(familyMember);
                    await _context.SaveChangesAsync();
                }

                var missingReport = new MissingPerson
                {
                    Name = model.MissingPersonName,
                    FatherName = model.MissingPersonFatherName,
                    Gender = model.MissingPersonGender,
                    PhotoPath = "/uploads/" + fileName,
                    FamilyMemberId = familyMember.Id
                };
                _context.MissingPersons.Add(missingReport);
                await _context.SaveChangesAsync();


                // Copy the image to the known_faces folder.
                string knownFacesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "known_faces");
                if (!Directory.Exists(knownFacesFolder))
                    Directory.CreateDirectory(knownFacesFolder);
                string destinationPath = Path.Combine(knownFacesFolder, fileName);
                System.IO.File.Copy(filePath, destinationPath, true);

                ViewBag.Message = "Missing person report submitted successfully.Now you will wait for the email notification";
                ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                return View("ReportSuccess");
            }
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }

        // GET: Report/EditMissingPerson/{id}
        public async Task<IActionResult> EditMissingPerson(int id)
        {
            var report = await _context.MissingPersons.Include(m => m.FamilyMember)
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
                return NotFound();
            var viewModel = new MissingPersonViewModel
            {
                Id = report.Id,
                MissingPersonName = report.Name,
                MissingPersonFatherName = report.FatherName,
                MissingPersonGender = report.Gender,
                // Note: We cannot bind an IFormFile; if user doesn't update image, this remains empty.
                FamilyMemberName = report.FamilyMember.Name,
                FamilyMemberEmail = report.FamilyMember.Email,
                FamilyMemberPhone = report.FamilyMember.Phone,
                FamilyMemberAddress = report.FamilyMember.Address
            };
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(viewModel);
        }

        // POST: Report/EditMissingPerson
        [HttpPost]
        public async Task<IActionResult> EditMissingPerson(MissingPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = await _context.MissingPersons.FirstOrDefaultAsync(m => m.Id == model.Id);
                if (report == null)
                    return NotFound();
                report.Name = model.MissingPersonName;
                report.FatherName = model.MissingPersonFatherName;
                report.Gender = model.MissingPersonGender;
                // If new image uploaded, update the image
                if (model.MissingPersonPhoto != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    string fileName = $"Missing_{model.MissingPersonName.Trim().Replace(" ", "")}_{model.FamilyMemberEmail.Trim()}_{model.FamilyMemberPhone.Trim()}.jpg";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.MissingPersonPhoto.CopyToAsync(stream);
                    }
                    report.PhotoPath = "/uploads/" + fileName;
                    string knownFacesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "known_faces");
                    if (!Directory.Exists(knownFacesFolder))
                        Directory.CreateDirectory(knownFacesFolder);
                    string destinationPath = Path.Combine(knownFacesFolder, fileName);
                    System.IO.File.Copy(filePath, destinationPath, true);
                }
                _context.MissingPersons.Update(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UsersProfile");
            }
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }

        // GET: Report/DeleteMissingPerson/{id}
        public async Task<IActionResult> DeleteMissingPerson(int id)
        {
            var report = await _context.MissingPersons.Include(m => m.FamilyMember)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
                return NotFound();
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(report);
        }

        // POST: Report/DeleteMissingPersonConfirmed
        [HttpPost, ActionName("DeleteMissingPerson")]
        public async Task<IActionResult> DeleteMissingPersonConfirmed(int id)
        {
            var report = await _context.MissingPersons.FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
                return NotFound();
            _context.MissingPersons.Remove(report);
            await _context.SaveChangesAsync();
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return RedirectToAction("Index", "UsersProfile");
        }

        // GET: Report/ReportFoundPerson
        public IActionResult ReportFoundPerson()
        {
            // Retrieve reporter details from session (set during login)
            var finderName = HttpContext.Session.GetString("UserName") ?? "";
            var finderEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            var finderPhone = HttpContext.Session.GetString("UserPhone") ?? "";
            var finderAddress = HttpContext.Session.GetString("UserAddress") ?? "";

            // Create a new view model and pre-populate the reporter fields
            var model = new FoundPersonViewModel
            {
                FinderName = finderName,
                FinderEmail = finderEmail,
                FinderPhone = finderPhone,
                FinderAddress = finderAddress
            };

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }


        // POST: Report/ReportFoundPerson
        [HttpPost]
        public async Task<IActionResult> ReportFoundPerson(FoundPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate filename with the "Found_" prefix.
                string fileName = $"Found_{model.FoundPersonName.Trim().Replace(" ", "")}_{model.FinderEmail.Trim()}_{model.FinderPhone.Trim()}.jpg";
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FoundPersonPhoto.CopyToAsync(stream);
                }

                // Save or retrieve the Finder record.
                var finder = _context.Finders.FirstOrDefault(f => f.Email == model.FinderEmail);
                if (finder == null)
                {
                    finder = new Finder
                    {
                        Name = model.FinderName, // Optionally, a separate Finder name field.
                        Email = model.FinderEmail,
                        Phone = model.FinderPhone,
                        Address = model.FinderAddress
                    };
                    _context.Finders.Add(finder);
                    await _context.SaveChangesAsync();
                }

                // Save the found person report.
                var foundReport = new FoundPerson
                {
                    Name = model.FoundPersonName,
                    FatherName = model.FoundPersonFatherName,
                    Gender = model.FoundPersonGender,
                    PhotoPath = "/uploads/" + fileName,
                    FinderId = finder.Id
                };
                _context.FoundPersons.Add(foundReport);
                await _context.SaveChangesAsync();

                // Copy the image to the known_faces folder.
                string knownFacesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "known_faces");
                if (!Directory.Exists(knownFacesFolder))
                    Directory.CreateDirectory(knownFacesFolder);
                string destinationPath = Path.Combine(knownFacesFolder, fileName);
                System.IO.File.Copy(filePath, destinationPath, true);

                ViewBag.Message = "Found person report submitted successfully.Now you will wait for the email notification.";
                ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                return View("ReportSuccess");
            }
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }

        // GET: Report/EditFoundPerson/{id}
        public async Task<IActionResult> EditFoundPerson(int id)
        {
            var report = await _context.FoundPersons.Include(f => f.Finder)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (report == null)
                return NotFound();
            var viewModel = new FoundPersonViewModel
            {
                Id = report.Id,
                FoundPersonName = report.Name,
                FoundPersonFatherName = report.FatherName,
                FoundPersonGender = report.Gender,
                FinderName = report.Finder.Name,
                FinderEmail = report.Finder.Email,
                FinderPhone = report.Finder.Phone,
                FinderAddress = report.Finder.Address
            };
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(viewModel);
        }

        // POST: Report/EditFoundPerson
        [HttpPost]
        public async Task<IActionResult> EditFoundPerson(FoundPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var report = await _context.FoundPersons.FirstOrDefaultAsync(f => f.Id == model.Id);
                if (report == null)
                    return NotFound();
                report.Name = model.FoundPersonName;
                report.FatherName = model.FoundPersonFatherName;
                report.Gender = model.FoundPersonGender;
                if (model.FoundPersonPhoto != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    string fileName = $"Found_{model.FoundPersonName.Trim().Replace(" ", "")}_{model.FinderEmail.Trim()}_{model.FinderPhone.Trim()}.jpg";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.FoundPersonPhoto.CopyToAsync(stream);
                    }
                    report.PhotoPath = "/uploads/" + fileName;
                    string knownFacesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "known_faces");
                    if (!Directory.Exists(knownFacesFolder))
                        Directory.CreateDirectory(knownFacesFolder);
                    string destinationPath = Path.Combine(knownFacesFolder, fileName);
                    System.IO.File.Copy(filePath, destinationPath, true);
                }
                _context.FoundPersons.Update(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UsersProfile");
            }
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(model);
        }

        // GET: Report/DeleteFoundPerson/{id}
        public async Task<IActionResult> DeleteFoundPerson(int id)
        {
            var report = await _context.FoundPersons.Include(f => f.Finder)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (report == null)
                return NotFound();
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View(report);
        }

        // POST: Report/DeleteFoundPersonConfirmed
        [HttpPost, ActionName("DeleteFoundPerson")]
        public async Task<IActionResult> DeleteFoundPersonConfirmed(int id)
        {
            var report = await _context.FoundPersons.FirstOrDefaultAsync(f => f.Id == id);
            if (report == null)
                return NotFound();
            _context.FoundPersons.Remove(report);
            await _context.SaveChangesAsync();
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return RedirectToAction("Index", "UsersProfile");
        }
    }
}

