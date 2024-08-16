using AnitsukiTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnitsukiTV.Controllers
{
    [Authorize]
    public class LanetlerKralıAdminController : Controller
    {
        AnitsukiTVEntities db = new AnitsukiTVEntities();
        // GET: LanetlerKralıAdmin
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Admin()
        {
            var degerler = db.TBLADMIN.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAdmin(TBLADMIN add)
        {
            TBLADMIN admin = new TBLADMIN();
            admin.USERNAME = add.USERNAME;
            admin.PASSWORD = add.PASSWORD;
            admin.ADMINROLEID = add.ADMINROLEID;
            admin.DATE = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            admin.STATUS = true;
            db.TBLADMIN.Add(admin);
            db.SaveChanges();
            TempData["success"] = "Admin başarıyla eklendi";
            return RedirectToAction("Admin");
        }
        
        public ActionResult UpdateAdmin(int id)
        {
            var FindAdmin = db.TBLADMIN.Find(id);
            return View("UpdateAdmin", FindAdmin);
        }

        public ActionResult AdminSave(TBLADMIN admin)
        {
            var updateadmin = db.TBLADMIN.Find(admin.ID);
            updateadmin.USERNAME = admin.USERNAME;
            updateadmin.PASSWORD = admin.PASSWORD;
            updateadmin.ADMINROLEID=admin.ADMINROLEID;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("Admin");
        }

        public ActionResult ActivePassive(int id)
        {
            var AdminActivePassive = db.TBLADMIN.Find(id);
            if (AdminActivePassive.STATUS == true)
            {
                AdminActivePassive.STATUS = false;
            }
            else
            {
                AdminActivePassive.STATUS = true;
            }
            db.SaveChanges();
            return RedirectToAction("Admin");
        }

        public ActionResult Sil(int id)
        {
            var AdminFind = db.TBLADMIN.Find(id);
            db.TBLADMIN.Remove(AdminFind);
            db.SaveChanges();
            TempData["error"] = "Admin başarıyla silindi";
            return RedirectToAction("Admin");
        }


        public ActionResult AdminRole()
        {
            var degerler = db.TBLADMINROLE.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRole(TBLADMINROLE add)
        {
            TBLADMINROLE admin = new TBLADMINROLE();
            admin.ROLENAME = add.ROLENAME;
            db.TBLADMINROLE.Add(admin);
            db.SaveChanges();
            TempData["success"] = "Eklendi";
            return RedirectToAction("AdminRole");
        }


        public ActionResult UpdateRole(int id)
        {
            var FindRole = db.TBLADMINROLE.Find(id);
            return View("UpdateRole", FindRole);
        }

        public ActionResult RoleSave(TBLADMINROLE role)
        {
            var updaterole = db.TBLADMINROLE.Find(role.ID);
            updaterole.ROLENAME = role.ROLENAME;
            db.SaveChanges();
            TempData["success"] = "Güncellendi";
            return RedirectToAction("AdminRole");
        }




        //public ActionResult Anime()
        //{
        //    var degerler = db.TBLANIME.ToList();
        //    return View(degerler);
        //}
    }
}