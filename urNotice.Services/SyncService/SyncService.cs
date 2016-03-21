using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GoogleApiResponse;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Services.ErrorLogger;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrVirtualFriends;

namespace urNotice.Services.SyncService
{
    public class SyncService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        
        private readonly urnoticeAnalyticsEntities _dbAnalytics = new urnoticeAnalyticsEntities();

        public String SyncGoogleApiContactList(String userEmail)
        {
            var googleApiContactList =
                _dbAnalytics.GoogleApiContacts.Where(x => x.isVirtualFriendListUpdated == false && x.emailId == userEmail).Take(150).ToList();

            var virtualFriendListHash = new Dictionary<string, List<VirtualFriendList>>();
            var virtualFriendSolrListHash = new Dictionary<string, List<UnVirtualFriendSolr>>();
            var uniqueFriendListHash = new Dictionary<string, List<string>>();

            ISolrUser solrUserModel = new SolrUser();
            var solrUser = solrUserModel.GetPersonData(userEmail, null, null,null,true);//new SolrService.SolrService().GetSolrUserFullData(userEmail, null, null);

            uniqueFriendListHash[userEmail]=solrUser.Virtualfriend.ToList();

            foreach (var googleApiContact in googleApiContactList)
            {

                if (!virtualFriendListHash.ContainsKey(googleApiContact.emailId))
                    virtualFriendListHash[googleApiContact.emailId] = new List<VirtualFriendList>();

                if (!virtualFriendSolrListHash.ContainsKey(googleApiContact.emailId))
                    virtualFriendSolrListHash[googleApiContact.emailId] = new List<UnVirtualFriendSolr>();

                if (!uniqueFriendListHash.ContainsKey(googleApiContact.emailId))
                    uniqueFriendListHash[googleApiContact.emailId] = new List<String>();

                var googleContacts =
                    JsonConvert.DeserializeObject<List<GoogleApiContactListResponse>>(googleApiContact.entryListString);
                
                foreach (var googleContact in googleContacts)
                {

                    if (googleContact.gdemail != null)
                    {
                        foreach (var id2email in googleContact.gdemail)
                        {
                            if (uniqueFriendListHash[googleApiContact.emailId].Contains(id2email.address))
                                continue;

                            uniqueFriendListHash[googleApiContact.emailId].Add(id2email.address);

                            var virtualFriend = new VirtualFriendList();
                            virtualFriend.id1 = googleApiContact.emailId;
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

                            virtualFriendListHash[googleApiContact.emailId].Add(virtualFriend);
                            virtualFriendSolrListHash[googleApiContact.emailId].Add(
                                new UnVirtualFriendSolr().ConvertVirtualFriendForSolr(virtualFriend,false));
                        }

                    }
                    else
                    {
                        if (googleContact.gdphoneNumber != null)
                        {
                            foreach (var id2phoneNumber in googleContact.gdphoneNumber)
                            {

                                if (uniqueFriendListHash[googleApiContact.emailId].Contains(id2phoneNumber.uri))
                                    continue;

                                uniqueFriendListHash[googleApiContact.emailId].Add(id2phoneNumber.uri);

                                var virtualFriend = new VirtualFriendList();
                                virtualFriend.id1 = googleApiContact.emailId;
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

                                virtualFriendListHash[googleApiContact.emailId].Add(virtualFriend);
                                virtualFriendSolrListHash[googleApiContact.emailId].Add(
                                new UnVirtualFriendSolr().ConvertVirtualFriendForSolr(virtualFriend,true));
                            }
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

                    _dbAnalytics.VirtualFriendListJsons.Add(virtualFriendListJson);
                    _dbAnalytics.SaveChanges();

                    //while (true)
                    //{
                    //    if (entry.Value.Count > counter)
                    //    {
                    //        // do something with entry.Value or entry.Key
                    //        _dbAnalytics.VirtualFriendLists.AddRange(entry.Value.Skip(counter).Take(itemPerIteration));
                    //        _dbAnalytics.SaveChanges();
                    //        counter += itemPerIteration;
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}
                    
                    
                }

                foreach (KeyValuePair<String, List<UnVirtualFriendSolr>> entry in virtualFriendSolrListHash)
                {                    
                    SaveVirtualFriendListToSolr(virtualFriendSolrListHash[entry.Key], entry.Key, solrUser);                 
                }

                var status = new Dictionary<string, string>();
                int[] googleApiContactIdList = googleApiContactList.Select(x => x.Id).ToArray();

                var googleApiContacts = _dbAnalytics.GoogleApiContacts.Where(f => googleApiContactIdList.Contains(f.Id)).ToList();
                googleApiContacts.ForEach(a => a.isVirtualFriendListUpdated = true);
                
            }
            catch (Exception e)
            {
                //DbContextException.LogDbContextException(e);
            }

            return null;
        }

        private String SaveVirtualFriendListToSolr(List<UnVirtualFriendSolr> userFriendList, String email, UnUserSolr solrUser)
        {
            if (email != null && email != "")
            {
                ISolrVirtualFriends solrVirtualFriends = new SolrVirtualFriends();
                solrVirtualFriends.InsertVirtualFriendListToSolr(userFriendList,false);                
            }

            return CommonConstants.SUCCESS_MSG;
        }

        //private String SaveToSolr(List<String> userFriendList, String email, UnUserSolr solrUser)
        //{
        //    if (email != null && email != "")
        //    {
               
        //        solrUser.Virtualfriend = userFriendList.ToArray();
        //        new SolrService.SolrService().InsertNewUserToSolr(solrUser, false);
        //    }
            
        //    return CommonConstants.SUCCESS_MSG;
        //}
    }
    
}
