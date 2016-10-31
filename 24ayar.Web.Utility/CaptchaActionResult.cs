using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _24ayar.Web.Utility
{
    public class CaptchaResult : ActionResult
    {

        string _sessionName = "";
        public CaptchaResult()
            : this("register")
        {

        }
        public CaptchaResult(string sessionName)
        {
            _sessionName = sessionName;
        }

        private string RandomKodUret()
        {
            System.Random mRandom = default(System.Random);
            mRandom = new Random();
            string s = "";
            for (int i = 0; i <= 3; i++)
            {
                s = String.Concat(s, mRandom.Next(10).ToString());
            }
            return s;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            string kod = RandomKodUret();
            context.HttpContext.Session[_sessionName] = kod;
            //Session["forgetPassSecImage"] = 
            SecurityImage mSecImage = new SecurityImage(kod, 100, 60, 40);
            HttpContextBase mContext = context.HttpContext;
            mContext.Response.Clear();
            mContext.Response.ContentType = "image/jpeg";


            mSecImage.Image.Save(mContext.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            //FileStreamResult result = new FileStreamResult(mStream, "image/jpeg");

        }
    }
    public class SecurityImage
    {
        #region "properties"
        public string Text
        {
            get { return this.m_text; }
        }
        public Bitmap Image
        {
            get { return this.m_image; }
        }
        public int Width
        {
            get { return this.m_width; }
        }
        public int Height
        {
            get { return this.m_height; }
        }
        public int FontSize
        {
            get { return mFontSize; }
        }
        private string m_text;
        private int m_width;
        private int m_height;
        private string familyName;
        private int mFontSize;
        private Bitmap m_image;
        #endregion
        private Random random = new Random();
        public SecurityImage(string s, int width, int height, int sFontSize)
        {
            m_text = s;
            this.m_width = width;
            this.m_height = height;
            this.mFontSize = sFontSize;
            ImageYarat();
        }
        private void ImageYarat()
        {
            this.familyName = System.Drawing.FontFamily.GenericSerif.Name;

            // 32bit bmp yaratalim 
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(this.m_width, this.m_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // cizim icin bir graphics nesnesi yaratalim. 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, this.m_width, this.m_height);
            // Resimde arka alani dolduralim 
            System.Drawing.Drawing2D.HatchBrush hb = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallConfetti, System.Drawing.Color.LightGray, System.Drawing.Color.White);
            g.FillRectangle(hb, rec);
            System.Drawing.Font font = new System.Drawing.Font(this.familyName, this.FontSize, System.Drawing.FontStyle.Bold);
            // textin formatini belirleyelim 
            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            format.Alignment = System.Drawing.StringAlignment.Center;
            format.LineAlignment = System.Drawing.StringAlignment.Center;
            // bir path belirleyip, texte random olarak aci verelim 
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddString(this.m_text, font.FontFamily, (int)font.Style, font.Size, rec, format);
            float v = 4;
            System.Drawing.PointF[] p = { new System.Drawing.PointF(this.random.Next(rec.Width) / v, this.random.Next(rec.Height) / v), new System.Drawing.PointF(rec.Width - this.random.Next(rec.Width) / v, this.random.Next(rec.Height) / v), new System.Drawing.PointF(this.random.Next(rec.Width) / v, rec.Height - this.random.Next(rec.Height) / v), new System.Drawing.PointF(rec.Width - this.random.Next(rec.Width) / v, rec.Height - this.random.Next(rec.Height) / v) };
            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix.Translate(0f, 0f);
            path.Warp(p, rec, matrix, System.Drawing.Drawing2D.WarpMode.Perspective, 0);
            hb = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LargeConfetti, System.Drawing.Color.LightGray, System.Drawing.Color.Blue);
            g.FillPath(hb, path);
            for (int i = 0; i <= (int)(rec.Width * rec.Height / 30) - 1; i++)
            {
                int x = this.random.Next(rec.Width);
                int y = this.random.Next(rec.Height);
                int w = this.random.Next(rec.Width / 50);
                int h = this.random.Next(rec.Width / 40);
                g.FillEllipse(hb, x, y, w, h);
            }
            this.m_image = bitmap;
        }
    }
}
