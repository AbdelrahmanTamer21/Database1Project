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
    }
}
