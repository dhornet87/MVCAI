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
            var vm = new HomeViewModel();
            var docModel = new DocumentModel(_documentContext);

            vm.Documents = await docModel.GetDocuments();
            vm.ToDos = await docModel.GetTodos();
           
            return View(vm);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult ViewDocument(Guid id)
        {
            return RedirectToAction("Index", "Document", new { id = id });
        }

        public async Task<IActionResult> ToDoDone(Guid id)
        {
            var docModel = new DocumentModel(_documentContext);

            await docModel.SetToDoDone(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> QueryChatGPT(HomeViewModel vm)
        {
            
            var newQuery = new OpenAIModel();
            var maincategories = await _documentContext.Maincategories.ToListAsync();
            await _documentContext.SaveChangesAsync();
            

            var fs = new MemoryStream();

                await vm.Dateiupload.CopyToAsync(fs);


            //To-Do: Corrss Platform?!
            var pngFile = System.Drawing.Image.FromStream(fs);
            var pngStream = new MemoryStream();
            pngFile.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);


            var service = new OCRService();
            DocumentModel docmodel = new DocumentModel(_documentContext);
            string queryDoc = service.ScanDocument(fs.ToArray());
            //To Do Kategorie auswählbar machen 
            DocumentViewModel newDoc = await OpenAIModel.QueryGPT(queryDoc);
            newDoc.File = pngStream.ToArray();
            DocumentViewModel newDbDoc = await docmodel.Save(newDoc);

            return RedirectToAction("Index","Document", new { id = newDbDoc.Id});
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
