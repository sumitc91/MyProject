using System;
using System.Web;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.Person
{
    public class Consumer : IPerson
    {
        public string RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<LoginResponse> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
