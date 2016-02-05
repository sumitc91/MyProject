﻿using System;

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
    }
}
