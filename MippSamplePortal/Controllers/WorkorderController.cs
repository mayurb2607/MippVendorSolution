using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MippSamplePortal.Models;

namespace MippSamplePortal.Controllers
{
    public class WorkorderController : Controller
    {
        private readonly MippTestContext _context;

        public WorkorderController(MippTestContext context)
        {
            _context = context;
        }

        // GET: Workorder
        public async Task<IActionResult> Index()
        {
              return _context.Workorders != null ? 
                          View(await _context.Workorders.ToListAsync()) :
                          Problem("Entity set 'MippTestContext.Workorders'  is null.");
        }

        // GET: Workorder/Details/5
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

            return View(workorder);
        }

        // GET: Workorder/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workorder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany,AssignedToAddress,AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate,ServiceRequestNumber,Status,Description,AdditionalComments,ExpectedNoOfHoursToComplete,WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent,PropertyName,PropertyAddress,PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress,TenantPhoneNumber,UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote,EntryDate,TimeEntered,TimeDeparted,EntryNote,WorkorderCompiledBy,WorkorderApprovedBy,DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workorder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workorder);
        }

        // GET: Workorder/Edit/5
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
            return View(workorder);
        }

        // POST: Workorder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderNumber,OrderDate,AssignedTo,AssignedToCompany,AssignedToAddress,AssignedToPhone,AssignedToEmailAddress,ExpectedStartDate,ExpectedEndDate,ServiceRequestNumber,Status,Description,AdditionalComments,ExpectedNoOfHoursToComplete,WorkPerformedBy,WorkCompletedAndMaterialsUsed,TotalHoursSpent,PropertyName,PropertyAddress,PropertyManager,PropertyManagerPhone,PropertyManagerEmail,TenantName,TenantEmailAddress,TenantPhoneNumber,UnitName,UnitAddress,Note,PreferredTime,EnterCondition,PermissionNote,EntryDate,TimeEntered,TimeDeparted,EntryNote,WorkorderCompiledBy,WorkorderApprovedBy,DateOfApproval,Priority,CostOfLabor,CostOfMaterials,TaxesPaid,Total,ClientId,VendorId")] Workorder workorder)
        {
            if (id != workorder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workorder);
                    await _context.SaveChangesAsync();
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

        // GET: Workorder/Delete/5
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

        // POST: Workorder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Workorders == null)
            {
                return Problem("Entity set 'MippTestContext.Workorders'  is null.");
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
