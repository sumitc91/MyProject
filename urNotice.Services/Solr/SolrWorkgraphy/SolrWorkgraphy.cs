using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrWorkgraphy
{
    public class SolrWorkgraphy: ISolrWorkgraphy
    {
        public Dictionary<string, string> InsertNewWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy, bool optimize)
        {
            var solrWorkgraphy = new UnWorkgraphySolr()
            {
                city = orbitPageWorkgraphy.city,
                company_name = orbitPageWorkgraphy.companyName,
                company_vertex_id = orbitPageWorkgraphy.companyVertexId,
                country = orbitPageWorkgraphy.country,
                created_by_email = orbitPageWorkgraphy.createdByEmail,
                created_by_vertex_id = orbitPageWorkgraphy.createdByVertexId,
                created_date = DateTimeUtil.GetUtcTimeString(),
                designation_name = orbitPageWorkgraphy.designation,
                designation_vertex_id = orbitPageWorkgraphy.designationVertexId,
                district = orbitPageWorkgraphy.district,
                formatted_address = orbitPageWorkgraphy.formatted_address,                
                heading = orbitPageWorkgraphy.heading,                
                id = orbitPageWorkgraphy.workgraphyVertexId,
                images = orbitPageWorkgraphy.ImagesUrl.ToArray(),
                is_anonymous = Convert.ToBoolean(orbitPageWorkgraphy.shareAnonymously),
                latitude = orbitPageWorkgraphy.latitude,
                longitude = orbitPageWorkgraphy.longitude,
                postal_code = orbitPageWorkgraphy.postal_code,
                short_desc = orbitPageWorkgraphy.heading,
                state = orbitPageWorkgraphy.state,
                story = orbitPageWorkgraphy.story,
                sublocality = orbitPageWorkgraphy.sublocality,
                vertex_id = orbitPageWorkgraphy.workgraphyVertexId
                
            };

            if (orbitPageWorkgraphy.ImagesUrl != null && orbitPageWorkgraphy.ImagesUrl.Count > 0)
                solrWorkgraphy.icon_image = orbitPageWorkgraphy.ImagesUrl[0];
            else
                solrWorkgraphy.icon_image = CommonConstants.WorkgraphyIconImage;

            if (orbitPageWorkgraphy.latitude != null && orbitPageWorkgraphy.longitude != null)
                solrWorkgraphy.geo = orbitPageWorkgraphy.latitude + "," + orbitPageWorkgraphy.longitude;
            

            return InsertNewWorkgraphyToSolr(solrWorkgraphy,false);
        }

        public Dictionary<String, String> InsertNewWorkgraphyToSolr(UnWorkgraphySolr solrWorkgraphy, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnWorkgraphySolr>>();

            solr.Add(solrWorkgraphy);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }
    }
}
