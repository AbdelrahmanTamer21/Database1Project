﻿using DatabaseProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Drawing;
using System.Diagnostics;
using System.Web.Routing;
using System.Web;

namespace DatabaseProject.Controllers
{
    public class StudentController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["Type"] != "Student" && actionName != "Login" && actionName != "loginStudent")
            {
                if (actionName != "Register" && actionName != "registerStudent")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                                { "Controller", "Student" },
                                { "Action", "Login" }
                                    });
                }
            }
        }
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
        ///////////// PART 1 /////////////
        /// A
        public ActionResult registerStudent(FormCollection form)
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
                    return RedirectToAction("Index");
                }
            }
        }

        /// B
        public ActionResult loginStudent(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT dbo.FN_StudentLogin(@Student_id,@password) AS Success", con);
                cmd.CommandType = CommandType.Text;
                using (cmd)
                {
                    try
                    {
                        int id = Convert.ToInt16(form["student_id"]);
                    }
                    catch (Exception ex)
                    {
                        TempData["LoginError"] = "ID or Password are wrong";
                        return RedirectToAction("Login");
                    }
                    //set up the parameters
                    cmd.Parameters.AddWithValue("@Student_id", form["student_id"]);
                    cmd.Parameters.AddWithValue("@password", form["password"]);

                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();


                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        bool success = Convert.ToBoolean(rdr["Success"]);
                        if (!success)
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

                            Session["userID"] = form["student_id"];
                            Session["type"] = "Student";

                            TempData["Alert"] = "Login successful";
                            return RedirectToAction("Index");
                        }
                    }
                    return RedirectToAction("Login");
                }
            }

        }

        /// C
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
                    //cmd.Parameters.Add("@StudentID", SqlDbType.VarChar, 40);
                    //cmd.Parameters.Add("@mobile_number", SqlDbType.VarChar, 40);


                    //set parameter values
                    //cmd.Parameters["@StudentID"].Value = form["student_id"];
                    //cmd.Parameters["@mobile_number"].Value = form["phone_number"];
                    cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);
                    cmd.Parameters.AddWithValue("@mobile_number", form["phone_number"]);


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult addStudentPhoneForm()
        {
            return View();
        }
        /// D
        public ActionResult optionalCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_ViewOptionalCourse", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);
                cmd.Parameters.AddWithValue("@current_semester_code", form["semester_code"]);

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

        public ActionResult optionalCoursesForm()
        {
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }

        /// E
        public ActionResult availableCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_SemsterAvailableCourses(@semstercode)", con);
                cmd.CommandType = CommandType.Text;

                //cmd.Parameters.Add("@current_semester_code", SqlDbType.Int);
                //cmd.Parameters.Add("@semstercode", SqlDbType.VarChar,40);
                //cmd.Parameters["@semstercode"].Value = form["semester_code"];
                cmd.Parameters.AddWithValue("@semstercode", form["semester_code"]);
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
        public ActionResult availableCoursesForm()
        {
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }
        /// F
        public ActionResult requiredCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_ViewRequiredCourses", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.Add("@current_semester_code", SqlDbType.Int);
                //cmd.Parameters.Add("@StudentID", SqlDbType.Int);
                //cmd.Parameters["@current_semester_code"].Value = form["current_semester_code"];
                //cmd.Parameters["@StudentID"].Value = form["student_id"];
                cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);
                cmd.Parameters.AddWithValue("@current_semester_code", form["semester_code"]);

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
        public ActionResult requiredCoursesForm()
        {
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }
        /// G
        public ActionResult missingCourses(FormCollection form)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con) 
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_ViewMS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.Add("@StudentID", SqlDbType.Int);
                //cmd.Parameters["@StudentID"].Value = form["student_id"];
                cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);


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
        public ActionResult missingCoursesForm()
        {
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }
        /// H
        public void sendCourseRequest(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentSendingCourseRequest", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    cmd.Parameters.Add("@courseID", SqlDbType.VarChar, 40);
                    //cmd.Parameters.Add("@StudentID", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@comment", SqlDbType.VarChar, 40);

                    cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);

                    //set parameter values
                    cmd.Parameters["@courseID"].Value = form["course_id"];
                    //cmd.Parameters["@StudentID"].Value = form["student_id"];
                    cmd.Parameters["@type"].Value = form["type"];
                    cmd.Parameters["@comment"].Value = form["comment"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult sendCourseRequestForm()
        {
            return View();
        }
        /// I
        public void sendCreditHourRequest(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentSendingCHRequest", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (cmd)
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //set up the parameters
                    //cmd.Parameters.Add("@StudentID", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@credit_hours", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 40);
                    cmd.Parameters.Add("@comment", SqlDbType.VarChar, 40);

                    cmd.Parameters.AddWithValue("@StudentID", Session["userID"]);

                    //set parameter values
                    //cmd.Parameters["@StudentID"].Value = form["student_id"];
                    cmd.Parameters["@credit_hours"].Value = form["credit_hours"];
                    cmd.Parameters["@type"].Value = form["type"];
                    cmd.Parameters["@comment"].Value = form["comment"];


                    //open connection and execute stored procedure
                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
        }
        public ActionResult sendCreditHourRequestForm()
        {
            return View();
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
                cmd.Parameters["@student_id"].Value = Convert.ToInt32(Session["userID"]);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                GraduationPlan graduationPlan = new GraduationPlan();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    graduationPlan.plan_id = Convert.ToInt32(rdr["plan_id"]);
                    graduationPlan.expected_grad_date = Convert.ToDateTime(rdr["expected_grad_date"]).ToShortDateString();
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
                    return View(graduationPlan);
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
        public ActionResult UpcomingInstallments()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Installment INNER JOIN Payment ON Payment.payment_id = Installment.payment_id AND Payment.student_id = @student_ID AND Installment.status='notpaid' WHERE Installment.deadline > CURRENT_TIMESTAMP ORDER BY Installment.deadline ASC", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@student_id", SqlDbType.Int);
                cmd.Parameters["@student_id"].Value = Convert.ToInt32(Session["userID"]);

                List<Installment> installments = new List<Installment>();

                con.Open();
                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    installments.Add(new Installment()
                    {
                        payment_id = Convert.ToInt32(rdr["payment_id"]),
                        startdate = rdr["startdate"].ToString(),
                        deadline = rdr["deadline"].ToString(),
                        amount = Convert.ToInt32(rdr["amount"]),
                        status = rdr["status"].ToString()
                    });
                }

                con.Close();
                return View(installments);
            }
        }

        // C
        // View all courses along with their exams details.
        public ActionResult AllCoursesExams()
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

        // D
        // Register for first makeup exam. You should show the output response.
        public ActionResult RegisterFirstMakeupForm()
        {
            ViewBag.Courses = new SelectList(AdvisorController.getCourseIDs(), "Value", "Text");
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }

        public ActionResult RegisterFirstMakeup(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {

                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentRegisterFirstMakeup", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // @StudentID int, @courseID int, @studentCurr_sem varchar(40), @result bit OUTPUT
                // set up the parameters
                cmd.Parameters.Add("@StudentID", SqlDbType.Int);
                cmd.Parameters.Add("@courseID", SqlDbType.Int);
                cmd.Parameters.Add("@studentCurr_sem", SqlDbType.VarChar, 40);

                // state output variable
                cmd.Parameters.Add("@result", SqlDbType.Bit).Direction = ParameterDirection.Output;

                // set parameter values
                cmd.Parameters["@StudentID"].Value = Convert.ToInt32(Session["userID"]);
                cmd.Parameters["@courseID"].Value = Convert.ToInt32(form["course_id"]);
                cmd.Parameters["@studentCurr_sem"].Value = form["studentCurr_sem"];

                // open connection and execute stored procedure
                con.Open();
                cmd.ExecuteNonQuery();

                if (Convert.ToBoolean(cmd.Parameters["@result"].Value))
                {
                    TempData["Alert"] = "You have successfully registered for the first makeup exam.";
                    con.Close();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Alert"] = "You couldn't register for the first makeup exam.";
                    con.Close();
                    return RedirectToAction("RegisterFirstMakeupForm");
                }
            }
        }

        // E
        // Register for second makeup exam. You should show the output response.
        public ActionResult RegisterSecondMakeupForm()
        {
            ViewBag.Courses = new SelectList(AdvisorController.getCourseIDs(), "Value", "Text");
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }

        public ActionResult RegisterSecondMakeup(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {

                SqlCommand cmd = new SqlCommand("dbo.Procedures_StudentRegisterFirstMakeup", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // @StudentID int, @courseID int, @studentCurr_sem varchar(40), @result bit OUTPUT
                // set up the parameters
                cmd.Parameters.Add("@StudentID", SqlDbType.Int);
                cmd.Parameters.Add("@courseID", SqlDbType.Int);
                cmd.Parameters.Add("@studentCurr_sem", SqlDbType.VarChar, 40);

                // state output variable
                cmd.Parameters.Add("@result", SqlDbType.Bit).Direction = ParameterDirection.Output;

                // set parameter values
                cmd.Parameters["@StudentID"].Value = Convert.ToInt32(Session["userID"]);
                cmd.Parameters["@courseID"].Value = Convert.ToInt32(form["course_id"]);
                cmd.Parameters["@studentCurr_sem"].Value = form["studentCurr_sem"];

                // open connection and execute stored procedure
                con.Open();
                cmd.ExecuteNonQuery();

                if (Convert.ToBoolean(cmd.Parameters["@result"].Value))
                {
                    TempData["Alert"] = "You have successfully registered for the first makeup exam.";
                    con.Close();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Alert"] = "You couldn't register for the second makeup exam.";
                    con.Close();
                    return RedirectToAction("RegisterSecondMakeupForm");
                }
            }
        }
        // F
        // View all courses along with their corresponding slots details and instructors.
        public ActionResult AllCoursesSlots()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Courses_Slots_Instructor", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Course_Slot_Instructor> courses_slot_instructor = new List<Course_Slot_Instructor>();

                while (rdr.Read())
                {
                    Course course = new Course();
                    course.course_id = Convert.ToInt32(rdr["course_id"]);
                    course.name = rdr["Course"].ToString();

                    Slot slot = new Slot();
                    slot.slot_id = Convert.ToInt32(rdr["slot_id"]);
                    slot.day = rdr["day"].ToString();
                    slot.time = rdr["time"].ToString();
                    slot.location = rdr["location"].ToString();

                    Instructor instructor = new Instructor();
                    instructor.instructor_id = Convert.ToInt32(rdr["instructor_id"]);
                    instructor.name = rdr["Instructor"].ToString();

                    Course_Slot_Instructor course_Slot = new Course_Slot_Instructor();
                    course_Slot.course = course;
                    course_Slot.slot = slot;
                    course_Slot.instructor = instructor;

                    courses_slot_instructor.Add(course_Slot);
                }
                rdr.Close();
                con.Close();

                return View(courses_slot_instructor);
            }
        }

        // G
        // View the slots of a certain course that is taught by a certain instructor.
        public ActionResult ViewSlotsForm()
        {
            ViewBag.Courses = new SelectList(AdvisorController.getCourseIDs(), "Value", "Text");
            ViewBag.Instructors = new SelectList(AdvisorController.getInstructorIDs(), "Value", "Text");
            return View();
        }
        public ActionResult ViewSlots(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.FN_StudentViewSlot(@course_id, @instructor_id)", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@course_id", SqlDbType.Int);
                cmd.Parameters.Add("@instructor_id", SqlDbType.Int);
                cmd.Parameters["@course_id"].Value = Convert.ToInt32(form["course_id"]);
                cmd.Parameters["@instructor_id"].Value = Convert.ToInt32(form["instructor_id"]);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Course_Slot_Instructor> records = new List<Course_Slot_Instructor>();

                while (rdr.Read())
                {
                    Course course = new Course();
                    course.course_id = Convert.ToInt32(rdr["course_id"]);
                    course.name = rdr["Course"].ToString();

                    Slot slot = new Slot();
                    slot.slot_id = Convert.ToInt32(rdr["slot_id"]);
                    slot.day = rdr["day"].ToString();
                    slot.time = rdr["time"].ToString();
                    slot.location = rdr["location"].ToString();

                    Instructor instructor = new Instructor();
                    instructor.instructor_id = Convert.ToInt32(rdr["instructor_id"]);
                    instructor.name = rdr["Instructor"].ToString();

                    Course_Slot_Instructor course_Slot = new Course_Slot_Instructor();
                    course_Slot.course = course;
                    course_Slot.slot = slot;
                    course_Slot.instructor = instructor;

                    records.Add(course_Slot);
                }
                rdr.Close();
                con.Close();

                return View(records);
            }
        }

        // H
        // Choose the instructor for a certain course
        public ActionResult ChooseInstructorForm()
        {
            ViewBag.Courses = new SelectList(AdvisorController.getCourseIDs(), "Value", "Text");
            ViewBag.Instructors = new SelectList(AdvisorController.getInstructorIDs(), "Value", "Text");
            ViewBag.Semesters = new SelectList(AdvisorController.getSemesters(), "Value", "Text");
            return View();
        }
        public ActionResult ChooseInstructor(FormCollection form)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("dbo.Procedures_Chooseinstructor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // @StudentID int, @courseID int, @instructorID int, @result bit OUTPUT
                // set up the parameters
                cmd.Parameters.Add("@StudentID", SqlDbType.Int);
                cmd.Parameters.Add("@courseID", SqlDbType.Int);
                cmd.Parameters.Add("@instructorID", SqlDbType.Int);
                cmd.Parameters.Add("@current_semester_code", SqlDbType.VarChar, 40);


                // set parameter values
                cmd.Parameters["@StudentID"].Value = Convert.ToInt32(Session["userID"]);
                cmd.Parameters["@courseID"].Value = Convert.ToInt32(form["course_id"]);
                cmd.Parameters["@instructorID"].Value = Convert.ToInt32(form["instructor_id"]);
                cmd.Parameters["@current_semester_code"].Value = form["current_semester_code"];

                // open connection and execute stored procedure
                con.Open();
                cmd.ExecuteNonQuery();

                TempData["Alert"] = "You have successfully chosen the instructor for the course.";

                con.Close();

                return RedirectToAction("Index");
            }
        }

        // I
        // View all details of all courses with their prerequisites.
        public ActionResult AllCoursesPrerequisites()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            using (con)
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM view_Course_prerequisites", con);
                cmd.CommandType = CommandType.Text;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<Course_Prerequisite> courses = new List<Course_Prerequisite>();

                while (rdr.Read())
                {
                    Course course = new Course();
                    course.course_id = Convert.ToInt32(rdr["course_id"]);
                    course.name = rdr["name"].ToString();
                    course.major = rdr["major"].ToString();
                    course.is_offered = Convert.ToBoolean(rdr["is_offered"]);
                    course.credit_hours = Convert.ToInt32(rdr["credit_hours"]);
                    course.semester = Convert.ToInt32(rdr["semester"]);

                    Course pre = new Course();
                    pre.course_id = Convert.ToInt32(rdr["preRequsite_course_id"]);
                    pre.name = rdr["preRequsite_course_name"].ToString();

                    Course_Prerequisite course_Prerequisite = new Course_Prerequisite();
                    course_Prerequisite.course = course;
                    course_Prerequisite.prerequisiteCourse = pre;

                    courses.Add(course_Prerequisite);
                }
                rdr.Close();
                con.Close();

                return View(courses);
            }
        }
    }
}