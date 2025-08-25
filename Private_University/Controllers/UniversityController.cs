using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.IO;
using Razorpay.Api;
using Razorpay.Api.Errors;
using System.Configuration;

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
					ReqStringData = R.UnEncryptedURLData,
                    Amount = R.Amount
                    
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
        public ActionResult FCLforHDFCPG()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            List<PG_txn_List> PUTxnL = appClass.Get_PU_txn_List(ARespo.University_ID);         
            return View(PUTxnL);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult GoToPay_old(PG_txn_List PGTL)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];


            int RNo = new Random().Next(100, 999);
            String Receipt_Id = "PU" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + RNo;

            RazorpayClient client = new RazorpayClient("rzp_test_e6PwERqvUunUHR", "JZH6W5L2EBFUCOSycfEVaIR6");


            //Inputs for the order

            Dictionary<string, object> Input = new Dictionary<string, object>();
            Input.Add("amount", PGTL.Payble_Amt*100); // amount in the smallest currency unit
            Input.Add("receipt", Receipt_Id);
            Input.Add("currency", "INR");


            //Order creation            
            Order TxnOrder = client.Order.Create(Input);

            //ViewBag.OrderId = "order_QgT98ObHLQMoKF";
            ViewBag.OrderId = TxnOrder["id"].ToString();
            ViewBag.Key = "rzp_test_e6PwERqvUunUHR";
            

            RazorPG_Request RPG = new RazorPG_Request
            {
                TxnID = Receipt_Id,
                Order_ID = ViewBag.OrderId,
                One_Percent_Amt = PGTL.One_Percent_Amt,
                Penal_Interest = PGTL.Penal_Interest,
                Payble_Amt = PGTL.Payble_Amt,
                UniversityID = Convert.ToInt32(PGTL.University_ID.ToString()),
                UniversityName = PGTL.UniversityName.ToString(),
                UniversityEmail = PGTL.UniversityEmail.ToString(),
                UniversityMobile = PGTL.UniversityMobile.ToString(),
                BillingAddress = PGTL.BillingAddress.ToString(),
                BillingMonth = PGTL.Txn_Month.ToString(),
                BillingYear = PGTL.Txn_Year.ToString(),
                TxnDate = System.DateTime.Now
            };

            if (ModelState.IsValid)
            {
                appClass.Insert_RP_Txn_Details(RPG);      
            }

            return View(RPG);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult GoToPay(int University_ID, int Txn_Month, int Txn_Year)
        {
            try
            {
                AppClass appClass = new AppClass();

                // ✅ Get transaction details from DB
                PG_txn_List PGTL = appClass.Get_PU_txn_ByMonthYear(University_ID, Txn_Month, Txn_Year);

                if (PGTL == null)
                {
                    return HttpNotFound("No pending transaction found for the given month/year.");
                }
            

                // ✅ Generate secure receipt id
                string receiptId = "PU" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);

                // ✅ Load Razorpay keys from Web.config
                //string key = ConfigurationManager.AppSettings["RazorpayKey"];
                //string secret = ConfigurationManager.AppSettings["RazorpaySecret"];

                RazorpayClient client = new RazorpayClient("rzp_test_e6PwERqvUunUHR", "JZH6W5L2EBFUCOSycfEVaIR6");

                // ✅ Amount in paise
           
                // ✅ Create Razorpay order
                var options = new Dictionary<string, object>();
                options.Add("amount", Convert.ToInt32(PGTL.Payble_Amt * 100)); // paise
                options.Add("currency", "INR");
                options.Add("receipt", receiptId);

                Order order = client.Order.Create(options);

                // ✅ Store order ID for verification
                Session["OrderId"] = order["id"].ToString();
                ViewBag.Key = "rzp_test_e6PwERqvUunUHR";

                // ✅ Prepare request model for View
                RazorPG_Request RPG = new RazorPG_Request
                {
                    TxnID = receiptId,
                    Order_ID = order["id"].ToString(),
                    One_Percent_Amt = PGTL.One_Percent_Amt,
                    Penal_Interest = PGTL.Penal_Interest,
                    Payble_Amt = PGTL.Payble_Amt,
                    UniversityID = PGTL.University_ID,
                    UniversityName = PGTL.UniversityName,
                    UniversityEmail = PGTL.UniversityEmail,
                    UniversityMobile = PGTL.UniversityMobile,
                    BillingAddress = PGTL.BillingAddress,
                    BillingMonth = PGTL.Txn_Month.ToString(),
                    BillingYear = PGTL.Txn_Year.ToString(),
                    TxnDate = DateTime.Now,
                   // RazorpayKey = key
                };

                // ✅ Insert txn in DB for tracking (optional)
                appClass.Insert_RP_Txn_Details(RPG);

                return View("GoToPay", RPG);
            }
            catch (Exception ex)
            {
                TempData["FinalMsg"] = "Error while initializing payment: " + ex.Message;
                return RedirectToAction("PaymentFailed");
            }
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

        public ActionResult RazorPayResponse_old()
        {
            String TxnStatus_ID = "";
            AppClass appClass = new AppClass();
            string PaymentId = Request.Form["razorpay_payment_id"];
            string Razorpay_Order_Id = Request.Form["razorpay_order_id"];
            string Razorpay_Signature = Request.Form["razorpay_signature"];

            TxnCheck TC = new TxnCheck();
            TC = appClass.PGTxnCheck(Razorpay_Order_Id);

            ViewBag.TxnNumber = TC.TxnNumber;
            ViewBag.TxnAmt = TC.TxnAmount;
            ViewBag.OrderId = Razorpay_Order_Id;


            //Dictionary<string, object> input = new Dictionary<string, object>();
            //input.Add("amount", 100); // this amount should be same as transaction amount


            string key = "rzp_test_e6PwERqvUunUHR";
            string secret = "JZH6W5L2EBFUCOSycfEVaIR6";

            RazorpayClient client = new RazorpayClient(key, secret);
            String generated_signature = appClass.HMAC_Sha256(Razorpay_Order_Id + "|" + PaymentId, secret);

            try
            {
                Dictionary<string, string> options = new Dictionary<string, string>();
                options.Add("razorpay_order_id", Razorpay_Order_Id);
                options.Add("razorpay_payment_id", PaymentId);
                options.Add("razorpay_signature", Razorpay_Signature);

                //Console.WriteLine("Before signature verification");
                Utils.verifyPaymentSignature(options);
                //Console.WriteLine("After signature verification");
            }
            catch (Razorpay.Api.Errors.SignatureVerificationError ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
                TxnStatus_ID = "2";
                ViewBag.TxnStatus = "Failed";
            }
            //catch (Exception ex)
            //{
            //    TxnStatus_ID = "2";
            //    ViewBag.TxnStatus = "Failed";
            //}


            if (generated_signature == Razorpay_Signature)
            {
                ViewBag.TxnStatus = "Success";
                TxnStatus_ID = "3";
                appClass.Update_Fees_txn(TC.UniversityId, TC.FMonth, TC.FYear);

            }
            else
            {
                ViewBag.TxnStatus = "Failed";
                TxnStatus_ID = "2";
            }


            RazorPG_Update RPU = new RazorPG_Update
            {
                OrderId = Razorpay_Order_Id,
                PG_Status_id = TxnStatus_ID,
                razorpay_payment_id = PaymentId,
                RP_signature = Razorpay_Signature,
            };

            appClass.Update_RP_Txn_Details(RPU);
            return View();
        }


        [HttpPost]
       
        public JsonResult RazorPayResponseJson(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            string TxnStatus_ID = "2"; // default failed
            AppClass appClass = new AppClass();

            TxnCheck TC = appClass.PGTxnCheck(razorpay_order_id);

            string key = "rzp_test_e6PwERqvUunUHR";
            string secret = "JZH6W5L2EBFUCOSycfEVaIR6";

            RazorpayClient client = new RazorpayClient(key, secret);

            string generated_signature = appClass.HMAC_Sha256(razorpay_order_id + "|" + razorpay_payment_id, secret);

            try
            {
                Dictionary<string, string> options = new Dictionary<string, string>
        {
            { "razorpay_order_id", razorpay_order_id },
            { "razorpay_payment_id", razorpay_payment_id },
            { "razorpay_signature", razorpay_signature }
        };

                Utils.verifyPaymentSignature(options);
            }
            catch
            {
                TxnStatus_ID = "2";
                return Json(new
                {
                    TxnStatus = "Failed (Signature Mismatch)",
                    OrderId = razorpay_order_id,
                    TxnNumber = TC?.TxnNumber ?? "",
                    TxnAmt = TC?.TxnAmount ?? ""
                });
            }

            try
            {
                Payment payment = client.Payment.Fetch(razorpay_payment_id);
                decimal paidAmount = Convert.ToDecimal(payment["amount"]) / 100;
                decimal expectedAmount = Convert.ToDecimal(TC.TxnAmount);

                if (generated_signature == razorpay_signature && paidAmount == expectedAmount)
                {
                    TxnStatus_ID = "3";
                    appClass.Update_Fees_txn(TC.UniversityId, TC.FMonth, TC.FYear);
                }
                else
                {
                    TxnStatus_ID = "2";
                }
            }
            catch
            {
                TxnStatus_ID = "2";
            }

            // Save transaction log
            RazorPG_Update RPU = new RazorPG_Update
            {
                OrderId = razorpay_order_id,
                PG_Status_id = TxnStatus_ID,
                razorpay_payment_id = razorpay_payment_id,
                RP_signature = razorpay_signature,
            };
            appClass.Update_RP_Txn_Details(RPU);

            return Json(new
            {
                TxnStatus = TxnStatus_ID == "3" ? "Success" : "Failed",
                OrderId = razorpay_order_id,
                TxnNumber = TC?.TxnNumber ?? "",
                TxnAmt = TC?.TxnAmount ?? ""
            });
        }
   
        public ActionResult PGtxnRpt()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.PUTxnDetailonPG(ARespo.University_ID));            
        }
	}
}