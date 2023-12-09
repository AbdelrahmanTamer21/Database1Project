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
        
        public ActionResult Login() {
            return View();
        }

        public ActionResult loginAdmin(FormCollection form) {
            if (form["admin_id"] == "1" && form["password"] == "pass") {
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
        /*
        private List<Request> listAllPendingRequests()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("Select * from all_Pending_Requests", con);
                cmd.CommandType = CommandType.Text;
                List<Request> lstPendingRequest = new List<Request>();
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
                            Request request = new Request();
                            // lma ne3raf
                            lstPendingRequest.add(request);
                        }
                    }
                    con.Close();
                    return lstPendingRequest;
                }
            }
        }
        */
        private void AddSemester(FormCollection form)
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
                }

            }


        }
        private void AddCourse(FormCollection form)
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

                }

            }


        }
        private void AdminLinkInstructor(FormCollection form)
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
                    cmd.Parameters.Add("@Type", SqlDbType.VarChar,40);
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
    }  
}

