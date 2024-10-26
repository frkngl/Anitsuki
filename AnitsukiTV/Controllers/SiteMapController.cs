using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace AnitsukiTV.Controllers
{
    public class SiteMapController : Controller
    {
        public ActionResult Index()
        {
            var urls = new List<string>();

            // Anasayfa
            urls.Add(Url.Action("Index", "Home", null, Request.Url.Scheme));

            // Animeler
            urls.Add(Url.Action("Index", "Category", null, Request.Url.Scheme));

            // Kategori
            // Bu URL'ler dinamik olarak oluşturulduğu için, örnek olarak bir kategoriID ve kategoriName ekliyoruz.
            urls.Add(Url.Action("UseCategory", "Category", new { kategoriID = 1, kategoriName = "ornek-kategori" }, Request.Url.Scheme));

            // Anime Detay
            // Bu URL'ler dinamik olarak oluşturulduğu için, örnek olarak bir animeID, animeName ve seasonNumber ekliyoruz.
            urls.Add(Url.Action("Index", "AnimeDetail", new { animeID = 1, animeName = "ornek-anime", seasonNumber = 1 }, Request.Url.Scheme));

            // Video
            // Bu URL'ler dinamik olarak oluşturulduğu için, örnek olarak bir episodeID, animeName, seasonNumber ve episodeNumber ekliyoruz.
            urls.Add(Url.Action("Video", "AnimeDetail", new { episodeID = 1, animeName = "ornek-anime", seasonNumber = 1, episodeNumber = 1 }, Request.Url.Scheme));

            // Bağış
            urls.Add(Url.Action("Donate", "Home", null, Request.Url.Scheme));

            // Hakkımızda
            urls.Add(Url.Action("About", "Home", null, Request.Url.Scheme));

            // Gizlilik Politikası
            urls.Add(Url.Action("PrivacyPolicy", "Home", null, Request.Url.Scheme));

            // Giriş
            urls.Add(Url.Action("Login", "LoginSignIn", null, Request.Url.Scheme));

            // Kayıt
            urls.Add(Url.Action("SignIn", "LoginSignIn", null, Request.Url.Scheme));

            // Şifremi Unuttum
            urls.Add(Url.Action("ResetPassword", "LoginSignIn", null, Request.Url.Scheme));

            // Çıkış Yap
            urls.Add(Url.Action("SignOut", "LoginSignIn", null, Request.Url.Scheme));

            // Hesabı Kilitle
            urls.Add(Url.Action("LockOut", "LoginSignIn", null, Request.Url.Scheme));

            var sitemap = new XElement("urlset",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xmlns", "http://www.sitemaps.org/schemas/sitemap-image/1.1")
            );

            foreach (var url in urls)
            {
                sitemap.Add(new XElement("url",
                    new XElement("loc", url),
                    new XElement("changefreq", "weekly"),
                    new XElement("priority", "0.5")
                ));
            }

            return Content(sitemap.ToString(), "text/xml");
        }
    }
}