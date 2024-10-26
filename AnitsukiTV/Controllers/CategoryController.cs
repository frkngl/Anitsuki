using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        [Route("animeler")]
        public ActionResult Index()
        {
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();
            return View(veri);
        }

        [Route("animeler/{kategoriID}/{kategoriName}-izle")]
        public ActionResult UseCategory(int kategoriID, string kategoriName)
        {
            var degerler = db.TBLCATEGORY.FirstOrDefault(x => x.ID == kategoriID);
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.Where(x => x.CATEGORYID == kategoriID && x.STATUS == true).OrderBy(x => Guid.NewGuid()).ToList();
            ViewBag.Category = degerler.CATEGORYNAME;
            return View(veri);
        }

    }
}