using System;

namespace urNotice.Common.Infrastructure.Session
{
    public class urNoticeSession
    {
        public urNoticeSession(string userName,string userVertexId)
        {
            var now = DateTime.Now;
            this.SessionId = Guid.NewGuid().ToString();
            this.UserName = userName;
            this.UserVertexId = userVertexId;
        }
        public urNoticeSession(string userName, string Guid, string userVertexId)
        {
            var now = DateTime.Now;
            this.SessionId = Guid;
            this.UserName = userName;
            this.UserVertexId = userVertexId;
        }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string UserVertexId { get; set; }
        public dynamic SignalRClient { get; set; }
    }
}