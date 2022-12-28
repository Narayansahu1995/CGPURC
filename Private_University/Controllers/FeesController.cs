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
	public class FeesController : Controller
	{
		// GET: Fees
		[SessionCheck]
		public ActionResult Index()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ViewBag.Session_ID = ARespo.Session_ID;
			ViewBag.Academic_Session = ARespo.Academic_Session;
			ModelState.Clear();
			List<Course_Fees> Cf = appClass.Get_university_course_List_ForFee(ARespo.University_ID, ARespo.Session_ID);
			return View(Cf);
		}

		[SessionCheck]
		public ActionResult Create(int Univ_Course_ID, int Univ_subCourse_ID)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ViewBag.Univ_Course_ID = Univ_Course_ID;
			ViewBag.course_Name = appClass.PopulateCourseNameOnly(Univ_Course_ID);
			ViewBag.Univ_subCourse_ID = Univ_subCourse_ID;
			ViewBag.subCourse_Name = appClass.PopulateSubCourseNameOnly(Univ_subCourse_ID);
			DataTable dt = appClass.PopulateSemYearNameOnly(Univ_Course_ID);
			TempData["FeeList"] = appClass.Get_university_course_semyear_list_froFee(ARespo.University_ID, ARespo.Session_ID, Univ_Course_ID, Univ_subCourse_ID, Convert.ToDecimal(dt.Rows[0]["Number_of_Year_Sem"].ToString()), dt.Rows[0]["Course_Mode"].ToString());
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult Create(FormCollection form)//(University_Fees_Insert ufi)
		{
			if (ModelState.IsValid)
			{
				AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			int UnivC_ID = Convert.ToInt32(form["Univ_Course_ID"].ToString());
			int UnivSC_ID = Convert.ToInt32(form["Univ_subCourse_ID"].ToString());
			int Univ_ID = Convert.ToInt32(form["University_ID"].ToString());
			int UnivE_ID = Convert.ToInt32(form["Entry_By"].ToString());
			//List<string> seatNumFromSession = form["Fees"] as List<string>;
			List<string> semYear = new List<string>(form["semYear"].Split(','));
			List<string> fee = new List<string>(form["Fees"].Split(','));

			
				AppClass appClass = new AppClass();
				bool Result = true;
				foreach (var Amt in fee)
				{
					if (Convert.ToDecimal(Amt) == 0)
						Result = false;
				}

				if (Result)
				{
					for (int i = 0; i < semYear.Count; i++)
					{
						University_Fees_Insert ufi = new University_Fees_Insert()
						{
							University_ID = ARespo.University_ID,
							Session_ID = ARespo.Session_ID,
							Univ_Course_ID = UnivC_ID,
							Univ_subCourse_ID = UnivSC_ID,
							Semester_Year = Convert.ToInt32(semYear[i]),
							Amount = Convert.ToDecimal(fee[i]),
							Entry_By = ARespo.Login_ID
						};

						bool Val = appClass.university_course_fees_Insert(ufi);
						if (!Val)
							Result = false;
					}


					if (Result)
					{
						TempData["Msg"] = "<script>alert('Fees Updated successfully.');</script>";
						return RedirectToAction("Index");

					}
					else
						TempData["Msg"] = "<script>alert('error while updating fees.');</script>";
				}
				else
					TempData["Msg"] = "<script>alert('Please Enter Amount that is more then 0.');</script>";
			}
			return RedirectToAction("Create", new { Univ_Course_ID = form["Univ_Course_ID"].ToString(), Univ_subCourse_ID = form["Univ_subCourse_ID"].ToString() });
		}

        public ActionResult FeesDetails ()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            return View(appClass.Get_Student_List(ARespo.University_ID));
        }
	}
}