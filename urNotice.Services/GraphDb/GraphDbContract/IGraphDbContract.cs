using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.GraphDb.GraphDbContract
{
    public interface IGraphDbContract
    {
        Dictionary<String, String> InsertNewUserInGraphDb(OrbitPageUser user);
        Dictionary<String, String> InsertNewDesignationInGraphDb(String adminEmail, String designationName);
        Dictionary<String, String> InsertNewCompanyInGraphDb(String adminEmail, String companyName);
    }
}
