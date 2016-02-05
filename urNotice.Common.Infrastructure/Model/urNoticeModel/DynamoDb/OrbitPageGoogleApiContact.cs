using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageGoogleApiContact
    {
        public int Id { get; set; }
        public string emailId { get; set; }
        public System.DateTime createdDate { get; set; }
        public int startIndex { get; set; }
        public int itemPerPage { get; set; }
        public string lastContactUpdated { get; set; }
        public string entryListString { get; set; }
        public bool isSolrUpdated { get; set; }
        public bool isVirtualFriendListUpdated { get; set; }
    }
}
