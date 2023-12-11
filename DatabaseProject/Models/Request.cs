using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Request
    {
        public int request_id { get; set; }
        public string type { get; set; }
        public string comment { get; set;}
        public string status { get; set; }
        public int credit_hours { get; set; }
        public Course course { get; set; }
        public Student student { get; set;}
        public Advisor advisor { get; set; }
    
    }
}