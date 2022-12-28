using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Private_University.App_Code;
using System.IO;
using CaptchaMvc.HtmlHelpers;
using Private_University.Models;

namespace Private_University.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public ActionResult Index(String LanguageAbbrevation)
        {
            ViewBag.Title = "Home Page";
            AppClass appClass = new AppClass();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            ModelState.Clear();
            if (LanguageAbbrevation != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguageAbbrevation);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageAbbrevation);
            }
            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = LanguageAbbrevation;
            ViewBag.lang = cookie.Value;
            TempData["lang"] = cookie.Value;
            Response.Cookies.Add(cookie);
            return View(appClass.GetNewsShowHomeMarquee());
        }

        //public ActionResult Change(String LanguageAbbrevation)
        //{
        //    AppClass appClass = new AppClass();
        //    News com = appClass.GetNewsForLanguageChange();
        //    ViewBag.news_ID = com.news_ID;
        //    ViewBag.News_Title = com.News_Title;
        //    if (LanguageAbbrevation != null)
        //    {
        //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguageAbbrevation);
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageAbbrevation);
        //    }

        //    HttpCookie cookie = new HttpCookie("Language");
        //    cookie.Value = LanguageAbbrevation;
        //    Response.Cookies.Add(cookie);

        //    return View("Index");
        //}
    }
}