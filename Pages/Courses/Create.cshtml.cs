using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FakeUniversity.Data;
using FakeUniversity.Models;

namespace FakeUniversity.Pages.Courses
{
    public class CreateModel : DepartmentNamePageModel
    {
        private readonly FakeUniversity.Data.SchoolContext _context;

        public CreateModel(FakeUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            PopulateDepartmentsDropDownList(_context);
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            var newCourse = new Course();
            if (await TryUpdateModelAsync<Course>(
                newCourse,
                "course",
                s => s.CourseID, s=>s.DepartmentID, s=>s.Title, s=> s.Credits)) {
                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // Select DepartmentID if TryUpdateModelAsync fails
            PopulateDepartmentsDropDownList(_context, newCourse.CourseID);
            return Page();
        }
    }
}
