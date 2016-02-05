using System;
using System.Collections.Generic;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.Vo
{
    public class NewCompanyRequestXlsFile
    {
        public CreateCompanyDetailVo companyDetails { get; set; }
        public List<imgurUploadImageResponse> ImgurList { get; set; }
        public List<GoogleApiLocationSubListResponse> location { get; set; }
        public String formatted_address { get; set; }

        public GeoLocationGoogleApi geolocation { get; set; }

        public RatingInformationXlsFile ratingInfo { get; set; }
    }
}
