using System;
using System.Globalization;
using System.Configuration;

namespace urNotice.Common.Infrastructure.Common.Constants
{
    public static class ServerConfig
    {
        public static bool IsDbFromAwsRds = Convert.ToBoolean(ConfigurationManager.AppSettings["CachingEnabled"] as string);
        public static string ContactUsReceivingEmailIds = ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"]
            .ToString(
                CultureInfo.InvariantCulture);

        public static string linkedinAppID = ConfigurationManager.AppSettings["linkedinAppID"].ToString(CultureInfo.InvariantCulture);
        public static string linkedinAppSecret = ConfigurationManager.AppSettings["linkedinAppSecret"].ToString(CultureInfo.InvariantCulture);

        public static string FacebookAppID = ConfigurationManager.AppSettings["FacebookAppID"].ToString(CultureInfo.InvariantCulture);
        public static string FacebookAppSecret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString(CultureInfo.InvariantCulture);

        public static string FacebookAppIDCautom = ConfigurationManager.AppSettings["FacebookAppIDCautom"].ToString(CultureInfo.InvariantCulture);
        public static string FacebookAppSecretCautom = ConfigurationManager.AppSettings["FacebookAppSecretCautom"].ToString(CultureInfo.InvariantCulture);

        public static string googleAppID = ConfigurationManager.AppSettings["googleAppID"].ToString(CultureInfo.InvariantCulture);
        public static string googleAppSecret = ConfigurationManager.AppSettings["googleAppSecret"].ToString(CultureInfo.InvariantCulture);

        public static string googleAppIDCautom = ConfigurationManager.AppSettings["googleAppIDCautom"].ToString(CultureInfo.InvariantCulture);
        public static string googleAppSecretCautom = ConfigurationManager.AppSettings["googleAppSecretCautom"].ToString(CultureInfo.InvariantCulture);

    }
}
