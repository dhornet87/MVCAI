
namespace MVCAI.Models
{
    public class HomeViewModel
    {

        public string Kategorie { get; set; } = string.Empty;
        public IFormFile Dateiupload { get; set; }

        public List<DocumentViewModel> Documents { get; set; }



    }

 
}
