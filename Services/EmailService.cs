//using System;
//using System.Threading.Tasks;
//using MailKit.Net.Smtp;
//using MimeKit;
//using Microsoft.Extensions.Configuration;

//namespace MissingPersonIdentificationSystem.Services
//{
//    public interface IEmailService
//    {
//        Task SendEmailAsync(string toEmail, string subject, string body);
//    }

//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _configuration;
//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            // Force TLS 1.2
//            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
//        }
        
//        public async Task SendEmailAsync(string toEmail, string subject, string body)
//        {
//            try
//            {
//                var message = new MimeMessage();
//                message.From.Add(new MailboxAddress("Sender", _configuration["Smtp:From"]));
//                message.To.Add(new MailboxAddress("", toEmail));
//                message.Subject = subject;
//                message.Body = new TextPart("html") { Text = body };

//                using (var client = new SmtpClient())
//                {
//                    // Accept all certificates (for development; remove in production)
//                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

//                    // Try connecting with StartTLS on port 587
//                    await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);

//                    await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);

//                    await client.SendAsync(message);
//                    await client.DisconnectAsync(true);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Email sending failed: " + ex.Message, ex);
//            }
//        }
//    }
//}
