namespace MissingPersonIdentificationSystem.Models
{
    public class FoundPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public string PhotoPath { get; set; } // e.g., "/uploads/Found_Salman_finder@example.com_9999999999.jpg"
        public int FinderId { get; set; }
        public virtual Finder Finder { get; set; }
    }
}
