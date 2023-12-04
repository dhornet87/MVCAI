
namespace MVCAI.Models
{
    public class HomeViewModel
    {

        public string Kategorie { get; set; } = string.Empty;
        public IFormFile Dateiupload { get; set; }

        public List<DocumentViewModel> Documents { get; set; }

        public List<ToDoViewModel> ToDos { get; set; }

    }

 
}
