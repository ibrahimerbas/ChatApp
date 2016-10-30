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
        List<UserViewModel> ReadedUsers { get; set; }
        public OutgoingMessageViewModel ReplyToMessage { get; set; }
        public string Message { get; set; }
        public AttachType AttachType { get; set; }
        public string FilePath { get; set; }
        public DateTime ReceivedDate { get; set; }
        public static implicit operator OutgoingMessageViewModel(ChatMessageSurrogate b)
        {
            var result = new OutgoingMessageViewModel();
            result.AttachType = (AttachType)b.AttachType;
            result.FilePath = b.FilePath;
            result.Message = b.Message;
            result.MessageID = b.ID;
            result.ReceivedDate = b.ReceivedDate;
            result.ReplyToMessage = b.ReplyToMessageID != null ? GetMessage(b.ReplyToMessageID.Value) : null;
            result.UserID = b.UserID;
            result.Nickname = HttpContext.Current.User.Identity.NickName();
            result.ReadedUsers = b.ReadedUsers.Cast<UserViewModel>().ToList();
            return result;
        }
        public bool LastReadedMessage { get; set; }
        public static OutgoingMessageViewModel GetMessage(Guid messageID)
        {
            ChatMessageRepository cRepo = new ChatMessageRepository();
            var messageSurrogate = cRepo.Get(messageID);
            OutgoingMessageViewModel result = messageSurrogate;
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
            return result;

        }
    }
}