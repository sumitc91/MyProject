using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.NoticePeriod
{
    public class NoticePeriodUserInputRequest
    {
        public NoticePeriodUserInputReviewRequest companyReview { get; set; }
        public List<GoogleApiLocationSubListResponse> location { get; set; }

        public String formatted_address { get; set; }
        public List<CompanyReviewInputPointsMapping> companyGoodPointList { get; set; }
        public List<CompanyReviewInputPointsMapping> companyBadPointList { get; set; }
        public List<NoticePeriodUserInputRatingListRequest> companyRatingList { get; set; }
        
    }
}
