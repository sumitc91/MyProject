using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageCompany
    {
        public int Id { get; set; }
        public string companyId { get; set; }        
        public string website { get; set; }
        public Nullable<long> size { get; set; }
        public string description { get; set; }
        public System.DateTime lastUpdated { get; set; }
        public string lastUpdatedBy { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public float averageRating { get; set; }
        public long totalNumberOfRatings { get; set; }
        public long totalReviews { get; set; }
        public bool isPrimary { get; set; }
        public string logoUrl { get; set; }
        public string squareLogoUrl { get; set; }
        public string city { get; set; }
        public string sublocality { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public string district { get; set; }
        public string formatted_address { get; set; }
        public string vertexId { get; set; }
        public string specialities { get; set; }
        public string guid { get; set; }

        public string founded { get; set; }
        public string founder { get; set; }
        public string turnover { get; set; }
        
        public string headquarter { get; set; }
        public string employees { get; set; }        
        public string competitors { get; set; }
        public float avgNoticePeriod { get; set; }
        public float buyoutPercentage { get; set; }
        public int maxNoticePeriod { get; set; }
        public int minNoticePeriod { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float avgHikePercentage { get; set; }
        public float percLookingForChange { get; set; }
        public Nullable<bool> isSolrUpdated { get; set; }
        public string telephone { get; set; }
        public Nullable<bool> isDBSynced { get; set; }

        public float workLifeBalanceRating { get; set; }
        public float salaryRating { get; set; }
        public float companyCultureRating { get; set; }
        public float careerGrowthRating { get; set; }        
    }
}
