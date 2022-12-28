using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.Models;

namespace Private_University.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
       [SessionCheck]
        public ActionResult Index()
        {
            return View();
        }
    }
}