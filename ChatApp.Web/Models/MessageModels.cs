using ChatApp.Data.Repository;
using ChatApp.Data.Surrogates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using ChatApp.Web.Helpers;
namespace ChatApp.Web.Models
{
    public class IncomingMessageViewModel
    {
        
        public string Message { get; set; }
        public AttachType AttachType { get; set; }
        public Guid? ReplyToMessageID { get; set; }
    }

    public enum AttachType
    {
        None = 0,
        Image = 1,
        Video = 2,
        PDF = 3,
        DOC = 4,
        Other = 5
    }
    public class OutgoingMessageViewModel
    {
        public Guid MessageID { get; set; }
        public string Nickname { get; set; }
        public int UserID { get; set; }
        public List<UserViewModel> ReadedUsers { get; set; }
        public OutgoingMessageViewModel ReplyToMessage { get; set; }
        public string Message { get; set; }
        //public AttachType AttachType { get; set; }
        //public string FilePath { get; set; }
        public DateTime ReceivedDate { get; set; }
        public static implicit operator OutgoingMessageViewModel(ChatMessageSurrogate b)
        {
            var user = MessageHelper.GetUser(b.UserID);
            var result = new OutgoingMessageViewModel();
            result.Message = b.Message;
            result.MessageID = b.ID;
            result.ReceivedDate = b.ReceivedDate;
            result.ReplyToMessage = b.ReplyToMessageID != null ? GetMessage(b.ReplyToMessageID.Value) : null;
            result.UserID = b.UserID;
            result.Nickname = user!= null ? user.NickName : "Taklaya gelmiş";
            result.ReadedUsers = b.ReadedUsers != null ?  b.ReadedUsers.Select(u=>(UserViewModel)u).ToList() : new List<UserViewModel>();
            result.Files = b.Files != null ? b.Files.Select(u => (MessageFileViewModel)u).ToList() : new List<MessageFileViewModel>();
            return result;
        }
        public bool LastReadedMessage { get; set; }
        public List<MessageFileViewModel> Files { get; set; }

        public static OutgoingMessageViewModel GetMessage(Guid messageID)
        {
            ChatMessageRepository cRepo = new ChatMessageRepository();
            var messageSurrogate = cRepo.Get(messageID);
            OutgoingMessageViewModel result = messageSurrogate;
            return result;
        }
    }

    public class MessageFileViewModel
    {
        public Guid MessageID { get; set; }
        public int AttachType { get; set; }
        public string FilePath { get; set; }

        public static implicit operator MessageFileViewModel(MessageFileSurrogate b)
        {
            var result = new MessageFileViewModel();
            result.FilePath = b.FilePath;
            result.AttachType= b.AttachType;
            result.MessageID = b.MessageID;
            return result;

        }
    }


    public class UserViewModel
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public static implicit operator UserViewModel(UserSurrogate b) 
        {
            var result = new UserViewModel();
            result.Name = b.NickName;
            result.UserID = b.ID;
            result.Avatar = b.Avatar;
            return result;
        }
    }
}