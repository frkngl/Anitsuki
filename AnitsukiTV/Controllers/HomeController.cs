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

        [Route("")]
        public ActionResult Index()
        {
            veri.Episode = db.TBLEPISODE.ToList();
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();
            return View(veri);
        }

        [Route("gizlilik-politikasi")]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("hakkimizda")]
        public ActionResult About()
        {
            return View();
        }

        [Route("bagis")]
        public ActionResult Donate()
        {
            var degerler = db.TBLDONATE.Where(x => x.STATUS == true).OrderByDescending(x => x.DONATE).Take(20).ToList();
            return View(degerler);
        }
    }
}