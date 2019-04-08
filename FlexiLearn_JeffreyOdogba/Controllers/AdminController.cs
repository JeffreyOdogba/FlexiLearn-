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
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

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

        // POST: Admin/Edit/5
        // Approve is click to approve course for a user
        public ActionResult Approve(int id)
        {
            Enrolled enrolled = new Enrolled();
            enrolled = db.Enrolleds.Find(id);
            enrolled.Request = "Approved".ToString();
            db.Entry(enrolled).State = EntityState.Modified;
            db.SaveChanges();

                return RedirectToAction("Edit");           
        }

        // GET: Admin/Delete/5
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
