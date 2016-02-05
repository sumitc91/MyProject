using System.Collections.Generic;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GoogleApiResponse
{
    public class GoogleApiContactListResponse
    {
        public Dictionary<string, string> title { get; set; }
        public List<GoogleApiContactListGdemailResponse> gdemail { get; set; }

        public List<GoogleApiContactListGdphoneNumberResponse> gdphoneNumber { get; set; }
    }
}
