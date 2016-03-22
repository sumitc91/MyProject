using System;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.AuthService
{
    public interface IAuthService
    {
        ResponseModel<LoginResponse> UserRegistration(RegisterationRequest req, HttpRequestBase request, string accessKey, string secretKey);
        ResponseModel<LoginResponse> WebLogin(string userName, string passwrod, string returnUrl, string keepMeSignedIn, string accessKey, string secretKey);
        ResponseModel<String> ValidateAccountService(ValidateAccountRequest req, string accessKey, string secretKey);
    }
}
