using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _24ayar.Web.Utility
{
    public static class LocalizationHelper
    {
        [Obsolete("specifying the language through <meta http-equiv=\"content-language\" content= > is obsolete. Use <html lang=> instead")]
        public static IHtmlString MetaContentLanguage(this HtmlHelper html)
        {
            var acceptLang = HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(string.Format("<meta http-equiv=\"content-language\" content=\"{0}\"/>", acceptLang));
        }
        public static string Language(this HtmlHelper html)
        {
            return HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
        }
        public static string Lang
        {
            get
            {
                return HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
            }
        }
    }
}
