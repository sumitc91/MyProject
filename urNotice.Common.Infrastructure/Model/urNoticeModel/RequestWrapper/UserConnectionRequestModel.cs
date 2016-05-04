using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class UserConnectionRequestModel
    {
        public string UserVertexId {get;set;}
        public string ConnectionType{get;set;}
        public string ConnectingBody { get; set; }

    }
}
