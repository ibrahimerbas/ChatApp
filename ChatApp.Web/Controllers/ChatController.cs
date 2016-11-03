using ChatApp.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Web.Controllers
{
    public class ChatController : HubController
    {
        const string ChatFileDir = "~/ChatFiles";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserTemplate()
        {
            return PartialView();
        }
        public ActionResult MessageTemplate()
        {
            return PartialView();

        }
        [HttpPost]
        public ActionResult UploadFile (Guid MessageID, HttpPostedFileBase File)
        {
            try
            {
                string fileName = File.FileName;
                FileInfo fileInfo = new FileInfo(fileName);
                string extension = fileInfo.Extension;
                string newFileName = (new Guid()).ToString().Replace("-", "") + extension;
                string chatFileDir = Server.MapPath(ChatFileDir);
                if (!Directory.Exists(chatFileDir))
                    Directory.CreateDirectory(chatFileDir);
                File.SaveAs(Server.MapPath(string.Format("{0}/{1}", chatFileDir, newFileName)));
                return Json(new { Result = "OK" , FilePath = Url.Content(string.Format("{0}/{1}",ChatFileDir,newFileName) });
            }
            catch (Exception e)
            {

                return Json(new { Result = "ERROR" , ErrorMessage = e.Message});
            }

           
        }

    }
}