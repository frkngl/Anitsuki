using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AnitsukiTV
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Anasayfa",
                url: "",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapRoute(
                name: "Animeler",
                url: "animeler",
                defaults: new { controller = "Category", action = "Index" }
            );

            routes.MapRoute(
                name: "Kategori",
                url: "animeler/{kategoriID}/{kategoriName}-izle",
                defaults: new { controller = "Category", action = "UseCategory" }
            );

            routes.MapRoute(
                name: "Anime Detay",
                url: "anime/{animeID}/{animeName}-{seasonNumber}-sezon-izle",
                defaults: new { controller = "AnimeDetail", action = "Index" }
            );

            routes.MapRoute(
                name: "Video",
                url: "{episodeID}/{animeName}-{seasonNumber}-sezon-{episodeNumber}-bolum-izle",
                defaults: new { controller = "AnimeDetail", action = "Video" }
            );

            routes.MapRoute(
                name: "Bağış",
                url: "bagis",
                defaults: new { controller = "Home", action = "Donate" }
            );

            routes.MapRoute(
                name: "Hakkımızda",
                url: "hakkimizda",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "Gizlilik Politikası",
                url: "gizlilik-politikasi",
                defaults: new { controller = "Home", action = "PrivacyPolicy" }
            );

            routes.MapRoute(
                name: "Giriş",
                url: "giris",
                defaults: new { controller = "LoginSignIn", action = "Login" }
            );

            routes.MapRoute(
                name: "Kayıt",
                url: "kayit",
                defaults: new { controller = "LoginSignIn", action = "SignIn" }
            );

            routes.MapRoute(
                name: "Şifremi Unuttum",
                url: "sifremi-unuttum",
                defaults: new { controller = "LoginSignIn", action = "ResetPassword" }
            );

            routes.MapRoute(
                name: "Çıkış Yap",
                url: "signout",
                defaults: new { controller = "LoginSignIn", action = "SignOut" }
            );

            routes.MapRoute(
                name: "Hesabı Kilitle",
                url: "lockout",
                defaults: new { controller = "LoginSignIn", action = "LockOut" }
            );

            routes.MapRoute(
                name: "Profil", 
                url: "hesap/{userName}",
                defaults: new { controller = "User", action = "Index" }
            );

            routes.MapRoute(
                name: "Ayarlar",
                url: "hesap-ayarı/{userName}",
                defaults: new { controller = "User", action = "ProfileSetting" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = @"^(AnimeDetail|User|LanetlerKralıAdmin|Error)$" }
            );

            routes.MapRoute(
               name: "NotFound",
               url: "{*url}",
               defaults: new { controller = "Error", action = "NotFound" },
               constraints: new { url = @"^(?!LanetlerKralıAdmin).*$" } 
           );
        }
    }
}
