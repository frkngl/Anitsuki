using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    public class HomeController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();   
        TabeList veri = new TabeList();
        public ActionResult Index()
        {
            veri.Episode = db.TBLEPISODE.ToList();
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();
            return View(veri);
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Donate()
        {
            var degerler = db.TBLDONATE.Where(x=>x.STATUS == true).OrderByDescending(x=>x.DONATE).Take(20).ToList();
            return View(degerler);
        }
    }
}