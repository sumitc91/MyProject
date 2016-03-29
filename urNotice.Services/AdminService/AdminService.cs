using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrCompany;
using urNotice.Services.Solr.SolrDesignation;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.AdminService
{
    public class AdminService
    {

        public bool CreateNewDesignation(string designationName, string createdBy, string accessKey, string secretKey)
        {
            IGraphDbContract graphDbContract = new GraphDbContract();
            var response = graphDbContract.InsertNewDesignationInGraphDb(createdBy, designationName);
            
            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageDesignation(designationName, response[TitanGraphConstants.Id]);
            
            ISolrDesignation solrDesignationModel = new SolrDesignation();
            solrDesignationModel.AddDesignation(response[TitanGraphConstants.Id], designationName, false);
            
            return true;
        }

        public bool CreateNewCompanyDesignationSalary(string companyName,string designationName,string salary, string createdBy, string accessKey, string secretKey)
        {
            //IGraphDbContract graphDbContract = new GraphDbContract();
            //var response = graphDbContract.InsertNewDesignationInGraphDb(createdBy, designationName);

            //IDynamoDb dynamoDbModel = new DynamoDb();
            //dynamoDbModel.UpsertOrbitPageDesignation(designationName, response[TitanGraphConstants.Id]);

            ISolrDesignation solrDesignationModel = new SolrDesignation();
            ISolrCompany solrCompanyModel = new SolrCompany();
            var designationDetails = solrDesignationModel.GetAbsoluteDesignationDetail(designationName);
            if (designationDetails != null && designationDetails.Count>0)
            {
                var companyDetails = solrCompanyModel.GetAbsoluteCompanyDetailsAutocomplete(companyName);
                if (companyDetails != null && companyDetails.Count>0)
                {                   
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    properties = new Dictionary<string, string>();
                    properties[EdgePropertyEnum._outV.ToString()] = companyDetails[0].guid;
                    properties[EdgePropertyEnum._inV.ToString()] = designationDetails[0].vertexId;
                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Salary.ToString();
                    properties[EdgePropertyEnum.SalaryAmount.ToString()] = "(i,"+Convert.ToString(Convert.ToDouble(salary) * 100000)+")";
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(createdBy, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
            
                }
            }
            //solrDesignationModel.AddDesignation(response[TitanGraphConstants.Id], designationName, false);

            return true;
        }

        public bool CreateNewCompany(OrbitPageCompany company, string createdBy, string accessKey, string secretKey)
        {
            IGraphDbContract graphDbContract = new GraphDbContract();

            var response = graphDbContract.InsertNewCompanyInGraphDb(createdBy,company.CompanyName);

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageCompany(company, response[TitanGraphConstants.Id]);
            
            company.vertexId = response[TitanGraphConstants.Id];
            ISolrCompany solrCompanyModel = new SolrCompany();
            solrCompanyModel.InsertNewCompany(company,false);
            return true;
        }
    }

}
