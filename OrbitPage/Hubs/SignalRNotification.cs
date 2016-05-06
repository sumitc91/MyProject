using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace OrbitPage.Hubs
{
    public class SignalRNotification
    {        
        public string SendNewPostNotification(HashSet<string> vertexIds)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            foreach (var vertexId in vertexIds)
            {
                if(!string.IsNullOrWhiteSpace(vertexId))
                    new ChatHub().AddNotificationMessage("1","message", vertexId);
            }

            return null;
        }

        public string SendNewConnectionRequestNotification(HashSet<string> vertexIds)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            foreach (var vertexId in vertexIds)
            {
                if(!string.IsNullOrWhiteSpace(vertexId))
                    new ChatHub().AddNotificationMessage("2",vertexId + " Sent Friend Request", vertexId);
            }

            return null;
        }
    }
}
