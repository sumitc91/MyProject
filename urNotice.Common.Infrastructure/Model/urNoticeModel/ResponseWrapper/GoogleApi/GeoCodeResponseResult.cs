using System;
using System.Collections.Generic;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper.GoogleApi
{
    public class GeoCodeResponseResult
    {
        //public GeoCodeResponseAddressComponents address_components { get; set; }
        public List<GoogleApiLocationSubListResponse> address_components { get; set; }
        public String formatted_address { get; set; }
        public GoogleApiLocationGeometry geometry { get; set; }
    }
}
