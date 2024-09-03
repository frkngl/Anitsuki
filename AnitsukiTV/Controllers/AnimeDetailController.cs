﻿using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
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
            bool isWatchLater = false;

            if (User.Identity.IsAuthenticated)
            {
                int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault().ID;
                var favorite = db.TBLFAVORITES.Where(x => x.USERID == userID && x.ANIMEID == id).FirstOrDefault();
                isFavorite = favorite != null;

                var watchlater = db.TBLWATCHLATER.Where(x => x.USERID == userID && x.ANIMEID == id).FirstOrDefault();
                isWatchLater = watchlater != null;
            }

            ViewBag.IsFavorite = isFavorite;
            ViewBag.isWatchLater = isWatchLater;

            return View(veri);
        }


        public PartialViewResult Comment(int id)
        {
            var degerler = db.TBLANIMECOMMENT.Where(x=>x.ANIMEID == id && x.STATUS == true).ToList();
            ViewBag.AnimeCount = degerler.Where(x=>x.STATUS == true).Count();
            ViewBag.anime = id;
            return PartialView(degerler);
        }



        public PartialViewResult LeaveComment(int id)
        {
            ViewBag.deger = id;
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult LeaveComment(TBLANIMECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLANIMECOMMENT.Add(y);
            db.SaveChanges();
            Response.Redirect("/AnimeDetail/Index/" + y.ANIMEID);
            return PartialView();
        }


        [HttpPost]
        public ActionResult ReplyComment(TBLANIMECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLANIMECOMMENT.Add(y);
            db.SaveChanges();
            return RedirectToAction("Index/" + y.ANIMEID);
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
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false });
            }

            int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault().ID;
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
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false });
            }

            int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault().ID;
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

        public PartialViewResult Comment1(int id)
        {
            var degerler = db.TBLEPISODECOMMENT.Where(x => x.EPISODEID == id && x.STATUS == true).ToList();
            ViewBag.EpisodeCount = degerler.Where(x => x.STATUS == true).Count();
            ViewBag.episode = id;
            return PartialView(degerler);
        }


        public PartialViewResult LeaveComment1(int id)
        {
            ViewBag.episode1 = id;
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult LeaveComment1(TBLEPISODECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLEPISODECOMMENT.Add(y);
            db.SaveChanges();
            Response.Redirect("/AnimeDetail/Video/" + y.EPISODEID);
            return PartialView();
        }


        [HttpPost]
        public ActionResult ReplyComment1(TBLEPISODECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();

            y.DATE = Convert.ToDateTime(DateTime.Now.ToLongDateString());
            y.STATUS = true;
            y.TBLUSER = user;
            db.TBLEPISODECOMMENT.Add(y);
            db.SaveChanges();
            return RedirectToAction("Video/" + y.EPISODEID);
        }



        [HttpPost]
        public ActionResult LikeOrUnlikeComment1(int id, int userId, bool isLike)
        {
            var yorum = db.TBLEPISODECOMMENT.Find(id);
            var like = db.TBLEPISODECOMMENTLIKE.FirstOrDefault(x => x.EPISODECOMMENTID == id && x.USERID == userId);

            if (isLike)
            {
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
            }
            else
            {
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
            }
            db.SaveChanges();

            var likeCount = db.TBLEPISODECOMMENTLIKE.Where(x => x.EPISODECOMMENTID == id && x.STATUS == true).Count();
            var unlikeCount = db.TBLEPISODECOMMENTLIKE.Where(x => x.EPISODECOMMENTID == id && x.STATUS == false).Count();

            return Json(new { likeCount, unlikeCount });
        }












        [HttpPost]
        public ActionResult LikeOrUnlikeComment2(int id, int userId, bool isLike)
        {
            var yorum = db.TBLEPISODE.Find(id);
            var like = db.TBLEPISODELIKE.FirstOrDefault(x => x.EPISODEID == id && x.USERID == userId);

            if (isLike)
            {
                if (like == null)
                {
                    like = new TBLEPISODELIKE { EPISODEID = id, USERID = userId, LIKESTATUS = true };
                    db.TBLEPISODELIKE.Add(like);
                }
                else
                {
                    like.LIKESTATUS = true;
                    db.Entry(like).State = EntityState.Modified;
                }
            }
            else
            {
                if (like == null)
                {
                    like = new TBLEPISODELIKE { EPISODEID = id, USERID = userId, LIKESTATUS = false };
                    db.TBLEPISODELIKE.Add(like);
                }
                else
                {
                    like.LIKESTATUS = false;
                    db.Entry(like).State = EntityState.Modified;
                }
            }
            db.SaveChanges();

            var likeCount = db.TBLEPISODELIKE.Where(x => x.EPISODEID == id && x.LIKESTATUS == true).Count();
            var unlikeCount = db.TBLEPISODELIKE.Where(x => x.EPISODEID == id && x.LIKESTATUS == false).Count();

            return Json(new { likeCount, unlikeCount });
        }
    }
}