using Microsoft.AspNetCore.Mvc;
using MVCAI.Models;
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

            var response = await newQuery.QueryGPT(vm.Query);
            var newvm = new ChatGPTTestViewModel { Query = vm.Query, Response = response };
            return View("Index", newvm);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
