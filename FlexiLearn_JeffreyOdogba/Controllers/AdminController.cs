using FlexiLearn_JeffreyOdogba.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexiLearn_JeffreyOdogba.Controllers
{
    public class AdminController : Controller
    {
        private FlexiLearnEntities db = new FlexiLearnEntities();
        
        /// <summary>
        /// View For Creating Courses
        /// </summary>
        /// <returns>Sign Up Page</returns>
        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// ActionResult For taking inputs from Admin to store Courses in the DataBase
        /// </summary>
        /// <param name="collection">param passed from view to Controller</param>
        /// <returns>Create View</returns>
        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(Course collection)
        {
            try
            {             

                if (ModelState.IsValid)
                {
                    db.Courses.Add(collection);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// This view All Courses Booked by users but not Approved Yet
        /// </summary>
        /// <returns>All COurses Booked</returns>
        // GET: Admin/Edit/5
        [HttpGet]
        public ActionResult Edit()
        {
            var EnrolledCourses = from e in db.Enrolleds
                                  join c in db.Courses on e.CourseId equals c.CourseId
                                  join m in db.Members on e.MemberId equals m.MemberId
                                  orderby e.EnrollmentId
                                  select e;
            return View(EnrolledCourses);
        }

        /// <summary>
        /// Approve is click to approve course for a user..
        /// Once Approved User Cannot Withdraw from Course
        /// </summary>
        /// <param name="id">get the id of the Course to be approved</param>
        /// <returns>return Edit View</returns>
        // POST: Admin/Edit/5
        public ActionResult Approve(int id)
        {
            Enrolled enrolled = new Enrolled();
            enrolled = db.Enrolleds.Find(id);
            enrolled.Request = "Approved".ToString();
            db.Entry(enrolled).State = EntityState.Modified;
            db.SaveChanges();

                return RedirectToAction("Edit");           
        }

        /// <summary>
        /// This Rejected Reject a Course  
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Return Edit View</returns>
        // GET: Admin/Rejected/5
        public ActionResult Rejected(int id)
        {
            Enrolled enrolled = new Enrolled();
            enrolled = db.Enrolleds.Find(id);
            enrolled.Request = "Rejected".ToString();
            db.Entry(enrolled).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit");
        }

    }
}
