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

        /*[SolrField("display_designation")]
        public String display_designation { get; set; }*/
    }
}
