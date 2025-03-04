using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MissingPersonIdentificationSystem.Models
{
    public class FoundPersonViewModel
    {
        // Added Id property for identifying the report (used in edit/delete operations)
        public int Id { get; set; }

        [Required]
        public string FoundPersonName { get; set; }
        public string FoundPersonFatherName { get; set; }
        public string FoundPersonGender { get; set; }
        [Required]
        public IFormFile FoundPersonPhoto { get; set; }

        // Reporter details (Finder)
        [Required]
        public string FinderName { get; set; }        // ADD this property
        [Required, EmailAddress]
        public string FinderEmail { get; set; }
        [Required]
        public string FinderPhone { get; set; }
        [Required]
        public string FinderAddress { get; set; }     // ADD this property

        public int FinderId { get; set; } // You can set this from session if needed
    }
}
