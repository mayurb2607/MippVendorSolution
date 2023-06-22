using Microsoft.AspNetCore.Mvc;
using MippVendorPortal.Models;
using Newtonsoft.Json;
using System.Text;

namespace MippVendorPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly MippVendorTestContext _context;

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public AccountController(IConfiguration configuration, IHostEnvironment hostEnvironment, MippVendorTestContext context)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Vendors
        public IActionResult Index(string msg)
        {
            if (msg != null)
                ViewBag.SuccessMsg = msg;
            return View();
        }

        public async Task<IActionResult> Accept(string vendorID, string clientID)
        {
            VendorClient vendorClient = new VendorClient();
            vendorClient.VendorId = int.Parse(vendorID);
            vendorClient.ClientId = int.Parse(clientID);
            _context.VendorClients.Add(vendorClient);
            await _context.SaveChangesAsync();
            ViewBag.msg = "Thank you for your action. Redirecting you..";
            return RedirectToAction("Index", "Homes");
        }

        public IActionResult Decline()
        {
            ViewBag.msg = "Thank you for your action. Redirecting you..";
            return RedirectToAction("Index");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> ForgotConfirmed(string email)
        //{
        //    var password = _context.Vendors.FirstOrDefault(x => x.VendorEmail == email).VendorPassword;
        //    LoginModel loginModel = new LoginModel();
        //    loginModel.EmailID = email;
        //    loginModel.Password = password;

        //    if (password != null)
        //    {
        //        var env = _hostEnvironment.EnvironmentName;
        //        string apiUrl = string.Empty;
        //        if (env == "Development")
        //        {
        //            // Configure services for development environment
        //            apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //            //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //        }
        //        else
        //        {
        //            // Configure services for local environment
        //            apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //        }


        //        using (var httpClient = new HttpClient())
        //        {
        //            StringContent content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

        //            using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/SendForgotPasswordEmail", content))
        //            {
        //                string apiResponse = await response.Content.ReadAsStringAsync();
        //                //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
        //                return RedirectToAction("Index", "Account", new { msg = "Please check you inbox!" });
        //            }
        //        }
        //    }
        //    return BadRequest();
        //}


        public IActionResult ResetPassword(string email, string msg)
        {
            ViewBag.Email = email;
            ViewBag.Msg = msg;
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> ResetConfirmed(string email, string password, string confirmPassword)
        //{
        //    if (password == confirmPassword)
        //    {
        //        LoginModel loginModel = new LoginModel();
        //        loginModel.EmailID = email;
        //        loginModel.Password = password;

        //        Vendor credentials = _context.Vendors.FirstOrDefault(x => x.VendorEmail == email);
        //        credentials.VendorPassword = password;
        //        _context.Vendors.Update(credentials);
        //        _context.SaveChanges();

        //        if (password != null)
        //        {
        //            var env = _hostEnvironment.EnvironmentName;
        //            string apiUrl = string.Empty;
        //            if (env == "Development")
        //            {
        //                // Configure services for development environment
        //                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //            }
        //            else
        //            {
        //                // Configure services for local environment
        //                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //            }


        //            using (var httpClient = new HttpClient())
        //            {
        //                StringContent content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

        //                using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/SendPasswordResetEmail", content))
        //                {
        //                    string apiResponse = await response.Content.ReadAsStringAsync();
        //                    //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
        //                    return RedirectToAction("Index", "Account", new { msg = "Password resetted successfully! Please continue to login" });
        //                }
        //            }
        //        }
        //        return BadRequest();
        //    }
        //    else
        //    {
        //        ViewBag.Email = email;
        //        return RedirectToAction("ResetPassword", "Account", new { email = email, msg = "Password & Confirm Password not matching!" });
        //    }
        //}

        //[HttpPost]
        //public ActionResult Login(LoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.SuccessMsg = "Login failed! Please go back & re-enter credentials..";
        //        return RedirectToAction("Index");
        //    }

        //    var vendor = _context.Vendors.FirstOrDefault(x => x.VendorEmail == model.EmailID && x.VendorPassword == model.Password);

        //    // User is authenticated, perform additional actions as needed
        //    // For example, set authentication cookies, redirect to home page, etc.
        //    if (vendor != null)
        //    {
        //        ViewBag.vendorId = vendor.Id;
        //        ViewBag.clientId = _context.VendorClients.FirstOrDefault(x => x.VendorId == vendor.Id).ClientId;

        //        return RedirectToAction("Index", "Workorders", new { rootvendorId = vendor.Id });
        //    }

        //    // User authentication failed, display error message
        //    ViewBag.SuccessMsg = "Invalid username or password, please try again..";
        //    return RedirectToAction("Index");
        //}

    }
}
