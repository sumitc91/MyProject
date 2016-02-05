using System.Collections.Generic;
using Newtonsoft.Json;

namespace urNotice.Services.SocialAuthService.google.Model
{
    public class GooglePlusContactListFeedEntryModel
    {
        //public GooglePlusContactListFeedEntryTitleModel title { get; set; }

        [JsonProperty("title")]
        public Dictionary<string, string> title { get; set; }
        //public GooglePlusContactListFeedEntryGdEmailModel gdemail { get; set; }

        [JsonProperty("gdemail")]
        public List<Dictionary<string, string>> gdemail { get; set; }

        [JsonProperty("gdphoneNumber")]        
        public List<Dictionary<string, string>> gdphoneNumber { get; set; }
        //public GooglePlusContactListFeedEntryGdPhoneNumberModel gdphoneNumber { get; set; }
    }

    
}
