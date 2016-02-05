using System;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class UpdateCompanyRequest
    {
        public String Id { get; set; }
        public String guid  { get; set; }
        public String companyid  { get; set; }
        public String companyname  { get; set; }
        public String rating { get; set; }
        public String website { get; set; }
        public String size { get; set; }
        public String description { get; set; }
        public String averagerating { get; set; }
        public String totalratingcount { get; set; }
        public String totalreviews { get; set; }
        public String isprimary { get; set; }
        public String logourl { get; set; }
        public String squarelogourl { get; set; }
        public String[] speciality { get; set; }     
        public String[] telephone { get; set; }
        public String avgnoticeperiod{ get; set; }
        public String buyoutpercentage{ get; set; }
        public String maxnoticeperiod{ get; set; }
        public String minnoticeperiod{ get; set; }
        public String avghikeperct{ get; set; }
        public String perclookingforchange{ get; set; }
        public String sublocality{ get; set; }
        public String city{ get; set; }
        public String district{ get; set; }
        public String formatted_address{ get; set; }
        public String state{ get; set; }
        public String country{ get; set; }
        public String postal_code{ get; set; }
        public String latitude{ get; set; }
        public String longitude{ get; set; }
        public String geo { get; set; }
    }
}
