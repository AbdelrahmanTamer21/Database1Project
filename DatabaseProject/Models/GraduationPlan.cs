using System.Collections.Generic;

namespace DatabaseProject.Models
{
    public class GraduationPlan
    {
        public int plan_id { get; set; }
        public string expected_grad_date { get; set; }
        public Student student { get; set; }

        public List<GraduationPlanSemester> semesters { get; set; }


        public GraduationPlan(int plan_id, string expected_grad_date, Student student, List<GraduationPlanSemester> semesters)
        {
            this.plan_id = plan_id;
            this.expected_grad_date = expected_grad_date;
            this.student = student;
            this.semesters = semesters;
        }

        public GraduationPlan()
        {
        }


    }
}