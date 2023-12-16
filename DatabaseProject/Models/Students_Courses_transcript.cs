using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Students_Courses_transcript
    {
        public Student student { get; set; }
        public Course course { get; set; }
        public Student_Instructor_Course_take student_Instructor_Course_take { get; set; }


        public Students_Courses_transcript() { }

        public Students_Courses_transcript(Student student, Course course, Student_Instructor_Course_take student_Instructor_Course_take)
        {
            this.student = student;
            this.course = course;
            this.student_Instructor_Course_take = student_Instructor_Course_take;

        }
    }
}