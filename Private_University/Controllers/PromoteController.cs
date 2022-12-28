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
	public class PromoteController : Controller
	{
		// GET: Promote
		public ActionResult Index()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			List<StudentDetails> stuDetails = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
			return View(stuDetails);
		}

		[SessionCheck]
		public ActionResult Fail(int id)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];

			///		First Get The values of Current Student {Mode of Year /sem, Current Duration,Max Duration of Course, Current Session }//
			///		Check The New Session Has Created or Not {If New Session has created then it is allow to Promote}.
			///		check even or odd if it is odd then it can allow for semester if new session not available. other wise it's not allow without new session for even promotion or yearly promotion.
			///		if all conditon mentioned above mentioned has suits then do the promormation.

			student_Details_promote sdp = appClass.Get_Student_Class_Detials_for_Promotion(id, ARespo.University_ID);
			int Ac_Session = appClass.Check_and_Get_Next_session_Generated(ARespo.Session_ID);

			student_promote sp = new student_promote();
			sp.Student_Id = sdp.Student_Id;

			if (sdp.Semester_Year == sdp.Max_Semester_Year)
				sp.Is_Passout = 1;
			else
				sp.Is_Passout = 0;
			if (sdp.Course_Mode_ID == 1)//Semester
				if (sdp.Semester_Year % 2 == 0)
				{
					if (Ac_Session > 0)
					{
						sp.Session_ID = Ac_Session;
						sp.Semester_Year = (sdp.Semester_Year + 1);
					}
					else
					{
						TempData["Msg"] = "<script>alert('New Academic Session has not been generated yet, please request to create New Academic Session first.');</script>";
						List<StudentDetails> stuDtls = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
						return View(stuDtls);
					}
				}
				else
				{
					sp.Session_ID = sdp.Session_ID;
					sp.Semester_Year = sdp.Semester_Year;
				}
			else if (sdp.Course_Mode_ID == 2)//Year
			{
				if (Ac_Session > 0)
				{
					sp.Session_ID = Ac_Session;
					sp.Semester_Year = (sdp.Semester_Year + 1);
				}
				else
				{
					TempData["Msg"] = "<script>alert('New Academic Session has not been generated yet, please request to create New Academic Session first.');</script>";
					List<StudentDetails> stuDtls = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
					return View(stuDtls);
				}
			}

			sp.Result = 0;
			sp.Promoted_By = ARespo.Login_ID;

			bool val = appClass.Update_student_Class_sem(sp);
			if (val)
			{
				bool _val = appClass.insert_student_details_promote(sp);
				if (_val)
				{
					TempData["Msg"] = "<script>alert('Student has been Promoted successfully.');</script>";
				}
				else
					TempData["Msg"] = "<script>alert('error while promoting Student.');</script>";
			}
			else
				TempData["Msg"] = "<script>alert('error while promoting Student.');</script>";

			List<StudentDetails> stuDetails = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
			return View(stuDetails);

		}

		[SessionCheck]
		public ActionResult Pass(int id)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];

			///		First Get The values of Current Student {Mode of Year /sem, Current Duration,Max Duration of Course, Current Session }//
			///		Check The New Session Has Created or Not {If New Session has created then it is allow to Promote}.
			///		check even or odd if it is odd then it can allow for semester if new session not available. other wise it's not allow without new session for even promotion or yearly promotion.
			///		if all conditon mentioned above mentioned has suits then do the promormation.

			student_Details_promote sdp = appClass.Get_Student_Class_Detials_for_Promotion(id, ARespo.University_ID);
			int Ac_Session = appClass.Check_and_Get_Next_session_Generated(ARespo.Session_ID);

			student_promote sp = new student_promote();
			sp.Student_Id = sdp.Student_Id;

			if (sdp.Semester_Year == sdp.Max_Semester_Year)
				sp.Is_Passout = 1;
			else
				sp.Is_Passout = 0;
			if (sdp.Course_Mode_ID == 1)//Semester
				if (sdp.Semester_Year % 2 == 0)
				{
					if (Ac_Session > 0)
					{
						sp.Session_ID = Ac_Session;
						sp.Semester_Year = (sdp.Semester_Year + 1);
					}
					else
					{
						TempData["Msg"] = "<script>alert('New Academic Session has not been generated yet, please request to create New Academic Session first.');</script>";
						List<StudentDetails> stuDtls = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
						return View(stuDtls);
					}
				}
				else
				{
					sp.Session_ID = sdp.Session_ID;
					sp.Semester_Year = sdp.Semester_Year;
				}
			else if (sdp.Course_Mode_ID == 2)//Year
			{
				if (Ac_Session > 0)
				{
					sp.Session_ID = Ac_Session;
					sp.Semester_Year = (sdp.Semester_Year + 1);
				}
				else
				{
					TempData["Msg"] = "<script>alert('New Academic Session has not been generated yet, please request to create New Academic Session first.');</script>";
					List<StudentDetails> stuDtls = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
					return View(stuDtls);
				}
			}

			sp.Result = 1;
			sp.Promoted_By = ARespo.Login_ID;

			bool val = appClass.Update_student_Class_sem(sp);
			if (val)
			{
				bool _val = appClass.insert_student_details_promote(sp);
				if (_val)
				{
					TempData["Msg"] = "<script>alert('Student has been Promoted successfully.');</script>";
				}
				else
					TempData["Msg"] = "<script>alert('error while promoting Student.');</script>";
			}
			else
				TempData["Msg"] = "<script>alert('error while promoting Student.');</script>";

			List<StudentDetails> stuDetails = appClass.Get_Student_List_Of_Univsersity(ARespo.University_ID, ARespo.Session_ID);
			return View(stuDetails);

		}
	}
}