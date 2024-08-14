using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
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
            // Create a new user object
            TBLUSER user = new TBLUSER();
            user.USERNAME = useradd.USERNAME.Trim();
            user.MAIL = useradd.MAIL.Trim();
            user.PASSWORD = useradd.PASSWORD.Trim();
            user.CONFIRMPASS = useradd.CONFIRMPASS.Trim();
            user.STATUS = true;
            user.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            var UserName = db.TBLUSER.FirstOrDefault(u => u.USERNAME == user.USERNAME);
            if (UserName != null)
            {
                TempData["error"] = "Kullanıcı Adı kullanımda";
                return View(useradd);
            }

            var UserMail = db.TBLUSER.FirstOrDefault(u => u.MAIL == user.MAIL);
            if (UserMail != null)
            {
                TempData["error2"] = "Mail adresi kullanımda";
                return View(useradd);
            }

            if (user.PASSWORD != user.CONFIRMPASS)
            {
                TempData["error3"] = "Şifreler aynı değil";
                return View(useradd);
            }

            db.TBLUSER.Add(user);
            db.SaveChanges();
            TempData["add"] = "Kayıt işlemi başarılı giriş sayfasından, giriş yapabilirsiniz";
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
             var degerler = db.TBLUSER.Where(x => x.USERNAME == user.USERNAME && x.PASSWORD == user.PASSWORD && x.STATUS == true).FirstOrDefault();
             if (degerler != null)
             {
                 FormsAuthentication.SetAuthCookie(degerler.USERNAME, true);
                 Session["username"] = degerler.USERNAME;
                 Session["id"] = degerler.ID;
                 return RedirectToAction("Index", "User");
             }
             else
             {
                 var userStatus = db.TBLUSER.Where(x => x.USERNAME == user.USERNAME && x.PASSWORD == user.PASSWORD).FirstOrDefault();
                 if (userStatus != null && userStatus.STATUS == false)
                 {
                       TempData["error"] = "Hesabınız askıya alınmıştır";
                       return RedirectToAction("Login");
                 }
                 else
                 {
                       TempData["error2"] = "Kullanıcı adınızı veya şifrenizi hatalı girdiniz!";
                 }
             }
                 return View();
         }



        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.hata = "Mail adresi boş olamaz!";
                return View();
            }

            string trimmedEmail = email.Trim();
            var user = db.TBLUSER.FirstOrDefault(x => x.MAIL.ToString().Equals(trimmedEmail, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 6);
                user.PASSWORD = tempPassword;
                user.CONFIRMPASS = tempPassword;
                db.SaveChanges();

                var mail = new MailMessage();
                mail.From = new MailAddress("info@anitsuki.com", "Anitsuki");
                mail.To.Add(trimmedEmail);
                mail.Subject = "Şifre Sıfırlama";
                mail.Body = $"Geçici şifreniz: {tempPassword}";

                using (var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("info@anitsuki.com", "hlvimrynpfxqlytm")
                })
                {
                    smtpClient.Send(mail);
                }
                TempData["success"] = "Kayıtlı mail adresinize geçici mail gönderilmiştir";
                return View();
            }
            else
            {
                TempData["error"] = "Kayıtlı mail adresiniz bulunamadı!";
                return View();
            }
        }



        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}