using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Caching;
using SolrNet.Impl;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;

namespace urNotice.Common.Infrastructure.Session
{
    public class TokenManager
    {
                
        //private urnoticeAuthEntities _db = null;

        public static void CreateSession(urNoticeSession session)
        {
            var sessionId = session.SessionId;
            const int hours = 1; // TODO: currently hard coded hour value;
            //MemoryCache.Default.Set(sessionId, session, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(hours, 0, 0) });
            setMemoryCacheValue(sessionId, session, hours, 0, 0);
        }

        private static void setMemoryCacheValue(string SessionId, urNoticeSession session, int hours, int minutes, int seconds)
        {
            MemoryCache.Default.Set(SessionId, session, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(hours, 0, 0) });
        }
        public static void RemoveSession(string sessionId)
        {
            MemoryCache.Default.Remove(sessionId);
        }

        public string GetUsernameFromSessionId(HeaderManager headers)
        {
            var session = getSessionInfo(headers.AuthToken, headers);
            if (session != null)
                return session.UserName;
            else
                return null;
        }

        public static void UpdateSignalRClientAddr(urNoticeSession session,dynamic signalRClientAddr)
        {
            session.SignalRClient = signalRClientAddr;
            const int hours = 1; // TODO: currently hard coded hour value;
            setMemoryCacheValue(session.SessionId, session, hours, 0, 0);
        }

        public static urNoticeSession getLogoutSessionInfo(string sessionId)
        {
            urNoticeSession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }
            return null;
        }

        public urNoticeSession getSessionInfo(string sessionId, HeaderManager headers)
        {
            
            urNoticeSession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }

            return null;
        }

        public static urNoticeSession getSessionInfo(string sessionId)
        {
            urNoticeSession session = null;
            if (IsValidSession(sessionId, out session))
            {
                return session;
            }
            else
            {
                return null;
            }
        }
        

        public static bool IsValidSession(string sessionId)
        {
            if (sessionId == null)
                return false;

            urNoticeSession session = null;
            return IsValidSession(sessionId, out session);
        }

        public bool Logout(string sessionId)
        {
            //_db = new urnoticeAuthEntities();
            if (sessionId == null)
                return false;

            urNoticeSession session = null;
            if (MemoryCache.Default.Contains(sessionId))
            {
                //todo: logout logic need to be implemented.
                session = (urNoticeSession)MemoryCache.Default.Get(sessionId);
                
                //var user = _db.Users.SingleOrDefault(x => x.username == session.UserName);
                //user.guid = Guid.NewGuid().ToString();
                //try
                //{
                //    _db.SaveChanges();
                //}
                //catch (DbEntityValidationException e)
                //{
                //    DbContextException.LogDbContextException(e);                    
                //}

                MemoryCache.Default.Remove(sessionId);
            }
            return true;
        }

        private static bool IsValidSession(string sessionId, out  urNoticeSession session)
        {
            session = null;

            if (sessionId == null)
                return false;
            if (MemoryCache.Default.Contains(sessionId))
            {
                session = (urNoticeSession)MemoryCache.Default.Get(sessionId);                
            }
            return VerifySessionObject(session);
        }

        private static bool VerifySessionObject(urNoticeSession session)
        {
            return session != null;
        }

    }
}