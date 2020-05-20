using CRUDWebService.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDWebService.DataLayer.Context
{
    public class UniversityContext : DbContext
    {
        public DbSet<University> Universities { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<UniversityBook> UniversityBooks { get; set; }

        public UniversityContext(DbContextOptions options) : base (options)
        {
            Database.EnsureCreated();
        }
    }
}
