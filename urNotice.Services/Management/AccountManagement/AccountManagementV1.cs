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
    public class AccountManagementV1 : IAccountManagement
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();            
            IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();

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
            IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();

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

                IGraphVertexDb graphVertexDbModel = new GremlinServerGraphVertexDb();
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
        public ResponseModel<IDictionary<string, string>> UserConnectionRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel, out HashSet<string> sendNotificationHashSetResponse)
        {
            var response = new ResponseModel<IDictionary<string, string>>();
            sendNotificationHashSetResponse = new HashSet<string>();

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
                        sendNotificationHashSetResponse.Add(userConnectionRequestModel.UserVertexId);
                        response.Payload = CreateNewAssociateRequest(session, userConnectionRequestModel);
                        CreateNewFollowRequest(session, userConnectionRequestModel.UserVertexId, session.UserVertexId, userConnectionRequestModel);
                        //response.Payload.Concat(CreateNewFollowRequest(session, userConnectionRequestModel).Where( x=> !response.Payload.Keys.Contains(x.Key)));
                        //response.Payload.ToList().ForEach(x => CreateNewFollowRequest(session, userConnectionRequestModel).Add(x.Key, x.Value));                        
                        break;
                    case CommonConstants.AssociateAccept:
                        response.Payload = CreateNewFriend(session, userConnectionRequestModel);
                        CreateNewFollowRequest(session, userConnectionRequestModel.UserVertexId, session.UserVertexId, userConnectionRequestModel);
                        RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId);
                        //response.Payload.Concat(RemoveAssociateRequestEdge(session.UserVertexId,userConnectionRequestModel.UserVertexId).Where(x => !response.Payload.Keys.Contains(x.Key)));
                        //response.Payload.ToList().ForEach(x => RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId).Add(x.Key, x.Value));
                        break;
                    case CommonConstants.AssociateFollow:
                        response.Payload = CreateNewFollowRequest(session, userConnectionRequestModel.UserVertexId, session.UserVertexId, userConnectionRequestModel);
                        break;
                    case CommonConstants.AssociateReject:
                        response.Payload = RemoveAssociateRequestEdge(session.UserVertexId, userConnectionRequestModel.UserVertexId);
                        break;
                    case CommonConstants.RemoveFollow:
                        response.Payload = RemoveFollowEdge(userConnectionRequestModel.UserVertexId, session.UserVertexId);
                        break;
                    case CommonConstants.AssociateRequestCancel:
                        response.Payload = RemoveAssociateRequestEdge(userConnectionRequestModel.UserVertexId, session.UserVertexId);
                        break;
                    case CommonConstants.Deassociate:
                        RemoveFriendEdge(userConnectionRequestModel.UserVertexId, session.UserVertexId);
                        response.Payload = RemoveFollowEdge(userConnectionRequestModel.UserVertexId, session.UserVertexId);
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
            dynamoDbModel.UpsertOrbitPageUpdateLastNotificationSeenTimeStamp(userName, DynamoDbHashKeyDataType.LastSeenNotification.ToString(), DateTimeUtil.GetUtcTime().Ticks);
            response.Status = 200;
            response.Message = "success";
            return response;
        }


        //For Gremling Query

        public string GetUserNotification(urNoticeSession session, string from, string to)
        {

            //string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";
            string gremlinQuery = string.Empty;
            gremlinQuery += "g.V(" + session.UserVertexId + ").in('NotificationSent').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('notificationInfo').match(";
            gremlinQuery += "__.as('notificationInfo').in('RelatedPost').as('postInfo'),";
            gremlinQuery += "__.as('notificationInfo').in('CreatedNotification').as('notificationByUser'),";
            gremlinQuery +=").select('notificationInfo','postInfo','notificationByUser')";
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
            return response;
        }

        public string GetAllFollowers(string vertexId)
        {

            string gremlinQuery = "g.V(" + vertexId + ").in('Follow')";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);
            return response;
        }

        public string GetUserFriendRequestNotification(urNoticeSession session, string from, string to)
        {
            //string gremlinQuery = "g.v(" + session.UserVertexId + ").inE('AssociateRequest').order{it.b.PostedDateLong <=> it.a.PostedDateLong}[" + from + ".." + to + "].transform{ [requestInfo:it,requestedBy:it.outV]}";
            string gremlinQuery = string.Empty;
            gremlinQuery += "g.V(" + session.UserVertexId + ").inE('AssociateRequest').order().by('PostedDateLong', decr).range(" + from + "," + to + ").as('requestInfo').match(";
            gremlinQuery += "__.as('requestInfo').outV().as('requestedBy'),";
            gremlinQuery+=").select('requestInfo','requestedBy')";
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);
            return response;
        }
        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;

            //string gremlinQuery = "g.V(" + userVertexId + ").in('WallPost').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('wallpostinfo','likeInfoCount','userInfo','postedOn','likeInfo','isLiked').select('wallpostinfo','likeInfoCount','userInfo','postedOn','likeInfo','isLiked').by().by(__.in('Like').values('Username').count()).by(__.in('Created')).by(__.out('WallPost')).by(__.in('Like').fold()).by(__.in('Like').has('Username','" + userEmail + "').fold())";
            //string gremlinQuery = "g.V(" + userVertexId + ").in('WallPost').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('wallpostinfo','likeInfoCount','userInfo','postedOn','likeInfo','isLiked','commentsCount','commentsInfo').select('wallpostinfo','likeInfoCount','userInfo','postedOn','likeInfo','isLiked','commentsCount','commentsInfo').by().by(__.in('Like').values('Username').count()).by(__.in('Created')).by(__.out('WallPost')).by(__.in('Like').fold()).by(__.in('Like').has('Username','" + userEmail + "').fold()).by(__.in('Comment').count()).by(__.in('Comment').fold())";
            string gremlinQuery = string.Empty;

            gremlinQuery += "g.V(" + userVertexId + ").in('WallPost').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('wallpostinfo').match(";
            gremlinQuery +="__.as('wallpostinfo').in('Like').values('Username').count().as('likeInfoCount'),";
            gremlinQuery +="__.as('wallpostinfo').in('Created').as('userInfo'),";
            gremlinQuery += "__.as('wallpostinfo').out('WallPost').fold().as('postedOn'),";
            gremlinQuery +="__.as('wallpostinfo').in('Like').fold().as('likeInfo'),";
            gremlinQuery += "__.as('wallpostinfo').in('Like').has('Username','" + userEmail + "').fold().as('isLiked'),";
            gremlinQuery +="__.as('wallpostinfo').in('Comment').count().as('commentsCount'),";
            gremlinQuery += "__.as('wallpostinfo').in('Comment').order().by('PostedTime', decr).range(" + messageStartIndex + "," + messageEndIndex + ").as('commentData').match(";
            gremlinQuery +="__.as('commentData').in('Created').as('commentedBy'),";
            gremlinQuery +="__.as('commentData').in('Like').count().as('likeCount'),";
            gremlinQuery += "__.as('commentData').in('Like').has('Username','" + userEmail + "').fold().as('isCommentLiked'),";
            gremlinQuery += ").select('commentData','commentedBy','likeCount','isCommentLiked').fold().as('commentsInfo'),";
            gremlinQuery += ").select('wallpostinfo','userInfo','postedOn','likeInfo','likeInfoCount','isLiked','commentsCount','commentsInfo')";

            
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);

            return response;
        }

        public string GetUserOrbitFeedPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;

            //string gremlinQuery = "g.v(" + userVertexId + ").out('Follow').in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it,likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},commentsCount: it.in('Comment').count(),userInfo:it.in('Created'),postedOn:it.out('WallPost')] }";
            //string gremlinQuery = "g.V(" + userVertexId + ").in('WallPost').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('wallpostinfo','likeInfo','likeInfoCount','userInfo','postedOn').select('wallpostinfo','likeInfo','likeInfoCount','userInfo','postedOn').by().by(__.in('Like')).by(__.in('Like').values('Username').count()).by(__.in('Created')).by(__.out('WallPost'))";
            string gremlinQuery = string.Empty;

            gremlinQuery += "g.V(" + userVertexId + ").out('Follow').in('WallPost').order().by('PostedTimeLong', decr).range(" + from + "," + to + ").as('wallpostinfo').match(";
            gremlinQuery += "__.as('wallpostinfo').in('Like').values('Username').count().as('likeInfoCount'),";
            gremlinQuery += "__.as('wallpostinfo').in('Created').as('userInfo'),";
            gremlinQuery += "__.as('wallpostinfo').out('WallPost').fold().as('postedOn'),";            
            gremlinQuery += "__.as('wallpostinfo').in('Like').fold().as('likeInfo'),";
            gremlinQuery += "__.as('wallpostinfo').in('Like').has('Username','" + userEmail + "').fold().as('isLiked'),";
            gremlinQuery += "__.as('wallpostinfo').in('Comment').count().as('commentsCount'),";
            gremlinQuery += "__.as('wallpostinfo').in('Comment').order().by('PostedTime', decr).range(" + messageStartIndex + "," + messageEndIndex + ").as('commentData').match(";
            gremlinQuery += "__.as('commentData').in('Created').as('commentedBy'),";
            gremlinQuery += "__.as('commentData').in('Like').count().as('likeCount'),";
            gremlinQuery += "__.as('commentData').in('Like').has('Username','" + userEmail + "').fold().as('isCommentLiked'),";
            gremlinQuery += ").select('commentData','commentedBy','likeCount','isCommentLiked').fold().as('commentsInfo'),";
            gremlinQuery += ").select('wallpostinfo','userInfo','postedOn','likeInfo','likeInfoCount','isLiked','commentsCount','commentsInfo')";
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);

            return response;
        }
        public string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            //string gremlinQuery = "g.v(" + userVertexId + ").in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[commentInfo:it, commentedBy: it.in('Created'),,likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]}";
            string gremlinQuery = string.Empty;
            gremlinQuery += "g.V(" + userVertexId + ").in('Comment').order().by('PostedTime', decr).range(" + from + "," + to + ").as('commentInfo').match(";
            gremlinQuery += "__.as('commentInfo').in('Created').as('commentedBy'),";
            gremlinQuery += "__.as('commentInfo').in('Like').count().as('likeCount'),";
            gremlinQuery += "__.as('commentInfo').in('Like').has('Username','" + userEmail + "').fold().as('isLiked'),";
            gremlinQuery += ").select('commentInfo','commentedBy','likeCount','isLiked')";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
        public string GetUserPostLikes(string userVertexId, string @from, string to)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            //string gremlinQuery = "g.v(" + userVertexId + ").in('Like').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[likeInfo:it]}";
            string gremlinQuery = "g.V(" + userVertexId + ").in('Like').order().by('PostedTime', decr).range(" + from + "," + to + ")";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            int messageStartIndex = 0;
            int messageEndIndex = 5;
            //string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'),likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";
            string gremlinQuery = string.Empty;
            gremlinQuery += "g.V(" + vertexId + ").as('wallpostinfo').match(";
            gremlinQuery += "__.as('wallpostinfo').out('WallPost').fold().as('postedOn'),";            
            gremlinQuery += "__.as('wallpostinfo').in('Like').range(0,2).fold().as('likeInfo'),";
            gremlinQuery += "__.as('wallpostinfo').in('Created').as('userInfo'),";
            gremlinQuery += "__.as('wallpostinfo').in('Like').count().as('likeInfoCount'),";
            gremlinQuery += "__.as('wallpostinfo').in('Comment').count().as('commentsCount'),";
            gremlinQuery += "__.as('wallpostinfo').in('Like').has('Username','" + userEmail + "').fold().as('isLiked'),";
            gremlinQuery += "__.as('wallpostinfo').in('Comment').order().by('PostedTime', decr).range(" + messageStartIndex + "," + messageEndIndex + ").as('commentData').match(";
            gremlinQuery += "__.as('commentData').in('Created').as('commentedBy'),";
            gremlinQuery += "__.as('commentData').in('Like').count().as('likeCount'),";
            gremlinQuery += "__.as('commentData').in('Like').has('Username','" + userEmail + "').fold().as('isCommentLiked'),";
            gremlinQuery +=").select('commentData','commentedBy','likeCount','isCommentLiked').fold().as('commentsInfo'),";
            gremlinQuery += ").select('wallpostinfo','userInfo','postedOn','likeInfo','likeInfoCount','isLiked','commentsCount','commentsInfo')";

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

            return response;
        }
        public string GetUserNetworkDetail(urNoticeSession session, string userVertexId, string from, string to)
        {

            string gremlinQuery = string.Empty;
            gremlinQuery += "g.V(" + userVertexId + ").as('userInfo').match(";
            gremlinQuery += "__.as('userInfo').in('AssociateRequest').has('Username','" + session.UserName + "').fold().as('associateRequestSent'),";
            gremlinQuery += "__.as('userInfo').out('AssociateRequest').has('Username','" + session.UserName + "').fold().as('associateRequestReceived'),";
            gremlinQuery += "__.as('userInfo').in('Follow').has('Username','" + session.UserName + "').fold().as('followRequestSent'),";
            gremlinQuery += "__.as('userInfo').in('Friend').has('Username','" + session.UserName + "').fold().as('isFriend'),";
            gremlinQuery += ").select('associateRequestSent','associateRequestReceived','followRequestSent','isFriend')";
            //string gremlinQuery = "g.v(" + userVertexId + ").transform{[associateRequestSent:it.in('AssociateRequest').has('Username','" + session.UserName + "'),associateRequestReceived:it.out('AssociateRequest').has('Username','" + session.UserName + "'),followRequestSent:it.in('Follow').has('Username','" + session.UserName + "'),isFriend:it.in('Friend').has('Username','" + session.UserName + "')]}";
            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
            string response = graphVertexDb.ExecuteGremlinQuery(gremlinQuery);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
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

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
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

            IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
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
                        DynamoDbHashKeyDataType.EdgeDetail.ToString(),
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
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
        private IDictionary<string, string> CreateNewFriend(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();

            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(userConnectionRequestModel.UserVertexId, EdgeLabelEnum.Friend.ToString(), session.UserVertexId);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.EdgeDetail.ToString(),
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
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);


            properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = userConnectionRequestModel.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Friend.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
        private IDictionary<string, string> RemoveAssociateRequestEdge(string inV, string outV)
        {
            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            return graphEdgeDbModel.DeleteEdge(inV, outV, EdgeLabelEnum.AssociateRequest.ToString());
        }

        private IDictionary<string, string> RemoveFriendEdge(string inV, string outV)
        {
            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            graphEdgeDbModel.DeleteEdge(outV, inV, EdgeLabelEnum.Friend.ToString());
            return graphEdgeDbModel.DeleteEdge(inV, outV, EdgeLabelEnum.Friend.ToString());
        }

        private IDictionary<string, string> RemoveFollowEdge(string inV, string outV)
        {
            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            return graphEdgeDbModel.DeleteEdge(inV, outV, EdgeLabelEnum.Follow.ToString());
        }
        private IDictionary<string, string> CreateNewFollowRequest(urNoticeSession session, string inV, string outV, UserConnectionRequestModel userConnectionRequestModel)
        {

            IDynamoDb dynamoDbModel = new DynamoDb();

            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(inV, EdgeLabelEnum.Follow.ToString(), outV);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.EdgeDetail.ToString(),
                        uniqueKey,
                        null);

            if (edgeInfo != null)
            {
                var response = new Dictionary<string, string>();
                response.Add("Follow", "Follow is already sent to this user.");
                return response;
            }

            var properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = outV;
            properties[EdgePropertyEnum._inV.ToString()] = inV;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Follow.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

            IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return addEdgeResponse;
        }
    }
}
