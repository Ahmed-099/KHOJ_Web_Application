using Microsoft.EntityFrameworkCore;
using MissingPersonIdentificationSystem.Models;

namespace MissingPersonIdentificationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<Finder> Finders { get; set; }
        public DbSet<MissingPerson> MissingPersons { get; set; }
        public DbSet<FoundPerson> FoundPersons { get; set; }
        public DbSet<AdminLogin> AdminLogins { get; set; }
    }
}
