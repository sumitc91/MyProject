using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class ModelAdapterUtil
    {
        public static CompanyNoticePeriodVertexModelResponse GetCompanyNoticePeriodInfoResponse(CompanyNoticePeriodVertexModelV1Response companyNoticePeriodVertexModelV1Response)
        {
            var companyNoticePeriodVertexModelResponse = new CompanyNoticePeriodVertexModelResponse();
            companyNoticePeriodVertexModelResponse.success = false;
            companyNoticePeriodVertexModelResponse.results = new List<CompanyNoticePeriodVertexModel>();

            if (companyNoticePeriodVertexModelV1Response == null || 
                companyNoticePeriodVertexModelV1Response.result == null || 
                companyNoticePeriodVertexModelV1Response.result.data == null) 
                    return companyNoticePeriodVertexModelResponse;
            
            companyNoticePeriodVertexModelResponse.success = true;
            foreach (var companyNoticePeriodInfo in companyNoticePeriodVertexModelV1Response.result.data)
            {
                var companyNoticePeriodVertexModel = new CompanyNoticePeriodVertexModel();
                companyNoticePeriodVertexModel.designationInfo = ParseDesignationInfo(companyNoticePeriodInfo.designationInfo);
                companyNoticePeriodVertexModel.range = ParseNoticePeriodRangeInfo(companyNoticePeriodInfo.range);

                companyNoticePeriodVertexModelResponse.results.Add(companyNoticePeriodVertexModel);
            }
            return companyNoticePeriodVertexModelResponse;
        }

        public static CompanySalaryVertexModelResponse GetCompanySalaryInfoResponse(CompanySalaryVertexModelV1Response companySalaryVertexModelV1Response)
        {
            var companySalaryVertexModelResponse = new CompanySalaryVertexModelResponse();
            companySalaryVertexModelResponse.success = false;
            companySalaryVertexModelResponse.results = new List<CompanySalaryVertexModel>();

            if (companySalaryVertexModelV1Response == null ||
                companySalaryVertexModelV1Response.result == null ||
                companySalaryVertexModelV1Response.result.data == null)
                return companySalaryVertexModelResponse;

            companySalaryVertexModelResponse.success = true;
            foreach (var companySalaryInfo in companySalaryVertexModelV1Response.result.data)
            {
                var companySalaryVertexModel = new CompanySalaryVertexModel();
                companySalaryVertexModel.designationInfo = ParseDesignationInfo(companySalaryInfo.designationInfo);
                companySalaryVertexModel.salaryInfo = ParseSalaryInfo(companySalaryInfo.salaryInfo);

                companySalaryVertexModelResponse.results.Add(companySalaryVertexModel);
            }

            return companySalaryVertexModelResponse;
        }

        private static List<CompanySalaryInfoVertexModel> ParseSalaryInfo(EdgeModelV1 salaryInfo)
        {
            var companySalaryInfoVertexModelList = new List<CompanySalaryInfoVertexModel>();
            var companySalaryInfoVertexModel = new CompanySalaryInfoVertexModel();
            companySalaryInfoVertexModel._id = salaryInfo.id;
            companySalaryInfoVertexModel._inV = salaryInfo.inV.ToString(CultureInfo.InvariantCulture);
            companySalaryInfoVertexModel._outV = salaryInfo.outV.ToString(CultureInfo.InvariantCulture);

            if (salaryInfo.properties.ContainsKey(EdgePropertyEnum.PostedDate.ToString()))
            {
                companySalaryInfoVertexModel.PostedDate = salaryInfo.properties[EdgePropertyEnum.PostedDate.ToString()];
            }

            if (salaryInfo.properties.ContainsKey(EdgePropertyEnum.SalaryAmount.ToString()))
            {
                companySalaryInfoVertexModel.SalaryAmount = Convert.ToInt32(salaryInfo.properties[EdgePropertyEnum.SalaryAmount.ToString()]);
            }

            companySalaryInfoVertexModelList.Add(companySalaryInfoVertexModel);
            return companySalaryInfoVertexModelList;
        }

        private static List<CompanyNoticePeriodInfoVertexModel> ParseNoticePeriodRangeInfo(EdgeModelV1 range)
        {
            var companyNoticePeriodInfoVertexModelList = new List<CompanyNoticePeriodInfoVertexModel>();
            var companyNoticePeriodInfoVertexModel = new CompanyNoticePeriodInfoVertexModel();
            companyNoticePeriodInfoVertexModel._id = range.id;
            companyNoticePeriodInfoVertexModel._inV = range.inV.ToString(CultureInfo.InvariantCulture);
            companyNoticePeriodInfoVertexModel._outV = range.outV.ToString(CultureInfo.InvariantCulture);

            if (range.properties.ContainsKey(EdgePropertyEnum.PostedDate.ToString()))
            {                
                companyNoticePeriodInfoVertexModel.PostedDate = range.properties[EdgePropertyEnum.PostedDate.ToString()];
            }

            if (range.properties.ContainsKey(EdgePropertyEnum.RangeValue.ToString()))
            {
                companyNoticePeriodInfoVertexModel.RangeValue = range.properties[EdgePropertyEnum.RangeValue.ToString()];
            }

            companyNoticePeriodInfoVertexModelList.Add(companyNoticePeriodInfoVertexModel);
            return companyNoticePeriodInfoVertexModelList;
        }

        private static List<CompanyDesignationInfoVertexModel> ParseDesignationInfo(VertexModelV1 designationInfo)
        {
            var companyDesignationInfoVertexModelList = new List<CompanyDesignationInfoVertexModel>();
            var companyDesignationInfoVertexModel = new CompanyDesignationInfoVertexModel();
            companyDesignationInfoVertexModel._id = designationInfo.id.ToString(CultureInfo.InvariantCulture);
            if (designationInfo.properties.ContainsKey(VertexPropertyEnum.CreatedTime.ToString()))
            {
                companyDesignationInfoVertexModel.CreatedTime = designationInfo.properties[VertexPropertyEnum.CreatedTime.ToString()][0].value;
            }
            if (designationInfo.properties.ContainsKey(VertexPropertyEnum.DesignationName.ToString()))
            {
                companyDesignationInfoVertexModel.DesignationName = designationInfo.properties[VertexPropertyEnum.DesignationName.ToString()][0].value;
            }
            companyDesignationInfoVertexModelList.Add(companyDesignationInfoVertexModel);
            return companyDesignationInfoVertexModelList;
        }


        
    }
}
