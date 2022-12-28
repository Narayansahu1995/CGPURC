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
	public class FeesCollectController : Controller
	{
		// GET: FeesCollect
		[SessionCheck]
		public ActionResult Index()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			List<StudentDetails> stuDetails = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
			return View(stuDetails);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult Index(FormCollection form)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			if (ModelState.IsValid)
			{
				FeesCollect ufi = new FeesCollect()
				{
					University_ID = ARespo.University_ID,
					Session_ID = ARespo.Session_ID,
					Student_ID = Convert.ToInt32(form["txt_Student_Id"].ToString()),
					Amount_Credit = Convert.ToDecimal(form["txt_Amount_Credit"].ToString()),
					Bill_Number = form["txt_Bill_Number"].ToString(),
					Txn_Date = Convert.ToDateTime(form["txt_Txn_Date"].ToString()),
					Narration = form["txt_Narration"].ToString(),
					Entry_By = ARespo.Login_ID
				};

				bool Val = appClass.university_transaction_Credit_Insert(ufi);
				if (Val)
					TempData["Msg"] = "<script>alert('Fees Collecton Updated successfully.');</script>";
				else
					TempData["Msg"] = "<script>alert('error while updating fees collection.');</script>";
			}
			List<StudentDetails> stuDetails = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
			return View(stuDetails);
		}

        [HttpGet]
        [SessionCheck]
        public ActionResult StudentFeesList()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_Stu_Fees_List(ARespo.University_ID));
        }
	}
}