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

namespace Private_University.Controllers
{
    public class StudentController : Controller
    {
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
                        student_promote sp = new student_promote()
                        {
                            Student_Id = Val,
                            Session_ID = std.Session_ID,
                            Semester_Year = std.Semester_Year,
                            Is_Passout = 0,
                            Result = 1,
                            Promoted_By = std.Entry_By
                        };

                        appClass.insert_student_details_promote(sp);

                        TempData["Msg"] = "<script>alert('Student " + std.Student_Name + " Data Save Successfully.');</script>";
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
            DataTable Sdt = AppC.Get_Student_Detail(Student_ID, ARespo.University_ID);
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
        public ActionResult StudentsRpt()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            ViewBag.CourseList = appClass.PopulateCourse(ARespo.University_ID);
            ViewBag.GenderList = appClass.GenderList();
           
            return View(appClass.Get_Students_Records(ARespo.University_ID));
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
            return RedirectToAction("StudentsRpt");
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
            return RedirectToAction("StudentsRpt");
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
            return View(appClass.Get_Files_List_for_University(ARespo.University_ID));
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
                                        File Uploaded Successfully !! </div>";                       
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
            return View(appClass.Get_Files_List_for_University(ARespo.University_ID));
        }

        public JsonResult DeleteSpreadsheet(int FI)
        {

            return Json(1, JsonRequestBehavior.AllowGet);
        }

     }
}