using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.User
{
    public class UserNotificationGraphModel
    {
        public string _outV {get;set;}
        public string _inV {get;set;}
        public string _label {get;set;}
        public string NotificationInitiatedByVertexId {get;set;}
        public string Type { get; set; }
        public string UserName { get; set; }
        public string isParentPost { get; set; }
        public string parentPostId { get; set; }
    }
}
