namespace MissingPersonIdentificationSystem.Models
{
    public class MissingPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public string PhotoPath { get; set; } // e.g., "/uploads/Missing_Hamza_family@example.com_1234567890.jpg"
        public int FamilyMemberId { get; set; }
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
