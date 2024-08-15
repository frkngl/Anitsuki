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
        // GET: LanetlerKralıAdmin
        public ActionResult Index()
        {
            return View();
        }
    }
}