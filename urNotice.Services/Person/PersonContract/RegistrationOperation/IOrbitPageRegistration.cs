using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public interface IOrbitPageRegistration
    {
        ResponseModel<string> RegisterUser(RegisterationRequest req, HttpRequestBase request);
        void SetIsValidationEmailRequired(bool res);
    }
}
