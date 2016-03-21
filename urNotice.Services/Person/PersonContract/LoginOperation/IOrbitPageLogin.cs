using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.Person.PersonContract.LoginOperation
{
    public interface IOrbitPageLogin
    {
        LoginResponse WebLogin(string userName, string password, string returnUrl, string keepMeSignedIn);
    }
}
