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

        public ActionResult Register(FormCollection form) {
            string name = form["name"];
            string email = form["email"];
            string password = form["password"];
            string office = form["office"];
            

            Advisor advisor = new Advisor(name, email, password, office);
            advisor.advisor_id = registerAdvisor(advisor);

            Session["advisor"] = advisor;

            return View();
        }

        public int registerAdvisor(Advisor advisor) {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdvisorRegistration", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@email", SqlDbType.VarChar);
                    cmd.Parameters.Add("@office", SqlDbType.VarChar, 40);
                    //state ouput variable
                    cmd.Parameters.Add("@advisor_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@name"].Value = advisor.name;
                    cmd.Parameters["@password"].Value = advisor.password;
                    cmd.Parameters["@email"].Value = advisor.email;
                    cmd.Parameters["@office"].Value = advisor.office;

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    //get the output variable
                    int id = Convert.ToInt32(cmd.Parameters["@advisor_id"].Value);

                    con.Close();
                    return id;
                }
            }
        }
    }
}