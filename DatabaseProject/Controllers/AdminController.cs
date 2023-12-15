using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DatabaseProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult loginAdmin(FormCollection form)
        {
            if (form["admin_id"] == "1" && form["password"] == "pass")
            {
                TempData["LoginError"] = null;

                Session["userID"] = form["admin_id"];
                Session["type"] = "Admin";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["LoginError"] = "ID or Password are wrong";
                return RedirectToAction("Login");
            }

        }
        private List<Advisor> listAllAdvisors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminListAdvisors", con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Advisor> lstAdviors = new List<Advisor>();
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Advisor advisor = new Advisor();
                            advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                            advisor.name = rdr["advisor_name"].ToString();
                            advisor.email = rdr["email"].ToString();
                            advisor.password = rdr["password"].ToString();
                            advisor.office = rdr["office"].ToString();
                            lstAdviors.Add(advisor);
                        }
                    }
                    con.Close();
                    return lstAdviors;
                }
            }
        }
        private List<Student> listAllStudent()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminListStudents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Student> lstStudent = new List<Student>();
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Student student = new Student();
                            student.student_id = Convert.ToInt32(rdr["student_id"]);
                            student.f_name = rdr["f_name"].ToString();
                            student.l_name = rdr["l_name"].ToString();
                            student.password = rdr["password"].ToString();
                            student.gpa = Convert.ToDecimal(rdr["gpa"]);
                            student.faculty = rdr["faculty"].ToString();
                            student.email = rdr["email"].ToString();
                            student.major = rdr["major"].ToString();
                            student.financial_status = Convert.ToBoolean(rdr["financial_status"]);
                            student.semester = Convert.ToInt16(rdr["semester"]);
                            student.acquired_hours = Convert.ToInt16(rdr["acquired_hours"]);
                            student.assigned_hours = Convert.ToInt16(rdr["assigned_hours"]);
                            student.advisor = new Advisor();
                            student.advisor.advisor_id = Convert.ToInt16(rdr["advisor_id"]);
                            lstStudent.Add(student);
                        }
                    }
                    con.Close();
                    return lstStudent;
                }
            }
        }

        public List<Request> AdminListPendingRequests()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM all_Pending_Requests", con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Request> lstRequest = new List<Request>();
                using (cmd)
                {
                    //set up parameteres
                    //cmd.Parameters.AddWithValue("@Advisor_ID", advisor_id);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Request request = new Request();
                            request.request_id = Convert.ToInt16(rdr["request_id"]);
                            request.type = rdr["type"].ToString();
                            request.comment = rdr["comment"].ToString();
                            request.status = rdr["status"].ToString();
                            switch (request.type)
                            {
                                case "credit_hours":
                                    request.credit_hours = Convert.ToInt16(rdr["credit_hours"]);
                                    break;
                                case "course":
                                    request.course = new Course();
                                    request.course.course_id = Convert.ToInt16(rdr["course_id"]);
                                    break;
                            }
                            request.student = new Student();
                            request.student.f_name = rdr["f_name"].ToString();
                            request.student.l_name = rdr["l_name"].ToString();
                            request.advisor = new Advisor();
                            request.advisor.name = rdr["name"].ToString();
                            lstRequest.Add(request);
                        }
                    }
                    con.Close();
                    return lstRequest;
                }
            }
        }

        private ActionResult AddSemester(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminAddingSemester", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@start_date", SqlDbType.Date);
                    cmd.Parameters.Add("@end_date", SqlDbType.Date);
                    cmd.Parameters.Add("@semester_code", SqlDbType.VarChar, 40);
                    //state ouput variable
                    cmd.Parameters.Add("@semester_code", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@start_date"].Value = form["start_date"];
                    cmd.Parameters["@end_date"].Value = form["end_date"];
                    cmd.Parameters["@semester_code"].Value = form["semester_code"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                    return RedirectToAction("Index");
                }

            }


        }
        private ActionResult AddCourse(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorAddCourseGP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@student_id", SqlDbType.Int);
                    cmd.Parameters.Add("@Semester_code", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@course_name", SqlDbType.VarChar, 40);
                    //state ouput variable
                    cmd.Parameters.Add("@semester_code", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@student_id"].Value = form["student_id"];
                    cmd.Parameters["@Semester_code"].Value = form["Semester_code"];
                    cmd.Parameters["@course_name"].Value = form["course_name"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();


                    con.Close();
                    return RedirectToAction("Index");
                }

            }


        }
        private ActionResult AdminLinkInstructor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {

                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminLinkInstructor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@cours_id", SqlDbType.Int);
                    cmd.Parameters.Add("@instructor_id", SqlDbType.Int);
                    cmd.Parameters.Add("@slot_id", SqlDbType.Int);

                    //state ouput variable
                    cmd.Parameters.Add("@slot_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@course_id"].Value = form["course_id"];
                    cmd.Parameters["@instructor_id"].Value = form["instructor_id"];
                    cmd.Parameters["@slot_id"].Value = form["slot_id"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();


                    con.Close();
                    return RedirectToAction("Index");
                }

            }


        }
        private void AdminLinkStudentToAdvisor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminLinkStudentToAdvisor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@studentID", SqlDbType.Int);
                    cmd.Parameters.Add("@advisorID", SqlDbType.Int);
                    //state ouput variable
                    cmd.Parameters.Add("@semester_code", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@studentID"].Value = form["studentID"];
                    cmd.Parameters["@advisorID"].Value = form["advisorID"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }
        private void student_course_instructor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminLinkStudent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@cours_id", SqlDbType.Int);
                    cmd.Parameters.Add("@instructor_id", SqlDbType.Int);
                    cmd.Parameters.Add("@studentID", SqlDbType.Int);
                    cmd.Parameters.Add("semester_code", SqlDbType.VarChar);


                    //state ouput variable
                    cmd.Parameters.Add("@semester_code", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@cours_id"].Value = form["cours_id"];
                    cmd.Parameters["@instructor_id"].Value = form["instructor_id"];
                    cmd.Parameters["@studentID"].Value = form["studentID"];
                    cmd.Parameters["@semester_code"].Value = form["semester_code"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }
        private void instructor_assingedcourses(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Instructors_AssignedCourses", con);
                cmd.CommandType = CommandType.Text;
                //  List<Instructor> lstInstructor = new List<Instructor>();

                // List < Request > lstRequest = new List<Request>();
                using (cmd)
                {
                    //set up parameteres
                    //   cmd.Parameters.AddWithValue("@instructor_id", instructor_id);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {

                    }
                    con.Close();
                    /*string view = "SELECT * FROM Semster_offered_Courses";
                    SqlDataAdapter views = new SqlDataAdapter(view, con);
                    DataTable dataTable = new DataTable();
                    views.Fill(dataTable);
                    //  YourGridView.DataSource = dataTable;
                    //  YourGridView.DataBind();*/
                }
            }
        }
        private void semesters_offeredcourses(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT* FROM Semster_offered_Courses", con);
                cmd.CommandType = CommandType.Text;
                // List<Course> lstCourse = new List<Course>();
                using (cmd)
                {
                    //set up parameteres
                    //cmd.Parameters.AddWithValue("@Advisor_ID", advisor_id);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {

                        con.Close();
                      /*  string view = "SELECT * FROM Semster_offered_Courses";
                        SqlDataAdapter views = new SqlDataAdapter(view, con);
                        DataTable dataTable = new DataTable();
                        views.Fill(dataTable);
                        //  YourGridView.DataSource = dataTable;
                        //  YourGridView.DataBind();*/
                    }
                }
            }
        }

        public void deleteCourse(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminDeleteCourse", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@courseID", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@courseID"].Value = form["course_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public void deleteSlots(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminDeleteSlots", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@current_semester", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@current_semester"].Value = form["semester_code"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public void addMakeUpExam(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminAddExam", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@Type", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@date", SqlDbType.DateTime);
                    cmd.Parameters.Add("@courseID", SqlDbType.Int);


                    //set parameter values
                    cmd.Parameters["@Type"].Value = form["type"];
                    cmd.Parameters["@date"].Value = form["date"];
                    cmd.Parameters["@courseID"].Value = form["course_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult AllPaymentsWithCorrStudents()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student_Payment", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Student_Payment> StudentPayments = new List<Student_Payment>();

                while (rdr.Read())
                {
                    Student student = new Student();
                    student.student_id = Convert.ToInt32(rdr["StudentID"]);
                    student.f_name = rdr["f_name"].ToString();
                    student.l_name = rdr["l_name"].ToString();
                    Payment payment = new Payment();
                    Payment.payment_id = Convert.ToInt32(rdr["payment_id"]);
                    Payment.amount = Convert.ToInt32(rdr["amount"]);
                    // datetime?   Payment.startdate = rdr["startdate"].ToString();
                    // datetime?   Payment.deadline = rdr["deadline"].ToString();
                    Payment.n_installments = Convert.ToInt32(rdr["n_installments"]);
                    Payment.fund_percentage = Convert.ToDecimal(rdr["fund_percentage"]);
                    Payment.status = rdr["status"].ToString();




                    Student_Payment student_payment = new Student_Payment();
                    student_payment.student = student;
                    student_payment.payment = payment;


                    StudentPayments.Add(student_payment);
                }
                rdr.Close();
                con.Close();

                return View(StudentPayments);
            }
        }
        public void issueInstallments(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminIssueInstallment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@payment_id", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@payment_id"].Value = form["payment_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public void updateStudentStatus(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedure_AdminUpdateStudentStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@student_id", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@student_id"].Value = form["student_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult AllActiveStudent()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM view_Students", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Student> students = new List<Student>();

                while (rdr.Read())
                {
                    Student student = new Student();
                    student.student_id = Convert.ToInt32(rdr["student_id"]);
                    student.f_name = rdr["f_name"].ToString();
                    student.l_name = rdr["l_name"].ToString();
                    student.password = rdr["password"].ToString();
                    student.gpa = Convert.ToDecimal(rdr["gpa"]);
                    student.faculty = rdr["faculty"].ToString();
                    student.email = rdr["email"].ToString();
                    student.major = rdr["major"].ToString();
                    student.financial_status = Convert.ToBoolean(rdr["financial_status"]);
                    student.semester = Convert.ToInt32(rdr["semester"]);
                    student.acquired_hours = Convert.ToInt32(rdr["acquired_hours"]);
                    student.assigned_hours = Convert.ToInt32(rdr["assigned_hours"]);
                    student.advisor = new Advisor();
                    student.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);


                    students.Add(student);
                }
                rdr.Close();
                con.Close();

                return View(students);
            }
        }
        /*  public ActionResult gradPlansWithInitAdvisors()
          {
              SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

              using (con)
              {
                  SqlCommand cmd = new SqlCommand("SELECT * FROM Advisors_Graduation_Plan", con);
                  cmd.CommandType = CommandType.Text;

                  con.Open();
                  SqlDataReader rdr = cmd.ExecuteReader();

                  List<Advisors_Graduation_Plan> AdvGradPlan = new List<Advisors_Graduation_Plan>();

                  while (rdr.Read())
                  {
                      GraduationPlan plan = new GraduationPlan();
                      plan.plan_id = Convert.ToInt32(rdr["plan_id"]);
                      Advisor advisor = new Advisor();
                      advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                      advisor.name = rdr["advisor_name"].ToString();

                      Advisors_Graduation_Plan advisor_grad_plan = new Advisors_Graduation_Plan();
                      advisor_grad_plan.graduation_plan = plan;
                      advisor_grad_plan.advisor = advisor;


                      courses.Add(advisor_grad_plan);
                  }
                  rdr.Close();
                  con.Close();

                  return View(AdvGradPlan);
              }
          }*/
        /* public ActionResult AllStudentsTranscript()
         {
             SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

             using (con)
             {
                 SqlCommand cmd = new SqlCommand("SELECT * FROM Students_Courses_transcript", con);
                 cmd.CommandType = CommandType.Text;

                 con.Open();
                 SqlDataReader rdr = cmd.ExecuteReader();

                 List<Students_Courses_transcript> transcript = new List<Students_Courses_transcript>();
                 while (rdr.Read())
                 {
                     Student student = new Student();
                     student.student_id = Convert.ToInt32(rdr["student_id"]);
                     student.f_name = rdr["f_name"].ToString();
                     student.l_name = rdr["l_name"].ToString();

                     Course course = new Course();
                     course.name = rdr["name"].ToString();

                     /*  not sure     
                                 Student_Instructor_Course_take take = new Student_Instructor_Course_take();
                                 take.course_id = Convert.ToInt32(rdr["course_id"]);
                                 take.exam_type = rdr["exam_type"].ToString();
                                 take.grade = rdr["grade"].ToString();
                                 take.semester_code = rdr["semester_code"].ToString(); 
                     */

        /*      Students_Courses_transcript studentCourseTranscript = new Students_Courses_transcript();
              studentCourseTranscript.student = student;
              studentCourseTranscript.course = course;
              studentCourseTranscript.student_Instructor_Course_take = take;

              transcript.Add(studentCourseTranscript);
          }
          rdr.Close();
          con.Close();

          return View(transcript);
      }
  }*/
        /*    public ActionResult AllSemestersWithOfferedCourses()
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

                using (con)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Semster_offered_Courses", con);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    List<Semster_offered_Courses> SemCourses = new List<Semster_offered_Courses>();
                    while (rdr.Read())
                    {
                        Course course = new Course();
                        course.course_id = Convert.ToInt32(rdr["course_id"]);
                        course.name = rdr["name"].ToString();

                        Semester semester = new Semester();
                        semester.semester_code = rdr["semester_code"].ToString();


                        Semster_offered_Courses soc = new Semster_offered_Courses();
                        soc.course = course;
                        soc.semester = semester;


                        SemCourses.Add(soc);
                    }
                    rdr.Close();
                    con.Close();

                    return View(SemCourses);
                }
                ﻿namespace DatabaseProject.Models
        {
            public class Semster_offered_Courses
            {

                public Course course { get; set; }
                public Semester semester { get; set; }


                public Semster_offered_Courses() { }

                public Semster_offered_Courses(Course course , Semester semester)
                {
                    this.course = course;
                    this.semester = semester;
                }
            }
        }
        ﻿namespace DatabaseProject.Models
	{
		public class Students_Courses_transcript
		{
		
       
			public Student student { get; set; }
			public Course course { get; set; }
			public Student_Instructor_Course_take student_Instructor_Course_take { get; set; }
			

			public Students_Courses_transcript() { }

			public Students_Courses_transcript(Student student ,Course course , Student_Instructor_Course_take student_Instructor_Course_take)
			{
				this.student = student;
				this.course = course;
				this.student_Instructor_Course_take = student_Instructor_Course_take ;
				
			}
		}
	}
        //lesa mt3mlsh hshoof lw hzwed take.course.course_id f (i)
	﻿namespace DatabaseProject.Models
	{
		public class Student_Instructor_Course_take
		{
		
       
			public Student student { get; set; }
			public Course course { get; set; }
			public Student_Instructor_Course_take student_Instructor_Course_take { get; set; }
			

			public Student_Instructor_Course_take() { }

			public Student_Instructor_Course_take(Student student ,Course course , Student_Instructor_Course_take student_Instructor_Course_take)
			{
				this.student = student;
				this.course = course;
				this.student_Instructor_Course_take = student_Instructor_Course_take ;
				
			}
		}
	}

        */



    }
}

