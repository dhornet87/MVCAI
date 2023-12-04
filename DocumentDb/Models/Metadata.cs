using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DocumentDb.Models
{
    public class Metadata
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Document")]
        public Guid DocId { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;

        public Document Document { get; set; }
    }
}
