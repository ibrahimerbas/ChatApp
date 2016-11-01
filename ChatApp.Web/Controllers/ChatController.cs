using ChatApp.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Web.Controllers
{
    public class ChatController : HubController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserTemplate()
        {
            return View();
        }
        public ActionResult MessageTemplate()
        {
            return View();

        }

    }
}