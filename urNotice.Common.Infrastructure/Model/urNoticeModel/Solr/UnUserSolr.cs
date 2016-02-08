using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Attributes;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Solr
{
    public class UnUserSolr
    {
        [SolrUniqueKey("id")]
        public String Id { get; set; }

        [SolrField("firstname")]
        public String Firstname { get; set; }

        [SolrField("lastname")]
        public String Lastname { get; set; }

        [SolrField("name")]
        public String Name { get; set; }

        [SolrField("gender")]
        public String Gender { get; set; }

        [SolrField("profilepic")]
        public String Profilepic { get; set; }

        [SolrField("coverpic")]
        public String Coverpic { get; set; }

        [SolrField("isactive")]
        public Boolean Isactive { get; set; }

        [SolrField("source")]
        public String Source { get; set; }

        [SolrField("email")]
        public String Email { get; set; }

        [SolrField("fid")]
        public String Fid { get; set; }

        [SolrField("phone")]
        public String Phone { get; set; }

        [SolrField("uidcode")]
        public String Uidcode { get; set; }

        [SolrField("username")]
        public String Username { get; set; }

        [SolrField("vertexId")]
        public String VertexId { get; set; }

        [SolrField("friends")]
        public String[] Friends { get; set; }

        [SolrField("virtualfriend")]
        public String[] Virtualfriend { get; set; }
        
    }
}
