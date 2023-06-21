﻿using Microsoft.AspNetCore.Mvc;
using MippPortalWebAPI.Helpers;
using MippSamplePortal.Models;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace MippVendorPortal.Controllers
{
    public class JoinController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public JoinController(MippVendorTestContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _context = context;

        }
        // GET: Join

        public IActionResult Index(string e, int cId, int rvId)
        {
            CryptographyHelper cryptographyHelper = new CryptographyHelper();
            var mail = cryptographyHelper.DecryptString(e);

            var mailpresent = _context.Vendors.FirstOrDefault(x => x.VendorEmail == mail);
            if (mailpresent == null)
            {
                if (e != null && cId != 0)
                {
                    ViewBag.Email = mail;
                    ViewBag.ClientId = cId;
                    ViewBag.RootVendorId = rvId;
                }

                return View();
            }
            ViewBag.msg = "Your account already exists!";
            return PartialView("_AcceptPartial");

        }

        // GET: Join/Details/5
        public async Task<IActionResult> Accept(int id, [Bind("VendorEmail, VendorPassword, VendorCompany, VendorPhone, ClientId")] VendorClientViewModel viewModel)
        {


            var existingVendors = _context.Vendors.Count();
            if (id == 0 || _context.Vendors == null)
            {
                return NotFound();
            }
            Models.Vendor vendor1 = new Models.Vendor
            {
                VendorEmail = viewModel.VendorEmail,
                VendorPhone = viewModel.VendorPhone,
                //vendor1.VendorPassword = viewModel.VendorPassword;
                VendorCompany = viewModel.VendorCompany,
                Id = existingVendors + 1
            };
            _context.Vendors.Add(vendor1);
            await _context.SaveChangesAsync();

            MippVendorPortal.Models.VendorClient vendorClient = new MippVendorPortal.Models.VendorClient();
            vendorClient.VendorId = vendor1.Id;
            vendorClient.ClientId = int.Parse(viewModel.ClientId);
            if (_context.VendorClients.FirstOrDefault(x => x.VendorId == vendor1.Id && x.ClientId == vendorClient.ClientId) != null)
            {
                _context.Vendors.Add(vendor1);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Hey! You are already registered..");
            }


            ViewBag.Id = vendor1.Id;
            ViewBag.VendorEmail = vendor1.VendorEmail;
            ViewBag.ClientId = viewModel.ClientId;


            VendorInvite vendorInvite = new VendorInvite();
            vendorInvite.Id = vendor1.Id;
            vendorInvite.VendorId = vendor1.Id;
            vendorInvite.VendorEmail = vendor1.VendorEmail;
            vendorInvite.ClientId = int.Parse(viewModel.ClientId);
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/UpdateVendorInvite", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (vendor1 == null)
                        {
                            return NotFound();
                        }
                        //ViewBag.SuccessMsg = "successfully added";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IActionResult> Register(int id, [Bind("VendorEmail, VendorPassword, VendorCompany, VendorPhone, ClientId, RootVendorId")] VendorClientViewModel vendor)
        {

            VendorInvite vendorInvite = new VendorInvite();
            vendorInvite.Id = 1;
            vendorInvite.VendorId = 1;
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vendorInvite), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/UpdateVendorInvite", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var vendorSaved = JsonConvert.DeserializeObject<MippSamplePortal.Models.Vendor>(apiResponse);

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
                            return RedirectToAction("Index", "Account", new { msg = message });
                        }
                        //return true;
                        if (vendor == null)
                        {

                            return NotFound();
                        }
                        ViewBag.msg = "Vendor already exists with the client!";
                        return View("_AcceptPartial");
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
