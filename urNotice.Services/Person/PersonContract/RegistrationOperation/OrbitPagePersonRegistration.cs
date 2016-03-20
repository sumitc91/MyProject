using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Services.GraphDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public class OrbitPagePersonRegistration : IOrbitPageRegistration
    {
        private ISolrUser _solrUserModel;
        private IGraphVertexDb _graphVertexDb;
        OrbitPagePersonRegistration(ISolrUser solrUserModel, IGraphVertexDb graphVertexDb)
        {
            this._solrUserModel = solrUserModel;
            this._graphVertexDb = graphVertexDb;
        }

        public ResponseModel<string> UserRegistration(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }
    }
}
