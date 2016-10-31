using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
namespace _24ayar.Web.Utility
{
    public class RssActionResult : ActionResult
    {
        private SyndicationFeed feed;
        public RssActionResult(SyndicationFeed Feed)
        {
            this.feed = Feed;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            Atom10FeedFormatter rssFormatter = new Atom10FeedFormatter(feed);
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }
    }
}
