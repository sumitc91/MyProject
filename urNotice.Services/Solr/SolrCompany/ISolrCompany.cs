using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using urNotice.Common.Infrastructure.Model.SourceDB;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrCompany
{
    public interface ISolrCompany : ISourceCompanyDb
    {
        SolrQueryResults<UnCompanySolr> GetCompanyDetailsAutocomplete(string queryText);
        SolrQueryResults<UnCompanySolr> CompanyDetailsById(string cid);
        SolrQueryResults<UnCompanySolr> Search(string q, string page, string perpage, string totalMatch);
        SolrQueryResults<UnCompanySolr> GetCompanyCompetitorsDetail(string size, string rating, string speciality);
    }
}
