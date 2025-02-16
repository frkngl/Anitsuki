using AnitsukiTV.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
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

        [Route("anime/{animeID}/{animeName}-{seasonNumber}-sezon-izle")]
        public ActionResult Index(int animeID, string animeName, int seasonNumber, int page = 1)
        {
            // Tüm aktif animeleri çekiyoruz
            ViewBag.AllAnimes = db.TBLANIME.Where(x => x.STATUS == true).ToList();

            // Tüm sezon bilgilerini çekiyoruz
            ViewBag.SeasonInfo = db.TBLSEASON.ToList();
            ViewBag.deger = animeID;

            // Belirli anime ID'sine sahip animeyi buluyoruz
            var degerler2 = db.TBLANIME.FirstOrDefault(x => x.ID == animeID);
            ViewBag.animeeee = degerler2?.ANIMENAME; // Anime adını ViewBag'e atıyoruz
            ViewBag.banner = degerler2?.BANNER; // Anime banner'ını ViewBag'e atıyoruz
            ViewBag.animee = degerler2?.ANIMENAME?.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "") ?? "";
            ViewBag.AnimeId = degerler2?.ID ?? 0; // Anime ID'sini ViewBag'e atıyoruz

            // Sezon ID değerini buluyoruz
            int sezonIDValue = db.TBLSEASON.FirstOrDefault(x => x.ANIMEID == animeID && x.SEASONNUMBER == seasonNumber)?.ID ?? 0;

            // Anime, sezon ve bölümleri veri modeline ekliyoruz
            veri.Anime = db.TBLANIME.Where(x => x.ID == animeID).ToList();
            veri.Season = db.TBLSEASON.Where(x => x.ANIMEID == animeID).ToList();
            veri.Episode = db.TBLEPISODE.Where(x => x.ANIMEID == animeID && x.SEASONID == sezonIDValue).ToList();

            // Seçilen sezonu buluyoruz
            var selectedSeason = db.TBLSEASON.FirstOrDefault(x => x.ID == sezonIDValue);
            ViewBag.Season = selectedSeason?.SEASONNAME ?? $"{selectedSeason?.SEASONNUMBER} Sezon"; // Sezon adını veya numarasını ViewBag'e atıyoruz
            ViewBag.Seasonnn = selectedSeason?.SEASONNUMBER ?? 0; // Sezon numarasını ViewBag'e atıyoruz

            bool isFavorite = false; // Anime favori mi?
            bool isWatchLater = false; // Anime izlenecekler listesinde mi?

            // Kullanıcı giriş yapmışsa
            if (User.Identity.IsAuthenticated)
            {
                int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID ?? 0;
                var favorite = db.TBLFAVORITES.FirstOrDefault(x => x.USERID == userID && x.ANIMEID == animeID);
                isFavorite = favorite != null; // Anime favori mi kontrolü

                var watchlater = db.TBLWATCHLATER.FirstOrDefault(x => x.USERID == userID && x.ANIMEID == animeID);
                isWatchLater = watchlater != null; // Anime izlenecekler listesinde mi kontrolü
            }

            ViewBag.IsFavorite = isFavorite; // Favori durumunu ViewBag'e atıyoruz
            ViewBag.isWatchLater = isWatchLater; // İzlenecekler listesi durumunu ViewBag'e atıyoruz

            // Giriş yapan kullanıcının ID'sini alıyoruz (varsa)
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.FirstOrDefault(x => x.USERNAME == User.Identity.Name)?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları alıyoruz
            veri.Users = currentUserId.HasValue ? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini alıyoruz (varsa)
            var followedUserIds = currentUserId.HasValue ? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds; // Takip edilen kullanıcı ID'lerini ViewBag'e atıyoruz

            // 5 gün önceki tarihi belirliyoruz
            var fiveDaysAgo = DateTime.Now.AddDays(-5);

            if (currentUserId.HasValue)
            {
                // Kullanıcının bildirimlerini kontrol ediyoruz ve 5 günden eski olanları temizliyoruz
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true; // Bildirimi temizle
                }

                db.SaveChanges(); // Değişiklikleri kaydet

                // Giriş yapan kullanıcının güncel bildirimlerini alıyoruz
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>(); // Kullanıcı giriş yapmamışsa boş bir liste oluşturuyoruz
            }

            // Tüm yorumları çek
            var allComments = db.TBLANIMECOMMENT.Where(c => c.ANIMEID == animeID && c.STATUS == true).ToList();

            // Yorumları kullanıcıya özel sırala
            if (User.Identity.IsAuthenticated)
            {
                var userID = db.TBLUSER.Where(u => u.USERNAME == User.Identity.Name).Select(u => u.ID).FirstOrDefault();

                // Kullanıcıya ait yorumlar ve bunlara yapılan cevapları topla
                var userRelatedComments = new HashSet<TBLANIMECOMMENT>(allComments.Where(c => c.USERID == userID));

                // Kullanıcıya bağlı yorum ağacını oluştur
                var currentChain = userRelatedComments.ToList();
                while (currentChain.Any())
                {
                    var newChain = new HashSet<TBLANIMECOMMENT>();
                    foreach (var comment in currentChain)
                    {
                        if (comment.USTID.HasValue)
                        {
                            var parent = allComments.FirstOrDefault(c => c.ID == comment.USTID);
                            if (parent != null && !userRelatedComments.Contains(parent))
                            {
                                userRelatedComments.Add(parent);
                                newChain.Add(parent);
                            }
                        }
                        var replies = allComments.Where(c => c.USTID == comment.ID);
                        foreach (var reply in replies)
                        {
                            if (!userRelatedComments.Contains(reply))
                            {
                                userRelatedComments.Add(reply);
                                newChain.Add(reply);
                            }
                        }
                    }
                    currentChain = newChain.ToList();
                }

                // Kullanıcının en son yaptığı yorumu veya cevabı bul
                var userLatestComment = allComments.Where(c => c.USERID == userID).OrderByDescending(c => c.DATE).FirstOrDefault();

                // Kullanıcıya ait yorumlar, verdiği cevaplar ve aldığı cevaplar en üstte olacak şekilde sırala
                allComments = allComments.OrderByDescending(c => c == userLatestComment) // Kullanıcının son yorumu en üste
                                          .ThenByDescending(c => userRelatedComments.Contains(c) && c != userLatestComment) // Diğer kullanıcıya ait veya ilgili yorumlar
                                          .ThenByDescending(c => c.ID).ToList(); // Sonra ID'ye göre sırala
            }
            else
            {
                // Kullanıcı giriş yapmamışsa, yorumları ID'ye göre ters sırala
                allComments = allComments.OrderByDescending(x => x.ID).ToList();
            }

            // Sadece USTID'si null olan yani ana yorumları sayfala
            var rootComments = allComments.Where(x => x.USTID == null).ToList();

            // Sayfalandırma işlemi, her sayfada 1 ana yorum gösterecek şekilde
            var pagedRootComments = rootComments.ToPagedList(page, 15);

            ViewBag.AnimeCount = allComments.Count; // Tüm yorumların sayısını ViewBag'e atıyoruz
            ViewBag.anime = animeID; // Anime ID'sini ViewBag'e atıyoruz
            ViewBag.animeName = db.TBLANIME.Find(animeID)?.ANIMENAME; // Anime adını ViewBag'e atıyoruz

            veri.AnimeComments = allComments; // Tüm yorumlar, kullanıcıya özel sıralanmış
            veri.PagedRootComments = pagedRootComments; // Sayfalanmış ana yorumlar

            // Set pagination info in ViewBag
            ViewBag.PageCount = pagedRootComments.PageCount; // Toplam sayfa sayısını ViewBag'e atıyoruz
            ViewBag.CurrentPage = pagedRootComments.PageNumber; // Mevcut sayfa numarasını ViewBag'e atıyoruz


            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 20);
            DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 2);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Check if the current date is within the activation range
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate; // Belirli bir dönemde özelliğin aktif olup olmadığını kontrol ediyoruz

            return View(veri); // Görünüme veri modelini iletiyoruz
        }

        [HttpPost]
        public ActionResult LeaveComment(int animeID, string animeName, int seasonNumber, TBLANIMECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();
            try
            {
                if (string.IsNullOrWhiteSpace(y.COMMENT) || y.COMMENT.Any(c => (char.IsControl(c) && c != '\r' && c != '\n') || (c == '\u00A0' || c == '\u200B' || c == '\u200C' || c == '\u200D' || c == '\uFEFF')))
                {
                    TempData["messageerror"] = "Yorumunuz boş olamaz veya kontrol karakterleri içeremez. Lütfen bir yorum girin.";
                    return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
                }

                y.DATE = DateTime.Now;
                y.STATUS = true;
                y.TBLUSER = user;
                y.SPOILER = Request.Form["SPOILER"] == "true";

                db.TBLANIMECOMMENT.Add(y);
                db.SaveChanges();

                TempData["success"] = "Yorumunuz yapılmıştır.";
                return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
            }
            catch (Exception)
            {
                TempData["error"] = "Yorum yaparken bir hata oluştu. Lütfen tekrar deneyin.";
                return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
            }
        }


        [HttpPost]
        public ActionResult ReplyComment(int animeID, string animeName, int seasonNumber, TBLANIMECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();
            try
            {
                if (string.IsNullOrWhiteSpace(y.COMMENT) || y.COMMENT.Any(c => (char.IsControl(c) && c != '\r' && c != '\n') || (c == '\u00A0' || c == '\u200B' || c == '\u200C' || c == '\u200D' || c == '\uFEFF')))
                {
                    TempData["messageerror1"] = "Yorumunuz boş olamaz veya kontrol karakterleri içeremez. Lütfen bir yorum girin.";
                    return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
                }

                y.DATE = DateTime.Now;
                y.STATUS = true;
                y.TBLUSER = user;
                y.SPOILER = Request.Form["SPOILER"] == "true";

                // Yeni yorumu veritabanına ekler
                db.TBLANIMECOMMENT.Add(y);
                db.SaveChanges();

                // Cevap verilen yorumun kullanıcısı için bildirim oluştur
                var parentComment = db.TBLANIMECOMMENT.Find(y.USTID);
                if (parentComment != null && parentComment.TBLUSER != null)
                {
                    var notification = new TBLNOTIFICATIONS
                    {
                        USERID = parentComment.TBLUSER.ID,
                        MESSAGE = user.USERNAME + " " + "adlı kullanıcıdan yorumunuza cevap geldi.",
                        USERNAME = user.USERNAME,
                        PROFILEPICTURE = user.PICTURE, 
                        CREATED = DateTime.Now,
                        ISCLEARED = false,
                        ANIMEID = animeID,
                        EPISODEID = null,
                        ADMINSTATUS = false
                    };

                    db.TBLNOTIFICATIONS.Add(notification);
                    db.SaveChanges();
                }

                TempData["success1"] = "Yoruma cevap yapılmıştır.";
                return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
            }
            catch (Exception)
            {
                TempData["error1"] = "Yoruma cevap yaparken bir hata oluştu. Lütfen tekrar deneyin.";
                return Redirect($"/anime/{animeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-izle");
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

        [HttpGet]
        public ActionResult CheckFavorite(int animeID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, isActive = false }, JsonRequestBehavior.AllowGet);
            }

            int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault().ID;
            var favorite = db.TBLFAVORITES.FirstOrDefault(x => x.USERID == userID && x.ANIMEID == animeID);

            return Json(new { success = true, isActive = favorite != null }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckWatchLater(int animeID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, isActive = false }, JsonRequestBehavior.AllowGet);
            }

            int userID = db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault().ID;
            var watchlater = db.TBLWATCHLATER.FirstOrDefault(x => x.USERID == userID && x.ANIMEID == animeID);

            return Json(new { success = true, isActive = watchlater != null }, JsonRequestBehavior.AllowGet);
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



        [Route("{episodeID}/{animeName}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle/{page?}")]
        public ActionResult Video(int? episodeID, string animeName, int? seasonNumber, int? episodeNumber, int page = 1)
        {
            // Tüm animeleri veri deposundan çeker
            veri.Anime = db.TBLANIME.ToList();
            ViewBag.episode1 = episodeID;
            // Belirli bir bölümü veritabanından getirir
            var degerler = db.TBLEPISODE.FirstOrDefault(x => x.ID == episodeID);

            // Banner bilgisini ViewBag'e ekler
            ViewBag.banner = degerler.TBLANIME.BANNER;

            // Durumu aktif olan belirli bir bölümü getirir
            veri.Episode = db.TBLEPISODE.Where(x => x.ID == episodeID && x.STATUS == true).ToList();

            // Anime ID'sini alır
            int animeId = veri.Episode.FirstOrDefault()?.TBLANIME?.ID ?? 0;
            ViewBag.episodeID = degerler.ID;

            // Sezon numarasını alır
            int sezonNumarası = veri.Episode.FirstOrDefault()?.TBLSEASON?.SEASONNUMBER ?? 0;

            // Önceki bölümün ID'sini bulur
            int oncekiBolumId = db.TBLEPISODE
                .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID < episodeID)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault()?.ID ?? 0;

            // Eğer önceki bölüm bulunamazsa, bir önceki sezondan son bölümü alır
            if (oncekiBolumId == 0)
            {
                int oncekiSezonNumarası = sezonNumarası - 1;
                oncekiBolumId = db.TBLEPISODE
                    .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == oncekiSezonNumarası)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault()?.ID ?? 0;
            }
            veri.OncekiBolum = db.TBLEPISODE.Where(x => x.ID == oncekiBolumId).ToList();

            // Sonraki bölümün ID'sini bulur
            int sonrakiBolumId = db.TBLEPISODE
                .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sezonNumarası && x.ID > episodeID)
                .OrderBy(x => x.ID)
                .FirstOrDefault()?.ID ?? 0;

            // Eğer sonraki bölüm bulunamazsa, bir sonraki sezondan ilk bölümü alır
            if (sonrakiBolumId == 0)
            {
                int sonrakiSezonNumarası = sezonNumarası + 1;
                sonrakiBolumId = db.TBLEPISODE
                    .Where(x => x.TBLANIME.ID == animeId && x.TBLSEASON.SEASONNUMBER == sonrakiSezonNumarası)
                    .OrderBy(x => x.ID)
                    .FirstOrDefault()?.ID ?? 0;
            }
            veri.SonrakiBolum = db.TBLEPISODE.Where(x => x.ID == sonrakiBolumId).ToList();

            // Giriş yapmış kullanıcının ID'sini alır
            int? currentUserId = User.Identity.IsAuthenticated ? db.TBLUSER.Where(x => x.USERNAME == User.Identity.Name).FirstOrDefault()?.ID : (int?)null;

            // Giriş yapan kullanıcı hariç tüm kullanıcıları alır
            veri.Users = currentUserId.HasValue ? db.TBLUSER.Where(u => u.ID != currentUserId.Value).ToList() : db.TBLUSER.ToList();

            // Kullanıcının takip ettiği kullanıcıların ID'lerini alır
            var followedUserIds = currentUserId.HasValue ? db.TBLFOLLOWERS.Where(f => f.FOLLOWERID == currentUserId.Value).Select(f => f.FOLLOWEDID.Value).ToList() : new List<int>();

            ViewBag.FollowedUserIds = followedUserIds;

            // 5 gün öncesini hesaplar
            var fiveDaysAgo = DateTime.Now.AddDays(-5);

            if (currentUserId.HasValue)
            {
                // Kullanıcının bildirimlerini kontrol eder ve 5 günden eski olanları temizler
                var oldNotifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false && n.CREATED < fiveDaysAgo).ToList();

                foreach (var notification in oldNotifications)
                {
                    notification.ISCLEARED = true;
                }

                db.SaveChanges();

                // Giriş yapan kullanıcının güncel bildirimlerini alır
                veri.Notifications = db.TBLNOTIFICATIONS.Where(n => n.USERID == currentUserId.Value && n.ISCLEARED == false).ToList();
            }
            else
            {
                veri.Notifications = new List<TBLNOTIFICATIONS>();
            }

            // Bölüm yorumlarını getirir
            var allEpisodeComments = db.TBLEPISODECOMMENT.Where(x => x.EPISODEID == episodeID && x.STATUS == true).ToList();

            // Yorumları kullanıcıya özel sırala
            if (User.Identity.IsAuthenticated)
            {
                int userID = db.TBLUSER.Where(u => u.USERNAME == User.Identity.Name).Select(u => u.ID).FirstOrDefault();

                // Kullanıcıya ait yorumlar ve bunlara yapılan cevapları topla
                var userRelatedComments = new HashSet<TBLEPISODECOMMENT>(allEpisodeComments.Where(c => c.USERID == userID));

                // Kullanıcıya bağlı yorum ağacını oluştur
                var currentChain = userRelatedComments.ToList();
                while (currentChain.Any())
                {
                    var newChain = new HashSet<TBLEPISODECOMMENT>();
                    foreach (var comment in currentChain)
                    {
                        if (comment.USTID.HasValue)
                        {
                            var parent = allEpisodeComments.FirstOrDefault(c => c.ID == comment.USTID);
                            if (parent != null && !userRelatedComments.Contains(parent))
                            {
                                userRelatedComments.Add(parent);
                                newChain.Add(parent);
                            }
                        }
                        var replies = allEpisodeComments.Where(c => c.USTID == comment.ID);
                        foreach (var reply in replies)
                        {
                            if (!userRelatedComments.Contains(reply))
                            {
                                userRelatedComments.Add(reply);
                                newChain.Add(reply);
                            }
                        }
                    }
                    currentChain = newChain.ToList();
                }

                // Kullanıcının en son yaptığı yorumu veya cevabı bul
                var userLatestComment = allEpisodeComments.Where(c => c.USERID == userID).OrderByDescending(c => c.DATE).FirstOrDefault();

                // Kullanıcıya ait yorumlar, verdiği cevaplar ve aldığı cevaplar en üstte olacak şekilde sırala
                allEpisodeComments = allEpisodeComments.OrderByDescending(c => c == userLatestComment) // Kullanıcının son yorumu en üste
                                                       .ThenByDescending(c => userRelatedComments.Contains(c) && c != userLatestComment) // Diğer kullanıcıya ait veya ilgili yorumlar
                                                       .ThenByDescending(c => c.ID).ToList(); // Sonra ID'ye göre sırala
            }
            else
            {
                // Kullanıcı giriş yapmamışsa, yorumları ID'ye göre ters sırala
                allEpisodeComments = allEpisodeComments.OrderByDescending(x => x.ID).ToList();
            }

            // Sadece USTID'si null olan yani ana yorumları sayfala
            var rootComments = allEpisodeComments.Where(x => x.USTID == null).ToList();

            // Sayfalandırma işlemi, her sayfada 20 ana yorum gösterecek şekilde
            var pagedRootComments = rootComments.ToPagedList(page, 20);

            ViewBag.EpisodeCount = allEpisodeComments.Count;
            ViewBag.episode = episodeID;
            ViewBag.animeName = db.TBLEPISODE.Find(episodeID).TBLANIME.ANIMENAME.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "");
            ViewBag.seasonNumber = seasonNumber ?? 0; // Eğer sezon numarası gelmezse 0 olması için
            ViewBag.episodeNumber = episodeNumber ?? 0; // Eğer bölüm numarası gelmezse 0 olması için

            veri.EpisodeComments = allEpisodeComments;
            veri.PageddRootComments = pagedRootComments;

            // Set pagination info in ViewBag
            ViewBag.PageCount = pagedRootComments.PageCount;
            ViewBag.CurrentPage = pagedRootComments.PageNumber;

            var tabeList = new TabeList
            {
                VideoInfo = new VideoViewModel
                {
                    Episode = veri.Episode,
                    AnimeTitle = veri.Episode.FirstOrDefault()?.TBLANIME?.ANIMENAME,
                    SeasonNumber = veri.Episode.FirstOrDefault()?.TBLSEASON?.SEASONNUMBER ?? 0,
                    EpisodeNumber = veri.Episode.FirstOrDefault()?.EPINUMBER ?? 0,
                    OncekiBolum = veri.OncekiBolum,
                    SonrakiBolum = veri.SonrakiBolum,
                    Notification = veri.Notifications,
                    Users = veri.Users,
                    Anime = veri.Anime,
                    EpisodeComment = veri.EpisodeComments,
                    PageddRootComments = veri.PageddRootComments
                }
            };

            // Özellik aktivasyon tarihlerini belirler
            DateTime startDate = new DateTime(DateTime.Now.Year, 12, 20);
            DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 2);

            // Güncel tarihi alır
            DateTime currentDate = DateTime.Now;

            // Özelliğin etkin olup olmadığını kontrol eder
            ViewBag.IsFeatureActive = currentDate >= startDate && currentDate <= endDate;

            return View(tabeList);
        }

        [HttpPost]
        public ActionResult ReportEpisode(int episodeID, string animeName, int seasonNumber, int episodeNumber)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user;
            bool isGuest = false;
            if (string.IsNullOrEmpty(username))
            {
                user = new TBLUSER { USERNAME = "Misafir Kullanıcı", PICTURE = null };
                isGuest = true;
            }
            else
            {
                user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();
            }
            try
            {
                var episode = db.TBLEPISODE.FirstOrDefault(x => x.ID == episodeID);
                if (episode != null)
                {
                    var notification = new TBLNOTIFICATIONS
                    {
                        MESSAGE = $"Videoda sorun var: {animeName.Replace("-", " ")} {seasonNumber}. sezon {episodeNumber}. bölüm",
                        USERID = isGuest ? (int?)null : user.ID,
                        USERNAME = isGuest ? "Misafir Kullanıcı" : user.USERNAME,
                        PROFILEPICTURE = isGuest ? null : user.PICTURE,
                        CREATED = DateTime.Now,
                        ISCLEARED = false,
                        ANIMEID = episode.ANIMEID,
                        EPISODEID = episodeID,
                        ADMINSTATUS = true
                    };
                    db.TBLNOTIFICATIONS.Add(notification);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Episode not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult LeaveComment1(int episodeID, string animeName, int seasonNumber, int episodeNumber, TBLEPISODECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();

            try
            {

                if (string.IsNullOrWhiteSpace(y.COMMENT) || y.COMMENT.Any(c => (char.IsControl(c) && c != '\r' && c != '\n') || (c == '\u00A0' || c == '\u200B' || c == '\u200C' || c == '\u200D' || c == '\uFEFF')))
                {
                    TempData["messageerror"] = "Yorumunuz boş olamaz veya kontrol karakterleri içeremez. Lütfen bir yorum girin.";
                    return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
                }


                y.DATE = DateTime.Now;
                y.STATUS = true;
                y.TBLUSER = user;
                y.SPOILER = Request.Form["SPOILER"] == "true";

                db.TBLEPISODECOMMENT.Add(y);
                db.SaveChanges();

                TempData["success"] = "Yorumunuz yapılmıştır.";
                return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
            }
            catch (Exception)
            {
                TempData["error"] = "Yorum yaparken bir hata oluştu. Lütfen tekrar deneyin.";
                return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
            }
        }

        [HttpPost]
        public ActionResult ReplyComment1(int episodeID, string animeName, int seasonNumber, int episodeNumber, TBLEPISODECOMMENT y)
        {
            string username = HttpContext.User.Identity.Name;
            TBLUSER user = db.TBLUSER.Where(x => x.USERNAME == username).FirstOrDefault();
            var animeID = db.TBLEPISODE.Where(x => x.ID == episodeID).Select(x => (int?)x.ANIMEID).FirstOrDefault();

            try
            {
                if (string.IsNullOrWhiteSpace(y.COMMENT) || y.COMMENT.Any(c => (char.IsControl(c) && c != '\r' && c != '\n') || (c == '\u00A0' || c == '\u200B' || c == '\u200C' || c == '\u200D' || c == '\uFEFF')))
                {
                    TempData["messageerror1"] = "Yorumunuz boş olamaz veya kontrol karakterleri içeremez. Lütfen bir yorum girin.";
                    return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
                }

                y.DATE = DateTime.Now;
                y.STATUS = true;
                y.TBLUSER = user;
                y.SPOILER = Request.Form["SPOILER"] == "true";

                db.TBLEPISODECOMMENT.Add(y);
                db.SaveChanges();

                // Cevap verilen yorumun kullanıcısı için bildirim oluştur
                var parentComment = db.TBLEPISODECOMMENT.Find(y.USTID);
                if (parentComment != null && parentComment.TBLUSER != null)
                {
                    var notification = new TBLNOTIFICATIONS
                    {
                        USERID = parentComment.TBLUSER.ID,
                        MESSAGE = user.USERNAME + " " + "adlı kullanıcıdan yorumunuza cevap geldi.",
                        USERNAME = user.USERNAME,
                        PROFILEPICTURE = user.PICTURE,
                        CREATED = DateTime.Now,
                        ISCLEARED = false,
                        ANIMEID = animeID,
                        EPISODEID = episodeID,
                        ADMINSTATUS = false
                    };

                    db.TBLNOTIFICATIONS.Add(notification);
                    db.SaveChanges();
                }

                TempData["success1"] = "Yoruma cevap yapılmıştır.";
                return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
            }
            catch (Exception)
            {
                TempData["error1"] = "Yoruma cevap yaparken bir hata oluştu. Lütfen tekrar deneyin.";
                return Redirect($"/{episodeID}/{animeName.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "")}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle");
            }            
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