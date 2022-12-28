using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;

namespace Private_University.Controllers
{
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        [SessionCheck]
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        public ActionResult Index(ChangePassword CP)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
                String UserName = '"'+ARespo.Name.ToString()+'"';
                if (appClass.CheckCurrentPWD(ARespo.Login_ID, CP))
                {
                    bool Val = appClass.UpdatePassword(CP);
                    if (Val)
                    {
                        TempData["Msg"] = "<script>alert('Password for "+ UserName.ToString() + " has been changed successfully !\\nPlease login with new password');</script>";                        
                        return RedirectToAction("Index", "Home");                        

                    }
                    else
                    {
                        TempData["Msg"] = "<script>alert('Error while changing login password for "+ UserName.ToString() + " !');</script>";                        
                        //return RedirectToAction("Index", "Dashboard");
                    }
                         
                }
                else
                {                    
                    TempData["Msg"] = "<script>alert('Current Password is wrong of " + UserName.ToString() + " !');</script>";
                   
                }
            }            
            return View();
        }


        
    }

 }
