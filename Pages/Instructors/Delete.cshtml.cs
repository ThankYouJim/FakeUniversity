using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FakeUniversity.Data;
using FakeUniversity.Models;

namespace FakeUniversity.Pages.Instructors
{
    public class DeleteModel : PageModel
    {
        private readonly FakeUniversity.Data.SchoolContext _context;

        public DeleteModel(FakeUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.ID == id);

            if (Instructor == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Instructor instructorToDelete = await _context.Instructors
                .Include(i => i.CourseAssignments)  // need this otherwise the course assignments will not be deleted after instructor is
                .SingleAsync(m => m.ID == id);
            if (instructorToDelete == null)
                return RedirectToPage("./Index");

            var departments = await _context.Departments
                .Where(d => d.InstructorID == id)
                .ToListAsync();
            departments.ForEach(d => d.InstructorID = null);

            _context.Instructors.Remove(instructorToDelete);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
