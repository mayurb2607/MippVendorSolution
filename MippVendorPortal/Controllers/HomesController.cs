using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MippVendorPortal.Models;
using System.Diagnostics;

namespace MippVendorPortal.Controllers
{
    [Authorize]
    public class HomesController : Controller
    {
        private readonly ILogger<HomesController> _logger;
        private readonly MippVendorTestContext _context;

        public HomesController(ILogger<HomesController> logger, MippVendorTestContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index(string email)
        {
            try
            {
                string emails = TempData["Email"].ToString();
                ViewBag.VendorId = _context.Vendors.FirstOrDefault(x => x.VendorEmail == emails).RootVendorId;
                return RedirectToAction("Index", "Workorders", new { rootVendorId = _context.Vendors.FirstOrDefault(x => x.VendorEmail == emails).RootVendorId, msg = "" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Workorders");
            }
        }

        [Authorize]
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