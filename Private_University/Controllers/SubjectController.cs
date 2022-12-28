using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.Data;

namespace Private_University.Controllers
{
	public class SubjectController : Controller
	{
		// GET: Subject
		[SessionCheck]
		public ActionResult Index()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			return View(appClass.Get_university_course_List(ARespo.University_ID));
		}

		[SessionCheck]
		public ActionResult Create(int Univ_Course_ID, int Univ_subCourse_ID, int Sem_year)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ViewBag.Univ_Course_ID = Univ_Course_ID;
			ViewBag.course_Name = appClass.PopulateCourseNameOnly(Univ_Course_ID);
			ViewBag.Univ_subCourse_ID = Univ_subCourse_ID;
			ViewBag.subCourse_Name = appClass.PopulateSubCourseNameOnly(Univ_subCourse_ID);
			ViewBag.Sem_year = Sem_year;
			DataTable dt = appClass.PopulateSemYearNameOnly(Univ_Course_ID);
			ViewBag.Sem_yearMode = dt.Rows[0]["Course_Mode"].ToString() + " - " + Sem_year;
			TempData["SubjectList"] = appClass.Get_university_course_semyear_list(ARespo.University_ID, Univ_Course_ID, Univ_subCourse_ID, Convert.ToDecimal(dt.Rows[0]["Number_of_Year_Sem"].ToString()), dt.Rows[0]["Course_Mode"].ToString());
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult Create(university_subject_insert usi)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();

				ViewBag.Univ_Course_ID = usi.Univ_Course_ID;
				ViewBag.course_Name = appClass.PopulateCourseNameOnly(usi.Univ_Course_ID);
				ViewBag.Univ_subCourse_ID = usi.Univ_subCourse_ID;
				string subCourse = appClass.PopulateSubCourseNameOnly(usi.Univ_subCourse_ID);
				ViewBag.subCourse_Name = subCourse;
				ViewBag.Sem_year = usi.Semester_Year;
				string sems = appClass.PopulateSemYearNameOnly(usi.Univ_Course_ID).Rows[0]["Course_Mode"].ToString() + " - " + usi.Semester_Year;
				ViewBag.Sem_yearMode = sems;

				bool Val = appClass.university_subject_Insert_subcourese(usi);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('New subject " + usi.Subject_Name + " as been added to " + subCourse + " [" + sems + "] successfully.');</script>";
					return RedirectToAction("Create", new { Univ_Course_ID = usi.Univ_Course_ID, Univ_subCourse_ID = usi.Univ_subCourse_ID, Sem_year = usi.Semester_Year });
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while adding new subject.');</script>";
				}
			}
			return View();
		}


		[SessionCheck]
		public ActionResult Delete(int id)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();
				AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
				university_subject_delete usd = new university_subject_delete()
				{
					Univ_Subject_ID = id,
					Deleted_By = ARespo.Login_ID
				};
				bool Val = appClass.university_subject_Delete(usd);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('Subject has been deleted successfully.');</script>";
					return RedirectToAction("Index");
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while deleting subject.');</script>";
					return RedirectToAction("Index");
				}
			}

			return View();
		}
	}
}