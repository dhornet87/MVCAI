using DocumentDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCAI.Models;
using MVCAI.Services;
using System.Diagnostics;
using System.Drawing.Imaging;
using Tesseract;

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
        public async Task<IActionResult> Delete(Guid docId, Guid metadataId)
        {
            var docModel = new DocumentModel(_documentContext);
            
            var doc = await docModel.GetDocument(docId);

            if(await docModel.RemoveMetadata(metadataId))
            {
                var item = doc.Metadata.Find(x => x.Id == metadataId);
                doc.Metadata.Remove(item);
            }
            
            return View("Document", doc);
        }
        public async Task<IActionResult> QueryChatGPT(ChatGPTTestViewModel vm)
        {
            var newQuery = new OpenAIModel();
            var maincategories = await _documentContext.Maincategories.ToListAsync();
            //vm.Response = maincategories.First().Name;
            await _documentContext.SaveChangesAsync();
            
            var tempfile = Path.Combine(Path.GetTempPath(),$"{Path.GetRandomFileName()}.tiff");

            var fs = new MemoryStream();

                await vm.Dateiupload.CopyToAsync(fs);


            //To-Do: Corrss Platform?!
            var pngFile = System.Drawing.Image.FromStream(fs);
            var pngStream = new MemoryStream();
            pngFile.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);


            var service = new OCRService();
            DocumentModel docmodel = new DocumentModel(_documentContext);
            string queryDoc = service.ScanDocument(fs.ToArray());
            string response = await newQuery.QueryGPT(queryDoc);
            DocumentViewModel newDoc = docmodel.ParseGPTText(response);
            newDoc.File = pngStream.ToArray();
            DocumentViewModel newDbDoc = await docmodel.Save(newDoc);

            System.IO.File.Delete(tempfile);
            return View("Document", newDbDoc);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
