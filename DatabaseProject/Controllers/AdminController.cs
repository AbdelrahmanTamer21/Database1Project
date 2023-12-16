using DatabaseProject.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult loginAdmin(FormCollection form)
        {
            if (form["admin_id"] == "1" && form["password"] == "pass")
            {
                TempData["LoginError"] = null;

                Session["userID"] = form["admin_id"];
                Session["type"] = "Admin";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["LoginError"] = "ID or Password are wrong";
                return RedirectToAction("Login");
            }

        }
        public ActionResult listAllAdvisors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminListAdvisors", con);
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
                            advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                            advisor.name = rdr["advisor_name"].ToString();
                            advisor.email = rdr["email"].ToString();
                            advisor.password = rdr["password"].ToString();
                            advisor.office = rdr["office"].ToString();
                            lstAdviors.Add(advisor);
                        }
                    }
                    con.Close();
                    return View(lstAdviors);
                }
            }
        }
        public ActionResult listAllStudent()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.AdminListStudentsWithAdvisors", con);
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
                            student.student_id = Convert.ToInt32(rdr["student_id"]);
                            student.f_name = rdr["f_name"].ToString();
                            student.l_name = rdr["l_name"].ToString();
                         
                            student.advisor = new Advisor();
                            student.advisor.advisor_id = Convert.ToInt16(rdr["advisor_id"]);
                            student.advisor.name = rdr["Advisor"].ToString();
                            lstStudent.Add(student);
                        }
                    }
                    con.Close();
                    return view(lstStudent);
                }
            }
        }

        public ActionResult AdminListPendingRequests()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM all_Pending_Requests", con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Request> lstRequest = new List<Request>();
                using (cmd)
                {
                    //set up parameteres
                    //cmd.Parameters.AddWithValue("@Advisor_ID", advisor_id);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Request request = new Request();
                            request.request_id = Convert.ToInt16(rdr["request_id"]);
                            request.type = rdr["type"].ToString();
                            request.comment = rdr["comment"].ToString();
                            request.status = rdr["status"].ToString();
                            switch (request.type)
                            {
                                case "credit_hours":
                                    request.credit_hours = Convert.ToInt16(rdr["credit_hours"]);
                                    break;
                                case "course":
                                    request.course = new Course();
                                    request.course.course_id = Convert.ToInt16(rdr["course_id"]);
                                    break;
                            }
                            request.student = new Student();
                            request.student.f_name = rdr["f_name"].ToString();
                            request.student.l_name = rdr["l_name"].ToString();
                            request.advisor = new Advisor();
                            request.advisor.name = rdr["name"].ToString();
                            lstRequest.Add(request);
                        }
                    }
                    con.Close();
                    return View(lstRequest);
                }
            }
        }
        public ActionResult AddSemesterForm()
        {
            return View();
        }
        public ActionResult AddSemester(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.AdminAddingSemester", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@start_date", SqlDbType.Date);
                    cmd.Parameters.Add("@end_date", SqlDbType.Date);
                    cmd.Parameters.Add("@semester_code", SqlDbType.VarChar, 40);

                    //set parameter values
                    cmd.Parameters["@start_date"].Value = form["start_date"];
                    cmd.Parameters["@end_date"].Value = form["end_date"];
                    cmd.Parameters["@semester_code"].Value = form["semester_code"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                    return RedirectToAction("Index");
                }

            }


        }
        public ActionResult AddCourseForm()
        {
            return View();
        }
        private ActionResult AddCourse(FormCollection form)
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
                    return RedirectToAction("Index");
                }

            }


        }
        public ActionResult linkInstructorCourseSlot() {
            ViewBag.Courses = new SelectList(AdvisorController.getCourseIDs(), "Value", "Text");
            ViewBag.Instructors = new SelectList(getInstructors(), "Value", "Text");
            ViewBag.Slots = new SelectList(getSlots(), "Value", "Text");
            return View();
        }

        public List<SelectListItem> getInstructors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT instructor_id,name FROM Instructor", con);
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
                            Text = rdr["name"].ToString(),
                            Value = rdr["instructor_id"].ToString(),
                            Selected = false
                        });
                    }
                    con.Close();
                }
                return list;
            }
        }

        public List<SelectListItem> getSlots()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Slot", con);
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
                            Text = rdr["day"].ToString() + " " + rdr["time"].ToString() + " " + rdr["location"].ToString(),
                            Value = rdr["slot_id"].ToString(),
                            Selected = false
                        });
                    }
                    con.Close();
                }
                return list;
            }
        }

        public ActionResult AdminLinkInstructor(FormCollection form)
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

                    //set parameter values
                    cmd.Parameters["@cours_id"].Value = form["course_id"];
                    cmd.Parameters["@instructor_id"].Value = form["instructor_id"];
                    cmd.Parameters["@slot_id"].Value = form["slot_id"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();


                    con.Close();
                    return RedirectToAction("Index");
                }

            }


        }
        public ActionResult AdminLinkStudentToAdvisorForm()
        {
            return View();
        }
        public ActionResult AdminLinkStudentToAdvisor(FormCollection form)
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
        public ActionResult student_course_instructorForm()
        {
            return View();
        }
        public ActionResult student_course_instructor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminLinkStudent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@cours_id", SqlDbType.Int);
                    cmd.Parameters.Add("@instructor_id", SqlDbType.Int);
                    cmd.Parameters.Add("@studentID", SqlDbType.Int);
                    cmd.Parameters.Add("semester_code", SqlDbType.VarChar);


                    //state ouput variable
                    cmd.Parameters.Add("@semester_code", SqlDbType.Int).Direction = ParameterDirection.Output;

                    //set parameter values
                    cmd.Parameters["@cours_id"].Value = form["cours_id"];
                    cmd.Parameters["@instructor_id"].Value = form["instructor_id"];
                    cmd.Parameters["@studentID"].Value = form["studentID"];
                    cmd.Parameters["@semester_code"].Value = form["semester_code"];

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();

                }

            }

        }
        public ActionResult instructor_assingedcourses()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Instructors_AssignedCourses", con);
                cmd.CommandType = CommandType.Text;
                //  List<Instructor> lstInstructor = new List<Instructor>();

                // List < Request > lstRequest = new List<Request>();
                using (cmd)
                {
                    //set up parameteres
                    List<Instructor> instructors = new List<Instructor>();

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        Instructor instructor = new Instructor();
                        instructor.courses = new List<Course>();
                        if (rdr.Read())
                        {
                            instructor.instructor_id = Convert.ToInt32(rdr["instructor_id"]);
                            instructor.name = rdr["Instructor"].ToString();
                            Course course = new Course();
                            course.course_id = Convert.ToInt32(rdr["course_id"]);
                            course.name = rdr["Course"].ToString();
                            instructor.courses.Add(course);
                        }
                        else
                        {
                            con.Close();
                            instructors.Add(instructor);
                            return View(instructors);
                        }
                        while (rdr.Read())
                        {
                            if (Convert.ToInt32(rdr["instructor_id"]) != instructor.instructor_id)
                            {
                                instructors.Add(instructor);
                                instructor = new Instructor();
                                instructor.instructor_id = Convert.ToInt32(rdr["instructor_id"]);
                                instructor.name = rdr["Instructor"].ToString();
                                instructor.courses = new List<Course>();
                            }
                            Course course = new Course();
                            course.course_id = Convert.ToInt32(rdr["course_id"]);
                            course.name = rdr["Course"].ToString();
                            instructor.courses.Add(course);
                        }
                        instructors.Add(instructor);

                    }
                    con.Close();
                    return View(instructors);
                }
            }
        }
        public ActionResult semesters_offeredcourses()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT* FROM Semster_offered_Courses", con);
                cmd.CommandType = CommandType.Text;
                List<Semester> semesters = new List<Semester>();
                using (cmd)
                {
                    Semester semester = new Semester();
                    semester.courses = new List<Course>();
                    if (rdr.Read())
                    {
                        semester.semester_code = rdr["semester_code"].ToString();
                      
                        Course course = new Course();
                        course.course_id = Convert.ToInt32(rdr["course_id"]);
                        course.name = rdr["Course"].ToString();
                        semester.courses.Add(course);
                    }
                    else
                    {
                        con.Close();
                        semesters.Add(semester);
                        return View(semesters);
                    }
                    while (rdr.Read())
                    {
                        if (rdr["semester_code"].ToString() != semester.semester_code)
                        {
                            semesters.Add(semester);
                            semester = new Semester();
                            semester.semester_code = rdr["semester_code"].ToString();
                            semester.courses = new List<Course>();
                        }
                        Course course = new Course();
                        course.course_id = Convert.ToInt32(rdr["course_id"]);
                        course.name = rdr["Course"].ToString();
                        semester.courses.Add(course);
                    }
                    semesters.Add(semester);

                }
                con.Close();
                return View(semesters);
            
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
        public void deleteSlots(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminDeleteSlots", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@current_semester", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@current_semester"].Value = form["semester_code"];


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
                    cmd.Parameters.Add("@Type", SqlDbType.VarChar, 40);
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
        public ActionResult AllPaymentsWithCorrStudents()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student_Payment", con);
                cmd.CommandType = CommandType.Text;
                using (cmd)
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    List<Student> StudentPayments = new List<Student>();
                    using (rdr)
                    {
                        Student student = new Student();
                        student.payments = new List<Payment>();
                        if (rdr.Read())
                        {
                            student.student_id = Convert.ToInt32(rdr["studentID"]);
                            student.f_name = rdr["f_name"].ToString();
                            student.l_name = rdr["l_name"].ToString();
                            Payment payment = new Payment();
                            payment.payment_id = Convert.ToInt32(rdr["payment_id"]);
                            payment.amount = Convert.ToInt32(rdr["amount"]);
                            payment.startdate = Convert.ToDateTime(rdr["startdate"]);
                            payment.deadline = Convert.ToDateTime(rdr["deadline"]);
                            payment.n_installments = Convert.ToInt32(rdr["n_installments"]);
                            payment.fund_percentage = Convert.ToDecimal(rdr["fund_percentage"]);
                            payment.status = rdr["status"].ToString();
                            payment.semester = new Semester();
                            payment.semester.semester_code = rdr["semester_code"].ToString();
                            student.payments.Add(payment);
                        }
                        else
                        {
                            con.Close();
                            StudentPayments.Add(student);
                            return View(StudentPayments);
                        }

                        while (rdr.Read())
                        {
                            if (Convert.ToInt32(rdr["studentID"]) != student.student_id)
                            {
                                StudentPayments.Add(student);
                                student = new Student();
                                student.student_id = Convert.ToInt32(rdr["studentID"]);
                                student.f_name = rdr["f_name"].ToString();
                                student.l_name = rdr["l_name"].ToString();
                            }
                            Payment payment = new Payment();
                            payment.payment_id = Convert.ToInt32(rdr["payment_id"]);
                            payment.amount = Convert.ToInt32(rdr["amount"]);
                            payment.startdate = Convert.ToDateTime(rdr["startdate"]);
                            payment.deadline = Convert.ToDateTime(rdr["deadline"]);
                            payment.n_installments = Convert.ToInt32(rdr["n_installments"]);
                            payment.fund_percentage = Convert.ToDecimal(rdr["fund_percentage"]);
                            payment.status = rdr["status"].ToString();
                            payment.semester = new Semester();
                            payment.semester.semester_code = rdr["semester_code"].ToString();
                            student.payments.Add(payment);


                            StudentPayments.Add(student);
                        }
                        StudentPayments.Add(student);
                    }
                    con.Close();

                    return View(StudentPayments);
                }
            }
        }
        public void issueInstallments(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_AdminIssueInstallment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@payment_id", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@payment_id"].Value = form["payment_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public void updateStudentStatus(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedure_AdminUpdateStudentStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@student_id", SqlDbType.Int);

                    //set parameter values
                    cmd.Parameters["@student_id"].Value = form["student_id"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult AllActiveStudent()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM view_Students", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Student> students = new List<Student>();

                while (rdr.Read())
                {
                    Student student = new Student();
                    student.student_id = Convert.ToInt32(rdr["student_id"]);
                    student.f_name = rdr["f_name"].ToString();
                    student.l_name = rdr["l_name"].ToString();
                    student.password = rdr["password"].ToString();
                    if (rdr["gpa"] != DBNull.Value)
                    {
                        student.gpa = Convert.ToDecimal(rdr["gpa"]);
                    }
                    student.faculty = rdr["faculty"].ToString();
                    student.email = rdr["email"].ToString();
                    student.major = rdr["major"].ToString();
                    if (rdr["financial_status"] != DBNull.Value)
                    {
                        student.financial_status = Convert.ToBoolean(rdr["financial_status"]);
                    }
                    student.semester = Convert.ToInt32(rdr["semester"]);
                    if (rdr["acquired_hours"] != DBNull.Value)
                    {
                        student.acquired_hours = Convert.ToInt32(rdr["acquired_hours"]);
                    }
                    if (rdr["assigned_hours"] != DBNull.Value)
                    {
                        student.assigned_hours = Convert.ToInt32(rdr["assigned_hours"]);
                    }
                    student.advisor = new Advisor();
                    if (rdr["advisor_id"] != DBNull.Value)
                    {
                        student.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                    }
                    students.Add(student);
                }
                rdr.Close();
                con.Close();

                return View(students);
            }
        }
        public ActionResult gradPlansWithInitAdvisors()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Advisors_Graduation_Plan", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<GraduationPlan> AdvGradPlan = new List<GraduationPlan  >();
                using (rdr)
                {
                    GraduationPlan graduationPlan = new GraduationPlan();
                    if (rdr.Read())
                    {
                        graduationPlan.plan_id = Convert.ToInt16(rdr["plan_id"]);
                        graduationPlan.expected_grad_date = Convert.ToDateTime(rdr["expected_grad_date"]).ToShortDateString();

                        // Semester
                        GraduationPlanSemester semester = new GraduationPlanSemester();

                        graduationPlan.semesters = new List<GraduationPlanSemester>();
                        semester.semester_code = rdr["semester_code"].ToString();
                        semester.credit_hours = Convert.ToInt32(rdr["semester_credit_hours"]);
                        semester.advisor = new Advisor();
                        semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                        semester.advisor.name = rdr["advisor_name"].ToString();

                        graduationPlan.semesters.Add(semester);

                    }
                    else
                    {
                        rdr.Close();
                        con.Close();
                        graduationPlan.semesters = new List<GraduationPlanSemester>();
                        GraduationPlanSemester semester = new GraduationPlanSemester();
                        semester.advisor = new Advisor();
                        AdvGradPlan.Add(graduationPlan);
                        return View(AdvGradPlan);
                    }

                    while (rdr.Read())
                    {
                        if (Convert.ToInt32(rdr["plan_id"]) != graduationPlan.plan_id)
                        {
                            AdvGradPlan.Add(graduationPlan);
                            graduationPlan = new GraduationPlan();
                            graduationPlan.plan_id = Convert.ToInt16(rdr["plan_id"]);
                            graduationPlan.expected_grad_date = Convert.ToDateTime(rdr["expected_grad_date"]).ToShortDateString();

                            // Semester
                            GraduationPlanSemester semester = new GraduationPlanSemester();

                            graduationPlan.semesters = new List<GraduationPlanSemester>();
                            semester.semester_code = rdr["semester_code"].ToString();
                            semester.credit_hours = Convert.ToInt32(rdr["semester_credit_hours"]);
                            semester.advisor = new Advisor();
                            semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                            semester.advisor.name = rdr["advisor_name"].ToString();

                            graduationPlan.semesters.Add(semester);
                        }
                        else
                        {
                            // Semester
                            if (rdr["semester_code"].ToString() != graduationPlan.semesters[graduationPlan.semesters.Count - 1].semester_code)
                            {
                                GraduationPlanSemester semester = new GraduationPlanSemester();
                                semester.semester_code = rdr["semester_code"].ToString();
                                semester.credit_hours = Convert.ToInt32(rdr["semester_credit_hours"]);
                                semester.advisor = new Advisor();
                                semester.advisor.advisor_id = Convert.ToInt32(rdr["advisor_id"]);
                                semester.advisor.name = rdr["advisor_name"].ToString();
                                graduationPlan.semesters.Add(semester);
                            }
                            else
                            {
                                graduationPlan.semesters[graduationPlan.semesters.Count - 1].courses.Add(new Course(Convert.ToInt32(rdr["course_id"]), rdr["name"].ToString()));
                            }
                        }
                    }
                    AdvGradPlan.Add(graduationPlan);
                    rdr.Close();
                    con.Close();

                }
                return View(AdvGradPlan);
            }
        }

        public ActionResult AllStudentsTranscript()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Students_Courses_transcript", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Student_Instructor_Course_take> lstTranscript = new List<Student_Instructor_Course_take>();
                using (rdr)
                {
                    while (rdr.Read())
                    {
                        Student_Instructor_Course_take transcript = new Student_Instructor_Course_take();
                        transcript.student = new Student();
                        transcript.student.student_id = Convert.ToInt32(rdr["student_id"]);
                        transcript.student.f_name = rdr["f_name"].ToString();
                        transcript.student.l_name = rdr["l_name"].ToString();

                        transcript.course = new Course();
                        transcript.course.course_id = Convert.ToInt32(rdr["course_id"]);
                        transcript.course.name = rdr["name"].ToString();

                        transcript.exam_type = rdr["exam_type"].ToString();
                        transcript.grade = rdr["grade"].ToString();
                        transcript.semester_code = rdr["semester_code"].ToString();

                        lstTranscript.Add(transcript);
                    }
                    rdr.Close();
                    con.Close();

                }
                return View(lstTranscript);
            }
        }
        public ActionResult AllSemestersWithOfferedCourses()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Semster_offered_Courses", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Semester> SemCourses = new List<Semester>();
                Semester semester = new Semester();
                semester.courses = new List<Course>();
                if (rdr.Read())
                {
                    semester.semester_code = rdr["semester_code"].ToString();
                    Course course = new Course();
                    course.course_id = Convert.ToInt32(rdr["course_id"]);
                    course.name = rdr["name"].ToString();
                    semester.courses.Add(course);
                    while (rdr.Read())
                    {

                        if (rdr["semester_code"] != semester.semester_code)
                        {
                            SemCourses.Add(semester);
                            semester = new Semester();
                            semester.courses = new List<Course>();
                            semester.semester_code = rdr["semester_code"].ToString();
                        }
                        course = new Course();
                        course.course_id = Convert.ToInt32(rdr["course_id"]);
                        course.name = rdr["name"].ToString();
                        semester.courses.Add(course);
                    }
                    SemCourses.Add(semester);
                }
                else
                {
                    con.Close();
                    SemCourses.Add(semester);
                    return View(SemCourses);
                }
                
                rdr.Close();
                con.Close();

                return View(SemCourses);
            }
        }



    }
}

