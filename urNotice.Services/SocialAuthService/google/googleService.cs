using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GoogleApiResponse;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Services.ErrorLogger;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.SocialAuthService.google.Model;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrVirtualFriends;

namespace urNotice.Services.SocialAuthService.google
{
    public class GoogleService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));       
        //private readonly urnoticeAnalyticsEntities _dbAnalytics = new urnoticeAnalyticsEntities();

        public delegate void GoogleApiContactSaveDelegate(string accessToken, string userEmail, string accessKey, string secretKey);

        public void GetUserFriendListAsync(string accessToken, string userEmail, string accessKey, string secretKey)
        {
            //var isValidToken = ThreadPool.QueueUserWorkItem(new WaitCallback(asyncLogoutThread), Request);
            GoogleApiContactSaveDelegate googleApiContactSaveDelegate = null;
            googleApiContactSaveDelegate = new GoogleApiContactSaveDelegate(GetUserFriendList);
            IAsyncResult CallAsynchMethod = null;
            CallAsynchMethod = googleApiContactSaveDelegate.BeginInvoke(accessToken,userEmail,accessKey,secretKey, null, null); //invoking the method
            
        }

        public void GetUserFriendList(string accessToken, string userEmail,string accessKey,string secretKey)
        {
            String URI = "";

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        userEmail,
                        null
                );

            //var googleContactSaved = _dbAnalytics.GoogleApiCheckLastSynceds.SingleOrDefault(x => x.emailId == userEmail);
            if (userInfo != null && userInfo.GoogleApiCheckLastSyncedDateTime != null)
            {
                URI =
                    "https://www.google.com/m8/feeds/contacts/default/full?alt=json&startIndex=1&itemsPerPage=25&updated-min=" +
                    userInfo.GoogleApiCheckLastSyncedDateTime;
            }
            else
            {
                URI = "https://www.google.com/m8/feeds/contacts/default/full?alt=json&startIndex=1&itemsPerPage=25";
            }
            
            var client = new RestClient(URI);
            var request = new RestRequest("", Method.GET);
            request.AddHeader("Authorization", "Bearer "+accessToken);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);
            var content = response.Content; // raw content as string

            content = content.Replace("openSearch$totalResults", "openSearchtotalResults").Replace("openSearch$itemsPerPage", "openSearchitemsPerPage").Replace("gd$email", "gdemail").Replace("gd$phoneNumber", "gdphoneNumber");
            try
            {
                var googleContacts = JsonConvert.DeserializeObject<GooglePlusContactListModel>(content);
                if (googleContacts.feed.entry == null)
                    return;

                int startIndex = 1;
                int perPage = 25;

                SaveGoogleApiContactToDynamoDb(userEmail, JsonConvert.SerializeObject(googleContacts.feed.entry), startIndex.ToString(CultureInfo.InvariantCulture),accessKey, secretKey);

                int totalContacts = Convert.ToInt32(googleContacts.feed.openSearchtotalResults["$t"].ToString());
                startIndex = 26;                
                try
                {
                    //_dbAnalytics.SaveChanges();
                    InsertIntoGoogleApiContactWithPagination(startIndex,perPage,totalContacts, accessToken, userEmail,accessKey,secretKey);

                    //new SyncService.SyncService().SyncGoogleApiContactList(userEmail);
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);                   
                }

                //Console.WriteLine(googleContacts);
            }
            catch (Exception ex)
            {
                Logger.Error("Error while saving google contacts : " + ex, ex);

            }
            
        }

        private void SaveGoogleApiContactToDynamoDb(string userEmail, string googleApiContactListString, string startIndex, string accessKey, string secretKey)
        {
            var virtualFriendListHash = new Dictionary<string, List<VirtualFriendList>>();
            var virtualFriendSolrListHash = new Dictionary<string, List<UnVirtualFriendSolr>>();
            var uniqueFriendListHash = new Dictionary<string, List<string>>();

            ISolrUser solrUserModel = new SolrUser();
            var solrUser = solrUserModel.GetPersonData(userEmail, null, null,null,true);//new SolrService.SolrService().GetSolrUserFullData(userEmail, null, null);

            uniqueFriendListHash[userEmail] = solrUser.Virtualfriend.ToList();

            if (!virtualFriendListHash.ContainsKey(userEmail))
                virtualFriendListHash[userEmail] = new List<VirtualFriendList>();

            if (!virtualFriendSolrListHash.ContainsKey(userEmail))
                virtualFriendSolrListHash[userEmail] = new List<UnVirtualFriendSolr>();

            if (!uniqueFriendListHash.ContainsKey(userEmail))
                uniqueFriendListHash[userEmail] = new List<String>();

                var googleContacts =
                    JsonConvert.DeserializeObject<List<GoogleApiContactListResponse>>(googleApiContactListString);

                foreach (var googleContact in googleContacts)
                {

                    if (googleContact.gdemail != null)
                    {
                        foreach (var id2email in googleContact.gdemail)
                        {
                            if (uniqueFriendListHash[userEmail].Contains(id2email.address))
                                continue;

                            uniqueFriendListHash[userEmail].Add(id2email.address);

                            var virtualFriend = new VirtualFriendList();
                            virtualFriend.id1 = userEmail;
                            if (id2email.address == null)
                                continue;

                            virtualFriend.id2 = id2email.address;
                            virtualFriend.isSolrUpdated = false;
                            virtualFriend.name1 = null;
                            virtualFriend.name2 = googleContact.title.ContainsKey("$t")
                                ? googleContact.title["$t"]
                                : null;
                            virtualFriend.source = CommonConstants.google;
                            virtualFriend.type = CommonConstants.email;

                            virtualFriendListHash[userEmail].Add(virtualFriend);
                            virtualFriendSolrListHash[userEmail].Add(
                                new UnVirtualFriendSolr().ConvertVirtualFriendForSolr(virtualFriend, false));
                        }

                    }
                    else
                    {
                        if (googleContact.gdphoneNumber != null)
                        {
                            foreach (var id2phoneNumber in googleContact.gdphoneNumber)
                            {

                                if (uniqueFriendListHash[userEmail].Contains(id2phoneNumber.uri))
                                    continue;

                                uniqueFriendListHash[userEmail].Add(id2phoneNumber.uri);

                                var virtualFriend = new VirtualFriendList();
                                virtualFriend.id1 = userEmail;
                                if (id2phoneNumber.uri == null)
                                    continue;

                                virtualFriend.id2 = id2phoneNumber.uri;
                                virtualFriend.isSolrUpdated = false;
                                virtualFriend.name1 = null;
                                virtualFriend.name2 = googleContact.title.ContainsKey("$t")
                                    ? googleContact.title["$t"]
                                    : null;
                                virtualFriend.source = CommonConstants.google;
                                virtualFriend.type = CommonConstants.phone;

                                virtualFriendListHash[userEmail].Add(virtualFriend);
                                virtualFriendSolrListHash[userEmail].Add(
                                new UnVirtualFriendSolr().ConvertVirtualFriendForSolr(virtualFriend, true));
                            }
                        }

                    }

                }


                try
                {
                    //int counter = 0;
                    //const int itemPerIteration = 150;
                    foreach (KeyValuePair<String, List<VirtualFriendList>> entry in virtualFriendListHash)
                    {
                        var virtualFriendListJson = new VirtualFriendListJson
                        {
                            user1 = entry.Key,
                            jsonData = JsonConvert.SerializeObject(entry.Value)
                        };

                        IDynamoDb dynamoDbModel = new DynamoDb();
                        dynamoDbModel.UpsertOrbitPageGoogleApiContacts(entry.Value, userEmail, startIndex);

                    }

                    foreach (KeyValuePair<String, List<UnVirtualFriendSolr>> entry in virtualFriendSolrListHash)
                    {
                        SaveVirtualFriendListToSolr(virtualFriendSolrListHash[entry.Key], entry.Key, solrUser);
                    }


                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                }


        }

        public void InsertIntoGoogleApiContactWithPagination(int startIndex, int perPage, int totalCount,
            String accessToken, String email, string accessKey, string secretKey)
        {
            if (startIndex > totalCount)
                return;

            String URI = "https://www.google.com/m8/feeds/contacts/default/full?alt=json&start-index=" + startIndex +
                         "&max-results=" + perPage;

            var client = new RestClient(URI);
            var request = new RestRequest("", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);
            var content = response.Content; // raw content as string

            content =
                content.Replace("openSearch$totalResults", "openSearchtotalResults")
                    .Replace("openSearch$itemsPerPage", "openSearchitemsPerPage")
                    .Replace("gd$email", "gdemail")
                    .Replace("gd$phoneNumber", "gdphoneNumber");
            try
            {
                var googleContacts = JsonConvert.DeserializeObject<GooglePlusContactListModel>(content);
                if (googleContacts.feed.entry == null)
                    return;

                SaveGoogleApiContactToDynamoDb(email, JsonConvert.SerializeObject(googleContacts.feed.entry), startIndex.ToString(CultureInfo.InvariantCulture), accessKey, secretKey);

                try
                {                    
                    startIndex = startIndex + perPage;
                    if (startIndex <= totalCount)
                    {
                        InsertIntoGoogleApiContactWithPagination(startIndex, perPage, totalCount, accessToken, email,accessKey,secretKey);
                    }
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error while saving google contacts : " + ex, ex);
            }
        }

        private String SaveVirtualFriendListToSolr(List<UnVirtualFriendSolr> userFriendList, String email, UnUserSolr solrUser)
        {
            if (email != null && email != "")
            {
                ISolrVirtualFriends solrVirtualFriends = new SolrVirtualFriends();
                solrVirtualFriends.InsertVirtualFriendListToSolr(userFriendList, false);                
            }

            return CommonConstants.SUCCESS_MSG;
        }
    }
}