using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MissingPersonIdentificationSystem.Services
{
    public class ImageRecognitionService : IImageRecognitionService
    {
        private readonly HttpClient _httpClient;
        public ImageRecognitionService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("http://127.0.0.1:5000/") // Your Python API address
            };
        }

        public async Task<RecognitionResult> RecognizeImageAsync(string imagePath)
        {
            using (var form = new MultipartFormDataContent())
            {
                var stream = File.OpenRead(imagePath);
                var content = new StreamContent(stream);
                content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                form.Add(content, "image", Path.GetFileName(imagePath));

                var response = await _httpClient.PostAsync("api/recognize", form);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<RecognitionResult>(jsonResult);
                    Console.WriteLine("PhotoPath from API: " + result.PhotoPath);
                    return result;
                }
            }
            return new RecognitionResult { IsMatchFound = false };
        }
    }
}
