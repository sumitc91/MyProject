using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageWorkgraphy
    {
        public String heading { get; set; }
        public String companyName { get; set; }
        public String companyVertexId { get; set; }
        public String story { get; set; }
        public String createdByEmail { get; set; }
        public String createdByVertexId { get; set; }        
        public String designation { get; set; }
        public String designationVertexId { get; set; }
        public String workgraphyVertexId { get; set; }
        public string city { get; set; }
        public string sublocality { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public string district { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string formatted_address { get; set; }
        public String shareAnonymously { get; set; }
        public Nullable<bool> isSolrUpdated { get; set; }

        public List<String> ImagesUrl { get; set; }
    }
}
