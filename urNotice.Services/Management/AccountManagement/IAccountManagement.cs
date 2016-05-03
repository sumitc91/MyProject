using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Management.AccountManagement
{
    public interface IAccountManagement
    {
        ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> Login(string userName, string password, bool isSocialLogin);
        ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail);
        ResponseModel<ClientDetailsModel> GetPersonDetails(string userEmail);
        ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel);

        ResponseModel<string> ValidateAccountService(ValidateAccountRequest req);
        ResponseModel<string> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request);
        ResponseModel<string> ForgetPasswordService(string id, HttpRequestBase request);
        ResponseModel<string> ResetPasswordService(ResetPasswordRequest req);
        ResponseModel<string> ContactUsService(ContactUsRequest req);
        ResponseModel<string> SeenNotification(string userName);


        //for Gremlin
        string GetUserNotification(urNoticeSession session, string from, string to);
        string GetUserPost(string userVertexId, string @from, string to, string userEmail);
        string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail);
        string GetUserPostLikes(string userVertexId, string @from, string to);
        string GetPostByVertexId(string vertexId, string userEmail);
        long GetUserUnreadNotificationCount(urNoticeSession session);
    }
}
