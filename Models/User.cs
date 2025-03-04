namespace MissingPersonIdentificationSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }         // Full name
        public string Email { get; set; }        // Unique email (login)
        public string PasswordHash { get; set; } // Hashed password
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }         // "Family" or "Finder"
    }
}
