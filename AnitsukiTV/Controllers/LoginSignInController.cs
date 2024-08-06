using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace AnitsukiTV.Controllers
{
    public class LoginSignInController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        // GET: LoginSignIn
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(TBLUSER useradd)
        {
            TBLUSER user = new TBLUSER();
            user.USERNAME = useradd.USERNAME;
            user.MAIL = useradd.MAIL;
            user.PASSWORD = useradd.PASSWORD;
            user.STATUS = true;
            user.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            db.TBLUSER.Add(user);
            db.SaveChanges();
            TempData["add"] = "Kayıt işlemi başarılı giriş sayfasından giriş yapabilirsiniz";
            return RedirectToAction("SignIn");
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(TBLUSER user)
        {
            var degerler = db.TBLUSER.FirstOrDefault(x => x.USERNAME == user.USERNAME && x.PASSWORD == user.PASSWORD);

            if (degerler != null)
            {
                FormsAuthentication.SetAuthCookie(degerler.USERNAME, false);
                Session["username"] = degerler.USERNAME;
                Session["id"] = degerler.ID;
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.hata = "Hatalı Giriş";
            }

            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            // Çerezleri silin
            var cookies = Request.Cookies;
            foreach (string cookieName in cookies.AllKeys)
            {
                HttpCookie httpCookie = Response.Cookies.Get(cookieName);
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Set(httpCookie);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}