using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAI_Db.Models
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

    }
}
