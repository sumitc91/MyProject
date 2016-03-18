using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.AdminService
{
    public class AdminService
    {

        public bool CreateNewDesignation(string designationName, string createdBy, string accessKey, string secretKey)
        {

            var response = new TitanService.TitanService().InsertNewDesignationToTitan(createdBy, designationName, false, accessKey, secretKey);

            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.Designation.ToString(),
                            designationName,
                            response[TitanGraphConstants.Id],
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            accessKey,
                            secretKey
                            );
            new SolrService.SolrService().AddDesignation(response[TitanGraphConstants.Id], designationName, false);
            return true;
        }

        public bool CreateNewCompany(OrbitPageCompany company, string createdBy, string accessKey, string secretKey)
        {

            var response = new TitanService.TitanService().InsertNewCompanyToTitan(createdBy, company.CompanyName, false, accessKey, secretKey);

            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.Company.ToString(),
                            company.CompanyName,
                            response[TitanGraphConstants.Id],
                            null,
                            null,
                            null,
                            null,
                            null,
                            company,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            accessKey,
                            secretKey
                            );

            company.vertexId = response[TitanGraphConstants.Id];
            new SolrService.SolrService().InsertNewCompanyToSolr(company, false);
            return true;
        }
    }

}
