using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.NoticePeriod
{
    public class NoticePeriodUserInputReviewRequest
    {
        public string employerStatus { get; set; }
        public string employerName { get; set; }
        public string expectedCtc { get; set; }
        public string employerVertexId { get; set; }
        public string lastYearAtEmployer { get; set; }
        public string employmentStatusSelect { get; set; }
        public string reviewTitle { get; set; }
        public string reviewDescription { get; set; }
        public string lookingForChange { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string salaryFrequency { get; set; }
        public string suggestionToBoss { get; set; }
        public string suggestionToCompany { get; set; }
        public string employerLogoImage { get; set; }
    }
}
