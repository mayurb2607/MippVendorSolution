using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Models;
using MippSamplePortal.ViewModel;

namespace MippPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkordersController : ControllerBase
    {
        private readonly MippTestContext _context;

        public WorkordersController(MippTestContext context)
        {
            _context = context;
        }

        [HttpPost("GetVendors")]
        public async Task<List<string>> GetVendors(WorkorderRequest workorderRequest)
        {
            workorderRequest.ClientID = 1;
            if (workorderRequest.ClientID != 0)
            {
                var clients = _context.VendorInvites.Where(x => x.ClientId == workorderRequest.ClientID);
                List<int> vIds = new List<int>();

                foreach (var item in clients)
                {
                    if (item.VendorId != null && item.VendorId != int.MinValue)
                        vIds.Add((int)item.VendorId);
                }
                List<string> vendors = new List<string>();

                foreach (var item in vIds)
                {
                    if (_context.Vendors.FirstOrDefault(x => x.Id == item) != null)
                    {
                        var name = _context.Vendors.FirstOrDefault(x => x.Id == item).FirstName + " " + _context.Vendors.FirstOrDefault(x => x.Id == item).LastName;
                        vendors.Add(name);
                    }
                }
                List<string> selectListItems = new List<string>(vendors);

                return selectListItems;

            }

            else
            {
                return null;
            }
        }

        [HttpPost("GetWorkorders")]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetWorkorders(WorkorderRequest workorderRequest)
        {
            if (workorderRequest.ClientID != 0)
            {
                var workorders = await _context.Workorders.Where(x => x.ClientId == workorderRequest.ClientID).ToListAsync();
                var productList = new List<Workorder>();

                foreach (var item in workorders)
                {
                    productList.Add(item);
                }

                foreach (var item in productList)
                {
                    if (item.ClientId != Convert.ToInt32(workorderRequest.ClientID))
                    {
                        productList.Remove(item);
                    }

                }
                return productList;

            }
            else if (workorderRequest.VendorID != 0)
            {
                var workorders = await _context.Workorders.Where(x => x.VendorId == workorderRequest.VendorID).ToListAsync();
                var productList = new List<Workorder>();

                foreach (var item in workorders)
                {
                    productList.Add(item);
                }

                foreach (var item in productList)
                {
                    if (item.VendorId != Convert.ToInt32(workorderRequest.VendorID))
                    {
                        productList.Remove(item);
                    }

                }
                return productList;

            }
            else
            {
                var workorders = await _context.Workorders.FindAsync(workorderRequest.Id);
                var productList = new List<Workorder> { workorders };
                //foreach (var item in productList)
                //{
                //if (item.ClientId != Convert.ToInt32(workorderRequest.ClientID))
                //{
                //    productList.Remove(item);
                //}

                //}
                return productList;

            }

        }

        // GET: api/Workorders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workorder>> GetWorkorder(int id)
        {
            if (_context.Workorders == null)
            {
                return NotFound();
            }
            var workorder = await _context.Workorders.FindAsync(id);

            if (workorder == null)
            {
                return NotFound();
            }

            return workorder;
        }

        // PUT: api/Workorders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PutWorkorder")]
        public async Task<IActionResult> PutWorkorder(WorkorderMasterModel workorderRequest)
        {
            //if (id != workorder.Id)
            //{
            //    return BadRequest();
            //}
            Workorder workorder = _context.Workorders.Find(workorderRequest.Id);
            string fname = workorderRequest.AssignedTo.Split(" ")[0];

            _context.Entry(workorder).State = EntityState.Modified;
            workorder.Status = workorderRequest.Status;
            workorder.AdditionalComments = workorderRequest.AdditionalComments;
            workorder.AssignedTo = workorderRequest.AssignedTo;
            workorder.VendorId = 1;
            //workorder.VendorId = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Id;
            workorder.AssignedToEmailAddress = workorderRequest.AssignedToEmailAddress;
            //workorder.AssignedToEmailAddress = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Email;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkorderExists(workorderRequest.Id))
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

        // POST: api/Workorders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostWorkorder")]
        public async Task<ActionResult<Workorder>> PostWorkorder([Bind] Workorder workorder)
        {
            if (_context.Workorders == null)
            {
                return Problem("Entity set 'MippdbContext.Workorders'  is null.");
            }
            _context.Workorders.Add(workorder);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WorkorderExists(workorder.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.ClientID = (int)workorder.ClientId;
            return CreatedAtAction("GetWorkorders", new { workorderRequest = workorderRequest });
        }

        // DELETE: api/Workorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkorder(int id)
        {
            if (_context.Workorders == null)
            {
                return NotFound();
            }
            var workorder = await _context.Workorders.FindAsync(id);
            if (workorder == null)
            {
                return NotFound();
            }

            _context.Workorders.Remove(workorder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkorderExists(int id)
        {
            return (_context.Workorders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
