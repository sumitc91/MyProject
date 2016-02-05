using System;
using System.Configuration;
using System.Globalization;

namespace urNoticeAnalytics.Common.Constants
{
    public class ServerConfig
    {
        public static bool IsDbFromAwsRds = Convert.ToBoolean(ConfigurationManager.AppSettings["googleAppIDCautom"].ToString(CultureInfo.InvariantCulture));
    }
}
