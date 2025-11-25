using ZakupkiGovRuIntegration.Models;
using ZakupkiGovRuIntegration.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace ZakupkiGovRuIntegration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPurchaseService _service;

        public HomeController(ILogger<HomeController> logger,IPurchaseService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetData(PurchaseModel product)
        {
            var products = await _service.Find(product.RegNumber);
            return View(products);
        }

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
