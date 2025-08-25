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

    }


   
}