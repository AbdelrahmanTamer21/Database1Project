using System.Collections;

namespace DatabaseProject.Models
{
    public class Course
    {
        public int course_id { get; set; }
        public string name { get; set; }
        public string major { get; set; }
        public bool is_offered { get; set; }
        public int credit_hours { get; set; }
        public int semester { get; set; }

        public Course(int course_id, string name, string major, bool is_offered, int credit_hours, int semester)
        {
            this.course_id = course_id;
            this.name = name;
            this.major = major;
            this.is_offered = is_offered;
            this.credit_hours = credit_hours;
            this.semester = semester;
        }

        public Course(int course_id, string name)
        {
            this.course_id = course_id;
            this.name = name;
        }
    }
}