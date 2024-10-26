using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    public class ErrorController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        public ActionResult NotFound()
        {
            var degerler = db.TBL404.ToList();
            return View(degerler);
        }
    }
}