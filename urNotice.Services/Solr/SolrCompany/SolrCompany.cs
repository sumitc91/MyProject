using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Services.Solr.SolrCompany
{
    public class SolrCompany : ISolrCompany
    {
        public Dictionary<string, string> InsertNewCompany(OrbitPageCompany company, bool toBeOptimized)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnCompanySolr>>();


            var companyToBeMovedToSolr = new UnCompanySolr
            {
                averagerating = company.averageRating,
                avghikeperct = company.avgHikePercentage,
                avgnoticeperiod = company.avgNoticePeriod,
                buyoutpercentage = company.buyoutPercentage,
                city = company.city,
                companyid = company.vertexId,
                companyname = company.CompanyName,
                displayname = company.DisplayName,
                country = company.country,
                description = company.description,
                district = company.district,
                formatted_address = company.formatted_address,
                geo = company.latitude + "," + company.longitude,
                guid = company.vertexId,
                id = company.vertexId,
                isprimary = true,
                latitude = company.latitude,
                longitude = company.longitude,
                logourl = company.logoUrl,
                maxnoticeperiod = company.maxNoticePeriod,
                minnoticeperiod = company.minNoticePeriod,
                perclookingforchange = company.percLookingForChange,
                postal_code = company.postal_code,
                rating = company.averageRating,
                size = company.size ?? 0,
                speciality = company.specialities != null ? company.specialities.Split(',').Select(s => s.Trim())
                               .Where(s => s != String.Empty)
                               .ToArray() : null,
                squarelogourl = company.squareLogoUrl,
                state = company.state,
                sublocality = company.sublocality,
                telephone = company.telephone != null ? company.telephone.Split(',').Select(s => s.Trim())
                               .Where(s => s != String.Empty)
                               .ToArray() : null,
                totalratingcount = company.totalNumberOfRatings,
                totalreviews = company.totalReviews,
                website = company.website,
                workLifeBalanceRating = company.workLifeBalanceRating,
                salaryRating = company.salaryRating,
                companyCultureRating = company.companyCultureRating,
                careerGrowthRating = company.careerGrowthRating,
                founded = company.founded,
                founder = company.founder.Trim(),
                turnover = company.turnover.Trim(),
                headquarter = company.headquarter.Trim(),
                employees = company.employees.Trim(),
                competitors = company.competitors != null ? company.competitors.Split(',').Select(s => s.Trim())
                               .Where(s => s != String.Empty)
                               .ToArray() : null,

            };

            solr.Add(companyToBeMovedToSolr);
            solr.Commit();
            if (toBeOptimized)
                solr.Optimize();

            var response = new Dictionary<String, String>();
            response["status"] = "200";

            return response;
        }

        public SolrQueryResults<UnCompanySolr> GetCompanyDetailsAutocomplete(string queryText)
        {
            queryText = queryText.Replace(" ", "*");
            //queryText = queryText.ToLower();
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
            var solrQuery = new SolrQuery("companyname:*" + queryText + "*");
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 15,
                Start = 0,
                Fields = new[] { "guid", "companyname", "companyid", "isprimary", "squarelogourl", "logourl" }
            });
            return solrQueryExecute;
        }

        public SolrQueryResults<UnCompanySolr> CompanyDetailsById(string cid)
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
            var solrQuery = new SolrQuery("guid:" + cid);
            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 10,
                Start = 0,
                Fields = new[] { "guid","companyid","companyname","rating","website","size","description","averagerating","totalratingcount","totalreviews","isprimary",
                        "logourl", "squarelogourl", "speciality", "telephone","avgnoticeperiod","buyoutpercentage","maxnoticeperiod","minnoticeperiod","avghikeperct","perclookingforchange",
                        "sublocality","city","district","state","country","postal_code","latitude","longitude","geo" }
            });
            return solrQueryExecute;
        }

        public SolrQueryResults<UnCompanySolr> Search(string q, string page, string perpage, string totalMatch)
        {
            q = string.IsNullOrEmpty(q) ? "*" : q.Replace(" ", "*");
            page = string.IsNullOrEmpty(page) ? "0" : (Convert.ToInt32(page) - 1).ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(perpage))
                perpage = "10";

            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
            var solrQuery = new SolrQuery("companyname:*" + q + "*");
            if (string.IsNullOrEmpty(totalMatch))
                totalMatch = solr.Query(solrQuery).Count().ToString();

            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = Convert.ToInt32(perpage),
                Start = Convert.ToInt32(page),
                Fields = new[] { "guid","companyid","companyname","rating","website","size","description","averagerating","totalratingcount","totalreviews","isprimary",
                        "logourl", "squarelogourl", "speciality", "telephone","avgnoticeperiod","buyoutpercentage","maxnoticeperiod","minnoticeperiod","avghikeperct","perclookingforchange",
                        "sublocality","city","district","state","country","postal_code","latitude","longitude","geo" }
            });

            return solrQueryExecute;
        }

        public SolrQueryResults<UnCompanySolr> GetCompanyCompetitorsDetail(string size, string rating, string speciality)
        {            
            if (speciality != null)
                speciality = speciality.Replace("(", "*").Replace(")", "*").Replace(" ", "*").Replace(":", "");

            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
            String query = CompanyCompetitorQueryBuilder(size, rating, speciality);
            var solrQuery = new SolrQuery(query);

            var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
            {
                Rows = 10,
                Start = 0,
                Fields = new[] { "guid", "rating","website","size", "companyname","isprimary",
                        "squarelogourl","logourl"}
            });

            if (solrQueryExecute.Count < 2)
            {
                query = CompanyCompetitorQueryBuilder(null, rating, speciality);
                solrQuery = new SolrQuery(query);
                solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 10,
                    Start = 0,
                    Fields = new[] { "guid", "rating","website","size", "companyname","isprimary",
                        "squarelogourl","logourl"}
                });
            }
            return solrQueryExecute;
        }

        private String CompanyCompetitorQueryBuilder(String size, String rating, String speciality)
        {
            StringBuilder query = new StringBuilder();

            query.Append("(");

            if (size != null)
            {
                query.Append("(");
                query.Append("size:" + size);
                query.Append(")");
            }



            if (speciality != null && speciality != " " && speciality != "")
            {
                if (size != null)
                    query.Append(" AND ");

                query.Append("(");
                foreach (var companySpeciality in speciality.Split(','))
                {
                    var companySpecialityLocal = companySpeciality.Replace(" ", "*");
                    query.Append("(");
                    query.Append("speciality:" + companySpecialityLocal.Trim());
                    query.Append(")");

                    query.Append(" OR ");

                    query.Append("(");
                    query.Append("speciality: " + companySpecialityLocal.Trim());
                    query.Append(")");

                    query.Append(" OR ");
                }

                //queryString = query.ToString();
                query.Remove(query.Length - 4, 4);

                query.Append(")");
            }
            query.Append(")");

            return query.ToString();
        }
    }
}
