using System.Collections.Generic;

namespace DatabaseProject.Models
{
    public class GraduationPlanSemester
    {
        public int plan_id { get; set; }
        public string semester_code { get; set; }
        public int credit_hours { get; set; }
        public Advisor advisor { get; set; }
        public List<Course> courses { get; set; }
    }
}