using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.SourceDB;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Common.Infrastructure.Model.Solr.SolrUser
{
    public interface ISolrUser : ISourceDb
    {                
        UnUserSolr GetPersonData(String email, String username, String phone, String fid,Boolean isFullDetails);
        Dictionary<String, String> InsertNewUserToSolr(UnUserSolr solrUser, bool toBeOptimized);
    }
}
