using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.Data;
using Newtonsoft.Json;
using System.Globalization;

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
            TempData["UnvName"] = ARespo.University_Name;
            ModelState.Clear();            
            return View();
        }
        public String MWFReport()
        {
            AppClass AC = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.MonthWiseFeesCollection(ARespo.University_ID, ARespo.Session_ID);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(DT);
            return JSONString;
        }

        public String CWFReport(int MonthNo, int Year)
        {
            AppClass AC = new AppClass();            
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.CourseWiseFeesCollection(ARespo.University_ID, ARespo.Session_ID, MonthNo, Year);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(DT);
            return JSONString;
        }
        public String SCWFReport(int USCID, int MonthNo, int Year)
        {
            AppClass AC = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.SubCourseFeesStudentList(USCID, ARespo.Session_ID, MonthNo, Year);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(DT);
            return JSONString;
        }

        [HttpGet]
        [SessionCheck]
        public ActionResult FeesReceived()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]

        public ActionResult FeesReceived(FeesReceived FR)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            if (ModelState.IsValid)
            {
                int Val = appClass.Fees_Insert(FR);
                if (Val > 0)
                {
                    TempData["Msg"] = "<script>alert('Data Save Successfully.');</script>";
                    return RedirectToAction("FeesReceived");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('Error in data saving.');</script>";
                }

                }
                return View();
        }
        public ActionResult FeesReport()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.FeesDetailList(ARespo.University_ID));
        }

        public ActionResult FeesCollection()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            //ViewBag.CourseList = appClass.PopulateCourseForFees(ARespo.University_ID);            
            return View();
        }
        public JsonResult CoursesForFees()
        {
            AppClass AppC = new AppClass();
            String CoursesListForFee = "<option>---- Select Course ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateCourseForFeesJR(ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CoursesListForFee += "<option CM=" + dt.Rows[i][3].ToString() + " NOSY=" + dt.Rows[i][2].ToString() + " value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][1].ToString() + " </option>";
            }
            return Json(CoursesListForFee, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubCoursesForFees(string Univ_Course_ID)
        {
            AppClass AppC = new AppClass();
            String SubCourseList = "<option>---- Select Branch ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateSubCourseForFees(Convert.ToInt32(Univ_Course_ID), ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SubCourseList += "<option value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][1].ToString() + " </option>";
            }
            return Json(SubCourseList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Sem_YearListForFees(String SC_ID)
        {
            AppClass AC = new AppClass();
            String SY_List = "<option>---- Select ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.PopulateSem_YearForFees(Convert.ToInt32(SC_ID), ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                SY_List += "<option value=" + DT.Rows[i][0].ToString() + ">" + DT.Rows[i][0].ToString() + " </option>";
            }
            return Json(SY_List, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SY_StudentList(int USC_ID, int SY)
        {
            AppClass AC = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            var StudentList = AC.StudentsListOfSem_Year(ARespo.University_ID, USC_ID, SY, ARespo.Session_ID);
            return Json(StudentList, JsonRequestBehavior.AllowGet);
        }

        public String InsertFees(String Values_Str)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            int val = appClass.Fees_Insert(Values_Str);
            if (val > 0)
            {
                
                Msg = "Student fees saved Successfully. !!";
                //RedirectToAction("FeesCollection");
            }
            else
            {
                //TempData["FinalMsg"] = "<script>alert('Error in saving student data  !!');</script>";
                Msg = "Error in saving student data  !!";
            }
            return Msg;
        }

        public String DeleteFees(Int32 TxnId)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            bool val = appClass.Fees_Delete(TxnId);
            if (val)
            {
                Msg = "Record deleted Successfully !!";
            }
            else
            {
                Msg = "Error in deleting record !!";
            }
            return Msg;
        }
        public String UpdateFees(Int32 TxnId, Double Amt, String R_Date)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            bool val = appClass.Update_Fees(TxnId, Amt, R_Date);
            if (val)
            {
                Msg = "Record modified Successfully !!";
            }
            else
            {
                Msg = "Error in modifing record !!";
            }
            return Msg;
        }
        [HttpGet]
        [SessionCheck]
        public ActionResult UploadFeesData()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Fees_Files_for_University(ARespo.University_ID, ARespo.Session_ID));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UploadFeesData(HttpPostedFileBase file)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            if (file != null && file.ContentLength > 0)
            {

                Upload_Student_data USD = new Upload_Student_data();
                FileInfo info = new FileInfo(file.FileName);
                String UserName = ARespo.UserName.ToString().ToUpper();
                USD.University_ID = ARespo.University_ID;
                USD.Session_Id = ARespo.Session_ID;
                if (UserName.ToLower().Contains('_'))
                {
                    USD.File_Name = UserName.Substring(0, UserName.LastIndexOf("_") + 1)
                        + ARespo.University_ID + "_" + System.DateTime.Now.ToString("dd_MM_yy_HHmmss") + info.Extension;
                }
                else
                {
                    USD.File_Name = UserName.ToString() + ARespo.University_ID + "_" + System.DateTime.Now.ToString("dd_MM_yy_HHmmss") + info.Extension;
                }
                try
                {
                    if (info.Extension.ToLower() == ".xlsx" || info.Extension.ToLower() == ".xls")
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string UploadPath = Server.MapPath("~/UploadedFeesData/" + ARespo.University_ID.ToString());
                            if (!Directory.Exists(UploadPath))
                                Directory.CreateDirectory(UploadPath);

                            string _path = Path.Combine(Server.MapPath("~/UploadedFeesData/" + ARespo.University_ID.ToString()), USD.File_Name);
                            USD.File_path = "/UploadedFeesData/" + ARespo.University_ID.ToString() + "/" + USD.File_Name.ToString();
                            file.SaveAs(_path);
                            appClass.UploadFeesData(USD);

                            TempData["msg"] = @"<div class='alert alert-success' role='alert'>
                                        File Uploaded Successfully !! <br/> Fees records will be reflected in report after processing of uploaded file </div>";
                        }
                    }
                    else
                    {
                        TempData["msg"] = @"<div class='alert alert-warning' role='alert'>
                        Upload correct file type !! </div>";
                    }
                }
                catch
                {
                    TempData["msg"] = @"<div class='alert alert-danger' role='alert'>
                                        File upload failed !! </div >";
                }

            }
            else
            {
                TempData["msg"] = @"<div class='alert alert-danger' role='alert'>
                                       No any file has been selected to upload  !! </div >";
            }
            return View(appClass.Fees_Files_for_University(ARespo.University_ID, ARespo.Session_ID));
        }


    


        [HttpGet]
        [SessionCheck]
        public ActionResult RptUploadedFeesData()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.PU_Uploaded_Fees_Files( ARespo.Session_ID));
        }

        public String DeleteFeesFile(Int32 File_Id, string FPath)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            bool val = appClass.FeesFile_Delete(File_Id);
            string FileLocated = Server.MapPath(FPath);
            if (val)
            {
                if (System.IO.File.Exists(FileLocated))
                {
                    System.IO.File.Delete(FileLocated);
                    Msg = "File deleted Successfully !!";
                }
                else
                {
                    Msg = "File Not Found !!";
                }
            }
            else
            {
                Msg = "Error in deleting File !!";
            }
            return Msg;
        }

        [HttpPost]
        public ActionResult ValidateExcelFile(HttpPostedFileBase file)
        {
            AppClass appClass = new AppClass();
            List<FeeExcelValidationResult> invalidRecords = new List<FeeExcelValidationResult>();

            if (file == null || file.ContentLength == 0)
            {
                TempData["msg"] = "<div class='alert alert-danger'>No file selected!</div>";
                return View("InvalidRecords", invalidRecords);
            }

            try
            {
                using (var stream = file.InputStream)
                {
                    IWorkbook workbook = new XSSFWorkbook(stream); // .xlsx
                    ISheet sheet = workbook.GetSheetAt(0);

                    if (sheet == null)
                    {
                        TempData["msg"] = "<div class='alert alert-danger'>Worksheet not found!</div>";
                        return View("InvalidRecords", invalidRecords);
                    }

                    int lastRow = sheet.LastRowNum;

                    // ✅ keep a set for duplicate check
                    HashSet<string> seenRecords = new HashSet<string>();

                    for (int row = 1; row <= lastRow; row++) // skip header
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        string enrollmentNo = currentRow.GetCell(1)?.ToString().Trim(); // Enrollment No (col index 1)
                        string dateStr = "";
                        string issues = "";
                        DateTime parsedDate;

                        ICell feeCell = currentRow.GetCell(2);  // Fee Amount column (col index 2)
                        ICell dateCell = currentRow.GetCell(3); // Date column (col index 3)

                        // ✅ STOP at first completely blank row
                        if (string.IsNullOrWhiteSpace(enrollmentNo) &&
                            (dateCell == null || string.IsNullOrWhiteSpace(dateCell.ToString())))
                        {
                            break;
                        }

                        // ===== Date Validation =====
                        if (dateCell != null)
                        {
                            if (dateCell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(dateCell))
                            {
                                parsedDate = dateCell.DateCellValue;
                                dateStr = parsedDate.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                dateStr = dateCell.ToString().Trim();

                                if (!DateTime.TryParseExact(dateStr,
                                    new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" },
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out parsedDate))
                                {
                                    issues += "Invalid Date Format (must be DD/MM/YYYY); ";
                                }
                                else
                                {
                                    dateStr = parsedDate.ToString("dd/MM/yyyy");
                                }
                            }
                        }
                        else
                        {
                            issues += "Date is missing; ";
                        }

                        // ===== Enrollment Validation =====
                        if (string.IsNullOrWhiteSpace(enrollmentNo) || !appClass.CheckEnrollmentExists(enrollmentNo))
                        {
                            issues += "Enrollment No not found; ";
                        }

                        // ===== Fee Amount Validation =====
                        double feeAmount = -1;
                        if (feeCell == null || string.IsNullOrWhiteSpace(feeCell.ToString()))
                        {
                            issues += "Fee Amount is missing; ";
                        }
                        else
                        {
                            if (feeCell.CellType == CellType.Numeric)
                            {
                                feeAmount = feeCell.NumericCellValue;
                            }
                            else if (!double.TryParse(feeCell.ToString().Trim(), out feeAmount))
                            {
                                issues += "Fee Amount must be numeric; ";
                            }

                            if (feeAmount <= 0)
                            {
                                issues += "Fee Amount must be greater than 0; ";
                            }
                        }

                        // ===== Duplicate Validation =====
                        if (!string.IsNullOrWhiteSpace(enrollmentNo) && !string.IsNullOrWhiteSpace(dateStr) && feeAmount > 0)
                        {
                            string key = $"{enrollmentNo}|{feeAmount}|{dateStr}";
                            if (seenRecords.Contains(key))
                            {
                                issues += "Duplicate record found in Excel; ";
                            }
                            else
                            {
                                seenRecords.Add(key);
                            }
                        }

                        // ===== Collect invalid rows =====
                        if (!string.IsNullOrEmpty(issues))
                        {
                            invalidRecords.Add(new FeeExcelValidationResult
                            {
                                RowNumber = row + 1, // Excel row number (1-based)
                                EnrollmentNo = enrollmentNo,
                                DateValue = dateStr,
                                Issues = issues.TrimEnd(' ', ';')
                            });
                        }
                    }
                }

                if (invalidRecords.Any())
                    return View("InvalidRecords", invalidRecords);

                // ✅ All records valid, redirect to UploadFees view
                ViewBag.Status = true;
                TempData["msg"] = "<div class='alert alert-success'>All records are valid!</div>";
                return View("UploadFeesData");
            }
            catch (Exception ex)
            {
                TempData["msg"] = $"<div class='alert alert-danger'>Error reading Excel: {ex.Message}</div>";
                return View("InvalidRecords", invalidRecords);
            }
        }

    }
}