using DocumentDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCAI.Models;
using MVCAI.Services;
using System.Diagnostics;

namespace MVCAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DocumentDbContext _documentContext;
        public HomeController(ILogger<HomeController> logger, DocumentDbContext dbContext)
        {
            _logger = logger;
            _documentContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new ChatGPTTestViewModel { Query = "Test", Response = "Test Repsonse" };
            var maincategories = await _documentContext.Maincategories.ToListAsync();
            vm.Response = maincategories.First().Name;
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> QueryChatGPT(ChatGPTTestViewModel vm)
        {
            var newQuery = new OpenAIModel();
            var maincategories = await _documentContext.Maincategories.ToListAsync();
            //vm.Response = maincategories.First().Name;
            await _documentContext.SaveChangesAsync();
            
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
