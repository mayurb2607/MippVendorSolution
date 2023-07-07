using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotless.Core.Parser.Functions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using MippSamplePortal.ViewModel;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;
using AspNetUser = MippVendorPortal.Models.AspNetUser;
using WorkorderRequest = MippPortalWebAPI.Helpers.WorkorderRequest;

namespace MippVendorPortal.Controllers
{
    [Authorize]
    public class WorkordersController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly MippPortalWebAPI.Models.MippTestContext _context1;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public WorkordersController(MippVendorTestContext context, MippPortalWebAPI.Models.MippTestContext context1, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _context1 = context1;
        }

        public async Task<IEnumerable<MippPortalWebAPI.Models.WorkorderWorkDescription>> GetWorkorderDescriptions(int workorderID)
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }
            IEnumerable<MippPortalWebAPI.Models.WorkorderWorkDescription> workDescriptions;

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderID), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorderDescriptions", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    workDescriptions = JsonConvert.DeserializeObject<IEnumerable<MippPortalWebAPI.Models.WorkorderWorkDescription>>(apiResponse);
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

        [HttpPost]
        public async void PostWorkorderDescriptions(IEnumerable<WorkorderWorkDescription> workDescriptions)
        {
            var one = workDescriptions.First();
            foreach (var item in workDescriptions)
            {
                WorkorderWorkDescription workDescription = new WorkorderWorkDescription();
                var existing = _context1.WorkorderWorkDescriptions.FirstOrDefault(x => x.Id== item.Id);
                if (existing != null)
                {
                    //workDescription.WorkorderId = item.WorkorderId;
                    existing.DescriptionOfWorkCompletedMaterialsUsed = item.DescriptionOfWorkCompletedMaterialsUsed;
                    existing.HoursSpent = item.HoursSpent;
                    _context1.WorkorderWorkDescriptions.Update(existing);
                    _context1.SaveChanges();
                }
                else
                {
                    workDescription.WorkorderId = item.WorkorderId;
                    workDescription.DescriptionOfWorkCompletedMaterialsUsed = item.DescriptionOfWorkCompletedMaterialsUsed;
                    workDescription.HoursSpent = item.HoursSpent;
                    _context1.WorkorderWorkDescriptions.Add(workDescription);
                    _context1.SaveChanges();
                }
            }
            //send email for workorder update here
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
            var workorder = _context1.Workorders.FirstOrDefault(x => x.Id == one.Id);
            using (var httpClient1 = new HttpClient())
            {
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorder), Encoding.UTF8, "application/json");

                using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content1))
                {
                    string apiResponse1 = await response1.Content.ReadAsStringAsync();
                    //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                    //return true;
                        //return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });
                }
            }
        }


        public IActionResult SubmitComments(int workorderId, string text, string email)
        {
            var commentsObj = new WorkorderComment();
            commentsObj.Email = email;
            commentsObj.Text = text;
            commentsObj.WorkorderId = workorderId;

            _context1.WorkorderComments.Add(commentsObj);
            _context1.SaveChanges();
            return RedirectToAction("Edit", new { id = workorderId });
            //send email on adding comments


        }
        public IActionResult OverlayPartial(int woid)
        {
            ViewBag.Id = woid;
            ViewBag.Email = _context1.Vendors.FirstOrDefault
                                (x => x.Id == _context1.Workorders.FirstOrDefault(x=> x.Id == woid).VendorId).Email;
            return View("_CommentsView");
            //return PartialView("_FeedbackPartial");
        }

        

        [Authorize]
        public async Task<IActionResult> Index(int rootvendorId, string msg)
        {
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
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
                        ViewBag.msg = msg;
                        if (_context.Vendors.FirstOrDefault(x => x.Id == rootvendorId) != null)
                        {
                            ViewBag.Email = _context.Vendors.FirstOrDefault(x => x.Id == rootvendorId).VendorEmail;
                        }

                        ViewData["GridData"] = workorder;
                        ViewBag.GridData = workorder;
                        ViewBag.Tax = "";
                        ViewBag.Subtotal = "";
                        ViewBag.Total = "";
                        ViewBag.VendorId = _context.Vendors.FirstOrDefault(x => x.Id == rootvendorId).Id;

                        foreach (var item in workorder)
                        {
                            item.Client = _context1.Clients.FirstOrDefault(x => x.ClientId == item.ClientId).ClientName;
                        }

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


        
        // GET: Workorders/Create
        public IActionResult Create(int? id)
        {
            ViewBag.VendorId = id;
            ViewBag.clientId = 1;

            return View();
        }

        //// POST: Workorders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany," +
            "AssignedToAddress,AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate," +
            "ServiceRequestNumber,Status,Description,AdditionalComments,ExpectedNoOfHoursToComplete," +
            "WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent,PropertyName,PropertyAddress," +
            "PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress," +
            "TenantPhoneNumber,UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote," +
            "EntryDate,TimeEntered,TimeDeparted,EntryNote,WorkorderCompiledBy,WorkorderApprovedBy," +
            "DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] MippPortalWebAPI.Models.Workorder workorder)
        {
            if (ModelState.IsValid)
            {
                _context1.Workorders.Add(workorder);
                await _context1.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workorder);
        }


        public async Task<IActionResult> AssignedWorkorders (int vendorId)
        {
            //API call for newly assigned workorders
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
            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                VendorID = vendorId
            };

            IEnumerable<Workorder> workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetAssignedWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject
                            <IEnumerable<MippPortalWebAPI.Models.Workorder>>(apiResponse);
                        return View();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

        }

        public async Task<IActionResult> DeclinedWorkorders(int vendorId)
        {
            //API call for newly declined workorders
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
            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                VendorID = vendorId
            };

            IEnumerable<Workorder> workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetDeclinedWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject
                            <IEnumerable<Workorder>>(apiResponse);
                        return View();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }



        }


        public async Task<IActionResult> CompletedWorkorders(int vendorId)
        {
            //API call for newly completed workorders
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
            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                VendorID = vendorId
            };

            IEnumerable<Workorder> workorder;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetCompletedWorkorders", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject
                            <IEnumerable<Workorder>>(apiResponse);
                        return View();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


        }



        //GET: Workorders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context1.Workorders == null)
            {
                return NotFound();
            }


            var workorder = await _context1.Workorders.FindAsync(id);
            if (workorder == null)
            {
                return NotFound();
            }

            WorkorderRequest workorderRequest = new WorkorderRequest
            {
                Id = workorder.Id,

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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }



            Workorder workorder1;
            IEnumerable <WorkorderComment> workorderComment = null;
            IEnumerable<ClientStatus> workorderStatus = null;
            IEnumerable<WorkorderWorkDescription> workorderWorkDescription = null;
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
                        ViewBag.Workorder = workorder;
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
                        ViewData["VendorId"] = workorder.VendorId;
                        ViewBag.VendorId = workorder.VendorId;
                        ViewBag.id = workorder.Id;
                        ViewBag.clientId = workorder.ClientId;
                        ViewBag.Comments = workorderComment;


                        using (var httpClient3 = new HttpClient())
                        {
                            StringContent content3 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                            using (var response3= await httpClient3.PostAsync(apiUrl + "Workorders/GetWorkorderWorkDescription", content3))
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

                        return View(workorder1);
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }



            MippPortalWebAPI.Models.Workorder workorder;

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject
                            <MippPortalWebAPI.Models.Workorder>(apiResponse);
                        workorder.Status = "In Progress";

                        _context1.Workorders.Update(workorder);
                        _context1.SaveChanges();

                        try
                        {

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
                        }
                        catch (DbUpdateConcurrencyException)
                        {

                        }
                        return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }



            MippPortalWebAPI.Models.Workorder workorder;

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder = JsonConvert.DeserializeObject
                            <MippPortalWebAPI.Models.Workorder>(apiResponse);
                        workorder.Status = "Declined";

                        _context1.Workorders.Update(workorder);
                        _context1.SaveChanges();

                        try
                        {

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
                        }
                        catch (DbUpdateConcurrencyException)
                        {

                        }
                        return RedirectToAction("Index", new { rootVendorId = workorder.VendorId, msg = "" });


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
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
                apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
            }

            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.Id = work.Id;


            MippPortalWebAPI.Models.Workorder workorder1;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Workorders/GetWorkorder", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        workorder1 = JsonConvert.DeserializeObject
                            <MippPortalWebAPI.Models.Workorder>(apiResponse);
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
                      _context1.Workorders.Update(workorder1);
                      _context1.SaveChanges();

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

        // GET: Workorders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context1.Workorders == null)
            {
                return NotFound();
            }

            var workorder = await _context1.Workorders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workorder == null)
            {
                return NotFound();
            }

            return View(workorder);
        }

        // POST: Workorders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context1.Workorders == null)
            {
                return Problem("Entity set 'MippdbContext.Workorders'  is null.");
            }
            var workorder = await _context1.Workorders.FindAsync(id);
            if (workorder != null)
            {
                _context1.Workorders.Remove(workorder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkorderExists(int id)
        {
            return (_context1.Workorders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
