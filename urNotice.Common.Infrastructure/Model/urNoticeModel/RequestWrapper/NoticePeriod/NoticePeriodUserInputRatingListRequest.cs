using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.NoticePeriod
{
    public class NoticePeriodUserInputRatingListRequest
    {
        public float rate { get; set; }
        //public int max { get; set; }
        //public bool isReadonly { get; set; }
        public string RatingText { get; set; }
        public string RatingID { get; set; }
        //public string overStar { get; set; }
        //public string percent { get; set; }

    }
}
