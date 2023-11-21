
namespace MVCAI.Models
{
    public class ChatGPTTestViewModel
    {
        public string Query { get; set; } = string.Empty;

        public string Response { get; set; } = string.Empty;
        public IFormFile Dateiupload { get; set; }


        public ChatGPTTestViewModel()
        {
                
        }

        public Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
