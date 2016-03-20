using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrUser
{
    public class SolrUser : ISolrUser
    {
        public Dictionary<String, String> InsertNewUserToSolr(UnUserSolr solrUser, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnUserSolr>>();

            solr.Add(solrUser);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }

        public SolrQueryResults<UnUserSolr> GetUserDetailsAutocomplete(string queryText)
        {
            queryText = queryText.Replace(" ", "*");            
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();

            String solrQueryString = "(name:" + queryText + "*) || (lastname:" + queryText + "*) || (email:" + queryText + ") || (username:" + queryText + ") || (phone:" + queryText + ")";
            var solrQuery = new SolrQuery(solrQueryString);
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 15,
                Start = 0,
                Fields = new[] { "email", "name", "profilepic", "vertexId" }
            });
            return solrQueryExecute;
        }

        public SolrQueryResults<UnUserSolr> UserDetailsById(string uid)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
            var solrQuery = new SolrQuery("(username:" + uid + ") || (vertexId:" + uid + ")");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 10,
                Start = 0,
                Fields = new[] { "id", "firstname", "lastname", "name", "gender", "profilepic", "isactive", "source", "email", "phone", "username", "coverpic" }
            });

            return solrQueryExecute;
        }

        public Dictionary<string, string> InsertNewUser(OrbitPageUser user, bool toBeOptimized)
        {
            var solrUser = new UnUserSolr
            {
                Id = user.email,
                Fid = user.fid,
                Coverpic = user.userCoverPic,
                Username = user.username,
                Email = user.email,
                Firstname = user.firstName,
                Lastname = user.lastName,
                Name = user.firstName + " " + user.lastName,
                Phone = (user.phone ?? 0).ToString(CultureInfo.InvariantCulture),
                Profilepic = user.imageUrl,
                Source = user.source,
                Uidcode = user.password,//CommonConstants.NA,//user.password,                
                Gender = user.gender,
                Isactive = Convert.ToBoolean(user.isActive),
                VertexId = user.vertexId
            };

            return InsertNewUserToSolr(solrUser, toBeOptimized);            
        }

        public UnUserSolr GetPersonData(string uniqueId, string username, string phone, string fid, bool isFullDetails) //uniqueId is generally email.
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
            var query = new StringBuilder();
            query.Append("((id:" + uniqueId + ") OR (username:" + uniqueId + ") OR (phone:" + uniqueId + ") OR (fid:" + uniqueId + "))");
            if (username != null)
                query.Append(" OR (username:" + username + ")");
            if (phone != null)
                query.Append(" OR (phone:" + phone + ")");
            if (phone != null)
                query.Append(" OR (fid:" + fid + "))");

            var solrQuery = new SolrQuery(query.ToString());
            String[] fieldsToBeReturned = {"id", "email"};
            
            if (isFullDetails)
            {
                fieldsToBeReturned = new String[]
                {
                    "id", "firstname", "lastname", "gender", "profilepic", "isactive", "source", "email", "phone",
                    "uidcode", "username", "friends", "virtualfriend", "vertexId"
                };
            }
            
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 1,
                Start = 0,
                Fields = fieldsToBeReturned
            });

            if (solrQueryExecute == null || solrQueryExecute.Count == 0)
            {
                return null;
            }
            
            return solrQueryExecute[0];
        }
    }
}
