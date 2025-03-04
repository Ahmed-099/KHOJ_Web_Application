using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MissingPersonIdentificationSystem.Models
{
    public class MissingPersonViewModel
    {
        // This Id property is used for editing or deleting an existing report.
        public int Id { get; set; }

        [Required]
        public string MissingPersonName { get; set; }
        public string MissingPersonFatherName { get; set; }
        public string MissingPersonGender { get; set; }
        [Required]
        public IFormFile MissingPersonPhoto { get; set; }

        // Reporter details (Family Member)
        [Required]
        public string FamilyMemberName { get; set; }     // ADD this property
        [Required, EmailAddress]
        public string FamilyMemberEmail { get; set; }
        [Required]
        public string FamilyMemberPhone { get; set; }
        [Required]
        public string FamilyMemberAddress { get; set; }  // ADD this property

        public int FamilyMemberId { get; set; } // You can set this from session if needed
    }
}

