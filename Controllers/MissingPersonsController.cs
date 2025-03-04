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
    public class MissingPersonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MissingPersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MissingPersons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MissingPersons.Include(m => m.FamilyMember);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MissingPersons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MissingPersons == null)
            {
                return NotFound();
            }

            var missingPerson = await _context.MissingPersons
                .Include(m => m.FamilyMember)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (missingPerson == null)
            {
                return NotFound();
            }

            return View(missingPerson);
        }
               

        // GET: MissingPersons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MissingPersons == null)
            {
                return NotFound();
            }

            var missingPerson = await _context.MissingPersons
                .Include(m => m.FamilyMember)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (missingPerson == null)
            {
                return NotFound();
            }

            return View(missingPerson);
        }

        // POST: MissingPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MissingPersons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MissingPersons'  is null.");
            }
            var missingPerson = await _context.MissingPersons.FindAsync(id);
            if (missingPerson != null)
            {
                _context.MissingPersons.Remove(missingPerson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MissingPersonExists(int id)
        {
          return (_context.MissingPersons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
