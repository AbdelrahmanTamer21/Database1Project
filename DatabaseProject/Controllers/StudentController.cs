using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace DatabaseProject.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public int registerStudent(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@first_name", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@last_name", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@faculty", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 50);
                    cmd.Parameters.Add("@Semester", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@major", SqlDbType.VarChar, 40);
                    //state ouput variable
                    cmd.Parameters.Add("@Student_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@first_name"].Value = form["f_name"];
                    cmd.Parameters["@last_name"].Value = form["l_name"];
                    cmd.Parameters["@password"].Value = form["password"];
                    cmd.Parameters["@faculty"].Value = form["faculty"];
                    cmd.Parameters["@email"].Value = form["email"];
                    cmd.Parameters["@Semester"].Value = form["Semester"];
                    cmd.Parameters["@major"].Value = form["major"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //get the output variable
                    int id = Convert.ToInt32(cmd.Parameters["@Student_id"].Value);

                    con.Close();
                    return id;
                }
            }
        }
        public void addStudentPhone(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentaddMobile", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@StudentID", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@mobile_number", SqlDbType.VarChar, 40);


                    //set parameter values
                    cmd.Parameters["@StudentID"].Value = form["student_id"];
                    cmd.Parameters["@mobile_number"].Value = form["phone_number"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult optionalCourse(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Procedures_ViewOptionalCourse", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters.Add("@current_semester_code", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = form["student_id"];
                cmd.Parameters["@current_semester_code"].Value = form["current_semester_code"];

                List<Course> courses = new List<Course>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    courses.Add(
                        new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()
                            ));
                }
                rdr.Close();
                con.Close();
            }

            return View();
        }
        public ActionResult availableCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_SemsterAvailableCourses(@semstercode)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters.Add("@current_semester_code", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = form["student_id"];
                cmd.Parameters["@current_semester_code"].Value = form["current_semester_code"];

                List<Course> courses = new List<Course>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    courses.Add(
                        new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()
                            ));
                }
                rdr.Close();
                con.Close();
            }

            return View();
        }
        public ActionResult requiredCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Procedures_ViewRequiredCourses", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@semstercode", SqlDbType.Int);
                cmd.Parameters["@semstercode"].Value = form["semstercode"];

                List<Course> courses = new List<Course>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    courses.Add(
                        new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()
                            ));
                }
                rdr.Close();
                con.Close();
            }

            return View();
        }
        public int get_student_id() { return 0; }

        ///////////// PART 2 /////////////

        // A
        // View his/her graduation plan along with his/her assigned courses.
        public ActionResult GraduationPlan()
        {
            int student_id = get_student_id();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_StudentViewGP(@student_id)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = student_id;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                GraduationPlan graduationPlan = new GraduationPlan();

                if (rdr.Read()) { 
                    graduationPlan.plan_id = Convert.ToInt32(rdr["plan_id"]);
                    graduationPlan.expected_grad_date = rdr["expected_grad_date"].ToString();
                    graduationPlan.student = new Student();
                    graduationPlan.student.student_id = Convert.ToInt32(rdr["student_id"]);
                    graduationPlan.student.f_name = rdr["Student_name"].ToString().Split(' ')[0];
                    graduationPlan.student.l_name = rdr["Student_name"].ToString().Split(' ')[1];
                    
                    // Semester
                    GraduationPlanSemester semester = new GraduationPlanSemester();
                    semester.semester_code = rdr["semester_code"].ToString();
                    semester.credit_hours = Convert.ToInt32(rdr["credit_hours"]);
                    semester.advisor = new Advisor();
                    semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                    semester.courses = new List<Course>();
                    semester.courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["course_name"].ToString()));
                 
                } else
                {
                    rdr.Close();
                    con.Close();
                    return null;
                }

                while (rdr.Read())
                {
                    // Semester
                    if (rdr["semester_code"].ToString() != graduationPlan.semesters[graduationPlan.semesters.Count - 1].semester_code)
                    {
                        GraduationPlanSemester semester = new GraduationPlanSemester();
                        semester.semester_code = rdr["semester_code"].ToString();
                        semester.credit_hours = Convert.ToInt32(rdr["credit_hours"]);
                        semester.advisor = new Advisor();
                        semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                        semester.courses = new List<Course>();
                        semester.courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["course_name"].ToString()));
                        graduationPlan.semesters.Add(semester);
                    }
                    else
                    {
                        graduationPlan.semesters[graduationPlan.semesters.Count - 1].courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["course_name"].ToString()));
                    }
                }
                rdr.Close();
                con.Close();
            }

            return View();
        }
    }
}