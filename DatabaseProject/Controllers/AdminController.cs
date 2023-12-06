using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
        private List<Advisor> listAllAdvisors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("Select * from Advisor", con);
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
                            advisor.advisor_id = rdr.GetInt32("advisor_id");
                            advisor.name = rdr.GetString("Name");
                            advisor.email = rdr.GetString("email");
                            advisor.password = rdr.GetString("password");
                            advisor.office = rdr.GetString("office");
                            lstAdviors.add(advisor);
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
                SqlCommand cmd = new SqlCommand("Select * from Student", con);
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
                            student.student_id = rdr.GetInt32("advisor_id");
                            student.f_name = rdr.GetString("f_name");
                            student.l_name = rdr.GetString("l_name");
                            student.advisor_id = rdr.GetInt32("advisor_id");
                            student.advisor_name = rdr.GetSingle("advisor_name");// mehtaga tet8ayar
                            lstStudent.add(student);
                        }
                    }
                    con.Close();
                    return lstStudent;
                }
            }
        }
        private List<Request> listAllPendingRequests()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("Select * from all_Pending_Requests", con);
                cmd.CommandType = CommandType.text;
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
                    cmd.Parameters.Add("@start_date", SqlDbType.date);
                    cmd.Parameters.Add("@end_date", SqlDbType.date);
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
        private void AdminLinkStudentToAdvisor()
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

