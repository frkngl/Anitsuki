using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace AnitsukiTV.Controllers
{
    public class AnimeDetailController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        TabeList veri =new TabeList();
        // GET: AnimeDetail
        public ActionResult Index(int id, int ? sezonID)   
        {
            veri.Anime = db.TBLANIME.Where(x => x.ID == id).ToList();
            veri.Season = db.TBLSEASON.Where(x => x.ANIMEID == id).ToList();
            int sezonIDValue = sezonID.HasValue ? sezonID.Value : veri.Season.FirstOrDefault()?.ID ?? 0;
            veri.Episode = db.TBLEPISODE.Where(x=>x.SEASONID == sezonIDValue).ToList();
            return View(veri);
        }
        public ActionResult Video(int id)
        {
            veri.Episode = db.TBLEPISODE.Where(x => x.ID == id).ToList();
            int animeId = veri.Episode.FirstOrDefault()?.TBLANIME?.ID ?? 0;
            int sezonNumarası = veri.Episode.FirstOrDefault()?.TBLSEASON?.SEASONNUMBER ?? 0;
            int oncekiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID < id) .OrderByDescending(x => x.ID).FirstOrDefault()?.ID ?? 0;
            if (oncekiBolumId == 0)
            {
                int oncekiSezonNumarası = sezonNumarası - 1;
                oncekiBolumId = db.TBLEPISODE .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == oncekiSezonNumarası).OrderByDescending(x => x.ID).FirstOrDefault()?.ID ?? 0;
            }
            veri.OncekiBolum = db.TBLEPISODE.Where(x => x.ID == oncekiBolumId).ToList();
            int sonrakiBolumId = db.TBLEPISODE .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID > id) .OrderBy(x => x.ID).FirstOrDefault()?.ID ?? 0;
            if (sonrakiBolumId == 0)
            {
                int sonrakiSezonNumarası = sezonNumarası + 1;
                sonrakiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sonrakiSezonNumarası).OrderBy(x => x.ID) .FirstOrDefault()?.ID ?? 0;
            }
            veri.SonrakiBolum = db.TBLEPISODE.Where(x => x.ID == sonrakiBolumId).ToList();
            return View(veri);
        }
    }
}