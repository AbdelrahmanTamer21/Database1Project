using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Semester
    {

        public string semester_code { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }


        public Semester() { }

        public Semester(string semester_code, DateTime start_date, DateTime end_date)
        {
            this.semester_code = semester_code;
            this.start_date = start_date;
            this.end_date = end_date;
        }
    }
}