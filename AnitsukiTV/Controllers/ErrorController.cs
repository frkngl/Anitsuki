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

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 20);
            DateTime endDate = new DateTime(DateTime.Now.Year + (DateTime.Now.Month == 1 && DateTime.Now.Day <= 5 ? 0 : 1), 1, 5);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(degerler);
        }
    }
}