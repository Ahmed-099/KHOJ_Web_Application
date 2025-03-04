using System.Threading.Tasks;

namespace MissingPersonIdentificationSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, string attachmentFilePath = null);
    }
}
