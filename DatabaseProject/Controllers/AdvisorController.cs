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
            return View();
        }

        public ActionResult InsertGradPlan(int student_id)
        {
            return View();
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

        public ActionResult insertGradPlan(int student_id,FormCollection form)
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
    }
}