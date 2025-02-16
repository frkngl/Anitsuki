using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Xml.Linq;

namespace AnitsukiTV.Controllers
{
    public class LoginSignInController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        // GET: LoginSignIn

        [Route("kayit")]
        public ActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        [Route("kayit")]
        public ActionResult SignIn(TBLUSER useradd)
        {
            try
            {
                TBLUSER user = new TBLUSER();
                user.USERNAME = useradd.USERNAME.Trim();
                user.MAIL = useradd.MAIL.Trim();
                user.PASSWORD = useradd.PASSWORD.Trim();
                user.CONFIRMPASS = useradd.CONFIRMPASS.Trim();
                user.STATUS = true;
                user.DATE = DateTime.Now;
                // Kullanıcı adı kontrolü
                if (user.USERNAME.Length < 3 || !Regex.IsMatch(user.USERNAME, @"^(?=.*[a-zA-Z])[a-zA-Z0-9_]+$"))
                {
                    TempData["error5"] = "Kullanıcı adı en az 3 karakterden oluşmalı, en az bir harf içermeli ve sadece harf, sayı ve _ karakterlerinden oluşmalıdır.";
                    return View(useradd);
                }
                // E-posta kontrolü - E-posta için daha esnek bir regex kullanıyoruz, bu kısım değişmiyor
                if (!Regex.IsMatch(user.MAIL, @"^(?!.*\.{2})(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$"))
                {
                    TempData["error6"] = "Geçerli bir e-posta adresi giriniz.";
                    return View(useradd);
                }
                // Şifre kontrolü
                if (!Regex.IsMatch(user.PASSWORD, @"^[a-zA-Z0-9_\p{P}]+$"))
                {
                    TempData["error7"] = "Şifre sadece harf, sayı, _ ve noktalama işaretlerinden oluşmalıdır, boşluk içermemelidir.";
                    return View(useradd);
                }
                // Şifre doğrulama kontrolü
                if (!Regex.IsMatch(user.CONFIRMPASS, @"^[a-zA-Z0-9_\p{P}]+$"))
                {
                    TempData["error7"] = "Şifre sadece harf, sayı, _ ve noktalama işaretlerinden oluşmalıdır, boşluk içermemelidir.";
                    return View(useradd);
                }
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
                TempData["add"] = "Kayıt işlemi başarılı, giriş sayfasından giriş yapabilirsiniz";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["error4"] = "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin";
                return View();
            }
        }




        [HttpGet]
        [Route("giris")]
        public ActionResult Login()
        {
            return View();
        }
            
        [HttpPost]
        [Route("giris")]
        public ActionResult Login(string username, string password, bool rememberMe)
        {
            var user = db.TBLUSER.Where(x => x.USERNAME == username && x.PASSWORD == password && x.STATUS == true).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    var ticket = new FormsAuthenticationTicket(1, user.USERNAME, DateTime.Now, DateTime.Now.AddMinutes(43200), rememberMe, Guid.NewGuid().ToString());
                    var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName.ToLower(), encryptedTicket);
                    authCookie.Path = "/";
                    authCookie.HttpOnly = true;
                    authCookie.Secure = true;

                    if (rememberMe)
                    {
                        authCookie.Expires = DateTime.Now.AddMinutes(43200);
                    }
                    else
                    {
                        authCookie.Expires = DateTime.Now.AddMinutes(30);
                    }

                    Response.SetCookie(authCookie);
                    string urlUsername = username.ToLower().Replace("ı", "i").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(">", "").Replace("<", "").Replace("&", "").Replace("%", "").Replace("$", "").Replace("#", "").Replace("@", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("\\", "").Replace(".", "").Replace(",", "");
                    return RedirectToRoute("Profil", new { userName = urlUsername });
                }
                catch (Exception)
                {
                    TempData["error10"] = "Giriş yaparken hata oluştu. lütfen tekrar deneyiniz.";
                    return View();
                }
                
            }
            else
            {
                var admin = db.TBLADMIN.Where(x => x.USERNAME == username && x.PASSWORD == password && x.STATUS == true).FirstOrDefault();
                if (admin != null)
                {
                    var ticket = new FormsAuthenticationTicket(1, admin.USERNAME, DateTime.Now, DateTime.Now.AddMinutes(43200), rememberMe, Guid.NewGuid().ToString());
                    var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName.ToLower(), encryptedTicket);
                    authCookie.Path = "/";
                    authCookie.HttpOnly = true;
                    authCookie.Secure = true;

                    if (rememberMe)
                    {
                        authCookie.Expires = DateTime.Now.AddMinutes(43200);
                    }
                    else
                    {
                        authCookie.Expires = DateTime.Now.AddMinutes(30);
                    }

                    Response.SetCookie(authCookie);

                    return RedirectToAction("Index", "LanetlerKralıAdmin");
                }
                else
                {
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
            try
            {
                var user = db.TBLUSER.FirstOrDefault(x => x.MAIL.Equals(trimmedEmail, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 6);
                    user.PASSWORD = tempPassword;
                    user.CONFIRMPASS = tempPassword;
                    db.SaveChanges();

                    var mail = new MailMessage
                    {
                        From = new MailAddress("anitsuki.destek@gmail.com", "Anitsuki"),
                        Subject = "Şifre Sıfırlama",
                        Body = $"Geçici şifreniz: {tempPassword}"
                    };
                    mail.To.Add(trimmedEmail);

                    using (var smtpClient = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false, // Kullanıcı adı ve şifre kullanılacak
                        Credentials = new NetworkCredential("anitsuki.destek@gmail.com", "zlgsgoeulyrkrncv")
                    })
                    {
                        smtpClient.Send(mail);
                    }

                    TempData["success"] = "Kayıtlı mail adresinize geçici mail gönderilmiştir";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"] = "Kayıtlı mail adresiniz bulunamadı!";
                    return View();
                }
            }
            catch (Exception)
            {
                TempData["error2"] = "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }

        public ActionResult LockOut()
        {

            FormsAuthentication.SignOut();

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName.ToLower(), string.Empty) { Expires = DateTime.Now.AddDays(-1) };
            Response.Cookies.Add(authCookie);

            return Redirect("~/giris");
        }


        public ActionResult SignOut()
        {

            FormsAuthentication.SignOut();

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName.ToLower(), string.Empty) { Expires = DateTime.Now.AddDays(-1) };
            Response.Cookies.Add(authCookie);

            return Redirect("~/");
        }
    }
}