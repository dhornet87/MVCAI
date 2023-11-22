using Microsoft.EntityFrameworkCore;
using MVCAI_Db.Models;

namespace MVCAI_Db
{
    public class DocumentContext : DbContext
    {
        public DbSet<Document> Documents { get;set; }
        public DbSet<MainCategory> MainCategories { get;set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options)
        {
            
        }
    }
}
