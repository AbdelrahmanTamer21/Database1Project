using System.Collections.Generic;

namespace DatabaseProject.Models
{
    public class GraduationPlan
    {
        public string expected_grad_date { get; set; }
        public Student student { get; set; }

        public List<GraduationPlanSemester> semesters { get; set; }


        public GraduationPlan(string expected_grad_date, Student student, List<GraduationPlanSemester> semesters)
        {
            this.expected_grad_date = expected_grad_date;
            this.student = student;
            this.semesters = semesters;
        }

        public GraduationPlan()
        {
        }


    }
}