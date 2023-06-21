using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;

namespace MippVendorPortal.Controllers
{
    public class WorkordersController : Controller
    {
        private readonly MippTestContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public WorkordersController(MippTestContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(int rootvendorId, string msg)
        {
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.VendorID = rootvendorId;
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
                        ViewBag.VendorId = rootvendorId;
                        ViewBag.Workorder = workorder;
                        ViewBag.msg = msg;
                        if (_context.Vendors.FirstOrDefault(x => x.Id == rootvendorId) != null)
                        {
                            ViewBag.Email = _context.Vendors.FirstOrDefault(x => x.Id == rootvendorId).Email;
                        }

                        ViewData["GridData"] = workorder;
                        ViewBag.GridData = workorder;
                        ViewBag.Tax = "";
                        ViewBag.Subtotal = "";
                        ViewBag.Total = "";

                        return View();

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return NotFound();
        }


        // GET: Workorders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Workorders == null)
            {
                return NotFound();
            }

            var workorder = await _context.Workorders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workorder == null)
            {
                return NotFound();
            }
            ViewBag.VendorId = workorder.VendorId;
            return View(workorder);
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
            "DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        {
            if (ModelState.IsValid)
            {
                _context.Workorders.Add(workorder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workorder);
        }

        //GET: Workorders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Workorders == null)
            {
                return NotFound();
            }


            var workorder = await _context.Workorders.FindAsync(id);
            if (workorder == null)
            {
                return NotFound();
            }

            var statuses = _context.ClientStatuses.Where(x => x.ClientId == workorder.ClientId);
            var status = new List<string>();
            foreach (var stat in statuses)
            {
                status.Add(stat.Status);
            }
            ViewBag.Statuses = status;
            ViewBag.Status = workorder.Status;
            ViewData["VendorId"] = workorder.VendorId;
            ViewBag.VendorId = workorder.VendorId;
            ViewBag.id = workorder.Id;
            ViewBag.clientId = workorder.ClientId;
            return View(workorder);
        }

        // POST: Workorders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany,AssignedToAddress," +
            "AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate,ServiceRequestNumber,Status,Description," +
            "AdditionalComments,ExpectedNoOfHoursToComplete,WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent,PropertyName," +
            "PropertyAddress,PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress,TenantPhoneNumber," +
            "UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote,EntryDate,TimeEntered,TimeDeparted,EntryNote," +
            "WorkorderCompiledBy,WorkorderApprovedBy,DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        {
            if (id != workorder.Id)
            {
                return NotFound();
            }
            var wo = _context.Workorders.FirstOrDefault(x => x.Id == id);
            wo.Status = workorder.Status;
            wo.Description = workorder.Description;
            wo.AdditionalComments = workorder.AdditionalComments;
            wo.ExpectedNoOfHoursToComplete = workorder.ExpectedNoOfHoursToComplete;
            wo.WorkPerformedBy = workorder.WorkPerformedBy;
            wo.WorkCompletedAndMaterialsUsed = workorder.WorkCompletedAndMaterialsUsed;
            wo.TotalHoursSpent = workorder.TotalHoursSpent;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Workorders.Update(wo);
                    await _context.SaveChangesAsync();
                    ViewBag.saveMsg = "Changes saved successfully!!";

                    var env = _hostEnvironment.EnvironmentName;
                    string apiUrl = string.Empty;
                    if (env == "Development")
                    {
                        // Configure services for development environment
                        //apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                        apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
                    }
                    else
                    {
                        // Configure services for local environment
                        apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                    }

                    using (var httpClient = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(wo), Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/SendWorkorderStatusUpdateEmail", content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                            //return true;
                            if (response.IsSuccessStatusCode)
                                return RedirectToAction("Index", new { vendorId = wo.VendorId, msg = ViewBag.savemsg });
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkorderExists(workorder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workorder);
        }

        // GET: Workorders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Workorders == null)
            {
                return NotFound();
            }

            var workorder = await _context.Workorders
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
            if (_context.Workorders == null)
            {
                return Problem("Entity set 'MippdbContext.Workorders'  is null.");
            }
            var workorder = await _context.Workorders.FindAsync(id);
            if (workorder != null)
            {
                _context.Workorders.Remove(workorder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkorderExists(int id)
        {
            return (_context.Workorders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
