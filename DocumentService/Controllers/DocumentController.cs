using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
