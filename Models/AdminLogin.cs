using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MissingPersonIdentificationSystem.Models
{
    public class AdminLogin
    {
        public int Id { get; set; }

        [Required]        
        public string Name { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]        
        public string Password { get; set; } = null!;
    }
}


