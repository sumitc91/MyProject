using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostVertexModel
    {
        public string PostImage{get;set;}
        public string PostedByUser { get;set;}
        public string PostedTime {get;set;}
        public string PostMessage {get;set;}
        public string _id {get;set;}
        public string _type { get; set; }
    }
}
