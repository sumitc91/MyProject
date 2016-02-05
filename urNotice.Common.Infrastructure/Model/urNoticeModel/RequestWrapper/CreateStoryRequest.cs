using System;
using System.Collections.Generic;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Vo;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class CreateStoryRequest
    {
        public CreateStoryDetailVo Data { get; set; }
        public List<imgurUploadImageResponse> ImgurList { get; set; }

        public List<GoogleApiLocationSubListResponse> location { get; set; }

        public String formatted_address { get; set; }
    }
}
