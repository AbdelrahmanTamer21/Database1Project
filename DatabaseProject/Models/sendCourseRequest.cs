using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class sendCourseRequest
    {
        public int courseID { get; set; }
        public int studentID { get; set; }
        public string comment { get; set; }
        public string type { get; set; }
    }
}