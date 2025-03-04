namespace MissingPersonIdentificationSystem.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }    // Must match a User with Role "Family"
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
