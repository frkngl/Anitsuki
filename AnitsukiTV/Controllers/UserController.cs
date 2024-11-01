using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            string urlUsername = RouteData.Values["userName"].ToString();

            // Kullanıcının profilini al
            var user = db.TBLUSER.Where(x => x.USERNAME.Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").ToLower() == urlUsername).FirstOrDefault();

            veri.User = user;
            ViewBag.UserName = user.USERNAME; // Kullanıcı adını ViewBag'e ekle

            // Favoriler ve İzlenecekler
            veri.Favorites = db.TBLFAVORITES.AsQueryable().Include(f => f.TBLANIME).Where(f => f.USERID == user.ID).ToList();
            veri.WatchLater = db.TBLWATCHLATER.AsQueryable().Include(f => f.TBLANIME).Where(f => f.USERID == user.ID).ToList();

            // Takipçiler (Kullanıcının takip ettiği kişiler)
            veri.Followers = db.TBLFOLLOWERS.Where(f => f.FOLLOWEDID == user.ID) .Select(f => f.TBLUSER).ToList();

            
            var takipEttikleriIDler = db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == user.ID) .Select(f => f.FOLLOWEDID).ToList();
            veri.Following = db.TBLUSER.Where(u => takipEttikleriIDler.Contains(u.ID)).ToList();

            // Kullanıcının kendi profilini ziyaret edip etmediğini kontrol et
            bool isCurrentUserProfile = (User.Identity.Name.ToLower() == urlUsername.ToLower());
            ViewBag.IsCurrentUserProfile = isCurrentUserProfile;

            return View(veri);
        }



        [HttpGet]
        public ActionResult ProfileSetting(string userName)
        {
            string username = userName;
            int userID = db.TBLUSER.Where(x => x.USERNAME.Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").ToLower() == username).FirstOrDefault().ID;
            var user = db.TBLUSER.Find(userID);
            return View("ProfileSetting", user);
        }

        [HttpPost]
        public ActionResult ProfileSetting(TBLUSER user, HttpPostedFileBase UserImage)
        {
            string username = HttpContext.User.Identity.Name;
            int userID = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault().ID;
            var UserUpdate = db.TBLUSER.Find(userID);

            if (UserImage != null && UserImage.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(UserImage.FileName).ToLower();

                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif")
                {
                    TempData["error3"] = "Lütfen resim formatında bir dosya seçiniz! Örnek a.jpg, a.jpeg, a.gif ve a.png !";
                    return View(user);
                }

                if (UserImage.ContentLength > 15 * 1024 * 1024) // 15 MB
                {
                    TempData["error5"] = "Lütfen 15 MB'den küçük bir dosya seçiniz!";
                    return View(user);
                }

                var file = new FileInfo(UserImage.FileName);
                var fileName = Path.GetFileName(UserImage.FileName);
                fileName = Guid.NewGuid().ToString() + file.Extension;
                var path = Path.Combine(Server.MapPath("~/IMG/"), fileName);

                WebImage Websize = new WebImage(UserImage.InputStream);
                if (Websize.Width > 750)
                    Websize.Resize(750, 750);
                Websize.Save(path);
                TempData["UploadedImage"] = fileName;
                UserUpdate.PICTURE = fileName;
            }

            if (string.IsNullOrWhiteSpace(user.MAIL))
            {
                TempData["error2"] = "Lütfen Mail alanınızı boş bırakmayınız!";
                return View(user);
            }
            else
            {
                user.MAIL = user.MAIL.Trim();
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(user.MAIL, emailPattern))
                {
                    TempData["error"] = "Lütfen geçerli bir Mail adresi giriniz!";
                    return View(user);
                }

                var existingUser = db.TBLUSER.FirstOrDefault(u => u.MAIL == user.MAIL && u.ID != userID);
                if (existingUser != null)
                {
                    TempData["error6"] = "Mail adresi kullanımda!";
                    return View(user);
                }
            }

            if (!string.IsNullOrWhiteSpace(user.PASSWORD))
            {
                if (user.PASSWORD != user.CONFIRMPASS)
                {
                    TempData["error4"] = "Şifreler aynı değil!";
                    ModelState.AddModelError("PASSWORD", "Şifreler aynı değil!");
                }
                else
                {
                    UserUpdate.PASSWORD = user.PASSWORD;
                    UserUpdate.CONFIRMPASS = user.PASSWORD;
                    ModelState.Remove("CONFIRMPASS");
                }
            }
            if (string.IsNullOrWhiteSpace(user.ABOUT))
            {
                UserUpdate.ABOUT = null;
            }
            else
            {
                UserUpdate.ABOUT = user.ABOUT;
            }

            UserUpdate.MAIL = user.MAIL;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                TempData["success"] = "Başarıyla Güncellenmiştir!";
                return RedirectToAction("ProfileSetting");
            }
            return View(user);
        }

        bool IsValidImageType(string contentType)
        {
            string[] validTypes = { "image/jpeg", "image/png", "image/jpg", "image/jfif", "image/gif" };
            return validTypes.Contains(contentType);
        }

    }
}
