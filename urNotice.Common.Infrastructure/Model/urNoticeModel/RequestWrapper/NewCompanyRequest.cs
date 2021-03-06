﻿using System;
using System.Collections.Generic;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Vo;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class NewCompanyRequest
    {

        public CreateCompanyDetailVo companyDetails { get; set; }
        public List<imgurUploadImageResponse> ImgurList { get; set; }
        public List<GoogleApiLocationSubListResponse> location { get; set; }
        public String formatted_address { get; set; }

        public GeoLocationGoogleApi geolocation { get; set; }
    }
}
