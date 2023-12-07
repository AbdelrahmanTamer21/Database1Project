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
using System.Runtime.Remoting.Messaging;

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
        public ActionResult optionalCourse()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Procedures_ViewOptionalCourse", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters.Add("@current_semester_code", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = 1;
                cmd.Parameters["@current_semester_code"].Value = "W23";

                List<Course> courses = new List<Course>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    courses.Add(
                        new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()));
                }
                rdr.Close();
                con.Close();
                
                return View(courses);
            }

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
            
                return View(courses);
            }

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
            
                return View(courses);
            }

        }

        ///////////// PART 2 /////////////

        // A
        // View his/her graduation plan along with his/her assigned courses.
        public ActionResult GraduationPlan()
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_StudentViewGP(@student_id)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = 1;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                GraduationPlan graduationPlan = new GraduationPlan();

                if (rdr.HasRows) {
                    rdr.Read();
                    graduationPlan.plan_id = Convert.ToInt32(rdr["plan_id"]);
                    graduationPlan.expected_grad_date = rdr["expected_grad_date"].ToString();
                    graduationPlan.student = new Student();
                    graduationPlan.student.student_id = Convert.ToInt32(rdr["student_id"]);
                    graduationPlan.student.f_name = rdr["Student_name"].ToString().Split(' ')[0];
                    graduationPlan.student.l_name = rdr["Student_name"].ToString().Split(' ')[1];
                    
                    // Semester
                    GraduationPlanSemester semester = new GraduationPlanSemester();

                    graduationPlan.semesters = new List<GraduationPlanSemester>();
                    semester.semester_code = rdr["semester_code"].ToString();
                    semester.credit_hours = Convert.ToInt32(rdr["semester_credit_hours"]);
                    semester.advisor = new Advisor();
                    semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                    semester.courses = new List<Course>();
                    semester.courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()));

                    graduationPlan.semesters.Add(semester);
                 
                } else
                {
                    rdr.Close();
                    con.Close();

                    return View();
                }

                while (rdr.Read())
                {
                    // Semester
                    if (rdr["semester_code"].ToString() != graduationPlan.semesters[graduationPlan.semesters.Count - 1].semester_code)
                    {
                        GraduationPlanSemester semester = new GraduationPlanSemester();
                        semester.semester_code = rdr["semester_code"].ToString();
                        semester.credit_hours = Convert.ToInt32(rdr["semester_credit_hours"]);
                        semester.advisor = new Advisor();
                        semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                        semester.courses = new List<Course>();
                        semester.courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()));
                        graduationPlan.semesters.Add(semester);
                    }
                    else
                    {
                        graduationPlan.semesters[graduationPlan.semesters.Count - 1].courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()));
                    }
                }
                rdr.Close();
                con.Close();

                return View(graduationPlan);
            }
        }

        // B
        // View the upcoming not paid installment.
        public ActionResult UpcomingInstallment()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.FN_StudentUpcoming_installment", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = 1;

                cmd.Parameters.Add("@installdeadline", SqlDbType.Date).Direction = ParameterDirection.ReturnValue;

                con.Open();
                cmd.ExecuteNonQuery();
                
                var date = cmd.Parameters["@installdeadline"].Value.ToString();

                con.Close();
                return View(date);
            }
        }

        // C
        // View all courses along with their exams details.
        public ActionResult AllCourses()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Courses_MakeupExams", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<MakeUp_Exam> courses = new List<MakeUp_Exam>();

                while (rdr.Read())
                {
                    Course course = new Course();
                    course.name = rdr["name"].ToString();
                    course.semester = Convert.ToInt32(rdr["semester"]);

                    MakeUp_Exam makeUp_Exam = new MakeUp_Exam();
                    makeUp_Exam.exam_id = Convert.ToInt32(rdr["exam_id"]);
                    makeUp_Exam.date = rdr["date"].ToString();
                    makeUp_Exam.type = rdr["type"].ToString();
                    makeUp_Exam.course = course;

                    courses.Add(makeUp_Exam);
                }
                rdr.Close();
                con.Close();

                return View(courses);
            }
        }
    }
}