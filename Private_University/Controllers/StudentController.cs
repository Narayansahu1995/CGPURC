using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Private_University.Controllers
{
    public class StudentController : Controller
    {

        private static readonly Regex MobileRegex = new Regex(@"^\d{10}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex AadhaarRegex = new Regex(@"^\d{12}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        FunctionClass Fnc = new FunctionClass();
        // GET: Student
        [SessionCheck]
        public ActionResult AddStudent()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            //ViewBag.SessionList = appClass.Get_master_academic_session();
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            ViewBag.GenderList = appClass.GenderList();
            return View();
        }

        // POST: Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult AddStudent(Student std)
        {
            
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            ViewBag.GenderList = appClass.GenderList();
            bool IsENOExists = appClass.IsENOExists(std.Enrollment_Number, ARespo.University_ID);
            if (ModelState.IsValid)
            {
                if (!IsENOExists)
                {
                    int Val = appClass.Student_Insert(std);
                    if (Val > 0)
                    {
                        //student_promote sp = new student_promote()
                        //{
                        //    Student_Id = Val,
                        //    Session_ID = std.Session_ID,
                        //    Semester_Year = std.Semester_Year,
                        //    Is_Passout = 0,
                        //    Result = 1,
                        //    Promoted_By = std.Entry_By
                        //};
                        //appClass.insert_student_details_promote(sp);

                        TempData["Msg"] = "<script>alert('Student " + std.Student_Name + "Data Save Successfully.');</script>";
                        return RedirectToAction("AddStudent");
                    }
                    else
                    {
                        TempData["Msg"] = "<script>alert('Some Error...!!! Student Details not Inserted Successfully.');</script>";

                    }
                }
                else
                {
                    TempData["Msg"] = "<script>alert('Entered enrollment No :- "+std.Enrollment_Number+" is already exists. !!');</script>";                    
                }
            }

            return View();
        }

       
        public JsonResult Sub_Course(string Univ_Course_ID)
        {
            AppClass AppC = new AppClass();
            String SubCourseList = "<option>---------Select Sub Course-----------</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateSubCourse(Convert.ToInt32(Univ_Course_ID), ARespo.University_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SubCourseList += "<option value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][3].ToString() + " </option>";
            }           
            return Json(SubCourseList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Course_Mode_1(string Univ_Course_ID)
        {
            AppClass AppC = new AppClass();
            String CourseModeList = "<option>---------Select Course Mode-----------</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateCourseMode1(Convert.ToInt32(Univ_Course_ID), ARespo.University_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CourseModeList += "<option value=" + dt.Rows[i][4].ToString() + ">" + dt.Rows[i][6].ToString() + " </option>";
            }
            return Json(CourseModeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Course_Year_Sem(string Univ_Course_ID)
        {
            AppClass AppC = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            List<SelectListItem> val = AppC.PopulateSemester_Year(Convert.ToInt32(Univ_Course_ID), ARespo.University_ID);
            String SemList = "<option>----------------------------Persuing Year/Semester-----------</option>";
            for (int count = 1; count <= int.Parse(val[0].Value.ToString()); count++)
            {
                SemList += "<option value=" + count + ">" + count + "</option>";
            }
            return Json(SemList, JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult GetStudentData(int Student_ID)
        {
            AppClass AppC = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();           
            DataTable Sdt = AppC.Get_Student_Detail(Student_ID, ARespo.University_ID, ARespo.Session_ID);
            var SD = new StudentEdit();
            if (Sdt.Rows.Count > 0)
            {
                if (Sdt.Rows[0]["Update_DateTime"].Equals(null) || Sdt.Rows[0]["Update_DateTime"].ToString() == "")
                {
                    SD = new StudentEdit()
                    {
                        Student_Id = Convert.ToInt32(Sdt.Rows[0]["Student_Id"].ToString()),
                        Enrollment_Number = Sdt.Rows[0]["Enrollment_Number"].ToString(),
                        Student_Name = Sdt.Rows[0]["Student_Name"].ToString(),
                        Father_Name = Sdt.Rows[0]["Father_Name"].ToString(),
                        Gender = Convert.ToInt16(Sdt.Rows[0]["Gender"]),                        
                        Date_of_Birth = Convert.ToDateTime(Sdt.Rows[0]["Date_of_Birth"]).ToString("yyyy-MM-dd"),                      
                        Aadhar_Number = Sdt.Rows[0]["Aadhar_Number"].ToString(),
                        Date_of_Admission = Convert.ToDateTime(Sdt.Rows[0]["Date_of_Admission"]).ToString("yyyy-MM-dd"),
                        Mobile_Number_Stu = Sdt.Rows[0]["Mobile_Number_Stu"].ToString(),
                        Mobile_Number_Father = Sdt.Rows[0]["Mobile_Number_Father"].ToString(),
                        Email_ID = Sdt.Rows[0]["Email_ID"].ToString(),
                        Address = Sdt.Rows[0]["Address"].ToString(),
                        Univ_Course_ID = Convert.ToInt32(Sdt.Rows[0]["Univ_Course_ID"].ToString()),
                        Univ_subCourse_ID = Convert.ToInt32(Sdt.Rows[0]["Univ_subCourse_ID"].ToString()),
                        Course_Mode_ID = Convert.ToInt32(Sdt.Rows[0]["Course_Mode_ID"].ToString()),
                        Semester_Year = Convert.ToInt32(Sdt.Rows[0]["Semester_Year"].ToString())
                    };
                    return Json(SD, JsonRequestBehavior.AllowGet);
                }
                else
                {                   
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                
            }            
            else
            {                
                return Json(2, JsonRequestBehavior.AllowGet);                
            }
            
        }

        public ActionResult StudentList()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            return View(appClass.Get_Student_List(ARespo.University_ID));
        }

        [HttpGet]
        [SessionCheck]
        public ActionResult StudentsRpt(int Type)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            //ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            //ViewBag.GenderList = appClass.GenderList();
            TempData["UN"] = ARespo.University_Name;
            TempData["AS"] = ARespo.Academic_Session;
            return View(appClass.Get_Students_Records(ARespo.University_ID, ARespo.Session_ID));
        }
               

        public ActionResult Edit(int id)
        {

            ModelState.Clear();
            AppClass appClass = new AppClass();
            StudentEdit SE = new StudentEdit();
            SE = appClass.Get_Std_Record(id);            
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            ViewBag.GenderList = appClass.GenderList();
            ViewBag.BranchList = appClass.GetSubCourse(Convert.ToInt32(SE.Univ_Course_ID), ARespo.University_ID);
            DataTable DT = appClass.PopulateCourseMode1(Convert.ToInt32(SE.Univ_Course_ID), ARespo.University_ID);
            ViewBag.CourseMode = DT.Rows[0]["Course_Mode"];
            ViewBag.NoSY = DT.Rows[0]["Number_of_Year_Sem"];            
            return View(SE);
        }

        public ActionResult Delete(int id)
        {
            ModelState.Clear();
            AppClass appClass = new AppClass();
            StudentDelete SD = new StudentDelete();
            SD = appClass.Get_Std_Record_For_Delete(id);
            return View(SD);
        }

        [HttpPost]       
        public ActionResult Delete(StudentDelete SD )
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            if (ModelState.IsValid)
            {
                bool Val = appClass.Student_Delete(SD.Student_Id);
                if (Val)
                {
                    TempData["FinalMsg"] = "<script>alert('Student record deleted Successfully. !!');</script>";                    
                }
                else
                {
                    TempData["FinalMsg"] = "<script>alert('Error in deleting student data  !!');</script>";                   
                }
            }
            return RedirectToAction("StudentsRpt",new { @Type = 1 });
        }

        public ActionResult CancelAdmission(int id)
        {
            ModelState.Clear();
            AppClass appClass = new AppClass();
            StudentDelete SD = new StudentDelete();
            SD = appClass.Get_Std_Record_For_Delete(id);
            return View(SD);
        }

        [HttpPost]
        public ActionResult CancelAdmission(StudentDelete SD)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            if (ModelState.IsValid)
            {
                bool Val = appClass.Student_Cancel(SD.Student_Id);
                if (Val)
                {
                    TempData["FinalMsg"] = "<script>alert('Student admission cancelled Successfully. !!');</script>";
                }
                else
                {
                    TempData["FinalMsg"] = "<script>alert('Error in canceling student admission  !!');</script>";
                }
            }
            return RedirectToAction("StudentsRpt", new { @Type = 1 });
            
        }

        public ActionResult EditStudent()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();         
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            ViewBag.GenderList = appClass.GenderList();
            //ViewBag.SubCourseList = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult EditStudent(StudentEdit SE)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            bool IsENOExistsForEdit = appClass.IsENOExistsForEdit(SE.Enrollment_Number, ARespo.University_ID, SE.Student_Id);
            ViewBag.GenderList = appClass.GenderList();
            ViewBag.JavaScriptFunction = string.Format("EnableEdit();");
            //ViewBag.JavaScriptFunction = string.Format("GetDDLSelections();");
            if (ModelState.IsValid)
            {
                if (!IsENOExistsForEdit)
                {
                    bool Val = appClass.Student_Update(SE);
                    if (Val)
                    {
                        TempData["FinalMsg"] = "<script>alert('Enrollment No. - " + SE.Enrollment_Number + " Data Updated Successfully. !!');</script>";
                        return RedirectToAction("EditStudent");
                    }
                    else
                    {
                        TempData["FinalMsg"] = "<script>alert('Error in Updating student data  !!');</script>";
                        return RedirectToAction("EditStudent");
                    }
                }
                else
                {
                    TempData["FinalMsg"] = "<script>alert('Entered enrollment No :- " + SE.Enrollment_Number + " is already exists, So data can not be updated  !!');</script>";
                    return RedirectToAction("EditStudent");
                }
            }

            return View();
        }

      
        [SessionCheck]
        public ActionResult DeleteStudent(StudentEdit SE)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();                
                bool Val = appClass.Student_Delete(SE.Student_Id);
                if (Val)
                {
                    TempData["FinalMsg"] = "<script>alert('Student details has been deleted successfully.');</script>";
                    return RedirectToAction("EditStudent");
                }
                else
                {
                    TempData["FinalMsg"] = "<script>alert('error while deleting Student details.');</script>";
                    return RedirectToAction("EditStudent");
                }
            }
            return View();
        }


        [HttpGet]
        [SessionCheck]
        public ActionResult UploadStudentData()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_Files_List_for_University(ARespo.University_ID, ARespo.Session_ID));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UploadStudentData(HttpPostedFileBase file)
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
                if (UserName.ToLower().Contains('_')) { 
                USD.File_Name = UserName.Substring(0, UserName.LastIndexOf("_") + 1)
                    + ARespo.University_ID + "_" + System.DateTime.Now.ToString("dd_MM_yy_HHmmss") + info.Extension;               
                }
                else
                {
                    USD.File_Name = UserName.ToString()+ ARespo.University_ID + "_" + System.DateTime.Now.ToString("dd_MM_yy_HHmmss") + info.Extension;
                }
                try
                {
                    if (info.Extension.ToLower() == ".xlsx" || info.Extension.ToLower() == ".xls")
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                        string UploadPath = Server.MapPath("~/UploadedStudentsData/" + ARespo.University_ID.ToString());
                        if (!Directory.Exists(UploadPath))
                            Directory.CreateDirectory(UploadPath);

                        string _path = Path.Combine(Server.MapPath("~/UploadedStudentsData/" + ARespo.University_ID.ToString()), USD.File_Name);
                        USD.File_path = "/UploadedStudentsData/" + ARespo.University_ID.ToString() + "/" + USD.File_Name.ToString();
                        file.SaveAs(_path);
                        appClass.UploadStudentData(USD);
                        
                        TempData["msg"] = @"<div class='alert alert-success' role='alert'>
                                        File Uploaded Successfully !! <br/> Students records will be reflected in report after processing of uploaded file</div>";                       
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
            return View(appClass.Get_Files_List_for_University(ARespo.University_ID, ARespo.Session_ID));
        }


        // new function to upload student data 16092025 //

        [HttpGet]
        public ActionResult UploadStudentData_new()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_Student_Count_for_University(ARespo.University_ID, ARespo.Session_ID));
        }


        [HttpPost]


        public ActionResult UploadStudentData_new(HttpPostedFileBase file)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
           

            if (file == null || file.ContentLength == 0)
            {
                TempData["msg"] = "<div class='alert alert-danger'>No file selected!</div>";
                return RedirectToAction("UploadStudentData");
            }

            var invalidRecords = new List<StudentExcelValidationResult>();
            var validRecords = new List<ValidatedStudentRecord>();

            try
            {
                using (var stream = file.InputStream)
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    var formatter = new DataFormatter();

                    // ✅ Validate headers
                    string[] expectedHeaders = {
                                    "Sno.",
                                    "Enrollment Number",
                                    "Student Name",
                                    "Father Name",
                                    "Gender",
                                    "Date Of Birth (DD-MM-YYYY)",
                                    "Aadhar Number",
                                    "Date of Addmission (DD-MM-YYYY)",
                                    "Student Mobile Number",
                                    "Father's Mobile Number",
                                    "E-Mail ID",
                                    "Permanent Address",
                                    "Course Name",
                                    "Course Branch Name",
                                    "Persuing Year/Semester"
                                };



                    IRow headerRow = sheet.GetRow(0);
                    if (headerRow == null || headerRow.LastCellNum < expectedHeaders.Length)
                    {
                        TempData["msg"] = "<div class='alert alert-danger'>Invalid Excel format!</div>";
                        return RedirectToAction("UploadStudentData_new");
                    }

                    for (int i = 0; i < expectedHeaders.Length; i++)
                    {
                        string actualHeader = Regex.Replace(formatter.FormatCellValue(headerRow.GetCell(i)).Trim(), @"\s+", " ");
                        if (!string.Equals(actualHeader, expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                        {
                            TempData["msg"] = $"<div class='alert alert-danger'>Expected '{expectedHeaders[i]}' in column {i + 1}, but found '{actualHeader}'</div>";
                            return RedirectToAction("UploadStudentData_new");
                        }
                    }

                    var seenEnrollmentNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var seenMobileNumbers = new HashSet<string>();
           

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        // ✅ Skip completely empty rows
                        bool isRowEmpty = true;
                        for (int col = 0; col < currentRow.LastCellNum; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(formatter.FormatCellValue(currentRow.GetCell(col))))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }
                        if (isRowEmpty) continue;

                        string issues = "";
                        var student = new ValidatedStudentRecord
                        {
                            Sno = formatter.FormatCellValue(currentRow.GetCell(0)).Trim(),
                            EnrollmentNumber = formatter.FormatCellValue(currentRow.GetCell(1)).Trim(),
                            StudentName = formatter.FormatCellValue(currentRow.GetCell(2)).Trim(),
                            FatherName = formatter.FormatCellValue(currentRow.GetCell(3)).Trim(),
                            Gender = formatter.FormatCellValue(currentRow.GetCell(4)).Trim(),
                            AadharNumber = formatter.FormatCellValue(currentRow.GetCell(6)).Trim(),
                            StudentMobileNumber = formatter.FormatCellValue(currentRow.GetCell(8)).Trim(),
                            FathersMobileNumber = formatter.FormatCellValue(currentRow.GetCell(9)).Trim(),
                            EmailID = formatter.FormatCellValue(currentRow.GetCell(10)).Trim(),
                            PermanentAddress = formatter.FormatCellValue(currentRow.GetCell(11)).Trim(),
                            CourseName = formatter.FormatCellValue(currentRow.GetCell(12)).Trim(),
                            CourseBranchName = formatter.FormatCellValue(currentRow.GetCell(13)).Trim(),
                            PersuingYearSemester = formatter.FormatCellValue(currentRow.GetCell(14)).Trim()
                        };

                        // ✅ Duplicate check
                        if (!string.IsNullOrWhiteSpace(student.EnrollmentNumber))
                        {
                            if (seenEnrollmentNumbers.Contains(student.EnrollmentNumber))
                                issues += "Duplicate Enrollment Number; ";
                            else
                                seenEnrollmentNumbers.Add(student.EnrollmentNumber);
                        }


                        // ✅ Duplicate check for Student Mobile Number
                        if (!string.IsNullOrWhiteSpace(student.StudentMobileNumber))
                        {
                            if (seenMobileNumbers.Contains(student.StudentMobileNumber))
                                issues += "Duplicate Student Mobile Number; ";
                            else
                                seenMobileNumbers.Add(student.StudentMobileNumber);
                        }
                        // ✅ Date of Birth (required)
                        string[] allowedDateFormats = { "dd-MM-yyyy","dd/MM/yyyy"};
                        DateTime dob;
                        DateTime doa;
                        ICell dobCell = currentRow.GetCell(5);

                        if (dobCell != null)
                        {
                            if (dobCell.CellType == NPOI.SS.UserModel.CellType.Numeric && DateUtil.IsCellDateFormatted(dobCell))
                            {
                                dob = dobCell.DateCellValue;
                                student.DateOfBirth = dob.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                string dobValue = formatter.FormatCellValue(dobCell).Trim();
                                if (DateTime.TryParseExact(dobValue, allowedDateFormats,
                                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                                {
                                    student.DateOfBirth = dob.ToString("dd-MM-yyyy");
                                }
                                else
                                {
                                    issues += "Invalid Date of Birth (use DD-MM-YYYY or DD/MM/YYYY); ";
                                }
                            }
                        }
                        else
                        {
                            issues += "Date of Birth is required; ";
                        }


                        ICell doaCell = currentRow.GetCell(7);
                      

                        if (doaCell != null)
                        {
                            if (doaCell.CellType == NPOI.SS.UserModel.CellType.Numeric && DateUtil.IsCellDateFormatted(doaCell))
                            {
                                doa = doaCell.DateCellValue;
                                student.DateOfAdmission = doa.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                string doaValue = formatter.FormatCellValue(doaCell).Trim();
                                if (DateTime.TryParseExact(doaValue, allowedDateFormats,
                                        CultureInfo.InvariantCulture, DateTimeStyles.None, out doa))
                                {
                                    student.DateOfBirth = doa.ToString("dd-MM-yyyy");
                                }
                                else
                                {
                                    issues += "Invalid Date of Admission (use DD-MM-YYYY or DD/MM/YYYY); ";
                                }
                            }
                        }
                        else
                        {
                            issues += "Date of Admission is required; ";
                        }

                        // ✅ Mobile number validation
                        // Required fields check (except Aadhar, Father's Mobile, Email)
                        if (string.IsNullOrWhiteSpace(student.Sno))
                            issues += "Sno is required; ";
                        if (string.IsNullOrWhiteSpace(student.EnrollmentNumber))
                            issues += "Enrollment Number is required; ";
                        if (string.IsNullOrWhiteSpace(student.StudentName))
                            issues += "Student Name is required; ";
                        if (string.IsNullOrWhiteSpace(student.FatherName))
                            issues += "Father Name is required; ";
                        if (string.IsNullOrWhiteSpace(student.Gender))
                        {
                            issues += "Gender is required; ";
                        }
                        else
                        {
                            if (student.Gender.Equals("M", StringComparison.OrdinalIgnoreCase) ||
                                student.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
                            {
                                student.Gender = "1"; // ✅ Male
                            }
                            else if (student.Gender.Equals("F", StringComparison.OrdinalIgnoreCase) ||
                                     student.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
                            {
                                student.Gender = "2"; // ✅ Female
                            }
                            else
                            {
                                issues += "Invalid Gender (only M/F allowed); ";
                            }
                        }


                        if (string.IsNullOrWhiteSpace(student.StudentMobileNumber))
                        {
                            issues += "Student Mobile Number is required; ";
                        }
                       
                        if (string.IsNullOrWhiteSpace(student.PermanentAddress))
                            issues += "Permanent Address is required; ";
                        if (!string.IsNullOrWhiteSpace(student.CourseName))
                        {
                            var course = appClass.GetCourseIDByUnvCourse(
                                student.CourseName,
                                ARespo.University_ID,
                                student.CourseBranchName
                            );

                            if (course == null)
                            {
                                issues += $"Course '{student.CourseName}' not found for this university; ";
                            }
                            else
                            {
                                student.CourseID = course.CourseID.ToString();
                                student.SubCourseID = course.SubCourseID.ToString();
                                student.CourseModeID = course.CourseModeID.ToString();
                            }
                        }
                        else
                        {
                            issues += "Course Name is required; ";
                        }
                        if (string.IsNullOrWhiteSpace(student.CourseBranchName))
                            issues += "Course Branch Name is required; ";


                        if (string.IsNullOrWhiteSpace(student.PersuingYearSemester))
                        {
                            issues += "Pursuing Year/Semester is required; ";
                        }
                        else
                        {
                            int temp;
                            if (!int.TryParse(student.PersuingYearSemester, out temp))
                            {
                                issues += "Pursuing Year/Semester must be a number; ";
                            }
                        }



                        // ✅ Add to invalid or valid list
                        if (!string.IsNullOrEmpty(issues))
                        {
                            invalidRecords.Add(new StudentExcelValidationResult
                            {
                                RowNumber = row + 1,
                                EnrollmentNumber = student.EnrollmentNumber,
                                StudentName = student.StudentName,
                                Issues = issues.TrimEnd(' ', ';')
                            });
                        }
                        else
                        {
                            validRecords.Add(student);
                        }
                    }
                }

                var resultModel = new StudentValidationViewModel
                {
                    InvalidRecords = invalidRecords,
                    ValidRecords = validRecords
                };

                return View("Uploadresult", resultModel);
            }
            catch (Exception ex)
            {
                TempData["msg"] = $"<div class='alert alert-danger'>Error reading Excel: {ex.Message}</div>";
                return RedirectToAction("UploadStudentData_new");
            }
        }


        // save valid student data


        [HttpPost]

        public ActionResult SaveValidStudents(List<StudentDataRecord> records)
        {
            foreach (var rec in records)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Enrollment: {rec.EnrollmentNumber}, Name: {rec.StudentName}, DOB: {rec.DateOfBirth}, DOA: {rec.DateOfAdmission}, Address: {rec.PermanentAddress}"
                );
            }

            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];

            if (records == null || records.Count == 0)
            {
                TempData["msg"] = @"<div class='alert alert-danger' role='alert'>No Record Updated</div>";
                return RedirectToAction("UploadStudentData_new");
            }

            // ✅ Now we get full result (insert count + duplicates)
            StudentSaveResult result = appClass.SaveValidStudents(records, ARespo.University_ID, ARespo.Session_ID);

            if (result.DuplicateEnrollments.Any())
            {
                TempData["Duplicates"] = result.DuplicateEnrollments;
                TempData["msg"] = $@"<div class='alert alert-warning' role='alert'>
                                {result.InsertedCount} new records saved. 
                                Some enrollment numbers already exist.
                             </div>";
            }
            else
            {
                TempData["msg"] = $@"<div class='alert alert-success' role='alert'>
                                {result.InsertedCount} new records saved successfully.
                             </div>";
            }

            return RedirectToAction("UploadStudentData_new");
        }



        public String DeleteDegreeFile(Int32 File_Id, string FPath)
        {
            AppClass appClass = new AppClass();
            String Msg = "";           
            bool val = appClass.DegreeFile_Delete(File_Id);            
            string FileLocated = Server.MapPath(FPath);
            if (val)
            {                
                if(System.IO.File.Exists(FileLocated))
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

        public String DeleteInternshipFile(Int32 File_Id, string FPath)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            bool val = appClass.InternshipFile_Delete(File_Id);
            string FileLocated = Server.MapPath(FPath);
            if (val)
            {
                if (System.IO.File.Exists(FileLocated))
                {
                    System.IO.File.Delete(FileLocated);
                    Msg = "File deleted Successfully !!";
                    TempData["msg"] = null;
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
        public String DeleteACFile(Int32 File_Id, string FPath)
        {
            AppClass appClass = new AppClass();
            String Msg = "";
            bool val = appClass.ACFile_Delete(File_Id);
            string FileLocated = Server.MapPath(FPath);
            if (val)
            {
                if (System.IO.File.Exists(FileLocated))
                {
                    System.IO.File.Delete(FileLocated);
                    Msg = "File deleted Successfully !!";
                    TempData["msg"] = null;
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



        [HttpGet]
        [SessionCheck]
        public ActionResult UploadDegreeData()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Degree_Files_for_University(ARespo.University_ID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UploadDegreeData(HttpPostedFileBase file)
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
                        + ARespo.University_ID + "DG_" + System.DateTime.Now.ToString("ddMMyyHHmmss") + info.Extension;
                }
                else
                {
                    USD.File_Name = UserName.ToString() + ARespo.University_ID + "DG_" + System.DateTime.Now.ToString("ddMMyyHHmmss") + info.Extension;
                }
                try
                {
                    if (info.Extension.ToLower() == ".xlsx" || info.Extension.ToLower() == ".xls")
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string UploadPath = Server.MapPath("~/UploadedDocs/IssuedDegree/" + ARespo.University_ID.ToString());
                            if (!Directory.Exists(UploadPath))
                                Directory.CreateDirectory(UploadPath);

                            string _path = Path.Combine(Server.MapPath("~/UploadedDocs/IssuedDegree/" + ARespo.University_ID.ToString()), USD.File_Name);
                            USD.File_path = "/UploadedDocs/IssuedDegree/" + ARespo.University_ID.ToString() + "/" + USD.File_Name.ToString();
                            file.SaveAs(_path);                            
                            appClass.UploadDegreeData(USD);

                            TempData["msg"] = @"<div class='alert alert-success' role='alert'>
                                        File Uploaded Successfully !! <br/> Students records will be reflected in report after processing of uploaded file</div>";
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
            return View(appClass.Degree_Files_for_University(ARespo.University_ID));
        }


        public JsonResult DeleteSpreadsheet(int FI)
        {

            return Json(1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StudentPromote()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();            
            return View();
        }

        [HttpGet]
        [SessionCheck]
        public ActionResult UploadInternshipData()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Internship_Files_of_University(ARespo.University_ID, ARespo.Session_ID));
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UploadInternshipData(HttpPostedFileBase file)
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
                        + ARespo.University_ID + "IS_" + System.DateTime.Now.ToString("ddMMyyHHmmss") + info.Extension;
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
                            string UploadPath = Server.MapPath("~/UploadedDocs/Internship/" + ARespo.University_ID.ToString());
                            if (!Directory.Exists(UploadPath))
                                Directory.CreateDirectory(UploadPath);

                            string _path = Path.Combine(Server.MapPath("~/UploadedDocs/Internship/" + ARespo.University_ID.ToString()), USD.File_Name);
                            USD.File_path = "/UploadedDocs/Internship/" + ARespo.University_ID.ToString() + "/" + USD.File_Name.ToString();
                            file.SaveAs(_path);
                            appClass.UploadInternshipData(USD);

                            TempData["msg"] = @"<div class='alert alert-success' role='alert'>
                                        File Uploaded Successfully !! <br/> Students records will be reflected in report after processing of uploaded file</div>";
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
            return View(appClass.Internship_Files_of_University(ARespo.University_ID, ARespo.Session_ID));
        }

        [HttpGet]
        [SessionCheck]
        public ActionResult UploadAcademicCalendar()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.AcademicCalendar_of_University(ARespo.University_ID, ARespo.Session_ID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UploadAcademicCalendar(HttpPostedFileBase file)
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
                        + ARespo.University_ID + "AC_" + System.DateTime.Now.ToString("ddMMyyHHmmss") + info.Extension;
                }
                else
                {
                    USD.File_Name = UserName.ToString() + ARespo.University_ID + "_" + System.DateTime.Now.ToString("dd_MM_yy_HHmmss") + info.Extension;
                }
                try
                {
                    if (info.Extension.ToLower() == ".pdf")
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string UploadPath = Server.MapPath("~/UploadedDocs/AcademicCalendar/" + ARespo.University_ID.ToString());
                            if (!Directory.Exists(UploadPath))
                                Directory.CreateDirectory(UploadPath);

                            string _path = Path.Combine(Server.MapPath("~/UploadedDocs/AcademicCalendar/" + ARespo.University_ID.ToString()), USD.File_Name);
                            USD.File_path = "/UploadedDocs/AcademicCalendar/" + ARespo.University_ID.ToString() + "/" + USD.File_Name.ToString();
                            file.SaveAs(_path);
                            appClass.UploadAcademicCalendar(USD);

                            TempData["msg"] = @"<div class='alert alert-success' role='alert'>
                                        File Uploaded Successfully !!</div>";
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
            return View(appClass.AcademicCalendar_of_University(ARespo.University_ID, ARespo.Session_ID));
        }

        public JsonResult CoursesForPromote()
        {
            AppClass AppC = new AppClass();
            String CoursesListForFee = "<option>---- Select Course ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateCourseForPromoteJR(ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CoursesListForFee += "<option CM="+ dt.Rows[i][3].ToString()+" NOSY=" + dt.Rows[i][2].ToString() + " value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][1].ToString() + " </option>";
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
                SubCourseList += "<option  value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][1].ToString() + " </option>";
            }
            return Json(SubCourseList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubCoursesForPromote(string Univ_Course_ID)
        {
            AppClass AppC = new AppClass();
            String SubCourseList = "<option>---- Select Branch ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable dt = AppC.PopulateSubCourseForPromote(Convert.ToInt32(Univ_Course_ID), ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SubCourseList += "<option  value=" + dt.Rows[i][0].ToString() + ">" + dt.Rows[i][1].ToString() + " </option>";
            }
            return Json(SubCourseList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Sem_YearList(String SC_ID)
        {
            AppClass AC = new AppClass();
            String SY_List = "<option>---- Select ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.PopulateSem_Year(Convert.ToInt32(SC_ID), ARespo.University_ID);
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                SY_List += "<option value=" + DT.Rows[i][1].ToString() + ">" + DT.Rows[i][1].ToString() + " </option>";
            }
            return Json(SY_List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Sem_YearList_Promote(String SC_ID)
        {
            AppClass AC = new AppClass();
            String SY_List = "<option>---- Select ----</option>";
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            DataTable DT = AC.PopulateSem_Year_Promote(Convert.ToInt32(SC_ID), ARespo.University_ID, ARespo.Session_ID);
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                SY_List += "<option value=" + DT.Rows[i][0].ToString() + ">" + DT.Rows[i][0].ToString() + " </option>";
            }
            return Json(SY_List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentListForPromote(int USC_ID, int SY)
        {
            AppClass AC = new AppClass();
            ModelState.Clear();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            var StudentList = AC.StudentsListForPromote(ARespo.University_ID, USC_ID, SY, ARespo.Session_ID);
            return Json(StudentList, JsonRequestBehavior.AllowGet);
        }
        public String PromoteStudentInsert(String Pmt_Str, String Pmt_Id, int NoOfPmt, String PO_Str, int NoOfPO, String DO_Str, int NoOfDO, int SC_ID)
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            String Msg = "";
            if (NoOfPmt > 0)
            {
                int val = appClass.StudentsPromoteInsert(Pmt_Str);
                if (val > 0)
                {
                   bool SP = appClass.StudentsPromoteUpdate(ARespo.Session_ID, ARespo.University_ID, SC_ID, Pmt_Id);
                    if (SP)
                    {
                        Msg = NoOfPmt + " - Students Promoted Successfully. !!";
                    }
                    else
                    {
                        Msg = NoOfPmt + " - Students flag not updated. !!";
                    }
                   
                }
                else
                {
                    Msg = "Error in promoting students !!";
                }
            }
            if (NoOfPO > 0)
            {
                bool PO = appClass.StudentsPassOutUpdate(PO_Str);
                if(PO)
                {
                    Msg = Msg + "\r\n"+ NoOfPO + " - Students Pass out Successfully. !!";
                }
                else
                {
                    Msg = Msg + "\r\nError in passing out students !!";
                }
            }
            if (NoOfDO > 0)
            {
                bool DO = appClass.StudentsDropOutUpdate(DO_Str);
                if(DO)
                {
                    Msg = Msg + "\r\n" + NoOfDO + " - Students Drop out Successfully. !!";
                }
                else
                {
                    Msg = Msg + "\r\nError in Droping out students !!";
                }
            }
            return Msg;
        }

    }
}