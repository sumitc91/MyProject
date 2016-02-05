using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageUser
    {
        public int Id { get; set; }
        public string username { get; set; }

        public string vertexId { get; set; }
        public string password { get; set; }
        public string isActive { get; set; }
        public string source { get; set; }
        public string guid { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string imageUrl { get; set; }
        public string gender { get; set; }
        public string locked { get; set; }
        public string keepMeSignedIn { get; set; }
        public string registrationTime { get; set; }
        public Nullable<System.DateTime> lastUpdatedDate { get; set; }
        public string fixedGuid { get; set; }
        public string isVerified { get; set; }
        public short priviledgeLevel { get; set; }
        public string timeZone { get; set; }
        public Nullable<long> phone { get; set; }
        public Nullable<bool> isPhoneActive { get; set; }
        public string userCoverPic { get; set; }
        public string locationId { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
        public string email { get; set; }
        public string fid { get; set; }

        public string lastContactUpdated { get; set; }
        public string validateUserKeyGuid { get; set; }

        public string forgetPasswordGuid { get; set; }

        public string facebookId { get; set; }
    }
}
