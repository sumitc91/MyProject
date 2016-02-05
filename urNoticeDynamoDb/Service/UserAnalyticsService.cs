using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNoticeDynamoDb.Service
{
    public class UserAnalyticsService
    {
        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.USWest2);

        public void MainMethod()
        {
            try
            {
                DynamoDBContext context = new DynamoDBContext(client);

                string userId = "sumitchourasia91@gmail.com";
                string companyId = "abcxyz";
                //CreateUserCompanyAnalyticsItem(context, userId, companyId);
                IncrementUserAnalyticsCounter(context, userId, companyId);
                //IncrementCounterUserAnalyticsItem(context);
                // Get item.
                //GetBook(context, 1);

                // Sample forum and thread to test queries.
                string forumName = "Amazon DynamoDB";
                string threadSubject = "DynamoDB Thread 1";
                // Sample queries.
                //FindRepliesInLast15Days(context, forumName, threadSubject);
                //FindRepliesPostedWithinTimePeriod(context, forumName, threadSubject);

                // Scan table.
                //FindProductsPricedLessThanZero(context);
                Console.WriteLine("To continue, press Enter");
                Console.ReadLine();
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static UserCompanyAnalytics CreateUserCompanyAnalyticsItem(DynamoDBContext context, string userId, string companyId)
        {
            Console.WriteLine("\n*** Executing CreateUserAnalyticsItem() ***");
            var userAnalytics = new UserCompanyAnalytics
            {
                UserId = userId,
                CompanyId =companyId,
                LastActivity = DateTimeUtil.GetUtcTime(),
                Count = 0                
            };

            context.Save(userAnalytics);
            return userAnalytics;
        }

        private static void CreateUserAnalyticsItem(DynamoDBContext context)
        {
            Console.WriteLine("\n*** Executing CreateUserAnalyticsItem() ***");
            var userAnalytics = new UserAnalytics
            {
                UserId = "sumitchourasia91@gmail.com",
                LoginDateTime = DateTimeUtil.GetUtcTime(),
                Count=0, 
                SystemDetails=""
            };

            context.Save(userAnalytics);
        }

        
        private static void IncrementUserAnalyticsCounter(DynamoDBContext context, string userId,string companyId)
        {
            var userCompanyAnalytics = context.Load<UserCompanyAnalytics>(userId,companyId);
            if (userCompanyAnalytics == null)
                userCompanyAnalytics = CreateUserCompanyAnalyticsItem(context, userId, companyId);
            userCompanyAnalytics.Count = userCompanyAnalytics.Count + 1;
            userCompanyAnalytics.LastActivity = DateTimeUtil.GetUtcTime();

            context.Save(userCompanyAnalytics);

        }
    }
}
