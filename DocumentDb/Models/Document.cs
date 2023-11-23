using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentDb.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("MainCategory")]
        public int MainCategoryId { get; set; }
        [ForeignKey("SubCategory")]

        public int SubCategorieId { get; set; }
    }
}
