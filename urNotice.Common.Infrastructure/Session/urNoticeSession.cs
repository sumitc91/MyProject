using System;

namespace urNotice.Common.Infrastructure.Session
{
    public class urNoticeSession
    {
        public urNoticeSession(string userName)
        {
            var now = DateTime.Now;
            this.SessionId = Guid.NewGuid().ToString();
            this.UserName = userName;
        }
        public urNoticeSession(string userName, string Guid)
        {
            var now = DateTime.Now;
            this.SessionId = Guid;
            this.UserName = userName;
        }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public dynamic SignalRClient { get; set; }
    }
}