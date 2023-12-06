namespace DatabaseProject.Models
{
    public class GraduationPlan
    {
        public int plan_id { get; set; }
        public string semester_code { get; set; }
        public int credit_hours { get; set; }
        public string expected_grad_date { get; set; }
        public Advisor advisor { get; set; }
        public Student student { get; set; }


        public GraduationPlan(int plan_id, string semester_code, int credit_hours, string expected_grad_date, Advisor advisor, Student student)
        {
            this.plan_id = plan_id;
            this.semester_code = semester_code;
            this.credit_hours = credit_hours;
            this.expected_grad_date = expected_grad_date;
            this.advisor = advisor;
            this.student = student;
        }
    }
}