using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserPostUnreadNotificationsModelV1Response
    {
        public string requestId { get; set; }
        public UserPostUnreadNotificationsModelV1ResultResponse result { get; set; }
    }
}
