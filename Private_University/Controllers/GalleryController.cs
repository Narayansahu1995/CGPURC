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
    public class GalleryController : Controller
    {

        // GET: Gallery
        [SessionCheck]
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            return View(appClass.Get_Gallery());
        }

        [SessionCheck]
        public ActionResult AddGallery()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]

        public ActionResult AddGallery(Gallery_Insert gallery)
        {
            var imageTypes = new string[]{
                    "image/jpg",
                    "image/jpeg"
                };
            if (gallery.File_Upload == null || gallery.File_Upload.ContentLength == 0)
            {
                ModelState.AddModelError("Picture", "This field is required");
            }
            else if (!imageTypes.Contains(gallery.File_Upload.ContentType))
            {
                ModelState.AddModelError("Picture", "Please choose either a .jpg or .jpeg image file only.");
            }

            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();

                string FileNm = "";
                if (gallery.File_Upload.ContentLength > 0)
                {
                    FileInfo info = new FileInfo(gallery.File_Upload.FileName);
                    if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg")
                    {
                        //  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
                        FileNm = "Gallery_" + Guid.NewGuid() + info.Extension;

                        string path = Server.MapPath("~/File_Gallery/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        gallery.File_Upload.SaveAs(Server.MapPath("~/File_Gallery/") + FileNm);
                        gallery.Picture = FileNm;
                        bool Val = appClass.Gallery_Insert(gallery);
                        if (Val)
                        {
                            TempData["Msg"] = "<script>alert('New Photo Upload successfully : " + gallery.Name + " .');</script>";
                            return RedirectToAction("Index");
                        }
                        else
                        { TempData["Msg"] = "<script>alert('error while creating session.');</script>"; }
                    }
                    else { TempData["Msg"] = "<script>alert('Please Select JPEG, or JPG File only to upload Image.');</script>"; }
                }
                else { TempData["Msg"] = "<script>alert('Please Select File to upload Image of Gallery.');</script>"; }
            }
            // return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [SessionCheck]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
                Gallery_Delete gallery = new Gallery_Delete()
                {
                    photo_ID = id,
                    Deleted_By = ARespo.Login_ID
                };
                bool Val = appClass.Gallery_Delete(gallery);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('Photo Gallery details has been deleted successfully.');</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Msg"] = "<script>alert('error while deleting Photo details.');</script>";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
    }
}