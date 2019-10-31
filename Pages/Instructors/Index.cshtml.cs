using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FakeUniversity.Data;
using FakeUniversity.Models;
using FakeUniversity.Models.SchoolViewModels;

namespace FakeUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly FakeUniversity.Data.SchoolContext _context;

        public IndexModel(FakeUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        public async Task OnGetAsync(int? id, int? CourseID)
        {
            //Instructor = await _context.Instructors.ToListAsync();
            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                /* 
                 * This was added for eager loading, however, if the user barely need to look at enrolment, we could delegate the query until user selects it 
                 */
                //.Include(i => i.CourseAssignments)
                //    .ThenInclude(i => i.Course)
                //    .ThenInclude(i => i.Enrollments)
                //    .ThenInclude(i => i.Student)
                //.AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            /*
             * The filters will return a collection of a single item
             * .Single() converts that to a single Instructor/Course entity
             * - will complain if the collection is empty though
             * .SingleOrDefault() will return null if the collection is empty
             */

            if (id != null)
            {
                InstructorID = id.Value;

                /* 
                 * .Where(cond).Single() or .Single(cond) are equivalent
                 */
                //Instructor instructor = InstructorData.Instructors.Where(i => i.ID == id.Value).Single();
                Instructor instructor = InstructorData.Instructors.Single(i => i.ID == id.Value);
                
                InstructorData.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if (CourseID != null)
            {
                this.CourseID = CourseID.Value;
                var selectedCourse = InstructorData.Courses
                    .Where(s => s.CourseID == CourseID.Value)
                    .Single();

                /*
                 * Added this to only load enrolment details when clicked
                 */
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment en in selectedCourse.Enrollments)
                    await _context.Entry(en).Reference(x => x.Student).LoadAsync();

                InstructorData.Enrollments = selectedCourse.Enrollments;
            }
        }
    }
}
