using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MippSamplePortal.Models;
using MippSamplePortal.ViewModel;
using Newtonsoft.Json;

namespace MippSamplePortal.Controllers
{
    [Authorize]
    public class WorkorderController : Controller
    {
        private readonly MippTestContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public WorkorderController(MippTestContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(string email)
        {

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.ClientID = _context.Clients.FirstOrDefault(x => x.Email == email).Id;
            workorderRequest.AdditionalComments = "";
            workorderRequest.Status = "";

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

            IEnumerable<WorkorderMasterModel> workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject<IEnumerable<WorkorderMasterModel>>(apiResponse);
                        //ViewBag.VendorId = vendorId;
                        ViewBag.Workorder = workorder;
                        //ViewBag.msg = msg;

                        ViewData["GridData"] = workorder;
                        ViewBag.GridData = workorder;
                        ViewBag.Tax = "";
                        ViewBag.Subtotal = "";
                        ViewBag.Total = "";
                        ViewBag.ClientID = workorderRequest.ClientID;
                        TempData["Email"] = email;
                        ViewBag.Email = email;
                        foreach (var item in workorder)
                        {
                            if(_context.VendorLists.FirstOrDefault(x => x.Id == item.VendorId) != null)
                            {
                                item.Vendor = _context.VendorLists.FirstOrDefault(x => x.Id == item.VendorId).VendorName;

                            }
                            else
                            {
                                item.Vendor = "NA";
                            }
                        }
                        return View(workorder.ToList());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return NotFound();
        }

        public ActionResult Create(string email, string vendorEmail)
        {
            ViewBag.ClientID = _context.Clients.FirstOrDefault(x => x.Email == email).ClientId;
            int cId = ViewBag.ClientID;
            var emailAddresses = new List<string>();
            var statuses = new List<string>();
            if (vendorEmail == null)
            {
                var vendors = _context.VendorLists.Where(x => x.ClientId == cId);
                foreach (var item in vendors)
                {
                    emailAddresses.Add(item.VendorEmail);
                }
                ViewBag.Vendors = vendors;
            }
            var vendor = _context.VendorLists.Where(x => x.VendorEmail == vendorEmail);
            var status = _context.ClientStatuses.Where(x => x.ClientId == cId);

            foreach (var item in status)
            {
                statuses.Add(item.Status);
            }
            
            ViewBag.EmailAddresses = emailAddresses;
            ViewBag.Status = statuses;

            ViewBag.Vendor = vendor.ToList();
            //ViewBag.Vendors = new List<string>
            //{
                
            //}
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(int clientId, [Bind] WorkorderMasterModel workorder)
        {
            workorder.ClientId = clientId;
            workorder.Status = "Assigned";
            workorder.EnterCondition = "Call for availiablity";
            workorder.VendorId = _context.VendorLists.FirstOrDefault(x=> x.VendorEmail == workorder.AssignedToEmailAddress).Id;
            var email = _context.Clients.FirstOrDefault(x => x.Id == clientId).Email;
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.ClientID = workorder.ClientId;
            workorderRequest.AdditionalComments = workorder.Status;
            workorderRequest.Status = "";

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


            IEnumerable<WorkorderMasterModel> workorders;
            int count = 0;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorders = JsonConvert.DeserializeObject<IEnumerable<WorkorderMasterModel>>(apiResponse);
                        count = 0;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }


            try
            {
                
                //workorder.Id = count + 1;


                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/PostWorkorder", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();


                        using (var httpClient1 = new HttpClient())
                        {
                            StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

                            using (var response1 = await httpClient.PostAsync(apiUrl + "SendEmail/SendWorkorderUpdateEmail", content))
                            {
                                string apiResponse1 = await response.Content.ReadAsStringAsync();
                                //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                                //return true;
                                return RedirectToAction("Index", new { email = email });
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public async Task<IActionResult> Edit(int? id, string email)
        {
            if (id == null)
            {
                return View(null);
            }
            IEnumerable<ClientStatus> workorderStatus = null;
            IEnumerable<WorkorderWorkDescription> workorderWorkDescription = null;
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

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = (int)id;
            //workorderRequest.ClientID = 0;
            workorderRequest.Status = String.Empty;
            workorderRequest.ClientID = (int)_context.Workorders.FirstOrDefault(x => x.Id == id).ClientId;
            workorderRequest.AdditionalComments = String.Empty;
            Workorder workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject<Workorder>(apiResponse);
                        

                        using (var httpClient1 = new HttpClient())
                        {
                            StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response1 = await httpClient.PostAsync(apiUrl+ "Workorders/GetVendors", content))
                            {
                                string apiResponse1 = await response1.Content.ReadAsStringAsync();
                                try
                                {
                                    List<string> data = JsonConvert.DeserializeObject<List<string>>(apiResponse1);

                                    List<string> vendorList = new List<string>();


                                    ViewBag.Vendors = data;
                                    ViewBag.Id = id;
                                    ViewBag.clientId = workorderRequest.ClientID;
                                    
                                    ViewData["Vendors"] = data;
                                    TempData["Email"] = email;
                                }
                                catch (Exception ex)
                                {

                                }

                            }

                        }

                        using (var httpClient2 = new HttpClient())
                        {
                            StringContent content2 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response2 = await httpClient2.PostAsync(apiUrl + "Workorders/GetWorkorderStatuses", content2))
                            {
                                string apiResponse2 = await response2.Content.ReadAsStringAsync();
                                try
                                {
                                    workorderStatus = JsonConvert.DeserializeObject
                                        <IEnumerable<ClientStatus>>(apiResponse2);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        var status = new List<string>();
                        foreach (var stat in workorderStatus)
                        {
                            status.Add(stat.Status);
                        }

                        ViewBag.Statuses = status;
                        ViewBag.Status = workorder.Status;

                        using (var httpClient3 = new HttpClient())
                        {
                            StringContent content3 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response3 = await httpClient3.PostAsync(apiUrl + "Workorders/GetWorkorderWorkDescription", content3))
                            {
                                string apiResponse3 = await response3.Content.ReadAsStringAsync();
                                try
                                {
                                    workorderWorkDescription = JsonConvert.DeserializeObject
                                        <IEnumerable<WorkorderWorkDescription>>(apiResponse3);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        ViewBag.Descriptions = workorderWorkDescription;
                        return View(workorder);

                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
            return View(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string email, [Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany,AssignedToAddress," +
            "AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate,ServiceRequestNumber,Status,Description," +
            "AdditionalComments,ExpectedNoOfHoursToComplete,WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent," +
            "PropertyName,PropertyAddress,PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress," +
            "TenantPhoneNumber,UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote,EntryDate,TimeEntered," +
            "TimeDeparted,EntryNote,WorkorderCompiledBy,WorkorderApprovedBy,DateOfApproval,Priority,CostOfLabor,CostOfMaterials," +
            "TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        {
            //workorder.AssignedTo = "Jacob Paul";
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.ClientID = (int)workorder.ClientId;
            workorderRequest.AdditionalComments = workorder.AdditionalComments;
            workorderRequest.Status = workorder.Status;
            workorderRequest.Id = workorder.Id;


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

            WorkorderMasterModel workorderMaster = new WorkorderMasterModel();
            IEnumerable<WorkorderMasterModel> workorder1;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject<IEnumerable<WorkorderMasterModel>>(apiResponse);
                        workorderMaster = workorder1.FirstOrDefault(x => x.Id == id);
                        workorderMaster.AssignedTo = workorder.AssignedTo;
                        workorderMaster.AdditionalComments = workorder.AdditionalComments;
                        workorderMaster.VendorId = _context.VendorLists.FirstOrDefault(x => x.VendorEmail == workorder.AssignedToEmailAddress).Id;
                        workorderMaster.Status = workorder.Status;
                        //workorderMaster.Id = _context.Workorders.Count() + 1;
                        //workorderMaster.AssignedToPhone = _context.V

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            string fname = workorderMaster.AssignedTo.Split(" ")[0];

            _context.Entry(workorder).State = EntityState.Modified;
            workorder.Status = workorderMaster.Status;
            workorder.AdditionalComments = workorderMaster.AdditionalComments;
            workorder.AssignedTo = workorderMaster.AssignedTo;
            //workorder.AssignedToPhone = _context.VendorInvites.FirstOrDefault(x => x. == workorderRequest.AssignedTo).P
            workorder.VendorId = workorderMaster.VendorId;
            //workorder.VendorId = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Id;
            workorder.AssignedToEmailAddress = workorderMaster.AssignedToEmailAddress;
            //workorder.AssignedToEmailAddress = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Email;

            await _context.SaveChangesAsync();


            using (var httpClient1 = new HttpClient())
            {
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorderMaster), Encoding.UTF8, "application/json");

                using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderUpdateEmail", content1))
                {
                    string apiResponse = await response1.Content.ReadAsStringAsync();
                    //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                    //return true;
                    var clientEmail = _context.Clients.FirstOrDefault(x => x.Id == workorderMaster.ClientId).Email;
                    return RedirectToAction("Index", new { email = clientEmail });
                }
            }
        }


        //[HttpPost]
        //public void UpdateWorkorderDetails([FromBody] string comments, string status)
        //{

        //}
    }
}
