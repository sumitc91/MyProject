using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrVirtualFriends
{
    public class SolrVirtualFriends:ISolrVirtualFriends
    {
        public Dictionary<string, string> InsertVirtualFriendListToSolr(List<UnVirtualFriendSolr> solrvirtualFriendList, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnVirtualFriendSolr>>();
            solr.AddRange(solrvirtualFriendList);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();
            var response = new Dictionary<String, String>();
            response["status"] = "200";
            return response;
        }
    }
}
