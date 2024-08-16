using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace AnitsukiTV.Controllers
{
    [Authorize]
    public class LanetlerKralıAdminController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        // GET: LanetlerKralıAdmin
        public ActionResult Index()
        {
            return View();
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



        public ActionResult Anime(string anime)
        {
            var degerler = from a in db.TBLANIME select a;
            if (!string.IsNullOrEmpty(anime))
            {
                degerler = degerler.Where(x => x.ANIMENAME.ToLower().Contains(anime.ToLower()));
            }
            return View(degerler.ToList());
        }


        [HttpGet]
        public ActionResult AddAnime()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAnime(TBLANIME add, HttpPostedFileBase Image, HttpPostedFileBase Banner, HttpPostedFileBase Video)
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

                        rr.Resize(800, 800);
                    rr.Save(path);

                    TBLANIME anime = new TBLANIME();
                    anime.ANIMENAME = add.ANIMENAME;
                    anime.DETAIL = add.DETAIL;
                    anime.IMDB = add.IMDB;
                    anime.STATUS = false;
                    anime.ADMINSTATUS = false;
                    anime.TYPE = add.TYPE;
                    anime.BANNER = fileName;

                    string yol = Path.Combine("~/IMG/" + Banner.FileName);
                    Banner.SaveAs(Server.MapPath(yol));
                    anime.BIGBANNER = Banner.FileName.ToString();

                    anime.CATEGORYID = add.CATEGORYID;
                    anime.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                    string yol2 = Path.Combine("~/Videos/" + Video.FileName);
                    Video.SaveAs(Server.MapPath(yol2));
                    anime.EDIT = Video.FileName.ToString();

                    db.TBLANIME.Add(anime);
                    db.SaveChanges();
                    TempData["add"] = "success";
                    return RedirectToAction("Anime");
                }
                else
                {
                    TempData["error"] = "error";
                    return View();
                }
            }
            return View();
        }

        public ActionResult UpdateAnime(int id)
        {
            var FindAnime = db.TBLANIME.Find(id);
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
                            rr.Resize(800, 800);
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
    }
}