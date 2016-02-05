using System;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class DateTimeUtil
    {
        public static DateTime GetUtcTime()
        {            
            return DateTime.Now.ToUniversalTime();
        }
    }
}
