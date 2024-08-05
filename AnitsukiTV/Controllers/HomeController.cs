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
            veri.Episode=db.TBLEPISODE.ToList();
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();
            return View(veri);
        }
    }
}