using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    public class HomeController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();   
        TabeList veri = new TabeList();

        [Route("")]
        public ActionResult Index()
        {
            veri.Episode = db.TBLEPISODE.ToList();
            veri.Category = db.TBLCATEGORY.ToList();
            veri.Anime = db.TBLANIME.ToList();

            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-7);

            if (currentUserId.HasValue)
            {

                var followedUserFavorites = db.TBLFAVORITES.Where(f => f.USERID.HasValue && followedUserIds.Contains(f.USERID.Value)).Select(f => f.ANIMEID).Distinct().ToList();

                // Takip edilen kullanıcıların favori animelerini al
                veri.FriendsAnime = db.TBLANIME.Where(a => followedUserFavorites.Contains(a.ID) && a.STATUS == true).OrderBy(a => Guid.NewGuid()).Take(12).ToList();


                // Kullanıcının bildirimlerini kontrol et ve 5 günden eski olanları temizle
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true; // Bildirimi temizle
                }

                db.SaveChanges(); // Değişiklikleri kaydet

                // Giriş yapan kullanıcının güncel bildirimlerini al
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>();
            }

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 25);
            DateTime endDate = new DateTime(DateTime.Now.Year + (DateTime.Now.Month == 1 && DateTime.Now.Day <= 5 ? 0 : 1), 1, 5);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(veri);
        }


        [Route("gizlilik-politikasi")]  
        public ActionResult PrivacyPolicy()
        {
            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue ? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue ? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-7);

            if (currentUserId.HasValue)
            {
                // Kullanıcının bildirimlerini kontrol et ve 5 günden eski olanları temizle
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true; // Bildirimi temizle
                }

                db.SaveChanges(); // Değişiklikleri kaydet

                // Giriş yapan kullanıcının güncel bildirimlerini al
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>();
            }

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 25);
            DateTime endDate = new DateTime(DateTime.Now.Year + (DateTime.Now.Month == 1 && DateTime.Now.Day <= 5 ? 0 : 1), 1, 5);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(veri);
        }

        [Route("hakkimizda")]
        public ActionResult About()
        {
            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue ? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue ? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-7);

            if (currentUserId.HasValue)
            {
                // Kullanıcının bildirimlerini kontrol et ve 5 günden eski olanları temizle
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true; // Bildirimi temizle
                }

                db.SaveChanges(); // Değişiklikleri kaydet

                // Giriş yapan kullanıcının güncel bildirimlerini al
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>();
            }

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 25);
            DateTime endDate = new DateTime(DateTime.Now.Year + (DateTime.Now.Month == 1 && DateTime.Now.Day <= 5 ? 0 : 1), 1, 5);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(veri);
        }

        [Route("bagis")]
        public ActionResult Donate()
        {
            veri.Donate = db.TBLDONATE.Where(x => x.STATUS == true).OrderByDescending(x => x.DONATE).Take(20).ToList();
            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue ? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue ? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-7);

            if (currentUserId.HasValue)
            {
                // Kullanıcının bildirimlerini kontrol et ve 5 günden eski olanları temizle
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true; // Bildirimi temizle
                }

                db.SaveChanges(); // Değişiklikleri kaydet

                // Giriş yapan kullanıcının güncel bildirimlerini al
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>();
            }

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 25);
            DateTime endDate = new DateTime(DateTime.Now.Year + (DateTime.Now.Month == 1 && DateTime.Now.Day <= 5 ? 0 : 1), 1, 5);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(veri);
        }
    }
}