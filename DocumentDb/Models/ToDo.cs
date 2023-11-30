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
        public string Title { get; set; }
        public string Details { get; set; }
        public  DateTime DueDate { get; set; }
        public bool Done { get; set; }
    }
}
