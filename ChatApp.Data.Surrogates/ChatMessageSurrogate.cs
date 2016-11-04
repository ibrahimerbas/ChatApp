using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Data.Surrogates
{
    public class ChatMessageSurrogate:BaseSurrogate<Guid>
    {
        public int UserID { get; set; }
        public string Message { get; set; }   
        public Guid? ReplyToMessageID { get; set; }
        public DateTime ReceivedDate { get; set; }
        public List<UserSurrogate> ReadedUsers { get; set; }
        public List<MessageFileSurrogate> Files { get; set; }
    }

    public class UserSurrogate:BaseSurrogate<int>
    {
        public string NickName { get; set; }
        public string Avatar { get; set; }
    }

    public class MessageFileSurrogate : BaseSurrogate<int>
    {
        public string FilePath { get; set; }
        public int AttachType { get; set; }
        public Guid MessageID { get; set; }
    }
}
