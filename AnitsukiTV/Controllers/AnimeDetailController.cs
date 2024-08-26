using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;
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
            var degerler2 = db.TBLANIME.FirstOrDefault(x => x.ID == id);
            ViewBag.anime = degerler2?.ANIMENAME;

            var degerler = db.TBLSEASON.FirstOrDefault(x => x.SEASONNUMBER == id);
            veri.Anime = db.TBLANIME.Where(x => x.ID == id).ToList();
            veri.Season = db.TBLSEASON.Where(x => x.ANIMEID == id).ToList();
            int sezonIDValue = sezonID.HasValue ? sezonID.Value : veri.Season.FirstOrDefault()?.ID ?? 0;
            veri.Episode = db.TBLEPISODE.Where(x => x.SEASONID == sezonIDValue).ToList();
            
            var selectedSeason = db.TBLSEASON.FirstOrDefault(x => x.ID == sezonIDValue);
            ViewBag.Season = selectedSeason?.SEASONNAME ?? $"{selectedSeason?.SEASONNUMBER} Sezon";

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


        public PartialViewResult Comment(int id)
        {
            var anime = db.TBLANIMECOMMENT.Where(x => x.ANIMEID == id).FirstOrDefault();
            var degerler = db.TBLANIMECOMMENT.Where(x => x.ANIMEID == id).ToList();
            ViewBag.CommentCount = degerler.Count;
            ViewBag.animeID = anime.ANIMEID;
            return PartialView(degerler);
        }

        [HttpPost]
        public ActionResult LeaveComment(TBLANIMECOMMENT y)
        {
            if (Session["username"] == null)
            {
                return Json(new { success = false, message = "Lütfen giriş yapın veya kayıt olun." });
            }

            try
            {
                TBLANIMECOMMENT yeni = new TBLANIMECOMMENT();
                yeni.COMMENT = y.COMMENT;
                yeni.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                yeni.USTID = null;
                yeni.STATUS = true;
                yeni.ANIMEID = y.ANIMEID;
                yeni.USERID = (int)Session["id"]; 

                // Veritabanından kullanıcı bilgilerini al
                var user = db.TBLUSER.Find(yeni.USERID);
                string username = user.USERNAME; 
                string picture = "/IMG/" + user.PICTURE; 

                db.TBLANIMECOMMENT.Add(yeni);
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    message = "Yorumunuz başarıyla kaydedildi.",
                    commentText = yeni.COMMENT,
                    username = username,
                    date = DateTime.Now.ToString("dd.MM.yyyy"),
                    picture = picture
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Yorumunuz kaydedilemedi. Lütfen tekrar deneyin." });
            }
        }


        [HttpPost]
        public ActionResult LikeOrUnlikeComment(int id, int userId, bool isLike)
        {
            var yorum = db.TBLANIMECOMMENT.Find(id);
            var like = db.TBLANIMECOMMENTLIKE.FirstOrDefault(x => x.ANIMECOMMENTID == id && x.USERID == userId);

            if (isLike)
            {
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
            }
            else
            {
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
            }
            db.SaveChanges();

            var likeCount = db.TBLANIMECOMMENTLIKE.Where(x => x.ANIMECOMMENTID == id && x.STATUS == true).Count();
            var unlikeCount = db.TBLANIMECOMMENTLIKE.Where(x => x.ANIMECOMMENTID == id && x.STATUS == false).Count();

            return Json(new { likeCount, unlikeCount });
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
                return Json(new { success = true, isFavorite = true, message = "Anime favorilere eklendi" });
            }
            else
            {
                db.TBLFAVORITES.Remove(favorite);
                db.SaveChanges();
                return Json(new { success = true, isFavorite = false, message = "Anime favorilerden çıkarıldı" });
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
                return Json(new { success = true, isWatchLater = true, message = "Anime izleme listesine eklendi" });
            }
            else
            {
                db.TBLWATCHLATER.Remove(watchlater);
                db.SaveChanges();
                return Json(new { success = true, isWatchLater = false, message = "Anime izleme listesinden çıkarıldı" });
            }
        }




        public ActionResult Video(int id)
        {
            veri.Episode = db.TBLEPISODE.Where(x => x.ID == id && x.STATUS == true).ToList();
            int animeId = veri.Episode.FirstOrDefault()?.TBLANIME?.ID ?? 0;
            int sezonNumarası = veri.Episode.FirstOrDefault()?.TBLSEASON?.SEASONNUMBER ?? 0;
            int oncekiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID < id).OrderByDescending(x => x.ID).FirstOrDefault()?.ID ?? 0;
            if (oncekiBolumId == 0)
            {
                int oncekiSezonNumarası = sezonNumarası - 1;
                oncekiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == oncekiSezonNumarası).OrderByDescending(x => x.ID).FirstOrDefault()?.ID ?? 0;
            }
            veri.OncekiBolum = db.TBLEPISODE.Where(x => x.ID == oncekiBolumId).ToList();
            int sonrakiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID > id).OrderBy(x => x.ID).FirstOrDefault()?.ID ?? 0;
            if (sonrakiBolumId == 0)
            {
                int sonrakiSezonNumarası = sezonNumarası + 1;
                sonrakiBolumId = db.TBLEPISODE.Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sonrakiSezonNumarası).OrderBy(x => x.ID).FirstOrDefault()?.ID ?? 0;
            }
            veri.SonrakiBolum = db.TBLEPISODE.Where(x => x.ID == sonrakiBolumId).ToList();

            var tabeList = new TabeList
            {
                // ... existing properties ...
                VideoInfo = new VideoViewModel
                {
                    Episode = veri.Episode,
                    AnimeTitle = veri.Episode.FirstOrDefault()?.TBLANIME?.ANIMENAME,
                    SeasonNumber = veri.Episode.FirstOrDefault()?.TBLSEASON?.SEASONNUMBER ?? 0,
                    EpisodeNumber = int.Parse(veri.Episode.FirstOrDefault()?.EPINUMBER ?? "0"),
                    OncekiBolum = veri.OncekiBolum,
                    SonrakiBolum = veri.SonrakiBolum
                }
            };

            return View(tabeList);
        }

        public PartialViewResult Comments1(int id)
        {
            var degerler = db.TBLEPISODECOMMENT.Where(x => x.EPISODEID == id && x.STATUS == true).OrderByDescending(x => x.DATE).ToList();
            ViewBag.animecom = id;
            ViewBag.CommentCount = db.TBLEPISODECOMMENT.Where(x => x.EPISODEID == id && x.STATUS == true).Take(1000).Count();
            return PartialView(degerler);
        }
        


        public ActionResult LikeComment1(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var comment = db.TBLEPISODECOMMENT.Find(id);
            var like = db.TBLEPISODECOMMENTLIKE.FirstOrDefault(x => x.EPISODECOMMENTID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLEPISODECOMMENTLIKE { EPISODECOMMENTID = id, USERID = userId, STATUS = true };
                db.TBLEPISODECOMMENTLIKE.Add(like);
            }
            else
            {
                like.STATUS = true;
                db.Entry(like).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Video", new { id = comment.EPISODEID });
        }



        public ActionResult UnlikeComment1(int id)
        {
            var user = HttpContext.User.Identity.Name;
            int userId = db.TBLUSER.Where(u => u.USERNAME == user).Select(u => u.ID).FirstOrDefault();
            var comment = db.TBLEPISODECOMMENT.Find(id);
            var like = db.TBLEPISODECOMMENTLIKE.FirstOrDefault(x => x.EPISODECOMMENTID == id && x.USERID == userId);

            if (like == null)
            {
                like = new TBLEPISODECOMMENTLIKE { EPISODECOMMENTID = id, USERID = userId, STATUS = false };
                db.TBLEPISODECOMMENTLIKE.Add(like);
            }
            else
            {
                like.STATUS = false;
                db.Entry(like).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Video", new { id = comment.EPISODEID });
        }




        public PartialViewResult LeaveComment1(int? id)
        {
            if (id.HasValue)
            {
                ViewBag.deger = id.Value;
            }
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult LeaveComment1(TBLEPISODECOMMENT y)
        {
            int userId = Convert.ToInt32(Session["id"]);
            TBLUSER user = db.TBLUSER.Find(userId);

            TBLEPISODECOMMENT yeni = new TBLEPISODECOMMENT();
            yeni.COMMENT = y.COMMENT;
            yeni.EPISODEID = y.EPISODEID;
            yeni.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            yeni.STATUS = true;
            yeni.USTID = null;
            yeni.USERID = userId;

            db.TBLEPISODECOMMENT.Add(yeni);
            db.SaveChanges();

            Response.Redirect("/AnimeDetail/Video/" + y.EPISODEID);
            return PartialView();
        }

        [HttpPost]
        public ActionResult ReplyComment1(TBLEPISODECOMMENT y)
        {
            int userId = Convert.ToInt32(Session["id"]);
            TBLUSER user = db.TBLUSER.Find(userId);

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLEPISODECOMMENT.Add(y);
            db.SaveChanges();
            return RedirectToAction("Video/" + y.EPISODEID);
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