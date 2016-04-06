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
    public class GraphDbContract:IGraphDbContract
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
            properties[VertexPropertyEnum.ImageUrl.ToString()] = user.imageUrl;
            properties[VertexPropertyEnum.CoverImageUrl.ToString()] = user.userCoverPic;

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(user.email, TitanGraphConfig.Graph, properties);

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewWorkgraphyInGraphDb(urNoticeSession session,StoryPostRequest story)
        {
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Workgraphy.ToString();            
            properties[VertexPropertyEnum.Email.ToString()] = story.Data.email;            
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();            
            //properties[VertexPropertyEnum.CoverImageUrl.ToString()] = story.ImgurList.Count>0?story.ImgurList[0].:CommonConstants.CompanySquareLogoNotAvailableImage;
            properties[VertexPropertyEnum.Heading.ToString()] = story.Data.heading;

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(story.Data.email, TitanGraphConfig.Graph, properties);

           
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.PublishedBy.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);

            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = story.Data.companyVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WorkgraphyStory.ToString();

            graphEdgeDbModel = new GraphEdgeDb();
            addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);

            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = story.Data.designationVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WorkgraphyDesignationStory.ToString();

            graphEdgeDbModel = new GraphEdgeDb();
            addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewDesignationInGraphDb(string adminEmail, string designationName)
        {
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Designation.ToString();
            properties[VertexPropertyEnum.DesignationName.ToString()] = designationName;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(adminEmail, TitanGraphConfig.Graph, properties);

            return addVertexResponse;
        }

        public Dictionary<string, string> InsertNewCompanyInGraphDb(string adminEmail, string companyName)
        {
            string url = TitanGraphConfig.Server;

            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Company.ToString();
            properties[VertexPropertyEnum.CompanyName.ToString()] = companyName;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            Dictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(adminEmail, TitanGraphConfig.Graph, properties);//new GraphVertexOperations().AddVertex(adminEmail, url, companyName, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            return addVertexResponse;
        }

        public String CompanySalaryInfo(string companyVertexId, string from, string to)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            
            //string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it, commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[0..5].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + companyVertexId + ").out('Salary').transform{[salaryInfo:it.inE('Salary')[0..5],designationInfo:it]}";
            string gremlinQuery = "g.v(" + companyVertexId + ").transform{[salaryInfo:it.outE('Salary'),designationInfo:it.out('Salary')]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, companyVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
    }
}
