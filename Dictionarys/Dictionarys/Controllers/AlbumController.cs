using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dictionarys.Controllers
{
    public class AlbumController : Controller
    {
        [HttpGet]
        public ActionResult MissingStamps() {
            return View();
        }
    }
}