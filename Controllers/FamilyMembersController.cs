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
    public class FamilyMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FamilyMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FamilyMembers
        public async Task<IActionResult> Index()
        {
              return _context.FamilyMembers != null ? 
                          View(await _context.FamilyMembers.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.FamilyMembers'  is null.");
        }

        // GET: FamilyMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FamilyMembers == null)
            {
                return NotFound();
            }

            var familyMember = await _context.FamilyMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (familyMember == null)
            {
                return NotFound();
            }

            return View(familyMember);
        }
               

        // GET: FamilyMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FamilyMembers == null)
            {
                return NotFound();
            }

            var familyMember = await _context.FamilyMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (familyMember == null)
            {
                return NotFound();
            }

            return View(familyMember);
        }

        // POST: FamilyMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FamilyMembers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FamilyMembers'  is null.");
            }
            var familyMember = await _context.FamilyMembers.FindAsync(id);
            if (familyMember != null)
            {
                _context.FamilyMembers.Remove(familyMember);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FamilyMemberExists(int id)
        {
          return (_context.FamilyMembers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
