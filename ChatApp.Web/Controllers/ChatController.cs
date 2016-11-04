using ChatApp.Data.Repository;
using ChatApp.Web.Helpers;
using ChatApp.Web.Models;
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
                if (!User.Identity.IsAuthenticated)
                    throw new Exception("Giriş yap da gel! Mevlana değiliz!");
                ChatMessageRepository cRepo = new ChatMessageRepository();
                var message = cRepo.Get(MessageID);
                if (message == null)
                    throw new Exception("Mesaj bulunamadı. Açık mı arıyon hacı?");

                string fileName = File.FileName;
                FileInfo fileInfo = new FileInfo(fileName);
                string extension = fileInfo.Extension;
                string newFileName = (new Guid()).ToString().Replace("-", "") + extension;
                string chatFileDir = Server.MapPath(ChatFileDir);
                if (!Directory.Exists(chatFileDir))
                    Directory.CreateDirectory(chatFileDir);
                string fullFilePath = Server.MapPath(string.Format("{0}/{1}", chatFileDir, newFileName));
                File.SaveAs(fullFilePath);
                string absoluteFilePath = Url.Content(string.Format("{0}/{1}", ChatFileDir, newFileName));
                try
                {
                    message.FilePath = absoluteFilePath;
                    cRepo.Save(message);

                }
                catch (Exception)
                {
                    if(System.IO.File.Exists(fullFilePath))
                    System.IO.File.Delete(fullFilePath);
                    throw;
                }

                return Json(new { Result = "OK" , FilePath = absoluteFilePath });
            }
            catch (Exception e)
            {

                return Json(new { Result = "ERROR" , ErrorMessage = e.Message});
            }
            
        
           
        }
        public ActionResult UploaderTest() {
            ViewData["UploadFile"] = new UploadModel() { GroupName = "MessageFile", Caption = "Dosya Yükle", UploaderOptions = "{action:'" + Url.Action("UploadFile", "Chat") + "',singleFile:false,group:'MessageFile',allFilesUploaded:function (groupName, options) { $('#lockscreen-uploader').hide();},uploadFinished:function(instance,file,data){$('#thumbImageThumb' + file.CurrentOptions.parameters.id).attr('src' , data.imagePath + '?' + Math.random());},createItemFunction:CreateFileItem,fileRemoved:UploaderFileRemoved,containerElementID:'fileContainer', parameters:{id:1},ResumeCheckUrl:'" + Url.Action("ExistsFileLength") + "'}" };
            return View();
        }

    }
}