using System;
using System.Collections.Generic;

namespace urNotice.Services.SocialAuthService.google.Model
{
    public class GooglePlusContactListFeedModel
    {
        public List<GooglePlusContactListFeedEntryModel> entry { get; set; }
        public Dictionary<String, String> updated { get; set; }
        public Dictionary<String, String> openSearchtotalResults { get; set; }

        public Dictionary<String, String> openSearchitemsPerPage { get; set; }
    }
}
