using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Management.PostManagement
{
    public interface IPostManagement
    {
        ResponseModel<string> EditMessageDetails(urNoticeSession session, EditMessageRequest messageReq);
        ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId, out HashSet<string> sendNotificationResponse);
        HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string commentVertexId, string postPostedByVertexId, string notificationType);
        ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, out HashSet<string> sendNotificationResponse);
        ResponseModel<String> DeleteCommentOnPost(urNoticeSession session, string vertexId);
        ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session,UserNewReactionRequest userNewReactionRequest, out HashSet<string> sendNotificationResponse);
        ResponseModel<String> RemoveReactionOnUserPost(urNoticeSession session, string vertexId);
    }
}
