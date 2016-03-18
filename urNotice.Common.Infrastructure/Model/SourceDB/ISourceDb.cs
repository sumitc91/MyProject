using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Common.Infrastructure.Model.SourceDB
{
    public interface ISourceDb
    {
        Dictionary<String, String> InsertNewUser(OrbitPageUser user, bool toBeOptimized); 
    }
}
