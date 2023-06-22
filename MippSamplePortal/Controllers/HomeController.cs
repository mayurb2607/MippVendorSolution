using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MippSamplePortal.Models;
using System.Diagnostics;

namespace MippSamplePortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MippTestContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, MippTestContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Url = _configuration.GetValue<string>("LocalEndpoint");
            ViewBag.VendorID = _context.Vendors.Count() + 1;
            ViewBag.Email = TempData["Email"];
            TempData["Email"] = TempData["Email"];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}