using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Common.Infrastructure.Model.Person
{
    public interface IPerson
    {
        ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> Login(string userName, string password,bool decryptPassword);
        ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail);
    }
}
