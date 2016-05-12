using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;

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

        public string SendNewFeedNotificationToUsers(UserFollowersVertexModelResponse userFollowersDeserialized, string messagePushNotification)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            if (userFollowersDeserialized == null || userFollowersDeserialized.results==null)
                return null;

            foreach (var user in userFollowersDeserialized.results)
            {
                if (!string.IsNullOrWhiteSpace(user._id))
                    new ChatHub().AddNotificationMessage("3", messagePushNotification, user._id);
            }

            return null;
        }
    }
}
