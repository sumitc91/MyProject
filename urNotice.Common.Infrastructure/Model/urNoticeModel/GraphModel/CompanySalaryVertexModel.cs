using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class CompanySalaryVertexModel
    {
        public List<CompanySalaryInfoVertexModel> salaryInfo{get;set;}
        public List<CompanyDesignationInfoVertexModel> designationInfo{get;set;}
    }
}
