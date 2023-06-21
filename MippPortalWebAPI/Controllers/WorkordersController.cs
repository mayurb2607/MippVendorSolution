using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Models;

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

        // GET: api/Workorders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workorder>>> GetWorkorders()
        {
          if (_context.Workorders == null)
          {
              return NotFound();
          }
            return await _context.Workorders.ToListAsync();
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkorder(int id, Workorder workorder)
        {
            if (id != workorder.Id)
            {
                return BadRequest();
            }

            _context.Entry(workorder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkorderExists(id))
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
        [HttpPost]
        public async Task<ActionResult<Workorder>> PostWorkorder(Workorder workorder)
        {
          if (_context.Workorders == null)
          {
              return Problem("Entity set 'MippTestContext.Workorders'  is null.");
          }
            _context.Workorders.Add(workorder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkorder", new { id = workorder.Id }, workorder);
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
