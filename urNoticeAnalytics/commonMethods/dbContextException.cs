using System;
using System.Data.Entity.Validation;
using System.Reflection;
using urNoticeAnalytics.Common.Logger;

namespace urNoticeAnalytics.commonMethods
{    
    public class DbContextException
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        public static void LogDbContextException(DbEntityValidationException e)
        {
            foreach (var eve in e.EntityValidationErrors)
            {
                Logger.Error("Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:",e);
                foreach (var ve in eve.ValidationErrors)
                {
                    Logger.Error("- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"",e);
                }
            }
        }
    }
}