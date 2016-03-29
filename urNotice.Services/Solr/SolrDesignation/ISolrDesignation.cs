using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using urNotice.Common.Infrastructure.Model.SourceDB;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrDesignation
{
    public interface ISolrDesignation : ISourceDesignationDb
    {
        SolrQueryResults<UnDesignationSolr> GetDesignationDetails(String queryText);
        SolrQueryResults<UnDesignationSolr> GetAbsoluteDesignationDetail(string queryText);
    }
}
