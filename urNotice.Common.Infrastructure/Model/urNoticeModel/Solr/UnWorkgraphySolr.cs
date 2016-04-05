using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Attributes;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    public class UnWorkgraphySolr
    {
        [SolrUniqueKey("id")]
        public String id { get; set; }

        [SolrField("vertex_id")]
        public String vertex_id { get; set; }

        [SolrField("heading")]
        public String heading { get; set; }

        [SolrField("short_desc")]
        public String short_desc { get; set; }

        [SolrField("story")]
        public String story { get; set; }

        [SolrField("company_name")]
        public String company_name { get; set; }

        [SolrField("company_vertex_id")]
        public String company_vertex_id { get; set; }

        [SolrField("is_anonymous")]
        public Boolean is_anonymous { get; set; }

        [SolrField("is_email_verified")]
        public Boolean is_email_verified { get; set; }

        [SolrField("is_admin_verified")]
        public Boolean is_admin_verified { get; set; }

        [SolrField("created_by_email")]
        public String created_by_email { get; set; }

        [SolrField("created_by_vertex_id")]
        public String created_by_vertex_id { get; set; }

        [SolrField("icon_image")]
        public String icon_image { get; set; }

        [SolrField("images")]
        public String[] images { get; set; }

        [SolrField("created_date")]
        public String created_date { get; set; }

        [SolrField("designation_name")]
        public String designation_name { get; set; }



        [SolrField("designation_vertex_id")]
        public String designation_vertex_id { get; set; }

        [SolrField("city")]
        public String city { get; set; }

        [SolrField("sublocality")]
        public String sublocality { get; set; }

        [SolrField("state")]
        public String state { get; set; }

        [SolrField("postal_code")]
        public String postal_code { get; set; }

        [SolrField("country")]
        public String country { get; set; }

        [SolrField("district")]
        public String district { get; set; }

        [SolrField("latitude")]
        public String latitude { get; set; }

        [SolrField("longitude")]
        public String longitude { get; set; }

        [SolrField("formatted_address")]
        public String formatted_address { get; set; }

        [SolrField("geo")]
        public String geo { get; set; }

    }
}
