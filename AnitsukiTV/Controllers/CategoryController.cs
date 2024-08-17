using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    public class CategoryController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities(); 
        // GET: Category
        TabeList veri = new TabeList();
        public ActionResult Index()
        {
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();
            return View(veri);
        }
        public ActionResult UseCategory(int ? id)
        {
            var degerler = db.TBLCATEGORY.FirstOrDefault(x => x.ID == id);
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.Where(x=>x.CATEGORYID == id && x.STATUS == true).ToList();
            ViewBag.Category = degerler.CATEGORYNAME;
            return View(veri);
        }
    }
}