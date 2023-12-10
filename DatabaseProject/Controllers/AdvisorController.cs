using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DatabaseProject.Controllers
{
    public class AdvisorController : Controller
    {
        // GET: Advisor
        public ActionResult Index()
        {
            if (Session["userID"] != null && Session["type"] == "Advisor")
            {
                List<Student> students = getAdvisingStudents(Convert.ToInt32(Session["userID"]));
                return View(students);
            }
            else
            {
                return View(new List<Student>());
            }
        }

        public ActionResult Register() {

            return View();
        }

        public ActionResult Login() 
        {
            return View();
        }

        public ActionResult GradPlan(int student_id)
        {
            return View(getStudentGraduationPlan(student_id));
        }

        public ActionResult InsertGradPlan(int student_id)
        {
            return View();
        }

        public ActionResult AssignedStudentsCourses()
        {
            ViewBag.Majors = new SelectList(getMajors(),"Value","Text");
            return View(getAssignedStudents("CS"));
        }

        public int registerAdvisor(FormCollection form) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {

                    //set up the parameters
                    cmd.Parameters.AddWithValue("@advisor_name", form["name"]);
                    cmd.Parameters.AddWithValue("@password", form["password"]);
                    cmd.Parameters.AddWithValue("@email", form["email"]);
                    cmd.Parameters.AddWithValue("@office", form["office"]);
                    //state ouput variable
                    cmd.Parameters.Add("@Advisor_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //get the output variable
                    int id = Convert.ToInt32(cmd.Parameters["@Advisor_id"].Value);

                    con.Close();
                    return id;
                }
            }
        }

        public ActionResult loginAdvisor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT dbo.FN_AdvisorLogin(@advisor_Id,@password) AS Success", con);
                cmd.CommandType = CommandType.Text;
                using (cmd)
                {
                    //set up the parameters
                    cmd.Parameters.AddWithValue("@advisor_Id", form["advisor_id"]);
                    cmd.Parameters.AddWithValue("@password", form["password"]);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();


                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        bool success = Convert.ToBoolean(rdr["Success"]);
                        if(!success)
                        {
                            TempData["LoginError"] = "ID or Password are wrong";
                            rdr.Close();
                            con.Close();
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            TempData["LoginError"] = null;
                            rdr.Close();
                            con.Close();

                            Session["userID"] = form["advisor_id"];
                            Session["type"] = "Advisor";
                            return RedirectToAction("Index");
                        }
                    }
                    return RedirectToAction("Login");
                }
            }

        }

        public List<Student> getAdvisingStudents(int id)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE advisor_id = @advisor_id", con);
                cmd.CommandType = CommandType.Text;
                List<Student> lstStudent = new List<Student>();
                using (cmd)
                {
                    //set up parameteres
                    cmd.Parameters.AddWithValue("@advisor_id", id);

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

        public GraduationPlan getStudentGraduationPlan(int student_id)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_StudentViewGP(@student_id)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@student_id", student_id);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                GraduationPlan graduationPlan = new GraduationPlan();

                if (rdr.HasRows)
                {
                    rdr.Read();
                    graduationPlan.plan_id = Convert.ToInt16(rdr["plan_id"]);
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

                }
                else
                {
                    rdr.Close();
                    con.Close();
                    graduationPlan.semesters = new List<GraduationPlanSemester>();
                    return graduationPlan;
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

                return graduationPlan;
            }
        }

        public ActionResult insertGradPlanSql (int student_id,FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorCreateGP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {

                    //set up the parameters
                    cmd.Parameters.AddWithValue("@Semester_code", form["Semester_code"]);
                    cmd.Parameters.AddWithValue("@expected_graduation_date", form["expected_graduation_date"]);
                    cmd.Parameters.AddWithValue("@sem_credit_hours", form["credit_hours"]);
                    cmd.Parameters.AddWithValue("@advisor_id", Session["userID"]);
                    cmd.Parameters.AddWithValue("@student_id", student_id);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
            return RedirectToAction("GradPlan", new { student_id = student_id});
        }

        private List<SelectListItem> getMajors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT major FROM Student", con);
                cmd.CommandType = CommandType.Text;
                List<SelectListItem> list = new List<SelectListItem>();
                using (cmd)
                {
                    //open connection and execute query
                    con.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Text = rdr["major"].ToString(),
                            Value = rdr["major"].ToString(),
                            Selected = false
                        });
                    }
                    con.Close();
                }
                return list;
            }
        }

        public List<Student> getAssignedStudents(string major)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorViewAssignedStudents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Student> lstStudent = new List<Student>();
                using (cmd)
                {
                    if (Session["userID"] == null)
                    {
                        return lstStudent;
                    }
                    //set up parameteres
                    cmd.Parameters.AddWithValue("@AdvisorID", Session["userID"]);
                    cmd.Parameters.AddWithValue("@major", major);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        Student student = new Student();
                        student.courses = new List<Course>();
                        if (rdr.Read())
                        {
                            student.student_id = Convert.ToInt32(rdr["student_id"]);
                            student.f_name = rdr["Student_name"].ToString().Split(' ')[0];
                            student.l_name = rdr["Student_name"].ToString().Split(' ')[1];
                            student.major = rdr["major"].ToString();
                            Course course = new Course();
                            course.name = rdr["Course_name"].ToString();
                            student.courses.Add(course);
                        }
                        while (rdr.Read())
                        {
                            if (Convert.ToInt32(rdr["student_id"]) == student.student_id)
                            {
                                Course course = new Course();
                                course.name = rdr["Course_name"].ToString();
                                student.courses.Add(course);
                            }
                            else
                            {
                                lstStudent.Add(student);
                                student = new Student();
                                student.courses = new List<Course>();
                                student.student_id = Convert.ToInt32(rdr["student_id"]);
                                student.f_name = rdr["Student_name"].ToString().Split(' ')[0];
                                student.l_name = rdr["Student_name"].ToString().Split(' ')[1];
                                student.major = rdr["major"].ToString();
                                Course course = new Course();
                                course.name = rdr["Course_name"].ToString();
                                student.courses.Add(course);
                            }
                        }
                        lstStudent.Add(student);
                    }
                    con.Close();
                    return lstStudent;
                }
            }
        }
    }
}