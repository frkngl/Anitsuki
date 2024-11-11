﻿using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using PagedList;
using PagedList.Mvc;
using System.Web.UI.WebControls;

namespace AnitsukiTV.Controllers
{
    [Authorize]
    public class LanetlerKralıAdminController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        TabeList veri = new TabeList();
        // GET: LanetlerKralıAdmin
        public ActionResult Index()
        {
            veri.Anime = db.TBLANIME.ToList();
            veri.Episode = db.TBLEPISODE.ToList();
            veri.Users = db.TBLUSER.ToList();
            ViewBag.animecount = veri.Anime.Count(x=>x.STATUS == true);
            ViewBag.episodecount = veri.Episode.Count(x=>x.STATUS == true);
            ViewBag.usercounttrue = db.TBLUSER.Count(x=>x.STATUS == true);
            ViewBag.usercountfalse = db.TBLUSER.Count(x => x.STATUS == false);
            return View(veri);
        }
        
        public ActionResult Admin()
        {
            var degerler = db.TBLADMIN.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAdmin(TBLADMIN add)
        {
            TBLADMIN admin = new TBLADMIN();
            admin.USERNAME = add.USERNAME;
            admin.PASSWORD = add.PASSWORD;
            admin.ADMINROLEID = add.ADMINROLEID;
            admin.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            admin.STATUS = true;
            db.TBLADMIN.Add(admin);
            db.SaveChanges();
            TempData["success"] = "Admin başarıyla eklendi";
            return RedirectToAction("Admin");
        }
        
        public ActionResult UpdateAdmin(int id)
        {
            var FindAdmin = db.TBLADMIN.Find(id);
            return View("UpdateAdmin", FindAdmin);
        }

        public ActionResult AdminSave(TBLADMIN admin)
        {
            var updateadmin = db.TBLADMIN.Find(admin.ID);
            updateadmin.USERNAME = admin.USERNAME;
            updateadmin.PASSWORD = admin.PASSWORD;
            updateadmin.ADMINROLEID=admin.ADMINROLEID;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Admin");
        }

        public ActionResult ActivePassive(int id)
        {
            var AdminActivePassive = db.TBLADMIN.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Admin");
        }

        public ActionResult Delete(int id)
        {
            var AdminFind = db.TBLADMIN.Find(id);
            db.TBLADMIN.Remove(AdminFind);
            db.SaveChanges();
            TempData["error"] = "Admin başarıyla silindi";
            return RedirectToAction("Admin");
        }


        public ActionResult AdminRole()
        {
            var degerler = db.TBLADMINROLE.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRole(TBLADMINROLE add)
        {
            TBLADMINROLE admin = new TBLADMINROLE();
            admin.ROLENAME = add.ROLENAME;
            db.TBLADMINROLE.Add(admin);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("AdminRole");
        }


        public ActionResult UpdateRole(int id)
        {
            var FindRole = db.TBLADMINROLE.Find(id);
            return View("UpdateRole", FindRole);
        }


        public ActionResult RoleSave(TBLADMINROLE role)
        {
            var updaterole = db.TBLADMINROLE.Find(role.ID);
            updaterole.ROLENAME = role.ROLENAME;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("AdminRole");
        }



        public ActionResult Category()
        {
            var degerler = db.TBLCATEGORY.ToList();
            return View(degerler);
        }


        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddCategory(TBLCATEGORY add)
        {
            TBLCATEGORY category = new TBLCATEGORY();
            category.CATEGORYNAME = add.CATEGORYNAME;
            category.STATUS = true;
            db.TBLCATEGORY.Add(category);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("Category");
        }


        public ActionResult UpdateCategory(int id)
        {
            var FindCategory = db.TBLCATEGORY.Find(id);
            return View("UpdateCategory", FindCategory);
        }

        public ActionResult CategorySave(TBLCATEGORY cat)
        {
            var updatecategory = db.TBLCATEGORY.Find(cat.ID);
            updatecategory.CATEGORYNAME = cat.CATEGORYNAME;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Category");
        }

        public ActionResult ActivePassive8(int id)
        {
            var AdminActivePassive = db.TBLCATEGORY.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Category");
        }



        public ActionResult Anime(string anime, int p = 1)
        {
            var degerler = from a in db.TBLANIME select a;
            if (!string.IsNullOrEmpty(anime))
            {
                degerler = degerler.Where(x => x.ANIMENAME.ToLower().Contains(anime.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLANIME>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Anime = anime; 
            return View(pagedList);
        }


        [HttpGet]
        public ActionResult AddAnime()
        {
            ViewBag.Categories = db.TBLCATEGORY.Where(x=>x.STATUS == true).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddAnime(TBLANIME add, HttpPostedFileBase Image, HttpPostedFileBase Banner, HttpPostedFileBase Video)
        {
            TBLANIME anime = new TBLANIME();
            anime.ANIMENAME = add.ANIMENAME;
            anime.DETAIL = add.DETAIL;
            anime.IMDB = add.IMDB;
            anime.STATUS = false;
            anime.ADMINSTATUS = false;
            anime.TYPE = add.TYPE;
            anime.DATE = add.DATE;
            anime.MYANIMELIST = add.MYANIMELIST;
            anime.CATEGORYID = add.CATEGORYID;

            if (Image != null && Image.ContentLength > 0)
            {
                // Resim yükleme ve anime.BANNER ayarla
                var file = new FileInfo(Image.FileName);
                var fileName = Path.GetFileName(Image.FileName);
                fileName = Guid.NewGuid().ToString() + file.Extension;
                var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                WebImage rr = new WebImage(Image.InputStream);

                if (rr.Width > 1000)
                    rr.Resize(1000, 1000);
                rr.Save(path);

                anime.BANNER = fileName;
            }
            else
            {
                anime.BANNER = null;
            }

            if (Banner != null && Banner.ContentLength > 0)
            {
                // Banner yükleme ve anime.BIGBANNER ayarla
                string yol = Path.Combine("~/IMG/" + Banner.FileName);
                Banner.SaveAs(Server.MapPath(yol));
                anime.BIGBANNER = Banner.FileName.ToString();
            }
            else
            {
                anime.BIGBANNER = null;
            }

            if (Video != null && Video.ContentLength > 0)
            {
                // Video yükleme ve anime.EDIT ayarla
                string videoYol = Path.Combine("~/Videos/" + Video.FileName);
                Video.SaveAs(Server.MapPath(videoYol));
                anime.EDIT = Video.FileName.ToString();
            }
            else
            {
                anime.EDIT = null;
            }

            db.TBLANIME.Add(anime);
            db.SaveChanges();
            TempData["add"] = "success";
            return RedirectToAction("Anime");
        }

        public ActionResult UpdateAnime(int id)
        {
            var FindAnime = db.TBLANIME.Find(id);
            var category = db.TBLCATEGORY.Find(FindAnime.CATEGORYID);
            ViewBag.CategoryName = db.TBLCATEGORY.Find(FindAnime.CATEGORYID)?.CATEGORYNAME;
            ViewBag.Categories = db.TBLCATEGORY.Where(x=>x.STATUS == true).ToList();
            return View("UpdateAnime", FindAnime);
        }

        [HttpPost]
        public ActionResult AnimeSave(TBLANIME update, HttpPostedFileBase Image, HttpPostedFileBase Banner, HttpPostedFileBase Video)
        {
            var updateanime1 = db.TBLANIME.Find(update.ID);

            if (Image != null && Image.ContentLength > 0)
            {
                if (Image.ContentType == "image/jpeg" || Image.ContentType == "image/jpg" || Image.ContentType == "image/png" || Image.ContentType == "image/jfif")
                {
                    var anipath = Request.MapPath("~/IMG/" + updateanime1.BANNER);
                    if (System.IO.File.Exists(anipath))
                    {
                        System.IO.File.Delete(anipath);
                    }
                    var file = new FileInfo(Image.FileName);
                    var fileName = Path.GetFileName(Image.FileName);
                    fileName = Guid.NewGuid().ToString() + file.Extension;
                    var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                    try
                    {
                        WebImage rr = new WebImage(Image.InputStream);
                        if (rr.Width > 1000)
                            rr.Resize(1000, 1000);
                        rr.Save(path);
                        updateanime1.BANNER = fileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Error uploading image: " + ex.Message;
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "Invalid image file type";
                    return View();
                }
            }

            if (Banner != null && Banner.ContentLength > 0)
            {
                if (Banner.ContentType == "image/jpeg" || Banner.ContentType == "image/jpg" || Banner.ContentType == "image/png" || Banner.ContentType == "image/jfif")
                {
                    var bannerPath = Request.MapPath("~/IMG/" + updateanime1.BIGBANNER);
                    if (System.IO.File.Exists(bannerPath))
                    {
                        System.IO.File.Delete(bannerPath);
                    }
                    var bannerFile = new FileInfo(Banner.FileName);
                    var bannerFileName = Path.GetFileName(Banner.FileName);
                    bannerFileName = Guid.NewGuid().ToString() + bannerFile.Extension;
                    var bannerPath2 = Path.Combine(Server.MapPath("~/IMG/"), bannerFileName);

                    try
                    {
                        Banner.SaveAs(bannerPath2);
                        updateanime1.BIGBANNER = bannerFileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Error uploading banner: " + ex.Message;
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "Invalid banner file type";
                    return View();
                }
            }

            if (Video != null && Video.ContentLength > 0)
            {
                var videoPath = Request.MapPath("~/Videos/" + Video.FileName);
                try
                {
                    Video.SaveAs(videoPath);
                    updateanime1.EDIT = Video.FileName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error uploading video: " + ex.Message;
                    return View();
                }
            }

            updateanime1.CATEGORYID = update.CATEGORYID;
            updateanime1.ANIMENAME = update.ANIMENAME;
            updateanime1.DETAIL = update.DETAIL;
            updateanime1.TYPE = update.TYPE;
            updateanime1.IMDB = update.IMDB;
            updateanime1.DATE = update.DATE;
            updateanime1.MYANIMELIST = update.MYANIMELIST;

            db.SaveChanges();
            TempData["success"] = "Success";
            return RedirectToAction("Anime");
        }

        public ActionResult ActivePassive1(int id)
        {
            var AdminActivePassive = db.TBLANIME.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Anime");
        }

        public ActionResult Deletee(int id)
        {
            var animefind = db.TBLANIME.Find(id);
            db.TBLANIME.Remove(animefind);
            db.SaveChanges();
            TempData["error"] = "Anime başarıyla silindi";
            return RedirectToAction("Anime");
        }


        public ActionResult ActivePassive2(int id)
        {
            var AdminActivePassive = db.TBLANIME.Find(id);
            if (AdminActivePassive.ADMINSTATUS == true)
            {
                AdminActivePassive.ADMINSTATUS = false;
            }
            else
            {
                AdminActivePassive.ADMINSTATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Anime");
        }


        public ActionResult AnimeDetail(int id)
        {
            var degerler = db.TBLANIME.Where(x=>x.ID == id).ToList();
            return View(degerler);
        }


        public ActionResult Season(string anime, int p = 1)
        {
            var degerler = from a in db.TBLSEASON select a;
            if (!string.IsNullOrEmpty(anime))
            {
                degerler = degerler.Where(x => x.TBLANIME.ANIMENAME.ToLower().Contains(anime.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLSEASON>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Anime = anime;
            return View(pagedList);
        }

        [HttpGet]
        public ActionResult AddSeason()
        {
            ViewBag.Anime = db.TBLANIME.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AddSeason(TBLSEASON add)
        {
            TBLSEASON season = new TBLSEASON();
            season.ANIMEID = add.ANIMEID;
            season.SEASONNUMBER = add.SEASONNUMBER;
            season.SEASONNAME = add.SEASONNAME;
            season.TRANSLATES = add.TRANSLATES;
            season.FANSUBAD = add.FANSUBAD;
            season.WEBSITE = add.WEBSITE;
            season.DISCORD = add.DISCORD;
            season.STATUS = false;
            db.TBLSEASON.Add(season);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("Season");
        }


        public ActionResult UpdateSeason(int id)
        {
            var FindSeason = db.TBLSEASON.Find(id);
            ViewBag.Anime = db.TBLANIME.ToList();
            ViewBag.SelectedAnimeId = FindSeason.ANIMEID; // Seçili animeyi ekleyin
            return View("UpdateSeason", FindSeason);
        }

        public ActionResult SeasonSave(TBLSEASON sea)
        {
            var updateseason = db.TBLSEASON.Find(sea.ID);
            updateseason.ANIMEID = sea.ANIMEID;
            updateseason.SEASONNUMBER = sea.SEASONNUMBER;
            updateseason.SEASONNAME = sea.SEASONNAME;
            updateseason.FANSUBAD = sea.FANSUBAD;
            updateseason.TRANSLATES = sea.TRANSLATES;
            updateseason.DISCORD = sea.DISCORD;
            updateseason.WEBSITE = sea.WEBSITE;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Season");
        }

        public ActionResult ActivePassive3(int id)
        {
            var AdminActivePassive = db.TBLSEASON.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Season");
        }

        public ActionResult Deleteee(int id)
        {
            var seasonfind = db.TBLSEASON.Find(id);
            db.TBLSEASON.Remove(seasonfind);
            db.SaveChanges();
            TempData["error"] = "Sezon başarıyla silindi";
            return RedirectToAction("Season");
        }




        public ActionResult Episode(string episode, int p = 1)
        {
            var degerler = from a in db.TBLEPISODE select a;
            if (!string.IsNullOrEmpty(episode))
            {
                degerler = degerler.Where(x => x.TBLANIME.ANIMENAME.ToLower().Contains(episode.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLEPISODE>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Episode = episode;
            return View(pagedList);
        }


        [HttpGet]
        public ActionResult AddEpisode()
        {
            ViewBag.Anime = db.TBLANIME.ToList();
            return View();
        }


        [HttpPost]
        public ActionResult AddEpisode(TBLEPISODE add, HttpPostedFileBase Image, HttpPostedFileBase Video, HttpPostedFileBase Video2, HttpPostedFileBase Video3, HttpPostedFileBase Video4)
        {
            if (Image != null && Image.ContentLength > 0)
            {
                if (Image.ContentType == "image/jpeg" || Image.ContentType == "image/jpg" || Image.ContentType == "image/png" || Image.ContentType == "image/jfif")
                {
                    var file = new FileInfo(Image.FileName);
                    var fileName = Path.GetFileName(Image.FileName);
                    fileName = Guid.NewGuid().ToString() + file.Extension;
                    var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                    WebImage rr = new WebImage(Image.InputStream);

                    if (rr.Width > 1000)
                        rr.Resize(1000, 1000);
                    rr.Save(path);

                    TBLEPISODE episode = new TBLEPISODE();
                    episode.ANIMEID = add.ANIMEID;
                    episode.SEASONID = add.SEASONID;
                    episode.EPINUMBER = add.EPINUMBER;
                    episode.STATUS = false;
                    episode.EPISODENAME = add.EPISODENAME;
                    episode.BANNER = fileName;
                    episode.TIME = add.TIME;
                    episode.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                    // Process the video URL for embedding
                    if (!string.IsNullOrEmpty(add.URL))
                    {
                        episode.URL = FormatVideoUrl(add.URL);
                    }

                    // Handle video uploads
                    if (Video2 != null && Video2.ContentLength > 0)
                    {
                        string yol2 = Path.Combine("~/Videos/" + Video2.FileName);
                        Video2.SaveAs(Server.MapPath(yol2));
                    }
                    episode.EP2 = Video2 != null ? Video2.FileName.ToString() : null;

                    if (Video3 != null && Video3.ContentLength > 0)
                    {
                        string yol3 = Path.Combine("~/Videos/" + Video3.FileName);
                        Video3.SaveAs(Server.MapPath(yol3));
                    }
                    episode.EP3 = Video3 != null ? Video3.FileName.ToString() : null;

                    if (Video4 != null && Video4.ContentLength > 0)
                    {
                        string yol4 = Path.Combine("~/Videos/" + Video4.FileName);
                        Video4.SaveAs(Server.MapPath(yol4));
                    }
                    episode.EP4 = Video4 != null ? Video4.FileName.ToString() : null;

                    db.TBLEPISODE.Add(episode);
                    db.SaveChanges();
                    TempData["add"] = "success";
                    return RedirectToAction("Episode");
                }
                else
                {
                    TempData["error"] = "error";
                    return View();
                }
            }
            return View();
        }

        private string FormatVideoUrl(string url)
        {
            if (url.Contains("youtube.com/watch?v="))
            {
                var videoId = url.Split(new[] { "v=" }, StringSplitOptions.None)[1].Split('&')[0];
                return "https://www.youtube.com/embed/" + videoId;
            }
            else if (url.Contains("drive.google.com/file/d/"))
            {
                var fileId = url.Split(new[] { "/d/" }, StringSplitOptions.None)[1].Split('/')[0];
                return "https://drive.google.com/file/d/" + fileId + "/preview";
            }
            return url; 
        }


        public ActionResult UpdateEpisode(int id)
        {
            var FindEpisode = db.TBLEPISODE.Find(id);
            ViewBag.Anime = db.TBLANIME.ToList();
            ViewBag.SelectedAnimeId = FindEpisode.ANIMEID; // Seçili animeyi ekleyin
            return View("UpdateEpisode", FindEpisode);
        }

        [HttpPost]
        public ActionResult EpisodeSave(TBLEPISODE update, HttpPostedFileBase Image, HttpPostedFileBase Video, HttpPostedFileBase Video2, HttpPostedFileBase Video3, HttpPostedFileBase Video4)
        {
            var updateepisode = db.TBLEPISODE.Find(update.ID);

            if (Image != null && Image.ContentLength > 0)
            {
                if (Image.ContentType == "image/jpeg" || Image.ContentType == "image/jpg" || Image.ContentType == "image/png" || Image.ContentType == "image/jfif")
                {
                    var anipath = Request.MapPath("~/IMG/" + updateepisode.BANNER);
                    if (System.IO.File.Exists(anipath))
                    {
                        System.IO.File.Delete(anipath);
                    }
                    var file = new FileInfo(Image.FileName);
                    var fileName = Path.GetFileName(Image.FileName);
                    fileName = Guid.NewGuid().ToString() + file.Extension;
                    var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                    try
                    {
                        WebImage rr = new WebImage(Image.InputStream);
                        if (rr.Width > 1000)
                            rr.Resize(1000, 1000);
                        rr.Save(path);
                        updateepisode.BANNER = fileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Error uploading image: " + ex.Message;
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "Invalid image file type";
                    return View();
                }
            }

            if (!string.IsNullOrEmpty(update.URL))
            {
                updateepisode.URL = FormatVideoUrl(update.URL);
            }

            if (Video2 != null && Video2.ContentLength > 0)
            {
                var videoPath = Request.MapPath("~/Videos/" + Video2.FileName);
                try
                {
                    Video2.SaveAs(videoPath);
                    updateepisode.EP2 = Video2.FileName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error uploading video: " + ex.Message;
                    return View();
                }
            }

            if (Video3 != null && Video3.ContentLength > 0)
            {
                var videoPath = Request.MapPath("~/Videos/" + Video3.FileName);
                try
                {
                    Video3.SaveAs(videoPath);
                    updateepisode.EP3 = Video3.FileName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error uploading video: " + ex.Message;
                    return View();
                }
            }

            if (Video4 != null && Video4.ContentLength > 0)
            {
                var videoPath = Request.MapPath("~/Videos/" + Video4.FileName);
                try
                {
                    Video4.SaveAs(videoPath);
                    updateepisode.EP4 = Video4.FileName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error uploading video: " + ex.Message;
                    return View();
                }
            }

            updateepisode.ANIMEID = update.ANIMEID;
            updateepisode.SEASONID = update.SEASONID;
            updateepisode.EPINUMBER = update.EPINUMBER;
            updateepisode.EPISODENAME = update.EPISODENAME;
            updateepisode.TIME = update.TIME;

            db.SaveChanges();
            TempData["success"] = "Success";
            return RedirectToAction("Episode");
        }


        public ActionResult ActivePassive4(int id)
        {
            var AdminActivePassive = db.TBLEPISODE.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Episode");
        }

        public ActionResult Deleteeee(int id)
        {
            var episodefind = db.TBLEPISODE.Find(id);
            db.TBLEPISODE.Remove(episodefind);
            db.SaveChanges();
            TempData["error"] = "Bölüm başarıyla silindi";
            return RedirectToAction("Episode");
        }

        public ActionResult EpisodeDetail(int id)
        {
            var degerler = db.TBLEPISODE.Where(x=>x.ID == id).ToList();
            return View(degerler);
        }

        public ActionResult Users(string user, int p = 1)
        {
            var degerler = from a in db.TBLUSER select a;
            if (!string.IsNullOrEmpty(user))
            {
                degerler = degerler.Where(x => x.USERNAME.ToLower().Contains(user.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLUSER>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.User = user;
            return View(pagedList);
        }


        public ActionResult UpdateUser(int id)
        {
            var FindUser = db.TBLUSER.Find(id);
            return View("UpdateUser", FindUser);
        }

        [HttpPost]
        public ActionResult UserSave(TBLUSER update, HttpPostedFileBase Image)
        {
            var updateuser = db.TBLUSER.Find(update.ID);
            updateuser.MAIL = update.MAIL;
            updateuser.ABOUT = update.ABOUT;
            
            db.SaveChanges();
            TempData["success"] = "Success";
            return RedirectToAction("Users");
        }

        public ActionResult ActivePassive5(int id)
        {
            var AdminActivePassive = db.TBLUSER.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Users");
        }

        public ActionResult Deleteeeee(int id)
        {
            var userfind = db.TBLUSER.Find(id);
            db.TBLUSER.Remove(userfind);
            db.SaveChanges();
            TempData["error"] = "Kullanıcı başarıyla silindi";
            return RedirectToAction("Users");
        }

        public ActionResult UserDetail(int id)
        {
            var degerler = db.TBLUSER.Where(x=>x.ID == id).ToList();
            return View(degerler);
        }

        public ActionResult AnimeComment(string Comment, int p = 1)
        {
            var degerler = from a in db.TBLANIMECOMMENT select a;
            if (!string.IsNullOrEmpty(Comment))
            {
                degerler = degerler.Where(x => x.TBLUSER.USERNAME.ToLower().Contains(Comment.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLANIMECOMMENT>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Comment = Comment;
            return View(pagedList);
        }

        public ActionResult UpdateComment(int id)
        {
            var FindComment = db.TBLANIMECOMMENT.Find(id);
            return View("UpdateComment", FindComment);
        }

        [HttpPost]
        public ActionResult CommentSave(TBLANIMECOMMENT update)
        {
            var updatecomment = db.TBLANIMECOMMENT.Find(update.ID);
            updatecomment.COMMENT = update.COMMENT;
            db.SaveChanges();
            TempData["success"] = "Success";
            return RedirectToAction("AnimeComment");
        }

        public ActionResult ActivePassive6(int id)
        {
            var AdminActivePassive = db.TBLANIMECOMMENT.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("AnimeComment");
        }

        public ActionResult Deleteeeeee(int id)
        {
            var animecomment = db.TBLANIMECOMMENT.Find(id);
            db.TBLANIMECOMMENT.Remove(animecomment);
            db.SaveChanges();
            TempData["error"] = "Kullanıcı başarıyla silindi";
            return RedirectToAction("AnimeComment");
        }

        public ActionResult AnımeCommentDetaıl(int id)
        {
            var degerler = db.TBLANIMECOMMENT.Where(x => x.ID == id).ToList();
            return View(degerler);
        }



        public ActionResult EpisodeComment(string Comment, int p = 1)
        {
            var degerler = from a in db.TBLEPISODECOMMENT select a;
            if (!string.IsNullOrEmpty(Comment))
            {
                degerler = degerler.Where(x => x.TBLUSER.USERNAME.ToLower().Contains(Comment.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLEPISODECOMMENT>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Comment = Comment;
            return View(pagedList);
        }

        public ActionResult UpdateEpisodeComment(int id)
        {
            var FindComment = db.TBLEPISODECOMMENT.Find(id);
            return View("UpdateEpisodeComment", FindComment);
        }

        [HttpPost]
        public ActionResult EpisodeCommentSave(TBLEPISODECOMMENT update)
        {
            var updatecomment = db.TBLEPISODECOMMENT.Find(update.ID);
            updatecomment.COMMENT = update.COMMENT;
            db.SaveChanges();
            TempData["success"] = "Success";
            return RedirectToAction("EpisodeComment");
        }

        public ActionResult ActivePassive7(int id)
        {
            var AdminActivePassive = db.TBLEPISODECOMMENT.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("EpisodeComment");
        }

        public ActionResult Deleteeeeeee(int id)
        {
            var animecomment = db.TBLEPISODECOMMENT.Find(id);
            db.TBLEPISODECOMMENT.Remove(animecomment);
            db.SaveChanges();
            TempData["error"] = "Kullanıcı başarıyla silindi";
            return RedirectToAction("EpisodeComment");
        }

        public ActionResult EpisodeCommentDetaıl(int id)
        {
            var degerler = db.TBLEPISODECOMMENT.Where(x => x.ID == id).ToList();
            return View(degerler);
        }


        public ActionResult Donater(string Donate, int p = 1)
        {
            var degerler = from a in db.TBLDONATE select a;
            if (!string.IsNullOrEmpty(Donate))
            {
                degerler = degerler.Where(x => x.DONATER.ToLower().Contains(Donate.ToLower()));
            }
            var pageSize = 50;
            var paginatedResults = degerler.OrderByDescending(x => x.ID).Skip((p - 1) * pageSize).Take(pageSize).ToList();
            var pagedList = new StaticPagedList<TBLDONATE>(paginatedResults, p, pageSize, degerler.Count());
            ViewBag.PageCount = (int)Math.Ceiling(degerler.Count() / (double)pageSize);
            ViewBag.CurrentPage = p;
            ViewBag.Donate = Donate;
            return View(pagedList);
        }


        [HttpGet]
        public ActionResult AddDonate()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddDonate(TBLDONATE add)
        {
            TBLDONATE donate = new TBLDONATE();
            donate.DONATER = add.DONATER;
            donate.DONATE = add.DONATE;
            donate.STATUS = false;
            db.TBLDONATE.Add(donate);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("Donater");
        }



        public ActionResult UpdateDonate(int id)
        {
            var FindDonate = db.TBLDONATE.Find(id);
            return View("UpdateDonate", FindDonate);
        }

        public ActionResult DonateSave(TBLDONATE cat)
        {
            var updatedonate = db.TBLDONATE.Find(cat.ID);
            updatedonate.DONATER = cat.DONATER;
            updatedonate.DONATE = cat.DONATE;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Donater");
        }

        public ActionResult Deleteeeeeeee(int id)
        {
            var Donate = db.TBLDONATE.Find(id);
            db.TBLDONATE.Remove(Donate);
            db.SaveChanges();
            TempData["error"] = "Kullanıcı başarıyla silindi";
            return RedirectToAction("Donater");
        }

        public ActionResult ActivePassive9(int id)
        {
            var AdminActivePassive = db.TBLDONATE.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Donater");
        }

        public ActionResult Error()
        {
            var degerler = db.TBL404.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult AddError()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddError(TBL404 add, HttpPostedFileBase Video)
        {
            TBL404 error = new TBL404();
            if (Video != null && Video.ContentLength > 0)
            {
                string yol = Path.Combine("~/Videos/" + Video.FileName);
                Video.SaveAs(Server.MapPath(yol));
            }
            error.VIDEO = Video != null ? Video.FileName.ToString() : null;
            error.TEXT = add.TEXT;
            error.STATUS = false;
            db.TBL404.Add(error);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("Error");
        }

        public ActionResult UpdateError(int id)
        {
            var FindError = db.TBL404.Find(id);
            ViewBag.Error = db.TBL404.ToList();
            return View("UpdateError", FindError);
        }

        public ActionResult ErrorSave(TBL404 sea, HttpPostedFileBase Video)
        {
            var updateerror = db.TBL404.Find(sea.ID);
            if (Video != null && Video.ContentLength > 0)
            {
                string yol = Path.Combine("~/Videos/" + Video.FileName);
                Video.SaveAs(Server.MapPath(yol));
            }
            updateerror.VIDEO = Video != null ? Video.FileName.ToString() : null;
            updateerror.TEXT = sea.TEXT;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Error");
        }

        public ActionResult ActivePassive10(int id)
        {
            var AdminActivePassive = db.TBL404.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Error");
        }
        public ActionResult ErrorDelete(int id)
        {
            var Error = db.TBL404.Find(id);
            db.TBL404.Remove(Error);
            db.SaveChanges();
            TempData["error"] = "Kullanıcı başarıyla silindi";
            return RedirectToAction("Error");
        }

        public ActionResult ErrorDetail(int id)
        {
            var degerler = db.TBL404.Where(x => x.ID == id).ToList();
            return View(degerler);
        }
    }
}