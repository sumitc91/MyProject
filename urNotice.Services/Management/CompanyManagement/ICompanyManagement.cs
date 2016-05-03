using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Management.CompanyManagement
{
    public interface ICompanyManagement
    {
        SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId, string cid);

        Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation,string salary,string jobFromYear, string jobToYear, string companyVertexId);
        bool CreateNewDesignation(string designationName, string createdBy);

        bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary,string createdBy);

        bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName,string noticePeriodRange,string createdBy);
        bool CreateNewCompany(OrbitPageCompany company, string createdBy);
    }
}
