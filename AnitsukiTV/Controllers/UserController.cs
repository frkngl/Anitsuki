using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        TabeList veri = new TabeList();
        // GET: User
        public ActionResult Index()
        {
            int userID = (int)Session["id"];
            veri.User = db.TBLUSER.Where(x => x.ID == userID).FirstOrDefault();
            return View(veri);
        }
        public ActionResult ProfileSetting()
        {
            int userID = (int)Session["id"];
            var user = db.TBLUSER.Find(userID);
            return View("ProfileSetting", user);
        }
        [HttpPost]
        public ActionResult ProfileSetting(TBLUSER user, HttpPostedFileBase UserImage)
        {
            int userID = (int)Session["id"];
            var UserUpdate = db.TBLUSER.Find(userID);
            if (UserImage != null && UserImage.ContentLength > 0)
            {
                if (UserImage.ContentType == "image/jpeg" || UserImage.ContentType == "image/png" || UserImage.ContentType == "image/jpg" || UserImage.ContentType == "image/jfif")
                {
                    var Userline = Request.MapPath("~/IMG/" + UserUpdate.PICTURE);
                    if (System.IO.File.Exists(Userline))
                    {
                        System.IO.File.Delete(Userline);
                    }

                    var file = new FileInfo(UserImage.FileName);
                    var fileName = Path.GetFileName(UserImage.FileName);
                    fileName = Guid.NewGuid().ToString() + file.Extension;
                    var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                    WebImage Websize = new WebImage(UserImage.InputStream);

                    if (Websize.Width > 1000)
                        Websize.Resize(800, 800);
                    Websize.Save(path);
                    UserUpdate.PICTURE = fileName;
                }
                else
                {
                    TempData["hata"] = "Lütfen resim formatında bir dosya seçiniz !!!";
                    return View();
                }
            }
            UserUpdate.ABOUT = user.ABOUT;
            UserUpdate.PASSWORD = user.PASSWORD;
            UserUpdate.MAIL = user.MAIL;
            db.SaveChanges();
            TempData["Guncelle"] = "Başarıyla Güncellenmiştir !!!";
            return RedirectToAction("ProfileSetting");
        }
    }
}