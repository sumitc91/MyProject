using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using urNotice.Services.Solr.SolrUser;
using urNoticeAnalytics.Common.Constants;
using urNoticeAnalytics.Common.Logger;
using urNoticeAnalytics.commonMethods;
using urNoticeAnalytics.Models.GoogleApiResponse;
using urNoticeAnalytics.Models.urNoticeAnalyticContext;

namespace urNoticeAnalytics.Services
{
    public class SyncService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        private readonly urnoticeAnalyticsEntities _dbAnalytics = new urnoticeAnalyticsEntities();

        public String SyncGoogleApiContactList(string id, System.Web.HttpRequestBase Request)
        {
            var googleApiContactList =
                _dbAnalytics.GoogleApiContacts.Where(x => x.isVirtualFriendListUpdated == false).Take(150).ToList();

            var virtualFriendListHash = new Dictionary<string, List<VirtualFriendList>>();
            var uniqueFriendListHash = new Dictionary<string, List<string>>();

                        
            foreach (var googleApiContact in googleApiContactList)
            {

                if (!virtualFriendListHash.ContainsKey(googleApiContact.emailId))
                    virtualFriendListHash[googleApiContact.emailId] = new List<VirtualFriendList>();

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
                            }
                        }
                        
                    }

                }

                
            }

           
            try
            {

                foreach (KeyValuePair<String, List<VirtualFriendList>> entry in virtualFriendListHash)
                {
                    // do something with entry.Value or entry.Key
                    _dbAnalytics.VirtualFriendLists.AddRange(entry.Value);
                    _dbAnalytics.SaveChanges();
                    SaveToSolr(uniqueFriendListHash[entry.Key], entry.Key);
                }

                var status = new Dictionary<string, string>();
                int[] googleApiContactIdList = googleApiContactList.Select(x => x.Id).ToArray();

                var googleApiContacts = _dbAnalytics.GoogleApiContacts.Where(f => googleApiContactIdList.Contains(f.Id)).ToList();
                googleApiContacts.ForEach(a => a.isVirtualFriendListUpdated = true);
                
            }
            catch (DbEntityValidationException e)
            {
                DbContextException.LogDbContextException(e);
            }

            return null;
        }

        private String SaveToSolr(List<String> userFriendList, String email)
        {
            if (email != null && email != "")
            {
                ISolrUser solrUserModel = new SolrUser();

                var solrUser = solrUserModel.GetPersonData(email, null, null,null,true);//new SolrService().GetSolrUserFullData(email, null, null);
                solrUser.Virtualfriend = userFriendList.ToArray();
                //new SolrService().InsertNewUserToSolr(solrUser, false);                
                solrUserModel.InsertNewUserToSolr(solrUser,false);
            }
            
            return CommonConstants.SUCCESS_MSG;
        }
    }
    
}
