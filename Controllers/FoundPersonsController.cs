using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissingPersonIdentificationSystem.Data;
using MissingPersonIdentificationSystem.Models;

namespace MissingPersonIdentificationSystem.Controllers
{
    public class FoundPersonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoundPersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FoundPersons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FoundPersons.Include(f => f.Finder);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FoundPersons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FoundPersons == null)
            {
                return NotFound();
            }

            var foundPerson = await _context.FoundPersons
                .Include(f => f.Finder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foundPerson == null)
            {
                return NotFound();
            }

            return View(foundPerson);
        }

       
        // GET: FoundPersons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FoundPersons == null)
            {
                return NotFound();
            }

            var foundPerson = await _context.FoundPersons
                .Include(f => f.Finder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foundPerson == null)
            {
                return NotFound();
            }

            return View(foundPerson);
        }

        // POST: FoundPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FoundPersons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FoundPersons'  is null.");
            }
            var foundPerson = await _context.FoundPersons.FindAsync(id);
            if (foundPerson != null)
            {
                _context.FoundPersons.Remove(foundPerson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoundPersonExists(int id)
        {
          return (_context.FoundPersons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
