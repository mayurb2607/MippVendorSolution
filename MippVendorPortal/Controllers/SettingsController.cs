using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MippVendorPortal.Models;

namespace MippVendorPortal.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly MippVendorTestContext _context;

        public SettingsController(MippVendorTestContext context)
        {
            _context = context;
        }

        // GET: Settings
        public IActionResult Index(int vendorID)
        {
            ViewBag.VendorId = vendorID;
            var data = _context.Settings.Where(x => x.VendorId == vendorID).AsEnumerable();
            ViewBag.GridData = data;
            ViewData["GridData"] = data;
            
            return View(data);
        }

        // GET: Settings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: Settings/Create
        public IActionResult Create(int vendorId)
        {
            ViewBag.VendorId = vendorId;
            ViewBag.Id = _context.Settings.Count() + 1;
            return View();
        }

        // POST: Settings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VendorId,BusinessName,CareOf,Phone,Email,AddressLine1,AddressLine2,City,Province,Zip,BillDate,DueDate")] Setting setting)
        {
            //setting.Id = _context.Settings.Count() + 1;
            if (ModelState.IsValid)
            {
                _context.Settings.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new {vendorID = setting.VendorId});
            }
            return View(setting);
        }

        // GET: Settings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            ViewBag.Id = setting.Id;
            ViewBag.VendorId = setting.VendorId;
            return View(setting);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VendorId,BusinessName,CareOf,Phone,Email,AddressLine1,AddressLine2,City,Province,Zip,BillDate,DueDate")] Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new {vendorID= setting.VendorId});
            }
            return View(setting);
        }

        // GET: Settings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: Settings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Settings == null)
            {
                return Problem("Entity set 'MippVendorTestContext.Settings'  is null.");
            }
            var setting = await _context.Settings.FindAsync(id);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettingExists(int id)
        {
          return (_context.Settings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
