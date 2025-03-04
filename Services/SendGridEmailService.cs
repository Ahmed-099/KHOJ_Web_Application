using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MissingPersonIdentificationSystem.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string attachmentFilePath = null)
        {
            try
            {
                string apiKey = _configuration["SendGrid:ApiKey"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
                var toAddress = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);

                // Add attachment if provided and file exists
                if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(attachmentFilePath);
                    var fileBase64 = Convert.ToBase64String(fileBytes);
                    string fileName = Path.GetFileName(attachmentFilePath);
                    msg.AddAttachment(fileName, fileBase64);
                }

                var response = await client.SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                    response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"SendGrid API returned status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message, ex);
            }
        }
    }
}
