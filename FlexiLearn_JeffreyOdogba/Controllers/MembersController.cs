using FlexiLearn_JeffreyOdogba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlexiLearn_JeffreyOdogba.Controllers
{
    public class MembersController : Controller
    {
        private FlexiLearnEntities db = new FlexiLearnEntities();
        // GET: Sign Up
        public ActionResult Index()
        {
            return View();
        }

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

        public ActionResult Login()
        {
            return View();
        }

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

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        

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

        //Post: To Book Enollment 
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

        //POST: Members/Withdraw/5
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
