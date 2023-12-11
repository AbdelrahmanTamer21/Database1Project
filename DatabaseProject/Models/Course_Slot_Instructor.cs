namespace DatabaseProject.Models
{
    public class Course_Slot_Instructor
    {
        public Course course { get; set; }
        public Slot slot { get; set; }
        public Instructor instructor { get; set; }
    }
}