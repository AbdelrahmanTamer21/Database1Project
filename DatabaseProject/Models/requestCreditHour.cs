using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class requestCreditHour
    {
        public int student_id { get; set; }
        public int credit_hours { get; set; }
        public string type { get; set; }
        public string comment { get; set; }

    }
}