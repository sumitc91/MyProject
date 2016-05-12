using System;

namespace urNotice.Common.Infrastructure.Common.Constants
{
    public static class CommonConstants
    {
        public const int SERVER_ERROR_CODE = 500;
        public const String SERVER_ERROR_MSG = "Internal Server Error Occured !!!";

        public const int SUCCESS_CODE = 200;
        public const String SUCCESS_MSG = "Success !!!";

        public const String TRUE = "true";
        public const String FALSE = "false";
        public const String NA = "NA";
        public const String AnonymousUserVertex = "5120256";

        public const string clientImageUrl = "http://i.imgur.com/zdfwnCAm.jpg";

        public static string currency_INR = "INR";

        public static string userType_user = "user";


        public const string CSSImage_info = "ion ion-ios7-people info";
        public const string CSSImage_danger = "fa fa-warning danger";
        public const string CSSImage_warning = "fa fa-users warning";
        public const string CSSImage_success = "ion ion-ios7-cart success";

        public static string reputationDeducted = "5";

        public static double newAccountCreationReferralBalanceAmount = 5;

        public static string payment_credit = "10";
        public static string status_open = "open";
        public static string status_true = "true";
        public static string logourl = "tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png";
        public static string NONE="none";
        public static string CompanySquareLogoNotAvailableImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/images/companyRectangleImageNotAvailable.png";

        public static string FemaleProfessionalAvatar = "https://s3-ap-southeast-1.amazonaws.com/urnotice/users/female_professional.png";
        public static string MaleProfessionalAvatar = "https://s3-ap-southeast-1.amazonaws.com/urnotice/users/male_professional.png";
        public static string google="google";
        public static string email="email";
        public static string phone="phone";
        public static string male="male";
        public static string female="female";
        public static string undefined = "undefined";
        public static string PushNotificationArray = "PushNotificationArray";
        public static string CommaDelimeter = ",";
        public static string Facebook = "facebook";
        public static string WorkgraphyIconImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/User/Sumit/WallPost/3033020a-4a44-4d31-aadc-a7ec8928e94f.jpg";
        
        public static string AssociateUsers = "1";
        public static string AssociateCompany = "2";
        public static string vertex = "vertex";

        public const string AssociateRequest = "1";
        public const string AssociateFollow = "2";
        public const string AssociateAccept = "3";
        public const string AssociateReject = "4";
        public const string RemoveFollow = "5";
        public const string Deassociate = "6";
        public const string AssociateRequestCancel = "7";

        public const string All = "All";
        public const string Company = "Company";
        public const string Users = "Users";
        public const string Workgraphy = "Workgraphy";        
    }
}
