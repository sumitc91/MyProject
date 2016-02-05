using System;

namespace urNoticeAnalytics.commonMethods
{
    public class DateTimeUtil
    {
        public static DateTime GetUtcTime()
        {            
            return DateTime.Now.ToUniversalTime();
        }
    }
}
