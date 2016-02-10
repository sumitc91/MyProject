using System;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class DateTimeUtil
    {
        public static DateTime GetUtcTime()
        {            
            return DateTime.Now.ToUniversalTime();
        }

        public static String GetUtcTimeString()
        {
            return GetUtcTime().ToString("o");
        }

        public static long GetUtcTimeLong()
        {
            return GetUtcTime().Ticks;
        }
    }
}
