using System;
using System.Collections.Generic;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class GeoCodeResponse
    {
        public List<GeoCodeResponseResult> results { get; set; }
        public String status { get; set; }
    }
}
