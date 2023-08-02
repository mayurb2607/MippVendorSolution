using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using Org.BouncyCastle.Crypto.Modes;

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
        public async Task<List<string>> GetVendors(Helpers.WorkorderRequest workorderRequest)
        {
            //workorderRequest.ClientID = 1;
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

        [HttpGet("GetWorkorderStatus")]
        public async Task<ActionResult<String>> GetWorkorderStatus(int workorderId)
        {
            try
            {
                var workorder = _context.Workorders.FirstOrDefault(x => x.Id == workorderId);
                if(workorder != null) {
                    return Ok(workorder.Status);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("GetWorkorderDescriptions")]
        public List<Models.WorkorderTask> GetWorkorderDescriptions (string workorderId)
        {
            var descriptions = _context.WorkorderTasks.Where(x => x.WorkorderId == int.Parse(workorderId)).ToList();
            return descriptions;
        }
        
        

        [HttpPost("GetWorkorders")]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetWorkorders(Helpers.WorkorderRequest workorderRequest)
        {
            if (workorderRequest.ClientID != 0 && workorderRequest.ClientID != null)
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
                
                var workorder = new Workorder();
                if(workorderRequest.Id != 0)
                {
                    workorder = workorders.FirstOrDefault(x => x.Id == workorderRequest.Id);
                }
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


        [HttpPost("GetAssignedWorkorders")]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetAssignedWorkorders(Helpers.WorkorderRequest workorderRequest)
        {
            if (workorderRequest.VendorID != 0)
            {
                var workorders = await _context.Workorders.Where(x => x.VendorId == workorderRequest.VendorID).ToListAsync();

                var workorder = new Workorder();
                if (workorderRequest.Id != 0)
                {
                    workorder = workorders.FirstOrDefault(x => x.Id == workorderRequest.Id);
                }
                var productList = new List<Workorder>();

                foreach (var item in workorders)
                {
                    if(item.Status == "Assigned")
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


        [HttpPost("GetCompletedWorkorders")]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetCompletedWorkorders(Helpers.WorkorderRequest workorderRequest)
        {
            if (workorderRequest.VendorID != 0)
            {
                var workorders = await _context.Workorders.Where(x => x.VendorId == workorderRequest.VendorID).ToListAsync();

                var workorder = new Workorder();
                if (workorderRequest.Id != 0)
                {
                    workorder = workorders.FirstOrDefault(x => x.Id == workorderRequest.Id);
                }
                var productList = new List<Workorder>();

                foreach (var item in workorders)
                {
                    if (item.Status == "Completed")
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

        [HttpPost("GetDeclinedWorkorders")]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetDeclinedWorkorders(Helpers.WorkorderRequest workorderRequest)
        {
            if (workorderRequest.VendorID != 0)
            {
                var workorders = await _context.Workorders.Where(x => x.VendorId == workorderRequest.VendorID).ToListAsync();

                var workorder = new Workorder();
                if (workorderRequest.Id != 0)
                {
                    workorder = workorders.FirstOrDefault(x => x.Id == workorderRequest.Id);
                }
                var productList = new List<Workorder>();

                foreach (var item in workorders)
                {
                    if (item.Status == "Declined")
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



        [HttpPost("GetWorkorderComments")]
        public async Task<ActionResult<IEnumerable<WorkorderComment>>> GetWorkorderComments (Helpers.WorkorderRequest workorderRequest)
        {
            var comments =  _context.WorkorderComments.Where(x => x.WorkorderId == workorderRequest.Id).ToList();
            
            return comments;
        }


        [HttpPost("GetWorkorderStatuses")]
        public async Task<ActionResult<IEnumerable<ClientStatus>>> GetWorkorderStatuses(Helpers.WorkorderRequest workorderRequest)
        {
            workorderRequest.ClientID = 1;
            var statuses = _context.ClientStatuses.Where(x => x.ClientId == workorderRequest.ClientID).ToList();

            return statuses;
        }

        [HttpPost("GetWorkorderTaskDescription")]
        public async Task<ActionResult<IEnumerable<WorkorderTask>>> GetWorkorderTaskDescription(Helpers.WorkorderRequest workorderRequest)
        {
            workorderRequest.ClientID = 1;
            var workorderTasks = _context.WorkorderTasks.Where(x => x.WorkorderId == workorderRequest.Id).ToList();



            return workorderTasks;
        }


        // GET: api/Workorders/5
        [HttpPost("GetWorkorder")]
        public async Task<ActionResult<Workorder>> GetWorkorder(Helpers.WorkorderRequest workorderRequest)
        {
            if (_context.Workorders == null)
            {
                return NotFound();
            }
            var workorder = await _context.Workorders.FindAsync(workorderRequest.Id);

            if (workorder == null)
            {
                return NotFound();
            }

            return workorder;
        }


        [HttpPost("UpdateWorkorderStatus")]
        public bool UpdateWorkorderStatus(WorkorderRequest workorderRequest)
        {
            if (workorderRequest.Id != 0)
            {
                var workorder = _context.Workorders.FirstOrDefault(x => x.Id == workorderRequest.Id);
                workorder.Status = workorderRequest.Status;
                _context.Workorders.Update(workorder);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // PUT: api/Workorders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PutWorkorder")]
        public async Task<IActionResult> PutWorkorder(WorkorderMasterModel workorderMaster)
        {
            //if (id != workorder.Id)
            //{
            //    return BadRequest();
            //}
            Workorder workorder = _context.Workorders.Find(workorderMaster.Id);
            string fname = workorderMaster.AssignedTo.Split(" ")[0];

            _context.Entry(workorder).State = EntityState.Modified;
            workorder.Status = workorderMaster.Status;
            workorder.AdditionalComments = workorderMaster.AdditionalComments;
            workorder.AssignedTo = workorderMaster.AssignedTo;
            workorder.AssignedToEmailAddress = _context.Vendors.FirstOrDefault(x => x.FirstName == workorderMaster.AssignedTo).Email;
            //workorder.AssignedToPhone = _context.VendorInvites.FirstOrDefault(x => x. == workorderRequest.AssignedTo).P
            workorder.VendorId = workorderMaster.VendorId;
            //workorder.VendorId = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Id;
            workorder.AssignedToEmailAddress = workorderMaster.AssignedToEmailAddress;
            //workorder.AssignedToEmailAddress = _context.Vendors.FirstOrDefault(x => x.FirstName == fname).Email;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkorderExists(workorderMaster.Id))
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
        public async Task<ActionResult> PostWorkorder([Bind] Workorder workorder)
        {
            if (_context.Workorders == null)
            {
                return Problem("Entity set 'MippdbContext.Workorders'  is null.");
            }
            var newworkorder= _context.Workorders.Add(workorder);
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
            Helpers.WorkorderRequest workorderRequest = new Helpers.WorkorderRequest();
            workorderRequest.ClientID = (int)workorder.ClientId;
            workorderRequest.Id= workorder.Id;
            return CreatedAtAction("GetWorkorders", new { workorderRequest = workorderRequest });
        }
        [HttpGet("GetWorkorderVendorWorkDescription")]
        public async Task<ActionResult<List<WorkorderWorkDescription>>>GetWorkorderVendorWorkDecsription(int workorderId)
        { 
            var respone=new List<WorkorderWorkDescription>();
            try
            {
                if(workorderId == 0) 
                {
                    throw new Exception("WorkOrderId does not exist");

                }
                var workorder=_context.Workorders.FirstOrDefault(x=>x.Id==workorderId);
                if(workorder == null)
                {
                    throw new Exception("WorkOrderId does not exist");

                }
                respone = _context.WorkorderWorkDescriptions.Where(x=>x.WorkorderId==workorderId).ToList();

                return respone;
            }
            catch(Exception ex)
            {
                throw ex;
            }
       

        }

        [HttpPost("PostWorkorderWorkDescription")]
        public async Task<ActionResult<Workorder>> PostWorkorderWorkDecsription([Bind] WorkorderWorkDescription workorderWorkDescription)
        {
           
            
            var workorder = _context.Workorders.FirstOrDefault(x => x.Id == workorderWorkDescription.WorkorderId);
            if (workorder == null)
            {
                throw new Exception("WorkOrderId does not exist");

            }
            if (workorder.Status != "In Progress")
            {
                throw new Exception("VenderDetails cannot beupdated");
            }
            try
            {
                
                        _context.WorkorderWorkDescriptions.Add(workorderWorkDescription);
                    
            }
            catch
            {
                throw;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                
                
                    throw;
     
            }
            
            return Ok(workorderWorkDescription);
        }

        [HttpPost("PutWorkorderWorkDescription")]
        public async Task<ActionResult<Workorder>> PutWorkorderWorkDecsription([Bind] WorkorderWorkDescription workorderWorkDescription)
        {


            var workorder = _context.Workorders.FirstOrDefault(x => x.Id == workorderWorkDescription.WorkorderId);
            if (workorder == null)
            {
                throw new Exception("WorkOrderId does not exist");

            }
            if (workorder.Status != "In Progress")
            {
                throw new Exception("VenderDetails cannot beupdated");
            }
            try
            {

                _context.WorkorderWorkDescriptions.Update(workorderWorkDescription);

            }
            catch
            {
                throw;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {


                throw;

            }

            return Ok(workorderWorkDescription);
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
