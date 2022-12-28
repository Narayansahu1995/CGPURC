using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Private_University.Models
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session == null || filterContext.HttpContext.Session["AuthResponse"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/Home/Index");
            }
        }
    }
}