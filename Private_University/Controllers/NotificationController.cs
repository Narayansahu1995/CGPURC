using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;
using Private_University.App_Code;

namespace Private_University.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        [SessionCheck]
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            return View(appClass.Get_master_notification());
        }

        [SessionCheck]
        public ActionResult Create()
        { return View(); }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]

        public ActionResult Create(master_notification_Insert mni)
        {
            var imageTypes = new string[]{
                    "image/pdf",
                    "image/jpg",
                    "image/jpeg"
                };
            if (mni.File_Upload == null || mni.File_Upload.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "This field is required");
            }
       
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();

                string FileNm = "";
                if (mni.File_Upload.ContentLength > 0)
                {
                    FileInfo info = new FileInfo(mni.File_Upload.FileName);
                    if (info.Extension.ToLower() == ".pdf" || info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg")
                    {
                        //  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
                        FileNm = "Notify_" + Guid.NewGuid() + info.Extension;

                        string path = Server.MapPath("~/File_Notification/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        mni.File_Upload.SaveAs(Server.MapPath("~/File_Notification/") + FileNm);
                        mni.Notification_File = FileNm;
                        bool Val = appClass.master_notification_Insert(mni);
                        if (Val)
                        {
                            TempData["Msg"] = "<script>alert('New notification as been created successfully : " + mni.Notification_Head + " .');</script>";
                            return RedirectToAction("Index");
                        }
                        else
                        { TempData["Msg"] = "<script>alert('error while creating session.');</script>"; }

                    }
                    else { TempData["Msg"] = "<script>alert('Please Select PDF, JPEG, or JPG File only to upload Notification.');</script>"; }
                }
                else { TempData["Msg"] = "<script>alert('Please Select File to upload Notification.');</script>"; }
            }
            // return RedirectToAction("Index", "Dashboard");
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
                master_notification_Delete mnd = new master_notification_Delete()
                {
                    Notification_ID = id,
                    Deleted_By = ARespo.Login_ID
                };
                bool Val = appClass.master_notification_Delete(mnd);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('Notification has been deleted successfully.');</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while deleting otification.');</script>";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
    }
}