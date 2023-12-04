namespace MVCAI.Models
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        public string Titel { get; set; }
        public string Hauptkategorie { get; set; } = string.Empty;
        public string Unterkategorie { get; set; } = string.Empty;
        public byte[] File { get; set; }
        public string MetadataNamePlatzhalter { get; set; } = string.Empty;
        public List<MetadataViewModel> Metadaten { get; set; } = new List<MetadataViewModel>();
        public List<ToDoViewModel> ToDos { get; set; } = new List<ToDoViewModel>();
    }

public class MetadataViewModel
{
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;


    }

    public class ToDoViewModel
    {
        public Guid Id { get; set; }
        public Guid DocId { get; set; }

        public string Titel { get; set; }
        public string Beschreibung { get; set; }
        public string Faelligkeit { get; set; }
        public bool Erledigt { get; set; } = false;
    }
}
