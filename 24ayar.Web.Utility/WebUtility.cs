using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Web.WebPages;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.Security.Cryptography;

namespace _24ayar.Web.Utility
{
    public static class HtmlExtensions
    {


        public static MvcHtmlString Styles(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_style_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }
        public static IHtmlString RenderStyles(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_style_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    //htmlHelper.ViewContext.HttpContext.Items.Remove(key);
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
        public static MvcHtmlString Image(this HtmlHelper helper,
                              string url,
                              object htmlAttributes)
        {
            
            TagBuilder builder = new TagBuilder("img");
            builder.Attributes.Add("src", url);
            if (htmlAttributes.GetType() != typeof(RouteValueDictionary))
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            else
                builder.MergeAttributes((RouteValueDictionary)htmlAttributes);
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
    public static class HtmlPrefixScopeExtensions
    {
        public static RouteValueDictionary ToRouteValues(this NameValueCollection queryString)
        {
            if (queryString == null || queryString.HasKeys() == false) return new RouteValueDictionary();

            var routeValues = new RouteValueDictionary();
            foreach (string key in queryString.AllKeys)
                routeValues.Add(key, queryString[key]);

            return routeValues;
        }
        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName, ViewDataDictionary adinationalViewDataDic)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo
                {
                    HtmlFieldPrefix = name
                }
            };

            return helper.Partial(partialViewName, model, viewData);

        }

        private const string idsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";

        public static IDisposable BeginCollectionItem(this HtmlHelper html, string collectionName)
        {
            var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
            string itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();

            // autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
            html.ViewContext.Writer.WriteLine(string.Format("<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />", collectionName, html.Encode(itemIndex)));

            return BeginHtmlFieldPrefixScope(html, string.Format("{0}[{1}]", collectionName, itemIndex));
        }

        public static IDisposable BeginHtmlFieldPrefixScope(this HtmlHelper html, string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
        }

        private static Queue<string> GetIdsToReuse(HttpContextBase httpContext, string collectionName)
        {
            // We need to use the same sequence of IDs following a server-side validation failure,  
            // otherwise the framework won't render the validation error messages next to each item.
            string key = idsToReuseKey + collectionName;
            var queue = (Queue<string>)httpContext.Items[key];
            if (queue == null)
            {
                httpContext.Items[key] = queue = new Queue<string>();
                var previouslyUsedIds = httpContext.Request[collectionName + ".index"];
                if (!string.IsNullOrEmpty(previouslyUsedIds))
                    foreach (string previouslyUsedId in previouslyUsedIds.Split(','))
                        queue.Enqueue(previouslyUsedId);
            }
            return queue;
        }

        private class HtmlFieldPrefixScope : IDisposable
        {
            private readonly TemplateInfo templateInfo;
            private readonly string previousHtmlFieldPrefix;

            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                this.templateInfo = templateInfo;

                previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            public void Dispose()
            {
                templateInfo.HtmlFieldPrefix = previousHtmlFieldPrefix;
            }
        }
    }

    //public static class RemoteServices
    //{
    //    public static userServiceForSitesClient UserServiceClient
    //    {
    //        get
    //        {
    //            userServiceForSitesClient mClient = new userServiceForSitesClient();
    //            mClient.ClientCredentials.UserName.UserName = "user";
    //            mClient.ClientCredentials.UserName.Password = "pass";
    //            return mClient;
    //        }
    //    }
    //}
    public static class Helper
    {
        public static string SmsCodeGenerate(int length)
        {
            System.Random mRandom = default(System.Random);
            mRandom = new Random();
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s = String.Concat(s, mRandom.Next(9).ToString());
            }
            return s;
        }
        public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
        {
            public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request.IsAjaxRequest();
            }
        }
        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model, ViewDataDictionary viewData)
        {
            if (viewData == null)
                viewData = new ViewDataDictionary();
            
            viewData.Model = model;
            using (var sw = new StringWriter())
            {
                
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            return RenderRazorViewToString(controllerContext, viewName, model, controllerContext.Controller.ViewData);
        }
        public static string RenderRazorViewToString<T>(string viewName, object model, RouteData routeData = null) where T:Controller, new()
        {
            if (HttpContext.Current != null)
            {
                var controller = ControllerCreator.CreateController<T>(routeData);//.ControllerContext;
                var controllerContext = new ControllerContext(controller.Request.RequestContext, controller);
                return RenderRazorViewToString(controllerContext, viewName, model, controllerContext.Controller.ViewData);
            }
            else
            {
                throw new Exception("RenderRazorViewToString must run in the context of an ASP.NET " +
                "Application and requires HttpContext.Current to be present.");
            }
            
        }
        static object syncObj = new object();
        public static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.
            lock (syncObj)
                using (var stream = new FileStream(path, FileMode.Append))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
        }
        public static string GetAllError(this Exception e, int pad = 0)
        {
            string result = ">".PadLeft(pad, '-') + e.Message;
            if (e.InnerException != null)
                result += String.Format("{1}{0}", GetAllError(e.InnerException, pad + 5), Environment.NewLine);
            return result;
        }

        public static string EscapeTurkish(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";
            string Temp = name.ToLower();
            Temp = Temp.Replace("â", "a");
            Temp = Temp.Replace("ê", "e");
            Temp = Temp.Replace("î", "i");
            Temp = Temp.Replace("€", "e");
            Temp = Temp.Replace("û", "u");
            Temp = Temp.Replace("Î", "i");
            Temp = Temp.Replace("Â", "a");
            Temp = Temp.Replace("Ê", "e");
            Temp = Temp.Replace("Û", "u");
            Temp = Temp.Replace("ç", "c"); Temp = Temp.Replace("ğ", "g");
            Temp = Temp.Replace("ı", "i"); Temp = Temp.Replace("ö", "o");
            Temp = Temp.Replace("ş", "s"); Temp = Temp.Replace("ü", "u");
            Temp = Temp.Replace("\"", ""); Temp = Temp.Replace("/", "");
            Temp = Temp.Replace("(", "_"); Temp = Temp.Replace(")", "");
            Temp = Temp.Replace("{", ""); Temp = Temp.Replace("}", "");
            Temp = Temp.Replace("%", ""); Temp = Temp.Replace("&", "");
            Temp = Temp.Replace("+", "");
            Temp = Temp.Replace("?", ""); Temp = Temp.Replace(",", "");
            Temp = Temp.Replace(":", ""); Temp = Temp.Replace("\r\n", "");
            Temp = Temp.Replace("'", "_");
            Temp = Temp.Replace(" ", "_"); Temp = Temp.Replace("-", "_");
            Temp = Temp.Replace(".", "");Temp = Temp.Replace("’","_"); Temp = Temp.Replace("*", "");

            return Temp;
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }
            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static string JSonSerialize(this object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        public static T JsonDeserialize<T>(this string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        public static string GetMD5(this string ClearText)
        {
            byte[] ByteData = Encoding.ASCII.GetBytes(ClearText);
            //MD5 nesnesi oluşturalım. 
            MD5 oMd5 = MD5.Create();
            //Hash değerini hesaplayalım. 
            byte[] HashData = oMd5.ComputeHash(ByteData);

            //byte dizisini hex formatına çevirelim 
            StringBuilder oSb = new StringBuilder();
            int x = 0;
            while (x < HashData.Length)
            {
                //hexadecimal string değeri 
                oSb.Append(HashData[x].ToString("x2"));
                x += 1;
            }
            return oSb.ToString();
        }
        public static bool TCKimlikValidate(this string tcKimlikNo)
        {
            bool returnvalue = false;
            if (tcKimlikNo.Length == 11)
            {
                Int64 ATCNO, BTCNO, TcNo;
                long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

                TcNo = Int64.Parse(tcKimlikNo);

                // bolu yuz islemi int tanimlanmis degiskende son 2 haneyi silmek icin kullanılır.

                ATCNO = TcNo / 100;
                BTCNO = TcNo / 100;

                C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

                /*
                Q1 TC nosunun 10. hanesi
                Q2 TC nosunun 11. hanesi
                BTCNO son 2 hanesi olmayan tckimlikNo
                */

                returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
            }
            return returnvalue;

        }
        public static void DeleteServerFile(this string filePath)
        {
            try
            {
                var server = HttpContext.Current.Server;
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var oldFilePath = server.MapPath(filePath);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
            }
            catch 
            {}
        }
        

    }

    public class QueryString : NameValueCollection
    {
        public QueryString()
        {
        }

        public QueryString(string queryString)
        {
            FillFromString(queryString);
        }
        public static QueryString Current
        {
            get
            {
                return new QueryString().FromCurrent();
            }
        }
        /// <summary>        
        /// extracts a querystring from a full URL        
        /// </summary>        
        /// <param name="s">the string to extract the querystring from</param>        
        /// <returns>a string representing only the querystring</returns>        
        public string ExtractQuerystring(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Contains("?"))
                    return s.Substring(s.IndexOf("?") + 1);

            }
            return s;
        }
        /// <summary>        
        /// returns a querystring object based on a string        
        /// </summary>        
        /// <param name="s">the string to parse</param>        
        /// <returns>the QueryString object </returns>        
        public QueryString FillFromString(string s)
        {
            base.Clear();
            if (string.IsNullOrEmpty(s))
                return this;
            foreach (string keyValuePair in ExtractQuerystring(s).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair))
                    continue;
                string[] split = keyValuePair.Split('=');
                if (split[0] != "AllRemoved")
                    base.Add(split[0], split.Length == 2 ? split[1] : "");
            }
            return this;
        }
        /// <summary>        
        /// returns a QueryString object based on the current querystring of the request        
        /// </summary>        
        /// <returns>the QueryString object </returns>        
        public QueryString FromCurrent()
        {
            if (HttpContext.Current != null)
            {
                return FillFromString(HttpContext.Current.Request.QueryString.ToString());
            }
            base.Clear();
            return this;
        }
        /// <summary>        
        /// add a name value pair to the collection        
        /// </summary>        
        /// <param name="name">the name</param>        
        /// <param name="value">the value associated to the name</param>        
        /// <returns>the QueryString object </returns>        
        public new QueryString Add(string name, string value)
        {
            return Add(name, value, false);
        }
        /// <summary>        
        /// adds a name value pair to the collection        
        /// </summary>        
        /// <param name="name">the name</param>        
        /// <param name="value">the value associated to the name</param>        
        /// <param name="isUnique">true if the name is unique within the querystring. This allows us to override existing values</param>        
        /// <returns>the QueryString object </returns>        
        public QueryString Add(string name, string value, bool isUnique)
        {

            string existingValue = base[name];
            if (existingValue == null)
                base.Add(name, HttpUtility.UrlEncode(value));
            else if (isUnique)
                base[name] = HttpUtility.UrlEncode(value);
            else
                base[name] += "," + HttpUtility.UrlEncode(value);
            return this;
        }
        /// <summary>        
        /// removes a name value pair from the querystring collection        
        /// </summary>        
        /// <param name="name">name of the querystring value to remove</param>        
        /// <returns>the QueryString object</returns>        
        public new QueryString Remove(string name)
        {
            string existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
                base.Remove(name);
            if (this.Count == 0)
                this.Add("AllRemoved", "");
            return this;
        }
        public QueryString Remove(string name, string value)
        {
            string existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
            {
                List<string> values = existingValue.Split(',').ToList();
                if (values.Count > 0)
                {
                    values.Remove(value);
                    if (values.Count > 1)
                        base[name] = string.Join(",", values.ToArray());
                    else if (values.Count == 1)
                    {
                        base[name] = values[0];
                    }
                    else
                        base.Remove(name);
                }
            }
            if (this.Count == 0)
                this.Add("AllRemoved", "");
            return this;
        }
        /// <summary>        
        /// clears the collection        
        /// </summary>        
        /// <returns>the QueryString object </returns>        
        public QueryString Reset()
        {
            base.Clear();
            return this;
        }
        /// <summary>        
        /// overrides the default        
        /// </summary>        
        /// <param name="name"></param>        
        /// <returns>the associated decoded value for the specified name</returns>        
        public new string this[string name]
        {
            get
            {
                return HttpUtility.UrlDecode(base[name]);
            }
        }
        /// <summary>        
        /// overrides the default indexer        
        /// </summary>        
        /// <param name="index"></param>        
        /// <returns>the associated decoded value for the specified index</returns>        
        public new string this[int index]
        {
            get
            {
                return HttpUtility.UrlDecode(base[index]);
            }
        }
        /// <summary>        
        /// checks if a name already exists within the query string collection        
        /// </summary>        
        /// <param name="name">the name to check</param>        
        /// <returns>a boolean if the name exists</returns>        
        public bool Contains(string name)
        {
            string existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }
        /// <summary>        
        /// outputs the querystring object to a string        
        /// </summary>        
        /// <returns>the encoded querystring as it would appear in a browser</returns>        
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    foreach (string val in base[base.Keys[i]].Split(','))
                        builder.Append((builder.Length == 0) ? "?" : "&").Append(HttpUtility.UrlEncode(base.Keys[i])).Append("=").Append(val);
                }
            }
            return builder.ToString();
        }
    }
    public class RequireRequestValueAttribute : ActionMethodSelectorAttribute
    {
        public RequireRequestValueAttribute(string valueName):this(valueName,"")
        {
            
        }
        public RequireRequestValueAttribute(string valueName, string notinValues)
        {
            Fields = valueName;
            NotInFields = notinValues;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var fields = Fields.Split(',').ToList();
            bool requiredFileds =  !(fields.Where(u => controllerContext.HttpContext.Request[u.Trim()] == null).Count() > 0);
            var notinFields = NotInFields.Split(',').ToList();
            bool requiredNotidFieldFileds = string.IsNullOrWhiteSpace(NotInFields) || !(notinFields.Where(u => controllerContext.HttpContext.Request[u.Trim()] != null).Count() > 0);
            return requiredFileds && requiredNotidFieldFileds;
            //bool result = false;
            //foreach (var item in fields)
            //{

            //}
            //return (controllerContext.HttpContext.Request[ValueName] != null);
        }
        public string NotInFields { get; private set; }
        public string Fields { get; private set; }
    }
    public class NotRequireRequestValueAttribute : ActionMethodSelectorAttribute
    {
        public NotRequireRequestValueAttribute(string valueName)
        {
            ValueName = valueName;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return !(controllerContext.HttpContext.Request[ValueName] != null);
        }
        public string ValueName { get; private set; }
    }
}
