using System;
using System.Collections.Generic;
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
    public class CommissionController : Controller
    {
        FunctionClass Fnc = new FunctionClass();
        [SessionCheck]
        public ActionResult AddUsers()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
           
            ViewBag.UniversityList = appClass.PopulateMasterUniversity();
            return View();
        }

        public ActionResult UniversityUserList()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.Get_University_Users());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult AddUsers(AddUsers user)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                bool Val = appClass.AddUser_Insert(user);
                if (Val)
                {
                    FunctionClass fn = new FunctionClass();
                    string Message = "Dear Sir/Ma'am, your ID has been created for PURC User id : "+user.UserName+" , Password : "+user.Password;
                    fn.SendEmail(user.Email_ID, "Credential Created Successfully", Message, true);
                    fn.SendSMS_T(user.Mobile_Number, "", Message);

                    TempData["Msg"] = "<script>alert('New credentials has been created successfully for " + user.Name +" Login ID : "+user.UserName+" and Password : " + user.Password + " .');</script>";
                    return RedirectToAction("AddUsers");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while creating New University.');</script>";
                }
            }
            return View();
        }

        [SessionCheck]
        public ActionResult AddMasterUniversity()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult AddMasterUniversity(MasterUniversity uni)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                bool Val = appClass.AddMasterUniversity_Insert(uni);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('New User" + uni.University_Name + " as been created successfully.');</script>";
                    return RedirectToAction("AddMasterUniversity");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while creating New University.');</script>";
                }
            }
            return View();
        }
        public ActionResult UniversityList()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.GetUniversityShowHome());
        }


        public ActionResult AddAddress()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult AddAddress(CommissionAddress address)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                bool Val = appClass.Commission_Address(address);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('Address Inserted successfully.');</script>";
                    return RedirectToAction("AddAddress");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while..');</script>";
                }
            }
            return View();
        }

        public ActionResult UpdateAddress()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            CommissionAddressUpdate com = appClass.GetCommissionAddress();
            ViewBag.Addressss = com.Addressss;
            ViewBag.Contact_No = com.Contact_No;
            ViewBag.Email_ID = com.Email_ID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult UpdateAddress(CommissionAddressUpdate address)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                ViewBag.Addressss = address.Addressss;
                ViewBag.Contact_No = address.Contact_No;
                ViewBag.Email_ID = address.Email_ID;
                bool Val = appClass.Commission_Address_Update(address);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('Address Update successfully.');</script>";
                    return RedirectToAction("UpdateAddress");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while..');</script>";
                }
            }
            return View();
        }


        [HttpGet]
        [SessionCheck]
        public ActionResult WorkReport()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View(appClass.GetUniversityWorkStatus());
        }

        [HttpGet]
        [SessionCheck]
        public ActionResult StudentEnrolledReport()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ViewBag.SessionID = ARespo?.Session_ID;
            ModelState.Clear();
            return View(appClass.UniversityEnrolledStudentData(ARespo.Session_ID));
        }



        [HttpGet]
        [SessionCheck]
        public ActionResult StudentFeesReport()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            int year;

            // Map session_ID to year
            switch (ARespo.Session_ID)
            {
                case 6:
                    year = 2025;
                    break;
                case 5:
                    year = 2024;
                    break;
                case 4:
                    year = 2023;
                    break;
                case 3:
                    year = 2022;
                    break;
                default:
                    year = DateTime.Now.Year; // fallback
                    break;
            }

            ModelState.Clear();

            // Get month-wise fees report for the correct year
            var list = appClass.GetFeesReportByMonth(year);

            // Prepare month headers dynamically for Razor table
            ViewBag.Months = new List<string>
    {
        $"Jan-{year}", $"Feb-{year}", $"Mar-{year}", $"Apr-{year}",
        $"May-{year}", $"Jun-{year}", $"Jul-{year}", $"Aug-{year}",
        $"Sep-{year}", $"Oct-{year}", $"Nov-{year}", $"Dec-{year}"
    };

            return View(list);
        }


    }



}