using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.IO;
using CaptchaMvc.HtmlHelpers;
using System.Threading;
using System.Globalization;


namespace Private_University.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";
			AppClass appClass = new AppClass();
			//AppClass appClass = new AppClass();
			System.DateTime lastdate = System.DateTime.Now.AddMonths(-1);
			bool Check = appClass.ayoge_transctions_Check(lastdate.Month, lastdate.Year);
			if (!Check)
			{
				appClass.FeesCollectionSummery(lastdate.Month, lastdate.Year);
			}
			
			//if (LanguageAbbrevation != null)
			//{
			//	Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguageAbbrevation);
			//	Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageAbbrevation);
			//}
			//HttpCookie cookie = new HttpCookie("Home");
			//cookie.Value = LanguageAbbrevation;
			//TempData["lang"] = cookie.Value;
			//Response.Cookies.Add(cookie);
			return View(appClass.GetNewsShowHomeMarquee());
		}

		public ActionResult AboutUs()
		{
			ViewBag.Title = "About Us";
			return View();
		}

		public ActionResult OfficeBearers()
		{
			ViewBag.Title = "Office Bearers";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			//if (cookie.Value == "en")
			//         {
			//             return View(appClass.GetMasterOfficeBearerShowHome());
			//         }
			//         else if (cookie.Value == "Hi")
			//         {
			//             return View();
			//         }

			return View(appClass.GetMasterOfficeBearerShowHome());
		}

		public ActionResult Univsersities()
		{
			ViewBag.Title = "Universities";
			//ViewBag.Title = "List of Universities";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			return View(appClass.UniversityFrontShow());
			//return View();
		}


		public ActionResult UniversityCourseList(int id)
		{
			ViewBag.Title = "Univsersity Course List";
			// return View();
			AppClass appClass = new AppClass();

			ModelState.Clear();
			return View(appClass.Get_university_course(id));
		}

		public ActionResult UniversityProfile(int id)
		{
			ViewBag.Title = "Univsersity Profile";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			//List<UniversityDetailsWithStudents> uniprofile = appClass.GetUniversityProfilewithStudentList(id);
			List<UniversityDetailsWithOfficeBearer> uniprofile = appClass.UniversityDetailsWithOfficeBearer(id);
			return View(uniprofile);
		}
		//public ActionResult OurValues()
		//{
		//	ViewBag.Title = "Our Values";
		//}

		public ActionResult UniversityFees(int id)
		{
			ViewBag.Title = "Univsersity Fees";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			//List<Course_Fees> Cf = appClass.Get_university_course_List_ForFee_Home(id);
			List<UniversityDetailsWithFeesDetails> Cf = appClass.UniversityDetailsWithFeesDetails(id);
			return View(Cf);
		}

		public ActionResult OurValues()
		{
			ViewBag.Title = "Our Values";
			return View();
		}

		public ActionResult Objective()
		{
			ViewBag.Title = "Objective";

			return View();
		}

		public ActionResult OrganizationStructure()
		{
			ViewBag.Title = "Organization Structure";

			return View();
		}

		public ActionResult WorldRecord()
		{
			ViewBag.Title = "World Record";

			return View();
		}


		public ActionResult ContactUs()
		{
			ViewBag.Title = "Contact Us";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			CommissionAddressUpdate com = appClass.GetCommissionAddress();
			ViewBag.Addressss = com.Addressss;
			ViewBag.Address_Hi = com.Address_Hi;
			ViewBag.Contact_No = com.Contact_No;
			ViewBag.Email_ID = com.Email_ID;
			return View();
		}

		public ActionResult Login()
		{
			AppClass appClass = new AppClass();
			ViewBag.Title = "Login";
			ViewBag.SessionList = appClass.Get_master_academic_session();
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Authentication Auth)
        {
            AppClass appClass = new AppClass();
            ViewBag.SessionList = appClass.Get_master_academic_session();
           
            if (ModelState.IsValid)
            {
                AuthenticationResponse ARespo = appClass.login_authentication(Auth);
                if (ARespo.Success == true)
                {
                    Session["AuthResponse"] = ARespo;
                    if (ARespo.PwdRstDateTime.ToString() == "")
                    {
                     return RedirectToAction("Index", "ChangePassword");
                    }
                    else
                    {                    
                    return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    ViewBag.Title = "Login";
                    ViewBag.Message = "Login Failed : " + ARespo.Message;
                    // Failed message from Function.
                }
            }
            return View();
        }
        public ActionResult RTI()
		{
			ViewBag.Title = "RTI";

			return View();
		}

		public ActionResult ReportAnual()
		{
			ViewBag.Title = "Anual Report";

			return View();
		}

        
		public ActionResult ForgetPassword()
		{
			ViewBag.Title = "Forget Password";

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgetPassword(ForgotPassword fp)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();
				ForgotPassword_Response FP_Respo = appClass.Forgot_Password_authentication(fp);
				if (FP_Respo.Response == true)
				{
					TempData["Msg"] = "<script>alert('" + FP_Respo.Message + "');</script>";
					return RedirectToAction("Index", "Dashboard");
				}
				else
				{
					ViewBag.Message = "Failed : " + FP_Respo.Message;
				}
			}
			return View();
		}

		public ActionResult Gallery()
		{
			ViewBag.Title = "Gallery";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			return View(appClass.GetPhotoGalleryShowHome());

		}

		public ActionResult News()
		{
			ViewBag.Title = "News";
			AppClass appClass = new AppClass();
			ModelState.Clear();
			return View(appClass.GetNewsShowHome());
		}

		public ActionResult Feedback()
		{
			ViewBag.Title = "Feedback";
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Feedback(Feedback fb)
		{
			AppClass appClass = new AppClass();
			if (ModelState.IsValid && (this.IsCaptchaValid("Captcha is not valid")))
			{
				string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
				bool Val = appClass.Feedback_Insert(fb);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('Feedback Submitted successfully.');</script>";
					return RedirectToAction("Feedback");
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while Submitting Feedback');</script>";
					ViewBag.ErrMessage = "Error: captcha is not valid.";
					return RedirectToAction("Feedback");
				}
			}
			return View();
		}



		public FileResult RulesRegulations()
		{
			ViewBag.Title = "Rules Regulations";
			string ReportURL = Server.MapPath("~/Content/pdf/RulesRegulations.pdf");
			byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
			return File(FileBytes, "application/pdf");
		}

		public FileResult Notifications()
		{
			ViewBag.Title = "Notifications";
			string ReportURL = Server.MapPath("~/Content/pdf/Notifications.pdf");
			byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
			return File(FileBytes, "application/pdf");
		}

		public FileResult AntiRagging()
		{
			ViewBag.Title = "Anti-Ragging";
			string ReportURL = Server.MapPath("~/Content/pdf/Ragging.pdf");
			byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
			return File(FileBytes, "application/pdf");
		}

		public FileResult AntiHarassment()
		{
			ViewBag.Title = "Anti-Harassment";
			string ReportURL = Server.MapPath("~/Content/pdf/Women_Harassment.pdf");
			byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
			return File(FileBytes, "application/pdf");
		}


		public ActionResult Logout() {

			Session.Clear();
			Session.RemoveAll();
			Session.Abandon();
			return RedirectToAction("Index");
		}

	}
}