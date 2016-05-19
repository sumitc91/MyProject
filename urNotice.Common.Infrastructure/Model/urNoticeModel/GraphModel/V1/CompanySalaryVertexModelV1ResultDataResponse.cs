using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class CompanySalaryVertexModelV1ResultDataResponse
    {
        public VertexModelV1 designationInfo { get; set; }
        public EdgeModelV1 salaryInfo { get; set; }
    }
}
