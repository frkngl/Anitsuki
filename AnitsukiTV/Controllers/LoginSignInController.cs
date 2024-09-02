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
        public ActionResult Login(string username, string password, bool rememberMe)
        {
            var user = db.TBLUSER.Where(x => x.USERNAME == username && x.PASSWORD == password && x.STATUS == true).FirstOrDefault();
            if (user != null)
            {
                if (rememberMe)
                {
                    var ticket = new FormsAuthenticationTicket(
                        1, // version
                        user.USERNAME, // username
                        DateTime.Now, // issue date
                        DateTime.Now.AddYears(1), // expiration date (1 year)
                        true, // is persistent
                        user.USERNAME // user data
                    );

                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                    cookie.Domain = "https://anitsuki.com"; 
                    cookie.Path = "/"; 
                    cookie.Secure = true;
                    cookie.Expires = ticket.Expiration;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.USERNAME, false);
                }

                Session["username"] = user.USERNAME;
                Session["id"] = user.ID;
                return RedirectToAction("Index", "User");
            }
            else
            {
                // Kullanıcı olup olmadığını kontrol et
                var admin = db.TBLADMIN.Where(x => x.USERNAME == username && x.PASSWORD == password && x.STATUS == true).FirstOrDefault();
                if (admin != null)
                {
                    FormsAuthentication.SetAuthCookie(admin.USERNAME, false);
                    Session["username"] = admin.USERNAME;
                    Session["role"] = admin.TBLADMINROLE.ID;
                    Session["id"] = admin.ID;
                    return RedirectToAction("Index", "LanetlerKralıAdmin"); // Yönetici paneline yönlendir
                }
                else
                {
                    // Geçersiz giriş denemelerini işle
                    var userStatus = db.TBLUSER.Where(x => x.USERNAME == username && x.PASSWORD == password).FirstOrDefault();
                    if (userStatus != null && userStatus.STATUS == false)
                    {
                        TempData["error"] = "Hesabınız askıya alınmıştır";
                        return RedirectToAction("Login", "LoginSignIn");
                    }
                    else
                    {
                        TempData["error2"] = "Kullanıcı adınızı veya şifrenizi hatalı girdiniz!";
                    }
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
                mail.Body = $"Geçici şifreniz:{tempPassword}";

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

        public ActionResult LockOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "LoginSignIn");
        }


        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}