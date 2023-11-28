using Microsoft.AspNetCore.Mvc;
using MVCAI.Models;

namespace MVCAI.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index(DocumentViewModel vm)
        {

            return View();
        }
    }
}
