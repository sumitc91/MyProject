using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Workgraphy
{
    public interface IWorkgraphyService
    {
        ResponseModel<StoryPostResponse> PublishNewWorkgraphy(urNoticeSession session, StoryPostRequest req,string type);
        SolrQueryResults<UnWorkgraphySolr> GetLatestWorkgraphy(int page, int perPage);
        SolrQueryResults<UnWorkgraphySolr> GetParticularWorkgraphyWithVertexId(int vertexId);
    }
}
