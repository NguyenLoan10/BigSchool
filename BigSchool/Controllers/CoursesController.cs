using BigSchool.Models;
using BigSchool.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                Categories = _dbcontext.Categories.ToList()
            };
            return View(viewModel);
        }
        private readonly ApplicationDbContext _dbcontext;
        public CoursesController()
        { 
            _dbcontext= new ApplicationDbContext();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _dbcontext.Categories.ToList();
                return View("Create", viewModel);
            }
            var course = new Course
            {
              LecturerId = User.Identity.GetUserId(),
              DateTime = viewModel.GetDateTime(),
              CategoryId = viewModel.Category,
              Place = viewModel.Place
            };
            _dbcontext.Courses.Add(course);
            _dbcontext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
       public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbcontext.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Course)
                .Include(l => l.Lecturer)
                .Include(l => l.Category)
                .ToList();
            var ViewModel = new CourseViewModel
            {
                UpcommingCourses = courses,
                ShowAction = User.Identity.IsAuthenticated
            };
            return View(ViewModel);
        }
        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbcontext.Courses
                .Where(c => c.LecturerId == userId && c.DateTime > DateTime.Now)
                .Include(l => l.Lecturer)
                .Include(l => l.Category)
                .ToList();
            return View(courses);
        }
    }
}