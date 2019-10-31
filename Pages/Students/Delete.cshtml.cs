using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FakeUniversity.Data;
using FakeUniversity.Models;

namespace FakeUniversity.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly FakeUniversity.Data.SchoolContext _context;

        public DeleteModel(FakeUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; }
        public string ErrorMessage { get; set; }

        /*
         * Added the variable saveChangesError to check if the method was called after 
         * a failure when deleting a student object
         */
        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError=false)
        {
            if (id == null)
                return NotFound();

            Student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Student == null)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
                ErrorMessage = "Delete failed. Try again.";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound();

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException /* ex */)
            {
                return RedirectToAction("./Delete",
                    new { id, saveChangesError = true });
            }
            //if (Student != null)
            //{
            //    _context.Students.Remove(Student);
            //    await _context.SaveChangesAsync();
            //}
            //
            //return RedirectToPage("./Index");
        }
    }
}
