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
    public class FindersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FindersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Finders
        public async Task<IActionResult> Index()
        {
              return _context.Finders != null ? 
                          View(await _context.Finders.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Finders'  is null.");
        }

        // GET: Finders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Finders == null)
            {
                return NotFound();
            }

            var finder = await _context.Finders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finder == null)
            {
                return NotFound();
            }

            return View(finder);
        }
               

        // GET: Finders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Finders == null)
            {
                return NotFound();
            }

            var finder = await _context.Finders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finder == null)
            {
                return NotFound();
            }

            return View(finder);
        }

        // POST: Finders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Finders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Finders'  is null.");
            }
            var finder = await _context.Finders.FindAsync(id);
            if (finder != null)
            {
                _context.Finders.Remove(finder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinderExists(int id)
        {
          return (_context.Finders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
