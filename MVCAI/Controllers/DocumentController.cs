using DocumentDb;
using Microsoft.AspNetCore.Mvc;
using MVCAI.Models;

namespace MVCAI.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DocumentDbContext _documentContext;

        public DocumentController(DocumentDbContext documentContext)
        {
            _documentContext = documentContext;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var docModel = new DocumentModel(_documentContext);

            var vm = await docModel.GetDocument(id);
            return View(vm);
        }
        public async Task AddMetadata()
        {

        }
        public async Task<IActionResult> Delete(Guid docId, Guid metadataId)
        {
            if (docId == Guid.Empty || metadataId == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            var docModel = new DocumentModel(_documentContext);

            var doc = await docModel.GetDocument(docId);

            if (await docModel.RemoveMetadata(metadataId))
            {
                var item = doc.Metadaten.Find(x => x.Id == metadataId);
                doc.Metadaten.Remove(item);
            }

            return RedirectToAction("Index", new { id = docId });
        }
        public async Task<IActionResult> SaveDoc(DocumentViewModel vm, Guid id)
        {
            var docModel = new DocumentModel(_documentContext);

            _ = await docModel.Save(vm);
            var vmHome = new HomeViewModel();

            vmHome.Documents = await docModel.GetDocuments();
            return RedirectToAction("Index", "Home");
            //return View("Index",vmHome);
        }
    }
}
