using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DatabaseProject.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }
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
                cmd.Parameters.Add("@Semester", SqlDbType.VarChar,40);
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
    public void AddStudentPhone(FormCollection form)
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
}