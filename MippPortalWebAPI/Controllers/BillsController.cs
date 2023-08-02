using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using BillItem = MippPortalWebAPI.Models.BillItem;

namespace MippPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly MippTestContext _context;

        public BillsController(MippTestContext context)
        {
            _context = context;
        }

        // GET: api/Bills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
          if (_context.Bills == null)
          {
              return NotFound();
          }
            return await _context.Bills.ToListAsync();
        }


        [HttpPost("GetBillsForVendor")]
        public async Task<ActionResult<List<Bill>>> GetBillsForVendor(BillRequest request)
        {
            try
            {
                var bills = _context.Bills.Where(x => x.ClientId == request.ClientID).ToList();
                if (bills == null)
                {
                    return NotFound();
                }
                return Ok(bills);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: api/Bills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
          if (_context.Bills == null)
          {
              return NotFound();
          }
            var bill = await _context.Bills.FindAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }

        // PUT: api/Bills/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, Bill bill)
        {
            if (id != bill.Id)
            {
                return BadRequest();
            }

            _context.Entry(bill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
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

        [HttpPost("GetTaxList")]
        public async Task<ActionResult<IEnumerable<Tax>>> GetTaxList (WorkorderRequest workorderRequest)
        {
            var taxes = _context.Taxes.Where(x => x.ClientId == workorderRequest.ClientID).ToList();

            return taxes;
        }


        [HttpGet("GetProductAndServices")]
        public ActionResult<List<Models.ProductsAndService>> GetProductusAndServices(int clientID)
        {

            try
            {
                var productsAndServices = _context.ProductsAndServices.Where(x => x.ClientId == clientID && x.IsDelete != true);
                if (productsAndServices.Any())
                {
                    return Ok(productsAndServices);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("GetProductAndServiceDetails")]
        public ActionResult<Models.ProductsAndService> GetProductusAndServiceDetails(int serviceId)
        {

            try
            {
                var productsAndServices = _context.ProductsAndServices.FirstOrDefaultAsync(x => x.Id == serviceId && x.IsDelete != true);
                if (productsAndServices.Result!= null)
                {
                    return Ok(productsAndServices.Result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // POST: api/Bills
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
          if (_context.Bills == null)
          {
              return Problem("Entity set 'MippTestContext.Bills'  is null.");
          }

            var workOrder = _context.Workorders.First(x => x.Id == bill.WorderId);
            if (workOrder == null)
            {
                return Problem("Workorder not found");
            }

            _context.Bills.Add(bill);

            workOrder.Status = "Bill Created";
           _context.Workorders.Update(workOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
        }


        [HttpPost("UpdateBill")]
        public async Task<ActionResult<Bill>> UpdateBill(Bill bill)
        {
            if (_context.Bills == null)
            {
                return Problem("Entity set 'MippTestContext.Bills'  is null.");
            }

            var workOrder = _context.Workorders.First(x => x.Id == bill.WorderId);
            if (workOrder == null)
            {
                return Problem("Workorder not found");
            }
            if(workOrder.Status != "Approved" )
            {
                return Problem("Workorder For this bill is not Approved");
            }

            _context.Bills.Update(bill);

           
            _context.Workorders.Update(workOrder);
            await _context.SaveChangesAsync();

            return Ok(bill);
        }

        // DELETE: api/Bills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            if (_context.Bills == null)
            {
                return NotFound();
            }
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("GetBillDetails")]
        public async Task<ActionResult<Bill>> GetBillDetails (BillRequest billRequest)
        {
            try
            {
                var bill = await _context.Bills.FirstOrDefaultAsync(x => x.Id == billRequest.BillId);
                if (bill != null)
                {
                    return bill;
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("GetBillItems")]
        public  ActionResult<List<BillItem>> GetBillItems(BillRequest billRequest)
        {
            try
            {
                var billitems = _context.BillItems.Where(x => x.BillId == billRequest.BillId).ToList();
                if (billitems.Count > 0)
                {
                    return billitems;
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddBillItems")]
        public async Task<ActionResult> AddBillItems (List<BillItem> billItems)
        {
            try
            {
                foreach (var item in billItems)
                {
                    var bill = _context.Bills.First(x => x.Id == item.BillId);
                    if (bill == null)
                    {
                        return Problem("Bill not found");
                    }
                }
                if (billItems != null || billItems.Count == 0)
                {
                    _context.BillItems.AddRange(billItems);
                    _context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("UpdateBillItems")]
        public async Task<ActionResult> UpdateBillItems(List<BillItem> billItems)
        {
            try
            {
                
                var bill = _context.Bills.First(x => x.Id == billItems[0].BillId);
                if (bill == null)
                {
                    return Problem("Bill not found");
                }

                if (_context.BillItems.FirstOrDefault(x => x.BillId == billItems[0].BillId) != null)
                {
                    _context.BillItems.Where(x => x.BillId == billItems[0].BillId).ExecuteDelete();
                }
                if (billItems != null || billItems.Count == 0)
                {
                    _context.BillItems.AddRange(billItems);
                    _context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("AddBill")]
        public async Task<ActionResult<Bill>> AddBill(Bill bill)
        {
            try
            {
                if (_context.Bills == null)
                {
                    return Problem("Entity set 'MippTestContext.Bills'  is null.");
                }



                var workOrder = _context.Workorders.First(x => x.Id == bill.WorderId);
                if (workOrder == null)
                {
                    return Problem("Workorder not found");
                }

                if(workOrder.Status!="Approved")
                {
                    return Problem("Can not create bill for this work order");
                }

                var prebill = _context.Bills.FirstOrDefault(x => x.WorderId == bill.WorderId);
                if (prebill != null)
                {
                    return Problem("Bill already exists");
                }
                _context.Bills.Add(bill);



               
                
                await _context.SaveChangesAsync();



                return Ok(bill);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("GetClientEmail")]
        public string GetClientEmail(WorkorderRequest workorderRequest)
        {
            if(workorderRequest.ClientID != 0)
            {
                return _context.Clients.FirstOrDefault(x => x.ClientId == workorderRequest.ClientID).Email;
            }
            return null;
        }

        private bool BillExists(int id)
        {
            return (_context.Bills?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
