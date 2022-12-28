using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.IO;

namespace Private_University.Controllers
{
	public class UniversityController : Controller
	{
		// GET: University

		[SessionCheck]
		public ActionResult AddUniversity()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult AddUniversity(University adduni)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();
				string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
				string finalPath = "\\UniversityLogo\\" + strDateTime + adduni.UploadFile.FileName;
				adduni.UploadFile.SaveAs(Server.MapPath("~") + finalPath);
				adduni.Univsersity_Logo = finalPath;
				bool Val = appClass.University_Insert(adduni);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('New University" + adduni.University_Name + " as been created successfully.');</script>";
					return RedirectToAction("AddUniversity");
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while creating New University.');</script>";
				}
			}
			return View();
		}

		[SessionCheck]
		public ActionResult UpdateUniversityProfile()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			UniversityProfile uniprofile = appClass.GetUniversityProfile(ARespo.University_ID);
			//ViewBag.University_ID = uniprofile.University_ID;
			ViewBag.University_Name = uniprofile.University_Name;
			ViewBag.Contact_Number = uniprofile.Contact_Number;
			ViewBag.Email_ID = uniprofile.Email_ID;
			ViewBag.Address = uniprofile.Address;
			ViewBag.Pin_Code = uniprofile.Pin_Code;
			ViewBag.Website_URL = uniprofile.Website_URL;
			ViewBag.Univsersity_Logo = uniprofile.Univsersity_Logo;
			ViewBag.University_Details = uniprofile.University_Details;
			ViewBag.Establishment_Year = uniprofile.Establishment_Year;
			ViewBag.Registration_Number = uniprofile.Registration_Number;
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult UpdateUniversityProfile(UniversityUpdate uniprofile)
		{
			var imageTypes = new string[]{
					"image/jpg",
					"image/jpeg"
				};
			if (uniprofile.UploadFile == null || uniprofile.UploadFile.ContentLength == 0)
			{
				ModelState.AddModelError("Picture", "This field is required");
			}
			else if (!imageTypes.Contains(uniprofile.UploadFile.ContentType))
			{
				ModelState.AddModelError("Picture", "Please choose either a .jpg or .jpeg image file only.");
			}
			string FileNm = "";
			if (ModelState.IsValid)
			{
				if (uniprofile.UploadFile.ContentLength > 0)
				{

					FileInfo info = new FileInfo(uniprofile.UploadFile.FileName);
					if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg")
					{
						//  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
						FileNm = "UniLogo_" + Guid.NewGuid() + info.Extension;

						string path = Server.MapPath("~/UniversityLogo/");
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);
						uniprofile.UploadFile.SaveAs(Server.MapPath("~/UniversityLogo/") + FileNm);
						uniprofile.Univsersity_Logo = FileNm;

						//string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
						//string finalPath = "\\UniversityLogo\\" + strDateTime + uniprofile.UploadFile.FileName;
						//uniprofile.UploadFile.SaveAs(Server.MapPath("~") + finalPath);
						//uniprofile.Univsersity_Logo = finalPath;

						AppClass appClass = new AppClass();
						ViewBag.University_Name = uniprofile.University_Name;
						ViewBag.Contact_Number = uniprofile.Contact_Number;
						ViewBag.Email_ID = uniprofile.Email_ID;
						ViewBag.Address = uniprofile.Address;
						ViewBag.Pin_Code = uniprofile.Pin_Code;
						ViewBag.Website_URL = uniprofile.Website_URL;
						ViewBag.Univsersity_Logo = uniprofile.Univsersity_Logo;
						ViewBag.University_Details = uniprofile.University_Details;
						ViewBag.Establishment_Year = uniprofile.Establishment_Year;
						ViewBag.Registration_Number = uniprofile.Registration_Number;
						bool Val = appClass.UniversityProfileUpdate(uniprofile);
						if (Val)
						{
							TempData["Msg"] = "<script>alert('University Profile Update successfully.');</script>";
							return RedirectToAction("UpdateUniversityProfile");
						}
						else
						{
							TempData["Msg"] = "<script>alert('error while..');</script>";
						}

					}

					else { TempData["Msg"] = "<script>alert('Please Select JPEG, or JPG File only to upload Image.');</script>"; }
				}
				else { TempData["Msg"] = "<script>alert('Please Select File to upload Image of University Logo.');</script>"; }
			}
			// return RedirectToAction("Index", "Dashboard");
			return View();
		}

		/*Office Bearer*/

		[SessionCheck]
		public ActionResult UniversityOfficeBearer()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]

		public ActionResult UniversityOfficeBearer(University_office_bearer_Insert unibearer)
		{
			var imageTypes = new string[]{
					"image/jpg",
					"image/jpeg"
				};
			if (unibearer.File_Upload == null || unibearer.File_Upload.ContentLength == 0)
			{
				ModelState.AddModelError("Picture", "This field is required");
			}
			else if (!imageTypes.Contains(unibearer.File_Upload.ContentType))
			{
				ModelState.AddModelError("Picture", "Please choose either a .jpg or .jpeg image file only.");
			}

			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();

				string FileNm = "";
				if (unibearer.File_Upload.ContentLength > 0)
				{
					FileInfo info = new FileInfo(unibearer.File_Upload.FileName);
					if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg")
					{
						//  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
						FileNm = "Bearer_" + Guid.NewGuid() + info.Extension;

						string path = Server.MapPath("~/UniversityBearer/");
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);
						unibearer.File_Upload.SaveAs(Server.MapPath("~/UniversityBearer/") + FileNm);
						unibearer.Picture = FileNm;
						bool Val = appClass.University_office_bearer_Insert(unibearer);
						if (Val)
						{
							TempData["Msg"] = "<script>alert('New Bearered as been created successfully : " + unibearer.Name + " .');</script>";
							return RedirectToAction("UniversityOfficeBearer");
						}
						else
						{ TempData["Msg"] = "<script>alert('error while creating University Office Bearer.');</script>"; }
					}
					else { TempData["Msg"] = "<script>alert('Please Select JPEG, or JPG File only to upload Image.');</script>"; }
				}
				else { TempData["Msg"] = "<script>alert('Please Select File to upload Image of Bearer.');</script>"; }
			}
			return View();
		}

		[SessionCheck]
		public ActionResult UniversityOfficeBearerShow()
		{
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			AppClass appClass = new AppClass();
			return View(appClass.Get_University_office_bearer(ARespo.University_ID));
		}

		//[HttpDelete]
		//[ValidateAntiForgeryToken]

		[SessionCheck]
		public ActionResult UniversityOfficeDelete(int id)
		{
			if (ModelState.IsValid)
			{
				AppClass appClass = new AppClass();
				AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
				University_office_bearer_Delete mnd = new University_office_bearer_Delete()
				{
					office_bearer_ID = id,
					Deleted_By = ARespo.Login_ID
				};
				bool Val = appClass.University_office_bearer_Delete(mnd);
				if (Val)
				{
					TempData["Msg"] = "<script>alert('Bearer details has been deleted successfully.');</script>";
					return RedirectToAction("UniversityOfficeBearerShow");
				}
				else
				{
					TempData["Msg"] = "<script>alert('error while deleting Bearer details.');</script>";
					return RedirectToAction("UniversityOfficeBearerShow");
				}
			}

			return View();
		}
		/*Office Bearer*/

		[SessionCheck]
		public ActionResult UniversityFeesCollectonList()
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			List<ayoge_transctions_List> aTxnL = appClass.Get_ayoge_transctions_List_Of_Univsersity(ARespo.University_ID);
			return View(aTxnL);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[SessionCheck]
		public ActionResult UniversityFeesCollectonList(FormCollection form)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			if (ModelState.IsValid)
			{
				int rno = new Random().Next(100, 999);
				Atom_PG_Request R = new Atom_PG_Request
				{
					Ayong_Txn_ID = form["hf_Ayong_Txn_ID"].ToString(),
					Amount = form["hf_Percent_Amount"].ToString(),
					//signature =, //login, pass, ttype, prodid, txnid, amt, txncurr
					CustomerName = form["hf_CustomerName"].ToString(),
					CustomerEmail = form["hf_CustomerEmail"].ToString(),
					CustomerMobile = form["hf_CustomerMobile"].ToString(),
					BillingAddress = form["hf_BillingAddress"].ToString(),
					BillingMonth = form["hf_Txn_Month"].ToString(),
					BillingYear = form["hf_Txn_Year"].ToString(),
					BankIdentifier = 2001,
					txnID = "PU" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + rno,
					TxnDate = System.DateTime.Now
				};
				string PG_URL = appClass.Get_PG_URL_Complete(R);

				PG_String_Insert i = new PG_String_Insert
				{
					Ayong_Txn_ID = Convert.ToInt32(form["hf_Ayong_Txn_ID"].ToString()),
					University_ID = Convert.ToInt32(form["hf_University_ID"].ToString()),
					RequestString = PG_URL,
					Txn_Number = R.txnID,
					ReqStringData = R.UnEncryptedURLData
				};

				appClass.Insert_payment_string(i);
				return Redirect(PG_URL);

				//decimal Amount_txn = Convert.ToDecimal(form["txt_Amount"].ToString());
				//decimal Amount_txn_ay = Convert.ToDecimal(form["txt_Ayong_Txn_Amt"].ToString());
				//if (Amount_txn >= Amount_txn_ay)
				//{
				//	string TxnNumb = "A" + System.DateTime.Now.ToString("yyyyMMddhhmmss");
				//	ayoge_transctions_Draft aTd = new ayoge_transctions_Draft()
				//	{
				//		Ayong_Txn_ID = Convert.ToInt32(form["txt_Ayong_Txn_ID"].ToString()),
				//		Demand_Draft_No = form["txt_Demand_Draft_No"].ToString(),
				//		Draft_Bank = form["txt_Draft_Bank"].ToString(),
				//		Txn_Number = TxnNumb,
				//		Date_of_issue = Convert.ToDateTime(form["txt_Issue_Date"].ToString()),
				//	};

				//	bool Val = appClass.ayoge_transctions_Update(aTd);
				//	if (Val)
				//		TempData["Msg"] = "<script>alert('Fees Collecton Draft Details Updated successfully.');</script>";
				//	else
				//		TempData["Msg"] = "<script>alert('error while updating Draft Details.');</script>";
				//}
				//else
				//	TempData["Msg"] = "<script>alert('Amount of Demand Draft and Amount to be Payable is not matched.');</script>";

			}
			List<ayoge_transctions_List> aTxnL = appClass.Get_ayoge_transctions_List_Of_Univsersity(ARespo.University_ID);
			return View(aTxnL);
		}

		[SessionCheck]
		public ActionResult UniversityFeesCollecton(int id)
		{
			AppClass appClass = new AppClass();
			AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
			ModelState.Clear();
			List<Challan_University> cu = appClass.Get_Challan_University_Of_Univsersity(id);
			return View(cu);
		}

		[SessionCheck]
		public ActionResult UniversityPenalFeesCollecton()
		{
			return View();
		}


		public ActionResult PaymentResponse()
		{

			AppClass appClass = new AppClass();
			var fc = Request.Form;
			string Login = fc["login"].ToString();
			string Data = fc["encdata"].ToString();

			byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			int iterations = 65536;

			string ReturnData = appClass.Decrypt(Data, System.Configuration.ConfigurationManager.AppSettings["PGAESRespKey"].ToString(), System.Configuration.ConfigurationManager.AppSettings["PGAESRespSltKey"].ToString(), iv, iterations);
			string[] DataSplit = ReturnData.Split('&');
			var valueDictionary = new Dictionary<string, string>();
			foreach (string a in DataSplit)
			{
				string[] b = a.Split('=');
				valueDictionary[b[0]] = b[1];
			}

			string ReSignature = valueDictionary["signature"];

			PG_String_Update u = new PG_String_Update
			{
				Ayong_Txn_ID = Convert.ToInt32(valueDictionary["udf9"]),
				Txn_Number = valueDictionary["mer_txn"],
				ResponseString = Data,
				RespoStringData = ReturnData,
				PG_Status = valueDictionary["desc"]
			};
			appClass.Update_payment_string(u);

			if (valueDictionary["desc"] == "SUCCESS")
			{
				string[] datetim = valueDictionary["date"].Split(' ');
				DateTime Dt_paid = DateTime.Parse(datetim[2] + "-" + datetim[1] + "-" + datetim[5]);//.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

				ayoge_transctions_Draft aTd = new ayoge_transctions_Draft()
				{
					Ayong_Txn_ID = Convert.ToInt32(valueDictionary["udf9"]),
					Demand_Draft_No = valueDictionary["bank_txn"],
					Draft_Bank = valueDictionary["bank_name"],
					Txn_Number = valueDictionary["mer_txn"],
					Date_of_issue = Dt_paid,
				};

				bool Val = appClass.ayoge_transctions_Update(aTd);
				if (Val)
				{
					FunctionClass fn = new FunctionClass();
					string Message = "Dear Sir/Ma'am, Amount Rupee" + valueDictionary["amt"] + " paid Successfully for Month of " + valueDictionary["udf11"] + ", " + valueDictionary["udf10"];
					fn.SendEmail("", "10% Amount Paid Successfully",Message , true);
					fn.SendSMS_T("", "", Message);
					TempData["Msg"] = "<script>alert('Fees Collecton Draft Details Updated successfully.');</script>";
				}
				else
					TempData["Msg"] = "<script>alert('error while updating Draft Details.');</script>";
			}
			ViewBag.TxnStatus = valueDictionary["desc"];
			ViewBag.TxnNumber = valueDictionary["mer_txn"];
			ViewBag.TxnBankNo = valueDictionary["bank_txn"];
			ViewBag.TxnAmt = valueDictionary["amt"];

			return View();//RedirectToAction("UniversityFeesCollectonList");
		}


	}
}