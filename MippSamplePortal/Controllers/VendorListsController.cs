using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MippSamplePortal.Models;
using MippSamplePortal.Services;
using MippSamplePortal.ViewModel;

namespace MippSamplePortal.Controllers
{
    public class VendorListsController : Controller
    {
        private readonly MippTestContext _context;
        private readonly EmailService _emailService;

        public VendorListsController(MippTestContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: VendorLists
        public async Task<IActionResult> Index(string email)
        {
            int clientID = (int)_context.Clients.FirstOrDefault(x => x.Email == email).ClientId;
            ViewData["GridData"] = _context.VendorLists.Where(x => x.ClientId == clientID).ToList();
            ViewBag.GridData = _context.VendorLists.Where(x => x.ClientId == clientID).ToList();
            ViewBag.Email = email;
            return View(_context.VendorLists.Where(x => x.ClientId == clientID).ToList());
        }

        // GET: VendorLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VendorLists == null)
            {
                return NotFound();
            }

            var vendorList = await _context.VendorLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendorList == null)
            {
                return NotFound();
            }

            return View(vendorList);
        }

        public async Task<IActionResult> Assign(int? id)
        {
            var mail = _context.Clients.FirstOrDefault(x => x.ClientId == _context.VendorLists.FirstOrDefault(x => x.Id == id).ClientId).Email;
            var vemail = _context.VendorLists.FirstOrDefault(x => x.Id == id).VendorEmail;
            return RedirectToAction("Create", "Workorder", new {email = mail, vendorEmail=vemail});
        }



        public async void Invite(int? id)
        {
            var vendorDetails = _context.VendorLists.FirstOrDefault(x => x.Id == id);
            InviteViewModel invite = new InviteViewModel()
            {
                ClientId = (int)vendorDetails.ClientId,
                Email = vendorDetails.VendorEmail,
                RootVendorId = vendorDetails.Id
            };

            string fromEmail = _context.Clients.FirstOrDefault(x => x.ClientId == invite.ClientId).Email;

            VendorInvite vendorInvite = _context.VendorInvites.FirstOrDefault(x => x.VendorEmail == invite.Email);
            if (vendorInvite == null)
            {
                await _emailService.SendInviteForNewVendor(_context, new SendEmailViewModel { ClientID = invite.ClientId, VendorID = invite.RootVendorId, ToEmail = invite.Email, Body = "", Cc = null, FromEmail = fromEmail, Subject = "" });
                ViewBag.msg = "An invite link has been shared!";
                //Call SendEmail API endpoint for valid invite
            }
            //validate invite using Service class method
            await _emailService.SendAcceptanceEmail(_context, new SendEmailViewModel { ClientID = invite.ClientId, VendorID = invite.RootVendorId, ToEmail = invite.Email, Body = "", Cc = null, FromEmail = fromEmail, Subject = "" });
            ViewBag.msg = "An invite link has been shared!";

        }

        // GET: VendorLists/Create
        public IActionResult Create(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        // POST: VendorLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VendorName,VendorEmail,ClientId,Location,BusinessName")] VendorList vendorList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendorList);
                await _context.SaveChangesAsync();
                string email = _context.Clients.FirstOrDefault(x => x.ClientId == vendorList.ClientId).Email;
                return RedirectToAction("Index", new {email = email});
            }
            return View(vendorList);
        }

        // GET: VendorLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VendorLists == null)
            {
                return NotFound();
            }

            var vendorList = await _context.VendorLists.FindAsync(id);
            if (vendorList == null)
            {
                return NotFound();
            }
            return View(vendorList);
        }

        // POST: VendorLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VendorName,VendorEmail,ClientId,Location,BusinessName")] VendorList vendorList)
        {
            if (id != vendorList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendorList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorListExists(vendorList.Id))
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
            return View(vendorList);
        }

        // GET: VendorLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VendorLists == null)
            {
                return NotFound();
            }

            var vendorList = await _context.VendorLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendorList == null)
            {
                return NotFound();
            }

            return View(vendorList);
        }

        // POST: VendorLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VendorLists == null)
            {
                return Problem("Entity set 'MippTestContext.VendorLists'  is null.");
            }
            var vendorList = await _context.VendorLists.FindAsync(id);
            if (vendorList != null)
            {
                _context.VendorLists.Remove(vendorList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendorListExists(int id)
        {
          return (_context.VendorLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
