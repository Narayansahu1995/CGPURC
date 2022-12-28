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
    public class Office_bearerController : Controller
    {
        // GET: office_bearer
        [SessionCheck]
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            return View(appClass.Get_master_office_bearer());
        }

        [SessionCheck]
        public ActionResult Create()
        { return View(); }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]

        public ActionResult Create(master_office_bearer_Insert mob)
        {
            var imageTypes = new string[]{
                    "image/jpg",
                    "image/jpeg"
                };
            if (mob.File_Upload == null || mob.File_Upload.ContentLength == 0)
            {
                ModelState.AddModelError("Picture", "This field is required");
            }
            else if (!imageTypes.Contains(mob.File_Upload.ContentType))
            {
                ModelState.AddModelError("Picture", "Please choose either a .jpg or .jpeg image file only.");
            }

            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();

                string FileNm = "";
                if (mob.File_Upload.ContentLength > 0)
                {
                    FileInfo info = new FileInfo(mob.File_Upload.FileName);
                    if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg")
                    {
                        //  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
                        FileNm = "Bearer_" + Guid.NewGuid() + info.Extension;

                        string path = Server.MapPath("~/File_Bearer/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        mob.File_Upload.SaveAs(Server.MapPath("~/File_Bearer/") + FileNm);
                        mob.Picture = FileNm;
                        bool Val = appClass.master_office_bearer_Insert(mob);
                        if (Val)
                        {
                            TempData["Msg"] = "<script>alert('New Beared as been created successfully : " + mob.Name + " .');</script>";
                            return RedirectToAction("Index");
                        }
                        else
                        { TempData["Msg"] = "<script>alert('error while creating session.');</script>"; }
                    }
                    else { TempData["Msg"] = "<script>alert('Please Select JPEG, or JPG File only to upload Image.');</script>"; }
                }
                else { TempData["Msg"] = "<script>alert('Please Select File to upload Image of Bearer.');</script>"; }
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
                master_office_bearer_Delete mnd = new master_office_bearer_Delete()
                {
                    office_bearer_ID = id,
                    Deleted_By = ARespo.Login_ID
                };
                bool Val = appClass.master_office_bearer_Delete(mnd);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('Bearer details has been deleted successfully.');</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while deleting Bearer details.');</script>";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
    }
}