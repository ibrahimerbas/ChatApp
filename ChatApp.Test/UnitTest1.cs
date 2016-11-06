using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatApp.Web.Helpers;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using System.Dynamic;
using System.Security.Principal;
using ChatApp.Web;
using ChatApp.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using System.Collections;
using System.Collections.Generic;

namespace ChatApp.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MessagereadedTest()
        {
            
            bool messageReaded = false;
            var hub = new ChatHub("24ayar@mail.com", "242424");
            var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            hub.Clients = mockClients.Object;
            dynamic all = new ExpandoObject();
            dynamic others = new ExpandoObject();
            //all.broadcastMessage = new Action<string, string>((name, message) => {
                
            //});
            others.MessageReaded = new Action<UserViewModel, Guid>((user, messageID) =>{
                messageReaded = true;
            });
            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            mockClients.Setup(m => m.Others).Returns((ExpandoObject)others);
            hub.MessageReaded(new Guid("a2888ed6-394b-42b8-bd93-c0677d493f36"));
            Assert.IsTrue(messageReaded);
        }
    }

    public class UserPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
