using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAI_Db.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public int MainCategoryId { get; set; }
        public Guid MetadataId { get; set; }

        //FK?!
    }
}
