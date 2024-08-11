using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ActionResult Index(int? id, int? sezonID)
        {
            veri.Anime = db.TBLANIME.Where(x => x.ID == id).ToList();
            veri.Season = db.TBLSEASON.Where(x => x.ANIMEID == id).ToList();
            int sezonIDValue = sezonID.HasValue ? sezonID.Value : veri.Season.FirstOrDefault()?.ID ?? 0;
            veri.Episode = db.TBLEPISODE.Where(x => x.SEASONID == sezonIDValue).ToList();

            bool isFavorite = false;
            if (Session["id"] != null)
            {
                int userID = (int)Session["id"];
                var favorite = db.TBLFAVORITES.Where(x => x.USERID == userID && x.ANIMEID == id).FirstOrDefault();
                isFavorite = favorite != null;
            }
            ViewBag.IsFavorite = isFavorite;

            bool isWatchLater = false;
            if (Session["id"] != null)
            {
                int userID = (int)Session["id"];
                var watchlater = db.TBLWATCHLATER.Where(x => x.USERID == userID && x.ANIMEID == id).FirstOrDefault();
                isWatchLater = watchlater != null;
            }
            ViewBag.isWatchLater = isWatchLater;

            return View(veri);
        }


        public PartialViewResult Comments(int id)
        {
            var degerler = db.TBLANIMECOMMENT.Where(x => x.ANIMEID == id && x.STATUS == true).ToList();
            ViewBag.animecom = id;
            ViewBag.CommentCount = db.TBLANIMECOMMENT.Where(x => x.ANIMEID == id && x.STATUS == true).Count();
            return PartialView(degerler);
        }


        public PartialViewResult LeaveComment(int ? id)
        {
            if (id.HasValue)
            {
                ViewBag.deger = id.Value;
            }
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult LeaveComment(TBLANIMECOMMENT y)
        {
            int userId = Convert.ToInt32(Session["id"]);
            TBLUSER user = db.TBLUSER.Find(userId);

            TBLANIMECOMMENT yeni = new TBLANIMECOMMENT();
            yeni.COMMENT = y.COMMENT;
            yeni.ANIMEID = y.ANIMEID;
            yeni.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            yeni.STATUS = true;
            yeni.USTID = null;
            yeni.USERID = userId;

            db.TBLANIMECOMMENT.Add(yeni);
            db.SaveChanges();

            Response.Redirect("/AnimeDetail/Index/" + y.ANIMEID);
            return PartialView();
        }

        [HttpPost]
        public ActionResult ReplyComment(TBLANIMECOMMENT y)
        {
            int userId = Convert.ToInt32(Session["id"]);
            TBLUSER user = db.TBLUSER.Find(userId);

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLANIMECOMMENT.Add(y);
            db.SaveChanges();
            return RedirectToAction("Index/" + y.ANIMEID);
        }

        public ActionResult LikeComment(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var comment = db.TBLANIMECOMMENT.Find(id);
            var like = db.TBLANIMECOMMENTLIKE.FirstOrDefault(x => x.ANIMECOMMENTID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLANIMECOMMENTLIKE { ANIMECOMMENTID = id, USERID = userId, STATUS = true };
                db.TBLANIMECOMMENTLIKE.Add(like);
            }
            else
            {
                like.STATUS = true;
                db.Entry(like).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = comment.ANIMEID });
        }

        public ActionResult UnlikeComment(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var comment = db.TBLANIMECOMMENT.Find(id);
            var like = db.TBLANIMECOMMENTLIKE.FirstOrDefault(x => x.ANIMECOMMENTID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLANIMECOMMENTLIKE { ANIMECOMMENTID = id, USERID = userId, STATUS = false };
                db.TBLANIMECOMMENTLIKE.Add(like);
            }
            else
            {
                like.STATUS = false;
                db.Entry(like).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = comment.ANIMEID });
        }










        [HttpPost]
        public ActionResult AddFavorite(int animeID)
        {
            if (Session["id"] == null)
            {
                return Json(new { success = false });
            }

            int userID = (int)Session["id"];
            var favorite = db.TBLFAVORITES.Where(x => x.USERID == userID && x.ANIMEID == animeID).FirstOrDefault();

            if (favorite == null)
            {
                db.TBLFAVORITES.Add(new TBLFAVORITES { USERID = userID, ANIMEID = animeID });
                db.SaveChanges();
                return Json(new { success = true, isFavorite = true });
            }
            else
            {
                db.TBLFAVORITES.Remove(favorite);
                db.SaveChanges();
                return Json(new { success = true, isFavorite = false });
            }
        }



        [HttpPost]
        public ActionResult WatchLater(int animeID)
        {
            if (Session["id"] == null)
            {
                return Json(new { success = false });
            }

            int userID = (int)Session["id"];
            var watchlater = db.TBLWATCHLATER.Where(x => x.USERID == userID && x.ANIMEID == animeID).FirstOrDefault();

            if (watchlater == null)
            {
                db.TBLWATCHLATER.Add(new TBLWATCHLATER { USERID = userID, ANIMEID = animeID });
                db.SaveChanges();
                return Json(new { success = true, isWatchLater = true });
            }
            else
            {
                db.TBLWATCHLATER.Remove(watchlater);
                db.SaveChanges();
                return Json(new { success = true, isWatchLater = false });
            }
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

        public ActionResult LikeEpisode(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var episode = db.TBLEPISODE.Find(id);
            var like = db.TBLEPISODELIKE.FirstOrDefault(x => x.EPISODEID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLEPISODELIKE { EPISODEID = id, USERID = userId, LIKESTATUS = true };
                db.TBLEPISODELIKE.Add(like);
            }
            else
            {
                // Kullanıcı zaten beğenme veya beğenmeme seçeneğini seçti, tekrar seçemez
                return RedirectToAction("Video", new { id = id });
            }
            db.SaveChanges();
            return RedirectToAction("Video", new { id = id });
        }

        public ActionResult UnlikeEpisode(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var episode = db.TBLEPISODE.Find(id);
            var like = db.TBLEPISODELIKE.FirstOrDefault(x => x.EPISODEID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLEPISODELIKE { EPISODEID = id, USERID = userId, LIKESTATUS = false };
                db.TBLEPISODELIKE.Add(like);
            }
            else
            {
                // Kullanıcı zaten beğenme veya beğenmeme seçeneğini seçti, tekrar seçemez
                return RedirectToAction("Video", new { id = id });
            }
            db.SaveChanges();
            return RedirectToAction("Video", new { id = id });
        }
    }
}