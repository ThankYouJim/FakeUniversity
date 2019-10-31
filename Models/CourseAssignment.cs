using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeUniversity.Models
{
    public class CourseAssignment
    {
        // The two FK is a composite PK
        public int InstructorID { get; set; }   // FK
        public int CourseID { get; set; }   // FK
        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
    }
}
