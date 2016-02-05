using System;
using SolrNet.Attributes;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    public class UnCompanySolr
    {
        [SolrUniqueKey("id")]
        public String id { get; set; }

        [SolrField("guid")]
        public String guid { get; set; }

        [SolrField("companyid")]
        public String companyid { get; set; }

        [SolrField("companyname")]
        public String companyname { get; set; }

        [SolrField("rating")]
        public float rating { get; set; }

        [SolrField("website")]
        public String website { get; set; }

        [SolrField("size")]
        public long size { get; set; }

        [SolrField("description")]
        public String description { get; set; }

        [SolrField("averagerating")]
        public float averagerating { get; set; }

        [SolrField("totalratingcount")]
        public long totalratingcount { get; set; }

        [SolrField("totalreviews")]
        public long totalreviews { get; set; }

        [SolrField("isprimary")]
        public Boolean isprimary { get; set; }

        [SolrField("logourl")]
        public String logourl { get; set; }

        [SolrField("squarelogourl")]
        public String squarelogourl { get; set; }

        [SolrField("speciality")]
        public String[] speciality { get; set; }

        [SolrField("telephone")]
        public String[] telephone { get; set; }

        [SolrField("avgnoticeperiod")]
        public float avgnoticeperiod { get; set; }

        [SolrField("buyoutpercentage")]
        public float buyoutpercentage { get; set; }

        [SolrField("maxnoticeperiod")]
        public long maxnoticeperiod { get; set; }

        [SolrField("minnoticeperiod")]
        public long minnoticeperiod { get; set; }

        [SolrField("avghikeperct")]
        public float avghikeperct { get; set; }

        [SolrField("perclookingforchange")]
        public float perclookingforchange { get; set; }

        [SolrField("sublocality")]
        public String sublocality { get; set; }

        [SolrField("city")]
        public String city { get; set; }

        [SolrField("district")]
        public String district { get; set; }

        [SolrField("formatted_address")]
        public String formatted_address { get; set; }

        [SolrField("state")]
        public String state { get; set; }

        [SolrField("country")]
        public String country { get; set; }

        [SolrField("postal_code")]
        public string postal_code { get; set; }

        [SolrField("latitude")]
        public float latitude { get; set; }

        [SolrField("longitude")]
        public float longitude { get; set; }

        [SolrField("geo")]
        public String geo { get; set; }
    }
}
