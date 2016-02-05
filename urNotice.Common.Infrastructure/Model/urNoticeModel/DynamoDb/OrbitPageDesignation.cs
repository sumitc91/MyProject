using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageDesignation
    {
        public int Id { get; set; }
        public string designationName { get; set; }
        public string displayName { get; set; }
        public bool isVerified { get; set; }
        public string verifiedBy { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdDate { get; set; }
        public string guid { get; set; }
        public Nullable<bool> isSolrUpdated { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}
