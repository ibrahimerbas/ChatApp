using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Web.Helpers
{
    public abstract class HubController : Controller
    {
        Lazy<IHubContext>chatHub = new Lazy<IHubContext>(
                () => GlobalHost.ConnectionManager.GetHubContext<ChatHub>()
            );
       
        protected IHubContext ChatHub
        {
            get { return chatHub.Value; }
        }
    }
}