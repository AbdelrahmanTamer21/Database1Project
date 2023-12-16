﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Xml.Linq;

namespace DatabaseProject.Models
{
    public class Student
    {
        public int student_id { get; set; }
        public string f_name { get; set; }
        public string l_name { get; set; }
        public string password { get; set; }
        public decimal gpa { get; set; }
        public string faculty { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; }
        public string major { get; set; }
        public bool financial_status { get; set; }
        public int semester { get; set; }
        public int acquired_hours { get; set; }
        public int assigned_hours { get; set; }
        public Advisor advisor { get; set; }
        public List<Course> courses { get; set; }
        public List<Payment> payments { get; set; }

        public Student(int student_id, string f_name, string l_name, string password, decimal gpa, string faculty, string email, string major, bool financial_status, int semester, int acquired_hours, int assigned_hours, Advisor advisor)
        {
            this.student_id = student_id;
            this.f_name = f_name;
            this.l_name = l_name;
            this.password = password;
            this.gpa = gpa;
            this.faculty = faculty;
            this.email = email;
            this.major = major;
            this.financial_status = financial_status;
            this.semester = semester;
            this.acquired_hours = acquired_hours;
            this.assigned_hours = assigned_hours;
            this.advisor = advisor;
        }

        public Student(string f_name, string l_name, string password, decimal gpa, string faculty, string email, string major, bool financial_status, int semester, int acquired_hours, int assigned_hours, Advisor advisor)
        {
            this.f_name = f_name;
            this.l_name = l_name;
            this.password = password;
            this.gpa = gpa;
            this.faculty = faculty;
            this.email = email;
            this.major = major;
            this.financial_status = financial_status;
            this.semester = semester;
            this.acquired_hours = acquired_hours;
            this.assigned_hours = assigned_hours;
            this.advisor = advisor;
        }

        public Student()
        {
        }

    }
}