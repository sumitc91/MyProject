using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public interface IOrbitPageRegistration
    {
        ResponseModel<LoginResponse> RegisterUser(RegisterationRequest req, HttpRequestBase request);
        void SetIsValidationEmailRequired(bool res);
    }
}
