using System.Threading.Tasks;

namespace MissingPersonIdentificationSystem.Services
{
    public interface IImageRecognitionService
    {
        Task<RecognitionResult> RecognizeImageAsync(string imagePath);
    }
}
