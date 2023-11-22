using Microsoft.AspNetCore.Mvc;
using MVCAI.Models;
using MVCAI.Services;
using System.Diagnostics;

namespace MVCAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new ChatGPTTestViewModel { Query = "Test", Response = "Test Repsonse" };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> QueryChatGPT(ChatGPTTestViewModel vm)
        {
            var newQuery = new OpenAIModel();
            var tempfile = Path.Combine(Path.GetTempPath(),$"{Path.GetRandomFileName()}.tiff");
            
            using (var fs = new FileStream(tempfile, FileMode.CreateNew))
            {

                await vm.Dateiupload.CopyToAsync(fs);

            }

            byte[] doc = System.IO.File.ReadAllBytes(tempfile);

            var service = new OCRService();
            var queryDoc = service.ScanDocument(doc);
            var response = await newQuery.QueryGPT(queryDoc);
            //response parsen für Anlage von Dokumenten

            var newvm = new ChatGPTTestViewModel {Response = response };
            System.IO.File.Delete(tempfile);
            return View("Index", newvm);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
