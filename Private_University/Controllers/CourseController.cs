using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;

namespace Private_University.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        [SessionCheck]
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_university_course(ARespo.University_ID));
        }

        [SessionCheck]
        public ActionResult Create()
        {
            AppClass appClass = new AppClass();
            ViewBag.CourseMode = appClass.PopulateCourseMode();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]

        public ActionResult Create(university_course_insert uci)
        {
            AppClass appClass = new AppClass();
            ViewBag.CourseMode = appClass.PopulateCourseMode();
            if (ModelState.IsValid)
            {

                bool Val = appClass.university_course_Insert(uci);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('New Course " + uci.Univ_Course_Name + " as been created successfully.');</script>";
                    return RedirectToAction("Index");
                }
                else
                { TempData["Msg"] = "<script>alert('error while creating Course.');</script>"; }

            }
            // return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [SessionCheck]
        public ActionResult Branch(int id)
        {
            AppClass appClass = new AppClass();
            ViewBag.courseName_Get = appClass.PopulateCourseNameOnly(id);
            ViewBag.courseID_Get = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult Branch(university_sub_course_Insert usci)
        {
            AppClass appClass = new AppClass();

            ViewBag.courseName_Get = appClass.PopulateCourseNameOnly(usci.Univ_Course_ID);
            ViewBag.courseID_Get = usci.Univ_Course_ID;

            if (ModelState.IsValid)
            {
                bool Val = appClass.university_sub_course_Insert(usci);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('New Course / Sub Course / Branch " + usci.Univ_subCourse_Name + " as been created successfully.');</script>";
                    return RedirectToAction("Index");
                }
                else
                { TempData["Msg"] = "<script>alert('error while creating Course / Sub Course / Branch.');</script>"; }
            }
            // return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [SessionCheck]
        public ActionResult Edit(int bid, int cid)
        {
            ModelState.Clear();
            AppClass appClass = new AppClass();           
            University_Course_details UCD = new University_Course_details();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            UCD = appClass.CoursesDetailsForDelete(bid,cid, ARespo.University_ID);
            ViewBag.ModeList = appClass.CourseMode();
            return View(UCD);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult Edit(University_Course_details UCD)
        {
            AppClass appClass = new AppClass();
            ViewBag.ModeList = appClass.CourseMode();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            var value = Request.Form["ModifyType"];
            
            if (ModelState.IsValid)
            {
                if(Convert.ToInt16(value) == 1)
                {
                    bool Val = appClass.SubCourseUpdate(UCD);
                    if (Val)
                    {
                        TempData["FinalMsg"] = "<script>alert('Branch / specialization Updated Successfully. !!');</script>";
                    }
                    else
                    {
                        TempData["FinalMsg"] = "<script>alert('Error in Updating Branch / specialization  !!');</script>";
                    }
                }
                else {                
                bool Val = appClass.CourseUpdate(UCD);
                    if (Val)
                    {
                        TempData["FinalMsg"] = "<script>alert('Course Updated Successfully. !!');</script>";
                    }
                    else
                    {
                        TempData["FinalMsg"] = "<script>alert('Error in Updating Course !!');</script>";
                    }
                }

            }
            return RedirectToAction("CourseRpt", new { @Type = 1 });
        }

        [SessionCheck]
        public ActionResult Delete(int bid, int cid)
        {
            ModelState.Clear();
            AppClass appClass = new AppClass();
            University_Course_details UCD = new University_Course_details();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            UCD = appClass.CoursesDetailsForDelete(bid,cid, ARespo.University_ID);           

            return View(UCD);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult Delete(University_Course_details USD)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            if (ModelState.IsValid)
            {
                if(USD.Univ_subCourse_ID == 0)
                {
                    bool Val = appClass.CourseDelete(USD.Univ_Course_ID);
                    if(Val)
                    {
                        TempData["FinalMsg"] = "<script>alert('Course deleted Successfully. !!');</script>";
                    }
                    else
                    {
                        TempData["FinalMsg"] = "<script>alert('Error in deleting Course. !!');</script>";
                    }
                }
                else
                { 
                    bool Val = appClass.SubCourseDelete(USD.Univ_subCourse_ID);
                    if (Val)
                    {
                        TempData["FinalMsg"] = "<script>alert('Course Branch deleted Successfully. !!');</script>";
                    }
                    else
                    {
                        TempData["FinalMsg"] = "<script>alert('Error in deleting Course Branch. !!');</script>";
                    }
                }
            }
            return RedirectToAction("CourseRpt", new { @Type = 1 });
        }

      
        [SessionCheck]
        public ActionResult CourseRpt(int type)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_UNV_Courses(ARespo.University_ID));
            
        }
    }
}