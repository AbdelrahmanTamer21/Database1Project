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
            return View();
        }

        public ActionResult Register() {

            return View();
        }

        public ActionResult Login() 
        {
            return View();
        }

        public int registerAdvisor(FormCollection form) {
            string name = form["name"];
            string email = form["email"];
            string password = form["password"];
            string office = form["office"];
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {

                    //set up the parameters
                    cmd.Parameters.Add("@advisor_name", SqlDbType.VarChar, 20);
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 20);
                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 50);
                    cmd.Parameters.Add("@office", SqlDbType.VarChar, 20);
                    //state ouput variable
                    cmd.Parameters.Add("@Advisor_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@advisor_name"].Value = name;
                    cmd.Parameters["@password"].Value = password;
                    cmd.Parameters["@email"].Value = email;
                    cmd.Parameters["@office"].Value = office;

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //get the output variable
                    int id = Convert.ToInt32(cmd.Parameters["@Advisor_id"].Value);

                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["userID"] = id.ToString();
                    userInfo["type"] = "Student";
                    Response.Cookies.Add(userInfo);

                    con.Close();
                    return id;
                }
            }
        }

        
    }
}