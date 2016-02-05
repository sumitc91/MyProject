using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNoticeAnalytics.Models.GoogleApiResponse
{
    public class GoogleApiContactListResponse
    {
        public Dictionary<string, string> title { get; set; }
        public List<GoogleApiContactListGdemailResponse> gdemail { get; set; }

        public List<GoogleApiContactListGdphoneNumberResponse> gdphoneNumber { get; set; }
    }
}
