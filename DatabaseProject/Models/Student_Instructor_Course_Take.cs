using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Student_Instructor_Course_take
    {
        public Student student { get; set; }
        public Course course { get; set; }
        public Instructor instructor { get; set; }
        public string semester_code { get; set; }
        public string exam_type { get; set; }
        public string grade { get; set; }



        public Student_Instructor_Course_take() { }

        public Student_Instructor_Course_take(Student student, Course course, Instructor instructor, string semester_code, string exam_type, string grade)
        {
            this.student = student;
            this.course = course;
            this.instructor = instructor;
            this.semester_code = semester_code;
            this.exam_type = exam_type;
            this.grade = grade;
        }
    }
}