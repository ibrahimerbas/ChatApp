
using _24ayar.Data.Utility;
using ChatApp.Data;
using ChatApp.Data.Surrogates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Data.Repository
{
    public class ChatMessageRepository : BaseRepository<Guid, ChatMessageSurrogate, ChatAppEntities, ChatMessage>
    {
        public override ChatMessageSurrogate CreateNewInstance(string initName)
        {
            return new ChatMessageSurrogate() { Message = initName };
        }

        protected override ChatMessage CreateNewInstanceDataModel()
        {
            return new ChatMessage();
        }

        protected override IQueryable<ChatMessageSurrogate> EntryToSurrogate(IQueryable<ChatMessage> query)
        {
            return (from c in query
                    select new ChatMessageSurrogate
                    {
                        ID = c.ID,
                        Message = c.Message,
                        ReplyToMessageID = c.ReplyToMessageID,
                        UserID = c.UserID,
                        ReadedUsers = c.AspNetUsers.Select(u => new UserSurrogate { ID = u.Id, NickName = u.NickName, Avatar = u.Avatar }).ToList(),
                        ReceivedDate = c.ReceivedDate,
                        Files = c.MessageFiles.Select(u=> new MessageFileSurrogate { AttachType = u.AttachType, FilePath = u.FilePath, ID  = u.id, MessageID = u.MessageID }).ToList()
                    });
        }

        protected override IQueryable<ChatMessage> Query(ChatAppEntities dataContext, Guid id)
        {
            return (from c in dataContext.ChatMessages where c.ID == id select c);
        }

        protected override IQueryable<ChatMessage> Query(ChatAppEntities dataContext, string searchText = "", string orderby = "")
        {
            return (from c in dataContext.ChatMessages where searchText != null ? c.Message.Contains(searchText) : true select c);
        }

        protected override void Save(ChatAppEntities dataContext, ChatMessage entry, ChatMessageSurrogate surrogate)
        {

            var readedUser = surrogate.ReadedUsers != null ? surrogate.ReadedUsers.Select(u => (from c in dataContext.AspNetUsers where c.Id == u.ID select c).SingleOrDefault()).ToList() : null;
            entry.AspNetUsers.Edit(dataContext, dataContext.AspNetUsers, readedUser, x => x.Id);

            var messageFiles = surrogate.Files != null ? surrogate.Files.Select(u => new MessageFile { AttachType = u.AttachType,  FilePath = u.FilePath}).ToList() : null;
            entry.MessageFiles.EditDetailed(dataContext, dataContext.MessageFiles, messageFiles, x => x.id);


            dataContext.SaveChanges();
            surrogate.ID = entry.ID;
        }

        protected override ChatMessage SurrogateToEntry(ChatMessageSurrogate surrogate, ChatMessage entry)
        {
            entry.ID = surrogate.ID;
            entry.Message = surrogate.Message;
            entry.ReplyToMessageID = surrogate.ReplyToMessageID;
            entry.UserID = surrogate.UserID;
            //entry.AttachType = surrogate.AttachType;
            //entry.FilePath = surrogate.FilePath;
            entry.ReceivedDate = surrogate.ReceivedDate;
            return entry;
        }
        public override IQueryable<ChatMessageSurrogate> Order(IQueryable<ChatMessageSurrogate> query, string orderby)
        {
            switch (orderby)
            {
                case "date":
                    query = query.OrderBy(u => u.ReceivedDate);
                    break;
                case "date desc":
                    query = query.OrderByDescending(u => u.ReceivedDate);
                    break;
                default:
                    break;
            }
            return query;
        }
    }
}
