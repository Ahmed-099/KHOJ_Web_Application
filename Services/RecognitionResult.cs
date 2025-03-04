using Newtonsoft.Json;

namespace MissingPersonIdentificationSystem.Services
{
    public class RecognitionResult
    {
        public bool IsMatchFound { get; set; }

        // For missing person reports (when a Finder searches)
        public string MissingPersonName { get; set; } = "";
        public string FamilyMemberEmail { get; set; } = "";
        public string FamilyMemberPhone { get; set; } = "";

        // For found person reports (when a Family Member searches)
        public string FoundPersonName { get; set; } = "";
        public string FinderEmail { get; set; } = "";
        public string FinderPhone { get; set; } = "";

        // PhotoPath for the report's image
        [JsonProperty("photoPath")]
        public string PhotoPath { get; set; } = "";

        public int ReportId { get; set; }
    }
}
