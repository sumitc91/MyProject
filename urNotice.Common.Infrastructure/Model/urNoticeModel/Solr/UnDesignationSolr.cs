using System;
using SolrNet.Attributes;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    public class UnDesignationSolr
    {
        
        [SolrUniqueKey("id")]
        public String id { get; set; }

        [SolrField("designation")]
        public String designation { get; set; }

        [SolrField("vertexId")]
        public String vertexId { get; set; }
    }
}
