using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Common.Infrastructure.Model.Person
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
