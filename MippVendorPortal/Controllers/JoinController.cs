using dotless.Core.Parser.Infrastructure;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MippPortalWebAPI.Helpers;

using MippVendorPortal.Areas.Identity.Data;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using MippVendorPortal.Helpers;
using Newtonsoft.Json;
using System.Text;
using AspNetUser = MippVendorPortal.Models.AspNetUser;

namespace MippVendorPortal.Controllers
{
    public class JoinController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly UserManager<MippVendorPortalUser> _userManager;
        private readonly SignInManager<MippVendorPortalUser> _signInManager;

        public JoinController(MippVendorTestContext context, UserManager<MippVendorPortalUser> userManager,
            SignInManager<MippVendorPortalUser> signInManager,IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        // GET: Join

        public async Task<IActionResult> Index(string e, string cId, string rvId)
        {
            CryptographyHelper cryptographyHelper = new CryptographyHelper();
            var mail = cryptographyHelper.DecryptString(e);

            var mailpresent = _context.Vendors.FirstOrDefault(x => x.VendorEmail == mail);

            //get vendor details from vendorList
            VendorList vendorSaved = null;
            VendorRequest vendorRequest = new VendorRequest();
            vendorRequest.email = mail;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vendorRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Vendors/GetVendorData", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        vendorSaved = JsonConvert.DeserializeObject<VendorList>(apiResponse);

                       
                     
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (mailpresent == null)
            {
                if (e != null && cId != null)
                {
                    ViewBag.Email = mail;
                    ViewBag.ClientId = Convert.ToInt32(cryptographyHelper.DecryptString(cId));
                    ViewBag.RootVendorId = Convert.ToInt32(cryptographyHelper.DecryptString(rvId));
                    ViewBag.Company = vendorSaved.BusinessName;
                    ViewBag.FullName = vendorSaved.VendorName;
                    ViewBag.PhoneNo = vendorSaved.VendorPhone;
                }

                return View();
            }
            ViewBag.ClientId = cId;
            ViewBag.VendorId = rvId;
            ViewBag.Email = mail;
            ViewBag.msg = "Your account already exists!";
            var model = new AcceptInvitationLoginModel();
            model.Email=mail;
            model.ClientId = Convert.ToInt32(cryptographyHelper.DecryptString(cId));
            model.VendorId = Convert.ToInt32(cryptographyHelper.DecryptString(cId));
            model.Password=string.Empty;
            return View("AcceptInvitationLogin",model);

        }

        // GET: Join/Details/5
        public async Task<IActionResult> Accept(AcceptInvitationLoginModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    ViewData["Email"] = model.Email;
                    TempData["Email"] = model.Email;
                    
                }
                else
                {
                    ViewBag.Msg("Unable to login Wron Credintials");

                    return View(model);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            var username = User.Identity.Name;
            
            

            var vendor = _context.Vendors.First(x => x.VendorEmail == username);
            vendor.RootVendorId = model.VendorId;
            await _context.SaveChangesAsync();



            MippVendorPortal.Models.VendorClient vendorClient = new MippVendorPortal.Models.VendorClient();
            vendorClient.Id = _context.VendorClients.Count() + 1;
            vendorClient.VendorId = vendor.Id;
            vendorClient.ClientId = model.ClientId;
            if (_context.VendorClients.FirstOrDefault(x => x.VendorId == vendorClient.Id && x.ClientId == vendorClient.ClientId) == null)
            {
                _context.VendorClients.Add(vendorClient);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Hey! You are already registered..");
            }


            VendorInvite vendorInvite = new VendorInvite();
            vendorInvite.Id = vendor.Id;
            vendorInvite.VendorId = model.VendorId;
            vendorInvite.VendorEmail = model.Email;
            vendorInvite.ClientId = model.ClientId;
            vendorInvite.JoinedDate = DateTime.Today.ToString();

            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/UpdateVendorInvite", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        
                        return RedirectToAction("Index", "Workorders");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

       

        public async Task<IActionResult> Register(int id, [Bind("VendorEmail, VendorPassword, VendorName, VendorCompany, VendorPhone, ClientId, RootVendorId")] VendorClientViewModel vendor)
        {
            try
            {
                MippVendorPortalUser aspNetUser = new MippVendorPortalUser()
                {
                    Email = vendor.VendorEmail,
                    UserName = vendor.VendorEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(aspNetUser, vendor.VendorPassword);
                if(!result.Succeeded) 
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            VendorInvite vendorInvite = new VendorInvite();
            vendorInvite.Id = int.Parse(vendor.RootVendorId);
            vendorInvite.VendorId = int.Parse(vendor.RootVendorId);
            vendorInvite.VendorEmail = vendor.VendorEmail;
            vendorInvite.ClientId = int.Parse(vendor.ClientId);
            vendorInvite.JoinedDate = DateTime.Today.ToString();
            //check here whether vendorclient relationship already exists or not

            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
            }

            
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/UpdateVendorInvite", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var vendorSaved = JsonConvert.DeserializeObject<VendorModel>(apiResponse);
                        
                        StringContent content1 = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");
                        using (var response1 = await httpClient.PostAsync(apiUrl + "SendEmail/UpdateVendorInvite", content1))
                        {
                            string apiResponse1 = await response.Content.ReadAsStringAsync();
                            
                           
                        }

                        StringContent content2 = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");

                        using (var response2 = await httpClient.PostAsync(apiUrl + "SendEmail/SendVendorOnboardedEmail", content2))
                        {
                            string apiResponse2 = await response2.Content.ReadAsStringAsync();
                        }

                   

                        Models.Vendor vendor1 = new Models.Vendor();
                        vendor1.Id = vendorSaved.Id;
                        vendor1.VendorCompany = vendorSaved.BusinessName;
                        vendor1.VendorEmail = vendorSaved.Email;
                        //vendor1.VendorPassword = vendor.VendorPassword;
                        vendor1.VendorPhone = vendor.VendorPhone;
                        vendor1.RootVendorId = int.Parse(vendor.RootVendorId);

                        VendorClient vendorClient = new VendorClient();
                        vendorClient.Id = _context.VendorClients.Count() + 1;
                        vendorClient.VendorId = vendorSaved.Id;
                        vendorClient.ClientId = (int)vendorInvite.ClientId;
                        _context.VendorClients.Add(vendorClient);
                        _context.Vendors.Add(vendor1);
                        _context.SaveChanges();

                        

                        CryptographyHelper cryptographyHelper = new CryptographyHelper();
                        var message = cryptographyHelper.EncryptString("Congrats, your registration was successfull!!");

                        if (_context.VendorClients.FirstOrDefault(x => x.VendorId == vendorSaved.Id && x.ClientId == int.Parse(vendor.ClientId)) != null)
                        {
                            return RedirectToAction("Index", "Homes");
                        }
                        //return true;
                        if (vendor == null)
                        {

                            return NotFound();
                        }
                        ViewBag.msg = "Vendor already exists with the client!";
                        return View("InvitationLogin");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
