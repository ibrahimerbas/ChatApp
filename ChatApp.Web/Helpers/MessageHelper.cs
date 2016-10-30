using ChatApp.Data.Repository;
using ChatApp.Data.Surrogates;
using ChatApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApp.Web.Helpers
{
    public class MessageHelper
    {
        public static List<OutgoingMessageViewModel> GetInitMessage(Guid lastMessageID)
        {
            List<OutgoingMessageViewModel> result = new List<OutgoingMessageViewModel>();
            var cRepo = new ChatMessageRepository();
            var lastMessage = cRepo.Get(lastMessageID);
            if (lastMessage != null)
            {
                int totalCount = 0;
                var beforeMessages = cRepo.GetAll(null, 30, 0, out totalCount, "date", (ChatMessageSurrogate u) => u.ReceivedDate < lastMessage.ReceivedDate);
                var afterMessages = cRepo.GetAll(null, 30, 0, out totalCount, "date", (ChatMessageSurrogate u) => u.ReceivedDate > lastMessage.ReceivedDate);
                result.AddRange(beforeMessages.Cast<OutgoingMessageViewModel>());
                OutgoingMessageViewModel lastOutgoinMessage = lastMessage;
                lastOutgoinMessage.LastReadedMessage = true;
                result.Add(lastOutgoinMessage);
                result.AddRange(afterMessages.Cast<OutgoingMessageViewModel>());
            }
            return result;
        }
    }
}