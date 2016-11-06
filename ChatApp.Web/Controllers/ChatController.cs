using _24ayar.Web.Utility;
using ChatApp.Data.Repository;
using ChatApp.Data.Surrogates;
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
    [Authorize()]
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


        protected string AppendFile(
            HttpPostedFileBase File,
            string UniqueIdentifier,
            ulong ChunkNumber,
            int ChunkSize,
            int CurrentChunkSize,
            ulong FileSize,
            string FileType,
            string FileName,
            string RelativePath,
            Guid MessageID)
        {
            string FilePath = Server.MapPath("~/Temp");
            FilePath = Path.Combine(FilePath, FileName);
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(File.InputStream))
            {
                fileData = binaryReader.ReadBytes(File.ContentLength);
            }

            FileInfo fi = new FileInfo(FilePath);
            if (ChunkNumber == 0 && fi.Exists)
                fi.Delete();
            if (fileData != null)
                Helper.AppendAllBytes(FilePath, fileData);
            fi = new FileInfo(FilePath);

            if ((long)FileSize == fi.Length)
            {
                return FilePath;
                //BookRepository repo = new BookRepository();
            }
            return null; //Json(new { Result = "OK", imagePath = clientImagePath });
        }
        [HttpPost]
        public ActionResult UploadFile(
              HttpPostedFileBase File,
              string UniqueIdentifier,
              ulong ChunkNumber,
              int ChunkSize,
              int CurrentChunkSize,
              ulong FileSize,
              string FileType,
              string FileName,
              string RelativePath,
              int AttachType,
              Guid MessageID)
        {
            string result = AppendFile(File,
                UniqueIdentifier,
                ChunkNumber,
                ChunkSize,
                CurrentChunkSize,
                FileSize,
                FileType,
                FileName,
                RelativePath,
                MessageID);
            if (result != null)
            {

                try
                {
                    if (!User.Identity.IsAuthenticated)
                        throw new Exception("Giriş yap da gel! Mevlana değiliz!");
                    ChatMessageRepository cRepo = new ChatMessageRepository();
                    var message = cRepo.Get(MessageID);
                    if (message == null)
                        throw new Exception("Mesaj bulunamadı. Açık mı arıyon hacı?");

                    
                    FileInfo fileInfo = new FileInfo(result);
                    string extension = fileInfo.Extension;
                    string newFileName = Guid.NewGuid().ToString().Replace("-", "_") + extension;
                    string chatFileDir = Server.MapPath(ChatFileDir);
                    if (!Directory.Exists(chatFileDir))
                        Directory.CreateDirectory(chatFileDir);
                    string fullFilePath = string.Format("{0}\\{1}", chatFileDir, newFileName);
                    fileInfo.MoveTo(fullFilePath);
                    string absoluteFilePath = Url.Content(string.Format("{0}/{1}", ChatFileDir, newFileName));
                    try
                    {
                        MessageFileSurrogate fileSurrogate = new MessageFileSurrogate();
                        fileSurrogate.FilePath = absoluteFilePath;
                        fileSurrogate.AttachType = AttachType;
                        message.Files.Add(fileSurrogate);
                        cRepo.Save(message);

                    }
                    catch (Exception)
                    {
                        if (System.IO.File.Exists(fullFilePath))
                            System.IO.File.Delete(fullFilePath);
                        throw;
                    }

                    return Json(new { Result = "OK", FilePath = absoluteFilePath });
                }
                catch (Exception e)
                {

                    return Json(new { Result = "ERROR", ErrorMessage = e.Message });
                }
            }
            return Json(new { Result = "OK", imagePath = "" }); ;
        }
        public ActionResult UploaderTest() {
            ViewData["UploadFile"] = new UploadModel() { GroupName = "MessageFile", Caption = "Dosya Yükle", UploaderOptions = "{action:'" + Url.Action("UploadFile", "Chat") + "',singleFile:false,group:'MessageFile',allFilesUploaded:function (groupName, options) { $('#lockscreen-uploader').hide();},uploadFinished:function(instance,file,data){$('#thumbImageThumb' + file.CurrentOptions.parameters.id).attr('src' , data.imagePath + '?' + Math.random());},createItemFunction:CreateFileItem,fileRemoved:UploaderFileRemoved,containerElementID:'fileContainer', parameters:{id:1},ResumeCheckUrl:'" + Url.Action("ExistsFileLength") + "'}" };
            return View();
        }

    }
}