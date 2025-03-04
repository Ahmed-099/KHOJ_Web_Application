using Microsoft.AspNetCore.Mvc;
using MissingPersonIdentificationSystem.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MissingPersonIdentificationSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly IImageRecognitionService _imageRecognitionService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public SearchController(IImageRecognitionService imageRecognitionService, IEmailService emailService, ApplicationDbContext context)
        {
            _imageRecognitionService = imageRecognitionService;
            _emailService = emailService;
            _context = context;
        }

        // GET: /Search/Index
        public IActionResult Index()
        {
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            return View();
        }


        // POST: /Search/Recognize
        [HttpPost]
        public async Task<IActionResult> Recognize(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "Please upload an image.";
                return View("Index");
            }

            // Save the uploaded image temporarily in wwwroot/temp
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string tempFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string tempFilePath = Path.Combine(tempFolder, tempFileName);
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Call the image recognition service
            var result = await _imageRecognitionService.RecognizeImageAsync(tempFilePath);
            // (Do not delete the file immediately; we need it as an attachment)



            //// Optionally, query the database to set ReportId based on PhotoPath.
            //string currentUserRole = HttpContext.Session.GetString("UserRole");
            //if (result != null && result.IsMatchFound && !string.IsNullOrEmpty(result.PhotoPath))
            //{
            //    if (currentUserRole == "Family")
            //    {
            //        // Family Member search: expected to match a FoundPerson report.
            //        var foundReport = await _context.FoundPersons.FirstOrDefaultAsync(f => f.PhotoPath == result.PhotoPath);
            //        if (foundReport != null)
            //            result.ReportId = foundReport.Id;
            //    }
            //    else if (currentUserRole == "Finder")
            //    {
            //        // Finder search: expected to match a MissingPerson report.
            //        var missingReport = await _context.MissingPersons.FirstOrDefaultAsync(m => m.PhotoPath == result.PhotoPath);
            //        if (missingReport != null)
            //            result.ReportId = missingReport.Id;
            //    }
            //}






            // Retrieve session details
            string currentUserRole = HttpContext.Session.GetString("UserRole"); // "Family" or "Finder"
            string currentUserName = HttpContext.Session.GetString("UserName") ?? "Not Provided";
            string currentUserEmail = HttpContext.Session.GetString("UserEmail") ?? "Not Provided";
            string currentUserPhone = HttpContext.Session.GetString("UserPhone") ?? "Not Provided";

            try
            {
                if (result != null && result.IsMatchFound)
                {
                    // When a Family Member is logged in, we expect a Found report; email goes to the Finder.
                    if (currentUserRole == "Family")
                    {
                        if (!string.IsNullOrWhiteSpace(result.FinderEmail) && !string.IsNullOrWhiteSpace(result.FoundPersonName))
                        {
                            string recipientEmail = result.FinderEmail;
                            string subject = "Match Found for Your Reported Found Person";
                            string body = $"Dear Finder,<br/><br/>" +
                                          $"A match has been found for the reported found person: <strong>{result.FoundPersonName}</strong>.<br/><br/>" +
                                          $"Family Member Details (searcher's info):<br/>" +
                                          $"Name: {currentUserName}<br/>" +
                                          $"Email: {currentUserEmail}<br/>" +
                                          $"Phone: {currentUserPhone}<br/><br/>" +
                                          $"Please contact the family member for further information.";
                            // Pass the temporary file path as attachment
                            await _emailService.SendEmailAsync(recipientEmail, subject, body, tempFilePath);
                            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                            return View("Result", result);
                        }
                        else
                        {
                            ViewBag.Error = "Found person report data is incomplete.";
                            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                            return View("Result", result);
                        }
                    }
                    // When a Finder is logged in, we expect a Missing report; email goes to the Family Member.
                    else if (currentUserRole == "Finder")
                    {
                        if (!string.IsNullOrWhiteSpace(result.FamilyMemberEmail) && !string.IsNullOrWhiteSpace(result.MissingPersonName))
                        {
                            string recipientEmail = result.FamilyMemberEmail;
                            string subject = "Match Found for Your Reported Missing Person";
                            string body = $"Dear Family Member,<br/><br/>" +
                                          $"A match has been found for your reported missing person: <strong>{result.MissingPersonName}</strong>.<br/><br/>" +
                                          $"Finder Details (searcher's info):<br/>" +
                                          $"Name: {currentUserName}<br/>" +
                                          $"Email: {currentUserEmail}<br/>" +
                                          $"Phone: {currentUserPhone}<br/><br/>" +
                                          $"Please contact the finder for further information.";
                            await _emailService.SendEmailAsync(recipientEmail, subject, body, tempFilePath);
                            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                            return View("Result", result);
                        }
                        else
                        {
                            ViewBag.Error = "Missing person report data is incomplete.";
                            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                            return View("Result", result);
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Invalid user role.";
                        ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                        return View("Result", result);
                    }
                }
                else
                {
                    ViewBag.Error = "No match found.";
                    ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail") ?? "";
                    return View("Result", result);
                }
            }
            finally
            {
                // Delete the temporary file after email is sent (or if an exception occurs)
                if (System.IO.File.Exists(tempFilePath))
                    System.IO.File.Delete(tempFilePath);
            }
        }

    }
}
