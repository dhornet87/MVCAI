namespace MVCAI.Models
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        public string Maincategory { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public byte[] File { get; set; }

        public List<MetadataViewModel> Metadata { get; set; }
    }

public class MetadataViewModel
{
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;


    }
}
