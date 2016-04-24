﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrWorkgraphy
{
    public class SolrWorkgraphy: ISolrWorkgraphy
    {
        public Dictionary<string, string> InsertNewWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy,string type, bool optimize)
        {
            var solrWorkgraphy = new UnWorkgraphySolr()
            {
                city = orbitPageWorkgraphy.city,
                company_name = orbitPageWorkgraphy.companyName,
                company_vertex_id = orbitPageWorkgraphy.companyVertexId,
                country = orbitPageWorkgraphy.country,
                created_by_email = orbitPageWorkgraphy.createdByEmail,
                created_by_vertex_id = orbitPageWorkgraphy.createdByVertexId,
                created_date = DateTimeUtil.GetUtcTimeString(),
                designation_name = orbitPageWorkgraphy.designation,
                designation_vertex_id = orbitPageWorkgraphy.designationVertexId,
                district = orbitPageWorkgraphy.district,
                formatted_address = orbitPageWorkgraphy.formatted_address,                
                heading = orbitPageWorkgraphy.heading,                
                id = orbitPageWorkgraphy.workgraphyVertexId,
                images = orbitPageWorkgraphy.ImagesUrl.ToArray(),
                is_anonymous = Convert.ToBoolean(orbitPageWorkgraphy.shareAnonymously),
                latitude = orbitPageWorkgraphy.latitude,
                longitude = orbitPageWorkgraphy.longitude,
                postal_code = orbitPageWorkgraphy.postal_code,
                short_desc = orbitPageWorkgraphy.subTitle,
                state = orbitPageWorkgraphy.state,
                story = orbitPageWorkgraphy.story,
                sublocality = orbitPageWorkgraphy.sublocality,
                vertex_id = orbitPageWorkgraphy.workgraphyVertexId,
                is_admin_verified=false,
                is_email_verified = false,
                type = type
            };

            if (orbitPageWorkgraphy.ImagesUrl != null && orbitPageWorkgraphy.ImagesUrl.Count > 0)
                solrWorkgraphy.icon_image = orbitPageWorkgraphy.ImagesUrl[0];
            else
                solrWorkgraphy.icon_image = CommonConstants.WorkgraphyIconImage;

            if (orbitPageWorkgraphy.latitude != null && orbitPageWorkgraphy.longitude != null)
                solrWorkgraphy.geo = orbitPageWorkgraphy.latitude + "," + orbitPageWorkgraphy.longitude;
            

            return InsertNewWorkgraphyToSolr(solrWorkgraphy,false);
        }

        public SolrQueryResults<UnWorkgraphySolr> GetLatestWorkgraphy(int page, int perPage)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnWorkgraphySolr>>();
            var solrQuery = new SolrQuery("(type:"+OrbitPageEnum.Workgraphy+")");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = perPage,
                Start = page,
                OrderBy = new[] { new SortOrder("created_date", Order.DESC)},
                Fields = new[] { "id", "vertex_id", "heading", "short_desc", "company_name", "company_vertex_id", "is_anonymous", "is_email_verified", "is_admin_verified", "created_by_email", "created_by_vertex_id", "icon_image", "created_date", "designation_name", "designation_vertex_id", "city", "sublocality", "state", "postal_code", "country", "district" },
            });

            return solrQueryExecute;
        }

        public SolrQueryResults<UnWorkgraphySolr> GetLatestBlogs(int page, int perPage)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnWorkgraphySolr>>();
            var solrQuery = new SolrQuery("(type:" + OrbitPageEnum.Blog + ")");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = perPage,
                Start = page,
                OrderBy = new[] { new SortOrder("created_date", Order.DESC) },
                Fields = new[] { "id", "vertex_id", "heading", "short_desc", "company_name", "company_vertex_id", "is_anonymous", "is_email_verified", "is_admin_verified", "created_by_email", "created_by_vertex_id", "icon_image", "created_date", "designation_name", "designation_vertex_id", "city", "sublocality", "state", "postal_code", "country", "district" },
            });

            return solrQueryExecute;
        }

        public SolrQueryResults<UnWorkgraphySolr> GetParticularWorkgraphyWithVertexId(int vertexId)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnWorkgraphySolr>>();
            var solrQuery = new SolrQuery("(vertex_id:" + vertexId+")");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 1,
                Start = 0,               
                Fields = new[] { "id", "vertex_id", "heading", "short_desc", "story", "company_name", "company_vertex_id", "is_anonymous", "is_email_verified", "is_admin_verified", "created_by_email", "created_by_vertex_id", "icon_image", "created_date", "designation_name", "designation_vertex_id", "city", "sublocality", "state", "postal_code", "country", "district" },
            });

            return solrQueryExecute;
        }

        public Dictionary<String, String> InsertNewWorkgraphyToSolr(UnWorkgraphySolr solrWorkgraphy, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnWorkgraphySolr>>();

            solr.Add(solrWorkgraphy);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }
    }
}
