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
    public class NewsController : Controller
    {
        // GET: News
        public ActionResult Index()
        {
            AppClass appClass = new AppClass();
            ModelState.Clear();
            return View(appClass.Get_News());
        }

        [SessionCheck]
        public ActionResult AddNews()
        {
            AppClass appClass = new AppClass();
            AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
            ModelState.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheck]
        [ValidateInput(false)]
        public ActionResult AddNews(News_Insert news)
        {
            var imageTypes = new string[]{
                    "image/jpg",          
                    "image/jpeg",         
                    "application/pdf"     
                                          
                };                         
            if (news.File_Upload == null || news.File_Upload.ContentLength == 0)
            {
                ModelState.AddModelError("File", "This field is required");
            }
            else if (!imageTypes.Contains(news.File_Upload.ContentType))
            {
                ModelState.AddModelError("File", "Please choose either a .jpg or .jpeg or .pdf image file only.");
            }

            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();

                string FileNm = "";
                if (news.File_Upload.ContentLength > 0)
                {
                    FileInfo info = new FileInfo(news.File_Upload.FileName);
                    if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg" || info.Extension.ToLower() == ".pdf")
                    {
                        //  if (mni.file.PostedFile.ContentLength > 3145728) throw new Exception("File Size for EM should be less than 2 MB");
                        FileNm = "News_" + Guid.NewGuid() + info.Extension;

                        string path = Server.MapPath("~/File_News/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        news.File_Upload.SaveAs(Server.MapPath("~/File_News/") + FileNm);
                        news.News_File = FileNm;
                        bool Val = appClass.News_Insert(news);
                        if (Val)
                        {
                            TempData["Msg"] = "<script>alert('New News Upload successfully : " + news.News_Title + " .');</script>";
                            return RedirectToAction("Index");
                        }
                        else
                        { TempData["Msg"] = "<script>alert('error while creating session.');</script>"; }
                    }
                    else { TempData["Msg"] = "<script>alert('Please Select JPEG, or JPG or PDF File only to upload Image.');</script>"; }
                }
                else { TempData["Msg"] = "<script>alert('Please Select File to upload Image of Gallery.');</script>"; }
            }
            // return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [SessionCheck]
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                AppClass appClass = new AppClass();
                AuthenticationResponse ARespo = (AuthenticationResponse)Session["AuthResponse"];
                News_Delete news = new News_Delete()
                {
                    news_ID = id,
                    Deleted_By = ARespo.Login_ID
                };
                bool Val = appClass.News_Delete(news);
                if (Val)
                {
                    TempData["Msg"] = "<script>alert('News details has been deleted successfully.');</script>";
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