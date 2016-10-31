using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _24ayar.Web.Utility
{
    public class FileDownloader : ActionResult
    {
        string fileName = null;
        string fullPath = null;
        Stream fileStream = null;
        public FileDownloader(string VirtualFilePath)
        {
            fullPath = HttpContext.Current.Server.MapPath(VirtualFilePath);
            fileName = Path.GetFileName(fullPath);
        }
        public FileDownloader(Stream FileStream, string FileName)
        {
            fileName = FileName;
            fileStream = FileStream;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            HttpContextBase mContext = context.HttpContext;
            if (fileStream != null || File.Exists(fullPath))
            {
                byte[] data = null;
                using (Stream fs = fileStream ?? new FileStream(fullPath, FileMode.Open))
                {
                    data = new byte[fs.Length];
                    //byte[] fileContent = new byte[fs.Length];
                    //fs.Read(fileContent, 0, (int)fs.Length);
                    fs.Read(data, 0, data.Length);
                    mContext.Response.Clear();
                    mContext.Response.AddHeader("Content-Length", fs.Length.ToString());
                    mContext.Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
                    //mContext.Response.ContentType = fs.con
                    //mContext.Response.OutputStream.Write(fileContent, 0, fileContent.Length);


                }
                mContext.Response.Flush();
                if (fileStream == null)
                    mContext.Response.TransmitFile(fullPath);
                else
                {
                    
                    
                    mContext.Response.OutputStream.Write( data,0, data.Length);
                }
                mContext.Response.End();
            }
            else
                return;
        }
    }
}
