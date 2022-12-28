using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;

namespace Private_University.Controllers
{
	public class academic_sessionController : Controller
	{

		// GET: academic_session
		[SessionCheck]
		public ActionResult Index()
		{
			AppClass appClass = new AppClass();
			ModelState.Clear();
			return View(appClass.Get_master_academic_session());
		}

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		[SessionCheck]

		public ActionResult Create(int id)
		{
			if (ModelState.IsValid)
			{

				AppClass appClass = new AppClass();
				string session_name = appClass.Get_Next_master_academic_session();

				Master_Session_Insert ms = new Master_Session_Insert()
				{
					Session_Name = session_name,
					Entry_By = id
				};

				bool msses = appClass.Get_master_academic_session_check(session_name);
				if (!msses)
				{
					bool Val = appClass.master_academic_session_Insert(ms);
					if (Val)
					{
						TempData["Msg"] = "<script>alert('New session " + session_name + " as been created successfully.');</script>";
						return RedirectToAction("Index");
					}
					else
					{
						TempData["Msg"] = "<script>alert('error while creating session.');</script>";
						return RedirectToAction("Index");
					}
				}
				else
				{
					TempData["Msg"] = "<script>alert('Session already exists!!!');</script>";
					return RedirectToAction("Index");
				}
			}

			return View();
		}


		//[HttpDelete]
		//[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult Delete(int id)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();
				AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
				Master_Session_Delete ms = new Master_Session_Delete()
				{
					Session_ID = id,
					Deleted_By = ARespo.Login_ID
				};
				bool Val = appClass.master_academic_session_Delete(ms);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('Session has been deleted successfully.');</script>";
					return RedirectToAction("Index");
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while deleting session.');</script>";
					return RedirectToAction("Index");
				}
			}

			return View();
		}
	}
}