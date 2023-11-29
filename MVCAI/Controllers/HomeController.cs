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
            var vm = new HomeViewModel();
            var docModel = new DocumentModel(_documentContext);

            vm.Documents = await docModel.GetDocuments();
           
            return View(vm);
        }
        public async Task<IActionResult> SaveDoc(DocumentViewModel vm, Guid id)
        {
            var docModel = new DocumentModel(_documentContext);

            _ = await docModel.Save(vm);
            var vmHome = new HomeViewModel();

            vmHome.Documents = await docModel.GetDocuments();

            return View("Index",vmHome);
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
        public async Task<IActionResult> ViewDocument(Guid docId)
        {
            var docModel = new DocumentModel(_documentContext);

            var vm = await docModel.GetDocument(docId);

            return View("Document", vm);
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
            string response = await newQuery.QueryGPT(queryDoc);
            DocumentViewModel newDoc = docmodel.ParseGPTText(response);
            newDoc.File = pngStream.ToArray();
            DocumentViewModel newDbDoc = await docmodel.Save(newDoc);

            return View("Document", newDbDoc);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
