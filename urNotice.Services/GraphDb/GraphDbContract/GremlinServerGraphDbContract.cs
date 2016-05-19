using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.GraphDb.GraphDbContract
{
    public class GremlinServerGraphDbContract : IGraphDbContract
    {
        public Dictionary<string, string> InsertNewUserInGraphDb(OrbitPageUser user)
        {
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.User.ToString();
            properties[VertexPropertyEnum.FirstName.ToString()] = user.firstName;
            properties[VertexPropertyEnum.LastName.ToString()] = user.lastName;
            properties[VertexPropertyEnum.Username.ToString()] = user.email;
            properties[VertexPropertyEnum.Gender.ToString()] = user.gender;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.CreatedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();
            properties[VertexPropertyEnum.ImageUrl.ToString()] = user.imageUrl;
            properties[VertexPropertyEnum.CoverImageUrl.ToString()] = user.userCoverPic ?? "";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(user.email, TitanGraphConfig.Graph, properties, null, null, null);

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewWorkgraphyInGraphDb(urNoticeSession session, StoryPostRequest story)
        {
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Workgraphy.ToString();
            properties[VertexPropertyEnum.Email.ToString()] = story.Data.email;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();
            //properties[VertexPropertyEnum.CoverImageUrl.ToString()] = story.ImgurList.Count>0?story.ImgurList[0].:CommonConstants.CompanySquareLogoNotAvailableImage;
            properties[VertexPropertyEnum.Heading.ToString()] = story.Data.heading;
            properties[VertexPropertyEnum.IsVerified.ToString()] = CommonConstants.FALSE;

            var hashtSet = new HashSet<String>() { session.UserVertexId };
            //var canEdit = new HashSet<String>() { session.UserVertexId};
            //var canDelete = new HashSet<String>() { session.UserVertexId };
            //var sendNotificationToUsers = new HashSet<String>() { session.UserVertexId };

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(story.Data.email, TitanGraphConfig.Graph, properties, hashtSet, hashtSet, hashtSet);


            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.PublishedBy.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);

            if (!string.IsNullOrEmpty(story.Data.companyVertexId))
            {
                properties = new Dictionary<string, string>();
                properties[EdgePropertyEnum._outV.ToString()] = story.Data.companyVertexId;
                properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
                properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WorkgraphyStory.ToString();

                //graphEdgeDbModel = new GraphEdgeDb();
                addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            }

            if (!string.IsNullOrEmpty(story.Data.designationVertexId))
            {
                properties = new Dictionary<string, string>();
                properties[EdgePropertyEnum._outV.ToString()] = story.Data.designationVertexId;
                properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
                properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WorkgraphyDesignationStory.ToString();

                //graphEdgeDbModel = new GraphEdgeDb();
                addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            }

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewDesignationInGraphDb(string adminEmail, string designationName)
        {
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Designation.ToString();
            properties[VertexPropertyEnum.DesignationName.ToString()] = designationName;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            //properties[VertexPropertyEnum.CreatedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(adminEmail, TitanGraphConfig.Graph, properties, null, null, null);

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewCompanyInGraphDb(string adminEmail, string companyName)
        {            
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Company.ToString();
            properties[VertexPropertyEnum.CompanyName.ToString()] = companyName;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            //properties[VertexPropertyEnum.CreatedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(adminEmail, TitanGraphConfig.Graph, properties, null, null, null);//new GraphVertexOperations().AddVertex(adminEmail, url, companyName, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            return addVertexResponse;
        }

        public string CompanySalaryInfo(string companyVertexId, string @from, string to)
        {
            
            //TODO: Query need to be changed.
            //string gremlinQuery = "g.v(" + companyVertexId + ").transform{[salaryInfo:it.outE('Salary'),designationInfo:it.out('Salary')]}";

            string gremlinQuery = "g.V(" + companyVertexId + ").as('company').outE('Salary').as('salaryInfo').select('company').out('Salary').as('designationInfo').select('designationInfo','salaryInfo');";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery); //graphVertexDb.GetVertexDetail(gremlinQuery, companyVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public string CompanyNoticePeriodInfo(string companyVertexId, string @from, string to)
        {           
            //TODO: Query need to be changed.
            //string gremlinQuery = "g.v(" + companyVertexId + ").transform{[range:it.outE('NoticePeriodRange'),designationInfo:it.out('NoticePeriodRange')]}";
            string gremlinQuery = "g.V(" + companyVertexId + ").as('company').outE('NoticePeriodRange').as('range').select('company').out('NoticePeriodRange').as('designationInfo').select('designationInfo','range');";
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public string CompanyWorkgraphyInfo(string companyVertexId, string username, string @from, string to)
        {
            //TODO: Query need to be changed.
            string gremlinQuery = "g.v(" + companyVertexId + ").transform{[workgraphyInfo:it.out('WorkgraphyStory').order{it.b.CreatedTime <=> it.a.CreatedTime}[" + from + ".." + to + "],count:it.in('Visited').count(),userCount:it.in('Visited').has('Username','" + username + "').count()]}";
            //string gremlinQuery1 = "g.v(" + companyVertexId + ").order{it.b.CreatedTime <=> it.a.CreatedTime}[" + from + ".." + to + "].transform{[workgraphyInfo:it.out('WorkgraphyStory')]}";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, companyVertexId, TitanGraphConfig.Graph, null);

            return response;
        }

        public Dictionary<string, string> PersonVisitedCompanyAddEdgeGraphDbAsync(string username, string vertexFrom, string vertexTo)
        {
            return AddEdgeGraphDbAsync(username, vertexFrom, vertexTo, EdgeLabelEnum.Visited.ToString());
        }

        private Dictionary<string, string> AddEdgeGraphDbAsync(string username, string vertexFrom, string vertexTo, string label)
        {

            var properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = vertexFrom;
            properties[EdgePropertyEnum._inV.ToString()] = vertexTo;
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();
            properties[EdgePropertyEnum._label.ToString()] = label;

            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            Dictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdgeAsync(username, TitanGraphConfig.Graph, properties);

            return addCreatedByEdgeResponse;
        }
    }
}
