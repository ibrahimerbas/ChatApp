using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ChatApp.Web.Models;
using ChatApp.Data.Repository;
using ChatApp.Data.Surrogates;
using Moq;
using System.Security.Principal;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Configuration;
using System.Threading;

namespace ChatApp.Web.Helpers
{
    public class ChatHub : Hub
    {
        
        //public Mock<IChatRepository> MockChatRepository { get; private set; }
        public IConfigurationManager _config;
        public ChatHub(string username, string password)

        {

            var identity = new GenericIdentity("24ayar@mail.com");
            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "2",System.Security.Claims.ClaimValueTypes.Integer));
            identity.AddClaim(new System.Security.Claims.Claim("NickName", "ibrahim", System.Security.Claims.ClaimValueTypes.Integer));
            //Thread.CurrentPrincipal = new GenericPrincipal(identity, null);
            //ApplicationSignInManager c = new ApplicationSignInManager("sa", HttpContext.GetOwinContext().Authentication)
            //SignInManager.PasswordSignIn(username, password, true, true);
            const string connectionId = "1234";
            const string hubName = "ChatHub";
            var resolver = new DefaultDependencyResolver();
            _config = resolver.Resolve<IConfigurationManager>();

            var mockConnection = new Mock<IConnection>();
            var mockUser = new Mock<IPrincipal>();
            //var mockCookies = new Mock<IRequestCookieCollection>();
            var mockHubPipelineInvoker = new Mock<IHubPipelineInvoker>();
            //IHubPipelineInvoker _pipelineInvoker = resolver.Resolve<IHubPipelineInvoker>();

            var mockRequest = new Mock<IRequest>();
            mockRequest.Setup(r => r.User).Returns(new GenericPrincipal(identity, null));
            //mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            StateChangeTracker tracker = new StateChangeTracker();

            //Clients = new HubConnectionContext(_pipelineInvoker, mockConnection.Object, hubName, connectionId, tracker);
            Clients = new HubConnectionContext(mockHubPipelineInvoker.Object, mockConnection.Object, hubName, connectionId, tracker);
            Context = new HubCallerContext(mockRequest.Object, connectionId);
            var x = Clients.Caller;
        }

        public static readonly ConcurrentDictionary<int, SignalRUser> Users
        = new ConcurrentDictionary<int, SignalRUser>();
         static ApplicationUserManager  _userManager;
        //private static ApplicationSignInManager _signInManager;

        private static ApplicationUserManager UserManager
        {
            get
            {
                return new ApplicationUserManager(new ApplicationUserStore(new ApplicationDbContext()));
            }
            set
            {
                _userManager = value;
            }
        }
        //public static ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? new System.Web.HttpContextWrapper(System.Web.HttpContext.Current).GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        public override System.Threading.Tasks.Task OnConnected()
        {
            if (!Context.User.Identity.IsAuthenticated)
                throw new Exception("Giriş yap da gel! Mevlana değiliz..");
            int userID = Context.User.Identity.GetUserId<int>();
            //if (!int.TryParse(Context.QueryString["ID"], out userID))
            //{
            //    throw new Exception("Kullanıcı ID'si gerekli...");
            //}
            //if (userID != HttpContext.Current.User.Identity.GetUserId<int>())
            //    throw new Exception("Çakallık yapmayalım. Lütfen!!");
            string nickName = Context.User.Identity.NickName();


            string connectionId = Context.ConnectionId;
            var user = Users.GetOrAdd(userID, _ => new SignalRUser
            {
                UserID = userID,
                Name = nickName,
                ConnectionIds = new ConcurrentBag<string>()

            });
            if (user != null)
            {
                if (!user.ConnectionIds.Any())
                    this.Clients.All.UserJoined(new UserViewModel { Name = nickName, UserID = userID });
                if (!user.ConnectionIds.Where(u => u == connectionId).Any())
                {
                    user.ConnectionIds.Add(connectionId);
                }
            }
            var otherUsers = Users.Where(u => u.Value.UserID != userID).Select(u => new UserViewModel { UserID = u.Value.UserID, Name = u.Value.Name, Avatar = this.Context.User.Identity.Avatar() }).ToList();
            //this.Clients.Caller.UsersInit(otherUsers);
            var lastMessageID = Context.User.Identity.LastReadedMessage() ?? MessageHelper.GetLastMessage();
            if (lastMessageID != null) {
                var initMessages = MessageHelper.GetInitMessage(lastMessageID.Value);
                if (initMessages != null && initMessages.Count > 0)
                    this.Clients.Caller.InitChatRoom(initMessages, otherUsers);
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            int userID = Context.User.Identity.GetUserId<int>();

            string nickName = Context.User.Identity.NickName();
            SignalRUser user;
            Users.TryGetValue(userID, out user);
            string connectionId = Context.ConnectionId;
            if (user != null)
            {
                user.ConnectionIds.TryTake(out connectionId);
                if (!user.ConnectionIds.Any())
                {
                    Users.TryRemove(userID, out user);
                    if (!user.ConnectionIds.Any())
                        this.Clients.All.UserLeft(new UserViewModel { Name = nickName, UserID = userID });
                }
            }
            return base.OnDisconnected(stopCalled);
        }

        public override System.Threading.Tasks.Task OnReconnected()
        { 
            return base.OnReconnected();
        }

        public Guid? SendMessage(IncomingMessageViewModel message)
        {
            try
            {
                int userID = HttpContext.Current.User.Identity.GetUserId<int>();
                string nickName = HttpContext.Current.User.Identity.NickName();
                ChatMessageRepository cRepo = new ChatMessageRepository();
                var messageSurrogate = cRepo.CreateNewInstance(message.Message);
                //messageSurrogate.AttachType = (byte)message.AttachType;

                messageSurrogate.ReplyToMessageID = message.ReplyToMessageID;
                messageSurrogate.UserID = userID;
                messageSurrogate.ReceivedDate = DateTime.Now;
                cRepo.Save(messageSurrogate);
                OutgoingMessageViewModel outgoingMessage = messageSurrogate;
                Clients.All.IncomingMessage(outgoingMessage);
                //if ((AttachType)messageSurrogate.AttachType != AttachType.None)
                //{
                //    this.Clients.Caller.StartMessageFileUpload(messageSurrogate.ID);
                //}
                return messageSurrogate.ID;
            }
            catch
            {
                return null;
            }
        }

        public List<OutgoingMessageViewModel> ScrollMoveMessages(Guid messageID, bool Up)
        {
            
            int totalCount = 0;
            ChatMessageRepository cRepo = new ChatMessageRepository();
            var message = cRepo.Get(messageID);
            var messages = cRepo.GetAll(null, 0, 30, out totalCount, null, (ChatMessageSurrogate u) => Up ? u.ReceivedDate < message.ReceivedDate : u.ReceivedDate > message.ReceivedDate);
            return messages.Select(u => (OutgoingMessageViewModel)u).ToList();//.Cast<OutgoingMessageViewModel>().ToList();
        }

        public void MessageReaded(Guid messageID)
        {
            int userid = Context.User.Identity.GetUserId<int>();
            var nickname = Context.User.Identity.NickName();
            ChatMessageRepository cRepo = new ChatMessageRepository();
            var message = cRepo.Get(messageID);
            if (message != null)
            {
                if (!message.ReadedUsers.Any(u => u.ID == userid))
                {
                    message.ReadedUsers.Add(new UserSurrogate { ID = userid, NickName = Context.User.Identity.NickName() });
                    cRepo.Save(message);
                    var user = UserManager.Users.Where(u => u.Id == userid).SingleOrDefault();
                    user.LastReadedMessage = messageID;
                    UserManager.Update(user);
                }
                Clients.Others.MessageReaded(new UserViewModel { UserID = userid, Name = nickname }, messageID);                
            }
        }
    }
    public class SignalRUser
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public ConcurrentBag<string> ConnectionIds { get; set; }
    }


}
