using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunday.Models;

namespace Sunday.Controllers
{
    public class HomeController : Controller
    {
        // 顺便实例化数据
        private ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View(context.SysRoles.ToList());
        }
    }
}
