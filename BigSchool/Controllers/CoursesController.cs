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
                Categories = _dbcontext.Categories.ToList(),
                Heading = "Add Courses"
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbcontext.Courses.Single(c => c.Id == id && c.LecturerId == userId);
            var ViewModel = new CourseViewModel
            {
                Categories = _dbcontext.Categories.ToList(),
                Date = courses.DateTime.ToString("dd/M/yyyy"),
                Time = courses.DateTime.ToString("HH:mm"),
                Category = courses.CategoryId,
                Place = courses.Place,
                Heading = "Edit Courses",
                Id = courses.Id
            };
            return View("Create", ViewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CourseViewModel ViewModel)
        {
            if(!ModelState.IsValid)
            {
                ViewModel.Categories= _dbcontext.Categories.ToList();
                return View("Create", ViewModel);
            }
            var userId = User.Identity.GetUserId();
            var course = _dbcontext.Courses.Single(c => c.Id == ViewModel.Id && c.LecturerId == userId);
            course.Place= ViewModel.Place;
            course.DateTime=ViewModel.GetDateTime();
            course.CategoryId = ViewModel.Category;
            _dbcontext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}