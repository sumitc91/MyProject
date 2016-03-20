using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrVirtualFriends
{
    public interface ISolrVirtualFriends
    {
        Dictionary<String, String> InsertVirtualFriendListToSolr(List<UnVirtualFriendSolr> solrvirtualFriendList,bool toBeOptimized);
    }
}
