﻿using System;

namespace urNoticeAnalytics.Common.Logger
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message, Exception ex);
        void Debug(string message, Exception ex);
        void Fatal(string message, Exception ex);
    }
}
