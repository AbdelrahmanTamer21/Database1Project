using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class requestCourse
    {
        public int student_id { get; set; }
        public string type { get; set; }
        public string comment { get; set; }
        public int course_id { get; set; }
    }
}