using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentDb.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("MainCategory")]
        public Guid MainCategoryId { get; set; }
        [ForeignKey("SubCategory")]

        public Guid SubCategorieId { get; set; }
        public byte[]? File { get; set; }
    }
}
