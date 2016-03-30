using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrDesignation
{
    public class SolrDesignation : ISolrDesignation
    {
        public Dictionary<string, string> AddDesignation(string vertexId, string designationName, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnDesignationSolr>>();
            var unDesignationSolrList = new List<UnDesignationSolr>();
            var designationObj = new UnDesignationSolr
            {
                id = vertexId,
                designation = designationName,
                vertexId = vertexId
            };

            unDesignationSolrList.Add(designationObj);
            solr.AddRange(unDesignationSolrList);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }

        public SolrQueryResults<UnDesignationSolr> GetDesignationDetails(string queryText)
        {
            queryText = queryText.Replace(" ", "?");            
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnDesignationSolr>>();
            var solrQuery = new SolrQuery("designation:" + queryText + "*");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 15,
                Start = 0,
                Fields = new[] { "designation", "id" }
            });

            if (solrQueryExecute == null || solrQueryExecute.Count == 0)
            {
                solrQuery = new SolrQuery("designation:*" + queryText + "*");
                solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 15,
                    Start = 0,
                    Fields = new[] { "designation", "id" }
                });
            }
            
            return solrQueryExecute;
        }

        public SolrQueryResults<UnDesignationSolr> GetAbsoluteDesignationDetail(string queryText)
        {
            queryText = queryText.Replace(" ", "?").Replace(")","?").Replace("(","?");
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnDesignationSolr>>();
            var solrQuery = new SolrQuery("designation:" + queryText + "");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 15,
                Start = 0,
                Fields = new[] { "designation", "id", "vertexId" }
            });
            return solrQueryExecute;
        }
    }
}
