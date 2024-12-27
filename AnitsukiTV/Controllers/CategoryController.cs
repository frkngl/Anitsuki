using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    public class CategoryController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities(); 
        // GET: Category
        TabeList veri = new TabeList();

        [Route("animeler")]
        public ActionResult Index(int page = 1)
        {
            veri.Category = db.TBLCATEGORY.ToList();

            // Pagination settings
            var pageSize = 50;
            var animeQuery = db.TBLANIME.Where(x => x.STATUS == true);
            var totalAnimeCount = animeQuery.Count(); // Get total count for pagination
            var paginatedAnime = animeQuery.OrderByDescending(x => x.ID) // Random order
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList(); // Get paginated results

            veri.Anime = paginatedAnime;
            ViewBag.TotalAnimeCount = totalAnimeCount;

            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-5);

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

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 20);
            DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 2);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            // Set pagination info in ViewBag
            ViewBag.PageCount = (int)Math.Ceiling(totalAnimeCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            // Sayfa numaralarını kısaltmak için gerekli değişkenler
            ViewBag.StartPage = Math.Max(1, page - 2); // Gösterilecek ilk sayfa
            ViewBag.EndPage = Math.Min(ViewBag.PageCount, page + 2); // Gösterilecek son sayfa

            return View(veri);
        }

        [Route("animeler/{kategoriID}/{kategoriName}-izle")]
        public ActionResult UseCategory(int kategoriID, string kategoriName, int page = 1)
        {
            var degerler = db.TBLCATEGORY.FirstOrDefault(x => x.ID == kategoriID);
            veri.Category = db.TBLCATEGORY.ToList();
            // Pagination settings
            var pageSize = 50; // Sayfa başına gösterilecek anime sayısı
            veri.Anime = db.TBLANIME
                .Where(x => x.CATEGORYID == kategoriID && x.STATUS == true)
                .OrderByDescending(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Sayfalama sonuçlarını al

            ViewBag.Category = degerler.CATEGORYNAME;
            ViewBag.Category1 = degerler.CATEGORYNAME.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "");
            ViewBag.CategoryId = degerler.ID;

            // Toplam anime sayısını al
            var totalAnimeCount = db.TBLANIME.Count(x => x.CATEGORYID == kategoriID && x.STATUS == true);
            ViewBag.PageCount = (int)Math.Ceiling(totalAnimeCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            ViewBag.totalanimecount = totalAnimeCount;
            // Sayfa numaralarını kısaltmak için gerekli değişkenler
            ViewBag.StartPage = Math.Max(1, page - 2); // Gösterilecek ilk sayfa
            ViewBag.EndPage = Math.Min(ViewBag.PageCount, page + 2); // Gösterilecek son sayfa

            // Giriş yapan kullanıcının ID'sini al (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları al
            veri.Users = currentUserId.HasValue? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini al (varsa)
            var followedUserIds = currentUserId.HasValue? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // DateTime.Now.AddDays(-7) ifadesini bir değişkene atayın
            var fiveDaysAgo = DateTime.Now.AddDays(-5);

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

            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 20);
            DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 2);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(veri);
        }

    }
}