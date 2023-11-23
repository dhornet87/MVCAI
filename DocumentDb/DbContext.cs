using Microsoft.EntityFrameworkCore;
using DocumentDb.Models;

namespace DocumentDb
{
    public class DocumentDbContext(DbContextOptions<DocumentDbContext> options) :DbContext(options)
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<Maincategory> Maincategories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
    }
}
