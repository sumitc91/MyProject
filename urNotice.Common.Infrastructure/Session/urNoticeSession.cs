using System;

namespace urNotice.Common.Infrastructure.Session
{
    public class urNoticeSession
    {
        public urNoticeSession(string userName,string displayName,string userVertexId,string imageUrl)
        {
            var now = DateTime.Now;
            this.SessionId = Guid.NewGuid().ToString();
            this.UserName = userName;
            this.UserVertexId = userVertexId;
            this.DisplayName = displayName;
            this.ImageUrl=imageUrl;
        }
        public urNoticeSession(string userName,string displayName, string Guid, string userVertexId,string imageUrl)
        {
            var now = DateTime.Now;
            this.SessionId = Guid;
            this.UserName = userName;
            this.UserVertexId = userVertexId;
            this.DisplayName = displayName;
            this.ImageUrl = imageUrl;
        }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public string UserVertexId { get; set; }
        public dynamic SignalRClient { get; set; }
    }
}