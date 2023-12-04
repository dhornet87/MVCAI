using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentDb.Models
{
    public class ToDo
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Document")]
        public Guid DocId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; } = DateTime.Now;
        public bool Done { get; set; } = false;
        public Document Document { get; set; }
    }
}
