using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
                while (rdr.Read())
                {
                    Console.WriteLine(rdr);
                }
                rdr.Close();
                con.Close();
            }

            return View();
        }

    }
}