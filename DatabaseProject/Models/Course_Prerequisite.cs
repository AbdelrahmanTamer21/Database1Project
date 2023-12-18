namespace DatabaseProject.Models
{
    public class Course_Prerequisite
    {
        public int PrerequisiteID { get; set; }
        public string name { get; set; }
        public Course course { get; set; }
        public Course prerequisiteCourse { get; set; }

        public Course_Prerequisite() { }

        public Course_Prerequisite(int PrerequisiteID, string name, Course course)
        {
            this.PrerequisiteID = PrerequisiteID;
            this.name = name;
            this.course = course;
        }
    }
}