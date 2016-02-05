using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Attributes;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    public class UnVirtualFriendSolr
    {
        [SolrUniqueKey("id")]
        public String Id { get; set; }

        [SolrField("user1")]
        public String User1 { get; set; }

        [SolrField("user2")]
        public String User2 { get; set; }

        [SolrField("type")]
        public String Type { get; set; }

        [SolrField("source")]
        public String Source { get; set; }

        [SolrField("name1")]
        public String Name1 { get; set; }

        [SolrField("name2")]
        public String Name2 { get; set; }

        [SolrField("isfriend")]
        public Boolean Isfriend { get; set; }

        public UnVirtualFriendSolr ConvertVirtualFriendForSolr(VirtualFriendList virtualFriend,Boolean isPhone)
        {
            var virtualFriendForSolr = new UnVirtualFriendSolr();
            if (virtualFriend != null)
            {
                
                virtualFriendForSolr.User1 = virtualFriend.id1;
                if (isPhone)
                {
                    virtualFriendForSolr.User2 = OrbitPageUtil.ConvertVirtualFriendListToUnVirtualFriendSolrPhoneWithoutFormatting(virtualFriend.id2);
                }
                else
                {
                    virtualFriendForSolr.User2 = virtualFriend.id2;
                }
                virtualFriendForSolr.Id = virtualFriend.id1 + virtualFriendForSolr.User2;
                virtualFriendForSolr.Type = virtualFriend.type;
                virtualFriendForSolr.Source = virtualFriend.source;
                virtualFriendForSolr.Name1 = virtualFriend.name1;
                virtualFriendForSolr.Name2 = virtualFriend.name2;
                virtualFriendForSolr.Isfriend = false;
            }
            return virtualFriendForSolr;
        }
      
    }
}
