using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    class TestSolrModel
    {

        [SolrUniqueKey("id")]
        public String id { get; set; }

        [SolrField("title")]
        public String title { get; set; }

        [SolrField("description")]
        public String description { get; set; }

        [SolrField("author")]
        public String author { get; set; }

        [SolrField("tags")]
        public List<String> tags { get; set; }

        [SolrField("tagsString")]
        public String tagsString { get; set; }

        [SolrField("imageurl_l")]
        public String imageurl_l { get; set; }

        [SolrField("last_modified")]
        public String last_modified { get; set; }

    }
}
