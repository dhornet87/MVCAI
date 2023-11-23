using System.ComponentModel.DataAnnotations;

namespace DocumentDb.Models
{
    public class Maincategory
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
