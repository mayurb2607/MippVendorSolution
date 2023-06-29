using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;

namespace MippPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly MippTestContext _context;

        public VendorsController(MippTestContext context)
        {
            _context = context;
        }

        [HttpPost("GetVendorData")]
        public VendorList GetVendorData(VendorRequest vendorRequest)
        {
            return _context.VendorLists.FirstOrDefault(x => x.VendorEmail == vendorRequest.email);
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorList>>> GetVendorLists()
        {
          if (_context.VendorLists == null)
          {
              return NotFound();
          }
            return await _context.VendorLists.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VendorList>> GetVendorList(int id)
        {
          if (_context.VendorLists == null)
          {
              return NotFound();
          }
            var vendorList = await _context.VendorLists.FindAsync(id);

            if (vendorList == null)
            {
                return NotFound();
            }

            return vendorList;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendorList(int id, VendorList vendorList)
        {
            if (id != vendorList.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendorList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VendorList>> PostVendorList(VendorList vendorList)
        {
          if (_context.VendorLists == null)
          {
              return Problem("Entity set 'MippTestContext.VendorLists'  is null.");
          }
            _context.VendorLists.Add(vendorList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendorList", new { id = vendorList.Id }, vendorList);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorList(int id)
        {
            if (_context.VendorLists == null)
            {
                return NotFound();
            }
            var vendorList = await _context.VendorLists.FindAsync(id);
            if (vendorList == null)
            {
                return NotFound();
            }

            _context.VendorLists.Remove(vendorList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorListExists(int id)
        {
            return (_context.VendorLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
