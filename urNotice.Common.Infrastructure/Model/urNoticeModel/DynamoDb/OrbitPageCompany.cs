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
        public string locationId { get; set; }
        public string website { get; set; }
        public Nullable<long> size { get; set; }
        public string description { get; set; }
        public System.DateTime lastUpdated { get; set; }
        public string lastUpdatedBy { get; set; }
        public string CompanyName { get; set; }
        public double averageRating { get; set; }
        public long totalNumberOfRatings { get; set; }
        public long totalReviews { get; set; }
        public bool isPrimary { get; set; }
        public string logoUrl { get; set; }
        public string squareLogoUrl { get; set; }
        public string specialities { get; set; }
        public string guid { get; set; }
        public double avgNoticePeriod { get; set; }
        public double buyoutPercentage { get; set; }
        public int maxNoticePeriod { get; set; }
        public int minNoticePeriod { get; set; }
        public double avgHikePercentage { get; set; }
        public double percLookingForChange { get; set; }
        public Nullable<bool> isSolrUpdated { get; set; }
        public string telephone { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}
