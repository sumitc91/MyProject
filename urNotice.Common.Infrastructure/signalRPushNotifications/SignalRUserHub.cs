using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Common.Infrastructure.signalRPushNotifications
{
    public class SignalRUserHub : Hub
    {
        private static Dictionary<string, dynamic> connectedUsers = new Dictionary<string, dynamic>();

        public void RegisterUser(string tokenId)
        {
            urNoticeSession session = TokenManager.getSessionInfo(tokenId);
            if (session != null)
            {
                SignalRManager.SignalRCreateManager(session.UserName, Clients.Caller);
                Clients.Caller.addMessage("'" + session.UserName + "'registered.");
            }            
        }

        public void AddNotification(string notificationMessage, string toUser)
        {
            dynamic client = SignalRManager.getSignalRDetail(toUser);
            if(client != null)
                client.addMessage(notificationMessage);
            
        }
    }
}