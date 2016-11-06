using _24ayar.ImageProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ChatApp.Web.Helpers
{
    public static class FileUploadHelper
    {
        internal static string ImageResizeAndLocation(string sourfilePath, string targetVirtualDirectory, string newFileName, int newsize, ResizeProportionType pROPORTION_W, bool NoForce, bool DeleteOrjFile = true)
        {
            FileInfo fileInfo = new FileInfo(sourfilePath);
            if (!fileInfo.Exists)
                return null;
            string fileExt = fileInfo.Extension;
            if (fileExt.ToLowerInvariant() == ".jpg" || fileExt.ToLowerInvariant() == ".gif" || fileExt.ToLowerInvariant() == ".png" || fileExt.ToLowerInvariant() == ".jpeg")
            {
                DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(targetVirtualDirectory));
                if (!di.Exists)
                    Directory.CreateDirectory(di.FullName);
                ImageResizer imgResizer = new ImageResizer();
                imgResizer.JPEGCompressionLevel = 80;
                newFileName = string.Format("{0}{1}", newFileName, fileInfo.Extension);
                string returnFilePath = imgResizer.ImageFileIsle(fileInfo, targetVirtualDirectory, newFileName, newsize, null, ResizeProportionType.PROPORTION_W, NoForce);
                if (DeleteOrjFile)
                    fileInfo.Delete();
                return returnFilePath;
            }
            else
            {

                if (fileInfo.Exists && DeleteOrjFile)
                    fileInfo.Delete();
            }
            return null;

        }
    }
}