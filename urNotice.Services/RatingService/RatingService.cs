using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeRatingContext;

namespace urNotice.Services.RatingService
{
    public class RatingService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();
        private urnoticeRatingEntities _dbUrNoticeRating = null;

        public ResponseModel<UserCompanyAnalyticsResponseModel> GetUserRatingStatus(string accessKey, string secretKey, string userName, bool isLoggedInUser, string cid)
        {
            _dbUrNoticeRating = new urnoticeRatingEntities();
            var response = new ResponseModel<UserCompanyAnalyticsResponseModel>();
            response.Payload = new UserCompanyAnalyticsResponseModel();

            if (userName != null)
            {
                var userDynamoDbAnalytics = new DynamoDbService.DynamoDbService().IncrementUserCompanyAnalyticsCounter(accessKey, secretKey, userName, cid);
                if (userDynamoDbAnalytics != null)
                {
                    if (response.Payload != null)
                    {
                        response.Payload.UserCompanyAnalytics = new UserCompanyAnalyticsResponse();

                        response.Payload.UserCompanyAnalytics = userDynamoDbAnalytics;
                    }
                }     
            }
            
            return response;
        }
    }
}
