using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "LoginSignIn"); // redirect to login page
            }

            int userID = (int)Session["id"];
            veri.User = db.TBLUSER.Where(x => x.ID == userID).FirstOrDefault();
            return View(veri);
        }



        [HttpGet]
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

            user.ABOUT = user.ABOUT.Trim();
            user.MAIL = user.MAIL.Trim();
            user.PASSWORD = user.PASSWORD.Trim();
            user.CONFIRMPASS = user.CONFIRMPASS.Trim();

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(user.MAIL, emailPattern))
            {
                ModelState.AddModelError("MAIL", "Lütfen geçerli bir e-posta adresi giriniz!");
            }

            if (UserImage == null || UserImage.ContentLength == 0)
            {
                UserUpdate.PICTURE = UserUpdate.PICTURE;
            }
            else
            {
                if (UserImage.ContentType == "image/jpeg" || UserImage.ContentType == "image/png" || UserImage.ContentType == "image/jpg" || UserImage.ContentType == "image/jfif" || UserImage.ContentType == "image/gif")
                {
                    var file = new FileInfo(UserImage.FileName);
                    var fileName = Path.GetFileName(UserImage.FileName);
                    fileName = Guid.NewGuid().ToString() + file.Extension;
                    var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                    WebImage Websize = new WebImage(UserImage.InputStream);
                    if (Websize.Width > 1000)
                        Websize.Resize(800, 800);
                    Websize.Save(path);
                    TempData["UploadedImage"] = fileName;
                    UserUpdate.PICTURE = fileName;
                }
                else
                {
                    TempData["hata"] = "Lütfen resim formatında bir dosya seçiniz !!!";
                }
            }
            if (!string.IsNullOrWhiteSpace(user.PASSWORD))
            {
                if (user.PASSWORD != user.CONFIRMPASS)
                {
                    ModelState.AddModelError("CONFIRMPASS", "Şifreler aynı değil !!!");
                }
                else
                {
                    UserUpdate.PASSWORD = user.PASSWORD;
                    UserUpdate.CONFIRMPASS = user.PASSWORD;
                    ModelState.Remove("CONFIRMPASS");
                }
            }
            UserUpdate.ABOUT = user.ABOUT;
            UserUpdate.MAIL = user.MAIL;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                TempData["Guncelle"] = "Başarıyla Güncellenmiştir !!!";
                return RedirectToAction("ProfileSetting");
            }
            return View(user);
        }
    }
}
