namespace MissingPersonIdentificationSystem.Models
{
    public class Finder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }    // Must match a User with Role "Finder"
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
