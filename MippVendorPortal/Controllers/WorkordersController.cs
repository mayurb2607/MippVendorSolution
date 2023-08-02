using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using dotless.Core.Parser.Functions;
using dotless.Core.Parser.Infrastructure;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;




using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;
using MippVendorPortal.Helpers;


namespace MippVendorPortal.Controllers
{
    [Authorize]
    public class WorkordersController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public WorkordersController(MippVendorTestContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            
        }

        public async Task<IEnumerable<WorkorderTask>> GetWorkorderTasks(int workorderID)
        {
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
            IEnumerable<WorkorderTask> workDescriptions;

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderID), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorderDescriptions", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    workDescriptions = JsonConvert.DeserializeObject<IEnumerable<WorkorderTask>>(apiResponse);
                    try
                    {
                        return workDescriptions;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }


        [HttpGet("GetWorkOrderStatus")]
        public async Task<ActionResult<string>> GetWorkOrderStatus(int workorderId)
        {
            try
            {
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

                string status = String.Empty;
                using (var httpClient = new HttpClient())
                {
                    

                    using (var response = await httpClient.GetAsync(apiUrl + "Workorders/GetWorkorderStatus?workorderId=" + workorderId))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        status = JsonConvert.DeserializeObject<string>(apiResponse);
                       
                    }
                }

                return Ok(status);
            }
            catch
            {
                return BadRequest();
            }
        }

        //[HttpPost]
        //public async void PostWorkorderDescriptions(IEnumerable<WorkorderWorkDescription> workDescriptions)
        //{
        //    var one = workDescriptions.First();
        //    foreach (var item in workDescriptions)
        //    {
        //        WorkorderWorkDescription workDescription = new WorkorderWorkDescription();
        //        var existing = _context1.WorkorderWorkDescriptions.FirstOrDefault(x => x.Id == item.Id);
        //        if (existing != null)
        //        {
        //            //workDescription.WorkorderId = item.WorkorderId;
        //            existing.DescriptionOfWorkCompletedMaterialsUsed = item.DescriptionOfWorkCompletedMaterialsUsed;
        //            existing.HoursSpent = item.HoursSpent;
        //            _context1.WorkorderWorkDescriptions.Update(existing);
        //            _context1.SaveChanges();
        //        }
        //        else
        //        {
        //            workDescription.WorkorderId = item.WorkorderId;
        //            workDescription.DescriptionOfWorkCompletedMaterialsUsed = item.DescriptionOfWorkCompletedMaterialsUsed;
        //            workDescription.HoursSpent = item.HoursSpent;
        //            _context1.WorkorderWorkDescriptions.Add(workDescription);
        //            _context1.SaveChanges();
        //        }
        //    }
        //    //send email for workorder update here
        //    var env = _hostEnvironment.EnvironmentName;
        //    string apiUrl = string.Empty;
        //    if (env == "Development")
        //    {
        //        // Configure services for development environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //        //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //    }
        //    else
        //    {
        //        // Configure services for local environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //    }
        //    var workorder = _context1.Workorders.FirstOrDefault(x => x.Id == one.Id);
        //    using (var httpClient1 = new HttpClient())
        //    {
        //        StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

        //        using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
        //        {
        //            string apiResponse1 = await response1.Content.ReadAsStringAsync();
        //            //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
        //            //return true;
        //            //return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });
        //        }
        //    }
        //}


        //public IActionResult SubmitComments(int workorderId, string text, string email)
        //{
        //    var commentsObj = new WorkorderComment();
        //    commentsObj.Email = email;
        //    commentsObj.Text = text;
        //    commentsObj.WorkorderId = workorderId;

        //    _context1.WorkorderComments.Add(commentsObj);
        //    _context1.SaveChanges();
        //    return RedirectToAction("Edit", new { id = workorderId });
        //    //send email on adding comments


        //}
        //public IActionResult OverlayPartial(int woid)
        //{
        //    ViewBag.Id = woid;
        //    ViewBag.Email = _context1.Vendors.FirstOrDefault
        //                        (x => x.Id == _context1.Workorders.FirstOrDefault(x => x.Id == woid).VendorId).Email;
        //    return View("_CommentsView");
        //    //return PartialView("_FeedbackPartial");
        //}

        

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var vendor = _context.Vendors.First(x=>x.VendorEmail== username);
            var rootvendorId = vendor.RootVendorId;
            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                VendorID = rootvendorId,
                AdditionalComments = "",
                Status = ""
            };

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



            IEnumerable<ViewModel.WorkorderMasterModel> workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject<IEnumerable<ViewModel.WorkorderMasterModel>>(apiResponse);
                        ViewBag.VendorId = rootvendorId;
                        ViewBag.Workorder = workorder;
                        
                        
                        ViewBag.Email = vendor.VendorEmail;
                        

                        ViewData["GridData"] = workorder;
                        ViewBag.GridData = workorder;
                        ViewBag.Tax = "";
                        ViewBag.Subtotal = "";
                        ViewBag.Total = "";
                        ViewBag.VendorId = vendor.Id;
                         

                        //foreach (var item in workorder)
                        //{
                        //    item.Client = _context1.Clients.FirstOrDefault(x => x.ClientId == item.ClientId).ClientName;
                        //}

                        var commandsObj = new string[4];
                    

                        return View();

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return NotFound();
        }


   #region commented code
        // GET: Workorders/Create
        //public IActionResult Create(int? id)
        //{
        //    ViewBag.VendorId = id;
        //    ViewBag.clientId = 1;

        //    return View();
        //}

        //// POST: Workorders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany," +
        //    "AssignedToAddress,AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate," +
        //    "ServiceRequestNumber,Status,Description,AdditionalComments,ExpectedNoOfHoursToComplete," +
        //    "WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent,PropertyName,PropertyAddress," +
        //    "PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress," +
        //    "TenantPhoneNumber,UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote," +
        //    "EntryDate,TimeEntered,TimeDeparted,EntryNote,WorkorderCompiledBy,WorkorderApprovedBy," +
        //    "DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context1.Workorders.Add(workorder);
        //        await _context1.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(workorder);
        //}


        //public async Task<IActionResult> AssignedWorkorders(int vendorId)
        //{
        //    //API call for newly assigned workorders
        //    var env = _hostEnvironment.EnvironmentName;
        //    string apiUrl = string.Empty;
        //    if (env == "Development")
        //    {
        //        // Configure services for development environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //        //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //    }
        //    else
        //    {
        //        // Configure services for local environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //    }
        //    WorkorderRequest workorderRequest = new WorkorderRequest
        //    {
        //        VendorID = vendorId
        //    };

        //    IEnumerable<Workorder> workorder;
        //    using (var httpClient = new HttpClient())
        //    {
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

        //        using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetAssignedWorkorders", content))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            try
        //            {
        //                workorder = (IEnumerable<Workorder>)JsonConvert.DeserializeObject
        //                    <IEnumerable<Workorder>>(apiResponse);
        //                return View();

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //    }

        //}

        //public async Task<IActionResult> DeclinedWorkorders(int vendorId)
        //{
        //    //API call for newly declined workorders
        //    var env = _hostEnvironment.EnvironmentName;
        //    string apiUrl = string.Empty;
        //    if (env == "Development")
        //    {
        //        // Configure services for development environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //        //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //    }
        //    else
        //    {
        //        // Configure services for local environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //    }
        //    WorkorderRequest workorderRequest = new WorkorderRequest
        //    {
        //        VendorID = vendorId
        //    };

        //    IEnumerable<MippSamplePortal.Models.Workorder> workorder;
        //    using (var httpClient = new HttpClient())
        //    {
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

        //        using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetDeclinedWorkorders", content))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            try
        //            {
        //                workorder = (IEnumerable<MippSamplePortal.Models.Workorder>)JsonConvert.DeserializeObject
        //                    <IEnumerable<Workorder>>(apiResponse);
        //                return View();

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //    }



        //}


        //public async Task<IActionResult> CompletedWorkorders(int vendorId)
        //{
        //    //API call for newly completed workorders
        //    var env = _hostEnvironment.EnvironmentName;
        //    string apiUrl = string.Empty;
        //    if (env == "Development")
        //    {
        //        // Configure services for development environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //        //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
        //    }
        //    else
        //    {
        //        // Configure services for local environment
        //        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
        //    }
        //    WorkorderRequest workorderRequest = new WorkorderRequest
        //    {
        //        VendorID = vendorId
        //    };

        //    IEnumerable<Workorder> workorder;
        //    using (var httpClient = new HttpClient())
        //    {
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

        //        using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetCompletedWorkorders", content))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            try
        //            {
        //                workorder = JsonConvert.DeserializeObject
        //                    <IEnumerable<Workorder>>(apiResponse);
        //                return View();

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //    }


        //}
    #endregion



        //GET: Workorders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null )
            {
                return BadRequest();
            }


            //var workorder = await _context1.Workorders.FindAsync(id);
            //if (workorder == null)
            //{
            //    return NotFound();
            //}

            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                Id = id,

            };

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



            Workorder workorder1;
            IEnumerable <WorkorderComment> workorderComment = null;
            
            IEnumerable<WorkorderTask> workordertasks = null;
            IEnumerable<WorkorderWorkDescription> workorderWorkDescription = new List<WorkorderWorkDescription>();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject
                            <Workorder>(apiResponse);
                        ViewBag.VendorId = workorder1.VendorId;
                        ViewBag.Workorder = workorder1;
                        ViewBag.msg = "";
                        if (_context.Vendors.FirstOrDefault(x => x.Id == workorder1.VendorId) != null)
                        {
                            ViewBag.Email = _context.Vendors.FirstOrDefault(x => x.Id == workorder1.VendorId).VendorEmail;
                        }

                        ViewData["GridData"] = workorder1;
                        ViewBag.GridData = workorder1;
                        ViewBag.Tax = "";
                        ViewBag.Subtotal = "";
                        ViewBag.Total = "";
                        ViewBag.VendorId = _context.Vendors.FirstOrDefault(x => x.RootVendorId == workorder1.VendorId).Id;

                        using (var httpClient1 = new HttpClient())
                        {
                            StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response1 = await httpClient1.PostAsync(apiUrl + "Workorders/GetWorkorderComments", content1))
                            {
                                string apiResponse1 = await response1.Content.ReadAsStringAsync();
                                try
                                {
                                    workorderComment = JsonConvert.DeserializeObject
                                        <IEnumerable<WorkorderComment>>(apiResponse1);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        //using (var httpClient2 = new HttpClient())
                        //{
                        //    StringContent content2 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                        //    using (var response2 = await httpClient2.PostAsync(apiUrl + "Workorders/GetWorkorderStatuses", content2))
                        //    {
                        //        string apiResponse2 = await response2.Content.ReadAsStringAsync();
                        //        try
                        //        {
                        //            workorderStatus = (IEnumerable<ClientStatus>)JsonConvert.DeserializeObject
                        //                <IEnumerable<ClientStatus>>(apiResponse2);
                        //        }
                        //        catch (Exception ex)
                        //        {

                        //        }
                        //    }
                        //}

                        //var status = new List<string>();
                        //foreach (var stat in workorderStatus)
                        //{
                        //    status.Add(stat.Status);
                        //}

                        //ViewBag.Statuses = status;
                        ViewBag.Status = workorder1.Status;
                        ViewData["VendorId"] = workorder1.VendorId;
                        ViewBag.VendorId = workorder1.VendorId;
                        ViewBag.id = workorder1.Id;
                        ViewBag.clientId = workorder1.ClientId;
                        ViewBag.Comments = workorderComment;


                        using (var httpClient3 = new HttpClient())
                        {
                            StringContent content3 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response3 = await httpClient3.PostAsync(apiUrl + "Workorders/GetWorkorderTaskDescription", content3))
                            {
                                string apiResponse3 = await response3.Content.ReadAsStringAsync();
                                try
                                {
                                    workordertasks = JsonConvert.DeserializeObject
                                        <IEnumerable<WorkorderTask>>(apiResponse3);
                                    ViewBag.workordertasks= workordertasks;
                                    ViewBag.taskCount = workordertasks.Count();
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        using (var httpClient4 = new HttpClient())
                        {

                            string url = apiUrl + "Workorders/GetWorkorderVendorWorkDescription/" + workorderRequest.Id;
                            using (var response4 = await httpClient4.GetAsync(apiUrl + "Workorders/GetWorkorderVendorWorkDescription?workorderId=" + workorderRequest.Id))
                            {
                                string apiResponse4 = await response4.Content.ReadAsStringAsync();
                                try
                                {
                                    workorderWorkDescription = (IEnumerable<WorkorderWorkDescription>)JsonConvert.DeserializeObject
                                        <IEnumerable<WorkorderWorkDescription>>(apiResponse4);
                                    ViewBag.workorderWorkDescription=workorderWorkDescription;
                                    
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }

                        ViewBag.WorkorderTasks = workordertasks;
                       
                        var workorderView = new WorkorderViewModel(workorder1);
                        
                        workorderView.WorkDescriptions = workorderWorkDescription.ToList();
                      
                        ViewBag.WorkOrderWorkDescription = workorderWorkDescription;
                        ViewBag.User = User.Identity.Name;
                        return View(workorderView);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
            



        }


        public async Task<IActionResult> Accept(int? id)
        {
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = id;
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

                Workorder workorder;

                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        workorder = JsonConvert.DeserializeObject
                            <Workorder>(apiResponse);
                    }
                } 
                var WorkorederRequest = new WorkorderRequest();
                workorderRequest.Id = workorder.Id;
                if (workorder.Status == "Assigned")
                {
                    workorderRequest.Status = "In Progress";
                }
                else
                {
                    return BadRequest();
                }
                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(WorkorederRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient1.PostAsync(apiUrl + "Workorders/UpdateWorkorderStatus", content1))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            //workorder1 = JsonConvert.DeserializeObject
                            //    <Workorder>(apiResponse);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                    

                ViewBag.saveMsg = "Changes saved successfully!!";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (response1.IsSuccessStatusCode)
                            return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });
                    }
                }


                return Ok();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
            
        


        public async Task<IActionResult> Decline(int? id)
        {
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = id;
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


            try { 
                Workorder workorder = new Workorder();

                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            workorder = JsonConvert.DeserializeObject
                                <Workorder>(apiResponse);
                           
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if (workorder.Status == "Assigned")
                {
                    workorderRequest.Status = "Declined";
                }
                else
                {
                    return BadRequest();
                }
                ViewBag.saveMsg = "Changes saved successfully!!";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (response1.IsSuccessStatusCode)
                            return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });
                    }
                }
                        
                return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });


            }
            catch (Exception ex)
            {
                throw ex;
            }
                
        }
        // POST: Workorders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(IEnumerable<MippPortalWebAPI.Helpers.WorkorderMasterModel> workorder)
        {
            var work = workorder.FirstOrDefault();
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

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = work.Id;


            Workorder workorder1;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject
                            <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            workorder1.Status = work.Status;
            workorder1.Description = work.Description;
            workorder1.AdditionalComments = work.AdditionalComments;
            workorder1.ExpectedNoOfHoursToComplete = work.ExpectedNoOfHoursToComplete;
            workorder1.WorkPerformedBy = work.WorkPerformedBy;
            //wo.WorkCompletedAndMaterialsUsed = workorder.WorkCompletedAndMaterialsUsed;
            //wo.TotalHoursSpent = workorder.TotalHoursSpent;


            using (var httpClient1 = new HttpClient())
            {
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(work.VendorWorkDescriptions), Encoding.UTF8, "application/json");

                using (var response = await httpClient1.PostAsync(apiUrl + "Workorders/PostWorkorderVendorWorkDescription", content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        //workorder1 = JsonConvert.DeserializeObject
                        //    <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            try
            {

                ViewBag.saveMsg = "Changes saved successfully!!";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder1), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (response1.IsSuccessStatusCode)
                            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });
                    }
                }
            
            }
            catch (DbUpdateConcurrencyException)
            {
                        
            }
            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });



        }

        [HttpPost]
        public async Task<IActionResult> EditPost(IEnumerable<WorkorderDto> workorder)
        {
            var work = workorder.FirstOrDefault();
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

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = work.Id;


            Workorder workorder1;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject
                            <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            workorder1.Status = work.Status;
            workorder1.Description = work.Description;
            workorder1.AdditionalComments = work.AdditionalComments;
            workorder1.ExpectedNoOfHoursToComplete = work.ExpectedNoOfHoursToComplete;
            workorder1.WorkPerformedBy = work.WorkPerformedBy;
            //wo.WorkCompletedAndMaterialsUsed = workorder.WorkCompletedAndMaterialsUsed;
            //wo.TotalHoursSpent = workorder.TotalHoursSpent;


            using (var httpClient1 = new HttpClient())
            {
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(work.WorkDescriptions), Encoding.UTF8, "application/json");

                using (var response = await httpClient1.PostAsync(apiUrl + "Workorders/PostWorkorderVendorWorkDescription", content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        //workorder1 = JsonConvert.DeserializeObject
                        //    <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            try
            {

                ViewBag.saveMsg = "Changes saved successfully!!";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder1), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (response1.IsSuccessStatusCode)
                            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });
                    }
                }

            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });



        }


        [HttpPost]
        public async Task<IActionResult> PostWorkorderWorkDescription(IFormCollection request)
        {
            var user = User.Identity.Name;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            string connectionString = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                
            }

            try
            {

                var files = request.Files;
                var apiRequest = new WorkorderWorkDescription();
               
                request.TryGetValue("Id",out var value);
                apiRequest.Id=Int32.Parse(value);
                request.TryGetValue("WorkorderId", out  value);
                apiRequest.WorkorderId = Int32.Parse(value);
                request.TryGetValue("WorkPerformedBy", out  value);
                apiRequest.WorkPerformedBy = value.ToString();
                request.TryGetValue("DescriptionOfWork", out  value);
                apiRequest.DescriptionOfWork = value.ToString();
                request.TryGetValue("WorkMaterials", out  value);
                apiRequest.WorkMaterials = value.ToString();
                request.TryGetValue("HourSpent", out value);
                apiRequest.HourSpent = decimal.Parse(value);
                request.TryGetValue("AdditionalComment", out value);
                apiRequest.AdditionalComment = value.ToString();
                apiRequest.TaskId = 0;
                apiRequest.IsDelete = false;
                apiRequest.CreatedAt= DateTime.Now;
                apiRequest.CreatedBy = user;
                apiRequest.ModifideBy=user;
                apiRequest.ModifiedAt= DateTime.Now;

                int id;
                int workorderid;
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(apiRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/PostWorkorderWorkDescription", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var respone = JsonConvert.DeserializeObject
                                                       <WorkorderWorkDescription>(apiResponse);
                            id = respone.Id;
                            workorderid= respone.WorkorderId;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                foreach (var file in files)
                {
                    if (file != null)
                    {
                        MemoryStream fileStream = new MemoryStream();

                        string containerName = "mipp-bill-accounts"; // Replace with your container name

                        //CloudBlobClient blobClient = account.CreateCloudBlobClient();
                        //CloudBlobContainer container = blobClient.GetContainerReference("images");

                        // Create a BlobServiceClient object
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        // Get a reference to the container
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        // Generate a unique blob name
                        string blobName = "WorkOrder/" + workorderid + "WorkDescription/" + id + "/" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        //CloudBlockBlob blockBlob = containerClient.GetBlockBlobReference("/JamesDon//1//" + blobName);


                        // Create a new blob in the container
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        fileStream.Position = 0;
                        using (fileStream)
                        {
                            file.CopyTo(fileStream);
                            fileStream.Position = 0;
                            blobClient.Upload(fileStream, true);

                        }
                    }
                }

                var WorkorderWorkDescriptionResponse = new List<WorkorderWorkDescription>();

                using (var httpClient = new HttpClient())
                {
                    
                    using (var response = await httpClient.GetAsync(apiUrl + "Workorders/GetWorkorderVendorWorkDescription?workorderId=" + workorderid))

                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            WorkorderWorkDescriptionResponse = JsonConvert.DeserializeObject
                                                        <List<WorkorderWorkDescription>>(apiResponse);
                                  
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                var viewModel = new List<WorkorderWorkDescriptionDTO>();
                foreach (var workDescription in WorkorderWorkDescriptionResponse)
                {
                    string containerName = "mipp-bill-accounts"; // Replace with your container name

                    // Create a BlobServiceClient object
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Get a reference to the container
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    var blobitem = containerClient.GetBlobs(prefix: "WorkOrder/" + workDescription.WorkorderId + "WorkDescription/" + workDescription.Id);
                    List<BlobFile> FileUrls= new List<BlobFile>();
                    foreach (var blob in blobitem)
                    {
                        var blobFile = new BlobFile();
                        blobFile.url="https://mippbills.blob.core.windows.net/mipp-bill-accounts/" + blob.Name;
                        blobFile.blobname=blob.Name;
                        FileUrls.Add(blobFile);
                    }
                    var model = new WorkorderWorkDescriptionDTO(workDescription, FileUrls);
                    viewModel.Add(model);
                    
                }

                ViewBag.WorkorderId = workorderid;
                return PartialView("_WorkDescription", viewModel);


            }
            catch (Exception ex) {
                throw ex;
            }
           
            

           
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttchment( string blobname)
        {
            var user = User.Identity.Name;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            string connectionString = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");

            }

            try
            {
                string containerName = "mipp-bill-accounts"; // Replace with your container name
                
                //CloudBlobClient blobClient = account.CreateCloudBlobClient();
                //CloudBlobContainer container = blobClient.GetContainerReference("images");

                // Create a BlobServiceClient object
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get a reference to the container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Extract the blob name from the URL
                

                // Get the blob client using the blob name
                //var blobClient = containerClient.GetBlobClient(blobName);
                var blobClient = containerClient.GetBlobClient(blobname);

                //BlobContainerClient subfolderClient = containerClient.GetSubContainerClient("1");


                // Delete the blob
                blobClient.DeleteIfExists();

                
            
                return Ok();
            }

            catch(Exception ex) 
            {
                throw ex;
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        public async Task<IActionResult> PutWorkorderWorkDescription(IFormCollection request)
        {
            var user = User.Identity.Name;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            string connectionString = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");

            }

            try
            {

                var files = request.Files;
                var apiRequest = new WorkorderWorkDescription();

                request.TryGetValue("Id", out var value);
                apiRequest.Id = Int32.Parse(value);
                request.TryGetValue("WorkorderId", out value);
                apiRequest.WorkorderId = Int32.Parse(value);
                request.TryGetValue("IsDelete", out value);
                apiRequest.IsDelete = bool.Parse(value);
                request.TryGetValue("WorkPerformedBy", out value);
                apiRequest.WorkPerformedBy = value.ToString();
                request.TryGetValue("DescriptionOfWork", out value);
                apiRequest.DescriptionOfWork = value.ToString();
                request.TryGetValue("WorkMaterials", out value);
                apiRequest.WorkMaterials = value.ToString();
                request.TryGetValue("HourSpent", out value);
                apiRequest.HourSpent = decimal.Parse(value);
                request.TryGetValue("AdditionalComment", out value);
                apiRequest.AdditionalComment = value.ToString();
                apiRequest.TaskId = 0;
                apiRequest.CreatedAt = DateTime.Now;
                apiRequest.CreatedBy = user;
                apiRequest.ModifideBy = user;
                apiRequest.ModifiedAt = DateTime.Now;

                int id;
                int workorderid;
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(apiRequest), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/PutWorkorderWorkDescription", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var respone = JsonConvert.DeserializeObject
                                                       <WorkorderWorkDescription>(apiResponse);
                            id = respone.Id;
                            workorderid = respone.WorkorderId;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                foreach (var file in files)
                {
                    if (file!=null)
                    {
                        MemoryStream fileStream = new MemoryStream();

                        string containerName = "mipp-bill-accounts"; // Replace with your container name

                        //CloudBlobClient blobClient = account.CreateCloudBlobClient();
                        //CloudBlobContainer container = blobClient.GetContainerReference("images");

                        // Create a BlobServiceClient object
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        // Get a reference to the container
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        // Generate a unique blob name
                        string blobName = "WorkOrder/" + workorderid + "WorkDescription/" + id + "/" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        //CloudBlockBlob blockBlob = containerClient.GetBlockBlobReference("/JamesDon//1//" + blobName);


                        // Create a new blob in the container
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        fileStream.Position = 0;
                        using (fileStream)
                        {
                            file.CopyTo(fileStream);
                            fileStream.Position = 0;
                            blobClient.Upload(fileStream, true);

                        }
                    }
                }

                var WorkorderWorkDescriptionResponse = new List<WorkorderWorkDescription>();

                using (var httpClient = new HttpClient())
                {

                    using (var response = await httpClient.GetAsync(apiUrl + "Workorders/GetWorkorderVendorWorkDescription?workorderId=" + workorderid))

                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            WorkorderWorkDescriptionResponse = JsonConvert.DeserializeObject
                                                        <List<WorkorderWorkDescription>>(apiResponse);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                var viewModel = new List<WorkorderWorkDescriptionDTO>();
                foreach (var workDescription in WorkorderWorkDescriptionResponse)
                {
                    string containerName = "mipp-bill-accounts"; // Replace with your container name

                    // Create a BlobServiceClient object
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Get a reference to the container
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    var blobitem = containerClient.GetBlobs(prefix: "WorkOrder/" + workDescription.WorkorderId + "WorkDescription/" + workDescription.Id);
                    List<BlobFile> FileUrls = new List<BlobFile>();
                    foreach (var blob in blobitem)
                    {
                        var blobFile = new BlobFile();
                        blobFile.url = "https://mippbills.blob.core.windows.net/mipp-bill-accounts/" + blob.Name;
                        blobFile.blobname = blob.Name;
                        FileUrls.Add(blobFile);
                    }
                    var model = new WorkorderWorkDescriptionDTO(workDescription, FileUrls);
                    viewModel.Add(model);
                    

                }

                ViewBag.WorkorderId = workorderid;
                return PartialView("_WorkDescription", viewModel);


            }
            catch (Exception ex)
            {
                throw ex;
            }




            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        public async Task<ActionResult> LoadWorkorderWorkDescription(int  workorderid)
        {
            var user = User.Identity.Name;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            string connectionString = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");

            }
            try
            {

                var WorkorderWorkDescriptionResponse = new List<WorkorderWorkDescription>();

                using (var httpClient = new HttpClient())
                {

                    using (var response = await httpClient.GetAsync(apiUrl + "Workorders/GetWorkorderVendorWorkDescription?workorderId=" + workorderid))

                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            WorkorderWorkDescriptionResponse = JsonConvert.DeserializeObject
                                                        <List<WorkorderWorkDescription>>(apiResponse);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                var viewModel = new List<WorkorderWorkDescriptionDTO>();
                foreach (var workDescription in WorkorderWorkDescriptionResponse)
                {
                    string containerName = "mipp-bill-accounts"; // Replace with your container name

                    // Create a BlobServiceClient object
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Get a reference to the container
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    var blobitem = containerClient.GetBlobs(prefix: "WorkOrder/" + workDescription.WorkorderId + "WorkDescription/" + workDescription.Id);
                    List<BlobFile> FileUrls = new List<BlobFile>();
                    foreach (var blob in blobitem)
                    {
                        var blobFile = new BlobFile();
                        blobFile.url = containerClient.Uri +"/"+ blob.Name;
                        blobFile.blobname = blob.Name;
                        FileUrls.Add(blobFile);
                    }
                    var model = new WorkorderWorkDescriptionDTO(workDescription, FileUrls);
                    viewModel.Add(model);
                   
                }
                ViewBag.WorkorderId = workorderid;
                return PartialView("_WorkDescription", viewModel);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateWorkorderWorkDescription(WorkorderWorkDescription request)
        {
            var user = User.Identity.Name;
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            string connectionString = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                connectionString = _configuration.GetValue<string>("AzureStorage");

            }
            try {

                int id;
                int workorderid;
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "Workorders/UpdateWorkorderWorkDescription", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var respone = JsonConvert.DeserializeObject
                                                       <WorkorderWorkDescription>(apiResponse);
                            id = respone.Id;
                            workorderid = respone.WorkorderId;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }


                Ok(id);
            }

            catch (Exception ex) { 
                throw ex;
            }
             
            return StatusCode(StatusCodes.Status500InternalServerError);
            

        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(IEnumerable<WorkorderMasterModel> workorder)
        {
            var work = workorder.FirstOrDefault();
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

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = work.Id;


            string Msg = "Status Updated";

            Workorder workorder1;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject
                            <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            try
            {
                if (workorder1.Status == "In Progress")
                {
                    workorder1.Status = "Submit for Approval";
                    work.Status = "Submit for Approval";
                }
                else if(workorder1.Status== "Submit for Approval")
                {
                    workorder1.Status = "In Progress";
                    work.Status = "In Progress";
                }
                workorder1.Description = work.Description;
                workorder1.AdditionalComments = work.AdditionalComments;
                workorder1.ExpectedNoOfHoursToComplete = work.ExpectedNoOfHoursToComplete;
                workorder1.WorkPerformedBy = work.WorkPerformedBy;
                
                //wo.WorkCompletedAndMaterialsUsed = workorder.WorkCompletedAndMaterialsUsed;
                //wo.TotalHoursSpent = workorder.TotalHoursSpent;
            }
            catch
            {
                throw;
            }

            using (var httpClient1 = new HttpClient())
            {
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(work), Encoding.UTF8, "application/json");

                using (var response = await httpClient1.PostAsync(apiUrl + "Workorders/UpdateWorkorderStatus", content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        //workorder1 = JsonConvert.DeserializeObject
                        //    <Workorder>(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            try
            {

                ViewBag.saveMsg = "Changes saved successfully!!";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder1), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        //return true;
                        if (response1.IsSuccessStatusCode)
                            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });
                    }
                }

            }
            catch (DbUpdateConcurrencyException)
            {

            }
            ViewBag.Msg = Msg;
            return RedirectToAction("Index", new { rootVendorId = workorder1.VendorId, msg = "" });



        }


        // GET: Workorders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context1.Workorders == null)
        //    {
        //        return NotFound();
        //    }

        //    var workorder = await _context1.Workorders
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (workorder == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(workorder);
        //}

        // POST: Workorders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context1.Workorders == null)
        //    {
        //        return Problem("Entity set 'MippdbContext.Workorders'  is null.");
        //    }
        //    var workorder = await _context1.Workorders.FindAsync(id);
        //    if (workorder != null)
        //    {
        //        _context1.Workorders.Remove(workorder);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool WorkorderExists(int id)
        //{
        //    return (_context1.Workorders?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
