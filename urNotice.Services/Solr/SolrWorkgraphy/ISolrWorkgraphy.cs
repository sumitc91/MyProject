using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrWorkgraphy
{
    public interface ISolrWorkgraphy
    {
        Dictionary<String,String> InsertNewWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy, bool optimize);
        SolrQueryResults<UnWorkgraphySolr> GetLatestWorkgraphy(int page, int perPage);
        SolrQueryResults<UnWorkgraphySolr> GetParticularWorkgraphyWithVertexId(int vertexId);
    }
}
