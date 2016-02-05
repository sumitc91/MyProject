using System.Collections.Generic;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class UserMessagesResponse
    {
        public string UnreadMessages { get; set; }
        public string CountLabelType { get; set; }
        public List<UserMessageList> MessageList { get; set; }
    }
}