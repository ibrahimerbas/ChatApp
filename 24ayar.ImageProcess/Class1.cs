using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace _24ayar.ImageProcess
{
    public enum ResizeProportionType
    {
        PROPORTION_W,
        PROPORTION_H
    }
    public enum ResizeCanvasType
    {
        CANVAS_W,
        CANVAS_H
    }
    /// <summary>
    /// Image İşlemleri
    /// </summary>
    public class ImageProcess
    {
        public ImageProcess()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public Bitmap CreateTextImage(string data, string fontPath, int fontSize, FontStyle fontStyle, GraphicsUnit graphicsUnit, Brush brush)
        {

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(fontPath);

            FontFamily mFamily = privateFontCollection.Families[0];
            Font mFont = new Font(mFamily, fontSize, fontStyle, graphicsUnit);
            return CreateTextImage(data, mFont, brush);

        }
        public Bitmap CreateTextImage(string data, Font mFont, Brush brush)
        {
            Bitmap mBitmap = new Bitmap(1, 1);

            Graphics mGrap = Graphics.FromImage(mBitmap);

            SizeF stringSize = mGrap.MeasureString(data, mFont);
            
            mBitmap = new Bitmap((int)stringSize.Width, (int)stringSize.Height);
            
            mGrap = Graphics.FromImage(mBitmap);
            
            mGrap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mGrap.TextRenderingHint = TextRenderingHint.AntiAlias;
            //mGrap.Clear(Color.White);
            //StringFormat mStrFormat = new StringFormat();
            //mStrFormat.
            mGrap.DrawString(data, mFont, brush, new PointF(0, 0));
            mGrap.Dispose();
            return mBitmap;
        }
        public Bitmap CreateTextImageWithTextRenderer(string data, string fontPath, int fontSize, FontStyle fontStyle, GraphicsUnit graphicsUnit, Brush brush)
        {

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(fontPath);

            FontFamily mFamily = privateFontCollection.Families[0];
            Font mFont = new Font(mFamily, fontSize, fontStyle, graphicsUnit);
            return CreateTextImage(data, mFont, brush);

        }
        public Bitmap CreateTextImageWithTextRenderer(string data, Font mFont, Brush brush)
        {
            Bitmap mBitmap = new Bitmap(1, 1);
            
            Graphics mGrap = Graphics.FromImage(mBitmap);

            SizeF stringSize = TextRenderer.MeasureText(data, mFont);//mGrap.MeasureString(data, mFont);
            
            mBitmap = new Bitmap((int)stringSize.Width, (int)stringSize.Height);

            mGrap = Graphics.FromImage(mBitmap);
            mGrap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mGrap.TextRenderingHint = TextRenderingHint.AntiAlias;
            //mGrap.Clear(Color.White);
            //StringFormat mStrFormat = new StringFormat();
            //mStrFormat.
            mGrap.DrawString(data, mFont, brush, new PointF(0, 0));
            mGrap.Dispose();
            return mBitmap;
        }
        public Bitmap CombineImage(Bitmap sourceImage, Bitmap destinationBitmap)
        {
            return CombineImage(sourceImage,destinationBitmap,new Point(0,0));
        }
        public Bitmap CombineImage(Bitmap sourceImage, Bitmap destinationBitmap, Point point)
        {
            Graphics mGrap = Graphics.FromImage(sourceImage);
            mGrap.DrawImage(destinationBitmap, point);
            mGrap.Dispose();
            return sourceImage;

        }

    }

    /// <summary>
    /// Bitmap boyutlandırma için çeşitli araçlar sunar.
    /// </summary>
    public class ImageResizer
    {
        /// <summary>
        /// ImageResizer sınıfı yapıcı method'u
        /// </summary>
        public ImageResizer()
        {
            this.Image = null;
            JPEGCompressionLevel = 75;
        }
        public long JPEGCompressionLevel { get; set; }
        /// <summary>
        /// ImageResizer sınıfı yapıcı method'u
        /// </summary>
        /// <param name="image">Araç içindeki Methodların kullanımı için Bitmap</param>
        public ImageResizer(Bitmap image)
        {
            this.Image = image;
            JPEGCompressionLevel = 75;
        }
        /// <summary>
        /// Araç içindeki Methodların kullanımı için Bitmap
        /// </summary>
        public Bitmap Image { get; set; }
        /// <summary>
        /// Bitmap'i kırpar
        /// </summary>
        /// <param name="TopLeft">Sol üst köşe için x ve y koordinatları için Point türünden değer</param>
        /// <param name="BottomRight">Sağ alt köşe için x ve y koordinatları için Point türünden değer</param>
        /// <returns>Kırpılmış yeni Bitmap</returns>
        public Bitmap ImageCrop(Point TopLeft, Point BottomRight)
        {
            Bitmap btmCropped = new Bitmap((BottomRight.X - TopLeft.X), (BottomRight.Y - TopLeft.Y));
            Graphics grpOriginal = Graphics.FromImage(btmCropped);
            grpOriginal.DrawImage(this.Image, new Rectangle(0, 0, btmCropped.Width, btmCropped.Height), TopLeft.X, TopLeft.Y, btmCropped.Width, btmCropped.Height, GraphicsUnit.Pixel);
            grpOriginal.Dispose();
            return btmCropped;
        }
        /// <summary>
        /// Bitmap'i kırpar
        /// </summary>
        /// <param name="OriginalImage">İşlenecek olan orjinal Bitmap</param>
        /// <param name="TopLeft">Sol üst köşe için x ve y koordinatları için Point türünden değer</param>
        /// <param name="BottomRight">Sağ alt köşe için x ve y koordinatları için Point türünden değer</param>
        /// <returns>Kırpılmış yeni Bitmap</returns>
        public Bitmap ImageCrop(Bitmap OriginalImage, Point TopLeft, Point BottomRight)
        {
            Bitmap btmCropped = new Bitmap((BottomRight.X - TopLeft.X), (BottomRight.Y - TopLeft.Y));
            Graphics grpOriginal = Graphics.FromImage(btmCropped);
            grpOriginal.DrawImage(OriginalImage, new Rectangle(0, 0, btmCropped.Width, btmCropped.Height), TopLeft.X, TopLeft.Y, btmCropped.Width, btmCropped.Height, GraphicsUnit.Pixel);
            grpOriginal.Dispose();
            return btmCropped;
        }
        private Bitmap ResizeAndCanvasW(Bitmap OrjBitmap, int istenenWidth, int istenenHeight)
        {
            try
            {
                Bitmap croppedImage = ImageCrop(ResizeWithProportionW(OrjBitmap, istenenWidth), new Point(0, 0), new Point(istenenWidth, istenenHeight));
                return croppedImage;
            }
            catch (Exception)
            {

                return null;
            }
        }
        private Bitmap ResizeAndCanvasH(Bitmap OrjBitmap, int istenenWidth, int istenenHeight)
        {
            try
            {
                Bitmap croppedImage = ImageCrop(ResizeWithProportionH(OrjBitmap, istenenHeight), new Point(0, 0), new Point(istenenWidth, istenenHeight));
                return croppedImage;
            }
            catch (Exception)
            {

                return null;
            }
        }
        private Bitmap ResizeWithProportionW(Bitmap OrjBitmap, int istenenWidth)
        {
            try
            {
                int newHeight = (int)(((double)OrjBitmap.Height) / (((double)OrjBitmap.Width) / ((double)istenenWidth)));
                return ImageResize(OrjBitmap, istenenWidth, newHeight);
            }
            catch (Exception)
            {

                return null;
            }
        }

        private Bitmap ResizeWithProportionH(Bitmap OrjBitmap, int istenenHeight)
        {
            try
            {
                int newWidth = (int)(((double)OrjBitmap.Width) / (((double)OrjBitmap.Height) / ((double)istenenHeight)));
                return ImageResize(OrjBitmap, newWidth, istenenHeight);
            }
            catch (Exception)
            {

                return null;
            }
        }
        /// <summary>
        /// Sınıf içinden tanımlı Bitmap'in yüksekliğini veya genişliğini boyutlandırır diğer yönünü kırpar
        /// </summary>
        /// <param name="width">Genişlik değeri.</param>
        /// <param name="height">Yükseklik değeri</param>
        /// <param name="type">Yükseklik veya genişliği ölçülendirileceğini belirler diğer yön kırpılacak</param>
        /// <returns>Ölçülendirilmiş yeni bitmap.</returns>
        public Bitmap ResizeAndCanvas(int width, int height, ResizeCanvasType type)
        {
            return ResizeAndCanvas(this.Image, width, height, type);
        }
        /// <summary>
        /// Bitmap'in yüksekliğini veya genişliğini boyutlandırır diğer yönünü kırpar
        /// </summary>
        /// <param name="orjBitmap">İşlenecek orjinal bitmap</param>
        /// <param name="width">Genişlik değeri.</param>
        /// <param name="height">Yükseklik değeri</param>
        /// <param name="type">Yükseklik veya genişliği ölçülendirileceğini belirler diğer yön kırpılacak</param>
        /// <returns>Ölçülendirilmiş yeni bitmap.</returns>
        public Bitmap ResizeAndCanvas(Bitmap orjBitmap, int width, int height, ResizeCanvasType type)
        {
            Bitmap mBitmap = null;
            switch (type)
            {
                case ResizeCanvasType.CANVAS_W:
                    mBitmap = this.ResizeAndCanvasW(orjBitmap, width, height);
                    break;
                case ResizeCanvasType.CANVAS_H:
                    mBitmap = this.ResizeAndCanvasH(orjBitmap, width, height);
                    break;
                default:
                    break;
            }
            return mBitmap;

        }
        /// <summary>
        /// Sınıf içinden tanımlı Bitmap'in yüksekliğini veya genişliğini boyutlandırır ayrıca diğer yönünü de orantılı olarak boyutlandır.
        /// </summary>
        /// <param name="istenenOlcu">type türüne göre istenen genişlik veya yükseklik ölçüsüdür.</param>
        /// <param name="type">Yükseklik veya genişliği ölçülendirileceğini belirler</param>
        /// <returns>Ölçülendirilmiş yeni bitmap.</returns>
        public Bitmap ResizeProportion(int istenenOlcu, ResizeProportionType type)
        {
            return ResizeProportion(this.Image,istenenOlcu, type);
        }
        /// <summary>
        /// Bitmap'in yüksekliğini veya genişliğini boyutlandırır ayrıca diğer yönünü de orantılı olarak boyutlandır.
        /// </summary>
        /// <param name="istenenOlcu">type türüne göre istenen genişlik veya yükseklik ölçüsüdür.</param>
        /// <param name="type">Yükseklik veya genişliği ölçülendirileceğini belirler</param>
        /// <param name="orjBitmap">İşlenecek orjinal bitmap</param>
        /// <returns>Ölçülendirilmiş yeni bitmap.</returns>
        public Bitmap ResizeProportion(Bitmap orjBitmap, int istenenOlcu, ResizeProportionType type)
        {
            Bitmap mBitmap = null;
            switch (type)
            {
                case ResizeProportionType.PROPORTION_W:
                    mBitmap = this.ResizeWithProportionW(orjBitmap, istenenOlcu);
                    break;
                case ResizeProportionType.PROPORTION_H:
                    mBitmap = this.ResizeWithProportionH(orjBitmap, istenenOlcu);
                    break;
                default:
                    break;
            }
            return mBitmap;
        }
        /// <summary>
        /// Verilen Ölçülerde Bitmap'i boyutlandırır.
        /// </summary>
        /// <param name="Height">Yükselik Değeri</param>
        /// <param name="Width">Genişlik Değeri</param>
        public Bitmap ImageResize(int Width, int Height)
        {
            return ImageResize(this.Image, Width, Height);
        }
        /// <summary>
        /// Verilen Ölçülerde Bitmap'i boyutlandırır.
        /// </summary>
        /// <param name="Height">Yükselik Değeri</param>
        /// <param name="Width">Genişlik Değeri</param>
        /// <param name="OrjBitmap">Orijinal Bitmap</param>
        public Bitmap ImageResize(Bitmap OrjBitmap, int Width, int Height)
        {
            
            try
            {

                Bitmap thumb = new Bitmap(Width, Height);
                Graphics mGraphs = Graphics.FromImage(thumb);
                if (OrjBitmap.RawFormat != ImageFormat.Png)
                {
                    mGraphs.InterpolationMode = InterpolationMode.High;
                    mGraphs.CompositingQuality = CompositingQuality.HighQuality;
                    mGraphs.SmoothingMode = SmoothingMode.AntiAlias;
                    mGraphs.PixelOffsetMode = PixelOffsetMode.HighQuality;
                }
                else
                {
                    mGraphs.SmoothingMode = SmoothingMode.None;
                    mGraphs.InterpolationMode = InterpolationMode.NearestNeighbor;
                    
                    mGraphs.SmoothingMode = SmoothingMode.None;
                    mGraphs.PixelOffsetMode = PixelOffsetMode.None;
                    
                    
                }
                System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes();

                attr.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY) ;


                mGraphs.DrawImage(OrjBitmap, 0, 0 ,thumb.Width, thumb.Height);
                mGraphs.Dispose();
                attr.Dispose();
                //thumb.Dispose();
                return thumb ;
            }
            catch (Exception)
            {

                return null;
            }
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) yükseklik veya genişliğe göre boyutlandırır ve diğer yönünü kırpar.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="Width">Genişlik değeri</param>
        /// <param name="Height">Yükseklik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler diğer yön verilen değerinde kırpılır</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(HttpPostedFile mFile, string SaveDirectory, string FileName, int Width,int Height ,ImageFormat SaveFormat, ResizeCanvasType type)
        {
            string fileName = null;
            if (mFile.ContentLength > 0)
            {
                string FileExtesion = Path.GetExtension(mFile.FileName).ToLower();
                ImageFormat mFormat = null;
                //
                switch (FileExtesion)
                {
                    case ".jpg":
                    case ".jpeg":
                        mFormat = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        mFormat = ImageFormat.Png;
                        break;
                    case ".gif":
                        mFormat = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }
                if (mFormat != null)
                {
                    if (SaveFormat == null)
                    {
                        SaveFormat = mFormat;
                    }
                    else if (SaveFormat == ImageFormat.Jpeg)
                    {
                        FileExtesion = ".jpg";
                    }
                    else if (SaveFormat == ImageFormat.Gif)
                    {
                        FileExtesion = ".gif";
                    }
                    else if (SaveFormat == ImageFormat.Png)
                    {
                        FileExtesion = ".png";
                    }

                    string fullDirectoryPath = HttpContext.Current.Server.MapPath(SaveDirectory);
                    Bitmap mBitmap = ReadBitmapFromPostedFile(mFile);
                    fileName = fullDirectoryPath + FileName + FileExtesion;
                    Bitmap sBitmap = this.ResizeAndCanvas(mBitmap, Width, Height, type);
                    sBitmap.Save(fileName, SaveFormat != null ? SaveFormat : mFormat);
                    mBitmap.Dispose();
                    sBitmap.Dispose();
                }
            }
            return fileName;
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) Yükseklik veya genişliğe göre orantılı olarak boyutlandırır ve kaydeder.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="istenenOlcu">Yukseklik veya Genişlik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(HttpPostedFile mFile, string SaveDirectory, string FileName, int istenenOlcu, ImageFormat SaveFormat, ResizeProportionType type,bool maxValue)
        {
           
            if (mFile.ContentLength > 0)
            {
                Bitmap mBitmap = ReadBitmapFromPostedFile(mFile);
                int orjWidth = mBitmap.Width;
                int orjHeight = mBitmap.Height;
                mBitmap.Dispose();
                switch (type)
                {
                    case ResizeProportionType.PROPORTION_W:
                        if (orjWidth <= istenenOlcu && maxValue)
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, orjWidth, SaveFormat, type);
                        }
                        else
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, istenenOlcu, SaveFormat, type);
                        }
                        
                    case ResizeProportionType.PROPORTION_H:
                        if (orjHeight <= istenenOlcu && maxValue)
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, orjHeight, SaveFormat, type);
                        }
                        else
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, istenenOlcu, SaveFormat, type);
                        }
                        
                    default:
                        return null;
                        
                }    
            }
            return null;
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) Yükseklik veya genişliğe göre orantılı olarak boyutlandırır ve kaydeder.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="istenenOlcu">Yukseklik veya Genişlik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(HttpPostedFile mFile, string SaveDirectory, string FileName, int istenenOlcu, ImageFormat SaveFormat, ResizeProportionType type)
        {
            string fileName = null;
            string virtualFileName = null;
            if (mFile.ContentLength > 0)
            {
                string FileExtesion = Path.GetExtension(mFile.FileName).ToLower();
                ImageFormat mFormat = null;
                //
                switch (FileExtesion)
                {
                    case ".jpg":
                    case ".jpeg":
                        mFormat = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        mFormat = ImageFormat.Png;
                        break;
                    case ".gif":
                        mFormat = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }
                if (mFormat != null)
                {
                    if (SaveFormat == null)
                    {
                        SaveFormat = mFormat;
                    }
                    else if (SaveFormat == ImageFormat.Jpeg)
                    {
                        FileExtesion = ".jpg";
                    }
                    else if (SaveFormat == ImageFormat.Gif)
                    {
                        FileExtesion = ".gif";
                    }
                    else if (SaveFormat == ImageFormat.Png)
                    {
                        FileExtesion = ".png";
                    }

                    string fullDirectoryPath = HttpContext.Current.Server.MapPath(SaveDirectory);
                    virtualFileName = SaveDirectory + FileName + FileExtesion;
                    Bitmap mBitmap = ReadBitmapFromPostedFile(mFile);
                    fileName = fullDirectoryPath + FileName + FileExtesion;
                    Bitmap sBitmap = this.ResizeProportion(mBitmap, istenenOlcu, type);
                    sBitmap.Save(fileName, SaveFormat != null ? SaveFormat : mFormat);
                    sBitmap.Dispose();
                    mBitmap.Dispose();
                }
            }
            return virtualFileName;
        }
        public Bitmap ReadBitmapFromPostedFile(HttpPostedFile mFile)
        {
            Stream mStream = mFile.InputStream;
            Bitmap mBitmap = new Bitmap(mStream);
            return mBitmap;
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) Yükseklik veya genişliğe göre orantılı olarak boyutlandırır ve kaydeder.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="istenenOlcu">Yukseklik veya Genişlik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(FileInfo mFile, string SaveDirectory, string FileName, int istenenOlcu, ImageFormat SaveFormat, ResizeProportionType type)
        {
            string fileName = null;
            string virtualFileName = null;
            if (mFile.Length > 0)
            {
                string FileExtesion = Path.GetExtension(mFile.FullName).ToLower();
                ImageFormat mFormat = null;
                //
                switch (FileExtesion)
                {
                    case ".jpg":
                    case ".jpeg":
                        mFormat = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        mFormat = ImageFormat.Png;
                        break;
                    case ".gif":
                        mFormat = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }


                if (SaveFormat == null)
                {
                    SaveFormat = mFormat;
                }
                else if (SaveFormat == ImageFormat.Jpeg)
                {
                    FileExtesion = ".jpg";
                }
                else if (SaveFormat == ImageFormat.Gif)
                {
                    FileExtesion = ".gif";
                }
                else if (SaveFormat == ImageFormat.Png)
                {
                    FileExtesion = ".png";
                }

                string fullDirectoryPath = HttpContext.Current.Server.MapPath(SaveDirectory);
                virtualFileName = SaveDirectory + Path.GetFileNameWithoutExtension(FileName) + FileExtesion;
                FileStream fs = mFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                Bitmap mBitmap = (Bitmap)Bitmap.FromStream(fs);
                fs.Close();
                fs.Dispose();
                fileName = fullDirectoryPath + Path.GetFileNameWithoutExtension(FileName) + FileExtesion;
                Bitmap sBitmap = this.ResizeProportion(mBitmap, istenenOlcu, type);
                SaveFormat = SaveFormat != null ? SaveFormat : mFormat;
                ImageCodecInfo jgpEncoder = GetEncoder(SaveFormat);
                System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, this.JPEGCompressionLevel);
                myEncoderParameters.Param[0] = myEncoderParameter;
                if (File.Exists(fileName))
                    File.Delete(fileName);
                sBitmap.Save(fileName, jgpEncoder, myEncoderParameters);
                    
                sBitmap.Dispose();
                mBitmap.Dispose();
            }
            
            return virtualFileName;
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) Yükseklik veya genişliğe göre orantılı olarak boyutlandırır ve kaydeder.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="istenenOlcu">Yukseklik veya Genişlik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(FileInfo mFile, string SaveDirectory, string FileName, int istenenOlcu, ImageFormat SaveFormat, ResizeProportionType type, bool maxValue)
        {

            if (mFile.Length > 0)
            {
                FileStream fs = mFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                Bitmap mBitmap = (Bitmap)Bitmap.FromStream(fs);
                fs.Close();
                int orjWidth = mBitmap.Width;
                int orjHeight = mBitmap.Height;
                mBitmap.Dispose();
                switch (type)
                {
                    case ResizeProportionType.PROPORTION_W:
                        if (orjWidth <= istenenOlcu && maxValue)
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, orjWidth, SaveFormat, type);
                        }
                        else
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, istenenOlcu, SaveFormat, type);
                        }
                        
                    case ResizeProportionType.PROPORTION_H:
                        if (orjHeight <= istenenOlcu && maxValue)
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, orjHeight, SaveFormat, type);
                        }
                        else
                        {
                            return ImageFileIsle(mFile, SaveDirectory, FileName, istenenOlcu, SaveFormat, type);
                        }
                        
                    default:
                        return null;
                        
                }
            }
            return null;
        }
        /// <summary>
        /// Upload edilen image dosyasını (HttpPostedFile) yükseklik veya genişliğe göre boyutlandırır ve diğer yönünü kırpar.
        /// </summary>
        /// <param name="mFile">HttpPostedFile türünden upload edilen dosya</param>
        /// <param name="SaveDirectory">Kaydedilecek sanal dizin yolu Örnek:"~/images/haberler/"</param>
        /// <param name="FileName">Uzantı hariç dosya adı Örnek:"haberheader"</param>
        /// <param name="Width">Genişlik değeri</param>
        /// <param name="Height">Yükseklik değeri</param>
        /// <param name="SaveFormat">Image File format'ı null değer verilirse dosya orjinal formatı ile kaydedilir</param>
        /// <param name="type">Boyutlandırma nın yükseliğe veya genişliğe göremi orantılanacağını belirler diğer yön verilen değerinde kırpılır</param>
        /// <returns>Kaydedilen dosyanın sanal yolunu verir Örnek:"~/images/haberler/haberheader.jpg"</returns>
        public string ImageFileIsle(FileInfo mFile, string SaveDirectory, string FileName, int Width, int Height, ImageFormat SaveFormat, ResizeCanvasType type)
        {
            string fileName = null;
            if (mFile.Length > 0)
            {
                string FileExtesion = Path.GetExtension(mFile.FullName).ToLower();
                ImageFormat mFormat = null;
                //
                switch (FileExtesion)
                {
                    case ".jpg":
                    case ".jpeg":
                        mFormat = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        mFormat = ImageFormat.Png;
                        break;
                    case ".gif":
                        mFormat = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }
                if (mFormat != null)
                {
                    if (SaveFormat == null)
                    {
                        SaveFormat = mFormat;
                    }
                    else if (SaveFormat == ImageFormat.Jpeg)
                    {
                        FileExtesion = ".jpg";
                    }
                    else if (SaveFormat == ImageFormat.Gif)
                    {
                        FileExtesion = ".gif";
                    }
                    else if (SaveFormat == ImageFormat.Png)
                    {
                        FileExtesion = ".png";
                    }

                    string fullDirectoryPath = HttpContext.Current.Server.MapPath(SaveDirectory);
                    FileStream fs = mFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    Bitmap mBitmap = (Bitmap)Bitmap.FromStream(fs);
                    fs.Close();
                    fileName = fullDirectoryPath + FileName + FileExtesion;
                    Bitmap sBitmap = this.ResizeAndCanvas(mBitmap, Width, Height, type);
                    mBitmap.Dispose();
                    SaveFormat = SaveFormat != null ? SaveFormat : mFormat;
                    ImageCodecInfo jgpEncoder = GetEncoder(SaveFormat);
                    System.Drawing.Imaging.Encoder myEncoder =
                            System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, this.JPEGCompressionLevel);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    sBitmap.Save(fileName, jgpEncoder, myEncoderParameters);
                    sBitmap.Dispose();
                }
            }
            return fileName;
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
namespace _24ayar.StringProcess 
{
    public static class HTMLOperation
    {
        /// <summary>
        /// Eksik tag ları kapatır..
        /// </summary>
        /// <param name="tagName">Tag adı</param>
        /// <param name="HTML">HTML</param>
        /// <returns></returns>
        public static string fixTag(string tagName, string HTML)
        {
            Regex mRegx = new Regex("(?><" + tagName + ".*?>.*?(?=<" + tagName + "|</" + tagName + ">|\\z))()(?!</" + tagName + ">)");
            return mRegx.Replace(HTML, "$0</" + tagName + ">");
        }

    }
}
