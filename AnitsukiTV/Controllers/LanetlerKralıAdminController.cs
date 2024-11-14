using AnitsukiTV.Models;
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
using System.Web.Services.Description;

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
            try
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
            catch (Exception ex)
            {
                TempData["error"] = "Admin eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        public ActionResult UpdateAdmin(int id)
        {
            var FindAdmin = db.TBLADMIN.Find(id);
            return View("UpdateAdmin", FindAdmin);
        }

        public ActionResult AdminSave(TBLADMIN admin)
        {
            try
            {
                var updateadmin = db.TBLADMIN.Find(admin.ID);
                updateadmin.USERNAME = admin.USERNAME;
                updateadmin.PASSWORD = admin.PASSWORD;
                updateadmin.ADMINROLEID = admin.ADMINROLEID;

                db.SaveChanges();
                TempData["success1"] = "Admin başarıyla güncellendi";
                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Admin güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public ActionResult ActivePassive(int id)
        {
            var AdminActivePassive = db.TBLADMIN.Find(id);
            if (AdminActivePassive == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive.STATUS = !AdminActivePassive.STATUS; 
            db.SaveChanges();

            return Json(new { status = AdminActivePassive.STATUS });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var AdminFind = db.TBLADMIN.Find(id);
                db.TBLADMIN.Remove(AdminFind);
                db.SaveChanges();
                TempData["success2"] = "Admin başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "Admin silinirken bir hata oluştu: " + ex.Message;
            }

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
            try
            {
                TBLADMINROLE admin = new TBLADMINROLE();
                admin.ROLENAME = add.ROLENAME;

                db.TBLADMINROLE.Add(admin);
                db.SaveChanges();
                TempData["success"] = "Rol başarıyla eklendi.";
                return RedirectToAction("AdminRole");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Rol eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("AdminRole");
            }
        }


        public ActionResult UpdateRole(int id)
        {
            var FindRole = db.TBLADMINROLE.Find(id);
            return View("UpdateRole", FindRole);
        }


        public ActionResult RoleSave(TBLADMINROLE role)
        {
            try
            {
                var updaterole = db.TBLADMINROLE.Find(role.ID);
                updaterole.ROLENAME = role.ROLENAME;
                db.SaveChanges();
                TempData["success1"] = "Rol başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Rol güncellenirken bir hata oluştu: " + ex.Message;
            }
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
            try
            {
                TBLCATEGORY category = new TBLCATEGORY();
                category.CATEGORYNAME = add.CATEGORYNAME;
                category.STATUS = true;
                db.TBLCATEGORY.Add(category);
                db.SaveChanges();
                TempData["success"] = "Kategori başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Kategori eklenirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Category");
        }


        public ActionResult UpdateCategory(int id)
        {
            var FindCategory = db.TBLCATEGORY.Find(id);
            return View("UpdateCategory", FindCategory);
        }

        public ActionResult CategorySave(TBLCATEGORY cat)
        {
            try
            {
                var updatecategory = db.TBLCATEGORY.Find(cat.ID);
                updatecategory.CATEGORYNAME = cat.CATEGORYNAME;
                db.SaveChanges();
                TempData["success1"] = "Kategori başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Kategori güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Category");
        }

        [HttpPost]
        public ActionResult ActivePassive8(int id)
        {
            var AdminActivePassive8 = db.TBLCATEGORY.Find(id);
            if (AdminActivePassive8 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive8.STATUS = !AdminActivePassive8.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive8.STATUS });
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
            try
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
                TempData["success"] = "Anime başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Anime eklenirken bir hata oluştu: " + ex.Message;
            }
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
            try
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
                        catch
                        {
                            return View();
                        }
                    }
                    else
                    {
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
                        catch
                        {
                            return View();
                        }
                    }
                    else
                    {
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
                    catch 
                    {
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
                TempData["success1"] = "Anime başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Anime güncellenirken bir hata oluştu: " + ex.Message;
                return View();
            }

            return RedirectToAction("Anime");
        }

        [HttpPost]
        public ActionResult ActivePassive1(int id)
        {
            var AdminActivePassive1 = db.TBLANIME.Find(id);
            if (AdminActivePassive1 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive1.STATUS = !AdminActivePassive1.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive1.STATUS });
        }

        public ActionResult Deletee(int id)
        {
            try
            {
                var animefind = db.TBLANIME.Find(id);
                db.TBLANIME.Remove(animefind);
                db.SaveChanges();
                TempData["success2"] = "Anime başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "Anime silinirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Anime");
        }

        [HttpPost]
        public ActionResult ActivePassive2(int id)
        {
            var AdminActivePassive2 = db.TBLANIME.Find(id);
            if (AdminActivePassive2 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive2.ADMINSTATUS = !AdminActivePassive2.ADMINSTATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive2.ADMINSTATUS });
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
            try
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
                TempData["success"] = "Sezon başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Sezon eklenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Season");
        }


        public ActionResult UpdateSeason(int id)
        {
            var FindSeason = db.TBLSEASON.Find(id);
            ViewBag.Anime = db.TBLANIME.ToList();
            ViewBag.SelectedAnimeId = FindSeason.ANIMEID;
            return View("UpdateSeason", FindSeason);
        }

        public ActionResult SeasonSave(TBLSEASON sea)
        {
            try
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
                TempData["success1"] = "Sezon başarıyla güncellendi.";
            }
            catch (Exception ex) {
                TempData["error1"] = "Sezon güncellenirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Season");
        }

        [HttpPost]
        public ActionResult ActivePassive3(int id)
        {
            var AdminActivePassive3 = db.TBLSEASON.Find(id);
            if (AdminActivePassive3 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive3.STATUS = !AdminActivePassive3.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive3.STATUS });
        }

        public ActionResult Deleteee(int id)
        {
            try
            {
                var seasonfind = db.TBLSEASON.Find(id);
                db.TBLSEASON.Remove(seasonfind);
                db.SaveChanges();
                TempData["success2"] = "Sezon başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "Sezon silinirken bir hata oluştu: " + ex.Message;
            }
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
            try
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
                        TempData["success"] = "Bölüm başarıyla eklendi.";
                        return RedirectToAction("Episode");
                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Bölüm eklenirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetSeasonsByAnimeId(int animeId)
        {
            var seasons = db.TBLSEASON.Where(s => s.ANIMEID == animeId).Select(s => new {
                s.ID,
                s.SEASONNUMBER,
                s.SEASONNAME
            }).ToList();

            return Json(seasons, JsonRequestBehavior.AllowGet);
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
            ViewBag.SelectedAnimeId = FindEpisode.ANIMEID; 
            ViewBag.SelectedSeasonId = FindEpisode.SEASONID; 
            ViewBag.SelectedSeasonName = !string.IsNullOrEmpty(FindEpisode.TBLSEASON.SEASONNAME) ? FindEpisode.TBLSEASON.SEASONNAME : FindEpisode.TBLSEASON.SEASONNUMBER?.ToString(); 

            return View("UpdateEpisode", FindEpisode);
        }

        [HttpPost]
        public ActionResult EpisodeSave(TBLEPISODE update, HttpPostedFileBase Image, HttpPostedFileBase Video, HttpPostedFileBase Video2, HttpPostedFileBase Video3, HttpPostedFileBase Video4)
        {
            try
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
                            TempData["error3"] = "Resim güncellenirken bir hata oluştu: " + ex.Message;
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }

                if (!string.IsNullOrEmpty(update.URL))
                {
                    updateepisode.URL = FormatVideoUrl(update.URL);
                }

                // Handle video uploads
                if (Video2 != null && Video2.ContentLength > 0)
                {
                    var videoPath = Request.MapPath("~/Videos/" + Video2.FileName);
                    Video2.SaveAs(videoPath);
                    updateepisode.EP2 = Video2.FileName;
                }

                if (Video3 != null && Video3.ContentLength > 0)
                {
                    var videoPath = Request.MapPath("~/Videos/" + Video3.FileName);
                    Video3.SaveAs(videoPath);
                    updateepisode.EP3 = Video3.FileName;
                }

                if (Video4 != null && Video4.ContentLength > 0)
                {
                    var videoPath = Request.MapPath("~/Videos/" + Video4.FileName);
                    Video4.SaveAs(videoPath);
                    updateepisode.EP4 = Video4.FileName;
                }

                // Update other fields
                updateepisode.ANIMEID = update.ANIMEID;
                updateepisode.SEASONID = update.SEASONID;
                updateepisode.EPINUMBER = update.EPINUMBER;
                updateepisode.EPISODENAME = update.EPISODENAME;
                updateepisode.TIME = update.TIME;

                db.SaveChanges();
                TempData["success1"] = "Bölüm başarıyla güncellendi.";
                return RedirectToAction("Episode");
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Bölüm güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Episode");
            }
        }

        [HttpPost]
        public ActionResult ActivePassive4(int id)
        {
            var AdminActivePassive4 = db.TBLEPISODE.Find(id);
            if (AdminActivePassive4 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive4.STATUS = !AdminActivePassive4.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive4.STATUS });
        }

        public ActionResult Deleteeee(int id)
        {
            try
            {
                var episodefind = db.TBLEPISODE.Find(id);
                db.TBLEPISODE.Remove(episodefind);
                db.SaveChanges();
                TempData["error2"] = "Bölüm başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "Bölüm silinirken bir hata oluştu: " + ex.Message;
            }
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
            try
            {
                var updateuser = db.TBLUSER.Find(update.ID);
                updateuser.MAIL = update.MAIL;
                updateuser.ABOUT = update.ABOUT;
                db.SaveChanges();
                TempData["success"] = "Kullanıcı hesabı başarıyla güncellenmiştir ";
            }
            catch (Exception ex) {
                TempData["error"] = "Kullanıcı hesabı güncellenirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Users");
        }

        [HttpPost]
        public ActionResult ActivePassive5(int id)
        {
            var AdminActivePassive5 = db.TBLUSER.Find(id);
            if (AdminActivePassive5 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive5.STATUS = !AdminActivePassive5.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive5.STATUS });
        }

        public ActionResult Deleteeeee(int id)
        {
            try
            {
                var userfind = db.TBLUSER.Find(id);
                db.TBLUSER.Remove(userfind);
                db.SaveChanges();
                TempData["success1"] = "Kullanıcı hesabı başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Kullanıcı hesabı silerken bir hata oluştu: " + ex.Message;
            }
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
            try
            {
                var updatecomment = db.TBLANIMECOMMENT.Find(update.ID);
                updatecomment.COMMENT = update.COMMENT;
                db.SaveChanges();
                TempData["success"] = "Kullanıcı yorumu başarıyla güncellenmiştir.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Kullanıcı yorumu güncellenirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("AnimeComment");
        }

        public ActionResult ActivePassive6(int id)
        {
            var AdminActivePassive6 = db.TBLANIMECOMMENT.Find(id);
            if (AdminActivePassive6 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive6.STATUS = !AdminActivePassive6.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive6.STATUS });
        }

        public ActionResult Deleteeeeee(int id)
        {
            try
            {
                var animecomment = db.TBLANIMECOMMENT.Find(id);
                db.TBLANIMECOMMENT.Remove(animecomment);
                db.SaveChanges();
                TempData["success"] = "Kullanıcı yorumu başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Kullanıcı yorumu silinirken bir hata oluştu: " + ex.Message;
            }
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
            try
            {
                var updatecomment = db.TBLEPISODECOMMENT.Find(update.ID);
                updatecomment.COMMENT = update.COMMENT;
                db.SaveChanges();
                TempData["success"] = "Kullanıcı yorumu başarıyla güncellenmiştir.";
            }
            catch (Exception ex)
            {

                TempData["error"] = "Kullanıcı yorumu güncellenirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("EpisodeComment");
        }

        public ActionResult ActivePassive7(int id)
        {
            var AdminActivePassive7 = db.TBLEPISODECOMMENT.Find(id);
            if (AdminActivePassive7 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive7.STATUS = !AdminActivePassive7.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive7.STATUS });
        }

        public ActionResult Deleteeeeeee(int id)
        {
            try
            {
                var animecomment = db.TBLEPISODECOMMENT.Find(id);
                db.TBLEPISODECOMMENT.Remove(animecomment);
                db.SaveChanges();
                TempData["success1"] = "Kullanıcı yorumu başarıyla silinmiştir.";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Kullanıcı yorumu silinirken bir hata oluştu: " + ex.Message;
            }
            
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
            try
            {
                TBLDONATE donate = new TBLDONATE();
                donate.DONATER = add.DONATER;
                donate.DONATE = add.DONATE;
                donate.STATUS = false;
                db.TBLDONATE.Add(donate);
                db.SaveChanges();
                TempData["success"] = "Bağış başarıyla eklendi";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Bağış eklenirken bir sorun oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Donater");
        }



        public ActionResult UpdateDonate(int id)
        {
            var FindDonate = db.TBLDONATE.Find(id);
            return View("UpdateDonate", FindDonate);
        }

        public ActionResult DonateSave(TBLDONATE cat)
        {
            try
            {
                var updatedonate = db.TBLDONATE.Find(cat.ID);
                updatedonate.DONATER = cat.DONATER;
                updatedonate.DONATE = cat.DONATE;
                db.SaveChanges();
                TempData["success1"] = "Bağışçı başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["error1"] = "Bağışçı güncellenirken bir sorun oluştu " + ex.Message;
            }
            
            return RedirectToAction("Donater");
        }

        public ActionResult Deleteeeeeeee(int id)
        {
            try
            {
                var Donate = db.TBLDONATE.Find(id);
                db.TBLDONATE.Remove(Donate);
                db.SaveChanges();
                TempData["success2"] = "Bağışçı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "Bağışçı silinirken bir sorun oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Donater");
        }

        public ActionResult ActivePassive9(int id)
        {
            var AdminActivePassive9 = db.TBLDONATE.Find(id);
            if (AdminActivePassive9 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive9.STATUS = !AdminActivePassive9.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive9.STATUS });
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
            try
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
                TempData["success"] = "404 Sayfası başaryla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "404 Sayfası eklerken bir sorun oluştu: " + ex.Message;
            }
            
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
            try
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
                TempData["success1"] = "404 Sayfası başarıyla güncellendi.";
            }
            catch (Exception ex)
            {

                TempData["error1"] = "404 Sayfası güncellerken bir sorun oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Error");
        }

        public ActionResult ActivePassive10(int id)
        {
            var AdminActivePassive10 = db.TBL404.Find(id);
            if (AdminActivePassive10 == null)
            {
                return Json(new { success = false });
            }
            AdminActivePassive10.STATUS = !AdminActivePassive10.STATUS;
            db.SaveChanges();

            return Json(new { status = AdminActivePassive10.STATUS });
        }

        public ActionResult ErrorDelete(int id)
        {
            try
            {
                var Error = db.TBL404.Find(id);
                db.TBL404.Remove(Error);
                db.SaveChanges();
                TempData["success2"] = "404 Sayfası başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["error2"] = "404 Sayfası silinirken bir sorun oluştu: " + ex.Message;
            }
            
            return RedirectToAction("Error");
        }

        public ActionResult ErrorDetail(int id)
        {
            var degerler = db.TBL404.Where(x => x.ID == id).ToList();
            return View(degerler);
        }
    }
}