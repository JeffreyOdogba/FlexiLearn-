using FlexiLearn_JeffreyOdogba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlexiLearn_JeffreyOdogba.Controllers
{
    /// <summary>
    /// Member Controller = Everything is Done Here :WIll Sectin everthing to units
    /// </summary>
    public class MembersController : Controller
    {
        /// <summary>
        /// Connection to database Entities 
        /// </summary>
        private FlexiLearnEntities db = new FlexiLearnEntities();

        /// <summary>
        /// Sign up Page
        /// Index is the default Route 
        /// </summary>
        /// <returns>Sign Up Page FORM</returns>
        // GET: Sign Up
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: Members/Create
        [HttpPost]
        public ActionResult Create(Member collection)
        {

            // TODO: Add insert logic here

            //Member member = new Member();

            //member.username = collection["username"];
            //member.email = collection["email"];
            //member.educationLevel = collection["educationLevel"];
            //member.phone = int.Parse(collection["phone"]);
            //member.dateofbirth = Convert.ToDateTime(collection["dateofbirth"]);
            if (ModelState.IsValid)
            {
                collection.dateOfRegistration = DateTime.UtcNow;
                db.Members.Add(collection);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View("Index");

        }

        /// <summary>
        /// This to display Login Page with all forms
        /// </summary>
        /// <returns>Login Page</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Get user input and validate to login Main Page
        /// </summary>
        /// <param name="member"> model for all properties posted</param>
        /// <returns>DashBoard if successfull Log in</returns>
        // Post: Members/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include ="username,password")]Member member)
        {
            if (member != null)
            {
                var user = db.Members.Where(u => u.username.Equals(member.username) && u.password.Equals(member.password)).SingleOrDefault();
                if (user != null)
                {
                    Session["MemberID"] = user.MemberId.ToString();
                    Session["Username"] = user.username.ToString();
                    return RedirectToAction("DashBoard");
                }
             
            }
            return View();
        }

        /// <summary>
        /// end session
        /// FormAuthentication Not used - Will be used in Upcoming Version
        /// </summary>
        /// <returns>Login Page</returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        
        /// <summary>
        /// ActionResult for Displaying All courses from database
        /// </summary>
        /// <returns>return all couses for booking</returns>
        // GET: Members/DashBoard         
        public ActionResult DashBoard()
        {
            if (Session["MemberID"] != null)
            {
                var course = db.Courses.ToList();
                return View(course);
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }

        //Accepts string from view - search database for course/subject/fee/duration and return result search to the view
        [HttpGet]
        public ActionResult DashBoard(string search)
        {
            var returnSearchedCourses = from c in db.Courses select c;

            if (!String.IsNullOrEmpty(search))
            {
                returnSearchedCourses = returnSearchedCourses.Where(s => s.title.Contains(search) || s.subject.Contains(search) || s.duration.ToString().Contains(search)
                                                                    || s.fee.ToString().Contains(search));
                return View(returnSearchedCourses);
            }           

            return View(returnSearchedCourses);
        }

        /// <summary>
        /// Courses Booked will be added to the db with user SessionID and CourseID with Awaititng Approval
        /// user cannot double book a course
        /// </summary>
        /// <param name="id"> To get which course is requested</param>
        /// <returns>return DashBoard View to keep booking</returns>
        //Get: To Book Enollment 
        public ActionResult Book(int id)
        {
                Enrolled en = new Enrolled();
                en.MemberId = int.Parse(Session["MemberID"].ToString());
                en.CourseId = int.Parse(this.RouteData.Values["id"].ToString());
                en.Request = "Awaiting Approval";

            if (db.Enrolleds.Any(e => e.MemberId == en.MemberId && e.CourseId == en.CourseId))
            {
                TempData["NotAdding"] = "You already added This Course";
            }
            else {

                db.Enrolleds.Add(en);
                db.SaveChanges();
                TempData["NotAdding"] = "Courses added to Request";
            }
               

            return RedirectToAction("DashBoard");
        }

        /// <summary>
        /// Gets courses of a users that placed a request from user
        /// </summary>
        /// <returns> return courses for each request book </returns>
        [HttpGet]
        public ActionResult CoursesRequest()
        {
            var memberSessionId = int.Parse(Session["MemberID"].ToString());
            var EnrolledCourses = from e in db.Enrolleds
                                  join c in db.Courses on e.CourseId equals c.CourseId
                                  join m in db.Members on e.MemberId equals m.MemberId
                                  where e.MemberId == memberSessionId
                                  orderby e.EnrollmentId
                                  select e;
          return View(EnrolledCourses);            
        }

        /// <summary>
        /// This Submit a request to get the id from each courses and delete from database
        /// </summary>
        /// <param name="id"> this gets the id of each course </param>
        /// <returns> this return back the view for CoursesRequest submitted</returns>
        //Get: Members/Withdraw/5
        public ActionResult Delete(int id)
        {
            Enrolled enrolled = db.Enrolleds.Find(id);
            if (enrolled == null)
            {
                return HttpNotFound();
            }
            db.Enrolleds.Remove(enrolled);
            db.SaveChanges();
            return RedirectToAction("CoursesRequest");
        }
    }
}
