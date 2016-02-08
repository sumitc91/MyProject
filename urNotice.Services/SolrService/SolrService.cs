using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.SolrService
{
    public class SolrService
    {
        public Dictionary<String, String> InsertNewUserToSolr(OrbitPageUser user, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnUserSolr>>();
            String[] friends = new String[1];
            friends[0] = user.email;

            String[] virtualfriends = new String[1];
            virtualfriends[0] = user.email;
            var friendsList = new List<String>();
            var userToBeMovedToSolr = new UnUserSolr
            {
                Id = user.email,
                Fid=user.fid,
                Coverpic = user.userCoverPic,
                Username = user.username,
                Email = user.email,
                Firstname = user.firstName,
                Lastname = user.lastName,
                Name = user.firstName + " " +user.lastName,
                Phone = (user.phone??0).ToString(CultureInfo.InvariantCulture),
                Profilepic = user.imageUrl,
                Source = user.source,
                Uidcode = user.password,//CommonConstants.NA,//user.password,
                Friends = friends,
                Virtualfriend = virtualfriends,
                Gender = user.gender,
                Isactive = Convert.ToBoolean(user.isActive),
                VertexId = user.vertexId
            };

            solr.Add(userToBeMovedToSolr);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }

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

        public Dictionary<String, String> InsertVirtualFriendListToSolr(List<UnVirtualFriendSolr> solrvirtualFriendList, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnVirtualFriendSolr>>();

            solr.AddRange(solrvirtualFriendList);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }

        public UnUserSolr GetSolrUserData(String email,String username,String phone,String fid)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
            var query = new StringBuilder();
            query.Append("((id:" + email + ") OR (username:" + email + ") OR (phone:" + email + ") OR (fid:" + email + "))");
            if(username != null)
                query.Append(" OR (username:" + username + ")");
            if (phone != null)
                query.Append(" OR (phone:" + phone + ")");
            if (phone != null)
                query.Append(" OR (fid:" + fid + "))");

            var solrQuery = new SolrQuery(query.ToString());
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 1,
                Start = 0,
                Fields = new[] { "id", "email" }
            });

            if (solrQueryExecute == null || solrQueryExecute.Count == 0)
            {
                return null;
            }
            else
            {
                return solrQueryExecute[0];
            }
        }

        public UnUserSolr GetSolrUserFullData(String email, String username, String phone)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
            var query = new StringBuilder();
            query.Append("((id:" + email + ") OR (email:" + email + ") OR (phone:" + email + "))");
            if (username != null)
                query.Append("OR ((id:" + username + ") OR (email:" + username + ") OR (phone:" + username + "))");
            if (phone != null)
                query.Append("OR ((id:" + phone + ") OR (email:" + phone + ") OR (phone:" + phone + "))");

            var solrQuery = new SolrQuery(query.ToString());
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 1,
                Start = 0,
                Fields = new[] { "id", "firstname", "lastname", "gender", "profilepic", "isactive", "source", "email", "phone", "uidcode", "username", "friends", "virtualfriend" }
            });

            if (solrQueryExecute == null || solrQueryExecute.Count == 0)
            {
                return null;
            }
            else
            {
                return solrQueryExecute[0];
            }
        }

        public ResponseModel<ClientDetailsModel> GetClientDetails(string username)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            try
            {
                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
                var solrQuery = new SolrQuery("(id:" + username + ")");
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 1,
                    Start = 0,
                    Fields = new[] { "id", "firstname", "lastname", "gender", "profilepic", "isactive", "source", "email","vertexId" }
                });

                if (solrQueryExecute != null && solrQueryExecute.Count > 0)
                {

                    var userDetail = new ClientDetailsModel
                    {
                        Email = solrQueryExecute[0].Email,
                        FirstName = solrQueryExecute[0].Firstname,
                        gender = solrQueryExecute[0].Gender,
                        imageUrl = solrQueryExecute[0].Profilepic,
                        IsActive = solrQueryExecute[0].Isactive,
                        LastName = solrQueryExecute[0].Lastname,
                        Username = solrQueryExecute[0].Username,
                        vertexId = solrQueryExecute[0].VertexId
                    };

                    response.Status = 200;
                    response.Message = "success";
                    response.Payload = userDetail;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                response.Status = 500;
                response.Message = "exception occured !!!";
            }
            return response;
        }

    }
}
