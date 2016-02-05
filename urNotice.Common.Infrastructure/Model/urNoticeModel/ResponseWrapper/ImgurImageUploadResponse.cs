namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    
        public class imgurUploadImageResponse
        {
            public data data { get; set; }
        }
        public class data
        {
            public string id { get; set; }
            public string deletehash { get; set; }
            public string link { get; set; }
        }
    
}