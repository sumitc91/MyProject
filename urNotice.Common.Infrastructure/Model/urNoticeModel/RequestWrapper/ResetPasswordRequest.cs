using System;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class ResetPasswordRequest
    {
        public String Username { get; set; }
        public String Guid { get; set; }
        public String Password { get; set; }
    }
}