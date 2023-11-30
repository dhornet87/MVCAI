using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentDb.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid MaincategoryId { get; set; }
        public Maincategory Maincategory { get; set; }
        public Subcategory Subcategory { get; set; }
        public Guid SubcategoryId { get; set; }
        public byte[]? File { get; set; }
        public ICollection<ToDo>? ToDos { get; set; }
        public ICollection<Metadata>? Metadatas { get; set; }
    }
}
