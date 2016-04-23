using System;
using System.Collections.Generic;
using SolrNet;
using urNotice.Common.Infrastructure.Model.SourceDB;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrUser
{
    public interface ISolrUser : ISourceDb
    {                
        UnUserSolr GetPersonData(String email, String username, String phone, String fid,Boolean isFullDetails);
        Dictionary<String, String> InsertNewUserToSolr(UnUserSolr solrUser, Boolean toBeOptimized);
        SolrQueryResults<UnUserSolr> GetUserDetailsAutocomplete(String queryText);
        SearchAllResponseModel SearchAllAutocomplete (String queryText);
        SolrQueryResults<UnUserSolr> UserDetailsById(String uid);

    }
}
