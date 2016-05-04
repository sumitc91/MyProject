using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.Email.EmailTemplate;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Person.PersonContract.RegistrationOperation;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Management.AccountManagement
{
    public class AccountManagement:IAccountManagement
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(true); //email validation is required.
            var response = orbitPageRegistration.RegisterUser(req, request);            
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }
        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(false); //email validation not required for social.
            var response = orbitPageRegistration.RegisterUser(req, request);            
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }
        public ResponseModel<LoginResponse> Login(string userName, string password, bool isSocialLogin)
        {
            IOrbitPageLogin loginModel = new OrbitPageLogin();

            return loginModel.Login(userName, password, null, CommonConstants.TRUE, isSocialLogin);

        }
        //Accessible From Admin only.
        public ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail)
        {
            var response = new ResponseModel<OrbitPageUser>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                userEmail,
                null
                );

            response.Status = 200;
            response.Message = "success";
            response.Payload = userInfo.OrbitPageUser;
            return response;
        }
        public ResponseModel<ClientDetailsModel> GetPersonDetails(string userEmail)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        userEmail,
                        null);

            if (userInfo != null)
            {
                var createClientDetailResponse = new ClientDetailsModel
                {
                    FirstName = userInfo.OrbitPageUser.firstName,
                    LastName = userInfo.OrbitPageUser.lastName,
                    Username = userInfo.OrbitPageUser.email,
                    imageUrl = userInfo.OrbitPageUser.imageUrl == CommonConstants.NA ? CommonConstants.clientImageUrl : userInfo.OrbitPageUser.imageUrl,
                    gender = userInfo.OrbitPageUser.gender,
                    isLocked = userInfo.OrbitPageUser.locked
                };

                response.Status = 200;
                response.Message = "success";
                response.Payload = createClientDetailResponse;

            }
            else
            {
                response.Status = 404;
                response.Message = "username not found";
            }

            return response;
        }
        public ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel)
        {
            var response = new ResponseModel<EditPersonModel>();

            if (session.UserName != editPersonModel.Email)
            {
                response.Status = 401;
                response.Message = "Unauthenticated";
                return response;
            }

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        editPersonModel.Email,
                        null);
            if (userInfo != null)
            {
                //update dynamodb
                var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
                orbitPageCompanyUserWorkgraphyTable = GenerateOrbitPageUserObject(userInfo, editPersonModel);
                dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(orbitPageCompanyUserWorkgraphyTable);

                //update graphDb
                var properties = new Dictionary<string, string>();
                properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.User.ToString();

                if (!string.IsNullOrEmpty(editPersonModel.FirstName))
                    properties[VertexPropertyEnum.FirstName.ToString()] = editPersonModel.FirstName;

                if (!string.IsNullOrEmpty(editPersonModel.LastName))
                    properties[VertexPropertyEnum.LastName.ToString()] = editPersonModel.LastName;


                if (!string.IsNullOrEmpty(editPersonModel.ImageUrl))
                    properties[VertexPropertyEnum.ImageUrl.ToString()] = editPersonModel.ImageUrl;

                if (!string.IsNullOrEmpty(editPersonModel.CoverPic))
                    properties[VertexPropertyEnum.CoverImageUrl.ToString()] = editPersonModel.CoverPic;

                IGraphVertexDb graphVertexDbModel = new GraphVertexDb();
                graphVertexDbModel.UpdateVertex(userInfo.OrbitPageUser.vertexId, editPersonModel.Email, TitanGraphConfig.Graph, properties);

                //update solr
                ISolrUser solrUserModel = new SolrUser();
                solrUserModel.InsertNewUser(orbitPageCompanyUserWorkgraphyTable.OrbitPageUser, false);

                response.Status = 200;
                response.Message = "success";
                response.Payload = editPersonModel;
            }
            else
            {
                response.Status = 404;
                response.Message = "username not found";
            }

            return response;
        }
        public ResponseModel<IDictionary<string, string>> UserConnectionRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel)
        {
            var response = new ResponseModel<IDictionary<string, string>>();
            if (session.UserVertexId == userConnectionRequestModel.UserVertexId)
            {
                response.Status = 201;
                response.Message = "You cann't associate you with yourself.";
                return response;
            }

            if (userConnectionRequestModel.ConnectingBody == CommonConstants.AssociateUsers)
            {
                switch (userConnectionRequestModel.ConnectionType)
                {
                    case CommonConstants.AssociateRequest:
                        // Create Associate Request and follow the user by default.
                        response.Payload = CreateNewAssociateRequest(session, userConnectionRequestModel);
                        response.Payload.Concat(CreateNewFollowRequest(session, userConnectionRequestModel).Where( x=> !response.Payload.Keys.Contains(x.Key)));
                        //response.Payload.ToList().ForEach(x => CreateNewFollowRequest(session, userConnectionRequestModel).Add(x.Key, x.Value));                        
                        break;
                    case CommonConstants.AssociateAccept:
                        response.Payload = CreateNewFriend(session, userConnectionRequestModel);
                        response.Payload.Concat(RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId).Where(x => !response.Payload.Keys.Contains(x.Key)));
                        //response.Payload.ToList().ForEach(x => RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId).Add(x.Key, x.Value));
                        break;
                    case CommonConstants.AssociateFollow:
                        response.Payload = CreateNewFollowRequest(session, userConnectionRequestModel);
                        break;
                    case CommonConstants.AssociateReject:
                        response.Payload = RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId);
                        break;
                    case CommonConstants.RemoveFollow:
                        response.Payload = RemoveFollowEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId);
                        break;
                }
            }
            return response;
        }



        public ResponseModel<string> ValidateAccountService(ValidateAccountRequest req)
        {
            var response = new ResponseModel<string>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );

            if (userInfo != null)
            {

                if (userInfo.OrbitPageUser == null)
                {
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    return response;
                }
                if (userInfo.OrbitPageUser.isActive == "true")
                {
                    response.Status = 405;
                    response.Message = "already active user";
                    return response;
                }
                userInfo.OrbitPageUser.isActive = "true";

                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                }
                catch (Exception ex)
                {
                    //Todo:need to log exception.
                    response.Status = 500;
                    response.Message = "Failed";
                    return response;
                }
                response.Status = 200;
                response.Message = "validated";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }
        public ResponseModel<string> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request)
        {
            var response = new ResponseModel<string>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );

            if (userInfo != null)
            {
                if (userInfo.OrbitPageUser.isActive == CommonConstants.TRUE)
                {
                    // Account has been already validated.
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }

                userInfo.OrbitPageUser.validateUserKeyGuid = Guid.NewGuid().ToString();
                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                    SendAccountCreationValidationEmail.SendAccountValidationEmailMessage(req.userName, userInfo.OrbitPageUser.validateUserKeyGuid, request);
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error !!!";
                    return response;
                }
                response.Status = 200;
                response.Message = "success";
                return response;
            }
            // User Doesn't Exist
            response.Status = 404;
            response.Message = "warning";
            return response;
        }
        public ResponseModel<string> ForgetPasswordService(string id, HttpRequestBase request)
        {
            var response = new ResponseModel<string>();
            id = id.ToLower();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                id,
                null
                );

            if (userInfo != null)
            {
                if ((userInfo.OrbitPageUser.isActive.Equals(CommonConstants.FALSE, StringComparison.InvariantCulture)))
                {
                    // User account has not validated yet
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }

                userInfo.OrbitPageUser.forgetPasswordGuid = Guid.NewGuid().ToString();
                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                    var forgetPasswordValidationEmail = new ForgetPasswordValidationEmail();
                    forgetPasswordValidationEmail.SendForgetPasswordValidationEmailMessage(id, userInfo.OrbitPageUser.forgetPasswordGuid, request, DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    return response;
                }
            }
            else
            {
                // User doesn't exist
                response.Status = 404;
                response.Message = "warning";
                return response;
            }
            response.Status = 200;
            response.Message = "Success";
            return response;
        }
        public ResponseModel<string> ResetPasswordService(ResetPasswordRequest req)
        {
            var response = new ResponseModel<string>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.Username,
                null
                );

            if (userInfo != null && userInfo.OrbitPageUser.forgetPasswordGuid == req.Guid && userInfo.OrbitPageUser.forgetPasswordGuid != CommonConstants.NA)
            {
                userInfo.OrbitPageUser.forgetPasswordGuid = CommonConstants.NA;

                var password = EncryptionClass.Md5Hash(req.Password);
                userInfo.OrbitPageUser.password = password;
                userInfo.OrbitPageUser.locked = CommonConstants.FALSE;

                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error.";
                    return response;
                }
                response.Status = 200;
                response.Message = "Success";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }
        public ResponseModel<string> ContactUsService(ContactUsRequest req)
        {
            //TODO: save in some database..
            var response = new ResponseModel<string>();
            var contactUsData = new ContactUs
            {
                Name = req.Name,
                Phone = req.Phone,
                RepliedBy = CommonConstants.NA,
                RepliedDateTime = CommonConstants.NA,
                ReplyMessage = CommonConstants.NA,
                Status = CommonConstants.status_open,
                Type = req.Type,
                dateTime = DateTime.Now,
                emailId = req.Email,
                heading = CommonConstants.NA,
                message = req.Message,
                username = req.Email
            };

            //_db.contactUs.Add(contactUsData);

            try
            {
                //_db.SaveChanges();
                //var contactUsEmailDelegate = ContactUsEmailSendDelegate(SendContactUsEmail.SendContactUsEmailMessage);

                string emailIds = req.SendMeACopy.Equals(CommonConstants.status_true,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? ServerConfig.ContactUsReceivingEmailIds + "," + req.Email
                    : ServerConfig.ContactUsReceivingEmailIds;

                //contactUsEmailDelegate.BeginInvoke(emailIds, req, null, null); //invoking the method

                //SendAccountCreationValidationEmail.SendContactUsEmailMessage(req.SendMeACopy.Equals(Constants.status_true,StringComparison.CurrentCultureIgnoreCase) ? ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture)+","+req.Email : ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture), req);
            }
            catch (Exception e)
            {
                //DbContextException.LogDbContextException(e);
                response.Status = 500;
                response.Message = "Internal Server Error.";
                return response;
            }
            response.Status = 200;
            response.Message = "success";
            return response;
        }
        public ResponseModel<string> SeenNotification(string userName)
        {
            var response = new ResponseModel<string>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageUpdateLastNotificationSeenTimeStamp(userName,DynamoDbHashKeyDataType.LastSeenNotification.ToString(), DateTimeUtil.GetUtcTime().Ticks);
            response.Status = 200;
            response.Message = "success";
            return response;
        }


        //For Gremling Query

        public string GetUserNotification(urNoticeSession session, string from, string to)
        {

            string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
            return response;
        }

        public string GetUserFriendRequestNotification(urNoticeSession session, string from, string to)
        {         
            string gremlinQuery = "g.v(" + session.UserVertexId + ").inE('AssociateRequest').order{it.b.PostedDateLong <=> it.a.PostedDateLong}[" + from + ".." + to + "].transform{ [requestInfo:it,requestedBy:it.outV]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);
            return response;
        }

        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;

            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it,likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);

            return response;
        }
        public string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            string gremlinQuery = "g.v(" + userVertexId + ").in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[commentInfo:it, commentedBy: it.in('Created'),,likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]}";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
        public string GetUserPostLikes(string userVertexId, string @from, string to)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            string gremlinQuery = "g.v(" + userVertexId + ").in('Like').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[likeInfo:it]}";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            int messageStartIndex = 0;
            int messageEndIndex = 4;
            string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'),likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, vertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

            return response;
        }

        public long GetUserUnreadNotificationCount(urNoticeSession session)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            long? lastNotificationSeenTimeStamp =
                dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableLastSeenNotifiationTimeStamp(session.UserName, NotificationEnum.Notifications.ToString());

            if (lastNotificationSeenTimeStamp == null)
                lastNotificationSeenTimeStamp = 0;

            string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').has('PostedDateLong',T.gte," + lastNotificationSeenTimeStamp + ").count()";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            var getUserUnreadNotificationsDeserialized =
                        JsonConvert.DeserializeObject<UserPostUnreadNotificationsResponse>(response);

            return getUserUnreadNotificationsDeserialized.results[0];
        }

        public long GetUserUnreadFriendRequestNotificationCount(urNoticeSession session)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            long? lastNotificationSeenTimeStamp =
                dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableLastSeenNotifiationTimeStamp(session.UserName, NotificationEnum.FriendRequests.ToString());
            
            if (lastNotificationSeenTimeStamp == null)
                lastNotificationSeenTimeStamp = 0;

            string gremlinQuery = "g.v(" + session.UserVertexId + ").inE('AssociateRequest').has('PostedDateLong',T.gte," + lastNotificationSeenTimeStamp + ").count()";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            var getUserUnreadNotificationsDeserialized =
                        JsonConvert.DeserializeObject<UserPostUnreadNotificationsResponse>(response);

            return getUserUnreadNotificationsDeserialized.results[0];
        }

        private OrbitPageCompanyUserWorkgraphyTable GenerateOrbitPageUserObject(
            OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable, EditPersonModel editPersonModel)
        {

            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.firstName = editPersonModel.FirstName;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.lastName = editPersonModel.LastName;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.imageUrl = editPersonModel.ImageUrl;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.userCoverPic = editPersonModel.CoverPic;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.lastUpdatedDate = DateTimeUtil.GetUtcTime();

            return orbitPageCompanyUserWorkgraphyTable;
        }

        private IDictionary<string, string> CreateNewAssociateRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            
            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(userConnectionRequestModel.UserVertexId, EdgeLabelEnum.AssociateRequest.ToString(), session.UserVertexId);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.EdgeForQueryDetail.ToString(),
                        uniqueKey,
                        null);

            if (edgeInfo != null)
            {
                var response = new Dictionary<string, string>();
                response.Add("AssociateRequest", "AssociateRequest is already sent to this user.");
                return response;
            }

            var properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = userConnectionRequestModel.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.AssociateRequest.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
        private IDictionary<string, string> CreateNewFriend(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            
            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(userConnectionRequestModel.UserVertexId, EdgeLabelEnum.Friend.ToString(), session.UserVertexId);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.EdgeForQueryDetail.ToString(),
                        uniqueKey,
                        null);

            if (edgeInfo != null)
            {
                var response = new Dictionary<string, string>();
                response.Add("Friend", "Friend is already friend to this user.");
                return response;
            }

            var properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = userConnectionRequestModel.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Friend.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);


            properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = userConnectionRequestModel.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Friend.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();

            addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
        private IDictionary<string, string> RemoveAssociateRequestEdge(string inV, string outV)
        {
            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            return graphEdgeDbModel.DeleteEdge(inV, outV, EdgeLabelEnum.AssociateRequest.ToString());
        }
        private IDictionary<string, string> RemoveFollowEdge(string inV, string outV)
        {
            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            return graphEdgeDbModel.DeleteEdge(inV, outV, EdgeLabelEnum.Follow.ToString());
        }
        private IDictionary<string, string> CreateNewFollowRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel)
        {

            IDynamoDb dynamoDbModel = new DynamoDb();
            
            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(userConnectionRequestModel.UserVertexId, EdgeLabelEnum.Follow.ToString(), session.UserVertexId);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.EdgeForQueryDetail.ToString(),
                        uniqueKey,
                        null);

            if (edgeInfo != null)
            {
                var response = new Dictionary<string, string>();
                response.Add("Follow", "Follow is already sent to this user.");
                return response;
            }

            var properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = userConnectionRequestModel.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Follow.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
    }
}
